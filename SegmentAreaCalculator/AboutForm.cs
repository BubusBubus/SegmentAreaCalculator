using System;
using System.Drawing;
using System.Windows.Forms;

namespace MonteCarloSegmentArea
{
    public class AboutForm : Form
    {
        public AboutForm()
        {
            // Настройки формы
            this.Text = "О программе";
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.Size = new Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Создаем и настраиваем элементы управления
            Label titleLabel = new Label
            {
                Text = "Вычисление площади\n" +
                "сегмента окружности",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                AutoSize = true,
                Top = 20
            };
            titleLabel.Left = (this.Width - titleLabel.Width) / 4;

            // Информация о программе
            Label infoLabel = new Label
            {
                Text = $"Версия: 0.1.8\n\n" +
                       $"Разработчик: Дмитриев Тимур Андреевич\n\n" +
                       $"Дата разработки: 26.06.2025\n\n" +
                       $"Метод: Монте-Карло с графической визуализацией",
                Font = new Font("Arial", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Top = 70,
                TextAlign = ContentAlignment.MiddleCenter
            };
            infoLabel.Left = (this.Width - infoLabel.Width) / 10;

            // Кнопка закрытия
            Button closeButton = new Button
            {
                Text = "Закрыть",
                Font = new Font("Arial", 10),
                Size = new Size(100, 30),
                Top = 220
            };
            closeButton.Left = (this.Width - closeButton.Width) / 2;
            closeButton.Click += (s, e) => this.Close();

            // Добавляем элементы на форму
            this.Controls.Add(titleLabel);
            this.Controls.Add(infoLabel);
            this.Controls.Add(closeButton);

            // Иконка (опционально)
            try { this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { /* Игнорируем ошибки загрузки иконки */ }
        }
    }
}