using DiplomaWork.Models;
using DiplomaWork.DataItems;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DiplomaWork.Views
{
    public partial class LaboratoryDayView : UserControl
    {
        public ObservableCollection<LaboratoryDayItem> DataItems { get; set; } = new ObservableCollection<LaboratoryDayItem>();

        public LaboratoryDayView()
        {
            InitializeComponent();

            DateTime now = DateTime.Now.Date;

            List<LaboratoryDayItem> laboratoryDayItems = new List<LaboratoryDayItem>();

            using (var context = new laboratory_2023Context())
            {
                var profiles = context.Profiles.Select(p => new { p.Id, p.Name }).ToList();

                DataGridComboBox.ItemsSource = profiles;
                DataGridComboBox.DisplayMemberPath = "Name";
                DataGridComboBox.SelectedValuePath = "Id";

                laboratoryDayItems = context.LaboratoryDays
                    .Where(ld => ld.Day == DateOnly.FromDateTime(now))
                    .Include(x => x.Profile)
                    .ThenInclude(x => x.ProfileHasLengthsPerimeters)
                    .Select(ld => new LaboratoryDayItem
                    {
                        Profile = ld.Profile.Id.ToString(),
                        ProfileLength = ld.Profile.ProfileHasLengthsPerimeters.FirstOrDefault().Length.ToString(),
                        ProfilePerimeter = ld.Profile.ProfileHasLengthsPerimeters.FirstOrDefault().Perimeter.ToString(),
                        MetersSquaredPerSample = ld.MetersSquaredPerSample.ToString(),
                        PaintedSamplesCount = ld.PaintedSamplesCount.ToString(),
                        PaintedMetersSquared = ld.PaintedMetersSquared.ToString(),
                        KilogramsPerMeter = ld.KilogramsPerMeter.ToString()
                    }).ToList();
             }

            LaboratoryDayDatePicker.SelectedDate = now;

            if (laboratoryDayItems.Count != 0)
            {
                DataItems = new ObservableCollection<LaboratoryDayItem>(laboratoryDayItems);
            }
            else
            {
                DataItems.Add(new LaboratoryDayItem());
            }

            LaboratoryDayDataGrid.ItemsSource = DataItems;
        }

        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            DataItems.Add(new LaboratoryDayItem());
        }
        
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button.DataContext as LaboratoryDayItem;

            // Get the data source of the DataGrid
            var source = LaboratoryDayDataGrid.ItemsSource as ObservableCollection<LaboratoryDayItem>;
            

            if (source.Contains(item))
            {
                if (source.Count == 1)
                {
                    item.Profile = null;
                    item.ProfilePerimeter = "";
                    item.ProfileLength = "";
                    item.KilogramsPerMeter = "";
                    item.MetersSquaredPerSample = "";
                    item.PaintedMetersSquared = "";
                    item.PaintedSamplesCount = "";

                    //Needed to update the datagrid properly
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        LaboratoryDayDataGrid.ItemsSource = null;
                        LaboratoryDayDataGrid.ItemsSource = source;
                    }));
                } 
                else
                {
                    source.Remove((LaboratoryDayItem)item);
                }
            }
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            LaboratoryDayScrollViewer.ScrollToVerticalOffset(LaboratoryDayScrollViewer.VerticalOffset - e.Delta / 3);
        }
    }
}
