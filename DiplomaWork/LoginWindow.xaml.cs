using DiplomaWork.Models;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
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

namespace DiplomaWork
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Controllers.CustomWindow
    {
        private laboratory_2023Context _dbContext;

        public LoginWindow()
        {
            InitializeComponent();

            // Initialize the DbContext
            _dbContext = new laboratory_2023Context();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UserNameTextBox.Text;

            // Check if the user exists
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == username);

            if (user != null)
            {
                ErrorLabel.Visibility = Visibility.Hidden;

                string saltString = user.PasswordSalt;
                string hashString = user.Password;

                // Convert salt and hash from Base64 strings to byte arrays
                byte[] salt = Convert.FromBase64String(saltString);
                byte[] hash = Convert.FromBase64String(hashString);

                byte[] passwordBytes = Encoding.UTF8.GetBytes(hash + Convert.ToBase64String(salt));
                byte[] hashedPassword;
                using (var sha256 = SHA256.Create())
                {
                    hashedPassword = sha256.ComputeHash(passwordBytes);
                }

                // Compare the resulting hash with the hash stored in the database
                if (hashedPassword.SequenceEqual(hash))
                {
                    // Authentication successful, open Main Window
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();

                    // Close the current window
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
                ErrorLabel.Visibility= Visibility.Visible;
            }
        }

        // Dispose the DbContext
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _dbContext.Dispose();
        }
    }
}
