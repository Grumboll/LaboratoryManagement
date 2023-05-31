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

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {

            if (validateEmail())
            {
                App.EmailUser = UserService.getUserByEmail(email);

                if (App.EmailUser != null)
                {

                    using (var dbContext = new laboratory_2023Context())
                    {
                        EmailCode newEmailCode = new EmailCode
                        {
                            UserId = App.EmailUser.Id,
                            ExpiredAt = DateTime.Now.AddMinutes(15),
                            IsValid = 1
                        };

                        dbContext.EmailCodes.Add(newEmailCode);

                        dbContext.SaveChanges();
                    }
                    
                    this.Close();

                    //TODO: STEP 3
                }

                notifier.ShowError("Не беше открит потребител с този имейл адрес!");
            }
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
    }
}
