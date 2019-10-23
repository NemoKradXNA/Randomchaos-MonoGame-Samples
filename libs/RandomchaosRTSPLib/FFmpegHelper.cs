using System;
using System.Runtime.InteropServices;

using FFmpeg.AutoGen;

namespace RandomchaosRTSPLib
{
    /// <summary>
    /// This static class is used to support the VideoFrameConverter and VideoStreamDecoder
    /// </summary>
    internal static class FFmpegHelper
    {
        /// <summary>
        /// Converts a number to an error string
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static unsafe string av_strerror(int error)
        {
            var bufferSize = 1024;
            var buffer = stackalloc byte[bufferSize];
            ffmpeg.av_strerror(error, buffer, (ulong)bufferSize);
            var message = Marshal.PtrToStringAnsi((IntPtr)buffer);
            return message;
        }

        /// <summary>
        /// Extension method to throw an exception if error < 0
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static int ThrowExceptionIfError(this int error)
        {
            if (error < 0) throw new ApplicationException(av_strerror(error));
            return error;
        }
    }
}
