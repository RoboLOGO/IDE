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

namespace Editor
{
    /// <summary>
    /// Interaction logic for Method_Name.xaml
    /// </summary>
    public partial class Method_Name : Window
    {
        public Method_Name()
        {
            InitializeComponent();
        }

        private void Method_Name_Add_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.A.Method_List.Items.Add(textBox1.Text);
            this.Close();
        }
    }
}
