using System;
using System.Windows.Forms;
using Client.Forms;
using Client.Network;

namespace Client
{
    internal static class Program
    {
        [STAThread]
        
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Чтобы видеть реальные ошибки (если что-то упадет)
            Application.ThreadException += (s, e) =>
                MessageBox.Show(e.Exception.ToString(), "UI Thread Exception");

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                MessageBox.Show(e.ExceptionObject?.ToString() ?? "Unknown", "Unhandled Exception");

            Application.Run(new AppContext());
        }
    }

    public class AppContext : ApplicationContext
    {
        private LoginForm _loginForm;
        private bool _loggedIn;

        public AppContext()
        {
            _loginForm = new LoginForm();
            _loginForm.LoginSucceeded += OnLoginSucceeded;
            _loginForm.FormClosed += (s, e) => ExitThread(); // если юзер закрыл логин — выходим
            _loginForm.Show();
        }

        private void OnLoginSucceeded(NetworkClient client, string login)
        {
            if (_loggedIn) return;
            _loggedIn = true;

            try
            {
                var main = new MainForm(client, login);
                MainForm = main;

                main.FormClosed += (s, e) => ExitThread();

                _loginForm.Hide();   // 🔥 ключ: не Close(), а Hide()

                main.Show();
                main.BringToFront();
                main.Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("MainForm error:\n" + ex);
                _loggedIn = false;
            }
        }
    }
}