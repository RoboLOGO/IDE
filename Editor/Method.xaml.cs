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
        LanguageHelper lh;

        public Method()
        {
            rtbhelper = new RTextboxHelper();
            lh = LanguageHelper.GetLanguageHelper();
            InitializeComponent();
            SetLanguage();
            SetMethodNames();
            SetVariables();
            MethodCommandLineEnable();
        }

        private void SetLanguage()
        {
            this.Title = lh.GetName("methodheader");
            addButton.Content = lh.GetName("add");
            removeButton.Content = lh.GetName("remove");
            okButton.Content = lh.GetName("ok");
            methodsHeader.Header = lh.GetName("methods");
            varHeader.Header = lh.GetName("variables");
            dataGrid.Columns[0].Header = lh.GetName("name");
            dataGrid.Columns[1].Header = lh.GetName("value");
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
            if (MessageBox.Show(lh.GetName("saveconfirmation"), lh.GetName("save"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
                if (MessageBox.Show(lh.GetName("savetext") + " " + (dataGrid.SelectedItem as Variable).Name + " " + lh.GetName("deletevariable") + "?", lh.GetName("delvar"), MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
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
                if (MessageBox.Show(lh.GetName("savetext") + " " + methodList.SelectedItem.ToString() + lh.GetName("deletemethod") + "?", lh.GetName("delmet"), MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
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
            if (((dataGrid.SelectedItem as Variable).Value != int.Parse((e.EditingElement as TextBox).Text)) && MessageBox.Show(lh.GetName("delconfirmation") + "?", lh.GetName("save"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    int value = int.Parse((e.EditingElement as TextBox).Text);
                    SQLiteHelper.GetSqlHelper.UpdateVariable((dataGrid.SelectedItem as Variable).Name, value);
                }
                catch
                {
                    MessageBox.Show(lh.GetExeption("valueint"));
                }
                
            }
            SetVariables();
        }

        #region Syntax
        private void TextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            if (methodCommandLine.Document == null)
                return;

            TextRange documentRange = new TextRange(methodCommandLine.Document.ContentStart, methodCommandLine.Document.ContentEnd);
            documentRange.ClearAllProperties();

            TextPointer navigator = methodCommandLine.Document.ContentStart;
            while (navigator.CompareTo(methodCommandLine.Document.ContentEnd) < 0)
            {
                TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);
                if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
                {
                    CheckWordsInRun((Run)navigator.Parent);

                }
                navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
            }

            Format();
        }
        new struct Tag
        {
            public TextPointer StartPosition;
            public TextPointer EndPosition;
            public string Word;

        }
        List<Tag> m_tags = new List<Tag>();
        void Format()
        {
            methodCommandLine.TextChanged -= this.TextChangedEventHandler;

            for (int i = 0; i < m_tags.Count; i++)
            {
                TextRange range = new TextRange(m_tags[i].StartPosition, m_tags[i].EndPosition);
                range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Red));
                range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }
            m_tags.Clear();

            methodCommandLine.TextChanged += this.TextChangedEventHandler;
        }

        void CheckWordsInRun(Run run)
        {
            string text = run.Text;

            int sIndex = 0;
            int eIndex = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsWhiteSpace(text[i]) | SyntaxHighlight.GetSpecials.Contains(text[i]))
                {
                    if (i > 0 && !(Char.IsWhiteSpace(text[i - 1]) | SyntaxHighlight.GetSpecials.Contains(text[i - 1])))
                    {
                        eIndex = i - 1;
                        string word = text.Substring(sIndex, eIndex - sIndex + 1);

                        if (SyntaxHighlight.IsKnownTag(word))
                        {
                            Tag t = new Tag();
                            t.StartPosition = run.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                            t.EndPosition = run.ContentStart.GetPositionAtOffset(eIndex + 1, LogicalDirection.Backward);
                            t.Word = word;

                            m_tags.Add(t);
                        }
                    }
                    sIndex = i + 1;
                }
            }

            string lastWord = text.Substring(sIndex, text.Length - sIndex);
            if (SyntaxHighlight.IsKnownTag(lastWord))
            {
                Tag t = new Tag();
                t.StartPosition = run.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                t.EndPosition = run.ContentStart.GetPositionAtOffset(eIndex + 1, LogicalDirection.Backward);
                t.Word = lastWord;
                m_tags.Add(t);
            }
        }
        #endregion
    }
}
