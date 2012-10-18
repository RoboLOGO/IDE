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
        SQLiteHelper sqlitehelper;
        #endregion
        //Canvas méretei
        CanvasSize canvasSize;

        public MainWindow()
        {
            InitializeComponent();
            canvasSize = CanvasSize.GetCanvasSize();
            InitializeCanvas();
            sqlitehelper = SQLiteHelper.GetSqlHelper();
            menu = Menu.GetMenu();
            //sqlitehelper.NewFile("teszt", 800, 600);
            //sqlitehelper.SetSourceCode("almfa körtefa íííí neo");
        }

        private void InitializeCanvas()
        {
            canvas.Height = canvasSize.Height;
            canvas.Width = canvasSize.Width;
            this.Height = canvas.Height + 75;
            this.Width = canvas.Width + 50;
            canvas.Background = new SolidColorBrush(Colors.White); 
        }
       
        #region Menu
        Menu menu;
        RTextboxHelper rtbhelper = new RTextboxHelper();
        //bezárás
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //mentés
        private void SaveClick(object sender, ExecutedRoutedEventArgs e)
        {
            menu.Save(rtbhelper.GetString(commandLine));
        }
        //mentés másként
        private void SaveAsClick(object sender, ExecutedRoutedEventArgs e)
        {
            menu.SaveAs(rtbhelper.GetString(commandLine), sqlitehelper.GetFile());
        }
        //megnyitás
        private void OpenClick(object sender, ExecutedRoutedEventArgs e)
        {
            if (menu.Open() == true)
            {
                
                rtbhelper.SetString(sqlitehelper.GetSourceCode(), commandLine);
                Format();
                EnableMenus();
            }
        }
        //új
        private void NewClick(object sender, ExecutedRoutedEventArgs e)
        {
            NewProject np = new NewProject();
            np.ShowDialog();
            if (np.Success())
            {
                InitializeCanvas();
                EnableMenus();
            }
        }

        private void EnableMenus()
        {
            commandLine.IsEnabled = true;
            RunButton.IsEnabled = true;
            RunMenu.IsEnabled = true;
            ClearButton.IsEnabled = true;
            ClearMenu.IsEnabled = true;
            SaveButton.IsEnabled = true;
            SaveMenu.IsEnabled = true;
            SaveAsMenu.IsEnabled = true;
            MethodMenu.IsEnabled = true;
        }

        Turtle turtle;
        //futtatás
        private void RunClick(object sender, RoutedEventArgs e)
        {
            menu.Save(rtbhelper.GetString(commandLine));
            menu.Run(ref turtle, canvas);
        }
        //kép mentés
        private void SaveImageClick(object sender, RoutedEventArgs e)
        {
            menu.Image_Save(canvas, this);
        }
        //képernyő törlés
        private void ClearClick(object sender, RoutedEventArgs e)
        {
            menu.clear(turtle);
        }

       
        #region Eljárás szerksztő
        private void Method_Click(object sender, RoutedEventArgs e)
        {
            Method method = new Method();
            method.ShowDialog();
        }
        #endregion
        #endregion
        #region Syntax
        private void TextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            if (commandLine.Document == null)
                return;

            TextRange documentRange = new TextRange(commandLine.Document.ContentStart, commandLine.Document.ContentEnd);
            documentRange.ClearAllProperties();

            TextPointer navigator = commandLine.Document.ContentStart;
            while (navigator.CompareTo(commandLine.Document.ContentEnd) < 0)
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
            commandLine.TextChanged -= this.TextChangedEventHandler;

            for (int i = 0; i < m_tags.Count; i++)
            {
                TextRange range = new TextRange(m_tags[i].StartPosition, m_tags[i].EndPosition);
                range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Red));
                range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }
            m_tags.Clear();

            commandLine.TextChanged += this.TextChangedEventHandler;
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
