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
using System.Text.RegularExpressions;
using DiplomaWork.Services;
using DiplomaWork.Models;
using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Configuration;
using DiplomaWork.Helpers;

namespace DiplomaWork
{
    public partial class EmailModal : Window
    {
        string email = string.Empty;

        public EmailModal()
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

        private async void validateAndSendEmail()
        {
            if (validateEmail())
            {
                App.EmailUser = UserService.getUserByEmail(email);

                if (App.EmailUser != null)
                {

                    using (var dbContext = new laboratory_2023Context())
                    {
                        var existingEmailCode = dbContext.EmailCodes.FirstOrDefault(x => x.UserId == App.EmailUser.Id && x.IsValid == 1);

                        DateTime dateTimeNow = DateTime.Now;
                        DateTime nowPlus15Minutes = dateTimeNow.AddMinutes(15);
                        DateTime fifteenMinutesAgo = dateTimeNow.AddMinutes(-15);

                        var pastCodes = dbContext.EmailCodes
                                .Where(x => x.ExpiredAt >= fifteenMinutesAgo && x.ExpiredAt <= nowPlus15Minutes && x.UserId == App.EmailUser.Id && x.IsValid == 0)
                                .ToList();

                        if (pastCodes.Count >= 5)
                        {
                            notifier.ShowError("Достигнахте лимита на опити за подновяване на парола! Опитайте по-късно.");
                            return;
                        }

                        if (existingEmailCode != null)
                        {
                            existingEmailCode.IsValid = 0;
                            dbContext.Entry(existingEmailCode).State = EntityState.Modified;

                            dbContext.SaveChanges();
                        }

                        EmailCode newEmailCode = new EmailCode
                        {
                            UserId = App.EmailUser.Id,
                            Code = GenerateRandomCode().ToString(),
                            ExpiredAt = nowPlus15Minutes,
                            IsValid = 1
                        };

                        dbContext.EmailCodes.Add(newEmailCode);

                        dbContext.SaveChanges();
                    }


                    ConfirmEmailCodeModal confirmEmailCodeModal = new ConfirmEmailCodeModal();
                    confirmEmailCodeModal.Show();

                    this.Close();

                    try
                    {
                        await sendForgottenPasswordEmail();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.ToString());
                    }

                    return;
                }

                notifier.ShowError("Не беше открит потребител с този имейл адрес!");
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            validateAndSendEmail();
        }

        private async Task sendForgottenPasswordEmail()
        {
            MailMessage message = new MailMessage();

            message.From = new MailAddress(App.companyEmail);
            message.To.Add(App.EmailUser.EMail);

            string emailCode = UserService.getUserEmailCodeByUserId(App.EmailUser.Id);

            message.Subject = "Код за възстановяване на парола!";
            message.Body = "Вашият код за възстановяване на парола: " + emailCode;

            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.Credentials = new NetworkCredential(ConfigurationHelper.getEmailValue(), ConfigurationHelper.getPasswordValue());
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;

                await smtpClient.SendMailAsync(message);
            }
        }

        private int GenerateRandomCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999);
        }

        private bool validateEmail()
        {
            email = UserEMailTextBox.Text;

            if (!string.IsNullOrEmpty(email))
            {
                string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

                if (!Regex.IsMatch(email, emailPattern))
                {
                    notifier.ShowWarning("Неправилен имейл формат!");
                    UserEMailTextBox.Focus();

                    return false;
                }
            }
            else
            {
                notifier.ShowWarning("Имейл е задължително поле!");
                UserEMailTextBox.Focus();

                return false;
            }

            return true;
        }
        
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                validateAndSendEmail();
            }
        }
    }
}
