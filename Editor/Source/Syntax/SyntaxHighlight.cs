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

namespace Editor
{
    class SyntaxHighlight
    {
        public SyntaxHighlight(RichTextBox TextInput, CommonSyntaxProvider Syntax, Color KeyWordColor)
        {
            textInput = TextInput;
            textInput.TextChanged += TextInput_TextChanged;
            syntax = Syntax;
            keyWordColor = KeyWordColor;
        }
        public SyntaxHighlight(RichTextBox TextInput, CommonSyntaxProvider Syntax) : this(TextInput, Syntax, Colors.Blue) { }

        CommonSyntaxProvider syntax;
        RichTextBox textInput;
        List<Tag> CurrentKeyWords = new List<Tag>();
        Color keyWordColor;

        private void TextInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textInput.Document == null) return;

            TextRange documentRange = new TextRange(textInput.Document.ContentStart, textInput.Document.ContentEnd);
            documentRange.ClearAllProperties();

            TextPointer navigator = textInput.Document.ContentStart;
            while (navigator.CompareTo(textInput.Document.ContentEnd) < 0)
            {
                TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);
                if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
                {
                    try
                    {
                        CheckWordsInRun((Run)navigator.Parent);
                    }
                    catch { }
                }
                navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
            }
            Format();
        }

        struct Tag
        {
            public TextPointer StartPosition;
            public TextPointer EndPosition;
            public string Word;
        }

        void CheckWordsInRun(Run run)
        {
            string text = run.Text;

            int sIndex = 0;
            int eIndex = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsWhiteSpace(text[i]) | syntax.GetSpecials.Contains(text[i]))
                {
                    if (i > 0 && !(Char.IsWhiteSpace(text[i - 1]) | syntax.GetSpecials.Contains(text[i - 1])))
                    {
                        eIndex = i - 1;
                        string word = text.Substring(sIndex, eIndex - sIndex + 1);

                        if (syntax.IsKnownTag(word))
                        {
                            Tag t;
                            t.StartPosition = run.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                            t.EndPosition = run.ContentStart.GetPositionAtOffset(eIndex + 1, LogicalDirection.Backward);
                            t.Word = word;
                            CurrentKeyWords.Add(t);
                        }
                    }
                    sIndex = i + 1;
                }
            }
            eIndex = text.Length - 1;
            string lastWord = text.Substring(sIndex, text.Length - sIndex);
            if (syntax.IsKnownTag(lastWord))
            {
                Tag t = new Tag();
                t.StartPosition = run.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                t.EndPosition = run.ContentStart.GetPositionAtOffset(eIndex + 1, LogicalDirection.Backward);
                t.Word = lastWord;
                CurrentKeyWords.Add(t);
            }
        }

        void Format()
        {
            textInput.TextChanged -= TextInput_TextChanged;

            TextRange r;
            for (int i = 0; i < CurrentKeyWords.Count; i++)
            {
                r = new TextRange(CurrentKeyWords[i].StartPosition, CurrentKeyWords[i].EndPosition);
                r.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(keyWordColor));
                r.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }
            CurrentKeyWords.Clear();
            textInput.TextChanged += TextInput_TextChanged;
        }
    }
}
