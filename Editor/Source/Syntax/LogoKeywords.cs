using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    class LogoKeywords
    {
        static LogoKeywords self;

        string[] kw_keys;
        string kw_spec;
        List<string> keyWords;

        private LogoKeywords()
        {
            kw_keys = new string[]
            {
                "kw_elore",
                "kw_elorer",
                "kw_hatra",
                "kw_hatrar",
                "kw_jobbra",
                "kw_jobbrar",
                "kw_balra",
                "kw_balrar",
                "kw_haza",
                "kw_xpoz",
                "kw_xpozr",
                "kw_ypoz",
                "kw_ypozr",
                "kw_torolkep",
                "kw_torolkepr",
                "kw_tollatfel",
                "kw_tollatfelr",
                "kw_tollatle",
                "kw_tollatler",
                "kw_ha",
                "kw_ismetles",
                "kw_ismetlesr",
                "kw_amig",
                "kw_spec",
                "kw_absz",
                "kw_eljaras",
                "kw_vege",
                "kw_keszit",
                "kw_orokke",
                "kw_intveletlen",
                "kw_veletlen",
                "kw_stop",
                "kw_igaz",
                "kw_hamis",
                "kw_eredmeny",
                "kw_kiir",
            };
            kw_spec = "kw_spec";
            keyWords = new List<string>();

        }

        public static LogoKeywords GetLogoKeywords()
        {
            if (self == null)
                self = new LogoKeywords();
            return self;
        }

        public List<string> GetKeywords()
        {
            return keyWords;
        }

        public void UpdateKeywords()
        {
            keyWords.Clear();
            for (int i = 0; i < kw_keys.Length; i++)
            {
                keyWords.Add(App.Current.TryFindResource(kw_keys[i]).ToString());
            }
        }

        public List<char> GetSpecialCharacters()
        {
            return App.Current.TryFindResource(kw_spec).ToString().ToList<char>();
        }
    }
}
