using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using OxyPlot.Axes;

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
            this.Text = "График точности вычислений";
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

                var plotModel = new PlotModel
                {
                    Title = "Статистика точности вычислений",
                    TitleFontSize = 14
                };

                // Ось X - номера экспериментов
                plotModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Title = "Номер эксперимента",
                    Minimum = 0,
                    Maximum = data.Rows.Count + 1,
                    MajorGridlineStyle = LineStyle.Solid,
                    MinorGridlineStyle = LineStyle.Dot
                });

                // Ось Y (левая) - количество точек
                plotModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Title = "Количество точек",
                    TitleColor = OxyColors.Blue,
                    TextColor = OxyColors.Blue,
                    MajorGridlineStyle = LineStyle.Solid,
                    Minimum = 0
                });

                // Ось Y2 (правая) - погрешность в %
                var errorAxis = new LinearAxis
                {
                    Position = AxisPosition.Right,
                    Title = "Погрешность (%)",
                    TitleColor = OxyColors.Red,
                    TextColor = OxyColors.Red,
                    MajorGridlineStyle = LineStyle.None,
                    Minimum = 0,
                    Maximum = GetMaxError(data) * 1.2,
                    Key = "ErrorAxis" // Уникальный ключ для оси
                };
                plotModel.Axes.Add(errorAxis);

                // Серия для количества точек
                var pointsSeries = new LineSeries
                {
                    Title = "Количество точек",
                    Color = OxyColors.Blue,
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 4,
                    MarkerStroke = OxyColors.Blue,
                    MarkerFill = OxyColors.Blue,
                    StrokeThickness = 2
                };

                // Серия для погрешности
                var errorSeries = new LineSeries
                {
                    Title = "Погрешность",
                    Color = OxyColors.Red,
                    MarkerType = MarkerType.Square,
                    MarkerSize = 4,
                    MarkerStroke = OxyColors.Red,
                    MarkerFill = OxyColors.Red,
                    StrokeThickness = 2,
                    YAxisKey = "ErrorAxis" // Используем тот же ключ, что и у оси
                };

                // Заполняем данные
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    var row = data.Rows[i];
                    int pointCount = Convert.ToInt32(row["PointCount"]);
                    double error = Convert.ToDouble(row["ErrorPercent"]);

                    pointsSeries.Points.Add(new DataPoint(i + 1, pointCount));
                    errorSeries.Points.Add(new DataPoint(i + 1, error));
                }

                plotModel.Series.Add(pointsSeries);
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

        private double GetMaxError(DataTable data)
        {
            double maxError = 0;
            foreach (DataRow row in data.Rows)
            {
                double error = Convert.ToDouble(row["ErrorPercent"]);
                if (error > maxError) maxError = error;
            }
            return maxError > 0 ? maxError : 1.0;
        }
    }
}