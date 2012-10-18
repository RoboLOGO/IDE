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
    public partial class MethodName : Window
    {
        public MethodName()
        {
            InitializeComponent();
        }

        private void Method_Name_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (textBox1.Text == string.Empty) throw new Exception("Nem lehet üres a név");
                SQLiteHelper.GetSqlHelper().NewMethod(textBox1.Text, "");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.Close();
        }
    }
}
