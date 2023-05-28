using DiplomaWork.Services;
using System;
using System.Windows;
using System.Windows.Input;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using DiplomaWork.DataItems;

namespace DiplomaWork
{
    public partial class ProfileModal : Window
    {
        private string mode;
        private uint? profileHasLengthsPerimeterId;
        public ProfileModal(string mode, ProfileSettingsItem? profile)
        {
            InitializeComponent();

            this.mode = mode;

            switch (mode)
            {
                case "create":
                    ProfileModalTitle.Text = "Създаване на профил";
                    ChangeProfileNamesAll.Visibility = Visibility.Hidden;

                    break;

                case "edit":
                    ProfileModalTitle.Text = "Редактиране на профил";

                    profileHasLengthsPerimeterId = profile.Id;
                    ProfilNameTextBox.Text = profile.Name;
                    ProfilLengthTextBox.Text = profile.Length.ToString();
                    ProfilPerimeterTextBox.Text = profile.Perimeter.ToString().TrimEnd('0').TrimEnd('.');
                    
                    break;

                default:
                    this.Close();
                    break;
            }
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
            decimal profileLength;
            decimal profilePerimeter;

            if (!string.IsNullOrEmpty(ProfilNameTextBox.Text) && decimal.TryParse(ProfilLengthTextBox.Text, out profileLength) &&
                        decimal.TryParse(ProfilPerimeterTextBox.Text, out profilePerimeter))
            {
                switch (mode)
                {
                    case "create":
                        uint creationResult = ProfileService.createProfile(ProfilNameTextBox.Text, profileLength, profilePerimeter);

                        if (creationResult > 0)
                        {
                            notifier.ShowSuccess("Успешно създадохте профил!");
                        }
                        this.Close();

                        break;
                    case "edit":
                        int editResult = ProfileService.editProfileHasLengthsPerimeter(profileHasLengthsPerimeterId, ProfilNameTextBox.Text, profileLength, profilePerimeter, ChangeAllToggleButton.IsChecked);

                        if (editResult > 0)
                        {
                            notifier.ShowSuccess("Успешно редактирахте профил!");
                        }
                        this.Close();

                        break;
                    default:
                        this.Close();
                        break;
                }
            }
            else
            {
                notifier.ShowWarning("Невалидни или непопълнени данни!");
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
    }
}
