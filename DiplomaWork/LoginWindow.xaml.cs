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
            (string hashedPassword, string hashedInputPassword) = getHashedPasswordTuple(username, password);

            if (user != null)
            {
                ErrorLabel.Visibility = Visibility.Hidden;

                if (hashedPassword == null || hashedInputPassword == null)
                {
                    ErrorLabel.Content = "Грешни потребителски данни!";
                    ErrorLabel.Visibility = Visibility.Visible;
                }
                else
                {
                    // Compare the resulting hash with the hash stored in the database
                    if (hashedPassword == hashedInputPassword)
                    {
                        // Authentication successful, open Main Window
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();

                        if (RememberMeCheckBox.IsChecked ?? false)
                        {
                            LoginCredentialsHelper.SaveLoginCredentials(username, password);
                        }

                        App.CurrentUser = user;
                        setUserPermissions(user.Id);

                        // Close the current window
                        this.Close();
                    }
                    else
                    {
                        ErrorLabel.Content = "Грешни потребителски данни!";
                        ErrorLabel.Visibility = Visibility.Visible;
                    }
                }
                                
            }
            else
            {
                ErrorLabel.Content = "Потребителското име не беше открито!";
                ErrorLabel.Visibility = Visibility.Visible;
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
                    ErrorLabel.Content = "Грешни потребителски данни!";
                    ErrorLabel.Visibility = Visibility.Visible;
                }
                else
                {
                    // Compare the resulting hash with the hash stored in the database
                    if (hashedPassword == hashedInputPassword)
                    {
                        // Authentication successful, open Main Window
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();

                        App.CurrentUser = user;
                        setUserPermissions(user.Id);

                        this.Close();
                    }
                    else
                    {
                        InitializeComponent();
                        ErrorLabel.Content = "Грешни потребителски данни!";
                        ErrorLabel.Visibility = Visibility.Visible;
                    }
                }

            }
            else
            {
                InitializeComponent();
                ErrorLabel.Content = "Потребителското име не беше открито!";
                ErrorLabel.Visibility = Visibility.Visible;
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
    }
}
