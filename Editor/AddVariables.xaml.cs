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
                string varname = nameBox.Text.Replace(" ", String.Empty);
                if (varname == string.Empty) throw new Exception("Nem lehet üres a név");
                int value;
                try { value = int.Parse(valueBox.Text); }
                catch { throw new Exception("A változó értéke csak egész szám lehet"); }
                SQLiteHelper.GetSqlHelper().NewVariable(varname, value);
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
