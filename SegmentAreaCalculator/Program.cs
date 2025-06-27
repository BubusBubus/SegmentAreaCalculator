using MonteCarloApp.Forms;
using System;
using System.Windows.Forms;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Показываем Splash Screen
        using (var splash = new SplashScreenForm())
        {
            splash.ShowDialog();
        }

        Application.Run(new MonteCarloForm());
    }
}