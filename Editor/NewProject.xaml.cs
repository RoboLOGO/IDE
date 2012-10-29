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
        bool _success = false;
        SaveFileDialog sfd = null;

        public NewProject()
        {
            InitializeComponent();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                int height = int.Parse(txtcanvasHeight.Text);
                int width = int.Parse(txtcanvasWidth.Text);
                if (nameText.Text == String.Empty)
                    throw new Exception(App.Current.TryFindResource("nameempty").ToString());
                if(projectText.Text == String.Empty)
                    throw new Exception(App.Current.TryFindResource("projectempty").ToString());
                SQLiteHelper sqlitehelp = SQLiteHelper.GetSqlHelper;
                sqlitehelp.NewFile(sfd.FileName, height, width, nameText.Text, projectText.Text, languageCombo.Text);
                CanvasSize canvasSize = CanvasSize.GetCanvasSize;

                canvasSize.Width = width;
                canvasSize.Height = height;

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(App.Current.TryFindResource("error").ToString() + ": \n" + ex.Message);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            sfd = FileDialogs.GetSaveFileDialog();
            sfd.ShowDialog();

            txtfileSource.Text = sfd.FileName;
            txtfileSource.ToolTip = sfd.FileName;
            this.IsSuccess = true;
        }

        public bool IsSuccess
        {
            get { return _success; }
            internal set { _success = value; }
        }
    }
}
