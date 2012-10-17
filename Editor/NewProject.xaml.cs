using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;


namespace Editor
{
    /// <summary>
    /// Interaction logic for NewProject.xaml
    /// </summary>
    public partial class NewProject : Window
    {
        bool success = false;
        SaveFileDialog sfd = null;
        public NewProject()
        {
            InitializeComponent();
        }
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                int height = int.Parse(canvasHeight.Text);
                int width = int.Parse(canvasWidth.Text);
                SQLiteHelper sqlitehelp = SQLiteHelper.GetSqlHelper();
                sqlitehelp.NewFile(sfd.FileName, height, width);
                CanvasSize canvasSize = CanvasSize.GetCanvasSize();
                sqlitehelp.IsOpen();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba:\n" + ex.Message);
            }
        }

        private void SaveButton_Click_1(object sender, RoutedEventArgs e)
        {
            
            sfd = new SaveFileDialog();
            sfd.DefaultExt = ".rbsln";
            sfd.Filter = "RoboLOGO Solution(.rbsln)|*.rbsln";
            sfd.FileName = "Projekt";
            sfd.ShowDialog();
            fileSource.Text = sfd.FileName;
            fileSource.ToolTip = sfd.FileName;
            success = true;
        }

        public bool Success()
        {
            return success;
        }
    }
}
