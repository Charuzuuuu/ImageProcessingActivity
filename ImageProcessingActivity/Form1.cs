using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ImageProcessingActivity
{
    public partial class Form1 : Form
    {
        Bitmap loaded, processed;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;

        public Form1()
        {
            InitializeComponent();
            LoadVideoDevices();
        }

        private void LoadVideoDevices()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count == 0)
            {
                MessageBox.Show("No video devices found.");
            }
        }

        private void StartWebcam()
        {
            if (videoDevices.Count > 0)
            {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(Video_NewFrame);
                videoSource.Start();
                //toolStripStatusLabel1.Text = "Webcam started.";
            }
            else
            {
                MessageBox.Show("No video devices found.");
            }
        }

        private void StopWebcam()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                pictureBox1.Image = null;
                //toolStripStatusLabel1.Text = "Webcam stopped.";
            }
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap frame = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = frame;
        }

        private void btnStartWebcam_Click(object sender, EventArgs e)
        {
            StartWebcam();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*";
            openFileDialog1.Title = "Select an Image File";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    loaded = new Bitmap(openFileDialog1.FileName);
                    pictureBox1.Image = loaded;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not load image. Original error: " + ex.Message);
                }
            }
        }

        private void grayscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                processed = new Bitmap(pictureBox1.Image);
                Color pixel;
                int ave;

                for (int x = 0; x < processed.Width; x++)
                {
                    for (int y = 0; y < processed.Height; y++)
                    {
                        pixel = processed.GetPixel(x, y);
                        ave = (int)(pixel.R + pixel.G + pixel.B) / 3;
                        Color gray = Color.FromArgb(ave, ave, ave);
                        processed.SetPixel(x, y, gray);
                    }
                }
                pictureBox2.Image = processed;
            }
            else
            {
                MessageBox.Show("No image to process. Please start the webcam or load an image.");
            }
        }

        private void basicCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                processed = new Bitmap(pictureBox1.Image);
                pictureBox2.Image = processed;
            }
            else
            {
                MessageBox.Show("No image to copy. Please start the webcam or load an image.");
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            processed.Save(saveFileDialog1.FileName);
        }

        private void colorInversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                processed = new Bitmap(pictureBox1.Image);
                Color pixel;

                for (int x = 0; x < processed.Width; x++)
                {
                    for (int y = 0; y < processed.Height; y++)
                    {
                        pixel = processed.GetPixel(x, y);
                        Color invert = Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B);
                        processed.SetPixel(x, y, invert);
                    }
                }
                pictureBox2.Image = processed;
            }
            else
            {
                MessageBox.Show("No image to invert colors. Please start the webcam or load an image.");
            }
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Bitmap currentFrame = new Bitmap(pictureBox1.Image);
                BasicDIP.Hist(currentFrame, ref processed);
                pictureBox2.Image = processed;
            }
            else
            {
                MessageBox.Show("No image available for histogram.");
            }
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                processed = new Bitmap(pictureBox1.Image);

                for (int x = 0; x < processed.Width; x++)
                {
                    for (int y = 0; y < processed.Height; y++)
                    {
                        Color originalColor = processed.GetPixel(x, y);

                        int tr = (int)(0.393 * originalColor.R + 0.769 * originalColor.G + 0.189 * originalColor.B);
                        int tg = (int)(0.349 * originalColor.R + 0.686 * originalColor.G + 0.168 * originalColor.B);
                        int tb = (int)(0.272 * originalColor.R + 0.534 * originalColor.G + 0.131 * originalColor.B);

                        int r = Math.Min(255, tr);
                        int g = Math.Min(255, tg);
                        int b = Math.Min(255, tb);

                        Color sepiaColor = Color.FromArgb(r, g, b);
                        processed.SetPixel(x, y, sepiaColor);
                    }
                }

                pictureBox2.Image = processed;
            }
            else
            {
                MessageBox.Show("No image to apply sepia effect. Please start the webcam or load an image.");
            }
        }

        private void subtractionBasicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 advancedForm = new Form2();
            advancedForm.Show();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            loaded = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = loaded;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            StartWebcam();
            var a = 3;
        }

        private void oNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartWebcam();
        }

        private void oFFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopWebcam();
        }

        private void smoothingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Bitmap image = new Bitmap(pictureBox1.Image);
                bool success = ConvMatrix.Smooth(image);

                if (success)
                {
                    pictureBox2.Image = image;
                }
                else
                {
                    MessageBox.Show("Smoothing failed. Please try again.");
                }
            }
            else
            {
                MessageBox.Show("No image to smooth. Please start the webcam or load an image.");
            }

        }

        private void gausianBlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Bitmap image = new Bitmap(pictureBox1.Image);
                bool success = ConvMatrix.GaussianBlur(image); 

                if (success)
                {
                    pictureBox2.Image = image; 
                }
                else
                {
                    MessageBox.Show("Gaussian Blur failed. Please try again.");
                }
            }
            else
            {
                MessageBox.Show("No image to gaussian blur. Please start the webcam or load an image.");
            }
        }

        private void sharpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Bitmap image = new Bitmap(pictureBox1.Image);
                bool success = ConvMatrix.Sharpen(image); // Call Sharpen and store the result

                if (success)
                {
                    pictureBox2.Image = image; // If sharpening is successful, display it in pictureBox2
                }
                else
                {
                    MessageBox.Show("Sharpening failed. Please try again.");
                }
            }
            else
            {
                MessageBox.Show("No image to sharpen. Please start the webcam or load an image.");
            }
        }

        private void meanRemovalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Bitmap image = new Bitmap(pictureBox1.Image);
                bool success = ConvMatrix.MeanRemoval(image); 

                if (success)
                {
                    pictureBox2.Image = image;
                }
                else
                {
                    MessageBox.Show("Mean Removal failed. Please try again.");
                }
            }
            else
            {
                MessageBox.Show("No image to remove the mean. Please start the webcam or load an image.");
            }
        }

        private void embossToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Bitmap image = new Bitmap(pictureBox1.Image);
                bool success = ConvMatrix.Emboss(image); 

                if (success)
                {
                    pictureBox2.Image = image; 
                }
                else
                {
                    MessageBox.Show(" Embossing failed. Please try again.");
                }
            }
            else
            {
                MessageBox.Show("No image to emboss. Please start the webcam or load an image.");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopWebcam();
            base.OnFormClosing(e);
        }



    }

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

        public static bool Conv3x3(Bitmap b, ConvMatrix m)
        {
            if (m.Factor == 0) return false;
            Bitmap bSrc = (Bitmap)b.Clone();
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                            ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height),
                           ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = bmData.Stride;
            int stride2 = stride * 2;

            IntPtr Scan0 = bmData.Scan0;
            IntPtr SrcScan0 = bmSrc.Scan0;

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
                        nPixel = (((pSrc[2] * m.TopLeft) +
                            (pSrc[5] * m.TopMid) +
                            (pSrc[8] * m.TopRight) +
                            (pSrc[2 + stride] * m.MidLeft) +
                            (pSrc[5 + stride] * m.Pixel) +
                            (pSrc[8 + stride] * m.MidRight) +
                            (pSrc[2 + stride2] * m.BottomLeft) +
                            (pSrc[5 + stride2] * m.BottomMid) +
                            (pSrc[8 + stride2] * m.BottomRight))
                            / m.Factor) + m.Offset;

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[5 + stride] = (byte)nPixel;

                        p += 3;
                        pSrc += 3;
                    }
                    p += nOffset;
                    pSrc += nOffset;
                }
            }

            b.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);
            return true;
        }

        public static bool Smooth(Bitmap b, int nWeight = 1)
        {
            /*ConvMatrix m = new ConvMatrix();
            m.SetAll(1);
            m.Pixel = nWeight;
            m.Factor = nWeight + 8;
            return Conv3x3(b, m);*/

            ConvMatrix m = new ConvMatrix();
            m.SetAll(1);   // Set all cells to 1
            m.Factor = 9;  // The sum of all cells is 9
            return Conv3x3(b, m);
        }

        public static bool GaussianBlur(Bitmap b)
        {
            ConvMatrix m = new ConvMatrix();
            m.TopLeft = m.TopRight = m.BottomLeft = m.BottomRight = 1;
            m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = 2;
            m.Pixel = 4;
            m.Factor = 16;
            return Conv3x3(b, m);
        }

        public static bool Sharpen(Bitmap b, int weight = 11)
        {
            /*ConvMatrix m = new ConvMatrix();
            m.SetAll(-2);
            m.Pixel = weight;
            m.Factor = weight - 8;
            return Conv3x3(b, m);*/

            ConvMatrix m = new ConvMatrix();
            m.SetAll(-1);
            m.Pixel = 9;
            m.Factor = 1;
            return Conv3x3(b, m);

        }

        public static bool MeanRemoval(Bitmap b)
        {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(-1);
            m.Pixel = 9;
            m.Factor = 1;
            return Conv3x3(b, m);
        }

        public static bool Emboss(Bitmap b)
        {
            ConvMatrix m = new ConvMatrix();
            m.TopLeft = m.TopRight = m.BottomLeft = m.BottomRight = -1;
            m.Pixel = 4;
            m.Offset = 127;
            return Conv3x3(b, m);
        }

    }
}