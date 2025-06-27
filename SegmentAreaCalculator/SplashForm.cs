using System;
using System.Drawing;
using System.Windows.Forms;

namespace MonteCarloSegmentArea
{
    public class SplashForm : Form
    {
        // Удаляем вызов InitializeComponent(), так как мы его не используем
        public SplashForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.LightSteelBlue;
            this.Size = new Size(900, 300);

            // Создаем и настраиваем элементы управления
            Label titleLabel = new Label
            {
                Text = "Вычисление площади сегмента окружности",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                AutoSize = true,
                Top = 30
            };
            titleLabel.Left = (this.Width - titleLabel.Width) / 2;

            Label paramsLabel = new Label
            {
                Text = "           x0: -1,  y0: 0,  R: 3\n" +
                "направление: горизонтально, C: -1",
                Font = new Font("Arial", 12),
                ForeColor = Color.DarkBlue,
                AutoSize = true,
                Top = 100,
                TextAlign = ContentAlignment.MiddleCenter
            };
            paramsLabel.Left = (this.Width - paramsLabel.Width) / 2;

            Label authorLabel = new Label
            {
                Text = "Автор: Дмитриев Тимур",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                AutoSize = true,
                Top = 200
            };
            authorLabel.Left = (this.Width - authorLabel.Width) / 2;

            // Добавляем элементы на форму
            this.Controls.Add(titleLabel);
            this.Controls.Add(paramsLabel);
            this.Controls.Add(authorLabel);

            // Рисуем рамку
            this.Paint += (s, e) =>
                e.Graphics.DrawRectangle(new Pen(Color.Navy, 5),
                    new Rectangle(0, 0, this.Width - 1, this.Height - 1));

            // Закрытие по клику (опционально)
            this.Click += (s, e) => this.Close();
        }
    }
}