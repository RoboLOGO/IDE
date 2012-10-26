using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    class SyntaxHighlight
    {
        static List<string> tags = new List<string>();
        static List<char> specials = new List<char>();
        static SyntaxHighlight()
        {
            string[] strs = {
                "előre",
                "hátra",
                "jobbra",
                "balra",
                "haza",
                "xpoz",
                "ypoz",
                "törölkép",
                "tollatle",
                "tollatfel",
                "ha",
                "ismétlés",
                "amíg",
                "eljárás",
                "vége",
            };
            tags = new List<string>(strs);

            char[] chrs = {
                '.',
                ')',
                '(',
                '[',
                ']',
            };
            specials = new List<char>(chrs);
        }
        public static List<char> GetSpecials
        {
            get { return specials; }
        }
        public static List<string> GetTags
        {
            get { return tags; }
        }
        public static bool IsKnownTag(string tag)
        {
            return tags.Exists(delegate(string s) { return s.ToLower().Equals(tag.ToLower()); });
        }
        public static List<string> GetLogoProvider(string tag)
        {
            return tags.FindAll(delegate(string s) { return s.ToLower().StartsWith(tag.ToLower()); });
        }
    }
}
