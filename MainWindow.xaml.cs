using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Configuration;
using System.Windows.Controls;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using QuizApplication.Models;

namespace QuizApplication
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Settings.Game = GameSettings.GetInstance();
            Settings.Application = ApplicationSettings.GetInstance();
            LoadConfiguration();
            Settings.Theme = Theme.getTheme(Settings.Application.Theme);
            Settings.Theme.ApplyConfiguration(this);
            LoadQuestions();
            /*var jsonFormatter = new DataContractJsonSerializer(typeof(List<Category>));
            using (var fs = new FileStream("Question.json", FileMode.OpenOrCreate)) {
                jsonFormatter.WriteObject(fs, Settings.Categories);
            }*/

            BtnClose.Click += CommonMethods.CloseWindow_OnClick;
            BtnMaxMin.Click += CommonMethods.MaxMin_Click;
            CloseButton.Click += CommonMethods.CloseWindow_OnClick;                          
        }
        private static void LoadQuestions()
        {
            var jsonFormatter = new DataContractJsonSerializer(typeof(List<Category>));
            using (var fs = new FileStream("Question.json", FileMode.OpenOrCreate)) {
                Settings.Game.Categories = jsonFormatter.ReadObject(fs) as List<Category>;
            }

            Settings.Game.CategoriesList = Settings.Game.Categories?.Select(x => x.Name).Distinct().ToArray();
        }

        private static void LoadConfiguration()
        {
            var jsonFormatter = new DataContractJsonSerializer(typeof(ApplicationSettings));
            using (var fs = new FileStream("Settings.Application.json", FileMode.OpenOrCreate)) {
                Settings.Application = jsonFormatter.ReadObject(fs) as ApplicationSettings;
            }

            jsonFormatter = new DataContractJsonSerializer(typeof(GameSettings));
            using (var fs = new FileStream("Settings.Game.json", FileMode.OpenOrCreate)) {
                Settings.Game = jsonFormatter.ReadObject(fs) as GameSettings;
                Player.Players = new List<Player>();
            }
        }

        private void StartGame(object sender, RoutedEventArgs e)
        {
            var window = new SecondWindow();
            window.Show();
            if (WindowState == WindowState.Maximized)
                window.WindowState = WindowState.Maximized;
        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow
            {
                Owner = this
            };
            window.ShowDialog();            
        }

        private Brush _foregroundMenu;
        private void MenuButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _foregroundMenu = ((Button)sender).Foreground;
            ((Button)sender).Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF383838"));
        }
        private void MenuButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((Button)sender).Foreground = _foregroundMenu;
        }
    }
}