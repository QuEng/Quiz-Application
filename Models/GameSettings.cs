using System.Collections.Generic;
using System.Runtime.Serialization;

namespace QuizApplication.Models
{
    [DataContract]
    class GameSettings {
        private GameSettings() { }
        private static GameSettings _instance;
        public static GameSettings GetInstance() { 
            _instance = _instance ?? new GameSettings();
            return _instance;
        }

        public const int NumberOfLevels = 3;

        [DataMember]
        public Difficulty Level { get; set; }
        [DataMember]
        public bool IsChooseLevel { get; set; }
        [DataMember]
        public bool IsMultiScore { get; set; }
        [DataMember]
        public bool IsKeepLastQuestion { get; set; }
        [DataMember]
        public int Time { get; set; }
        [DataMember]
        public bool IsShowAnswers { get; set; }

        public string[] CategoriesList { get; set; }
        public List<Category> Categories = new List<Category>();

        public int ActiveCategoryId { get; set; }

        public void SetLevel(Difficulty level)
        {
            Level = level;
        }
        public void SetTime(int time)
        {
            if (time < 0 || 
                time > 20 * 60)
                time = 90;
            Time = time;
        }
    }
}
