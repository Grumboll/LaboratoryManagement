using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace DiplomaWork.Views
{
    public partial class LaboratorySettingsView : UserControl
    {
        public LaboratorySettingsView()
        {
            InitializeComponent();

            if (App.UserPermissions.Contains("permissions.all"))
            {
                Button btn = new Button()
                {
                    MinHeight = 30,
                    Height = Double.NaN,
                    Width = 110,
                    Margin = new Thickness(5),
                    Style = Application.Current.FindResource("MaterialDesignRaisedButton") as Style
                };

                TextBlock textBlock = new TextBlock()
                {
                    Text = "Заявки за нова парола",
                    FontSize = 12,
                    TextWrapping = TextWrapping.Wrap
                };

                btn.Content = textBlock;

                SettingsButtonStackPanel.Children.Add(btn);
            }
        }
    }
}
