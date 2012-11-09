using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static void StopError(int id, string description, int atLine = 1, int atColumn = 1, string file = "main")
        {
            MessageBox.Show("error;id=" + id + ";description=\"" + description + "\";line=" + atLine + ";column=" + atColumn + ";file=" + file + ";");
        }
    }

}
