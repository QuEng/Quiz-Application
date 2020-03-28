using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using QuizApplication.Models;
using ColorConverter = System.Windows.Media.ColorConverter;
using FontFamily = System.Windows.Media.FontFamily;
using Color = System.Windows.Media.Color;

namespace QuizApplication {
    public partial class SecondWindow : Window {
        public SecondWindow() {
            InitializeComponent();
            Settings.Theme.ApplyConfiguration(this);

            BtnClose.Click += CommonMethods.CloseWindow_OnClick;
            BtnMaxMin.Click += CommonMethods.MaxMin_Click;

            Label_CategoryName.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Theme.TextColor));
            Label_CategoryName.FontFamily = new FontFamily(Settings.Theme.TextFontFamily);

            
            ConfigurationButtons();            
        }
        private void ConfigurationButtons() {
            SolidColorBrush color = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Theme.ButtonBackgroundColor)) {
                Opacity = Settings.Theme.ButtonTransparency / 100.0
            };
            var buttonStyle = Resources["btnStyle"] as Style;
            buttonStyle?.Setters.Add(new Setter(ForegroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Theme.ButtonTextColor))));
            buttonStyle?.Setters.Add(new Setter(FontFamilyProperty, new FontFamily(Settings.Theme.ButtonFontFamily)));
            buttonStyle?.Setters.Add(new Setter(BackgroundProperty, color));

            foreach (var button in Grid_Buttons.Children.OfType<Button>()) {
                button.Content = Settings.Game.CategoriesList[ Convert.ToInt32(button.Tag) - 1 ];
                button.Style = buttonStyle;
                button.Click += Buttons_Click;
                button.MouseEnter += Buttons_MouseEnter;
                button.MouseLeave += Buttons_MouseLeave;
            }
        }

        private void Buttons_Click(object sender, RoutedEventArgs e) {
            Settings.Game.ActiveCategoryId = Convert.ToInt32(((Button)sender).Tag);
            var window = new ThirdWindow();
            window.Show();
            if (WindowState == WindowState.Maximized)
                window.WindowState = WindowState.Maximized;
            ((Button)sender).Visibility = Visibility.Hidden;
        }

        private static void Buttons_MouseEnter(object sender, MouseEventArgs e) 
            => ((Button)sender).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Theme.ButtonBackgroundColor)) {
                Opacity = Settings.Theme.ButtonTransparency > 90 ? (Settings.Theme.ButtonTransparency - 10) / 100.0 : (Settings.Theme.ButtonTransparency + 10) / 100.0
            };
        
        private static void Buttons_MouseLeave(object sender, MouseEventArgs e) 
            => ((Button)sender).Background = new SolidColorBrush((Color) ColorConverter.ConvertFromString(Settings.Theme.ButtonBackgroundColor)) {
                Opacity = Settings.Theme.ButtonTransparency / 100.0
            };
    }
}
