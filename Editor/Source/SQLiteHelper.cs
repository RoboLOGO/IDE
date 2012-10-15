using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace Editor
{
    //Test OpenFIle
    //New method, Get method, Get Canvas Size, source code
    class SQLiteHelper
    {
        static SQLiteHelper self = null;
        SQLiteConnection sqliteCon = null;
        SQLiteCommand command = null;
        protected SQLiteHelper()
        {

        }

        static public SQLiteHelper GetSqlHelper()
        {
            if(self == null)
            {
                self = new SQLiteHelper();
            }
            return self;
        }

        public void NewFile(string filesource, int canvasHeight, int canvasWidth)
        {
            SetConnection(filesource);
            CreateFile(filesource);
            Open();
            CreateStruct(canvasHeight, canvasWidth);
        }

        public string GetSourceCode()
        {
            return GetValue("SELECT Value FROM Options WHERE Name='sourcecode'");
        }


        public void SetSourceCode(string source)
        {
            string sourceSQL = "UPDATE Options SET Value='" + source + "' WHERE Name='sourcecode'";
            SetValue(sourceSQL);
        }

        public void OpenFile(string filesource)
        {
            SetConnection(filesource);
            Open();
            SetCanvasSize();

        }

        private void CreateFile(string filesource)
        {
            try
            {
                SQLiteConnection.CreateFile(filesource);
            }
            catch
            {
            }
        }

        public void SetCanvasSize()
        {
            string canvasheightSQL = "SELECT Value FROM Options WHERE Name='canvasheight'";
            string canvaswidthSQL = "SELECT Value FROM Options WHERE Name='canvaswidth'";

            CanvasSize.GetCanvasSize().Height = int.Parse(GetValue(canvasheightSQL));
            CanvasSize.GetCanvasSize().Width = int.Parse(GetValue(canvaswidthSQL));

        }

        private string GetValue(string nameSQL)
        {
            SQLiteCommand command = new SQLiteCommand(nameSQL, sqliteCon);
            SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();
            return (string)reader["value"];
        }

        private void SetValue(string nameSQL)
        {
            command = new SQLiteCommand(nameSQL, sqliteCon);
            command.ExecuteNonQuery();
        }

        public void Connect()
        {
            if (sqliteCon != null)
            {
                sqliteCon.Open();
            }
        }

        public bool IsOpen()
        {
            if (sqliteCon == null) return false;
            return(System.Data.ConnectionState.Open == sqliteCon.State); 
        }

        private void Open()
        {
            sqliteCon.Open();
        }

        private void Close()
        {
            sqliteCon.Close();
        }

        private void SetConnection(string filesource)
        {
            sqliteCon = new SQLiteConnection("Data Source=" + filesource + ";Version=3;");
        } 

        private void CreateStruct(int canvasHeight, int canvasWidth)
        {
            TablesInit();
            OptionsInit(canvasHeight, canvasWidth);
        }

        private void TablesInit()
        {
            string methodsSQL = "CREATE TABLE Methods (Name VARCHAR(25) NOT NULL, Method TEXT)";
            string optionsSQL = "CREATE TABLE Options (Name VARCHAR(25) UNIQUE, Value TEXT)";
            command = new SQLiteCommand(methodsSQL, sqliteCon);
            command.ExecuteNonQuery();
            command = new SQLiteCommand(optionsSQL, sqliteCon);
            command.ExecuteNonQuery();

        }

        private void OptionsInit(int canvasHeight, int canvasWidth)
        {
            string cavasHeightSQL = "INSERT INTO Options (Name, Value) VALUES ('canvasheight',"+ canvasHeight +")";
            string cavasWidthSQL = "INSERT INTO Options (Name, Value) VALUES ('canvaswidth'," + canvasWidth + ")";
            string sourceCode = "INSERT INTO Options (Name, Value) VALUES ('sourcecode', '')";
            command = new SQLiteCommand(cavasHeightSQL, sqliteCon);
            command.ExecuteNonQuery();
            command = new SQLiteCommand(cavasWidthSQL, sqliteCon);
            command.ExecuteNonQuery();
            command = new SQLiteCommand(sourceCode, sqliteCon);
            command.ExecuteNonQuery();
        }

        ~SQLiteHelper()
        {
            try
            {
                Close();
            }
            catch
            {
            }
        }

    }
}
