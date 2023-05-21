using DiplomaWork.DataItems;
using DiplomaWork.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaWork.Services
{
    public class ReportService
    {
        public static List<LaboratoryDayItem> getDailyReportData(DateTime? beginningDate, DateTime? endDate)
        {
            var context = new laboratory_2023Context();

            List<LaboratoryDayItem> items = context.LaboratoryDays
                    .Where(ld => ld.Day >= DateOnly.FromDateTime((DateTime)beginningDate) && ld.Day <= DateOnly.FromDateTime((DateTime)endDate))
                    .Where(ld => ld.DeletedAt == null)
                    .Include(x => x.ProfileHasLengthsPerimeter)
                    .ThenInclude(x => x.Profile)
                    .Select(ld => new LaboratoryDayItem
                    {
                        Id = ld.Id,
                        ProfileHasLengthsPerimeterId = ld.ProfileHasLengthsPerimeterId,
                        Day = ld.Day.ToString(),
                        ProfileName = ld.ProfileHasLengthsPerimeter.Profile.Name,
                        ProfileLength = ld.ProfileHasLengthsPerimeter.Length.ToString(),
                        ProfilePerimeter = ld.ProfileHasLengthsPerimeter.Perimeter.ToString(),
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

            context.Dispose();

            return items;
        }
        
        public static List<LaboratoryMonthItem> getMonthlyReportData(DateTime? beginningDate, DateTime? endDate)
        {
            var context = new laboratory_2023Context();

            List<LaboratoryMonthItem> items = context.LaboratoryMonths
                    .Where(ld => ld.MonthId >= beginningDate.Value.Month && ld.MonthId <= endDate.Value.Month)
                    .Where(ld => ld.Year >= beginningDate.Value.Year && ld.Year <= endDate.Value.Year)
                    .Where(ld => ld.DeletedAt == null)
                    .Select(ld => new LaboratoryMonthItem
                    {
                        Id = ld.Id,
                        LaboratoryDayDate = new DateTime(ld.Date.Year, ld.Date.Month, ld.Date.Day),
                        Kilograms = ld.Kilograms.ToString(),
                        MetersSquared = ld.MetersSquared.ToString(),
                    }).ToList();

            foreach (LaboratoryMonthItem item in items)
            {
                item.Kilograms = item.Kilograms.TrimEnd('0').TrimEnd('.');
                item.MetersSquared = item.MetersSquared.TrimEnd('0').TrimEnd('.');
            }

            context.Dispose();

            return items;
        }
        
        public static List<LaboratoryMonthChemicalItem> getMonthlyChemicalReportData(DateTime? beginningDate, DateTime? endDate)
        {
            var context = new laboratory_2023Context();

            List<LaboratoryMonthChemicalItem> chemicalItems = context.LaboratoryMonthChemicals
                    .Where(ld => ld.MonthId >= beginningDate.Value.Month && ld.MonthId <= endDate.Value.Month)
                    .Where(ld => ld.Year >= beginningDate.Value.Year && ld.Year <= endDate.Value.Year)
                    .Where(ld => ld.DeletedAt == null)
                    .Select(ld => new LaboratoryMonthChemicalItem
                    {
                        Id = ld.Id,
                        ChemicalName = ld.Name,
                        ChemicalExpenditure = ld.ChemicalExpenditure.ToString(),
                        ExpensePerMeterSquared = ld.ExpensePerMeterSquared.ToString(),
                    }).ToList();

            foreach (LaboratoryMonthChemicalItem chemicalItem in chemicalItems)
            {
                chemicalItem.ChemicalExpenditure = chemicalItem.ChemicalExpenditure.TrimEnd('0').TrimEnd('.');
                chemicalItem.ExpensePerMeterSquared = chemicalItem.ExpensePerMeterSquared.TrimEnd('0').TrimEnd('.');
            }

            context.Dispose();

            return chemicalItems;
        }
        
        public static List<MonthlyProfileReportItem> getAverageExpenseProfileReportData(DateTime? beginningDate, DateTime? endDate)
        {
            var context = new laboratory_2023Context();

            List<MonthlyProfileReportItem> profileItems = context.LaboratoryDays
                .Where(ld => ld.MonthId >= beginningDate.Value.Month && ld.MonthId <= endDate.Value.Month)
                .Where(ld => ld.Year >= beginningDate.Value.Year && ld.Year <= endDate.Value.Year)
                .Where(x => x.DeletedAt == null)
                .GroupBy(g => g.ProfileHasLengthsPerimeter.Id)
                .Select(g => new MonthlyProfileReportItem
                {
                    Name = g.FirstOrDefault().ProfileHasLengthsPerimeter.Profile.Name,
                    ProfilePerimeter = g.FirstOrDefault().ProfileHasLengthsPerimeter.Perimeter.ToString().TrimEnd('0').TrimEnd('.'),
                    ProfileMetersSquaredPerSample = g.Average(x => x.MetersSquaredPerSample).ToString().TrimEnd('0').TrimEnd('.')
                })
                .ToList();

            context.Dispose();

            return profileItems;
        }
        
        public static List<YearlyChemicalReportItem> getAverageExpenseChemicalProfileReportData(DateTime? beginningDate, DateTime? endDate)
        {
            var context = new laboratory_2023Context();

            List<YearlyChemicalReportItem> chemicalItems = context.LaboratoryMonthChemicals
                .Where(ld => ld.MonthId >= beginningDate.Value.Month && ld.MonthId <= endDate.Value.Month)
                .Where(ld => ld.Year >= beginningDate.Value.Year && ld.Year <= endDate.Value.Year)
                .Where(x => x.DeletedAt == null)
                .GroupBy(x => x.Name)
                .Select(g => new YearlyChemicalReportItem
                {
                    Name = g.Key,
                    ChemicalExpenseSum = g.Sum(x => x.ExpensePerMeterSquared).ToString().TrimEnd('0').TrimEnd('.'),
                    ChemicalExpenseAverage = g.Average(x => x.ExpensePerMeterSquared).ToString().TrimEnd('0').TrimEnd('.')
                })
                .ToList();

            context.Dispose();

            return chemicalItems;
        }
    }
}
