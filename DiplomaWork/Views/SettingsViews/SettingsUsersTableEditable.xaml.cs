using DiplomaWork.DataItems;
using DiplomaWork.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace DiplomaWork.Views.SettingsViews
{
    public partial class SettingsUsersTableEditable : UserControl
    {
        private ObservableCollection<UserItem> DataItems { get; set; } = new ObservableCollection<UserItem>();
        private int pageSize = 25;
        private int currentPage = 1;
        private string filterText = string.Empty;
        private int totalRecords = -1;
        private bool isUserModalOpen = false;

        public SettingsUsersTableEditable()
        {
            InitializeComponent();

            LoadDataForPage(currentPage, filterText);
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

        private void LoadDataForPage(int pageNumber, string filterText)
        {
            int startIndex = (pageNumber - 1) * pageSize;

            DataItems = new ObservableCollection<UserItem>(UserService.getUsersListFilteredByNamePaginated(startIndex, pageSize, filterText));

            SettingsUserEditableDataGrid.ItemsSource = DataItems;
        }

        private void UsersPagePrevious_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadDataForPage(currentPage, filterText);
            }
        }

        private void UsersPageNext_Click(object sender, RoutedEventArgs e)
        {
            if (totalRecords < 0)
            {
                totalRecords = UserService.getTotalUsersCount();
            }

            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadDataForPage(currentPage, filterText);
            }
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterText = UsernameTextBox.Text;
            currentPage = 1;

            LoadDataForPage(currentPage, filterText);
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            SettingsUserEditableScrollViewer.ScrollToVerticalOffset(SettingsUserEditableScrollViewer.VerticalOffset - e.Delta / 3);
        }

        private void LockUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.UserPermissions.Contains("permissions.all") || App.UserPermissions.Contains("permissions.lock_users"))
            {
                var button = sender as Button;
                var item = button.DataContext as UserItem;

                var source = SettingsUserEditableDataGrid.ItemsSource as ObservableCollection<UserItem>;

                if (source.Contains(item))
                {
                    bool? Result = new CustomMessageBox("Сигурни ли сте, че искате да \nотключите/заключите " + item.Username + "?", "Отключване/Заключване").ShowDialog();

                    if (Result.Value)
                    {
                        UserService.lockUnlockUserById(item.Id);
                        LoadDataForPage(currentPage, filterText);
                    }
                }
            }
            else
            {
                notifier.ShowWarning("Нямате нужните права!");
            }
        }

        private void ResetPassButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.UserPermissions.Contains("permissions.all") || App.UserPermissions.Contains("permissions.lock_users"))
            {
                var button = sender as Button;
                var item = button.DataContext as UserItem;

                var source = SettingsUserEditableDataGrid.ItemsSource as ObservableCollection<UserItem>;


                if (source.Contains(item))
                {
                    bool? Result = new CustomMessageBox("Сигурни ли сте, че искате да принудите " + item.Username + "\n да промени паролата си?", "Промяна на парола").ShowDialog();

                    if (Result.Value)
                    {
                        UserService.forcePasswordResetToUserById(item.Id);
                        LoadDataForPage(currentPage, filterText);
                    }
                }
            }
            else
            {
                notifier.ShowWarning("Нямате нужните права!");
            }
        }
        
        private void UsersCreateNew_Click(object sender, RoutedEventArgs e)
        {
            if (!isUserModalOpen)
            {
                isUserModalOpen = true;

                UserModal userModal = new UserModal();
                userModal.Closed += UserModal_Closed;
                userModal.Show();
            }
            
        }

        private void UserModal_Closed(object sender, EventArgs e)
        {
            LoadDataForPage(currentPage, filterText);
            isUserModalOpen = false;
        }
    }
}
