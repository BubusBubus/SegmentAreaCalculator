using MonteCarloApp.Forms;
using MonteCarloSegmentArea;
using System;
using System.Windows.Forms;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Создаем и показываем заставку
        SplashForm splash = new SplashForm();
        splash.Show();
        Application.DoEvents(); // Обрабатываем сообщения, чтобы форма отрисовалась

        // Ждем 3 секунды
        System.Threading.Thread.Sleep(3000);

        // Закрываем заставку и открываем основную форму
        splash.Close();
        splash.Dispose();

        Application.Run(new MonteCarloForm());
    }
}