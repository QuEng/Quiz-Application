using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using QuizApplication.Models;

namespace QuizApplication {
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private Button _activeMenuButton;
        private ScrollViewer _activePanel;

        public SettingsWindow() {
            InitializeComponent();
            ApplySettingsForWindow();
            _activeMenuButton = InterfaceMenuButton;
            _activePanel = InterfacePanel;
        }
        private void Button_LoadImageOnClick(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Фон";
            openFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.PNG;)|*.BMP;*.JPG;*.PNG;";
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + @"background";
            if (openFileDialog.ShowDialog() == true) {
                string filePath = openFileDialog.FileName;
                string path = AppDomain.CurrentDomain.BaseDirectory + @"background\" + openFileDialog.SafeFileName;
                if (filePath != path)
                    File.Copy(filePath, path);
                Settings.Theme.BackgroudImageUrl = openFileDialog.SafeFileName;
                LabelSelectFile.Content = openFileDialog.SafeFileName;
            }
        }

        private void MenuButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (_activeMenuButton == button) return;
            _activeMenuButton.Background = Brushes.Transparent;
            _activeMenuButton = button;
            _activeMenuButton.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF28C182");

            _activePanel.Visibility = Visibility.Hidden;
            _activePanel = _activePanel == InterfacePanel ? OtherSettingsPanel : InterfacePanel;
            _activePanel.Visibility = Visibility.Visible;
        }
        private void ApplySettingsForWindow() {
            //Interface settings
            var aFontsCollection = CommonMethods.GetCyrillicFontFamilies().ToList();
            aFontsCollection.Sort();
            ComboBoxTextsFont.ItemsSource = aFontsCollection;
            ComboBoxTextsFont.SelectedValue = aFontsCollection.Single(family => family == Settings.Theme.TextFontFamily);
            ComboBoxButtonsFont.ItemsSource = aFontsCollection;
            ComboBoxButtonsFont.SelectedValue = aFontsCollection.Single(family => family == Settings.Theme.ButtonFontFamily);
            ClrPicker_ButtonsTextColor.SelectedColor = (Color)ColorConverter.ConvertFromString(Settings.Theme.ButtonTextColor);
            ClrPicker_ColorText.SelectedColor = (Color)ColorConverter.ConvertFromString(Settings.Theme.TextColor);
            ClrPicker_ColorButtons.SelectedColor = (Color)ColorConverter.ConvertFromString(Settings.Theme.ButtonBackgroundColor);
            TextBoxButtonTransparency.Text = Settings.Theme.ButtonTransparency.ToString();
            ComboBoxChooseTheme.SelectedIndex = Settings.Theme.Id - 1;

            //Other settings
            ComboBoxChooseLevel.SelectedIndex = (int)Settings.Game.Level - 1;
            CheckBoxChooseLevel.IsChecked = Settings.Game.IsChooseLevel;
            CheckBoxMultiScore.IsChecked = Settings.Game.IsMultiScore;
            TextBoxTime.Text = Settings.Game.Time.ToString();
            CheckBoxKeepLastQuestion.IsChecked = Settings.Game.IsKeepLastQuestion;
            CheckBoxShowAnswers.IsChecked = Settings.Game.IsShowAnswers;
        }

        private void ButtonExit_OnClick(object sender, RoutedEventArgs e) => Close();

        private void ButtonApply_OnClick(object sender, RoutedEventArgs e)
        {
            if (_activePanel == InterfacePanel)
            {
                Settings.Application.Theme = ComboBoxChooseTheme.Text;
                if (ComboBoxChooseTheme.SelectedIndex == 2)
                {
                    Settings.Theme.Id = 3;
                    Settings.Theme.TextFontFamily = ComboBoxTextsFont.Text;
                    Settings.Theme.TextColor = ClrPicker_ColorText.SelectedColorText;
                    Settings.Theme.ButtonFontFamily = ComboBoxButtonsFont.Text;
                    Settings.Theme.ButtonTextColor = ClrPicker_ButtonsTextColor.SelectedColorText;
                    Settings.Theme.ButtonBackgroundColor = ClrPicker_ColorButtons.SelectedColorText;
                    Settings.Theme.SetButtonTransparency(Convert.ToInt32(TextBoxButtonTransparency.Text));

                    string path = AppDomain.CurrentDomain.BaseDirectory + @"background\" + Settings.Theme.BackgroudImageUrl;
                    if (Application.Current.MainWindow.Background != new ImageBrush(new BitmapImage(new Uri(path)))) {
                        Application.Current.MainWindow.Background = new ImageBrush(new BitmapImage(new Uri(path)));
                        
                    }
                    var jsonFormatter = new DataContractJsonSerializer(typeof(Theme));
                    using (var fs = new FileStream("Themes//Custom.Theme.json", FileMode.OpenOrCreate)) {
                        jsonFormatter.WriteObject(fs, Settings.Theme);
                    }
                }
                else
                {
                    Settings.Theme = Theme.getTheme(Settings.Application.Theme);
                    Settings.Theme.ApplyConfiguration(Application.Current.MainWindow);
                }

                using (var fs = new FileStream("Settings.Application.json", FileMode.OpenOrCreate)) {
                    new DataContractJsonSerializer(typeof(ApplicationSettings)).WriteObject(fs, Settings.Application);
                }
            } else if (_activePanel == OtherSettingsPanel) {
                Settings.Game.IsChooseLevel = (bool)CheckBoxChooseLevel.IsChecked;
                Settings.Game.IsMultiScore = (bool)CheckBoxMultiScore.IsChecked;
                Settings.Game.IsKeepLastQuestion = (bool)CheckBoxKeepLastQuestion.IsChecked;
                Settings.Game.IsShowAnswers = (bool)CheckBoxShowAnswers.IsChecked;
                Settings.Game.SetLevel((Difficulty)ComboBoxChooseLevel.SelectedIndex + 1);
                Settings.Game.SetTime(Convert.ToInt32(TextBoxTime.Text));

                var jsonFormatter = new DataContractJsonSerializer(typeof(GameSettings));
                using (var fs = new FileStream("Settings.Game.json", FileMode.OpenOrCreate)) {
                    jsonFormatter.WriteObject(fs, Settings.Game);
                }
            }
            Close();
        }
    }

}
