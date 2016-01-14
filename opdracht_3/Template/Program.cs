using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using ImageUtilities;
using System.Numerics;

namespace Template
{
    public class Program
    {                
        public class Image : Dictionary<Tuple<int, int>, Vector3>
        {
            public static Image fromColors(Color[,] img)
            {
                Image ret = new Image();

                MultiEach(img, (c, x, y) =>
                {
                    ret[Tuple.Create(x, y)] = new Vector3(c.R, c.G, c.B);
                });

                return ret;
            }

            public static Image fromClusters(Cluster[] clusters)
            {
                Image pixels = new Image();
                Each(clusters, cluster =>
                {
                    foreach (KeyValuePair<Tuple<int, int>, Vector3> pixel in cluster)
                    {
                        if(!pixels.ContainsKey(pixel.Key))
                            pixels.Add(pixel.Key, pixel.Value);
                    }
                });

                return pixels;
            }

            public Color[,] toColors()
            {            
                Tuple<int, int> res = this.res();
                Color[,] img = new Color[res.Item1 +1 , res.Item2 +1];

                foreach (KeyValuePair<Tuple<int, int>, Vector3> pixel in this)
                {
                    img[pixel.Key.Item1, pixel.Key.Item2] = Color.FromArgb((int)pixel.Value.X, (int)pixel.Value.Y, (int)pixel.Value.Z);
                }

                return img;
            }

            public Tuple<int, int> res()
            {
                int x = 0;
                int y = 0;

                foreach (KeyValuePair<Tuple<int, int>, Vector3> pixel in this)
                {
                    if (pixel.Key.Item1 > x)
                        x = pixel.Key.Item1;

                    if (pixel.Key.Item2 > y)
                        y = pixel.Key.Item1;
                }

                return Tuple.Create(x, y);
            }
        };

        public class Cluster : Image
        {
            public Vector3 centroid;

            public Cluster() : base()
            {
                centroid = Vector3.Zero;
            }

            public void calcCentroid()
            {
                Vector3 sum = Vector3.Zero;

                foreach (KeyValuePair<Tuple<int, int>, Vector3> pixel in this)
                {
                    sum = Vector3.Add(sum, pixel.Value);
                }

                this.centroid = Vector3.Divide(sum, this.Count());
            }

            public Cluster setAllToCentroid()
            {
                Cluster c = new Cluster();
                foreach (KeyValuePair<Tuple<int, int>, Vector3> pixel in this)
                {
                    c[pixel.Key] = this.centroid;
                }

                return c;
            }
        };

        static void Main(string[] args)
        {
            Color[,] img = ImageUtilities.ImageViewer.LoadImage(@"E:\Documents\Visual Studio 2015\Projects\CMIAAR01K-AugmentedReallity-cSharp\opdracht_2\Content\Content\forest.jpg");

            Cluster[] clusters = kMean(Image.fromColors(img), 2, 2);
            Image kmeanImage = Image.fromClusters(Map(clusters, c => c.setAllToCentroid()));

            Cluster[] clusters2 = kMean(Image.fromColors(img), 8, 4);
            Image kmeanImage2 = Image.fromClusters(Map(clusters2, c => c.setAllToCentroid()));

            ImageUtilities.ImageViewer.DrawImagePair(kmeanImage.toColors(), kmeanImage2.toColors());
        }

        static Cluster[] kMean(Image img, int clustersAmount, int itterations)
        {
            Random random = new Random();
            Cluster[] clusters = new Cluster[clustersAmount];
            clusters = Map(clusters, x => new Cluster() );

            // generate random clusters
            Each(clusters, x => { x.centroid = new Vector3(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)); } );

            for (int i = 0; i < itterations; i++)
            {
                foreach (KeyValuePair<Tuple<int, int>, Vector3> pixel in img)
                {
                    closestCluster(clusters, pixel.Value)[pixel.Key] = pixel.Value;
                }

                Each(clusters, x => { x.calcCentroid(); });
            }

            return clusters;
        }

        static Cluster closestCluster(Cluster[] clusters, Vector3 pixel) {
            Cluster ret = (Cluster)clusters.GetValue(0);
            float closestDistance = Vector3.Distance(pixel, ret.centroid);

            for (int i = 0; i < clusters.GetLength(0); i++)
            {
                float distance = Vector3.Distance(pixel, clusters[i].centroid);
                if (distance < closestDistance)
                {
                    ret = clusters[i];
                    closestDistance = distance;
                }
            }

            return ret;
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

        static T[] Map<T>(T[] coll, Func<T, T> f)
        {
            T[] ret = new T[coll.GetLength(0)];
            for (int x = 0; x < coll.GetLength(0); x += 1)
            {
                ret[x] = f(coll[x]);
            }

            return ret;
        }

        static void Each<T>(T[] coll, Action<T> f)
        {
            for (int x = 0; x < coll.GetLength(0); x += 1)
            {
                f(coll[x]);
            }
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

        static void MultiEach<T>(T[,] colors, Action<T, int, int> f)
        {
            for (int x = 0; x < colors.GetLength(0); x += 1)
            {
                for (int y = 0; y < colors.GetLength(1); y += 1)
                {
                    f(colors[x, y], x, y);
                }
            }
        }

    }
}
