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
using DiplomaWork.Models;
using DiplomaWork.Services;
using System.Text.RegularExpressions;

namespace DiplomaWork
{
    public partial class UserModal : Window
    {
        string passwordText = string.Empty;
        public UserModal()
        {
            InitializeComponent();
            LoadRoles();
        }
        private void LoadRoles()
        {
            using (var dbContext = new laboratory_2023Context())
            {
                var roles = dbContext.Roles.ToList();

                RolesComboBox.ItemsSource = roles;
                RolesComboBox.DisplayMemberPath = "Name";
                RolesComboBox.SelectedValuePath = "Id";
            }
        }

        private void RolesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RolesComboBox.SelectedIndex == -1)
            {
                RolesComboBox.Text = "Изберете роля*:";
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (validateFormData())
            {
                UserService.createUser(UserNameTextBox.Text, UserEMailTextBox.Text, DateOnly.FromDateTime(UserDateOfBirthDatePicker.SelectedDate.Value.Date),
                    UserPhoneNumberTextBox.Text, passwordText, UserFirstNameTextBox.Text, UserLastNameTextBox.Text, ((Role)RolesComboBox.SelectedItem).Id);

                notifier.ShowSuccess("Успешно запазихте потребител.");
                this.Close();
            }
        }

        private bool validateFormData()
        {
            string username = UserNameTextBox.Text;
            string email = UserEMailTextBox.Text;
            string phoneNumber = UserPhoneNumberTextBox.Text;
            string password = string.Empty;
            string confirmPassword = string.Empty;
            string firstName = UserFirstNameTextBox.Text;
            string lastName = UserLastNameTextBox.Text;

            if (string.IsNullOrEmpty(username))
            {
                notifier.ShowWarning("Потребителското име не може да бъде празно!");
                UserNameTextBox.Focus();

                return false;
            }
            else if (username.Length < 3)
            {
                notifier.ShowWarning("Потребителското име трябва да бъде поне 3 символа!");
                UserNameTextBox.Focus();

                return false;
            }

            if (!string.IsNullOrEmpty(email))
            {
                string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

                if (!Regex.IsMatch(email, emailPattern))
                {
                    notifier.ShowWarning("Неправилен имейл формат!");
                    UserEMailTextBox.Focus();

                    return false;
                }
            }

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                string phoneNumberPattern = @"^\d{10}$";

                if (!Regex.IsMatch(phoneNumber, phoneNumberPattern))
                {
                    notifier.ShowWarning("Неправилен формат на телефонния номер!");
                    UserPhoneNumberTextBox.Focus();

                    return false;   
                }
            }

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

                return false;
            }

            passwordText = password;

            if (string.IsNullOrEmpty(firstName))
            {
                notifier.ShowWarning("Името е задължително!");
                UserFirstNameTextBox.Focus();

                return false;
            }
            else if (!IsNameValid(firstName))
            {
                notifier.ShowWarning("Името е невалидно!");
                UserFirstNameTextBox.Focus();

                return false;
            }
            
            if (string.IsNullOrEmpty(lastName))
            {
                notifier.ShowWarning("Името е задължително!");
                UserLastNameTextBox.Focus();

                return false;
            }
            else if (!IsNameValid(lastName))
            {
                notifier.ShowWarning("Името е невалидно!");
                UserLastNameTextBox.Focus();

                return false;
            }

            if (RolesComboBox.SelectedIndex == -1)
            {
                notifier.ShowWarning("Ролята е задължителна!");
                RolesComboBox.Focus();

                return false;
            }

            return true;
        }
        
        private bool IsNameValid(string name)
        {
            string namePattern = @"^[A-Za-z]+$";

            return Regex.IsMatch(name, namePattern);
        }

        private bool IsPasswordValid(string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                return false;
            }

            bool isLengthValid = password.Length >= 8;
            bool hasSpecialSymbol = password.Any(c => !char.IsLetterOrDigit(c));
            bool hasUppercase = password.Any(c => char.IsUpper(c));
            bool hasNumber = password.Any(c => char.IsDigit(c));

            return isLengthValid && hasSpecialSymbol && hasUppercase && hasNumber;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

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
    }
}
