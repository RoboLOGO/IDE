using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using Editor.SQLite;

namespace Editor
{
    class SQLiteHelper
    {
        static SQLiteHelper self;

        SQLiteConnection sqliteCon;
        SQLiteReader sqlitereader;
        SQLiteWriter sqlitewriter;

        string _filesource;

        protected SQLiteHelper()
        {
            sqlitereader = new SQLiteReader();
            sqlitewriter = new SQLiteWriter();
        }

        public static SQLiteHelper GetSqlHelper
        {
            get
            {
                if (self == null) self = new SQLiteHelper();
                return self;
            }
        }

        public void NewFile(string filesource, int canvasHeight, int canvasWidth, string name, string project, string language)
        {
            this.FileSource = filesource;
            SetConnection(this.FileSource);
            CreateFile(this.FileSource);
            Open();
            CreateStruct(canvasHeight, canvasWidth, name, project, language);
        }

        public string FileSource
        {
            get { return _filesource; }
            internal set { _filesource = value; } 
        }

        public string GetSourceCode()
        {
            string sourceSQL = "SELECT Value FROM Options WHERE Name='sourcecode'";
            return sqlitereader.ExecuteOneReader(sourceSQL, "value", sqliteCon);
        }

        public void NewMethod(string name, string code)
        {
            string methodSQL = "INSERT INTO Methods (Name, Method) VALUES ('" + name.ToLower() + "','" + code + "')";
            sqlitewriter.ExecuteQuery(methodSQL, sqliteCon);
        }

        public string GetName()
        {
            string nameSQL = "SELECT Method FROM Methods WHERE Name='name'";
            return sqlitereader.ExecuteOneReader(nameSQL, "value", sqliteCon);
        }

        public string GetProjectName()
        {
            string projectSQL = "SELECT Method FROM Methods WHERE Name='project'";
            return sqlitereader.ExecuteOneReader(projectSQL, "value", sqliteCon);
        }

        public string GetLanguage()
        {
            string langSQL = "SELECT Method FROM Methods WHERE Name='language'";
            return sqlitereader.ExecuteOneReader(langSQL, "value", sqliteCon);
        }

        public string GetMethod(string name)
        {
            string methodSQL = "SELECT Method FROM Methods WHERE Name='" + name + "'";
            return sqlitereader.ExecuteOneReader(methodSQL, "method", sqliteCon);
        }

        public void DeleteMethod(string name)
        {
            string methodSQL = "DELETE FROM Methods WHERE Name='" + name +"'";
            sqlitewriter.ExecuteQuery(methodSQL, sqliteCon);
        }

        public List<string> GetAllMethodName()
        {
            string getallmethodSQL = "SELECT Name FROM Methods";
            return sqlitereader.ExecuteMoreReader(getallmethodSQL, "name", sqliteCon);
        }

        public void NewVariable(string name, int value)
        {
            string variableSQL = "INSERT INTO Variables (Name, Value) VALUES ('" + name.ToLower() + "'," + value + ")";
            sqlitewriter.ExecuteQuery(variableSQL, sqliteCon);
        }

        public int GetVariable(string name)
        {
            string variableSQL = "SELECT Value FROM Variables WHERE Name='" + name + "'";
            return int.Parse(sqlitereader.ExecuteOneReader(variableSQL, "value", sqliteCon));
        }

        public List<string> GetAllVariablesName()
        {
            string getallvariablesSQL = "SELECT Name FROM Variables";
            return sqlitereader.ExecuteMoreReader(getallvariablesSQL, "name", sqliteCon);
        }

        public void UpdateVariable(string name, int value)
        {
            string variableSQL = "UPDATE Variables SET Value =" + value + " WHERE Name='" + name + "'";
            sqlitewriter.ExecuteQuery(variableSQL, sqliteCon);
        }

