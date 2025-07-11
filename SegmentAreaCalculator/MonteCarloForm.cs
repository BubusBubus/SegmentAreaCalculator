﻿using MonteCarloSegmentArea;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;


namespace MonteCarloApp.Forms
{
    public class MonteCarloForm : Form
    {
        private List<(PointF point, bool isHit)> monteCarloPoints = new List<(PointF, bool)>();
        private TextBox txtX0, txtY0, txtRadius, txtC;
        private Button btnCalculate, btnViewDatabase;
        private Label lblResult;
        private PictureBox canvas;
        private NumericUpDown numTrials;
        private DBHelper dbHelper;
        private Button btnAbout;

        //параметры
        private double x0 = 2, y0 = -1, R = 2, C = -2;

        public MonteCarloForm()
        {
            //настройки формы
            this.Text = "Вычисление площади сегмента окружности";
            this.ClientSize = new Size(1000, 700);
            this.DoubleBuffered = true;

            dbHelper = new DBHelper("Data Source=MonteCarloResults.db");

            //создаем элемент управления
            CreateControls();
        }

        private void CreateControls()
        {
            // Панель для параметров 
            Panel paramPanel = new Panel()
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = SystemColors.Control
            };

            // Параметры окружности (без изменений)
            GroupBox gbCircle = new GroupBox()
            {
                Text = "Параметры окружности",
                Location = new Point(10, 20),
                Size = new Size(230, 145)
            };

            Label lblX0 = new Label() { Text = "X центра:", Location = new Point(10, 25), Width = 70 };
            txtX0 = new TextBox() { Text = x0.ToString(), Location = new Point(90, 22), Width = 50 };

            Label lblY0 = new Label() { Text = "Y центра:", Location = new Point(10, 55), Width = 70 };
            txtY0 = new TextBox() { Text = y0.ToString(), Location = new Point(90, 52), Width = 50 };

            Label lblRadius = new Label() { Text = "Радиус:", Location = new Point(10, 85), Width = 70 };
            txtRadius = new TextBox() { Text = R.ToString(), Location = new Point(90, 82), Width = 50 };

            Label lblC1 = new Label() { Text = "Горизонталь:", Location = new Point(10, 115), Width = 80 };
            txtC = new TextBox() { Text = C.ToString(), Location = new Point(90, 112), Width = 50 };

            gbCircle.Controls.AddRange(new Control[] { lblX0, txtX0, lblY0, txtY0, lblRadius, txtRadius, lblC1, txtC });

            // Параметры для Монте-Карло (без изменений)
            Label lblTrials = new Label() { Text = "Точек Монте-Карло:", Location = new Point(10, 190), Width = 120 };
            numTrials = new NumericUpDown()
            {
                Minimum = 100,
                Maximum = 1000000,
                Value = 10000,
                Increment = 1000,
                Location = new Point(130, 188),
                Width = 80
            };

            // Кнопка рассчета (без изменений)
            btnCalculate = new Button()
            {
                Text = "Вычислить площадь",
                Location = new Point(10, 230),
                Size = new Size(150, 40),
                BackColor = Color.LightBlue
            };
            btnCalculate.Click += BtnCalculate_Click;

            // Кнопка просмотра БД (без изменений)
            btnViewDatabase = new Button()
            {
                Text = "Просмотр базы данных",
                Location = new Point(10, 280),
                Size = new Size(150, 40),
                BackColor = Color.LightGreen
            };
            btnViewDatabase.Click += BtnViewDatabase_Click;

            // Сначала инициализируем lblResult
            lblResult = new Label()
            {
                Location = new Point(10, 380), // Теперь это значение будет использоваться
                Size = new Size(230, 150),
                Font = new Font("Arial", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Теперь добавляем кнопку "О программе" (после инициализации lblResult)
            btnAbout = new Button()
            {
                Text = "О программе",
                Location = new Point(10, 330), // Размещаем между btnViewDatabase и lblResult
                Size = new Size(150, 40),
                BackColor = Color.LightGoldenrodYellow
            };
            btnAbout.Click += BtnAbout_Click;

            // Добавляем элементы на панель в правильном порядке
            paramPanel.Controls.AddRange(new Control[] {
                gbCircle,
                lblTrials,
                numTrials,
                btnCalculate,
                btnViewDatabase,
                btnAbout,    // Добавляем новую кнопку
                lblResult    // Теперь lblResult инициализирован
            });

            canvas = new PictureBox()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            canvas.Paint += Canvas_Paint;

            // Добавляем панель и холст на форму
            this.Controls.Add(paramPanel);
            this.Controls.Add(canvas);
        }

        private void BtnViewDatabase_Click(object sender, EventArgs e)
        {
            var dbViewer = new DatabaseViewerForm(dbHelper);
            dbViewer.DataSelected += (x0, y0, radius, c) =>
            {
                // Устанавливаем значения в текстовые поля
                this.txtX0.Text = x0.ToString();
                this.txtY0.Text = y0.ToString();
                this.txtRadius.Text = radius.ToString();
                this.txtC.Text = c.ToString();

                // Вызываем расчет автоматически
                this.BtnCalculate_Click(null, null);
            };
            dbViewer.ShowDialog();
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(txtX0.Text, out x0) ||
                !double.TryParse(txtY0.Text, out y0) ||
                !double.TryParse(txtRadius.Text, out R) ||
                !double.TryParse(txtC.Text, out C))
            {
                MessageBox.Show("Ошибка ввода данных. Проверьте значения.");
                return;
            }

            //- предидущие отчки 
            monteCarloPoints.Clear();
            canvas.Invalidate();


            //считаем площади
            double exactArea = Math.PI * R * R;
            double mathArea = CalculateMathematicalArea(R, x0, y0, C);
            double monteCarloArea = CalculateMonteCarloArea((int)numTrials.Value);
            double error = Math.Abs(monteCarloArea - mathArea) / mathArea * 100;

            lblResult.Text = $"Площадь всей окружности: {exactArea:F4}\n" +
                             $"Площадь по матанализу: {mathArea:F4}\n" +
                             $"Монте-Карло: {monteCarloArea:F4}\n" +
                             $"Несовпадение: {error:F2}%";

            dbHelper.SaveResult(x0, y0, R, C, exactArea, mathArea, monteCarloArea, (int)numTrials.Value, error);
            canvas.Invalidate();//обновление холста с новыми точками 
        }

