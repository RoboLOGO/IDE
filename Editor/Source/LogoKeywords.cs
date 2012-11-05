using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    class LogoKeywords
    {
        // A resource-ban definiált kulcsszavakhoz a key-ek:
        static string[] kw_keys = 
        {
         "kw_elore",
         "kw_hatra",
         "kw_jobbra",
         "kw_balra",
         "kw_haza",
         "kw_xpoz",
         "kw_ypoz",
         "kw_torolkep",
         "kw_tollat",
         "kw_le",
         "kw_fel",
         "kw_ha",
         "kw_ismetles",
         "kw_amig",
        };
        static string kw_spec = "kw_spec";

        public static List<string> GetKeywords()
        {
            List<string> keyWords = new List<string>();
            for (int i = 0; i < kw_keys.Length; i++)
            {
                keyWords.Add(App.Current.TryFindResource(kw_keys[i]).ToString());
            }
            return keyWords;
        }
        public static List<char> GetSpecialCharacters()
        {
            return App.Current.TryFindResource(kw_spec).ToString().ToList<char>();
        }
    }
}
