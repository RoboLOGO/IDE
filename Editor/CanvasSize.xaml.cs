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
    /// Interaction logic for CanvasSize.xaml
    /// </summary>
    public partial class CanvasSize : Window
    {
        public CanvasSize()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.cHeight = int.Parse(canvasHeight.Text);
            MainWindow.cWidth = int.Parse(canvasWidth.Text);
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = ".txt";
            sfd.Filter = "Text documents (.txt)|*.txt";
            sfd.ShowDialog();
            try
            {
                File.Create(sfd.FileName);
            }
            catch { MessageBox.Show("Nem adtál meg filenevet!"); }
        }
    }
}
