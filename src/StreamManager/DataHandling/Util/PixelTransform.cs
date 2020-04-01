using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Golem2.Manager.DataHandling.Util
{
    public static class PixelTransform
    {
        public static Color InterpolateColor(Color start, Color stop, float time)
        {
            double r = ((1 - time) * Convert.ToDouble(start.R)) + (time * Convert.ToDouble(stop.R));
            double g = ((1 - time) * Convert.ToDouble(start.G)) + (time * Convert.ToDouble(stop.G));
            double b = ((1 - time) * Convert.ToDouble(start.B)) + (time * Convert.ToDouble(stop.B));

            Color nC = Color.FromArgb(Convert.ToInt32(r), Convert.ToInt32(g), Convert.ToInt32(b));

            return nC;
        }

        public static Color[,] Interpolate(Color[,] img, int width, int height)
        {
            Color[,] nMatrix = InterpolateWidth(img, width);
            nMatrix = InterpolateHeight(nMatrix, height);

            return nMatrix;
        }
        public static Color[,] InterpolateWidth(Color[,] img, int width)
        {
            if (width == img.GetLength(0))
                return img;

            Color[,] nMatrix = new Color[width, img.GetLength(1)];

            for (int j = 0; j < img.GetLength(1); j++)
            {
                Point lastIndex = new Point(0, 0);
                for (int i = 0; i < img.GetLength(0); i++)
                {
                    double percentX = (double)i / (double)img.GetLength(0);

                    int nX = (int)Math.Round(((double)nMatrix.GetLength(0) * (double)percentX));
                    if (nX >= nMatrix.GetLength(0))
                        nX = nMatrix.GetLength(0) - 1;

                    nMatrix[nX, j] = img[i, j];

                    for (int tX = lastIndex.X + 1; tX < nX; tX++)
                    {
                        float time = (float)tX / (float)nX;
                        nMatrix[tX, j] = PixelTransform.InterpolateColor(nMatrix[lastIndex.X, lastIndex.Y], nMatrix[nX, j], time);
                    }
                    lastIndex = new Point(nX, j);

                }

            }
            return nMatrix;
        }
        public static Color[,] InterpolateHeight(Color[,] img, int height)
        {
            if (height == img.GetLength(1))
                return img;

            Color[,] nMatrix = new Color[img.GetLength(0), height];

            for (int i = 0; i < img.GetLength(0); i++)
            {
                Point lastIndex = new Point(0, 0);
                for (int j = 0; j < img.GetLength(1); j++)
                {
                    double percentY = (double)j / (double)img.GetLength(1);

                    int nY = (int)Math.Round(((double)nMatrix.GetLength(1) * (double)percentY));
                    if (nY >= nMatrix.GetLength(1))
                        nY = nMatrix.GetLength(1) - 1;
                    nMatrix[i, nY] = img[i, j];

                    for (int tY = lastIndex.Y + 1; tY < nY; tY++)
                    {
                        float time = (float)tY / (float)nY;
                        nMatrix[i, tY] = PixelTransform.InterpolateColor(nMatrix[lastIndex.X, lastIndex.Y], nMatrix[i, nY], time);
                    }
                    lastIndex = new Point(i, nY);

                }
            }

            return nMatrix;

        }

        public static Color[,] LoadBitmap(Image img)
        {
            return LoadBitmap((Bitmap)img);
        }
        public static Color[,] LoadBitmap(Bitmap bmp)
        {
            return uLoadBitmap(bmp);
        }
        public static Bitmap SaveBitmap(Color[,] pixels)
        {
            return uCreateBitmap(pixels);
        }

        public static Bitmap GetLesserBitmap(Bitmap bmp1, Bitmap bmp2)
        {
            int s1 = bmp1.Width;
            int v1 = bmp1.Height;

            int s2 = bmp2.Width;
            int v2 = bmp2.Height;

            double p1 = Math.Sqrt(s1 * s1 + v1 * v1);
            double p2 = Math.Sqrt(s2 * s2 + v2 * v2);

            if (p1 <= p2)
                return bmp1;
            else
                return bmp2;


        }
        public static Color[,] InversePixels(Color[,] pixels)
        {
            for (int i = 0; i < pixels.GetLength(0); i++)
            {
                for (int j = 0; j < pixels.GetLength(1); j++)
                {
                    int nR = 255 - pixels[i, j].R;
                    int nG = 255 - pixels[i, j].G;
                    int nB = 255 - pixels[i, j].B;

                    pixels[i, j] = Color.FromArgb(nR, nG, nB);
                }
            }

            return pixels;
        }
        public static Color[,] MoveWithPixels(Color[,] pixels, int x, int y)
        {
            Color[,] npixels = new Color[pixels.GetLength(0), pixels.GetLength(1)];
            if (x > pixels.GetLength(0))
                return npixels;

            if (y > pixels.GetLength(1))
                return npixels;

            if (x >= 0)
            {
                for (int i = pixels.GetLength(0) - 1; i - x >= 0; i--)
                {
                    if (y >= 0)
                    {
                        for (int j = pixels.GetLength(1) - 1; j - y >= 0; j--)
                        {
                            npixels[i, j] = pixels[i - x, j - y];
                        }
                    }
                    else
                    {
                        for (int j = -y; j < pixels.GetLength(1); j++)
                        {
                            npixels[i, j + y] = pixels[i - x, j];
                        }
                    }
                }
            }
            else
            {
                for (int i = -x; i < pixels.GetLength(0); i++)
                {
                    if (y >= 0)
                    {
                        for (int j = pixels.GetLength(1) - 1; j - y >= 0; j--)
                        {
                            npixels[i+x, j] = pixels[i, j - y];
                        }
                    }
                    else
                    {
                        for (int j = -y; j < pixels.GetLength(1); j++)
                        {
                            npixels[i+x, j + y] = pixels[i, j];
                        }
                    }
                }
            }

            return npixels;
        }
        public static Color[,] AddPixels(Color[,] pixels1, Color[,] pixels2)
        {
            if (pixels1.GetLength(0) != pixels2.GetLength(0))
                return null;

            if (pixels1.GetLength(1) != pixels2.GetLength(1))
                return null;

            Color[,] nPixels = new Color[pixels1.GetLength(0), pixels1.GetLength(1)];

            for (int i = 0; i < pixels1.GetLength(0); i++)
            {
                for (int j = 0; j < pixels1.GetLength(1); j++)
                {
                    int r1 = pixels1[i, j].R;
                    int r2 = pixels2[i, j].R;

                    int g1 = pixels1[i, j].G;
                    int g2 = pixels2[i, j].G;

                    int b1 = pixels1[i, j].B;
                    int b2 = pixels2[i, j].B;

                    int nR = r1 + r2;
                    int nG = g1 + g2;
                    int nB = b1 + b2;

                    if (nR > 255) nR = 255;
                    if (nG > 255) nG = 255;
                    if (nB > 255) nB = 255;

                    nPixels[i, j] = Color.FromArgb(nR, nG, nB);
                }
            }

            return nPixels;
        }

        private unsafe static Color[,] uLoadBitmap(Bitmap bmp)
        {
            Color[,] img = new Color[bmp.Width, bmp.Height];
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            for (int y = 0; y < data.Height; y++)
            {
                // vypocte ukazatel na zacatek y-teho radku
                int* retPos = (int*)((int)data.Scan0 + (y * data.Stride));

                int x = 0;
                while (x < data.Width)
                {
                    // vyplni pixel nahodnou barvou
                    img[x, y] = Color.FromArgb(*retPos);

                    // posun na dalsi pixel
                    retPos++; x++;
                }
            }
            bmp.UnlockBits(data);

            return img;
        }
        private unsafe static Bitmap uCreateBitmap(Color[,] pixels)
        {
            Bitmap bmp = new Bitmap(pixels.GetLength(0), pixels.GetLength(1));
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            for (int y = 0; y < data.Height; y++)
            {
                // vypocte ukazatel na zacatek y-teho radku
                int* retPos = (int*)((int)data.Scan0 + (y * data.Stride));

                int x = 0;
                while (x < data.Width)
                {
                    // vyplni pixel nahodnou barvou
                    *retPos = pixels[x, y].ToArgb();

                    // posun na dalsi pixel
                    retPos++; x++;
                }
            }
            bmp.UnlockBits(data);

            return bmp;
        }
    }
}
