using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using ImageUtilities;

namespace Template
{

    public struct YUV
    {
        public float Y, U, V;

        public YUV(float y, float u, float v)
        {
            this.Y = y;
            this.U = u;
            this.V = v;
        }

        public static YUV fromRGB(Color color)
        {
            float Wr = 0.299f;
            float Wb = 0.114f;
            float Wg = 1 - Wr - Wb;
            float UMax = 0.436f;
            float VMax = 0.615f;

            float y = Wr * color.R + Wg * color.G + Wb * color.B;

            return new YUV(
                y, 
                UMax * ( (color.B - y) / ( 1 - Wb) ),
                VMax * ( (color.R - y) / (1 - Wb) )
            );
        }

        public Color toRGBA()
        {
            float Wr = 0.299f;
            float Wb = 0.114f;
            float Wg = 1 - Wr - Wb;
            float UMax = 0.436f;
            float VMax = 0.615f;

            return Color.FromArgb(
                (byte) (Y + V * ((1 - Wr) / VMax) ),
                (byte) (Y - U * ((Wb * (1 - Wb)) / (UMax * Wg)) ),
                (byte) (Y + U * ((1 - Wb) / UMax) )
            );
        }
    }


    public class Program
    {        
        static void Main(string[] args)
        {
            Color[,] img = ImageUtilities.ImageViewer.LoadImage(@"Content\forest.jpg");

            Color[,] greyImg = MultiMap(img, c =>
            {
                int m = (c.R + c.G + c.B) / 3;
                return Color.FromArgb(c.A, m, m, m);
            });

            YUV[,] yuvImg = MultiMap(img, YUV.fromRGB);          
            YUV[,] lum = MultiMap(yuvImg, c => new YUV(c.Y, 0, 0) );
            Color[,] lumRGB = MultiMap(lum, c => c.toRGBA());

            // ImageUtilities.ImageViewer.DrawImagePair(greyImg, img);
            // ImageUtilities.ImageViewer.DrawImagePair(MultiMap(yuvImg, c => c.toRGBA()), img);
            // ImageUtilities.ImageViewer.DrawImagePair(lumRGB, img);
            ImageUtilities.ImageViewer.DrawImagePair(lumRGB, greyImg);
        }

        static R[,] MultiMap<T, R>(T[,] colors, Func<T, R> f)
        {
            R[,] ret = new R[colors.GetLength(0), colors.GetLength(1)];
            for (int x = 0; x < colors.GetLength(0); x += 1)
            {
                for (int y = 0; y < colors.GetLength(1); y += 1)
                {
                    ret[x, y] = f(colors[x, y]);
                }
            }

            return ret;
        }

    }
}
