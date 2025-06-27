using System;
using System.Drawing;
using System.Windows.Forms;

namespace MonteCarloApp.Forms
{
    public class SplashScreenForm : Form
    {
        private Timer timer;
        private int displayTime = 3000; // 3 секунды
        private Image logoImage; 

        public SplashScreenForm()
        {
 
            try
            {
                logoImage = Image.FromFile("logo.png"); 
            }
            catch
            {
                logoImage = null; 
            }

            // Настройки формы
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(500, 350); 
            this.BackColor = Color.White;
            this.DoubleBuffered = true;
            this.Paint += SplashScreenForm_Paint;

            // Таймер для закрытия
            timer = new Timer { Interval = displayTime };
            timer.Tick += (s, e) => { timer.Stop(); this.Close(); };
            timer.Start();
        }

        private void SplashScreenForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Фон
            using (var brush = new SolidBrush(Color.FromArgb(240, 240, 240)))
                g.FillRectangle(brush, this.ClientRectangle);

            if (logoImage != null)
            {
                int imageWidth = 150; // Ширина изображения
                int imageHeight = 150; // Высота изображения
                int imageX = (this.Width - imageWidth) / 2;
                int imageY = 20;

                g.DrawImage(logoImage, imageX, imageY, imageWidth, imageHeight);
            }

            string appName = "Monte-Carlo Segment Area Calculator";
            using (var font = new Font("Arial", 20, FontStyle.Bold))
            using (var textBrush = new SolidBrush(Color.Blue))
            {
                SizeF textSize = g.MeasureString(appName, font);
                float x = (this.Width - textSize.Width) / 2;
                float y = logoImage != null ? 180 : 50; 
                g.DrawString(appName, font, textBrush, x, y);
            }

            string version = "Version 0.1.9";
            using (var font = new Font("Arial", 12))
            using (var textBrush = new SolidBrush(Color.Gray))
            {
                SizeF textSize = g.MeasureString(version, font);
                float x = (this.Width - textSize.Width) / 2;
                float y = logoImage != null ? 220 : 100;
                g.DrawString(version, font, textBrush, x, y);
            }

            using (var pen = new Pen(Color.Blue, 3))
            {
                int progressWidth = (int)(this.Width * 0.7);
                int progressX = (this.Width - progressWidth) / 2;
                int progressY = this.Height - 50;
                g.DrawRectangle(pen, progressX, progressY, progressWidth, 10);

                int animatedWidth = (int)(progressWidth * (1 - (double)timer.Interval / displayTime));
                using (var progressBrush = new SolidBrush(Color.LightBlue))
                    g.FillRectangle(progressBrush, progressX, progressY, animatedWidth, 10);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                logoImage?.Dispose();
            base.Dispose(disposing);
        }
    }
}