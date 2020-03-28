using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace QuizApplication.Models {
    [DataContract]
    class Theme {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string TextFontFamily { get; set; }
        [DataMember]
        public string TextColor { get; set; }
        
        [DataMember]
        public string ButtonFontFamily { get; set; }
        [DataMember]
        public string ButtonTextColor { get; set; }
        [DataMember]
        public string ButtonBackgroundColor { get; set; }
        [DataMember]
        public int ButtonTransparency { get; set; }
        
        [DataMember]
        public string BackgroudImageUrl { get; set; }
        [DataMember]
        public int WindowSizeWidth { get; set; }
        [DataMember]
        public int WindowSizeHeight { get; set; }

        public static Theme getTheme(string ThemeName) {
            var jsonFormatter = new DataContractJsonSerializer(typeof(Theme));
            using (var fs = new FileStream($"Themes//{Settings.Application.Theme}.Theme.json", FileMode.OpenOrCreate)) {
                return jsonFormatter.ReadObject(fs) as Theme;
            }
        }

        public void SetButtonTransparency(int buttonTransparency) {
            if (buttonTransparency < 0 ||
                buttonTransparency > 100)
                buttonTransparency = 10;
            ButtonTransparency = buttonTransparency;
        }
        public void ApplyConfiguration(Window window) {
            window.Background = new ImageBrush(new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"background\" + BackgroudImageUrl)));
            window.Width = WindowSizeWidth;
            window.Height = WindowSizeHeight;
        }
    }
}
