using DiplomaWork.DataItems;
using DiplomaWork.Models;
using DiplomaWork.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Input;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;

namespace DiplomaWork.Views
{
    public partial class LaboratoryMonthView : UserControl
    {
        public ObservableCollection<LaboratoryMonthItem> DataItems { get; set; } = new ObservableCollection<LaboratoryMonthItem>();
        public ObservableCollection<LaboratoryMonthChemicalItem> ChemicalDataItems { get; set; } = new ObservableCollection<LaboratoryMonthChemicalItem>();
        public List<uint?> LaboratoryMonthIds { get; set; } = new List<uint?> { };
        public List<uint?> LaboratoryMonthChemicalIds { get; set; } = new List<uint?> { };

        public LaboratoryMonthView()
        {
            InitializeComponent();

            DateTime now = DateTime.Now.Date;

            LaboratoryMonthPicker.SelectedDate = now;

            List<LaboratoryMonthItem> laboratoryMonthItems = new List<LaboratoryMonthItem>();

            var context = new laboratory_2023Context();
            laboratoryMonthItems = getLaboratoryMonthItemsList(context, now);

            if (laboratoryMonthItems.Count != 0)
            {
                DataItems = new ObservableCollection<LaboratoryMonthItem>(laboratoryMonthItems);
            }
            else
            {
                DataItems = new ObservableCollection<LaboratoryMonthItem>(generateLaboratoryMonthItems(context, now));
            }

            addDataItemsIfNeeded();

            context.Dispose();

            LaboratoryMonthDataGrid.ItemsSource = DataItems;
            LaboratoryMonthChemicalDataGrid.ItemsSource = ChemicalDataItems;
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

        private void addDataItemsIfNeeded()
        {
            if (DataItems.Count == 0)
            {
                DataItems.Add(new LaboratoryMonthItem());
            }

            if (ChemicalDataItems.Count == 0)
            {
                ChemicalDataItems.Add(new LaboratoryMonthChemicalItem());
            }
        }
        
        private List<LaboratoryMonthItem> getLaboratoryMonthItemsList(laboratory_2023Context context, DateTime time)
        {
            List<LaboratoryMonthItem> items = context.LaboratoryMonths
                    .Where(ld => ld.MonthId == time.Month)
                    .Where(ld => ld.Year == time.Year)
                    .Where(ld => ld.DeletedAt == null)
                    .Select(ld => new LaboratoryMonthItem
                    {
                        Id = ld.Id,
                        LaboratoryDayDate = new DateTime(ld.Date.Year, ld.Date.Month, ld.Date.Day),
                        Kilograms = ld.Kilograms.ToString(),
                        MetersSquared = ld.MetersSquared.ToString(),
                    }).ToList();
            
            List<LaboratoryMonthChemicalItem> chemicalItems = context.LaboratoryMonthChemicals
                    .Where(ld => ld.MonthId == time.Month)
                    .Where(ld => ld.Year == time.Year)
                    .Where(ld => ld.DeletedAt == null)
                    .Select(ld => new LaboratoryMonthChemicalItem
                    {
                        Id = ld.Id,
                        ChemicalName = ld.Name,
                        ChemicalExpenditure = ld.ChemicalExpenditure.ToString(),
                        ExpensePerMeterSquared = ld.ExpensePerMeterSquared.ToString(),
                    }).ToList();

            //Get ids so we can track which items where deleted during save operation
            LaboratoryMonthIds = items.Select(x => x.Id).ToList();
            LaboratoryMonthChemicalIds = chemicalItems.Select(x => x.Id).ToList();

            ChemicalDataItems = new ObservableCollection<LaboratoryMonthChemicalItem>(chemicalItems);

            return items;
        }
        
        private List<LaboratoryMonthItem> generateLaboratoryMonthItems(laboratory_2023Context context, DateTime time)
        {
            List<LaboratoryDayItem> LaboratoryDayItems = LaboratoryDayService.getLaboratoryDayItems(context, time, true);
            List<LaboratoryMonthItem> items = new List<LaboratoryMonthItem>();

            var GroupedLaboratoryDayItems = LaboratoryDayItems.GroupBy(d => d.Day).ToList();

            foreach (var group in GroupedLaboratoryDayItems)
            {
                float kgSum = 0;
                float m2Sum = 0;
                foreach (var item in group)
                {
                    kgSum += item.KilogramsPerMeter != null ? float.Parse(item.KilogramsPerMeter) : 0;
                    m2Sum += float.Parse(item.PaintedMetersSquared);
                }
                DateOnly date = DateOnly.Parse(group.Key);
                items.Add(new LaboratoryMonthItem
                {
                    Id = 0,
                    LaboratoryDayDate = new DateTime(date.Year, date.Month, date.Day),
                    Kilograms = kgSum.ToString(),
                    MetersSquared = m2Sum.ToString(),
                });
            }

            return items;
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            bool? Result = new CustomMessageBox("Ще прегенерирате данните в таблицата! \nАко имате промени ги запазете!", "Прегенериране на данни?").ShowDialog();

            if (Result.Value)
            {
                var context = new laboratory_2023Context();
                DateTime selectedDate = (DateTime)LaboratoryMonthPicker.SelectedDate;

                DataItems = new ObservableCollection<LaboratoryMonthItem>(generateLaboratoryMonthItems(context, selectedDate));

                addDataItemsIfNeeded();

                LaboratoryMonthDataGrid.ItemsSource = DataItems;

                context.Dispose();
            }
        }
        
        private void ReloadFromDb_Click(object sender, RoutedEventArgs e)
        {
            bool? Result = new CustomMessageBox("Ще презаредите данните в таблицата! \nАко имате промени ги запазете!", "Презареждане на данни?").ShowDialog();

            if (Result.Value)
            {
                var context = new laboratory_2023Context();
                DateTime selectedDate = (DateTime)LaboratoryMonthPicker.SelectedDate;

                List<LaboratoryMonthItem> laboratoryMonthItems = new List<LaboratoryMonthItem>(getLaboratoryMonthItemsList(context, selectedDate));
                DataItems = new ObservableCollection<LaboratoryMonthItem>(laboratoryMonthItems);

                addDataItemsIfNeeded();

                LaboratoryMonthDataGrid.ItemsSource = DataItems;
                LaboratoryMonthChemicalDataGrid.ItemsSource = ChemicalDataItems;

                context.Dispose();
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = (DateTime)LaboratoryMonthPicker.SelectedDate;

            if (selectedDate != null)
            {
                var context = new laboratory_2023Context();

                List<LaboratoryMonthItem> laboratoryMonthItems = new List<LaboratoryMonthItem>(getLaboratoryMonthItemsList(context, selectedDate));
                DataItems = new ObservableCollection<LaboratoryMonthItem>(laboratoryMonthItems);

                addDataItemsIfNeeded();

                LaboratoryMonthDataGrid.ItemsSource = DataItems;
                LaboratoryMonthChemicalDataGrid.ItemsSource = ChemicalDataItems;

                context.Dispose();
            }
        }
        
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var context = new laboratory_2023Context();
            List<LaboratoryMonth> newlyCreatedMonthItems = new List<LaboratoryMonth>();
            List<LaboratoryMonthChemical> newlyCreatedMonthChemicalItems = new List<LaboratoryMonthChemical>();
            if (App.UserPermissions.Contains("permissions.all") || App.UserPermissions.Contains("permissions.create_months") ||
                App.UserPermissions.Contains("permissions.edit_months") || App.UserPermissions.Contains("permissions.delete_months"))
            {
                try
                {
                    foreach (LaboratoryMonthItem item in DataItems)
                    {
                        if (item.Id == 0)
                        {
                            LaboratoryMonth newLaboratoryMonth = new LaboratoryMonth
                            {
                                Date = DateOnly.FromDateTime((DateTime)item.LaboratoryDayDate),
                                MonthId = (uint)LaboratoryMonthPicker.SelectedDate.Value.Month,
                                Year = (short)LaboratoryMonthPicker.SelectedDate.Value.Year,
                                Kilograms = decimal.Parse(item.Kilograms),
                                MetersSquared = decimal.Parse(item.MetersSquared),
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                DeletedAt = null,
                                CreatedBy = App.CurrentUser.Id,
                                UpdatedBy = App.CurrentUser.Id,
                            };

                            context.LaboratoryMonths.Add(newLaboratoryMonth);

                            //Needed so we can later update LaboratoryMonthIds, this makes it possible  to delete an item that was just created
                            newlyCreatedMonthItems.Add(newLaboratoryMonth);

                            continue;
                        }

                        //In DataItems if a row has been deleted it won't get needlessly updated since it won't be present in the ObservableCollection
                        var rowToUpdate = context.LaboratoryMonths.FirstOrDefault(row => row.Id == item.Id);

                        if (item.Id == null && DataItems.Count == 1)
                        {
                            rowToUpdate = context.LaboratoryMonths.FirstOrDefault(row => row.Id == LaboratoryMonthIds[0]);
                            rowToUpdate.DeletedAt = DateTime.Now;
                        }
                        else
                        {
                            if (rowToUpdate != null)
                            {
                                rowToUpdate.Date = DateOnly.FromDateTime((DateTime)item.LaboratoryDayDate);
                                rowToUpdate.Kilograms = decimal.Parse(item.Kilograms);
                                rowToUpdate.MetersSquared = decimal.Parse(item.MetersSquared);
                                rowToUpdate.UpdatedAt = DateTime.Now;
                                rowToUpdate.UpdatedBy = App.CurrentUser.Id;

                            }
                        }
                    }

                    //Get the difference between LaboratoryMonthIds, which is set when loading DataItems, and DataItems that has only ids to be updated
                    List<uint?> idsToDeleteMonths = LaboratoryMonthIds.Except(DataItems.Select(x => x.Id)).ToList();

                    foreach (uint id in idsToDeleteMonths)
                    {
                        var rowToDelete = context.LaboratoryMonths.FirstOrDefault(x => x.Id == id);

                        //Soft Delete the item from the database
                        rowToDelete.DeletedAt = DateTime.Now;
                    }

                    foreach (LaboratoryMonthChemicalItem item in ChemicalDataItems)
                    {
                        if (item.Id == 0 && item.ChemicalExpenditure != null && item.ChemicalName != null && item.ExpensePerMeterSquared != null)
                        {
                            LaboratoryMonthChemical newLaboratoryMonthChemical = new LaboratoryMonthChemical
                            {
                                MonthId = (uint)LaboratoryMonthPicker.SelectedDate.Value.Month,
                                Year = (ushort)LaboratoryMonthPicker.SelectedDate.Value.Year,
                                Name = item.ChemicalName,
                                ChemicalExpenditure = decimal.Parse(item.ChemicalExpenditure),
                                ExpensePerMeterSquared = decimal.Parse(item.ExpensePerMeterSquared),
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                DeletedAt = null,
                                CreatedBy = App.CurrentUser.Id,
                                UpdatedBy = App.CurrentUser.Id,
                            };

                            context.LaboratoryMonthChemicals.Add(newLaboratoryMonthChemical);

                            //Needed so we can later update LaboratoryMonthChemicalIds, this makes it possible  to delete an item that was just created
                            newlyCreatedMonthChemicalItems.Add(newLaboratoryMonthChemical);

                            continue;
                        }

                        //In DataItems if a row has been deleted it won't get needlessly updated since it won't be present in the ObservableCollection
                        var rowToUpdate = context.LaboratoryMonthChemicals.FirstOrDefault(row => row.Id == item.Id);

                        if (item.Id == null && ChemicalDataItems.Count == 1)
                        {
                            if (LaboratoryMonthChemicalIds.Count != 0)
                            {
                                rowToUpdate = context.LaboratoryMonthChemicals.FirstOrDefault(row => row.Id == LaboratoryMonthChemicalIds[0]);
                                rowToUpdate.DeletedAt = DateTime.Now;
                            }
                        }
                        else
                        {
                            if (rowToUpdate != null)
                            {
                                rowToUpdate.Name = item.ChemicalName;
                                rowToUpdate.ChemicalExpenditure = decimal.Parse(item.ChemicalExpenditure);
                                rowToUpdate.ExpensePerMeterSquared = decimal.Parse(item.ExpensePerMeterSquared);
                                rowToUpdate.UpdatedAt = DateTime.Now;
                                rowToUpdate.UpdatedBy = App.CurrentUser.Id;
                            }
                        }
                    }

                    //Get the difference between LaboratoryMonthChemicalIds, which is set when loading DataItems, and DataItems that has only ids to be updated
                    List<uint?> idsToDeleteChemical = LaboratoryMonthChemicalIds.Except(ChemicalDataItems.Select(x => x.Id)).ToList();

                    foreach (uint id in idsToDeleteChemical)
                    {
                        var rowToDelete = context.LaboratoryMonthChemicals.FirstOrDefault(x => x.Id == id);

                        //Soft Delete the item from the database
                        rowToDelete.DeletedAt = DateTime.Now;
                    }

                    context.SaveChanges();
                    notifier.ShowSuccess("Успешно запазени промени!");

                    LaboratoryMonthIds.AddRange(newlyCreatedMonthItems.Select(o => (uint?)o.Id));

                    DataItems = new ObservableCollection<LaboratoryMonthItem>(getLaboratoryMonthItemsList(context, DateTime.Now.Date));
                    LaboratoryMonthDataGrid.ItemsSource = null;
                    LaboratoryMonthDataGrid.ItemsSource = DataItems;

                    LaboratoryMonthChemicalIds.AddRange(newlyCreatedMonthChemicalItems.Select(o => (uint?)o.Id));

                    LaboratoryMonthChemicalDataGrid.ItemsSource = null;
                    LaboratoryMonthChemicalDataGrid.ItemsSource = ChemicalDataItems;

                    addDataItemsIfNeeded();
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

                DataItems = new ObservableCollection<LaboratoryMonthItem>(getLaboratoryMonthItemsList(context, DateTime.Now.Date));
                LaboratoryMonthDataGrid.ItemsSource = null;
                LaboratoryMonthDataGrid.ItemsSource = DataItems;

                LaboratoryMonthChemicalDataGrid.ItemsSource = null;
                LaboratoryMonthChemicalDataGrid.ItemsSource = ChemicalDataItems;

                addDataItemsIfNeeded();
            }

            context.Dispose();
        }

        private void AddChemicalRow_Click(object sender, RoutedEventArgs e)
        {
            ChemicalDataItems.Add(new LaboratoryMonthChemicalItem());
        }

        private void AddMonthRow_Click(object sender, RoutedEventArgs e)
        {
            DataItems.Add(new LaboratoryMonthItem());
        }

        private void DeleteChemicalButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button.DataContext as LaboratoryMonthChemicalItem;

            // Get the data source of the DataGrid
            var source = LaboratoryMonthChemicalDataGrid.ItemsSource as ObservableCollection<LaboratoryMonthChemicalItem>;


            if (source.Contains(item))
            {
                if (source.Count == 1)
                {
                    item.Id = null;
                    item.ChemicalName = "";
                    item.ChemicalExpenditure = "";
                    item.ExpensePerMeterSquared = "";

                    //Needed to update the datagrid properly
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        LaboratoryMonthChemicalDataGrid.ItemsSource = null;
                        LaboratoryMonthChemicalDataGrid.ItemsSource = source;
                    }));
                }
                else
                {
                    source.Remove((LaboratoryMonthChemicalItem)item);
                }
            }
        }

        private void DeleteMonthButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button.DataContext as LaboratoryMonthItem;

            // Get the data source of the DataGrid
            var source = LaboratoryMonthDataGrid.ItemsSource as ObservableCollection<LaboratoryMonthItem>;


            if (source.Contains(item))
            {
                if (source.Count == 1)
                {
                    item.Id = null;
                    item.LaboratoryDayDate = null;
                    item.Kilograms = "";
                    item.MetersSquared = "";

                    //Needed to update the datagrid properly
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        LaboratoryMonthDataGrid.ItemsSource = null;
                        LaboratoryMonthDataGrid.ItemsSource = source;
                    }));
                }
                else
                {
                    source.Remove((LaboratoryMonthItem)item);
                }
            }
        }

        private void MonthDataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            LaboratoryMonthScrollViewer.ScrollToVerticalOffset(LaboratoryMonthScrollViewer.VerticalOffset - e.Delta / 3);
        }
    }
}
