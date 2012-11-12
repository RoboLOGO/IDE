using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Exeptions
{
    class RPExeption : Exception
    {
        string newmessage;
        public string NewMessage
        {
            get { return newmessage; }
        }
        public RPExeption(int id, string description, int atLine = 1, int atColumn = 1)
        {
            newmessage = App.Current.TryFindResource("error").ToString() + "! \nID = " + id + "\n" + App.Current.TryFindResource("descript").ToString() + ": " + description + "\n" + App.Current.TryFindResource("line").ToString() + ": " + atLine + "\n" + App.Current.TryFindResource("col").ToString() + ": " + atColumn;
        }

    }
}
