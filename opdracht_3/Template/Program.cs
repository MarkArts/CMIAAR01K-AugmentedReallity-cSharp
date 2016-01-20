using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using ImageUtilities;
using System.Numerics;
using System.Diagnostics;

namespace Template
{
    public class Program
    {               

        static void Main(string[] args)
        {
            Color[,] img = ImageUtilities.ImageViewer.LoadImage(@"Content/forest.jpg");

            KMEansPixel[] pixels = colorsToPixels(img);
            KMEansPixel[] kmeans = kMean(pixels, 6, 4);

            Color[,] c = pixelsToColors(kmeans);

            ImageUtilities.ImageViewer.DrawImagePair(img, c);
        }

        public struct KMEansPixel
        {
            public Vector3 Color;
            public Vector2 Position;
            public int Group;

            public KMEansPixel(Vector3 color, Vector2 position)
            {
                Color = color;
                Position = position;
                Group = 0;
            }
        }

        static KMEansPixel[] kMean(KMEansPixel[] pixels, int clustersAmount, int itterations)
        {
            Random random = new Random(43);
            Vector3[] centroids = new Vector3[clustersAmount];

            // initialize our centroinds
            for (int i = 0; i < centroids.Length; i++)
            {
                centroids[i] = new Vector3(121, 49, 124);
            }

            // We limmit the itterations for performance reasons
            for (int i = 0; i < itterations; i++)
            {
                // assing pixels to groups
                for (int p = 0; p < pixels.GetLength(0); p++)
                {
                    int group = 0;

                    for (int c = 0; c < centroids.Length; c++) // <--- but this c# lolz
                    {
                        if (Vector3.Distance(pixels[p].Color, centroids[c]) < Vector3.Distance(pixels[p].Color, centroids[group]))
                        {
                            group = c;
                        }
                    }

                    pixels[p].Group = group;
                }

                // re calculate centroids
                // init the new centroids as zero so we can add to them
                Vector3[] newCentroids = new Vector3[centroids.Length];
                int[] newCentroidsCount = new int[newCentroids.Length]; // we keep track of the amount of pixels in each centroid so we can divide the sum of all pixels in a group with it
                for (int c = 0; c < newCentroids.Length; c++)
                {
                    newCentroids[c] = Vector3.Zero;
                    newCentroidsCount[c] = 0;
                }

                // add the pixels to their new centroid
                for (int p = 0; p < pixels.Length; p++)
                {
                    newCentroids[pixels[p].Group] = Vector3.Add(newCentroids[pixels[p].Group], pixels[p].Color);
                    newCentroidsCount[pixels[p].Group]++;
                }

                // divide the centroids with their count so we get the average
                for (int c = 0; c < newCentroids.Length; c++)
                {
                    // no elements where assignet to this centroid
                    if(newCentroidsCount[c] == 0)
                    {
                        newCentroids[c] = centroids[c];
                    }
                    else
                    {
                        newCentroids[c] = Vector3.Divide(newCentroids[c], newCentroidsCount[c]);
                    }                
                }

                centroids = newCentroids;
            }

            // we set the pixels to their centroid so we can visualy debug it
            centerPixelToCentroid(pixels, centroids);

            return pixels;
        }

        static KMEansPixel[] centerPixelToCentroid(KMEansPixel[] pixels, Vector3[] centroids)
        {
            for (int p = 0; p < pixels.Length; p++)
            {
                pixels[p].Color = centroids[pixels[p].Group];
            }

            return pixels;
        }

        static KMEansPixel[] colorsToPixels(Color[,] img)
        {
            KMEansPixel[] pixels = new KMEansPixel[img.GetLength(0) * img.GetLength(1)];

            int counter = 0; // maybe we could calculate this in the loops
            for (int x = 0; x < img.GetLength(0); x++)
            {
                for (int y = 0; y < img.GetLength(1); y++)
                {
                    pixels[counter] = new KMEansPixel(
                        new Vector3(img[x, y].R, img[x, y].G, img[x, y].B),
                        new Vector2(x, y)
                    );
                    counter++;
                }
            }

            return pixels;
        }

        static Color[,] pixelsToColors(KMEansPixel[] pixels)
        {
            // because we don't shuffle the pixels array after creation we can assume the last pixel in it is the bottom right
            Color[,] colors = new Color[(int)pixels.Last().Position.X + 1, (int)pixels.Last().Position.Y + 1];

            for (int p = 0; p < pixels.Length; p++)
            {
                KMEansPixel pixel = pixels[p]; // find out if a array lookup is cheaper then the struct copy
                colors[(int)pixel.Position.X, (int)pixel.Position.Y] = Color.FromArgb((int)pixel.Color.X, (int)pixel.Color.Y, (int)pixel.Color.Z);
            }

            return colors;
        }
    }
}
