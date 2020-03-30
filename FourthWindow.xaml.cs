using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using QuizApplication.Models;

namespace QuizApplication {
    public partial class FourthWindow : Window {
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private int _time,
                    _currentQuestion = 0;
        private readonly Player _player;
        private Category CurrentCategory { get; }
        public FourthWindow() {            
            InitializeComponent();

            this.KeyDown += CommonMethods.KeyEvents;

            Settings.Theme.ApplyConfiguration(this);

            BtnClose.Click += CommonMethods.CloseWindow_OnClick;
            BtnMaxMin.Click += CommonMethods.MaxMin_Click;
            ButtonBack.Click += ButtonBackOnClick;

            _player = new Player {
                Level = Settings.Game.Level,
                CategoryId = Settings.Game.ActiveCategoryId
            };

            LabelCategoryQuestion.Foreground = 
            LabelCategoryName.Foreground     = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Theme.TextColor));
            LabelCategoryQuestion.FontFamily = 
            LabelCategoryName.FontFamily = new FontFamily(Settings.Theme.TextFontFamily);
            LabelCategoryName.Content = $"Категорія: {Settings.Game.CategoriesList[Settings.Game.ActiveCategoryId - 1]}";

            
            CurrentCategory = Settings.Game.Categories[_player.CategoryId - 1];

            if (Settings.Game.IsShowAnswers) {
                GridButtons.Visibility = Visibility.Hidden;
                GridAnswers.Visibility = Visibility.Visible;
            }

            _time = Settings.Game.Time;
            _timer.Tick += new EventHandler(Timer_Tick);
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Start();
            ShowNextQuestion();
        }
        private void Button_AnswerOnClick(object sender, RoutedEventArgs e) {
            if (CurrentCategory.Questions[_currentQuestion - 1].IsCorrectAnswer((sender as Button).Content.ToString())) {
                _player.NumberOfCorrectAnswers++;
                _player.Score += Settings.Game.IsMultiScore ? (int)_player.Level : 1;
            }
            ShowNextQuestion();
        }

        private void Button_TrueAnswerOnClick(object sender, RoutedEventArgs e) {
            _player.NumberOfCorrectAnswers++;
            _player.Score += Settings.Game.IsMultiScore ? (int)_player.Level : 1;
            ShowNextQuestion();
        }

        private void Button_FalseAnswerOnClick(object sender, RoutedEventArgs e) 
            => ShowNextQuestion();

        private void ButtonBackOnClick(object sender, RoutedEventArgs e) {
            Player.Players.Add(_player);
            Application.Current.Windows.OfType<SecondWindow>().Single().Focus();
            Close();
        }

        private void Timer_Tick(object sender, EventArgs e) {
            if (_time != 0) _time--;
            else if (_time == 0) {
                if (Settings.Game.IsKeepLastQuestion) return;
                _timer.Stop();
                LabelCategoryQuestion.Text = "Час закінчився!\nРезультат: " + _player.NumberOfCorrectAnswers + " вірних відповідей!";
                LabelTimer.Content = string.Empty;

                if (Settings.Game.IsShowAnswers) {
                    GridAnswers.Visibility = Visibility.Hidden;
                    GridButtons.Visibility = Visibility.Visible;
                }
                ButtonFailedAnswer.Visibility = Visibility.Hidden;
                ButtonTrueAnswer.Visibility = Visibility.Hidden;
                ButtonBack.Visibility = Visibility.Visible;
                return;
            }

            int minute = _time >= 60 ? (_time / 60) : 0, second = _time % 60;
            string str_minute = Convert.ToString(minute), str_second = second < 10 ? "0" + Convert.ToString(second) : Convert.ToString(second);
            LabelTimer.Content = $"{str_minute}:{str_second}";
        }
        
        public void ShowNextQuestion()
        {
            if ((_time == 0 && Settings.Game.IsKeepLastQuestion) || _currentQuestion == 15) {
                _timer.Stop();
                LabelCategoryQuestion.Text = "Результат: " + _player.NumberOfCorrectAnswers + " вірних відповідей!";
                LabelTimer.Content = string.Empty;

                ButtonFailedAnswer.Visibility = Visibility.Hidden;
                ButtonTrueAnswer.Visibility = Visibility.Hidden;
                GridAnswers.Visibility = Visibility.Hidden;
                GridButtons.Visibility = Visibility.Visible;
                ButtonBack.Visibility = Visibility.Visible;
            } else {
                LabelCategoryQuestion.Text = CurrentCategory.Questions[_currentQuestion].ToString();
                if (Settings.Game.IsShowAnswers) {
                    Button1.Content = CurrentCategory.Questions[_currentQuestion].Answers[0];
                    Button2.Content = CurrentCategory.Questions[_currentQuestion].Answers[1];
                    Button3.Content = CurrentCategory.Questions[_currentQuestion].Answers[2];
                    Button4.Content = CurrentCategory.Questions[_currentQuestion].Answers[3];
                }
                _currentQuestion++;
            }
        }

        private void Buttons_MouseEnter(object sender, MouseEventArgs e) 
            => ((Button)sender).Background.Opacity = 0.8;

        private void Buttons_MouseLeave(object sender, MouseEventArgs e) 
            => ((Button)sender).Background.Opacity = 1;
    }
    
}
