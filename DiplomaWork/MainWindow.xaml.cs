using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace DiplomaWork
{
    public partial class MainWindow : Controllers.CustomWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Сигурни ли сте, че искате да излезете от профила си?", "Излизане", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                if (File.Exists("login.dat"))
                {
                    File.Delete("login.dat");
                }

                this.Close();

                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
            }
        }
    }
}
