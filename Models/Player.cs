using System.Collections.Generic;
using System.Linq;

namespace QuizApplication.Models
{
    public class Player
    {
        /// <summary>
        /// Player Id
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// Difficulty
        /// </summary>
        public Difficulty Level { get; set; }
        /// <summary>
        /// Category Id
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// Number of correct answers
        /// </summary>
        public int NumberOfCorrectAnswers { get; set; }
        /// <summary>
        /// Player score
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// Player list 
        /// </summary>
        public static List<Player> Players = new List<Player>();

        public Player()
        {
            Id = Players.Count();
            Level = Difficulty.Easy;
            NumberOfCorrectAnswers = 0;
            Score = 0;
        }
        
        public string GetStatisticString() 
            => Settings.Game.IsChooseLevel 
                ? $"Гравець №{Id + 1}[{Level}] - {Score}" 
                : $"Гравець №{Id + 1} - {Score}";
    }
}
