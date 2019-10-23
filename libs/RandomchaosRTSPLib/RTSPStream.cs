using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

using FFmpeg.AutoGen;

namespace RandomchaosRTSPLib
{
    /// <summary>
    /// This class will stream an AV feed via a url
    /// </summary>
    public class RTSPStream : IDisposable
    {
        protected string streamURL = string.Empty;
        protected volatile bool isThreadRunning = false;

        /// <summary>
        /// States if the thread is running
        /// </summary>
        public bool IsThreadRunning { get { return isThreadRunning; } }

        public int FramesLost = 0;

        protected volatile IntPtr streamTexturePtr;
        protected volatile int streamTextureSize;
        protected Size texturesize;

        protected int frameWidth;
        protected int frameHeight;
        protected int streamLineSize;

        protected int threadSleep = 100;
        protected bool forceTCP = true;
        protected int tcpTimeout = 100;
        protected int maxFrameLoss = 10;

        protected int frameNumber = 0;
        /// <summary>
        /// Current frame count
        /// </summary>
        public int FrameCount { get { return frameNumber; } }

        protected bool streamReady = false;
        /// <summary>
        /// True if the stream is ready to start reading
        /// </summary>
        public bool StreamReady { get { return streamReady; } }

        protected string errMsg = string.Empty;
        /// <summary>
        /// Any error messages created by the system.
        /// </summary>
        public string ErrorMessage { get { return errMsg; } }

        /// <summary>
        /// With of the destination texture
        /// </summary>
        public int TextureWidth { get { return texturesize.Width; } }

        /// <summary>
        /// Height of the destination texture
        /// </summary>
        public int TextureHeight { get { return texturesize.Height; } }

        VideoStreamDecoder.CBFunction thisCBF;

        object lockObj = new object();

        Thread workerThread;

        /// <summary>
        /// Default ctor
        /// </summary>
        public RTSPStream() { }




        /// <summary>
        /// Method to start the threaded AV stream
        /// </summary>
        /// <param name="url">url of the given stream</param>
        /// <param name="threadSleep">How long the thread will sleep between each frame grab</param>
        /// <param name="forceTCP">If true, stream will be read over TCP rather than UDP</param>
        /// <param name="tcpTimeout">If TCP, this is the tcp message time out in milliseconds.</param>
        /// <param name="udpMaxDelay">Max UDP delay</param>
        /// <param name="connectionTimeout">Connection timeout in seconds</param>
        public virtual void StartStream(string url, int threadSleep = 100, bool forceTCP = true, int tcpTimeout = 100, int maxFrameLoss = 10, VideoStreamDecoder.CBFunction cbf = null)
        {
            frameNumber = 0;
            errMsg = string.Empty;
            FramesLost = 0;

            thisCBF = cbf;

            this.streamURL = url;
            this.threadSleep = threadSleep;
            this.forceTCP = forceTCP;
            this.tcpTimeout = tcpTimeout;
            this.maxFrameLoss = maxFrameLoss;

            workerThread = new Thread(new ThreadStart(Worker));
            workerThread.Start();
        }

        /// <summary>
        /// Method to stop the stream
        /// </summary>
        public virtual void StopStream()
        {
            isThreadRunning = false;
            if(workerThread != null)
                workerThread.Join(threadSleep);
        }

        /// <summary>
        /// Method to get a native pointer and size of the current frame texture
        /// </summary>
        /// <param name="ptr">IntPtr to the texture</param>
        /// <param name="size">Size of the texture.</param>
        public virtual void GetStreamTexture(out IntPtr ptr, out int size)
        {
            ptr = streamTexturePtr;
            size = streamTextureSize;
        }

        //public Bitmap bitMapTexture;

        public virtual unsafe Bitmap GetStreamTexture()
        {
            //lock (lockObj)
            {
                if (streamTexturePtr != IntPtr.Zero)
                {
                    Bitmap img = new Bitmap(frameWidth, frameHeight, streamLineSize, PixelFormat.Format24bppRgb, streamTexturePtr);
                    return img;
                }
                else
                    return null;
            }
        }

        public static int ThreadCount = 0;

