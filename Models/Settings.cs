using System.Runtime.Serialization;

namespace QuizApplication.Models {
    [DataContract]
    class Settings {
        [DataMember]
        public static GameSettings Game { get; set; }
        [DataMember]
        public static ApplicationSettings Application { get; set; }
        [DataMember]
        public static Theme Theme { get; set; }
    }
}
