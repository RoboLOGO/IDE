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

        public Method()
        {
            InitializeComponent();
            SetMethodNames();
        }

        private void Method_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Method_Add_Click(object sender, RoutedEventArgs e)
        {
            MethodName methodname = new MethodName();
            methodname.ShowDialog();
            SetMethodNames();
        }

        private void SetMethodNames()
        {
            List<string> items = SQLiteHelper.GetSqlHelper().GetAllMethodName();
            methodList.ItemsSource = items;
        }

        private void select_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (methodList.SelectedItem != null)
            {
                RTextboxHelper rtbhelper = new RTextboxHelper();
                if (prevItem != null && TextChanged())
                {                    
                    if (MessageBox.Show("Szeretnéd menteni a változások?", "Mentés", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        SQLiteHelper.GetSqlHelper().UpdateMethod(prevItem.ToString(), rtbhelper.GetString(methodCommandLine));
                    }
                }
                methodCommandLine.Document.Blocks.Clear();//nem az igazi:S
                rtbhelper.SetString(SQLiteHelper.GetSqlHelper().GetMethod(methodList.SelectedItem.ToString()), methodCommandLine);
                prevItem = methodList.SelectedItem;
            }
        }

        private void AddVariable_Click(object sender, RoutedEventArgs e)
        {
            AddVariables AddVar = new AddVariables();
            AddVar.ShowDialog();
        }

        private void DeleteValriable_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteMethod_Click(object sender, RoutedEventArgs e)
        {
            if (methodList.SelectedItem != null)
            {
                if (MessageBox.Show("Biztosan törlöd a(z) " + methodList.SelectedItem.ToString() + " eljárást?", "Eljárás törlés", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
                {
                    SQLiteHelper.GetSqlHelper().DeleteMethod(methodList.SelectedItem.ToString());
                    methodCommandLine.Document.Blocks.Clear();
                    SetMethodNames();
                }
            }
            prevItem = null;
        }

        private bool TextChanged()
        {
            RTextboxHelper rtbhelper = new RTextboxHelper();
            string s1 = rtbhelper.GetString(methodCommandLine).Replace("\r\n", "");
            string s2 = SQLiteHelper.GetSqlHelper().GetMethod(prevItem.ToString()).Replace("\n", "");
            s2 = s2.Replace("\r", "");
            return !(s1.Equals(s2));    
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
