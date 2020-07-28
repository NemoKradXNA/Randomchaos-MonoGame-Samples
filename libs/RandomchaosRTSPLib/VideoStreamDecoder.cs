using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

using FFmpeg.AutoGen;


namespace RandomchaosRTSPLib
{
    public sealed unsafe class VideoStreamDecoder : IDisposable
    {
        private AVCodecContext* _pCodecContext;
        private AVFormatContext* _pFormatContext;
        private int _streamIndex;
        private AVFrame* _pFrame;
        private AVPacket* _pPacket;

        public delegate int CBFunction();

        public VideoStreamDecoder(string url, bool forceTCP = false, int tcpTimeout = 100, CBFunction callBack = null)
        {
            _pFormatContext = ffmpeg.avformat_alloc_context();
            var pFormatContext = _pFormatContext;


            AVDictionary* dic = null;

            pFormatContext->max_delay = 0;

            ///https://ffmpeg.org/ffmpeg-protocols.html
            if (forceTCP)
            {
                ffmpeg.av_dict_set(&dic, "rtsp_transport", "tcp", 0);
                ffmpeg.av_dict_set(&dic, "stimeout", $"{tcpTimeout}", 0);
            }
            else
            {
                pFormatContext->max_delay = 0;
            }


            if (callBack != null)
                _pFormatContext->interrupt_callback = new AVIOInterruptCB() { callback = new AVIOInterruptCB_callback_func() { Pointer = Marshal.GetFunctionPointerForDelegate(callBack) }, opaque = pFormatContext };

            ffmpeg.avformat_open_input(&pFormatContext, url, null, &dic).ThrowExceptionIfError();
            ffmpeg.avformat_find_stream_info(_pFormatContext, null).ThrowExceptionIfError();

            // find the first video stream
            AVStream* pStream = null;
            for (var i = 0; i < _pFormatContext->nb_streams; i++)
                if (_pFormatContext->streams[i]->codec->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
                {
                    pStream = _pFormatContext->streams[i];
                    break;
                }

            if (pStream == null) throw new InvalidOperationException("Could not found video stream.");

            _streamIndex = pStream->index;
            _pCodecContext = pStream->codec;

            var codecId = _pCodecContext->codec_id;
            var pCodec = ffmpeg.avcodec_find_decoder(codecId);
            if (pCodec == null) throw new InvalidOperationException("Unsupported codec.");

            ffmpeg.avcodec_open2(_pCodecContext, pCodec, &dic).ThrowExceptionIfError();

            CodecName = ffmpeg.avcodec_get_name(codecId);
            FrameSize = new Size(_pCodecContext->width, _pCodecContext->height);
            PixelFormat = _pCodecContext->pix_fmt;

            _pPacket = ffmpeg.av_packet_alloc();
            _pFrame = ffmpeg.av_frame_alloc();
        }


        public string CodecName { get; }
        public Size FrameSize { get; }
        public AVPixelFormat PixelFormat { get; }

        public void Dispose()
        {
            ffmpeg.av_frame_unref(_pFrame);
            ffmpeg.av_free(_pFrame);

            ffmpeg.av_packet_unref(_pPacket);
            ffmpeg.av_free(_pPacket);

            ffmpeg.avcodec_close(_pCodecContext);

            var pFormatContext = _pFormatContext;
            ffmpeg.avformat_close_input(&pFormatContext);
        }

        public bool TryDecodeNextFrame(out AVFrame frame)
        {
            ffmpeg.av_frame_unref(_pFrame);
            int error;
            do
            {
                do
                {
                    ffmpeg.av_packet_unref(_pPacket);

                    error = ffmpeg.av_read_frame(_pFormatContext, _pPacket);
                    if (error == ffmpeg.AVERROR_EOF)
                    {
                        frame = *_pFrame;
                        return false;
                    }

                    error.ThrowExceptionIfError();
                } while (_pPacket->stream_index != _streamIndex);

                ffmpeg.avcodec_send_packet(_pCodecContext, _pPacket).ThrowExceptionIfError();

                error = ffmpeg.avcodec_receive_frame(_pCodecContext, _pFrame);
            } while (error == ffmpeg.AVERROR(ffmpeg.EAGAIN));

            error.ThrowExceptionIfError();
            frame = *_pFrame;
            return true;
        }



        public IReadOnlyDictionary<string, string> GetContextInfo()
        {
            AVDictionaryEntry* tag = null;
            var result = new Dictionary<string, string>();
            while ((tag = ffmpeg.av_dict_get(_pFormatContext->metadata, "", tag, ffmpeg.AV_DICT_IGNORE_SUFFIX)) != null)
            {
                var key = Marshal.PtrToStringAnsi((IntPtr)tag->key);
                var value = Marshal.PtrToStringAnsi((IntPtr)tag->value);
                result.Add(key, value);
            }

            return result;
        }
    }
}
