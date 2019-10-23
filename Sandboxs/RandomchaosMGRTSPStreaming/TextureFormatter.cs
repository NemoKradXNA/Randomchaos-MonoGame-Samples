using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

using Microsoft.Xna.Framework.Graphics;


namespace RandomchaosMGRTSPStreaming
{
    public static class TextureFormatter
    {
        public static void GetTexture2DFromBitmap(GraphicsDevice device, Bitmap bitmap, ref Texture2D tex)
        {
            if (bitmap != null)
            {
                if (tex == null)
                    tex = new Texture2D(device, bitmap.Width, bitmap.Height, false, SurfaceFormat.Bgr565);

                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

                int bufferSize = data.Height * data.Stride;

                //create data buffer 
                byte[] bytes = new byte[bufferSize];

                // copy bitmap data into buffer
                Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

                // copy our buffer to the texture
                tex.SetData(bytes);
                
                // unlock the bitmap data
                bitmap.UnlockBits(data);
            }
        }        
    }
}