        public void DeleteVariable(string name)
        {
            string variableSQL = "DELETE FROM Variables WHERE Name='" + name + "'";
            sqlitewriter.ExecuteQuery(variableSQL, sqliteCon);
        }

        public void UpdateMethod(string name, string source)
        {
            string methodSQL = "UPDATE Methods SET Method ='" + source + "' WHERE Name='" + name + "'";
            sqlitewriter.ExecuteQuery(methodSQL, sqliteCon);
        } 

        public void SetSourceCode(string source)
        {
            string sourceSQL = "UPDATE Options SET Value='" + source + "' WHERE Name='sourcecode'";
            sqlitewriter.ExecuteQuery(sourceSQL, sqliteCon);
        } 

        public void OpenFile(string filesource)
        {
            if (sqliteCon != null) sqliteCon.Close();
            this.FileSource = filesource;
            SetConnection(this.FileSource);
            Open();
            SetCanvasSize();
        } 

        private void CreateFile(string filesource)
        {
            try
            {
                if (File.Exists(filesource))
                {
                    File.Delete(filesource);
                    System.Threading.Thread.Sleep(2000);
                }
                SQLiteConnection.CreateFile(filesource);
            }
            catch { }
        } 

        public void SetCanvasSize()
        {
            string canvasheightSQL = "SELECT Value FROM Options WHERE Name='canvasheight'";
            string canvaswidthSQL = "SELECT Value FROM Options WHERE Name='canvaswidth'";

            CanvasSize.GetCanvasSize.Height = int.Parse(sqlitereader.ExecuteOneReader(canvasheightSQL, "value", sqliteCon));
            CanvasSize.GetCanvasSize.Width = int.Parse(sqlitereader.ExecuteOneReader(canvaswidthSQL, "value", sqliteCon));

        }

        private void InsertOption(string name, string value)
        {
            string cavasHeightSQL = "INSERT INTO Options (Name, Value) VALUES ('" + name + "','" + value + "')";
            sqlitewriter.ExecuteQuery(cavasHeightSQL, sqliteCon);
        }

        public void Connect()
        {
            if (sqliteCon != null) sqliteCon.Open();
        } 

        public bool IsOpen
        {
            get
            {
                if (sqliteCon == null) return false;
                return (System.Data.ConnectionState.Open == sqliteCon.State);
            }
        } 

        public void Open()
        {
            sqliteCon.Open();
        } 

        public void Close()
        {
            sqliteCon.Close();
        } 

        private void SetConnection(string filesource)
        {
            sqliteCon = new SQLiteConnection("Data Source=" + filesource + ";Version=3;");
        }  

        private void CreateStruct(int canvasHeight, int canvasWidth, string name, string project, string language)
        {
            TablesInit();
            OptionsInit(canvasHeight, canvasWidth, name, project, language);
        }

        private void TablesInit()
        {
            string methodsSQL = "CREATE TABLE Methods (Name VARCHAR(25) UNIQUE, Method TEXT)";
            string optionsSQL = "CREATE TABLE Options (Name VARCHAR(25) UNIQUE, Value TEXT)";
            string variablesSQL = "CREATE TABLE Variables (Name VARCHAR(25) UNIQUE, Value NUMBER)";
            sqlitewriter.ExecuteQuery(methodsSQL, sqliteCon);
            sqlitewriter.ExecuteQuery(optionsSQL, sqliteCon);
            sqlitewriter.ExecuteQuery(variablesSQL, sqliteCon);
        } 

        private void OptionsInit(int canvasHeight, int canvasWidth, string name, string project, string language)
        {
            InsertOption("canvasheight", canvasHeight.ToString());
            InsertOption("canvaswidth", canvasWidth.ToString());
            InsertOption("sourcecode", "");
            InsertOption("name", name);
            InsertOption("project", project);
            InsertOption("language", language);
        }

        ~SQLiteHelper()
        {
            try { Close(); }
            catch { }
        } 

    }
}
