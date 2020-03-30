using System.Windows;

namespace QuizApplication {
    public partial class StatisticWindow : Window {
        public StatisticWindow() {
            InitializeComponent();

            BtnClose.Click += CommonMethods.CloseWindow_OnClick;
        }
    }
}
