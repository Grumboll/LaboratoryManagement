using DiplomaWork.Models;
using DiplomaWork.DataItems;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Serilog;
using DiplomaWork.Services;
using System.Xml.Linq;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;

namespace DiplomaWork.Views
{
    public partial class LaboratoryDayView : UserControl
    {
        public ObservableCollection<LaboratoryDayItem> DataItems { get; set; } = new ObservableCollection<LaboratoryDayItem>();
        public List<uint?> LaboratoryDayIds { get; set; } = new List<uint?> {};

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

            LaboratoryDayDataGrid.ItemsSource = DataItems;
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
                    item.Id = null;
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
            List<LaboratoryDayItem> items = LaboratoryDayService.getLaboratoryDayItems(context, now);

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

                if (DataItems.Count == 0)
                {
                    DataItems.Add(new LaboratoryDayItem());
                }

                LaboratoryDayDataGrid.ItemsSource = DataItems;
                context.Dispose();
            }
        }

        private void LaboratoryDaySave_Click(object sender, RoutedEventArgs e)
        {
            var context = new laboratory_2023Context();
            List <LaboratoryDay> newlyCreatedDayItems = new List<LaboratoryDay>();
            if (App.UserPermissions.Contains("permissions.all") || App.UserPermissions.Contains("permissions.create_days") || 
                App.UserPermissions.Contains("permissions.edit_days") || App.UserPermissions.Contains("permissions.delete_days"))
            {
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
                                MonthId = (uint)LaboratoryDayDatePicker.SelectedDate.Value.Month,
                                Year = (ushort)LaboratoryDayDatePicker.SelectedDate.Value.Year,
                                ProfileId = (uint)item.ProfileId,
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

                            //Needed so we can later update LaboratoryDayIds, this makes it possible  to delete an item that was just created
                            newlyCreatedDayItems.Add(newLaboratoryDay);

                            continue;
                        }

                        //In DataItems if a row has been deleted it won't get needlessly updated since it won't be present in the ObservableCollection
                        var rowToUpdate = context.LaboratoryDays.FirstOrDefault(row => row.Id == item.Id);

                        if (item.Id == null && DataItems.Count == 1)
                        {
                            rowToUpdate = context.LaboratoryDays.FirstOrDefault(row => row.Id == LaboratoryDayIds[0]);
                            rowToUpdate.DeletedAt = DateTime.Now;
                        }
                        else
                        {
                            rowToUpdate.ProfileId = (uint)item.ProfileId;
                            rowToUpdate.MetersSquaredPerSample = decimal.Parse(item.MetersSquaredPerSample);
                            rowToUpdate.PaintedSamplesCount = uint.Parse(item.PaintedSamplesCount);
                            rowToUpdate.PaintedMetersSquared = decimal.Parse(item.PaintedMetersSquared);
                            rowToUpdate.KilogramsPerMeter = item.KilogramsPerMeter != null ? decimal.Parse(item.KilogramsPerMeter) : null;
                            rowToUpdate.UpdatedAt = DateTime.Now;
                            rowToUpdate.UpdatedBy = App.CurrentUser.Id;
                        }
                    }

                    //Get the difference between LaboratoryDayIds, which is set when loading DataItems, and DataItems that has only ids to be updated
                    List<uint?> idsToDelete = LaboratoryDayIds.Except(DataItems.Select(x => x.Id)).ToList();

                    foreach (uint id in idsToDelete)
                    {
                        var rowToDelete = context.LaboratoryDays.FirstOrDefault(x => x.Id == id);

                        //Soft Delete the item from the database
                        rowToDelete.DeletedAt = DateTime.Now;
                    }

                    context.SaveChanges();
                    notifier.ShowSuccess("Успешно запазени промени!");

                    LaboratoryDayIds.AddRange(newlyCreatedDayItems.Select(o => (uint?)o.Id));

                    DataItems = new ObservableCollection<LaboratoryDayItem>(getLaboratoryDayItemsList(context, DateTime.Now.Date));
                    LaboratoryDayDataGrid.ItemsSource = null;
                    LaboratoryDayDataGrid.ItemsSource = DataItems;
                }
                catch (Exception ex)
                {
                    context.Database.CurrentTransaction?.Rollback();

                    Log.Error(ex, "An error occurred while saving changes.");
                    notifier.ShowError("Грешка при запазване на промените!");
                }
            }
            else
            {
                notifier.ShowWarning("Нямате нужните права, за да изпълните тази операция!");
                DataItems = new ObservableCollection<LaboratoryDayItem>(getLaboratoryDayItemsList(context, DateTime.Now.Date));
                LaboratoryDayDataGrid.ItemsSource = null;
                LaboratoryDayDataGrid.ItemsSource = DataItems;
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

                var metersSquaredPerSample = editedRow.MetersSquaredPerSample;
                decimal metersSquaredPerSampleDecimal;
                decimal paintedSamplesCountDecimal;
                var paintedSamplesCount = editedRow.PaintedSamplesCount;

                if (editedRow.MetersSquaredPerSample != null && editedRow.PaintedSamplesCount != null)
                {
                    if (decimal.TryParse(editedRow.MetersSquaredPerSample, out metersSquaredPerSampleDecimal) && decimal.TryParse(editedRow.PaintedSamplesCount, out paintedSamplesCountDecimal))
                    {
                        editedRow.PaintedMetersSquared = (metersSquaredPerSampleDecimal * paintedSamplesCountDecimal).ToString();
                        
                        LaboratoryDayDataGrid.ItemsSource = null;
                        LaboratoryDayDataGrid.ItemsSource = DataItems;
                    }
                }

            }
        }
    }
}