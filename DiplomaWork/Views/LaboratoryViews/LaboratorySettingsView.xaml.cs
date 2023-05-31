using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DiplomaWork.DataItems;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DiplomaWork.Views.SettingsViews;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;

namespace DiplomaWork.Views
{
    public partial class LaboratorySettingsView : UserControl
    {
        Dictionary<string, object> loadedViews = new Dictionary<string, object>();

        public LaboratorySettingsView()
        {
            InitializeComponent();
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

        private void SettingsUsers_Click(object sender, RoutedEventArgs e)
        {
            string viewName = string.Empty;

            if (App.UserPermissions.Contains("permissions.all") || App.UserPermissions.Contains("permissions.create_users") || 
                App.UserPermissions.Contains("permissions.delete_users") || App.UserPermissions.Contains("permissions.lock_users"))
            {
                viewName = "SettingsUsersAdminView";
            } 
            else
            {
                viewName = "SettingsUsersView";
            }

            object view;


            if (loadedViews.TryGetValue(viewName, out view))
            {
                SettingsContentControl.Content = view;
            }
            else
            {
                if (App.UserPermissions.Contains("permissions.all") || App.UserPermissions.Contains("permissions.create_users") ||
                    App.UserPermissions.Contains("permissions.delete_users") || App.UserPermissions.Contains("permissions.lock_users"))
                {
                    view = new SettingsUsersTableEditable();
                } 
                else if (App.UserPermissions.Contains("permissions.show_users"))
                {
                    view = new SettingsUsersTableReadOnly();
                }
                else
                {
                    notifier.ShowWarning("Нямате нужните права!");
                    return;
                }

                loadedViews.Add(viewName, view);
                SettingsContentControl.Content = view;
            }
        }
        
        private void SettingsProfiles_Click(object sender, RoutedEventArgs e)
        {
            string viewName = string.Empty;

            if (App.UserPermissions.Contains("permissions.all") || App.UserPermissions.Contains("permissions.create_profiles") ||
                App.UserPermissions.Contains("permissions.edit_profiles") || App.UserPermissions.Contains("permissions.delete_profiles"))
            {
                viewName = "SettingsProfilesEditableView";
            } 
            else
            {
                viewName = "SettingsProfilesView";
            }

            object view;

            if (loadedViews.TryGetValue(viewName, out view))
            {
                SettingsContentControl.Content = view;
            }
            else
            {
                if (App.UserPermissions.Contains("permissions.all") || App.UserPermissions.Contains("permissions.create_profiles") ||
                    App.UserPermissions.Contains("permissions.edit_profiles") || App.UserPermissions.Contains("permissions.delete_profiles"))
                {
                    view = new SettingsProfilesTableEditable();
                } 
                else if (App.UserPermissions.Contains("permissions.show_profiles"))
                {
                    view = new SettingsProfilesTableReadOnly();
                }
                else
                {
                    notifier.ShowWarning("Нямате нужните права!");
                    return;
                }

                loadedViews.Add(viewName, view);
                SettingsContentControl.Content = view;
            }
        }
        
        private void SettingsOwnProfile_Click(object sender, RoutedEventArgs e)
        {
            SettingsContentControl.Content = new SettingsOwnUserProfile();
        }
    }
}
