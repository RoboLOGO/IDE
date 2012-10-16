using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace Editor.SQLite
{
    class SQLiteReader
    {
        public string ExecuteReader(string commandSQL, string returnColumn, SQLiteConnection connection)
        {
            SQLiteCommand command = new SQLiteCommand(commandSQL, connection);
            SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();
            return (string)reader[returnColumn];
        }
    }
}
