using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Controls;
using QuizApplication.Models;

namespace QuizApplication {
    public partial class EditQuestionWindow : Window {
        private int 
            _currentCategoryIndex = 0,
            _currentQuestionIndex = 0;

        private Category
            _currentCategory;

        public EditQuestionWindow() {
            InitializeComponent();

            ButtonClose.Click += CommonMethods.CloseWindow_OnClick;

            ComboBoxChooseCategory.ItemsSource = Settings.Game.CategoriesList.Select(s => new ComboBoxItem() { Content = s });
            ComboBoxChooseCategory.SelectedIndex = 0;
            _currentCategory = Settings.Game.Categories.Where(category => category.Level == Difficulty.Easy).ToArray()[_currentCategoryIndex];

            FillTextBoxes();
        }

        private void ButtonNextQuestion_Click(object sender, RoutedEventArgs e) {
            if (!ButtonPrevQuestion.IsEnabled) ButtonPrevQuestion.IsEnabled = true;
            SaveCurrentQuestion();
            if (++_currentQuestionIndex == _currentCategory.Questions.Count) {
                ButtonNextQuestion.IsEnabled = false;
                _currentQuestionIndex = _currentCategory.Questions.Count - 1;
                return;
            }

            FillTextBoxes();
        }

        private void ButtonPrevQuestion_Click(object sender, RoutedEventArgs e) {
            if (!ButtonNextQuestion.IsEnabled) ButtonNextQuestion.IsEnabled = true; 
            SaveCurrentQuestion();
            if (--_currentQuestionIndex == 0) ButtonPrevQuestion.IsEnabled = false;

            FillTextBoxes();
        }

        void SaveCurrentQuestion() {
            List<string> answers = new List<string>();
            foreach (var itemTextBox in GridAnswers.Children.OfType<TextBox>()) {
                answers.Add(itemTextBox.Text);
            }

            _currentCategory.Questions[_currentQuestionIndex] = new Category.Question(_currentCategory.Questions[_currentQuestionIndex].Id, TextBoxQuestion.Text, answers.ToArray());
        }

        private void ButtonSaveCategory_Click(object sender, RoutedEventArgs e) {
            SaveCurrentQuestion();
            Settings.Game.Categories[_currentCategoryIndex] = _currentCategory;

            var jsonFormatter = new DataContractJsonSerializer(typeof(List<Category>));
            using (var fs = new FileStream("Question.json", FileMode.OpenOrCreate)) {
                jsonFormatter.WriteObject(fs, Settings.Game.Categories);
            }

            MessageBox.Show($"Запитання для категорії {_currentCategory.Name} було оновлено.");
        }

        private void ButtonLoadCategory_Click(object sender, RoutedEventArgs e)
        {
            if (_currentCategoryIndex == ComboBoxChooseCategory.SelectedIndex
                && _currentCategory.Level == (Difficulty) (ComboBoxChooseDifficulty.SelectedIndex + 1))
                return;

            _currentCategoryIndex = ComboBoxChooseCategory.SelectedIndex;
            _currentQuestionIndex = 0;

            _currentCategory = Settings.Game.Categories
                .Where(category 
                    => category.Level == (Difficulty)(ComboBoxChooseDifficulty.SelectedIndex + 1))
                .ToArray()[_currentCategoryIndex];

            FillTextBoxes();
        }

        void FillTextBoxes() {
            TextBoxQuestion.Text = Settings.Game.Categories[_currentCategoryIndex].Questions[_currentQuestionIndex].QuestionText;
            int currentAnswer = 0;
            foreach (var itemTextBox in GridAnswers.Children.OfType<TextBox>()) {
                itemTextBox.Text = string.Empty;
                itemTextBox.Text = _currentCategory.Questions[_currentQuestionIndex]
                    .UnshuffledAnswers[currentAnswer++];
            }
        }
    }
}
