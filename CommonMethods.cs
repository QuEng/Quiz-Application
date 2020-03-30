using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Input;
using QuizApplication.Models;

namespace QuizApplication {
    class CommonMethods {
        private static Key _oldKey;

        public static void MaxMin_Click(object sender, RoutedEventArgs e) {
            foreach (Window window in Application.Current.Windows) {
                window.WindowState = window.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
            }
        }
        public static void CloseWindow_OnClick(object sender, RoutedEventArgs e) {
            Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive)?.Close();
        }
        public static void KeyEvents(object sender, KeyEventArgs e) {
            var currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            if (_oldKey == Key.LeftCtrl && e.Key == Key.Q) {
                
                if (Player.Players.Count > 0)
                {
                    var window = new StatisticWindow {
                        Owner = currentWindow
                    };

                    List<Player> sortedPlayers = Player.Players.OrderByDescending(player => player.Score).ToList();

                    int gridCounter = 0,
                        labelCounter = 0;
                    foreach (var grid in window.StatisticGrid.Children.OfType<System.Windows.Controls.Grid>()) {
                        if (gridCounter == Player.Players.Count) break;
                        foreach (var label in grid.Children.OfType<System.Windows.Controls.Label>()) {
                            if (labelCounter % 2 == 0) {
                                label.Content = $"Гравець {sortedPlayers[gridCounter].Id + 1}";
                            } else {
                                label.Content = $"{sortedPlayers[gridCounter].Score}";
                            }

                            labelCounter++;
                        }
                        gridCounter++;
                    }

                    window.ShowDialog();
                }

            }

            _oldKey = e.Key;
        }
        public static void UpdateProperty(string key, string value) {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        public static string[] GetCyrillicFontFamilies() {
            var cyrillicFamilies = new List<string>();
            const string RUSSIAN_ABC = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            var lang = System.Windows.Markup.XmlLanguage.GetLanguage("en-us");

            var fontFamilies = System.Windows.Media.Fonts.GetFontFamilies(Environment.GetFolderPath(Environment.SpecialFolder.Fonts));
            foreach (System.Windows.Media.FontFamily ff in fontFamilies) {
                var typefaces = ff.GetTypefaces();
                foreach (System.Windows.Media.Typeface typeface in typefaces) {
                    System.Windows.Media.GlyphTypeface glyph;
                    if (!typeface.TryGetGlyphTypeface(out glyph)) continue;
                    bool isCyrillic = RUSSIAN_ABC.All(ch => { ushort temp; return glyph.CharacterToGlyphMap.TryGetValue((int)ch, out temp); });
                    if (isCyrillic) {
                        string familyName;
                        ff.FamilyNames.TryGetValue(lang, out familyName);
                        cyrillicFamilies.Add(familyName);
                        break;
                    }
                }
            }
            cyrillicFamilies.Sort();
            return cyrillicFamilies.ToArray();
        }
    }
}
