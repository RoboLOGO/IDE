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
                if (txtMethod.Text == string.Empty) throw new Exception("Error: empty method name");
                SQLiteHelper.GetSqlHelper().NewMethod(txtMethod.Text, "eljárás " + txtMethod.Text + "\r\n\r\nvége");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { this.Close(); }
        }
    }
}
