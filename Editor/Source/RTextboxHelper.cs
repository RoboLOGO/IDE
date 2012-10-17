using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.IO;

namespace Editor
{
    class RTextboxHelper
    {
        //rtb ->string
        public string GetString(RichTextBox rtb)
        {
            var textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            return textRange.Text;
        }
        //string ->rtb
        public void SetString(string text, RichTextBox rtb)
        {
            MemoryStream stream = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(text));
            rtb.Selection.Load(stream, DataFormats.Text);
        }
    }
}
