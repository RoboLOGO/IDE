using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Editor
{
    class NameChecker
    {

        public bool IsUsable(string name)
        {
            name = name.ToLower();

            if (name == String.Empty)
                throw new Exception(App.Current.TryFindResource("nameempty").ToString());

            if (name.Contains(" "))
                throw new Exception(App.Current.TryFindResource("namespace").ToString());

            if (IsNumber(name[0].ToString()))
                throw new Exception(App.Current.TryFindResource("namestartnumber").ToString());

            if (IsAlphaNumeric(name))
                throw new Exception(App.Current.TryFindResource("namenotallowed").ToString());

            if (IsCommand(name))
                throw new Exception(App.Current.TryFindResource("namecommand").ToString());

            if(IsVariableName(name))
                throw new Exception(App.Current.TryFindResource("namevar").ToString());

            if(IsMethodName(name))
                throw new Exception(App.Current.TryFindResource("namemethod").ToString());

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
            return LogoKeywords.GetLogoKeywords().GetKeywords().Contains(name);
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
