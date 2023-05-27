using DiplomaWork.DataItems;
using DiplomaWork.Models;
using DiplomaWork.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DiplomaWork.Views.SettingsViews
{
    public partial class SettingsProfilesTableReadOnly : UserControl
    {
        private ObservableCollection<ProfileSettingsItem> DataItems { get; set; } = new ObservableCollection<ProfileSettingsItem>();
        private int pageSize = 25;
        private int currentPage = 1;
        private string filterText = string.Empty;
        private int totalRecords = -1;

        public SettingsProfilesTableReadOnly()
        {
            InitializeComponent();

            LoadDataForPage(currentPage, filterText);
        }

        private void LoadDataForPage(int pageNumber, string filterText)
        {
            int startIndex = (pageNumber - 1) * pageSize;

            DataItems = new ObservableCollection<ProfileSettingsItem>(ProfileService.getProfilesListFilteredByNamePaginated(startIndex, pageSize, filterText));

            SettingsProfileReadOnlyDataGrid.ItemsSource = DataItems;
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

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            SettingsProfileReadOnlyScrollViewer.ScrollToVerticalOffset(SettingsProfileReadOnlyScrollViewer.VerticalOffset - e.Delta / 3);
        }
    }
}
