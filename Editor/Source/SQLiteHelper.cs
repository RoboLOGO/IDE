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

            CanvasSize.GetCanvasSize().Height = GetCanvasSize(canvasheightSQL);
            CanvasSize.GetCanvasSize().Width = GetCanvasSize(canvaswidthSQL);

        }

        private int GetCanvasSize(string canvasheightSQL)
        {
            SQLiteCommand command = new SQLiteCommand(canvasheightSQL, sqliteCon);
            SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();
            return (int)reader["name"];
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
            string methodsSQL = "CREATE TABLE Methods (Name VARCHAR(25) NOT NULL, Method VARCHAR)";
            string optionsSQL = "CREATE TABLE Options (Name VARCHAR(25) UNIQUE, Value VARCHAR)";
            command = new SQLiteCommand(methodsSQL, sqliteCon);
            command.ExecuteNonQuery();
            command = new SQLiteCommand(optionsSQL, sqliteCon);
            command.ExecuteNonQuery();

        }

        private void OptionsInit(int canvasHeight, int canvasWidth)
        {
            string cavasHeightSQL = "INSERT INTO Options (Name, Value) VALUES ('canvasheight',"+ canvasHeight +")";
            string cavasWidthSQL = "INSERT INTO Options (Name, Value) VALUES ('canvaswidth'," + canvasWidth + ")";
            command = new SQLiteCommand(cavasHeightSQL, sqliteCon);
            command.ExecuteNonQuery();
            command = new SQLiteCommand(cavasWidthSQL, sqliteCon);
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
