using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Editor
{
    class NameChecker
    {

        string[] commands;

        public NameChecker()
        {
            commands = new string[]{
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
                "várj",
                "eljárás",
                "vége",
            };
        }

        public NameChecker(string[] commands)
        {
            this.commands = commands;
        }

        public bool IsUsable(string name)
        {
            name = name.ToLower();
            LanguageHelper lp = LanguageHelper.GetLanguageHelper();

            if (name == String.Empty)
                throw new Exception(lp.GetExeption("nameempty"));

            if (name.Contains(" "))
                throw new Exception(lp.GetExeption("namespace"));

            if (IsNumber(name[0].ToString()))
                throw new Exception(lp.GetExeption("namestartnumber"));

            if (IsAlphaNumeric(name))
                throw new Exception(lp.GetExeption("namenotallowed"));

            if (IsCommand(name))
                throw new Exception(lp.GetExeption("namecommand"));

            if(IsVariableName(name))
                throw new Exception(lp.GetExeption("namevar"));

            if(IsMethodName(name))
                throw new Exception(lp.GetExeption("namemethod"));

            return true;
        }

        private bool IsNumber(string num)
        {
            Regex numbers = new Regex("[^0-9-]");
            Regex notnumbers = new Regex("^-[0-9]+$|^[0-9]+$");
            return !numbers.IsMatch(num) && notnumbers.IsMatch(num);
        }

        private bool IsAlphaNumeric(string name)
        {
            Regex objAlphaNumericPattern = new Regex("[^a-z0-9éáűőúöüóí_]");
            return objAlphaNumericPattern.IsMatch(name); 
        }

        private bool IsCommand(string name)
        {
            return commands.Contains(name);
        }

        private bool IsVariableName(string name)
        {
            return SQLiteHelper.GetSqlHelper.GetAllVariablesName().Contains(name);
        }

        private bool IsMethodName(string name)
        {
            return SQLiteHelper.GetSqlHelper.GetAllMethodName().Contains(name);
        }

    }
}
