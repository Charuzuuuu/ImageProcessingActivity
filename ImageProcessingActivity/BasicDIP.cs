﻿using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageProcessingActivity
{ 

    public class ConvMatrix
    {
        public int TopLeft = 0, TopMid = 0, TopRight = 0;
        public int MidLeft = 0, Pixel = 1, MidRight = 0;
        public int BottomLeft = 0, BottomMid = 0, BottomRight = 0;
        public int Factor = 1;
        public int Offset = 0;

        public void SetAll(int nVal)
        {
            TopLeft = TopMid = TopRight = MidLeft = Pixel = MidRight =
                      BottomLeft = BottomMid = BottomRight = nVal;
        }
    }
    static class BasicDIP
    {
        public static void Hist(Bitmap inputBitmap, ref Bitmap histogramBitmap)
        {
            // Create a new grayscale bitmap
            Bitmap grayBitmap = new Bitmap(inputBitmap.Width, inputBitmap.Height);
            Color sample;
            byte graydata;

            // Grayscale Conversion
            for (int x = 0; x < inputBitmap.Width; x++)
            {
                for (int y = 0; y < inputBitmap.Height; y++)
                {
                    sample = inputBitmap.GetPixel(x, y);
                    graydata = (byte)((sample.R + sample.G + sample.B) / 3);
                    grayBitmap.SetPixel(x, y, Color.FromArgb(graydata, graydata, graydata));
                }
            }

            int[] histdata = new int[256];

            // Histogram Calculation
            for (int x = 0; x < grayBitmap.Width; x++)
            {
                for (int y = 0; y < grayBitmap.Height; y++)
                {
                    sample = grayBitmap.GetPixel(x, y);
                    histdata[sample.R]++; // Use R since the image is now grayscale
                }
            }

            // Initialize the histogram bitmap
            histogramBitmap = new Bitmap(256, 800);
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 800; y++)
                {
                    histogramBitmap.SetPixel(x, y, Color.White);
                }
            }

            // Plot histogram data
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < Math.Min(histdata[x] / 5, histogramBitmap.Height - 1); y++)
                {
                    histogramBitmap.SetPixel(x, (histogramBitmap.Height - 1) - y, Color.Black);
                }
            }
        }

        public static Bitmap Conv3x3(Bitmap b, ConvMatrix m)
        {
            // Avoid divide by zero errors 

            if (0 == m.Factor || b == null)
                return null;


            //Uncomment below to check for matrix
            //Debug.WriteLine("Matrix: " + "\n" +
            //        m.TopLeft + "\t" + m.TopMid + "\t" + m.TopRight + "\n" +
            //        m.MidLeft + "\t" + m.Pixel + "\t" + m.MidRight + "\n" +
            //        m.BottomLeft + "\t" + m.BottomMid + "\t" + m.BottomRight + "\n" +
            //        "/" + m.Factor + " + " + m.Offset);


            // GDI+ still lies to us - the return format is BGR, NOT RGB.  

            Bitmap bSrc = (Bitmap)b.Clone();
            Bitmap result = new Bitmap(bSrc.Width, bSrc.Height);

            //edited bitmap
            BitmapData bmData = result.LockBits(new Rectangle(0, 0, result.Width, result.Height),
                                ImageLockMode.ReadWrite,
                                PixelFormat.Format24bppRgb);
            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height),
                               ImageLockMode.ReadWrite,
                               PixelFormat.Format24bppRgb);
            int stride = bmData.Stride;
            int stride2 = stride * 2;

            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr SrcScan0 = bmSrc.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pSrc = (byte*)(void*)SrcScan0;
                int nOffset = stride - b.Width * 3;
                int nWidth = b.Width - 2;
                int nHeight = b.Height - 2;

                int nPixel;

                for (int y = 0; y < nHeight; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {
                        nPixel = ((((pSrc[2] * m.TopLeft) +
                            (pSrc[5] * m.TopMid) +
                            (pSrc[8] * m.TopRight) +
                            (pSrc[2 + stride] * m.MidLeft) +
                            (pSrc[5 + stride] * m.Pixel) +
                            (pSrc[8 + stride] * m.MidRight) +
                            (pSrc[2 + stride2] * m.BottomLeft) +
                            (pSrc[5 + stride2] * m.BottomMid) +
                            (pSrc[8 + stride2] * m.BottomRight))
                            / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;
                        p[5 + stride] = (byte)nPixel;

                        nPixel = ((((pSrc[1] * m.TopLeft) +
                            (pSrc[4] * m.TopMid) +
                            (pSrc[7] * m.TopRight) +
                            (pSrc[1 + stride] * m.MidLeft) +
                            (pSrc[4 + stride] * m.Pixel) +
                            (pSrc[7 + stride] * m.MidRight) +
                            (pSrc[1 + stride2] * m.BottomLeft) +
                            (pSrc[4 + stride2] * m.BottomMid) +
                            (pSrc[7 + stride2] * m.BottomRight))
                            / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;
                        p[4 + stride] = (byte)nPixel;

                        nPixel = ((((pSrc[0] * m.TopLeft) +
                                       (pSrc[3] * m.TopMid) +
                                       (pSrc[6] * m.TopRight) +
                                       (pSrc[0 + stride] * m.MidLeft) +
                                       (pSrc[3 + stride] * m.Pixel) +
                                       (pSrc[6 + stride] * m.MidRight) +
                                       (pSrc[0 + stride2] * m.BottomLeft) +
                                       (pSrc[3 + stride2] * m.BottomMid) +
                                       (pSrc[6 + stride2] * m.BottomRight))
                            / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;
                        p[3 + stride] = (byte)nPixel;
                        p += 3;
                        pSrc += 3;
                    }

                    p += nOffset;
                    pSrc += nOffset;
                }
            }
            result.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);
            return result;

        }

        public static void Smooth(Bitmap reference, ref Bitmap result, int nWeight = 1)
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(1);
            m.Pixel = nWeight;
            m.Factor = nWeight + 8;
            result = Conv3x3(reference, m);
        }

        public static void GaussianBlur(Bitmap reference, ref Bitmap result, int nWeight = 4)
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(1);
            m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = 2;
            m.Pixel = nWeight;
            m.Factor = nWeight + 12;
            result = Conv3x3(reference, m);
        }

        public static void Sharpen(Bitmap reference, ref Bitmap result, int nWeight = 11)
        {
            ConvMatrix m = new ConvMatrix();
            m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = -2;
            m.Pixel = nWeight;
            m.Factor = nWeight - 8;
            result = Conv3x3(reference, m);

        }

        public static void MeanRemoval(Bitmap reference, ref Bitmap result, int nWeight = 9)
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(-1);
            m.Pixel = nWeight;
            m.Factor = nWeight - 8;
            result = Conv3x3(reference, m);
        }

       public static void EmbossLaplascian(Bitmap reference, ref Bitmap result, int nWeight = 4)
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(-1);
            m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = 0;
            m.Pixel = nWeight;
            m.Factor = nWeight - 3;
            m.Offset = 127;
            result = Conv3x3(reference, m);
        }

        public static void EmbossHoriVerti(Bitmap reference, ref Bitmap result, int nWeight = 4)
        {
            ConvMatrix m = new ConvMatrix();
            m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = -1;
            m.Pixel = nWeight;
            m.Factor = nWeight - 3;
            m.Offset = 127;
            result = Conv3x3(reference, m);
        }

        public static void EmbossAllDirections(Bitmap reference, ref Bitmap result, int nWeight = 8)
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(-1);
            m.Pixel = nWeight;
            m.Factor = nWeight - 7;
            m.Offset = 127;
            result = Conv3x3(reference, m);
        }

        public static void EmbossLossy(Bitmap reference, ref Bitmap result, int nWeight = 4)
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(-2);
            m.TopLeft = m.TopRight = m.BottomMid = 1;
            m.Pixel = nWeight;
            m.Factor = nWeight - 3;
            m.Offset = 127;
            result = Conv3x3(reference, m);
        }

        public static void EmbossHorizontal(Bitmap reference, ref Bitmap result, int nWeight = 2)
        {
            ConvMatrix m = new ConvMatrix();
            m.MidLeft = m.MidRight = -1;
            m.Pixel = nWeight;
            m.Factor = nWeight - 1;
            m.Offset = 127;
            result = Conv3x3(reference, m);
        }

        public static void EmbossVertical(Bitmap reference, ref Bitmap result, int nWeight = 0)
        {
            ConvMatrix m = new ConvMatrix();
            m.TopMid = -1;
            m.BottomMid = 1;
            m.Pixel = nWeight;
            m.Factor = nWeight + 1;
            m.Offset = 127;
            result = Conv3x3(reference, m);
        }
    }
}