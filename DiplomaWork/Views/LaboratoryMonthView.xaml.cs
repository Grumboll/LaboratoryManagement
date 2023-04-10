using DiplomaWork.DataItems;
using DiplomaWork.Models;
using DiplomaWork.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DiplomaWork.Views
{
    public partial class LaboratoryMonthView : UserControl
    {
        public ObservableCollection<LaboratoryMonthItem> DataItems { get; set; } = new ObservableCollection<LaboratoryMonthItem>();
        public List<uint?> LaboratoryMonthHasChemicalIds { get; set; } = new List<uint?> { };

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

                if (laboratoryMonthItems.Count != 0)
                {
                    DataItems.Add(new LaboratoryMonthItem());
                }
            }

            context.Dispose();

            LaboratoryMonthDataGrid.ItemsSource = DataItems;
        }
        
        private List<LaboratoryMonthItem> getLaboratoryMonthItemsList(laboratory_2023Context context, DateTime time)
        {
            List<LaboratoryMonthItem> items = context.LaboratoryMonths
                    .Where(ld => ld.MonthId == time.Month)
                    .Where(ld => ld.Year == time.Year)
                    .Include(x => x.LaboratoryMonthHasChemical)
                    .Select(ld => new LaboratoryMonthItem
                    {
                        Id = ld.Id,
                        Kilograms = ld.Kilograms.ToString(),
                        MetersSquared = ld.MetersSquared.ToString(),
                        LaboratoryMonthChemicalId = ld.LaboratoryMonthHasChemical.Id,
                        ChemicalExpenditure = ld.LaboratoryMonthHasChemical.ChemicalExpenditure.ToString(),
                        LaboratoryMonthChemical = ld.LaboratoryMonthHasChemical.Name,
                        ExpensePerMonthMetersSquared = ld.LaboratoryMonthHasChemical.ExpensePerMeterSquared.ToString(),
                    }).ToList();


            //Get ids so we can track which items where deleted during save operation
            LaboratoryMonthHasChemicalIds = items.Select(x => x.LaboratoryMonthChemicalId).ToList();

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
                    Id = null,
                    LaboratoryDayDate = new DateTime(date.Year, date.Month, date.Day),
                    Kilograms = kgSum.ToString(),
                    MetersSquared = m2Sum.ToString(),
                    LaboratoryMonthChemicalId = null,
                    ChemicalExpenditure = null,
                    LaboratoryMonthChemical = null,
                    ExpensePerMonthMetersSquared = null,
                });
            }


            return items;
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Ще прегенерирате данните в таблицата! Ако имате промени ги запазете!", "Прегенериране на данни?", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                var context = new laboratory_2023Context();
                DateTime selectedDate = (DateTime)LaboratoryMonthPicker.SelectedDate;

                DataItems = new ObservableCollection<LaboratoryMonthItem>(generateLaboratoryMonthItems(context, selectedDate));
                LaboratoryMonthDataGrid.ItemsSource = DataItems;

                context.Dispose();
            }
        }
        
        private void ReloadFromDb_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Ще презаредите данните в таблицата! Ако имате промени ги запазете!", "Презареждане на данни?", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                var context = new laboratory_2023Context();
                DateTime selectedDate = (DateTime)LaboratoryMonthPicker.SelectedDate;

                DataItems = new ObservableCollection<LaboratoryMonthItem>(getLaboratoryMonthItemsList(context, selectedDate));
                LaboratoryMonthDataGrid.ItemsSource = DataItems;

                context.Dispose();
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = (DateTime)LaboratoryMonthPicker.SelectedDate;

            if (selectedDate != null)
            {
                var context = new laboratory_2023Context();

                DataItems = new ObservableCollection<LaboratoryMonthItem>(getLaboratoryMonthItemsList(context, selectedDate));
                LaboratoryMonthDataGrid.ItemsSource = DataItems;

                context.Dispose();
            }
        }
    }
}
