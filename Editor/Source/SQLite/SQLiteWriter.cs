using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace Editor
{
    class SQLiteWriter
    {
        public void ExecuteQuery(string commandSQL, SQLiteConnection connection)
        {
            SQLiteCommand command = new SQLiteCommand(commandSQL, connection);
            command.ExecuteNonQuery();
        }
    }
}
