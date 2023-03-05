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
using System.Windows.Media;
using System.Windows.Media.Animation;
using Serilog;

namespace DiplomaWork.Views
{
    public partial class LaboratoryDayView : UserControl
    {
        public ObservableCollection<LaboratoryDayItem> DataItems { get; set; } = new ObservableCollection<LaboratoryDayItem>();
        public List<uint> LaboratoryDayIds { get; set; } = new List<uint> {};

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

                laboratoryDayItems = getLaboratoryDayItemsList(context, now);
                context.Dispose();
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

        private List<LaboratoryDayItem> getLaboratoryDayItemsList(laboratory_2023Context context, DateTime now)
        {
            List<LaboratoryDayItem> items = context.LaboratoryDays
                    .Where(ld => ld.Day == DateOnly.FromDateTime(now))
                    .Where(ld => ld.DeletedAt == null)
                    .Include(x => x.Profile)
                    .ThenInclude(x => x.ProfileHasLengthsPerimeters)
                    .Select(ld => new LaboratoryDayItem
                    {
                        Id = ld.Id,
                        Profile = ld.Profile.Id.ToString(),
                        ProfileLength = ld.Profile.ProfileHasLengthsPerimeters.FirstOrDefault().Length.ToString(),
                        ProfilePerimeter = ld.Profile.ProfileHasLengthsPerimeters.FirstOrDefault().Perimeter.ToString(),
                        MetersSquaredPerSample = ld.MetersSquaredPerSample.ToString(),
                        PaintedSamplesCount = ld.PaintedSamplesCount.ToString(),
                        PaintedMetersSquared = ld.PaintedMetersSquared.ToString(),
                        KilogramsPerMeter = ld.KilogramsPerMeter.ToString()
                    }).ToList();

            //Get ids so we can track which items where deleted during save operation
            LaboratoryDayIds = items.Select(x => x.Id).ToList();

            return items;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = (DateTime) LaboratoryDayDatePicker.SelectedDate;

            if (selectedDate != null)
            {
                var context = new laboratory_2023Context();
                DataItems = new ObservableCollection<LaboratoryDayItem>(getLaboratoryDayItemsList(context, selectedDate));
                LaboratoryDayDataGrid.ItemsSource = DataItems;
            }
        }

        private void LaboratoryDaySave_Click(object sender, RoutedEventArgs e)
        {
            var context = new laboratory_2023Context();

            try
            {
                foreach (LaboratoryDayItem item in DataItems)
                {
                    //DataItems contains everything from the database with their Ids, If there is a LaboratoryDayItem with no Id, create a new row in the database for it
                    if (item.Id == 0)
                    {
                        LaboratoryDay newLaboratoryDay = new LaboratoryDay
                        {
                            Day = DateOnly.FromDateTime((DateTime)LaboratoryDayDatePicker.SelectedDate),
                            ProfileId = uint.Parse(item.Profile),
                            MetersSquaredPerSample = decimal.Parse(item.MetersSquaredPerSample),
                            PaintedSamplesCount = uint.Parse(item.PaintedSamplesCount),
                            PaintedMetersSquared = decimal.Parse(item.PaintedMetersSquared),
                            KilogramsPerMeter = decimal.Parse(item.KilogramsPerMeter),
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            DeletedAt = null,
                            CreatedBy = App.CurrentUser.Id,
                            UpdatedBy = App.CurrentUser.Id,
                        };

                        context.LaboratoryDays.Add(newLaboratoryDay);
                        continue;
                    }

                    //In DataItems if a row has been deleted it won't get needlessly updated since it won't be present in the ObservableCollection
                    var rowToUpdate = context.LaboratoryDays.FirstOrDefault(row => row.Id == item.Id);

                    rowToUpdate.ProfileId = uint.Parse(item.Profile);
                    rowToUpdate.MetersSquaredPerSample = decimal.Parse(item.MetersSquaredPerSample);
                    rowToUpdate.PaintedSamplesCount = uint.Parse(item.PaintedSamplesCount);
                    rowToUpdate.PaintedMetersSquared = decimal.Parse(item.PaintedMetersSquared);
                    rowToUpdate.KilogramsPerMeter = decimal.Parse(item.KilogramsPerMeter);
                    rowToUpdate.UpdatedAt = DateTime.Now;
                    rowToUpdate.UpdatedBy = App.CurrentUser.Id;

                    //Get the difference between LaboratoryDayIds, which is set when loading DataItems, and DataItems that has only ids to be updated
                    List<uint> idsToDelete = LaboratoryDayIds.Except(DataItems.Select(x => x.Id)).ToList();

                    foreach (uint id in idsToDelete)
                    {
                        var rowToDelete = context.LaboratoryDays.FirstOrDefault(x => x.Id == id);

                        //Soft Delete the item from the database
                        rowToDelete.DeletedAt = DateTime.Now;
                    }
                }
                //270 error Грешка при запазване на промените!
                context.SaveChanges();
                SaveLabel.Content = "Успешно запазени промени!";
                SaveLabel.Foreground = new SolidColorBrush(Colors.Green);
                var storyboard = (Storyboard)FindResource("FadeInOut");
                storyboard.Stop();
                storyboard.Begin(SaveLabel);
            }
            catch(Exception ex)
            {
                context.Database.CurrentTransaction?.Rollback();

                Log.Error(ex, "An error occurred while saving changes.");

                SaveLabel.Content = "Грешка при запазване на промените!";
                SaveLabel.Foreground = new SolidColorBrush(Colors.Red);
                var storyboard = (Storyboard)FindResource("FadeInOut");
                storyboard.Stop();
                storyboard.Begin(SaveLabel);
            }
            
        }

    }
}
