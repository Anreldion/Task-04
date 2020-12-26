using System;
using System.Collections.Generic;
using System.Text;

namespace Messenger.Classes.ClientClasses.Tools
{
    public class Transliteration
    {
        private static readonly Dictionary<string, string> conformity = new Dictionary<string, string>()
            {
                {"а","a"},
                {"б","b"},
                {"в","v"},
                {"г","g"},
                {"д","d"},
                {"е","e"},
                {"ё","e"},
                {"ж","zh"},
                {"з","z"},
                {"и","i"},
                {"й","i"},
                {"к","k"},
                {"л","l"},
                {"м","m"},
                {"н","n"},
                {"о","o"},
                {"п","p"},
                {"р","r"},
                {"с","s"},
                {"т","t"},
                {"у","u"},
                {"ф","f"},
                {"х","kh"},
                {"ц","ts"},
                {"ч","ch"},
                {"ш","sh"},
                {"щ","shch"},
                {"ы","y"},
                {"ь",""},
                {"ъ","ie"},
                {"э","e"},
                {"ю","iu"},
                {"я","ia"},
            };

        /// <summary>
        /// Транслитерация сообщения, написанного русскими буквами на английский
        /// </summary>
        /// <param name="message">Строка с сообщением</param>
        /// <returns>Строка после транслитерации</returns>
        public static string Run(string message)
        {
            string result = "";
            foreach (var ch in message)
            {
                bool isUpper = char.IsUpper(ch);
                if (conformity.TryGetValue(char.ToLower(ch).ToString(), out string eng_ch))
                {
                    if (isUpper)
                    {
                        result += eng_ch.ToUpper();
                    }
                    else
                    {
                        result += eng_ch;
                    }
                }
                else
                {
                    result += ch;
                }
                
            }
            return result;
        }

    }
}
