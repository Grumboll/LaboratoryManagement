using DiplomaWork.Models;
using System.Security.Cryptography;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DiplomaWork.Helpers;
using System.Data.SqlTypes;

namespace DiplomaWork
{
    public partial class LoginWindow : Controllers.CustomWindow
    {
        private laboratory_2023Context _dbContext;

        private User user;

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

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UserNameTextBox.Text;
            string password = PasswordBox.Password;

            loginUserThroughForm(username, password);
        }

        // Dispose the DbContext
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _dbContext.Dispose();
        }

        private void loginUserThroughForm(string username, string password)
        {
            (byte[] hashedPassword, byte[] hashedInputPassword) = getHashedPasswordTuple(username, password);

            if (user != null)
            {
                ErrorLabel.Visibility = Visibility.Hidden;

                // Compare the resulting hash with the hash stored in the database
                if (hashedPassword.SequenceEqual(hashedInputPassword))
                {
                    // Authentication successful, open Main Window
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();

                    if (RememberMeCheckBox.IsChecked ?? false)
                    {
                        LoginCredentialsHelper.SaveLoginCredentials(username, password);
                    }

                    App.CurrentUser = user;

                    // Close the current window
                    this.Close();
                }
                else
                {
                    InitializeComponent();
                    ErrorLabel.Content = "Грешни потребителски данни!";
                    ErrorLabel.Visibility = Visibility.Visible;
                }

            }
            else
            {
                InitializeComponent();
                ErrorLabel.Content = "Потребителското име не беше открито!";
                ErrorLabel.Visibility = Visibility.Visible;
            }
        }

        private void preLoginUser(string username, string password)
        {
            (byte[] hashedPassword, byte[] hashedInputPassword) = getHashedPasswordTuple(username, password);

            if (user != null)
            {

                // Compare the resulting hash with the hash stored in the database
                if (hashedPassword.SequenceEqual(hashedInputPassword))
                {
                    // Authentication successful, open Main Window
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();

                    App.CurrentUser = user;

                    this.Close();
                }
                else
                {
                    ErrorLabel.Content = "Грешни потребителски данни!";
                    ErrorLabel.Visibility = Visibility.Visible;
                }

            }
            else
            {
                ErrorLabel.Content = "Потребителското име не беше открито!";
                ErrorLabel.Visibility = Visibility.Visible;
            }
        }

        private (byte[] hashedPassword, byte[] HashedInputPassword) getHashedPasswordTuple(string username, string password)
        {
            // Check if the user exists
            user = _dbContext.Users.FirstOrDefault(u => u.Username == username);

            if (user != null)
            {
                string saltString = user.PasswordSalt;
                string hashString = user.Password;

                // Convert salt and hash from Base64 strings to byte arrays
                byte[] salt = Convert.FromBase64String(saltString);
                byte[] hash = Convert.FromBase64String(hashString);
                byte[] inputHash = Encoding.UTF8.GetBytes(password);

                byte[] passwordBytes = Encoding.UTF8.GetBytes(hash + Convert.ToBase64String(salt));
                byte[] inputPasswordBytes = Encoding.UTF8.GetBytes(inputHash + Convert.ToBase64String(salt));

                byte[] hashedPassword;
                byte[] hashedInputPassword;

                using (var sha256 = SHA256.Create())
                {
                    hashedPassword = sha256.ComputeHash(passwordBytes);
                    hashedInputPassword = sha256.ComputeHash(inputPasswordBytes);
                }


                return (hashedPassword, hashedInputPassword);
            }
            else
            {
                return (null, null);
            }
            
        }

    }


}