        /// <summary>
        /// This is the worker thread.
        /// </summary>
        protected virtual unsafe void Worker()
        {
            isThreadRunning = true;
            ThreadCount++;
            try
            {
                using (VideoStreamDecoder vsd = new VideoStreamDecoder(streamURL, forceTCP, tcpTimeout, thisCBF))
                {
                    var info = vsd.GetContextInfo();
                    info.ToList().ForEach(x => Console.WriteLine($"{x.Key} = {x.Value}"));

                    Size sourceSize = vsd.FrameSize;
                    AVPixelFormat sourcePixelFormat = vsd.PixelFormat;
                    Size destinationSize = sourceSize;

                    AVPixelFormat destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_RGB565LE;

                    streamTexturePtr = IntPtr.Zero;
                    texturesize = destinationSize;
                    streamReady = true;

                    using (VideoFrameConverter vfc = new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
                    {
                        frameNumber = 0;

                        while (isThreadRunning)
                        {
                            if (vsd.TryDecodeNextFrame(out AVFrame frame))
                            {
                                AVFrame convertedFrame = vfc.Convert(frame);

                                frameWidth = convertedFrame.width;
                                frameHeight = convertedFrame.height;

                                streamTexturePtr = (IntPtr)convertedFrame.data[0];
                                streamLineSize = convertedFrame.linesize[0];
                                streamTextureSize = convertedFrame.linesize[0] * convertedFrame.height;

                                frameNumber++;
                                FramesLost = 0;
                            }
                            else
                            {
                                streamTexturePtr = IntPtr.Zero;
                                streamTextureSize = 0;

                                FramesLost++;

                                if (FramesLost > maxFrameLoss)
                                {
                                    errMsg = "Stream lost...";
                                    streamReady = true;
                                    frameNumber = -999;
                                }
                            }

                            Thread.Sleep(threadSleep);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                streamTexturePtr = IntPtr.Zero;
                streamTextureSize = 0;

                errMsg = ex.Message;
                streamReady = true;
                frameNumber = -999;
            }

            isThreadRunning = false;
            ThreadCount--;
        }

        protected virtual unsafe void Worker2()
        {
            int framesLost = 0;
            int maxFameLoss = 50;

            isThreadRunning = true;
            try
            {
                using (VideoStreamDecoder vsd = new VideoStreamDecoder(streamURL))
                {
                    var info = vsd.GetContextInfo();
                    info.ToList().ForEach(x => Console.WriteLine($"{x.Key} = {x.Value}"));

                    Size sourceSize = vsd.FrameSize;
                    AVPixelFormat sourcePixelFormat = vsd.PixelFormat;
                    Size destinationSize = sourceSize;

                    AVPixelFormat destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_RGB24;

                    streamTexturePtr = IntPtr.Zero;
                    texturesize = destinationSize;
                    streamReady = true;

                    using (VideoFrameConverter vfc = new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat))
                    {
                        frameNumber = 0;

                        while (isThreadRunning)
                        {
                            if (vsd.TryDecodeNextFrame(out AVFrame frame))
                            {
                                AVFrame convertedFrame = vfc.Convert(frame);

                                streamTexturePtr = (IntPtr)convertedFrame.data[0];
                                streamTextureSize = convertedFrame.linesize[0] * convertedFrame.height;

                                frameNumber++;
                                framesLost = 0;
                            }
                            else
                            {
                                streamTexturePtr = IntPtr.Zero;
                                streamTextureSize = 0;

                                framesLost++;

                                if (framesLost > maxFameLoss)
                                {
                                    errMsg = "Stream lost...";
                                    streamReady = true;
                                    frameNumber = -999;
                                }
                            }

                            Thread.Sleep(threadSleep);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                streamTexturePtr = IntPtr.Zero;
                streamTextureSize = 0;

                errMsg = ex.Message;
                streamReady = true;
                frameNumber = -999;
            }

            isThreadRunning = false;
        }

        /// <summary>
        /// Method used to clear up when disposed
        /// </summary>
        public virtual void Dispose()
        {
            isThreadRunning = false;
        }
    }
}
