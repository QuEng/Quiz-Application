using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace QuizApplication.Models
{
    [DataContract]
    public sealed class Category {
        /// <summary>
        /// Category id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Category name
        /// </summary>
        [DataMember]
        public readonly string Name;

        /// <summary>
        /// Category difficulty
        /// </summary>
        [DataMember]
        public readonly Difficulty Level;

        /// <summary>
        /// A list of questions
        /// </summary>
        [DataMember]
        public List<Question> Questions { get; set; }

        /// <summary>
        /// Category ctor
        /// </summary>
        /// <param name="name">Category name</param>
        /// <param name="level">Category difficulty</param>
        /// <param name="questions">A list of questions</param>
        public Category(int id, string name, Difficulty level, ICollection<Question> questions) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentNullException("Category name cannot be empty", nameof(Name));
            }

            Id = id;
            Name = name;
            Level = level;
            Questions = questions.ToList();
        }

        public override string ToString() => Name;
        [DataContract]
        public sealed class Question {
            public int Id;
            private string[] _answers;
            [DataMember]
            private string _correctAnswer;
            [DataMember]
            public string QuestionText { get; private set; }
            [DataMember]
            public string[] Answers {
                get => _answers;
                private set {
                    //Shuffle array
                    _correctAnswer = value[0];
                    int[] randomIndex = { 0, 1, 2, 3 };
                    var n = randomIndex.Length;
                    while (n > 1) {
                        var k = new Random(Guid.NewGuid().GetHashCode()).Next(n--);
                        var temp = randomIndex[n];
                        randomIndex[n] = randomIndex[k];
                        randomIndex[k] = temp;
                    }
                    _answers = new[] { value[randomIndex[0]], value[randomIndex[1]], value[randomIndex[2]], value[randomIndex[3]] };
                }
            }

            public Question(int id, string questionText, string[] answers) {
                Id = id;
                QuestionText = questionText;
                Answers = answers;
            }

            public bool IsCorrectAnswer(string answer)
                => string.Equals(answer, _correctAnswer);

            public override string ToString()
                => QuestionText;
        }
    }

    public enum Difficulty {
        Easy = 1,
        Medium, 
        Hard
    }
}
