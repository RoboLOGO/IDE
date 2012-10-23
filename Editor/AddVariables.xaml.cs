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
    /// Interaction logic for AddVariables.xaml
    /// </summary>
    public partial class AddVariables : Window
    {
        public AddVariables()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string varname = txtnameBox.Text.Replace(" ", String.Empty);
                if (varname == string.Empty) throw new Exception("Error: name can not be empty");
                int value;
                try { value = int.Parse(txtvalueBox.Text); }
                catch { throw new Exception("Error: value must be integer"); }
                SQLiteHelper.GetSqlHelper.NewVariable(varname, value);
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
