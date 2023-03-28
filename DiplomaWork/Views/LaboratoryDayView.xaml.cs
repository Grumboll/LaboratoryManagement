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
using System.Reflection;
using System.Xml.Linq;
using System.Windows.Markup;

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
                DataContext = new ViewModel();

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
                    item.ProfileId = null;
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
                        ProfileId = ld.Profile.Id,
                        ProfileName = ld.Profile.Name,
                        ProfileLength = ld.Profile.ProfileHasLengthsPerimeters.FirstOrDefault().Length.ToString(),
                        ProfilePerimeter = ld.Profile.ProfileHasLengthsPerimeters.FirstOrDefault().Perimeter.ToString(),
                        MetersSquaredPerSample = ld.MetersSquaredPerSample.ToString(),
                        PaintedSamplesCount = ld.PaintedSamplesCount.ToString(),
                        PaintedMetersSquared = ld.PaintedMetersSquared.ToString(),
                        KilogramsPerMeter = ld.KilogramsPerMeter.ToString()
                    }).ToList();

            foreach (LaboratoryDayItem item in items)
            {
                item.ProfileLength = item.ProfileLength.TrimEnd('0').TrimEnd('.');
                item.ProfilePerimeter = item.ProfilePerimeter.TrimEnd('0').TrimEnd('.');
                item.MetersSquaredPerSample = item.MetersSquaredPerSample.TrimEnd('0').TrimEnd('.');
                item.PaintedMetersSquared = item.PaintedMetersSquared.TrimEnd('0').TrimEnd('.');
                item.KilogramsPerMeter = item.KilogramsPerMeter != null ? item.KilogramsPerMeter.TrimEnd('0').TrimEnd('.') : null;
            }

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
                            MonthId = (uint) LaboratoryDayDatePicker.SelectedDate.Value.Month,
                            ProfileId = (uint) item.ProfileId,
                            MetersSquaredPerSample = decimal.Parse(item.MetersSquaredPerSample),
                            PaintedSamplesCount = uint.Parse(item.PaintedSamplesCount),
                            PaintedMetersSquared = decimal.Parse(item.PaintedMetersSquared),
                            KilogramsPerMeter = item.KilogramsPerMeter != null ? decimal.Parse(item.KilogramsPerMeter) : null,
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

                    rowToUpdate.ProfileId = (uint) item.ProfileId;
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

            context.Dispose();
        }

        private void DataGridComboBox_DropDownClosed(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedItem = comboBox.SelectedItem as ProfileItem;
            var item = comboBox.DataContext as LaboratoryDayItem;

            if (selectedItem == null)
            {
                return;
            }

            // Get the data source of the DataGrid
            var source = LaboratoryDayDataGrid.ItemsSource as ObservableCollection<LaboratoryDayItem>;
            var context = new laboratory_2023Context();


            if (source.Contains(item))
            {
                var profileId = selectedItem.Id;
                var profileHasLengthsPerimeter = context.ProfileHasLengthsPerimeters.FirstOrDefault(p => p.ProfileId == profileId);

                if (profileHasLengthsPerimeter != null)
                {
                    var profileLength = profileHasLengthsPerimeter.Length;
                    var profilePerimeter = profileHasLengthsPerimeter.Perimeter;
                    item.ProfileLength = profileLength.ToString().TrimEnd('0').TrimEnd('.');
                    item.ProfilePerimeter = profilePerimeter.ToString().TrimEnd('0').TrimEnd('.');
                    item.MetersSquaredPerSample = ((profileLength * profilePerimeter) / 1000000).ToString().TrimEnd('0').TrimEnd('.');

                    LaboratoryDayDataGrid.ItemsSource = null;
                    LaboratoryDayDataGrid.ItemsSource = DataItems;
                }
            }

            context.Dispose();
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataGridColumn editedColumn = e.Column;
            int editedColumnIndex = editedColumn.DisplayIndex;

            if (editedColumnIndex == 3 || editedColumnIndex == 4)
            {
                LaboratoryDayItem editedRow = (LaboratoryDayItem) e.Row.Item;

                if (editedRow.MetersSquaredPerSample != null && editedRow.PaintedSamplesCount != null)
                {
                    editedRow.PaintedMetersSquared = (decimal.Parse(editedRow.MetersSquaredPerSample) * decimal.Parse(editedRow.PaintedSamplesCount)).ToString();

                    LaboratoryDayDataGrid.ItemsSource = null;
                    LaboratoryDayDataGrid.ItemsSource = DataItems;
                }

            }
        }
    }
}