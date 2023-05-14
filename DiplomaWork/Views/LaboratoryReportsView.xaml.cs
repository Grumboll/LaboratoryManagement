using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using DiplomaWork.DataItems;
using DiplomaWork.Models;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Collections.ObjectModel;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using System.ComponentModel;

namespace DiplomaWork.Views
{
    public partial class LaboratoryReportsView : UserControl, INotifyPropertyChanged
    {
        public LaboratoryReportsView()
        {
            InitializeComponent();

            loadYearlyReportCards();
            loadMonthlyReportCards();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<MonthlyProfileReportItem> monthlyProfileReportItems;
        public ObservableCollection<MonthlyProfileReportItem> MonthlyProfileReportItems
        {
            get { return monthlyProfileReportItems; }
            set
            {
                monthlyProfileReportItems = value;
                OnPropertyChanged(nameof(MonthlyProfileReportItems));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        private void loadYearlyReportCards()
        {
            var yearlyReportItems = getYearlyChemicalReport();
            if (yearlyReportItems.Count != 0)
            {
                foreach (var item in yearlyReportItems)
                {
                    createAndAddToUIMaterialDesignCard(item.Name, item.ChemicalExpenseSum, item.ChemicalExpenseAverage);
                }
            }
            else
            {
                ReportsYearlyCards.Children.Add(new TextBlock
                {
                    Margin = new Thickness(5),
                    Text = "Липсва информация!"
                });
            }
        }

        private void loadMonthlyReportCards()
        {
            var monthlyReportItems = getMonthlyProfileReport();
            if (monthlyReportItems.Count != 0)
            {
                MonthlyProfileReportItems = new ObservableCollection<MonthlyProfileReportItem>(monthlyReportItems);

                DataContext = this;
            }
            else
            {
                ReportsMonthlyCards.Children.Add(new TextBlock
                {
                    Margin = new Thickness(5),
                    Text = "Липсва информация!"
                });
            }
        }

        private void createAndAddToUIMaterialDesignCard(string chemicalName, string chemicalExpenseSum, string chemicalExpenseAverage)
        {
            Card card = new Card
            {
                Margin = new Thickness(5),
                Content = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Children =
                    {
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            VerticalAlignment = VerticalAlignment.Center,
                            Children =
                            {
                                new PackIcon
                                {
                                    Kind = PackIconKind.ChemicalWeapon,
                                    Margin = new Thickness(5)
                                },
                                new TextBlock
                                {
                                    Margin = new Thickness(5),
                                    Text = chemicalName
                                }
                            }
                        },
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            VerticalAlignment = VerticalAlignment.Center,
                            Children =
                            {
                                new PackIcon
                                {
                                    Kind = PackIconKind.Summation,
                                    Margin = new Thickness(5)
                                },
                                new TextBlock
                                {
                                    Margin = new Thickness(5),
                                    Text = chemicalExpenseSum + " м2"
                                }
                            }
                        },
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            VerticalAlignment = VerticalAlignment.Center,
                            Children =
                            {
                                new PackIcon
                                {
                                    Kind = PackIconKind.Tilde,
                                    Margin = new Thickness(5)
                                },
                                new TextBlock
                                {
                                    Margin = new Thickness(5),
                                    Text = chemicalExpenseAverage + " м2"
                                }
                            }
                        }
                    }
                }
            };

            card.MouseEnter += (sender, e) =>
            {
                // Create a new ScaleTransform and apply it to the card's RenderTransform
                var transform = new ScaleTransform(1.0, 1.0);
                card.RenderTransform = transform;

                var animation = new DoubleAnimation(1.0, 1.1, TimeSpan.FromSeconds(0.2));
                animation.FillBehavior = FillBehavior.HoldEnd; // Set FillBehavior to HoldEnd
                transform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
                transform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
            };

            card.MouseLeave += (sender, e) =>
            {
                var transform = card.RenderTransform as ScaleTransform;
                if (transform != null)
                {
                    var animation = new DoubleAnimation(1.1, 1.0, TimeSpan.FromSeconds(0.2));
                    transform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
                    transform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
                }
            };

            ReportsYearlyCards.Children.Add(card);
        }

        private List<YearlyChemicalReportItem> getYearlyChemicalReport()
        {
            int currentYear = DateTime.Now.Year;
            var context = new laboratory_2023Context();

            var result = context.LaboratoryMonthChemicals
                    .Where(x => x.Year == currentYear)
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

            return result;
        }
        
        private List<MonthlyProfileReportItem> getMonthlyProfileReport()
        {
            int currentMonth = DateTime.Now.Month;
            var context = new laboratory_2023Context();

            var result = context.LaboratoryDays
                .Where(x => x.MonthId == currentMonth)
                .Where(x => x.DeletedAt == null)
                .Join(context.ProfileHasLengthsPerimeters,
                    ld => ld.ProfileHasLengthsPerimeterId,
                    phlp => phlp.Id,
                    (ld, phlp) => new { ld, phlp })
                .Join(context.Profiles,
                    joinResult => joinResult.phlp.ProfileId,
                    p => p.Id,
                    (joinResult, p) => new { joinResult.ld, joinResult.phlp, p })
                .GroupBy(g => g.phlp.Id)
                .Select(g => new MonthlyProfileReportItem
                {
                    Name = g.FirstOrDefault().p.Name,
                    ProfilePerimeter = g.FirstOrDefault().phlp.Perimeter.ToString().TrimEnd('0').TrimEnd('.'),
                    ProfileMetersSquaredPerSample = g.Sum(x => x.ld.MetersSquaredPerSample).ToString().TrimEnd('0').TrimEnd('.') + " м2"
                })
                .ToList();

            context.Dispose();

            return result;
        }

        private void ReloadReports_Click(object sender, RoutedEventArgs e)
        {
            ReportsYearlyCards.Children.Clear();
            MonthlyProfileReportItems.Clear();
            loadYearlyReportCards();
            loadMonthlyReportCards();

            notifier.ShowSuccess("Успешно обновихте използваните химикали/профили!");
        }
        
        private void AdditionalReport_Click(object sender, RoutedEventArgs e)
        {
            GenerateReportModal generateReportModal = new GenerateReportModal();
            generateReportModal.Show();
        }
    }
}
