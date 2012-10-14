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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Data.SQLite;

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region SQLite
        SQLiteConnection sqLiteConnection;
        #endregion
        #region Turtle
        //Canvas méretei
        public static int cHeight = 600;
        public static int cWidth = 800;

        public MainWindow()
        {
            InitializeComponent();
            InitializeCanvas();
        }

        private void InitializeCanvas()
        {
            canvas.Height = cHeight;
            canvas.Width = cWidth;
            this.Height = canvas.Height + 75;
            this.Width = canvas.Width + 50;
            canvas.Background = new SolidColorBrush(Colors.White);
        }
       
        #endregion 
        #region Menu
        Menu menu = new Menu();
        //bezárás
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //mentés
        private void Save_Click(object sender, ExecutedRoutedEventArgs e)
        {
            menu.Save(GetString(Command_line));
        }
        //rtb -> string
        string GetString(RichTextBox rtb)
        {
            var textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            return textRange.Text;
        }
        //string ->rtb
        public void SetRTFText(string text, RichTextBox rtb)
        {
            MemoryStream stream = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(text));
            rtb.Selection.Load(stream, DataFormats.Text);
        }
        //mentés másként
        private void SaveAs_Click(object sender, ExecutedRoutedEventArgs e)
        {
            menu.SaveAs(GetString(Command_line));
        }
        //megnyitás
        private void Open_Click(object sender, ExecutedRoutedEventArgs e)
        {
            menu.Open();
            Format();
        }
        //új
        private void New_Click(object sender, ExecutedRoutedEventArgs e)
        {
            CanvasSize cs = new CanvasSize();
            cs.ShowDialog();
            InitializeCanvas();

        }
        Turtle turtle;
        //futtatás
        private void Run_Click(object sender, RoutedEventArgs e)
        {
            menu.Save(GetString(Command_line));
            menu.run(ref turtle, canvas);
        }
        //kép mentés
        private void Save_Image_Click(object sender, RoutedEventArgs e)
        {
            menu.Image_Save(canvas, this);
        }
        //képernyő törlés
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            menu.clear(turtle);
        }

       
        #region Eljárás szerksztő
        public static Method A;
        private void Method_Click(object sender, RoutedEventArgs e)
        {
            A = new Method();
            A.Show();
        }
        #endregion
        #endregion
        #region Syntax
        private void TextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            if (Command_line.Document == null)
                return;

            TextRange documentRange = new TextRange(Command_line.Document.ContentStart, Command_line.Document.ContentEnd);
            documentRange.ClearAllProperties();

            TextPointer navigator = Command_line.Document.ContentStart;
            while (navigator.CompareTo(Command_line.Document.ContentEnd) < 0)
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
            Command_line.TextChanged -= this.TextChangedEventHandler;

            for (int i = 0; i < m_tags.Count; i++)
            {
                TextRange range = new TextRange(m_tags[i].StartPosition, m_tags[i].EndPosition);
                range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Red));
                range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }
            m_tags.Clear();

            Command_line.TextChanged += this.TextChangedEventHandler;
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
