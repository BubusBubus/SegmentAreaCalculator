using System;
using System.Drawing;
using System.Windows.Forms;

public class MonteCarloForm : Form
{
    // Элементы управления
    private TextBox txtX0, txtY0, txtRadius, txtC;
    private Button btnCalculate;
    private Label lblResult;
    private PictureBox canvas;

    // Параметры
    private double x0 = 2, y0 = -1, R = 2, C = -2;
   // private bool showHorizontal = true, showVertical = true;

    public MonteCarloForm()
    {
        // Настройка формы
        this.Text = "Методы Монте-Карло и мат.анализа для вычисления площади сегмента";
        this.ClientSize = new Size(1000, 700);
        this.DoubleBuffered = true;

        // Создаем элементы управления
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

        // Параметры окружности
        GroupBox gbCircle = new GroupBox()
        {
            Text = "Параметры окружности",
            Location = new Point(10, 20),
            Size = new Size(230, 145)
        };

        Label lblX0 = new Label() { Text = "X центра:", Location = new Point(10, 25), Width = 70 };
        txtX0 = new TextBox() { Text = "0", Location = new Point(90, 22), Width = 50 };

        Label lblY0 = new Label() { Text = "Y центра:", Location = new Point(10, 55), Width = 70 };
        txtY0 = new TextBox() { Text = "0", Location = new Point(90, 52), Width = 50 };

        Label lblRadius = new Label() { Text = "Радиус:", Location = new Point(10, 85), Width = 70 };
        txtRadius = new TextBox() { Text = "0", Location = new Point(90, 82), Width = 50 };

        Label lblC1 = new Label() { Text = "Горизонталь:", Location = new Point(10, 115), Width = 80 };
        txtC = new TextBox() { Text = "0", Location = new Point(90, 112), Width = 50 };

        gbCircle.Controls.AddRange(new Control[] { lblX0, txtX0, lblY0, txtY0, lblRadius, txtRadius, lblC1, txtC });

        // Кнопка расчета
        btnCalculate = new Button()
        {
            Text = "Вычислить площадь",
            Location = new Point(10, 280),
            Size = new Size(150, 40),
            BackColor = Color.LightBlue
        };
        btnCalculate.Click += BtnCalculate_Click;

        // Поле для результатов
        lblResult = new Label()
        {
            Location = new Point(10, 340),
            Size = new Size(230, 200),
            Font = new Font("Arial", 10),
            BorderStyle = BorderStyle.FixedSingle
        };

        // Добавляем элементы на панель
        paramPanel.Controls.AddRange(new Control[] { gbCircle, btnCalculate, lblResult });

        // Холст для рисования
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

    private void DotAdd() 
    { 

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
        // Вычисляем площади
        double exactArea = Math.PI * R * R; // Полная площадь круга
        double mathArea = CalculateMathematicalArea(R, x0, y0, C);
        double monteCarloArea = CalculateMonteCarloArea(10000);
        double error = Math.Abs(monteCarloArea - mathArea) / mathArea * 100;

        lblResult.Text = $"Площадь всей окружности: {exactArea:F4}\n" +
                         $"Площадь по матанализу: {mathArea:F4}\n" +
                         $"Монте-Карло: {monteCarloArea:F4}\n" +
                         $"Несовпадение: {error:F2}%";

        canvas.Invalidate();
    }

    private double CalculateMonteCarloArea(int trials)
    {
        // Определяем ограничивающий прямоугольник
        double xMin = x0 - R;
        double xMax = x0 + R;
        double yMin = y0 - R;
        double yMax = y0 + R;
        double totalArea = (xMax - xMin) * (yMax - yMin);

        int hits = 0;
        Random rand = new Random();

        for (int i = 0; i < trials; i++)
        {
            // Генерируем случайную точку в прямоугольнике
            double x = xMin + rand.NextDouble() * (xMax - xMin);
            double y = yMin + rand.NextDouble() * (yMax - yMin);

            // Проверяем, находится ли точка внутри окружности
            bool inCircle = Math.Pow(x - x0, 2) + Math.Pow(y - y0, 2) <= R * R;

            // Определяем, в большом сегменте ли точка (который содержит центр)
            bool inLargeSegment;
            if (C > y0) // Прямая выше центра - большой сегмент снизу
            {
                inLargeSegment = y < C;
            }
            else // Прямая ниже или на уровне центра - большой сегмент сверху
            {
                inLargeSegment = y > C;
            }

            if (inCircle && inLargeSegment)
            {
                hits++;
            }
        }
        return totalArea * hits / trials;
    }
    private double CalculateMathematicalArea(double R, double x0, double y0, double C)
    {
        // Вычисляем расстояние от центра до прямой
        double d = Math.Abs(y0 - C);

        if (d >= R)
        {
            // Прямая не пересекает окружность
            return Math.PI * R * R;
        }

        // Вычисляем площадь сегмента
        double segmentArea = R * R * Math.Acos(d / R) - d * Math.Sqrt(R * R - d * d);

        // Если прямая пересекает окружность, возвращаем площадь большого сегмента
        return Math.PI * R * R - segmentArea;
    }

    private void Canvas_Paint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.Clear(Color.White);

        // Масштабирование
        float scale = Math.Min(canvas.Width / (float)(2 * R + 4), canvas.Height / (float)(2 * R + 4));
        float centerX = canvas.Width / 2;
        float centerY = canvas.Height / 2;

        // Рисуем сетку (теперь учитываем положение осей)
        Pen gridPen = new Pen(Color.LightGray, 1);

        // Вертикальные линии сетки (должны проходить через ось Y)
        float firstGridX = centerX % scale; // Начинаем с ближайшей линии к оси Y
        for (float x = firstGridX; x < canvas.Width; x += scale)
        {
            g.DrawLine(gridPen, x, 0, x, canvas.Height);
            // Выделяем ось Y жирнее
            if (Math.Abs(x - centerX) < 0.5f) // Если это ось Y
            {
                g.DrawLine(Pens.Black, x, 0, x, canvas.Height);
            }
        }

        // Горизонтальные линии сетки (должны проходить через ось X)
        float firstGridY = centerY % scale; // Начинаем с ближайшей линии к оси X
        for (float y = firstGridY; y < canvas.Height; y += scale)
        {
            g.DrawLine(gridPen, 0, y, canvas.Width, y);
            // Выделяем ось X жирнее
            if (Math.Abs(y - centerY) < 0.5f) // Если это ось X
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
    }

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new MonteCarloForm());
    }
}