        private double CalculateMonteCarloArea(int trials)
        {
            monteCarloPoints.Clear();//- предидущие точки 
            float scale = GetScaleFactor();
            float centerX = canvas.Width / 2f;
            float centerY = canvas.Height / 2f;

            double xMin = x0 - R;
            double xMax = x0 + R;
            double yMin = y0 - R;
            double yMax = y0 + R;
            double totalArea = (xMax - xMin) * (yMax - yMin);

            int hits = 0;
            Random rand = new Random();

            for (int i = 0; i < trials; i++)
            {
                //случайная точка
                double x = xMin + rand.NextDouble() * (xMax - xMin);
                double y = yMin + rand.NextDouble() * (yMax - yMin);

                //проверяем условия
                bool inCircle = Math.Pow(x - x0, 2) + Math.Pow(y - y0, 2) <= R * R;
                bool inLargeSegment = (C > y0) ? y < C : y > C;
                bool isHit = inCircle && inLargeSegment;

                if (isHit) hits++;



                // Преобразуем координаты в экранные и сохраняем точку
                float screenX = centerX + (float)x * scale;
                float screenY = centerY - (float)y * scale;
                monteCarloPoints.Add((new PointF(screenX, screenY), isHit));
            }
            return totalArea * hits / trials;
        }

        private float GetScaleFactor()
        {
            return Math.Min(canvas.Width / (float)(2 * R + 4), canvas.Height / (float)(2 * R + 4));
        }

        private double CalculateMathematicalArea(double R, double x0, double y0, double C)
        {
            // Вычисляем расстояние от центра до прямой
            double d = Math.Abs(y0 - C);

            if (d >= R)
            {
                return Math.PI * R * R; // Прямая не пересекает окружность
            }

            double segmentArea = R * R * Math.Acos(d / R) - d * Math.Sqrt(R * R - d * d);
            return Math.PI * R * R - segmentArea;  // Если прямая пересекает окружность, возвращаем площадь большого сегмента
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            // Масштабирование
            float scale = GetScaleFactor();
            float centerX = canvas.Width / 2;
            float centerY = canvas.Height / 2;

            // Рисуем сетку
            Pen gridPen = new Pen(Color.LightGray, 1);


            float firstGridX = centerX % scale;
            for (float x = firstGridX; x < canvas.Width; x += scale)
            {
                g.DrawLine(gridPen, x, 0, x, canvas.Height);
                if (Math.Abs(x - centerX) < 0.5f)   // Выделяем ось Y жирнее
                {
                    g.DrawLine(Pens.Black, x, 0, x, canvas.Height);
                }
            }

            float firstGridY = centerY % scale; // Горизонтальные линии сетки
            for (float y = firstGridY; y < canvas.Height; y += scale)
            {
                g.DrawLine(gridPen, 0, y, canvas.Width, y);
                if (Math.Abs(y - centerY) < 0.5f)// Выделяем ось X жирнее
                {
                    g.DrawLine(Pens.Black, 0, y, canvas.Width, y);
                }
            }

            // Окружность
            float circleX = centerX + (float)x0 * scale;
            float circleY = centerY - (float)y0 * scale;
            float diameter = (float)(2 * R) * scale;
            g.DrawEllipse(new Pen(Color.Blue, 2), circleX - diameter / 2, circleY - diameter / 2, diameter, diameter);
            // Горизонтальная прямая (y = C)
            float lineY = centerY - (float)C * scale;
            g.DrawLine(new Pen(Color.Red, 2), 0, lineY, canvas.Width, lineY);

            if (monteCarloPoints.Count > 0)  // Рисуем точки Монте-Карло (если есть)
            {
                float pointSize = Math.Max(2, 3f - (float)monteCarloPoints.Count / 5000);

                foreach (var (point, isHit) in monteCarloPoints)// Автомасштабирование
                {
                    using (var brush = new SolidBrush(isHit ? Color.Green : Color.Red))
                    {
                        g.FillEllipse(brush, point.X - pointSize / 2, point.Y - pointSize / 2, pointSize, pointSize);
                    }
                }
            }
        }
        private void BtnAbout_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog(this);
        }
    }
}