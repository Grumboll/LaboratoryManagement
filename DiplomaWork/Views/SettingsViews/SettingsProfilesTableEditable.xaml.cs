using DiplomaWork.DataItems;
using DiplomaWork.Models;
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
using ToastNotifications.Position;
using ToastNotifications.Messages;

namespace DiplomaWork.Views.SettingsViews
{
    public partial class SettingsProfilesTableEditable : UserControl
    {
        private ObservableCollection<ProfileSettingsItem> DataItems { get; set; } = new ObservableCollection<ProfileSettingsItem>();
        private int pageSize = 25;
        private int currentPage = 1;
        private string filterText = string.Empty;
        private int totalRecords = -1;

        public SettingsProfilesTableEditable()
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

            DataItems = new ObservableCollection<ProfileSettingsItem>(ProfileService.getProfilesListFilteredByNamePaginated(startIndex, pageSize, filterText));

            SettingsProfileEditableDataGrid.ItemsSource = DataItems;
        }

        private void ProfilesPagePrevious_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadDataForPage(currentPage, filterText);
            }
        }

        private void ProfilesPageNext_Click(object sender, RoutedEventArgs e)
        {
            if (totalRecords < 0)
            {
                totalRecords = ProfileService.getTotalProfilesCount();
            }

            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadDataForPage(currentPage, filterText);
            }
        }

        private void ProfilNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterText = ProfilNameTextBox.Text;
            currentPage = 1;

            LoadDataForPage(currentPage, filterText);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button.DataContext as ProfileSettingsItem;

            // Get the data source of the DataGrid
            var source = SettingsProfileEditableDataGrid.ItemsSource as ObservableCollection<ProfileSettingsItem>;

            if (source.Contains(item))
            {
                bool? Result = new CustomMessageBox("Сигурни ли сте, че искате да изтриете профил с ИД: " + item.Id + "?", "Изтриване на профил").ShowDialog();

                if (Result.Value)
                {
                    using (var dbContext = new laboratory_2023Context())
                    {
                        var rowToDelete = dbContext.ProfileHasLengthsPerimeters.FirstOrDefault(x => x.Id == item.Id);

                       // rowToDelete.DeletedAt = DateTime.Now;
                        source.Remove((ProfileSettingsItem)item);
                    }
                }
            }
        }
        
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ProfilesCreateNew_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            SettingsProfileEditableScrollViewer.ScrollToVerticalOffset(SettingsProfileEditableScrollViewer.VerticalOffset - e.Delta / 3);
        }
    }
}
