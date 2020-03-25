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
using QuizApplication.Models;

namespace QuizApplication
{
    public partial class ThirdWindow : Window
    {
        private readonly ImageSource[] _imageSources = new ImageSource[3];
        public ThirdWindow()
        {
            InitializeComponent();
            Settings.Theme.ApplyConfiguration(this);

            BtnClose.Click += CommonMethods.CloseWindow_OnClick;
            BtnMaxMin.Click += CommonMethods.MaxMin_Click;

            Label_CategoryName.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Theme.TextColor));
            Label_CategoryName.FontFamily = new FontFamily(Settings.Theme.TextFontFamily); 
            Label_CategoryName.Content = $"Категорія: {Settings.Game.CategoriesList[Settings.Game.ActiveCategoryId - 1]}";

            if (Settings.Game.IsChooseLevel)
            {
                Grid_ChooseLevel.Visibility = Visibility.Visible;
                switch (Settings.Game.Level)
                {
                    case Difficulty.Easy:
                        Image_Star_1.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/star-full.png"));
                        break;
                    case Difficulty.Medium:
                        Image_Star_1.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/star-full.png"));
                        Image_Star_2.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/star-full.png"));
                        break;
                    case Difficulty.Hard:
                        Image_Star_1.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/star-full.png"));
                        Image_Star_2.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/star-full.png"));
                        Image_Star_3.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/star-full.png"));
                        break;
                    default:
                        break;
                }
                _imageSources[0] = Image_Star_1.Source;
                _imageSources[1] = Image_Star_2.Source;
                _imageSources[2] = Image_Star_3.Source;
            }

            Button_start.Style = Resources["btnStyle"] as Style;

            Button_start.Foreground =
                new SolidColorBrush((Color) ColorConverter.ConvertFromString(Settings.Theme.ButtonTextColor));
            Button_start.FontFamily = new FontFamily(Settings.Theme.ButtonFontFamily);
            Button_start.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Theme.ButtonBackgroundColor)) {
                Opacity = Settings.Theme.ButtonTransparency / 100.0
            };

            Button_start.Click += Button_start_OnClick;
            Button_start.MouseEnter += Button_start_MouseEnter;
            Button_start.MouseLeave += Button_start_MouseLeave;
        }
         private void Button_start_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new FourthWindow();
            window.Show();
            if (WindowState == WindowState.Maximized)
                window.WindowState = WindowState.Maximized;
            Close();           
        }
        
        private void Image_Star_OnClick(object sender, MouseButtonEventArgs e)
        {
            Settings.Game.SetLevel((Difficulty)Convert.ToInt32(((Image)sender).Tag));
            UpdateStars((int)Settings.Game.Level);
        }
        private void UpdateStars(int level)
        {
            switch (level)
            {
                case 1:
                    _imageSources[0] = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/star-full.png"));
                    _imageSources[1] = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/star-empty.png"));
                    _imageSources[2] = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/star-empty.png"));
                    break;
                case 2:
                    _imageSources[0] = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/star-full.png"));
                    _imageSources[1] = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/star-full.png"));
                    _imageSources[2] = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/star-empty.png"));
                    break;
                case 3:
                    _imageSources[0] = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/star-full.png"));
                    _imageSources[1] = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/star-full.png"));
                    _imageSources[2] = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/star-full.png"));
                    break;
                default:
                    break;
            }
            Image_Star_1.Source = _imageSources[0];
            Image_Star_2.Source = _imageSources[1];
            Image_Star_3.Source = _imageSources[2];
        }
        
        private void PaintStars(int level)
        {
            switch (level)
            {
                case 1:
                    PaintStar(Image_Star_1, 1);
                    UnPaintStar(Image_Star_2, 2);
                    UnPaintStar(Image_Star_3, 3);
                    break;
                case 2:
                    PaintStar(Image_Star_1, 1);
                    PaintStar(Image_Star_2, 2);
                    UnPaintStar(Image_Star_3, 3);
                    break;
                case 3:
                    PaintStar(Image_Star_1, 3);
                    PaintStar(Image_Star_2, 3);
                    PaintStar(Image_Star_3, 3);
                    break;
            }
        }

        private void PaintStar(Image img, int numberOfStar)
        {
            _imageSources[numberOfStar - 1] = img.Source;
            img.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/star-full.png"));
        }

        private void UnPaintStar(Image img, int numberOfStar)
        {
            _imageSources[numberOfStar - 1] = img.Source;
            img.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/star-empty.png"));
        }

        private void Image_Star_MouseEnter(object sender, MouseEventArgs e)
        {
            PaintStars(Convert.ToInt32(((Image)sender).Tag));
        }
        private void Image_Star_MouseLeave(object sender, MouseEventArgs e)
        {
            Image_Star_1.Source = _imageSources[0];
            Image_Star_2.Source = _imageSources[1];
            Image_Star_3.Source = _imageSources[2];
        }


        private static void Button_start_MouseEnter(object sender, MouseEventArgs e)
        {
            var color = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Theme.ButtonBackgroundColor))
            {
                Opacity = Settings.Theme.ButtonTransparency > 90 ? (Settings.Theme.ButtonTransparency - 10) / 100.0 : (Settings.Theme.ButtonTransparency + 10) / 100.0
            };
            ((Button)sender).Background = color;
        }
        private static void Button_start_MouseLeave(object sender, MouseEventArgs e)
        {
            var color = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Theme.ButtonBackgroundColor))
            {
                Opacity = Settings.Theme.ButtonTransparency / 100.0
            };
            ((Button)sender).Background = color;
        }
    }
}
