using System;
using System.Drawing;
using System.Windows.Forms;

namespace MonteCarloSegmentArea
{
    public partial class SplashForm : Form
    {
        public SplashForm()
        {
            this.FormBorderStyle = FormBorderStyle.None; // Убираем рамку
            this.StartPosition = FormStartPosition.CenterScreen; // Центрируем на экране
            this.BackColor = Color.AliceBlue; // Фоновый цвет
            this.Size = new Size(900, 300); // Размер формы
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Настройки шрифтов
            Font titleFont = new Font("Arial", 24, FontStyle.Bold);
            Font infoFont = new Font("Arial", 14);
            Font authorFont = new Font("Arial", 18, FontStyle.Bold);

            // Кисти для рисования
            Brush textBrush = Brushes.YellowGreen;
            Brush borderBrush = Brushes.DarkSeaGreen;

            // Рисуем рамку вокруг формы
            e.Graphics.DrawRectangle(new Pen(borderBrush, 5),
                new Rectangle(0, 0, this.Width - 1, this.Height - 1));

            // Рисуем заголовок
            string title = "Вычисление площади сегмента окружности";
            SizeF titleSize = e.Graphics.MeasureString(title, titleFont);
            e.Graphics.DrawString(title, titleFont, textBrush,
                (this.Width - titleSize.Width) / 2, 30);

            // Рисуем информацию о параметрах
            string parameters = "               x0: -1,  y0: 0,  R: 3\n" +
                               "направление: горизонтально, C: -1";
            SizeF paramSize = e.Graphics.MeasureString(parameters, infoFont);
            e.Graphics.DrawString(parameters, infoFont, textBrush,
                (this.Width - paramSize.Width) / 2, 100);

            // Рисуем информацию об авторе
            string author = "Автор: Дмитриев Тимур";
            SizeF authorSize = e.Graphics.MeasureString(author, authorFont);
            e.Graphics.DrawString(author, authorFont, textBrush,
                (this.Width - authorSize.Width) / 2, 200);
        }
    }
}