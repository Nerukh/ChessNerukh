using System.Collections.Generic;

namespace Chess22
{
    public static class Language
    {
        private static string _currentLanguage = "ua"; // Мова за замовчуванням

        private static readonly Dictionary<string, Dictionary<string, string>> Translations =
    new Dictionary<string, Dictionary<string, string>>
    {
        { "en", new Dictionary<string, string>
            {
                { "Registration", "Registration" },
                { "Name", "Enter Name:" },
                { "Password", "Enter Password:" },
                { "Network", "In Network:" },
                { "Play", "In Game:" },
                { "Start", "Start" }
            }
        },
        { "ua", new Dictionary<string, string>
            {
                { "Registration", "Реєстрація" },
                { "Name", "Введіть ім'я:" },
                { "Password", "Введіть Пароль:" },
                { "Network", "В мережі:" },
                { "Play", "В грі:" },
                { "Start", "Запустити" },
                { "Error_Registration", "ім'я таке наразі існує!"},
                { "Error_Password", "пароль не підходить до акаунта або не існує такого акаунта!"},
                {"sing in", "Заєрестрування" },
                {"login", "вхід" },
            }
        }
    };


        public static string GetTranslation(string key)
        {
            if (Translations.ContainsKey(_currentLanguage) && Translations[_currentLanguage].ContainsKey(key))
            {
                return Translations[_currentLanguage][key];
            }
            return key; 
        }


        public static void SetLanguage(string language)
        {
            if (Translations.ContainsKey(language))
            {
                _currentLanguage = language;
            }
        }
    }
}
