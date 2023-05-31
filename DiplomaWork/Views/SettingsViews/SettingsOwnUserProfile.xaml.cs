using DiplomaWork.Models;
using DiplomaWork.Services;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using System.Text.RegularExpressions;

namespace DiplomaWork.Views.SettingsViews
{
    public partial class SettingsOwnUserProfile : UserControl
    {
        string passwordText = string.Empty;

        User user;

        public SettingsOwnUserProfile()
        {
            InitializeComponent();

            user = UserService.getUserById(App.CurrentUser.Id);

            UserNameTextBox.Text = user.Username;
            UserEMailTextBox.Text = user.EMail;
            DateTime? dateOfBirth = user.DateOfBirth.HasValue
                ? new DateTime(user.DateOfBirth.Value.Year, user.DateOfBirth.Value.Month, user.DateOfBirth.Value.Day)
                : (DateTime?)null;
            UserDateOfBirthDatePicker.SelectedDate = dateOfBirth;
            UserPhoneNumberTextBox.Text = user.PhoneNumber;
            UserFirstNameTextBox.Text = user.FirstName;
            UserMiddleNameTextBox.Text = user.MiddleName;
            UserLastNameTextBox.Text = user.LastName;
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

        private void OwnProfileSave_Click(object sender, RoutedEventArgs e)
        {
            if (validateFormData())
            {
                if (UserService.editUser(user, UserNameTextBox.Text, UserEMailTextBox.Text, DateOnly.FromDateTime(UserDateOfBirthDatePicker.SelectedDate.GetValueOrDefault()),
                    UserPhoneNumberTextBox.Text, passwordText, UserFirstNameTextBox.Text, UserMiddleNameTextBox.Text, UserLastNameTextBox.Text) == 1)
                {
                    notifier.ShowSuccess("Успешно редактирахте вашият профил.");
                }
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
            string middleName = UserMiddleNameTextBox.Text;
            string lastName = UserLastNameTextBox.Text;

            if (!string.IsNullOrEmpty(username))
            {
                if (username.Length < 3)
                {
                    notifier.ShowWarning("Потребителското име трябва да бъде поне 3 символа!");
                    UserNameTextBox.Focus();

                    return false;
                }
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
            else
            {
                notifier.ShowWarning("Имейлът е  задължително поле!");
                UserEMailTextBox.Focus();

                return false;
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

            if (!string.IsNullOrEmpty(password))
            {
                if (!UserService.IsPasswordValid(password, confirmPassword))
                {
                    notifier.ShowWarning("Паролите не са еднакви или са невалидни (Трябва да съдържат поне 8 символа, голяма буква, специален символ и число)!");

                    return false;
                }
            }

            passwordText = password;

            if (!string.IsNullOrEmpty(firstName))
            {
                if (!UserService.IsNameValid(firstName))
                {
                    notifier.ShowWarning("Името е невалидно!");
                    UserFirstNameTextBox.Focus();

                    return false;
                }
            }

            if (!string.IsNullOrEmpty(middleName))
            {
                if (!UserService.IsNameValid(middleName))
                {
                    notifier.ShowWarning("Името е невалидно!");
                    UserMiddleNameTextBox.Focus();

                    return false;
                }
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                if (!UserService.IsNameValid(lastName))
                {
                    notifier.ShowWarning("Името е невалидно!");
                    UserLastNameTextBox.Focus();

                    return false;
                }
            }

            return true;
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
