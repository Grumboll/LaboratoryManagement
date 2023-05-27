using DiplomaWork.DataItems;
using DiplomaWork.Models;
using DiplomaWork.Services;
using iText.Kernel.Geom;
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

namespace DiplomaWork.Views.SettingsViews
{
    public partial class SettingsUsersTableReadOnly : UserControl
    {
        private ObservableCollection<UserItem> DataItems { get; set; } = new ObservableCollection<UserItem>();
        private int pageSize = 25;
        private int currentPage = 1;
        private string filterText = string.Empty;
        private int totalRecords = -1;

        public SettingsUsersTableReadOnly()
        {
            InitializeComponent();

            LoadDataForPage(currentPage, filterText);
        }

        private void LoadDataForPage(int pageNumber, string filterText)
        {
            int startIndex = (pageNumber - 1) * pageSize;

            DataItems = new ObservableCollection<UserItem>(UserService.getUsersListFilteredByNamePaginated(startIndex, pageSize, filterText));

            SettingsUserReadOnlyDataGrid.ItemsSource = DataItems;
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
            SettingsUserReadOnlyScrollViewer.ScrollToVerticalOffset(SettingsUserReadOnlyScrollViewer.VerticalOffset - e.Delta / 3);
        }
    }
}
