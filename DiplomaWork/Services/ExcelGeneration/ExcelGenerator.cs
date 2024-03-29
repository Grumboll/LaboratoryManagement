﻿using DiplomaWork.DataItems;
using DiplomaWork.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
                try
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

                        bool? Result = new CustomMessageBox("Файлът беше запазен успешно!", "Генериране на справка").ShowDialog();
                    }
                }
                catch (IOException ex)
                {
                    bool? Result = new CustomMessageBox("Възникна грешка при запазването на файла!", "Грешка при запазване").ShowDialog();
                }
            }
        }

        private static void generateDailyExcelReportAndFillCells(ExcelWorksheet worksheet, DateTime? beginningDate, DateTime? endDate)
        {
            List<LaboratoryDayItem> items = ReportService.getDailyReportData(beginningDate, endDate);

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
            List<LaboratoryMonthItem> items = ReportService.getMonthlyReportData(beginningDate, endDate);
            List<LaboratoryMonthChemicalItem> chemicalItems = ReportService.getMonthlyChemicalReportData(beginningDate, endDate);

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
        }

        private static void generateAverageExpenseExcelReportAndFillCells(ExcelWorksheet worksheet, DateTime? beginningDate, DateTime? endDate)
        {
            List<MonthlyProfileReportItem> profileItems = ReportService.getAverageExpenseProfileReportData(beginningDate, endDate);
            List<YearlyChemicalReportItem> chemicalItems = ReportService.getAverageExpenseChemicalProfileReportData(beginningDate, endDate);

            int i = 0;
            int j = 0;

            for (int column = 1; column <= 7; column++)
            {
                var headerCells = worksheet.Cells[1, column];

                headerCells.Style.Font.Bold = true;
                headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
            }

            worksheet.Cells[1, 1].Value = "Вид профил";
            worksheet.Cells[1, 2].Value = "Периметър";
            worksheet.Cells[1, 3].Value = "Среден разход (м2)";

            worksheet.Cells[1, 5].Value = "Име на химикал";
            worksheet.Cells[1, 6].Value = "Разход сума";
            worksheet.Cells[1, 7].Value = "Среден разход";

            for (int row = 2; row <= profileItems.Count + 1; row++)
            {
                worksheet.Cells[row, 1].Value = profileItems[i].Name;

                worksheet.Cells[row, 2].Value = decimal.Parse(profileItems[i].ProfilePerimeter);

                worksheet.Cells[row, 3].Value = decimal.Parse(profileItems[i].ProfileMetersSquaredPerSample);
                i++;
            }
            
            for (int rowChem = 2; rowChem <= chemicalItems.Count + 1; rowChem++)
            {
                worksheet.Cells[rowChem, 5].Value = chemicalItems[j].Name;

                worksheet.Cells[rowChem, 6].Value = decimal.Parse(chemicalItems[j].ChemicalExpenseSum);

                worksheet.Cells[rowChem, 7].Value = decimal.Parse(chemicalItems[j].ChemicalExpenseAverage);
                j++;
            }
        }
    }
}
