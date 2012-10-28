using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    class LanguageHelper
    {
        static LanguageHelper self = null;

        Dictionary<string, string> words;
        Dictionary<string, string> exeptions;

        private LanguageHelper()
        {
            words = new Dictionary<string, string>();
            exeptions = new Dictionary<string, string>();
            SetWords();
            SetExeptions();
        }

        static public LanguageHelper GetLanguageHelper()
        {
            if (self == null)
                self = new LanguageHelper();
            return self;
        }
        
        public string GetName(string name)
        {
            return words[name.ToLower()];
        }

        public string GetExeption(string name)
        {
            return exeptions[name.ToLower()];
        }

        private void SetWords()
        {
            words.Add("newvarheader", "Változó");
            words.Add("name", "Név");
            words.Add("value", "Érték");
            words.Add("add", "Hozzáad");
            words.Add("newprojectheader","Új Projekt");
            words.Add("canvaswidth", "Vászon szélessége");
            words.Add("canvasheight", "Vászon magassága");
            words.Add("savesource", "Mentés helye:");
            words.Add("browse", "Tallózás");
            words.Add("save", "Mentés");
            words.Add("methodnameheader","Új eljárás");
            words.Add("methodname", "Eljárás neve");
            words.Add("methodheader", "Eljárás/Változó");
            words.Add("remove", "Eltávolítás");
            words.Add("ok", "OK");
            words.Add("methods", "Eljárások");
            words.Add("variables", "Változók");
            words.Add("mainheader", "RoboLOGO");
            words.Add("file", "Fájl");
            words.Add("new", "Új");
            words.Add("open", "Megnyitás");
            words.Add("save", "Mentés");
            words.Add("saveas", "Mentés másként");
            words.Add("exit", "Kilépés");
            words.Add("edit", "Szerkesztés");
            words.Add("undo", "Vissza");
            words.Add("redo", "Előre");
            words.Add("copy", "Másolás");
            words.Add("cut", "Kivágás");
            words.Add("paste", "Beillesztés");
            words.Add("run", "Futtatás");
            words.Add("clean", "Képernyő törlése");
            words.Add("savecanvas", "Rajzvászon mentése");
            words.Add("method", "eljárás");
            words.Add("end", "vége");
            words.Add("saveconfirmation", "Szeretnéd menteni a változások?");
            words.Add("savetext", "Biztosan törlöd a(z)");
            words.Add("deletevariable", "nevű változót");
            words.Add("delvar", "Változó törlése");
            words.Add("deletemethod", "nevű eljárást");
            words.Add("delmet", "Eljárás törlés");
            words.Add("delconfirmation", "Szeretnéd menteni a változások");
        }

        private void SetExeptions()
        {
            exeptions.Add("nameempty","Nem lehet üres a név");
            exeptions.Add("namespace", "Nem lehet szóköz a névben");
            exeptions.Add("namestartnumber", "Nem kezdődhet számmal a név");
            exeptions.Add("namenotallowed", "Nem megengedett karakter a névben");
            exeptions.Add("namecommand", "Nem használható a parancs névként");
            exeptions.Add("namevar", "Ez már egy változó név");
            exeptions.Add("namemethod", "Ez már egy eljárás név");
            exeptions.Add("valueint", "Az érték csak egész típusú lehet");
            exeptions.Add("error", "Hiba");
        }
    }
}
