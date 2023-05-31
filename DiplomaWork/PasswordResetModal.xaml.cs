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
using DiplomaWork.Services;

namespace DiplomaWork
{
    public partial class PasswordResetModal : Window
    {
        public PasswordResetModal(string mode)
        {
            InitializeComponent();

            switch (mode)
            {
                case "resetRequired":
                    ChangePasswordSubText.Text = "Администратор е изискал да промените паролата си.";
                    break;
                case "resetForgotten":
                    ChangePasswordSubText.Text = "Напишете нова парола.";
                    break;
            }
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
            PasswordBox.Password = PasswordTextBox.Text;
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
            ConfirmPasswordBox.Password = ConfirmPasswordTextBox.Text;
            ConfirmPasswordBox.Focus();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private bool IsPasswordValid(string password, string confirmPassword)
        {
            // Check if passwords match
            if (password != confirmPassword)
            {
                return false;
            }

            // Check if passwords meet the required criteria
            bool isLengthValid = password.Length >= 8;
            bool hasSpecialSymbol = password.Any(c => !char.IsLetterOrDigit(c));
            bool hasUppercase = password.Any(c => char.IsUpper(c));
            bool hasNumber = password.Any(c => char.IsDigit(c));

            return isLengthValid && hasSpecialSymbol && hasUppercase && hasNumber;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            string password = string.Empty;
            string confirmPassword = string.Empty;

            if (PasswordBox.Visibility == Visibility.Visible)
            {
                password = PasswordBox.Password;
            }
            else if (PasswordTextBox.Visibility == Visibility.Visible)
            {
                password = PasswordTextBox.Text;
            }
            
            if (ConfirmPasswordBox.Visibility == Visibility.Visible)
            {
                confirmPassword = ConfirmPasswordBox.Password;
            }
            else if (ConfirmPasswordTextBox.Visibility == Visibility.Visible)
            {
                confirmPassword = ConfirmPasswordTextBox.Text;
            }

            if (!IsPasswordValid(password, confirmPassword))
            {
                notifier.ShowWarning("Паролите не са еднакви или са невалидни (Трябва да съдържат поне 8 символа, голяма буква, специален символ и число)!");
                return;
            }

            string salt = UserService.GenerateSalt();

            string hashedPassword1 = UserService.HashPassword(password, salt);

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
