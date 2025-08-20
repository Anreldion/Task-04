using System.Collections.Generic;
using System.Text;

namespace Messenger.Tools
{
    /// <summary>
    /// Transliterator of a message written in Russian letters into English letters
    /// </summary>
    public static class Transliterator
    {
        /// <summary>
        /// Dictionary containing comparison of Russian characters to English
        /// </summary>
        private static readonly Dictionary<char, string> Map = new()
            {
                {'а',"a"},
                {'б',"b"},
                {'в',"v"},
                {'г',"g"},
                {'д',"d"},
                {'е',"e"},
                {'ё',"e"},
                {'ж',"zh"},
                {'з',"z"},
                {'и',"i"},
                {'й',"i"},
                {'к',"k"},
                {'л',"l"},
                {'м',"m"},
                {'н',"n"},
                {'о',"o"},
                {'п',"p"},
                {'р',"r"},
                {'с',"s"},
                {'т',"t"},
                {'у',"u"},
                {'ф',"f"},
                {'х',"kh"},
                {'ц',"ts"},
                {'ч',"ch"},
                {'ш',"sh"},
                {'щ',"sch"},
                {'ы',"y"},
                {'ь',""},
                {'ъ',"ie"},
                {'э',"e"},
                {'ю',"iu"},
                {'я',"ia"},
            };

        /// <summary>
        /// Transliterator of a message written in Russian letters into English letters
        /// </summary>
        /// <param name="message">Message line</param>
        /// <returns>String after transliteration</returns>
        public static string ToLatin(string message)
        {
            var result = new StringBuilder();
            foreach (var ch in message)
            {
                if (Map.TryGetValue(char.ToLower(ch), out var engCh))
                {
                    result.Append(char.IsUpper(ch) ? Capitalize(engCh) : engCh);
                }
                else
                {
                    result.Append(ch);
                }
            }
            return result.ToString();
        }
        private static string Capitalize(string? text)
        {
            if (string.IsNullOrEmpty(text))
                return text ?? string.Empty;

            return char.ToUpper(text[0]) + text.Substring(1);
        }
    }
}
