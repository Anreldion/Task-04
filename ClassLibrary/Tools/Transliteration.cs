using System.Collections.Generic;

namespace Messenger.Tools
{
    /// <summary>
    /// Transliteration of a message written in Russian letters into English
    /// </summary>
    public class Transliteration
    {
        /// <summary>
        /// Dictionary containing comparison of Russian characters to English
        /// </summary>
        private static readonly Dictionary<string, string> Conformity = new()
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
        /// Transliteration of a message written in Russian letters into English
        /// </summary>
        /// <param name="message">Message line</param>
        /// <returns>String after transliteration</returns>
        public static string Run(string message)
        {
            var result = "";
            foreach (var ch in message)
            {
                var isUpper = char.IsUpper(ch);
                if (Conformity.TryGetValue(char.ToLower(ch).ToString(), out var engCh))
                {
                    if (isUpper)
                    {
                        result += engCh.ToUpper();
                    }
                    else
                    {
                        result += engCh;
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
