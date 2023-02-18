using DiplomaWork.Models;
using DiplomaWork.Properties;
using System.Security.Cryptography;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO;

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
            string password = PasswordBox.Password;

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

                // Compare the resulting hash with the hash stored in the database
                if (hashedPassword.SequenceEqual(hashedInputPassword))
                {
                    // Authentication successful, open Main Window
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();

                    if(RememberMeCheckBox.IsChecked ?? false)
                    {
                        SaveLoginCredentials(username, password);
                    }

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

        public static void SaveLoginCredentials(string username, string password)
        {
            // Convert the username and password to bytes
            byte[] usernameBytes = Encoding.UTF8.GetBytes(username);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Generate a random salt value
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Derive a key from the password and salt using PBKDF2
            byte[] key = new Rfc2898DeriveBytes(password, salt, 10000).GetBytes(32);

            // Encrypt the username and password with the key
            byte[] encryptedUsername = ProtectedData.Protect(usernameBytes, key, DataProtectionScope.LocalMachine);
            byte[] encryptedPassword = ProtectedData.Protect(passwordBytes, key, DataProtectionScope.LocalMachine);

            // Save the encrypted username and password and salt to a file
            File.WriteAllBytes("login.dat", encryptedUsername.Concat(encryptedPassword).Concat(salt).ToArray());
        }

        public static bool TryLoadLoginCredentials(out string username, out string password)
        {
            username = null;
            password = null;

            if (!File.Exists("login.dat"))
            {
                return false;
            }

            byte[] encryptedData = File.ReadAllBytes("login.dat");
            if (encryptedData.Length < 48) // username (16 bytes) + password (16 bytes) + salt (16 bytes)
            {
                return false;
            }

            byte[] encryptedUsername = encryptedData.Take(16).ToArray();
            byte[] encryptedPassword = encryptedData.Skip(16).Take(16).ToArray();
            byte[] salt = encryptedData.Skip(32).Take(16).ToArray();

            string savedPassword = Properties.Settings.Default.password;

            // Derive a key from the password and salt using PBKDF2
            byte[] key = new Rfc2898DeriveBytes(savedPassword, salt, 10000).GetBytes(32);

            // Decrypt the username and password with the key
            byte[] usernameBytes = ProtectedData.Unprotect(encryptedUsername, key, DataProtectionScope.LocalMachine);
            byte[] passwordBytes = ProtectedData.Unprotect(encryptedPassword, key, DataProtectionScope.LocalMachine);

            // Convert the decrypted bytes to strings
            username = Encoding.UTF8.GetString(usernameBytes);
            password = Encoding.UTF8.GetString(passwordBytes);

            return true;
        }
    }


}
