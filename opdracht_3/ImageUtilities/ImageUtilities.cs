using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ImageUtilities
{
  public class ImageViewer
  {
    private static Bitmap CreateBitmapFromColors(Color[,] image)
    {
      Bitmap bitmap = new Bitmap(image.GetLength(0), image.GetLength(1));
      for (int i = 0; i < image.GetLength(0); i++)
      {
        for (int j = 0; j < image.GetLength(1); j++)
        {
          bitmap.SetPixel(i, j, image[i, j]);
        }
      }
      return bitmap;
    }

    /// <summary>
    /// Load an image from file and returns a matrix of Color
    /// </summary>
    /// <param name="path">File path relative to the executable</param>
    public static Color[,] LoadImage(string path)
    {
      Bitmap img = new Bitmap(path);
      Color[,] colors = new Color[img.Width, img.Height];
      for (int i = 0; i < img.Width; i++)
      {
        for (int j = 0; j < img.Height; j++)
        {
          colors[i, j] = img.GetPixel(i, j);
        }
      }
      return colors;
    }

    /// <summary>
    /// Draw a single image in a window
    /// </summary>
    /// <param name="inputImage">Color matrix representing the input image</param>
    public static void DrawImage(Color[,] inputImage)
    {
      int sw = Screen.PrimaryScreen.WorkingArea.Width;
      int sh = Screen.PrimaryScreen.WorkingArea.Height;
      Form imageWindow = new Form();
      Bitmap inputBitmap = CreateBitmapFromColors(inputImage);
      PictureBox inputPicture = new PictureBox();
      inputPicture.Image = inputBitmap;
      inputPicture.Width = inputBitmap.Width;
      inputPicture.Height = inputBitmap.Height;
      inputPicture.SizeMode = PictureBoxSizeMode.Zoom;
      if (inputBitmap.Width > sw || inputBitmap.Height > sh)
      {
        int scaleFactor = (int)Math.Ceiling((float)inputBitmap.Width / (float)sw);
        inputPicture.Size = new Size(inputBitmap.Width / scaleFactor, inputBitmap.Height / scaleFactor);
      }
      imageWindow.Width = inputPicture.Width;
      imageWindow.Height = inputPicture.Height;
      imageWindow.Controls.Add(inputPicture);
      imageWindow.ShowDialog();
    }

    /// <summary>
    /// Draw a pair of images in a window. The images are scaled down if their size is too big.
    /// </summary>
    /// <param name="inputImage">Color matrix representing the input image</param>
    /// <param name="outputImage">Color matrix representing the output image</param>
    public static void DrawImagePair(Color[,] inputImage, Color[,] outputImage)
    {
      int sw = Screen.PrimaryScreen.WorkingArea.Width;
      int sh = Screen.PrimaryScreen.WorkingArea.Height;
      Form imageWindow = new Form();
      Bitmap inputBitmap = CreateBitmapFromColors(inputImage);
      Bitmap outputBitmap = CreateBitmapFromColors(outputImage);
      PictureBox inputPicture = new PictureBox();
      PictureBox outputPicture = new PictureBox();
      inputPicture.Width = inputBitmap.Width;
      inputPicture.Height = inputBitmap.Height;
      outputPicture.Width = outputBitmap.Width;
      outputPicture.Height = outputBitmap.Height;
      inputPicture.Image = inputBitmap;
      outputPicture.Image = outputBitmap;
      inputPicture.SizeMode = outputPicture.SizeMode = PictureBoxSizeMode.Zoom;
      if (inputBitmap.Width + outputBitmap.Width > sw || Math.Max(inputBitmap.Height, outputBitmap.Height) > sh)
      {
        int scaleFactor = (int)Math.Ceiling((float)(inputBitmap.Width + outputBitmap.Width) / (float)sw);
        inputPicture.Size = new Size(inputBitmap.Width / scaleFactor, inputBitmap.Height / scaleFactor);
        outputPicture.Size = new Size(outputBitmap.Width / scaleFactor, outputBitmap.Height / scaleFactor);
      }
      imageWindow.Width = inputPicture.Width + outputPicture.Width;
      imageWindow.Height = Math.Max(inputPicture.Height, outputPicture.Height);
      outputPicture.Location = new Point(inputPicture.Width, 0);
      imageWindow.Controls.Add(inputPicture);
      imageWindow.Controls.Add(outputPicture);
      imageWindow.ShowDialog();
    }
  }
}
