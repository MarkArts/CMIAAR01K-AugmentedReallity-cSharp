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
    public class Program
    {        

        public enum Method
        {
            Nearest,
            Bilinear
        }

        public struct Resultion
        {
            public int x, y;
            public Resultion(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        static void Main(string[] args)
        {
            Color[,] img = ImageUtilities.ImageViewer.LoadImage(@"E:\Documents\Visual Studio 2015\Projects\CMIAAR01K-AugmentedReallity-cSharp\opdracht_2\Content\Content\minion.jpg");

            Color[,] nearImg = Resize(img, new Resultion(1920*2, 1080*2), Method.Nearest);
            Color[,] BilImg = Resize(img, new Resultion(1920*2, 1080*2), Method.Bilinear);
            ImageUtilities.ImageViewer.DrawImagePair(nearImg, BilImg);
        }

        static Color[,] Resize(Color[,] img, Resultion res, Method method)
        {
            Color[,] resImg = new Color[res.x, res.y];

            float xScale = (float)img.GetLength(0) / (float)res.x;
            float yScale = (float)img.GetLength(1) / (float)res.y;

            int boundX = img.GetLength(0) - 1;
            int boundY = img.GetLength(1) - 1;

            if (method == Method.Nearest)
            {
                resImg = MultiMap(resImg, (c, x, y) => img[(int)clamp((float)Math.Round(xScale * x), 0, boundX), (int)clamp((float)Math.Round(yScale * y), 0, boundY)]);
            }else if(method == Method.Bilinear)
            {
                resImg = MultiMap(resImg, (c, x, y) =>
                {
                    float nX = xScale * x;
                    float nY = yScale * y;

                    float factorX = nX - (float)Math.Truncate(nX);
                    float factorY = nY - (float)Math.Truncate(nY);

                    Color t1 = img[(int)clamp((float)Math.Floor(nX),0, boundX), (int)clamp((float)Math.Floor(nY),0,boundY)];
                    Color t2 = img[(int)clamp((float)Math.Ceiling(nX), 0, boundX), (int)clamp((float)Math.Floor(nY), 0, boundY)];
                    Color b1 = img[(int)clamp((float)Math.Floor(nX), 0, boundX), (int)clamp((float)Math.Ceiling(nY), 0, boundY)];
                    Color b2 = img[(int)clamp((float)Math.Ceiling(nX), 0, boundX), (int)clamp((float)Math.Ceiling(nY), 0, boundY)];

                    return Color.FromArgb(
                        (int)lerp(lerp(t1.R, t2.R, factorX), lerp(b1.R, b2.R, factorX), factorY),
                        (int)lerp(lerp(t1.G, t2.G, factorX), lerp(b1.G, b2.G, factorX), factorY),
                        (int)lerp(lerp(t1.B, t2.B, factorX), lerp(b1.B, b2.B, factorX), factorY)
                    );
                });
            }

            return resImg;
        }


        static float lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }
        static float clamp(float val, float min, float max)
        {
            if (val < min) return min;
            if (val > max) return max;
            return val;
        }

        static R[,] MultiMap<T, R>(T[,] colors, Func<T, int, int, R> f)
        {
            R[,] ret = new R[colors.GetLength(0), colors.GetLength(1)];
            for (int x = 0; x < colors.GetLength(0); x += 1)
            {
                for (int y = 0; y < colors.GetLength(1); y += 1)
                {
                    ret[x, y] = f(colors[x, y], x, y );
                }
            }

            return ret;
        }

    }
}
