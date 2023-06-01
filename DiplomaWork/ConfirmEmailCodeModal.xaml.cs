using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using DiplomaWork.Models;
using DiplomaWork.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DiplomaWork
{
    public partial class ConfirmEmailCodeModal : Window
    {
        private int maxTries = 3;
        private int currentTry = 1;

        public ConfirmEmailCodeModal()
        {
            InitializeComponent();
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

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            checkCodeValidity();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                checkCodeValidity();
            }
        }

        private void checkCodeValidity()
        {
            using (var dbContext = new laboratory_2023Context())
            {
                var emailCode = dbContext.EmailCodes.FirstOrDefault(x => x.UserId == App.EmailUser.Id && x.IsValid == 1);

                if (UserEmailCodeTextBox.Text == emailCode.Code)
                {
                    if (DateTime.Now > emailCode.ExpiredAt)
                    {
                        emailCode.IsValid = 0;
                        dbContext.Entry(emailCode).State = EntityState.Modified;

                        dbContext.SaveChanges();

                        notifier.ShowError("Кодът е изтекъл! Опитайте отново по-късно!");
                        this.Close();
                    }

                    PasswordResetModal passwordResetModal = new PasswordResetModal("resetForgotten");
                    passwordResetModal.Show();
                    
                    this.Close();
                }
                else
                {
                    int remainingTries = maxTries - currentTry;
                    if (remainingTries == 0)
                    {
                        emailCode.IsValid = 0;
                        dbContext.Entry(emailCode).State = EntityState.Modified;

                        dbContext.SaveChanges();

                        notifier.ShowError("Достигнахте максимума на опитите! Опитайте отново по-късно!");
                        this.Close();

                        return;
                    }
                    string triesText = remainingTries == 1 ? " опит" : " опита";
                    notifier.ShowError("Кодът е неправилен! Имате още " + remainingTries + triesText);
                }
                currentTry++;
            }
        }
    }
}
