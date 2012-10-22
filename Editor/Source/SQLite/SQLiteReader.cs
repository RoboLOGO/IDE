using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace Editor
{
    class SQLiteReader
    {
        public string ExecuteOneReader(string commandSQL, string returnColumn, SQLiteConnection connection)
        {
            SQLiteCommand command = new SQLiteCommand(commandSQL, connection);
            SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();
            return (string)reader[returnColumn];
        }

        public List<string> ExecuteMoreReader(string commandSQL, string returnColumn, SQLiteConnection connection)
        {
            List<string> items = new List<string>();
            SQLiteCommand command = new SQLiteCommand(commandSQL, connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add((string)reader["name"]);
            }
            return items;
        }
    }
}
