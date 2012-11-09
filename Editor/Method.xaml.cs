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
    /// Interaction logic for Method.xaml
    /// </summary>
    public partial class Method : Window
    {
        object prevItem;
        RTextboxHelper rtbhelper;
        CommonSyntaxProvider logoSynProvider;
        SyntaxHighlight synHighligt;

        public Method()
        {
            rtbhelper = new RTextboxHelper();
            InitializeComponent();
            SetMethodNames();
            SetVariables();
            MethodCommandLineEnable();
            logoSynProvider = new CommonSyntaxProvider(LogoKeywords.GetLogoKeywords().GetKeywords, LogoKeywords.GetLogoKeywords().GetSpecialCharacters, false);
            synHighligt = new SyntaxHighlight(methodCommandLine, logoSynProvider);
        }

        private void Method_Close_Click(object sender, RoutedEventArgs e)
        {
            CloseSave();
            this.Close();
        }

        private void CloseSave()
        {
            if (methodList.SelectedItem != null && TextChanged()) MethodSaver(methodList.SelectedItem);
        }

        private void Method_Add_Click(object sender, RoutedEventArgs e)
        {
            MethodName methodname = new MethodName();
            methodname.ShowDialog();
            SetMethodNames();
            MethodCommandLineEnable();
        }

        private void MethodCommandLineEnable()
        {
            if (methodList.Items.Count != 0)
            {
                methodCommandLine.IsEnabled = true;
            }
        }

        private void SetMethodNames()
        {
            List<string> items = SQLiteHelper.GetSqlHelper.GetAllMethodName();
            methodList.ItemsSource = items;
        }

        private void Select_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (methodList.SelectedItem != null)
            {
                if (prevItem != null && TextChanged())
                {
                    MethodSaver(prevItem);
                }
                CommandLineClear();
                rtbhelper.SetString(SQLiteHelper.GetSqlHelper.GetMethod(methodList.SelectedItem.ToString()), methodCommandLine);
                prevItem = methodList.SelectedItem;
            }
        }

        private void MethodSaver(object item)
        {
            if (MessageBox.Show(App.Current.TryFindResource("saveconfirmation").ToString(), App.Current.TryFindResource("save").ToString(), 
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                SQLiteHelper.GetSqlHelper.UpdateMethod(item.ToString(), rtbhelper.GetString(methodCommandLine));
            }
        }

        private void AddVariable_Click(object sender, RoutedEventArgs e)
        {
            AddVariables AddVar = new AddVariables();
            AddVar.ShowDialog();
            SetVariables();
        }

        private void DeleteValriable_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                if (MessageBox.Show(App.Current.TryFindResource("savetext").ToString() + " " + (dataGrid.SelectedItem as Variable).Name + " " + App.Current.TryFindResource("deletevariable").ToString(),
                    App.Current.TryFindResource("delvar").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
                {
                    SQLiteHelper.GetSqlHelper.DeleteVariable((dataGrid.SelectedItem as Variable).Name);
                    SetVariables();
                }
            }
        }

        private void DeleteMethod_Click(object sender, RoutedEventArgs e)
        {
            if (methodList.SelectedItem != null)
            {
                if (MessageBox.Show(App.Current.TryFindResource("savetext").ToString() + " " + methodList.SelectedItem.ToString() + App.Current.TryFindResource("deletemethod").ToString() + "?", 
                    App.Current.TryFindResource("delmet").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
                {
                    SQLiteHelper.GetSqlHelper.DeleteMethod(methodList.SelectedItem.ToString());
                    CommandLineClear();
                    SetMethodNames();
                    CommandLineClear();
                }
            }
            prevItem = null;
        }

        private bool TextChanged()
        {
            string s1 = rtbhelper.GetString(methodCommandLine).Replace("\r\n", "");
            string s2 = (SQLiteHelper.GetSqlHelper.GetMethod(prevItem.ToString()).Replace("\n", "")).Replace("\r", "");
            return !(s1.Equals(s2));
        }

        private void CommandLineClear()
        {
            rtbhelper.DeleteString(methodCommandLine,fdtext);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseSave();
        }

        private void SetVariables()
        {
            List<string> names = SQLiteHelper.GetSqlHelper.GetAllVariablesName();
            List<Variable> l = new List<Variable>();
            foreach (string x in names) l.Add(new Variable(x, SQLiteHelper.GetSqlHelper.GetVariable(x)));
            dataGrid.ItemsSource = l;
        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (((dataGrid.SelectedItem as Variable).Value != int.Parse((e.EditingElement as TextBox).Text)) && MessageBox.Show(App.Current.TryFindResource("delconfirmation").ToString() + "",
                App.Current.TryFindResource("save").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    int value = int.Parse((e.EditingElement as TextBox).Text);
                    SQLiteHelper.GetSqlHelper.UpdateVariable((dataGrid.SelectedItem as Variable).Name, value);
                }
                catch
                {
                    MessageBox.Show(App.Current.TryFindResource("valueint").ToString());
                }

            }
            SetVariables();
        }

    }
}
