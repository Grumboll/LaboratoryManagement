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

namespace DiplomaWork.Views
{
    public partial class LaboratoryReportsView : UserControl
    {
        public LaboratoryReportsView()
        {
            InitializeComponent();

            loadYearlyReportCards();
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

        private void ReloadReports_Click(object sender, RoutedEventArgs e)
        {
            ReportsYearlyCards.Children.Clear();
            loadYearlyReportCards();

            notifier.ShowSuccess("Успешно обновихте използваните химикали/профили!");
        }
    }
}
