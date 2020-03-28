using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using QuizApplication.Models;

namespace QuizApplication {
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
                var fileName = openFileDialog.SafeFileName;
                if (fileName.Length > 28)
                    fileName = fileName.Substring(0, 28).PadLeft(30, '.');
                LabelSelectFile.Content = fileName;
            }
        }

        private void MenuButton_OnClick(object sender, RoutedEventArgs e) {
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

        private void ButtonExit_OnClick(object sender, RoutedEventArgs e) 
            => Close();

        private void ButtonApply_OnClick(object sender, RoutedEventArgs e) {
            if (_activePanel == InterfacePanel) {
                Settings.Application.Theme = ComboBoxChooseTheme.Text;
                if (ComboBoxChooseTheme.SelectedIndex == 2) {
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
                } else {
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
                Settings.Game.Level = (Difficulty)ComboBoxChooseLevel.SelectedIndex + 1;
                Settings.Game.SetTime(Convert.ToInt32(TextBoxTime.Text));

                var jsonFormatter = new DataContractJsonSerializer(typeof(GameSettings));
                using (var fs = new FileStream("Settings.Game.json", FileMode.OpenOrCreate)) {
                    jsonFormatter.WriteObject(fs, Settings.Game);
                }
            }
            Close();
        }

        private void ButtonInformation_Click(object sender, RoutedEventArgs e) {
            string informationText = string.Empty,
                informationCaption = string.Empty;
            if (_activePanel == InterfacePanel) {
                informationCaption = "Інтерфейс";
                informationText = "Тема. Ви можете обрати один із трьох запропонованих варіантів: Light, Dark, Custom. Вибравши Light або Dark буде застосовано набір заздалегіть обраних налаштувань, але, якщо ви оберете кастомізовану(Custom) тему, то зможете налаштувати вигляд програми власноруч.\n\n\n" +
                                  "=> Фон. Ви можете встановити будь-яке фонове зображення. Проте не забудьте кастомізувати інші елементи, щоб інтерфейс програми виглядав гармонічно.\n\n" +
                                  "=> Шрифт тексту. Із запропонованого списку ви можете обрати шрифт, який буде застосовано до основного тексту програми(Назви категорій, запитання).\n" +
                                  "=> Колір тексту. Ви маєте можливість встановити будь-який колір для основного тексту(Назви категорій, запитання).\n\n" +
                                  "=> Шрифт тексту на кнопках. Аналогічно до шрифту тексту, ви можете обрати шрифт для кнопок в головному меню категорій із запропонованого списку.\n" +
                                  "=> Колір тексту на кнопках. Аналогічно до кольору тексту ви маєте можливість встановити будь-який колір для тексту на кнопках в головному меню категорій.\n" +
                                  "=> Колір кнопок. Ви можете налаштувати безпосередньо колір кнопок.\n" +
                                  "=> Прозорість кнопок. Ви можете налаштувати прозорість кнопок. Значення задається від 0 до 100. Де 100 - це 100% кольору.";
            } else if (_activePanel == OtherSettingsPanel) {
                informationCaption = "Інші налаштування";
                informationText = "=> Рівень складності. Програмою передбачено 3 рівня складності(Легкий, середній, важкий).\n" +
                                  "=> Ви можете обрати рівень складності для усіх категорій або обирати його кожного разу перед бліцом запитань.\n" +
                                  "=> Програма веде підрахунок очок для кожного гравця. За замовчуванням за одну правильну відповідь присвоюється 1 очко. Поставивши галочку, гравці будуть отримувати 1, 2, 3 очка за легкий, середній та складний рівень відповідно.\n" +
                                  "=> Ви можете встановити час, який буде відведено кожному гравцеві на бліц.\n" +
                                  "=> Якщо час закінчився, гру буде припинено, але ви маєте можливість налаштувати цю ситуцію та залишити останнє запитання, незважаючи на те, що час закінчився.\n" +
                                  "=> Програмою передбачено два варіанти гри. Перший - із варіантами відповідей, другий - без варіантів відповідей.";
            }
            MessageBox.Show(informationText, $"Довідка: {informationCaption}", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }

}
