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
            newmessage = "Error! \nID=" + id + ";Description=" + description + " " + "Line=" + atLine + " Column=" + atColumn;
        }

    }
}
