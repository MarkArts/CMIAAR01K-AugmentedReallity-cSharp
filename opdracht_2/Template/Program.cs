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
            Color[,] img = ImageUtilities.ImageViewer.LoadImage(@"C:\Users\Mark\Documents\CMIAAR01K\opdracht_1\Content\Content\forest.jpg");

            Color[,] resImg = Resize(img, new Resultion(1920, 1080));
            ImageUtilities.ImageViewer.DrawImagePair(resImg, img);
        }

        static Color[,] Resize(Color[,] img, Resultion res)
        {
            Color[,] resImg = new Color[res.x, res.y];

            float xScale = (float)img.GetLength(0) / (float)res.x;
            float yScale = (float)img.GetLength(1) / (float)res.y;            

            resImg = MultiMap(resImg, (c, x, y) => img[(int)Math.Round(xScale * x), (int)Math.Round(yScale * y)] );

            return resImg;
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
