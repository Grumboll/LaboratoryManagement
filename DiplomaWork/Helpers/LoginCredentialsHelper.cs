using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace DiplomaWork.Helpers
{
    public static class LoginCredentialsHelper
    {
        private static readonly byte[] entropy = Encoding.Unicode.GetBytes("your entropy string here");

        public static void SaveLoginCredentials(string username, string password)
        {
            try
            {
                byte[] encryptedUsername = ProtectedData.Protect(Encoding.UTF8.GetBytes(username), entropy, DataProtectionScope.CurrentUser);
                byte[] encryptedPassword = ProtectedData.Protect(Encoding.UTF8.GetBytes(password), entropy, DataProtectionScope.CurrentUser);
                File.WriteAllBytes("login.dat", encryptedUsername.Concat(encryptedPassword).ToArray());
            }
            catch (Exception)
            {
                MessageBox.Show("Грешка при запазване на данните!");
            }
        }

        public static (string Username, string Password) TryLoadLoginCredentials()
        {
            try
            {
                byte[] encryptedData = File.ReadAllBytes("login.dat");
                byte[] encryptedUsername = encryptedData.Take(encryptedData.Length / 2).ToArray();
                byte[] encryptedPassword = encryptedData.Skip(encryptedData.Length / 2).ToArray();

                string username = Encoding.UTF8.GetString(ProtectedData.Unprotect(encryptedUsername, entropy, DataProtectionScope.CurrentUser));
                string password = Encoding.UTF8.GetString(ProtectedData.Unprotect(encryptedPassword, entropy, DataProtectionScope.CurrentUser));

                return (username, password);
            }
            catch (FileNotFoundException)
            {
                return (null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Грешка при зареждане на потребителско име и парола! Моля въведете данните отново.");
                return (null, null);
            }
        }
    }
}
