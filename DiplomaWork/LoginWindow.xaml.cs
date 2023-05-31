using DiplomaWork.Models;
using System.Security.Cryptography;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DiplomaWork.Helpers;
using System.Data.SqlTypes;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using DiplomaWork.DataItems;

namespace DiplomaWork
{
    public partial class LoginWindow : Controllers.CustomWindow
    {
        private laboratory_2023Context _dbContext;

        private User user;

        private bool isEmailModalOpen = false;

        public LoginWindow()
        {
            // Initialize the DbContext
            _dbContext = new laboratory_2023Context();

            (string username, string password) = LoginCredentialsHelper.TryLoadLoginCredentials();

            if (username == null || password == null)
            {
                InitializeComponent();
            }
            else
            {
                preLoginUser(username, password);
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

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UserNameTextBox.Text;
            string password = PasswordBox.Password;

            loginUserThroughForm(username, password);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _dbContext.Dispose();
        }

        private void loginUserThroughForm(string username, string password)
        {
            (string hashedPassword, string hashedInputPassword) = getHashedPasswordTuple(username, password);

            if (user != null)
            {

                if (hashedPassword == null || hashedInputPassword == null)
                {
                    notifier.ShowError("Грешни потребителски данни!");
                }
                else
                {
                    // Compare the resulting hash with the hash stored in the database
                    if (hashedPassword == hashedInputPassword)
                    {
                        App.CurrentUser = user;
                        setUserPermissions(user.Id);

                        // Authentication successful, open Main Window
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();

                        if (RememberMeCheckBox.IsChecked ?? false)
                        {
                            LoginCredentialsHelper.SaveLoginCredentials(username, password);
                        }

                        // Close the current window
                        this.Close();
                    }
                    else
                    {
                        notifier.ShowError("Грешни потребителски данни!");
                    }
                }
                                
            }
            else
            {
                notifier.ShowError("Потребителското име не беше открито!");
            }
        }

        private void preLoginUser(string username, string password)
        {
            (string hashedPassword, string hashedInputPassword) = getHashedPasswordTuple(username, password);

            if (user != null)
            {
                if (hashedPassword == null || hashedInputPassword == null)
                {
                    InitializeComponent();
                    this.Show();
                    notifier.ShowError("Грешни потребителски данни!");
                }
                else
                {
                    // Compare the resulting hash with the hash stored in the database
                    if (hashedPassword == hashedInputPassword)
                    {
                        App.CurrentUser = user;
                        setUserPermissions(user.Id);

                        // Authentication successful, open Main Window
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();

                        this.Close();
                    }
                    else
                    {
                        InitializeComponent();
                        this.Show();
                        notifier.ShowError("Грешни потребителски данни!");
                    }
                }

            }
            else
            {
                InitializeComponent();
                this.Show();
                notifier.ShowError("Потребителското име не беше открито!");
            }
        }

        private (string hashedPassword, string HashedInputPassword) getHashedPasswordTuple(string username, string password)
        {
            // Check if the user exists
            try
            {
                user = _dbContext.Users.FirstOrDefault(u => u.Username == username);

                if (user != null)
                {
                    string dbSaltString = user.PasswordSalt;
                    string dbHashString = user.Password;

                    byte[] inputHash;
                    using (var sha256 = SHA256.Create())
                    {
                        // Concatenate the salt with the user inputted password and hash the resulting string
                        inputHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + dbSaltString));
                    }

                    // Convert the hash to a Base64-encoded string
                    string inputHashString = Convert.ToBase64String(inputHash);
                    dbHashString = Convert.ToBase64String(Convert.FromBase64String(dbHashString));

                    return (dbHashString, inputHashString);
                }
                else
                {
                    return (null, null);
                }
            } catch(Exception e)
            {
                return (null, null);
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string username = UserNameTextBox.Text;
                string password = PasswordBox.Password;

                loginUserThroughForm(username, password);
            }
        }

        private void setUserPermissions(uint id)
        {
            App.UserPermissions = _dbContext.Permissions
            .Where(p => _dbContext.RoleHasPermissions
                .Where(rp => _dbContext.UserHasRoles
                    .Where(ur => ur.UserId == id)
                    .Select(ur => ur.RoleId)
                    .Contains(rp.RoleId))
                .Select(rp => rp.PermissionId)
                .Contains(p.Id))
            .Select(p => p.Slug)
            .ToList();
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

        private void ForgottenPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isEmailModalOpen)
            {
                isEmailModalOpen = true;

                EmailModal emailModal = new EmailModal();
                emailModal.Closed += EmailModal_Closed;
                emailModal.Show();
            }
        }

        private void EmailModal_Closed(object sender, EventArgs e)
        {
            isEmailModalOpen = false;
        }
    }
}
