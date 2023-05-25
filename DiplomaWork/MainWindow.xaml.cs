using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using DiplomaWork.Views;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;

namespace DiplomaWork
{
    public partial class MainWindow : Controllers.CustomWindow
    {

        Dictionary<string, object> loadedViews = new Dictionary<string, object>();

        public MainWindow()
        {
            InitializeComponent();

            LaboratoryDayView laboratoryDayView = new LaboratoryDayView();
            ContentControl.Content = laboratoryDayView;
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

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            bool? Result = new CustomMessageBox("Сигурни ли сте, че искате да излезете от профила си?", "Излизане").ShowDialog();

            if (Result.Value)
            {
                if (File.Exists("login.dat"))
                {
                    File.Delete("login.dat");
                }

                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();

                this.Close();
            }
        }

        private void LaboratoryDay_Click(object sender, RoutedEventArgs e)
        {
            string viewName = "LaboratoryDayView";
            object view;

            if (App.UserPermissions.Contains("permissions.all") || App.UserPermissions.Contains("permissions.show_days"))
            {
                if (loadedViews.TryGetValue(viewName, out view))
                {
                    ContentControl.Content = view;
                }
                else
                {
                    view = new LaboratoryDayView();
                    loadedViews.Add(viewName, view);
                    ContentControl.Content = view;
                }
            } 
            else
            {
                notifier.ShowWarning("Нямате нужните права!");
            }            
        }
        
        private void LaboratoryMonth_Click(object sender, RoutedEventArgs e)
        {
            string viewName = "LaboratoryMonthView";
            object view;

            if (App.UserPermissions.Contains("permissions.all") || App.UserPermissions.Contains("permissions.show_months"))
            {
                if (loadedViews.TryGetValue(viewName, out view))
                {
                    ContentControl.Content = view;
                }
                else
                {
                    view = new LaboratoryMonthView();
                    loadedViews.Add(viewName, view);
                    ContentControl.Content = view;
                }
            }
            else
            {
                notifier.ShowWarning("Нямате нужните права!");
            }
        }
        
        private void LaboratoryReports_Click(object sender, RoutedEventArgs e)
        {
            string viewName = "LaboratoryReportsView";
            object view;


            if (loadedViews.TryGetValue(viewName, out view))
            {
                ContentControl.Content = view;
            }
            else
            {
                view = new LaboratoryReportsView();
                loadedViews.Add(viewName, view);
                ContentControl.Content = view;
            }
        }
        
        private void LaboratorySettings_Click(object sender, RoutedEventArgs e)
        {
            string viewName = "LaboratorySettingsView";
            object view;


            if (loadedViews.TryGetValue(viewName, out view))
            {
                ContentControl.Content = view;
            }
            else
            {
                view = new LaboratorySettingsView();
                loadedViews.Add(viewName, view);
                ContentControl.Content = view;
            }
        }
    }
}
