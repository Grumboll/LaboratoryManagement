using DiplomaWork.DataItems;
using DiplomaWork.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace DiplomaWork.Services.ExcelGeneration
{
    public class ExcelGenerator
    {
        public static void generateExcelByReportType(string reportType, DateTime? beginningDate, DateTime? endDate)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(reportType + " заявка");

                switch (reportType)
                {
                    case "Дневна":
                        var dailyHeaderCells = worksheet.Cells["A1:F1"];
                        dailyHeaderCells.Style.Font.Bold = true;
                        dailyHeaderCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        dailyHeaderCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);

                        worksheet.Cells["G1"].Style.Font.Bold = true;

                        worksheet.Cells["A1"].Value = "Вид профил";
                        worksheet.Cells["B1"].Value = "Дължина, mm";
                        worksheet.Cells["C1"].Value = "Периметър, mm";
                        worksheet.Cells["D1"].Value = "m2";
                        worksheet.Cells["E1"].Value = "Брой боядисани профили";
                        worksheet.Cells["F1"].Value = "Боядисани m2";
                        worksheet.Cells["G1"].Value = "КГ/М";

                        generateDailyExcelReportAndFillCells(worksheet, beginningDate, endDate);

                        break;
                    case "Месечна":
                        var monthlyHeaderCells = worksheet.Cells["A1:C1"];
                        monthlyHeaderCells.Style.Font.Bold = true;
                        monthlyHeaderCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        monthlyHeaderCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);

                        worksheet.Cells["A1"].Value = "Дата";
                        worksheet.Cells["B1"].Value = "Кг";
                        worksheet.Cells["C1"].Value = "М2";

                        generateMonthlyExcelReportAndFillCells(worksheet, beginningDate, endDate);

                        break;
                    case "Среден Разход":
                        generateAverageExpenseExcelReportAndFillCells(worksheet, beginningDate, endDate);
                        break;
                }

                worksheet.Cells.AutoFitColumns();

                promptUserAndSaveExcelFile(package);
            }
            
        }

        private static void promptUserAndSaveExcelFile(ExcelPackage package)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                package.SaveAs(stream);

                stream.Position = 0;

                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;

                    // Write the stream data to the file
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        stream.WriteTo(fileStream);
                    }
                }

                bool? Result = new CustomMessageBox("Файлът беше запазен успешно!", "Генериране на справка").ShowDialog();
            }
        }

        private static void generateDailyExcelReportAndFillCells(ExcelWorksheet worksheet, DateTime? beginningDate, DateTime? endDate)
        {
            var context = new laboratory_2023Context();

            List<LaboratoryDayItem> items = context.LaboratoryDays
                    .Where(ld => ld.Day >= DateOnly.FromDateTime((DateTime) beginningDate) && ld.Day <= DateOnly.FromDateTime((DateTime) endDate))
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

            int i = 0;

            for (int row = 2; row <= items.Count + 1; row++)
            {
                worksheet.Cells[row, 1].Value = items[i].ProfileName;
                worksheet.Cells[row, 2].Value = items[i].ProfileLength;
                worksheet.Cells[row, 3].Value = items[i].ProfilePerimeter;
                worksheet.Cells[row, 4].Value = items[i].MetersSquaredPerSample;
                worksheet.Cells[row, 5].Value = items[i].PaintedSamplesCount;
                worksheet.Cells[row, 6].Value = items[i].PaintedMetersSquared;
                worksheet.Cells[row, 7].Value = items[i].KilogramsPerMeter != null ? items[i].KilogramsPerMeter : "";
                i++;
            }
        }
        
        private static void generateMonthlyExcelReportAndFillCells(ExcelWorksheet worksheet, DateTime? beginningDate, DateTime? endDate)
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

            foreach (LaboratoryMonthItem item in items)
            {
                item.Kilograms = item.Kilograms.TrimEnd('0').TrimEnd('.');
                item.MetersSquared = item.MetersSquared.TrimEnd('0').TrimEnd('.');
            }
            
            foreach (LaboratoryMonthChemicalItem chemicalItem in chemicalItems)
            {
                chemicalItem.ChemicalExpenditure = chemicalItem.ChemicalExpenditure.TrimEnd('0').TrimEnd('.');
                chemicalItem.ExpensePerMeterSquared = chemicalItem.ExpensePerMeterSquared.TrimEnd('0').TrimEnd('.');
            }

            context.Dispose();

            int i = 0;
            int j = 0;

            for (int p = 1; p <= 3; p++)
            {
                worksheet.Cells[items.Count + 2, p].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[items.Count + 2, p].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
            }

            for (int column = 4; column <= chemicalItems.Count * 2 + 3; column = column + 2)
            {
                var chemicalNameCell = worksheet.Cells[1, column];

                chemicalNameCell.Style.Font.Bold = true;
                chemicalNameCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                chemicalNameCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                chemicalNameCell.Value = chemicalItems[j].ChemicalName;

                var chemicalExpenseCell = worksheet.Cells[1, column + 1];

                chemicalExpenseCell.Value = "м2";
                chemicalExpenseCell.Style.Font.Bold = true;
                chemicalExpenseCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                chemicalExpenseCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);

                worksheet.Cells[items.Count + 2, column].Value = chemicalItems[j].ChemicalExpenditure;
                worksheet.Cells[items.Count + 2, column + 1].Value = chemicalItems[j].ExpensePerMeterSquared;

                worksheet.Cells[items.Count + 2, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[items.Count + 2, column].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                j++;
            }

            for (int row = 2; row <= items.Count + 1; row++)
            {
                worksheet.Cells[row, 1].Style.Numberformat.Format = "dd-mm-yyyy";
                worksheet.Cells[row, 1].Value = items[i].LaboratoryDayDate;

                worksheet.Cells[row, 2].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, 2].Value = decimal.Parse(items[i].Kilograms);

                worksheet.Cells[row, 3].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, 3].Value = decimal.Parse(items[i].MetersSquared);
                i++;
            }

            worksheet.Cells[items.Count + 2, 2].Formula = $"SUM({'B'}{2}:{'B'}{items.Count + 1})";
            worksheet.Cells[items.Count + 2, 3].Formula = $"SUM({'C'}{2}:{'C'}{items.Count + 1})";

            worksheet.Calculate();
        }

        private static void generateAverageExpenseExcelReportAndFillCells(ExcelWorksheet worksheet, DateTime? beginningDate, DateTime? endDate)
        {

        }
    }
}
