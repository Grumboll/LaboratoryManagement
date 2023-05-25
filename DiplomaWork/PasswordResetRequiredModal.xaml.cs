using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using System.Security.Cryptography;
using DiplomaWork.Models;
using System.IO;

namespace DiplomaWork
{
    public partial class PasswordResetRequiredModal : Window
    {
        public PasswordResetRequiredModal()
        {
            InitializeComponent();
        }

        Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.TopRight,
                offsetX: 10,
                offsetY: 10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            PasswordBox.Visibility = Visibility.Collapsed;
            PasswordTextBox.Visibility = Visibility.Visible;
            PasswordTextBox.Text = PasswordBox.Password;
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordBox.Visibility = Visibility.Visible;
            PasswordTextBox.Visibility = Visibility.Collapsed;
            PasswordBox.Focus();
        }

        private void ConfirmToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ConfirmPasswordBox.Visibility = Visibility.Collapsed;
            ConfirmPasswordTextBox.Visibility = Visibility.Visible;
            ConfirmPasswordTextBox.Text = ConfirmPasswordBox.Password;
        }

        private void ConfirmToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ConfirmPasswordBox.Visibility = Visibility.Visible;
            ConfirmPasswordTextBox.Visibility = Visibility.Collapsed;
            ConfirmPasswordBox.Focus();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private bool ContainsSpecialSymbol(string password)
        {
            const string specialSymbols = "!@#$%^&*()-_=+[]{};:'\",.<>/?";
            return password.Any(ch => specialSymbols.Contains(ch));
        }

        private bool ContainsNumber(string password)
        {
            return password.Any(ch => char.IsDigit(ch));
        }

        private string GenerateSalt()
        {
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(allowedChars, 16)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            string newPassword1 = PasswordBox.Password;
            string newPassword2 = ConfirmPasswordBox.Password;

            if (newPassword1 != newPassword2)
            {
                notifier.ShowError("Паролите не съвпадат, опитайте отново!");
                return;
            }

            if (newPassword1.Length < 8)
            {
                notifier.ShowError("Паролата трябва да е поне 8 символа!");
                return;
            }

            if (!ContainsSpecialSymbol(newPassword1))
            {
                notifier.ShowError("Паролата трябва да съдържа специални символи!");
                return;
            }

            if (!ContainsNumber(newPassword1))
            {
                notifier.ShowError("Паролата трябва да съдържа цифри!");
                return;
            }

            string salt = GenerateSalt();

            string hashedPassword1 = HashPassword(newPassword1, salt);

            using (var dbContext = new laboratory_2023Context())
            {
                User user = dbContext.Users.FirstOrDefault(u => u.Id == App.CurrentUser.Id);

                user.PasswordResetRequired = false;
                user.Password = hashedPassword1;
                user.PasswordSalt = salt;

                dbContext.SaveChanges();

                if (File.Exists("login.dat"))
                {
                    File.Delete("login.dat");
                }
            }

            bool? successMessage = new CustomMessageBox("Успешно променихте паролата си!", "Промяна на паролата!").ShowDialog();

            this.Close();
        }
    }
}
