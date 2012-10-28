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
            SetLanguage();
        }

        private void SetLanguage()
        {
            LanguageHelper lh = LanguageHelper.GetLanguageHelper();
            this.Title = lh.GetName("methodnameheader");
            methodNameText.Text = lh.GetName("methodname");
            addButton.Content = lh.GetName("add");
        }

        private void Method_Name_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LanguageHelper lh = LanguageHelper.GetLanguageHelper();
                string methodname = methodnameTextBox.Text.ToLower();
                NameChecker nc = new NameChecker();
                nc.IsUsable(methodname);
                SQLiteHelper.GetSqlHelper.NewMethod(methodname, lh.GetName("method") + " " + methodname + "\r\n\r\n" + lh.GetName("end"));
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }


    }
}
