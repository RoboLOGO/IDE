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
    /// Interaction logic for NewVariables.xaml
    /// </summary>
    public partial class AddVariables : Window
    {
        public AddVariables()
        {
            InitializeComponent();
            SetLanguage();
        }

        private void SetLanguage()
        {
            LanguageHelper lh = LanguageHelper.GetLanguageHelper();
            this.Title = lh.GetName("newvarheader");
            nameText.Text = lh.GetName("name") + ":";
            valueText.Text = lh.GetName("value") + ":";
            addButton.Content = lh.GetName("add");
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string varname = txtnameBox.Text.ToLower();
                NameChecker nc = new NameChecker();
                nc.IsUsable(varname);
                int value;
                try { value = int.Parse(txtvalueBox.Text); }
                catch { throw new Exception(LanguageHelper.GetLanguageHelper().GetExeption("valueint")); }
                SQLiteHelper.GetSqlHelper.NewVariable(varname, value);
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
