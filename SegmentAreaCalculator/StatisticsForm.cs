using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using OxyPlot.Axes;
using System.Collections.Generic;

namespace MonteCarloApp.Forms
{
    public class StatisticsForm : Form
    {
        private PlotView plotView;
        private DBHelper dbHelper;

        public StatisticsForm(DBHelper dbHelper)
        {
            this.dbHelper = dbHelper;
            InitializeComponents();
            LoadChartData();
        }

        private void InitializeComponents()
        {
            // Настройки окна
            this.Text = "Зависимость погрешности от количества точек";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;

            // Инициализация PlotView
            plotView = new PlotView
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // Кнопка закрытия
            var btnClose = new Button
            {
                Text = "Закрыть",
                Size = new Size(100, 40),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnClose.Click += (s, e) => this.Close();

            // Размещение элементов
            this.Controls.Add(plotView);
            this.Controls.Add(btnClose);
        }

        private void LoadChartData()
        {
            try
            {
                var data = dbHelper.GetAllResults();
                if (data.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных для построения графика",
                                "Информация",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                    this.Close();
                    return;
                }

                // Группируем данные по количеству точек и вычисляем среднюю погрешность
                var groupedData = new Dictionary<int, List<double>>();

                foreach (DataRow row in data.Rows)
                {
                    int pointCount = Convert.ToInt32(row["PointCount"]);
                    double error = Convert.ToDouble(row["ErrorPercent"]);

                    if (!groupedData.ContainsKey(pointCount))
                    {
                        groupedData[pointCount] = new List<double>();
                    }
                    groupedData[pointCount].Add(error);
                }

                // Сортируем по количеству точек
                var sortedPoints = groupedData.Keys.OrderBy(x => x).ToList();

                // Создаем модель графика
                var plotModel = new PlotModel
                {
                    Title = "Зависимость погрешности от количества точек",
                    TitleFontSize = 14,
                    Subtitle = "Средняя погрешность для каждого количества точек",
                    SubtitleFontSize = 10
                };

                // Ось X (логарифмическая) - количество точек
                plotModel.Axes.Add(new LogarithmicAxis
                {
                    Position = AxisPosition.Bottom,
                    Title = "Количество точек (логарифмическая шкала)",
                    Minimum = 100,
                    Maximum = 1000000,
                    MajorGridlineStyle = LineStyle.Solid,
                    MinorGridlineStyle = LineStyle.Dot,
                    UseSuperExponentialFormat = false,
                    Base = 10
                });

                // Ось Y - погрешность в %
                plotModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Title = "Средняя погрешность (%)",
                    MajorGridlineStyle = LineStyle.Solid,
                    MinorGridlineStyle = LineStyle.Dot,
                    Minimum = 0
                });

                // Серия для средней погрешности
                var errorSeries = new LineSeries
                {
                    Title = "Средняя погрешность",
                    Color = OxyColors.Red,
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 4,
                    MarkerStroke = OxyColors.Red,
                    MarkerFill = OxyColors.White,
                    StrokeThickness = 2
                };

                // Заполняем данные (средняя погрешность для каждого количества точек)
                foreach (var pointCount in sortedPoints)
                {
                    double avgError = groupedData[pointCount].Average();
                    errorSeries.Points.Add(new DataPoint(pointCount, avgError));
                }

                plotModel.Series.Add(errorSeries);

                // Настраиваем легенду
                plotModel.LegendTitle = "Легенда";
                plotModel.LegendOrientation = LegendOrientation.Horizontal;
                plotModel.LegendPlacement = LegendPlacement.Outside;
                plotModel.LegendPosition = LegendPosition.BottomCenter;

                plotView.Model = plotModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при построении графика:\n{ex.Message}",
                              "Ошибка",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
                this.Close();
            }
        }
    }
}