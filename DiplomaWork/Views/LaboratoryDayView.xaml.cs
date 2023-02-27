using DiplomaWork.Controllers;
using DiplomaWork.DataItems;
using System.Collections.ObjectModel;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiplomaWork.Views
{
    public partial class LaboratoryDayView : UserControl
    {
        public ObservableCollection<LaboratoryDayItem> DataItems { get; set; } = new ObservableCollection<LaboratoryDayItem>();

        public LaboratoryDayView()
        {
            InitializeComponent();

            //TODO: LOAD FROM DB AND ADD DATAITEMS ACCORDINGLY
            LaboratoryDayDataGrid.ItemsSource = DataItems;
        }

        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            DataItems.Add(new LaboratoryDayItem());
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            LaboratoryDayScrollViewer.ScrollToVerticalOffset(LaboratoryDayScrollViewer.VerticalOffset - e.Delta / 3);
        }
    }
}
