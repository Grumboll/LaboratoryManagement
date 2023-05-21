using OfficeOpenXml;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DiplomaWork.Services.ExcelGeneration;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using DiplomaWork.Services.PDFGeneration;

namespace DiplomaWork
{
    public partial class GenerateReportModal : Window
    {
        private string? fileType;
        private string? reportType;

        public GenerateReportModal()
        {
            InitializeComponent();

            fileType = null;
            reportType = null;
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (fileType != null && reportType != null && ReportBeginningDate.SelectedDate.HasValue && ReportEndDate.SelectedDate.HasValue)
            {
                switch(fileType)
                {
                    case "Excel":
                        ExcelGenerator.generateExcelByReportType(reportType, ReportBeginningDate.SelectedDate, ReportEndDate.SelectedDate);
                        break;

                    case "PDF":
                        PDFGenerator.GeneratePdfByReportType(reportType, ReportBeginningDate.SelectedDate, ReportEndDate.SelectedDate);
                        break;
                }
                this.Close();
            } 
            else
            {
                notifier.ShowError("Има неизбрани полета!");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void FileTypeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton selectedRadioButton = sender as RadioButton;
            string selectedOption = selectedRadioButton.Content.ToString();

            fileType = selectedOption;
        }

        private void ReportTypeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton selectedRadioButton = sender as RadioButton;
            string selectedOption = selectedRadioButton.Content.ToString();

            reportType = selectedOption;
        }

        private void BeginningDate_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            if (!ReportBeginningDate.IsDropDownOpen)
            {
                ReportBeginningDate.IsDropDownOpen = true;
                e.Handled = true;
            }
        }

        private void EndDate_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            if (!ReportEndDate.IsDropDownOpen)
            {
                ReportEndDate.IsDropDownOpen = true;
                e.Handled = true;
            }
        }

        private void BeginningDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReportBeginningDate.SelectedDate.HasValue)
            {
                DateTime? selectedDate = ReportBeginningDate.SelectedDate;
                ReportEndDate.DisplayDateStart = selectedDate;
                if (ReportEndDate.SelectedDate.HasValue && ReportEndDate.SelectedDate.Value < selectedDate)
                {
                    ReportEndDate.SelectedDate = selectedDate;
                }
            }
        }
    }
}
