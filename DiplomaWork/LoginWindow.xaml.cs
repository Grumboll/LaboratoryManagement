using DiplomaWork.Models;
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
            string password = PasswordBox.Password;

            // Check if the user exists
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Open another window
                MainWindow otherWindow = new MainWindow();
                otherWindow.Show();

                // Close the current window
                this.Close();
            }
            else
            {
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
