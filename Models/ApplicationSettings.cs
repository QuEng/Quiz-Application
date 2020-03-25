using System.Runtime.Serialization;

namespace QuizApplication.Models {
    [DataContract]
    class ApplicationSettings {
        private ApplicationSettings() { }
        private static ApplicationSettings _instance;

        public static ApplicationSettings GetInstance() {
            _instance = _instance ?? new ApplicationSettings();
            return _instance;
        }
        [DataMember]
        public string Theme { get; set; }

    }
}
