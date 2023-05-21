using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using Microsoft.Win32;
using System.IO;
using Document = iText.Layout.Document;
using DiplomaWork.DataItems;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using iText.IO.Font;
using iText.Kernel.Font;
using System.Reflection;
using System.Drawing;
using iText.Kernel.Events;
using iText.Layout;
using iText.Kernel.Geom;

namespace DiplomaWork.Services.PDFGeneration
{
    public class PDFGenerator
    {
        public static readonly String FONT = "../../../Resources/Fonts/RobotoLight.ttf";

        public static void GeneratePdfByReportType(string reportType, DateTime? beginningDate, DateTime? endDate)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(memoryStream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                switch (reportType)
                {
                    case "Дневна":
                        generateDailyPDFReportAndFillTable(document, beginningDate, endDate);
                        break;

                    case "Месечна":
                        generateMonthlyPDFReportAndFillTable(document, beginningDate, endDate);
                        break;

                    case "Среден Разход":
                        generateAverageExpensePDFReportAndFillTable(document, beginningDate, endDate);
                        break;
                }

                document.Close();

                promptUserAndSavePDFFile(memoryStream);
            }
        }

        private static void promptUserAndSavePDFFile(MemoryStream memoryStream)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    File.WriteAllBytes(filePath, memoryStream.ToArray());

                    bool? Result = new CustomMessageBox("Файлът беше запазен успешно!", "Генериране на справка").ShowDialog();
                }
            }
            catch (IOException ex)
            {
                bool? Result = new CustomMessageBox("Възникна грешка при запазването на файла!", "Грешка при запазване").ShowDialog();
            }
        }

        private static void generateDailyPDFReportAndFillTable(Document document, DateTime? beginningDate, DateTime? endDate)
        {
            List<LaboratoryDayItem> items = ReportService.getDailyReportData(beginningDate, endDate);

            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 15, 15, 20, 10, 20, 10, 10 }));

            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.IDENTITY_H);

            table.SetFont(font);

            table.AddHeaderCell(new Cell().Add(new Paragraph("Вид профил")));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Дължина, mm")));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Периметър, mm")));
            table.AddHeaderCell(new Cell().Add(new Paragraph("m2")));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Брой боядисани профили")));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Боядисани m2")));
            table.AddHeaderCell(new Cell().Add(new Paragraph("КГ/М")));

            foreach (LaboratoryDayItem item in items)
            {
                table.AddCell(new Cell().Add(new Paragraph(item.ProfileName.ToString())));
                table.AddCell(new Cell().Add(new Paragraph(item.ProfileLength.ToString())));
                table.AddCell(new Cell().Add(new Paragraph(item.ProfilePerimeter.ToString())));
                table.AddCell(new Cell().Add(new Paragraph(item.MetersSquaredPerSample.ToString())));
                table.AddCell(new Cell().Add(new Paragraph(item.PaintedSamplesCount.ToString())));
                table.AddCell(new Cell().Add(new Paragraph(item.PaintedMetersSquared.ToString())));
                table.AddCell(new Cell().Add(new Paragraph(item.KilogramsPerMeter != null ? item.KilogramsPerMeter.ToString() : "")));

                table.StartNewRow();
            }

            document.Add(table);
        }
        
        private static void generateMonthlyPDFReportAndFillTable(Document document, DateTime? beginningDate, DateTime? endDate)
        {
            List<LaboratoryMonthItem> items = ReportService.getMonthlyReportData(beginningDate, endDate);
            List<LaboratoryMonthChemicalItem> chemicalItems = ReportService.getMonthlyChemicalReportData(beginningDate, endDate);

            document.GetPdfDocument().SetDefaultPageSize(PageSize.A4.Rotate());

            Table table = new Table(UnitValue.CreatePercentArray(3 + chemicalItems.Count * 2)).UseAllAvailableWidth();

            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.IDENTITY_H);

            table.SetFont(font);

            table.AddHeaderCell(new Cell().Add(new Paragraph("Дата")));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Кг")));
            table.AddHeaderCell(new Cell().Add(new Paragraph("М2")));

            foreach (LaboratoryMonthChemicalItem chemicalItem in chemicalItems)
            {
                table.AddHeaderCell(new Cell().Add(new Paragraph(chemicalItem.ChemicalName)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Разход на м2")));
            }

            foreach (LaboratoryMonthItem item in items)
            {
                table.AddCell(new Cell().Add(new Paragraph(item.LaboratoryDayDate.Value.Date.ToString("dd-MM-yyyy"))));
                table.AddCell(new Cell().Add(new Paragraph(item.Kilograms.ToString())));
                table.AddCell(new Cell().Add(new Paragraph(item.MetersSquared.ToString())));
                for (int i = 0; i < chemicalItems.Count * 2; i++)
                {
                    table.AddCell(new Cell().Add(new Paragraph("")));
                }

                table.StartNewRow();
            }

            table.StartNewRow();
            table.AddCell(new Cell().Add(new Paragraph("")));
            table.AddCell(new Cell().Add(new Paragraph("")));
            table.AddCell(new Cell().Add(new Paragraph("")));
            foreach (LaboratoryMonthChemicalItem chemicalItem in chemicalItems)
            {
                table.AddCell(new Cell().Add(new Paragraph(chemicalItem.ChemicalExpenditure)));
                table.AddCell(new Cell().Add(new Paragraph(chemicalItem.ExpensePerMeterSquared)));
            }

            document.Add(table);
        }
        
        private static void generateAverageExpensePDFReportAndFillTable(Document document, DateTime? beginningDate, DateTime? endDate)
        {
            List<MonthlyProfileReportItem> profileItems = ReportService.getAverageExpenseProfileReportData(beginningDate, endDate);
            List<YearlyChemicalReportItem> chemicalItems = ReportService.getAverageExpenseChemicalProfileReportData(beginningDate, endDate);

            document.GetPdfDocument().SetDefaultPageSize(PageSize.A4.Rotate());

            Table table = new Table(UnitValue.CreatePercentArray(3 + chemicalItems.Count * 2)).UseAllAvailableWidth();

            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.IDENTITY_H);

            table.SetFont(font);

            table.AddHeaderCell(new Cell().Add(new Paragraph("Вид профил")));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Периметър")));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Среден разход (М2)")));
            table.AddHeaderCell(new Cell().Add(new Paragraph("")));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Име на химикал")));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Разход сума")));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Среден разход")));

            for (int i = 0; i < profileItems.Count; i++)
            {
                table.AddCell(new Cell().Add(new Paragraph(profileItems[i].Name)));
                table.AddCell(new Cell().Add(new Paragraph(profileItems[i].ProfilePerimeter)));
                table.AddCell(new Cell().Add(new Paragraph(profileItems[i].ProfileMetersSquaredPerSample)));
                table.AddCell(new Cell().Add(new Paragraph("")));

                if (chemicalItems.Count > i && chemicalItems[i] != null)
                {
                    table.AddCell(new Cell().Add(new Paragraph(chemicalItems[i].Name)));
                    table.AddCell(new Cell().Add(new Paragraph(chemicalItems[i].ChemicalExpenseSum)));
                    table.AddCell(new Cell().Add(new Paragraph(chemicalItems[i].ChemicalExpenseAverage)));
                }
                else
                {
                    table.AddCell(new Cell().Add(new Paragraph("")));
                    table.AddCell(new Cell().Add(new Paragraph("")));
                    table.AddCell(new Cell().Add(new Paragraph("")));
                }

                table.StartNewRow();
            }

            document.Add(table);
        }
    }
}