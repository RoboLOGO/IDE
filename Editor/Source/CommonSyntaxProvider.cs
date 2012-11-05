using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Editor
{
    class CommonSyntaxProvider
    {
        public CommonSyntaxProvider(string SyntaxFilePath, bool CaseSensitive, string SplitSign)
        {
            caseSens = CaseSensitive;
            splitSign = SplitSign;
            ReadSyntax(SyntaxFilePath);
        }
        public CommonSyntaxProvider(string SyntaxFilePath) : this(SyntaxFilePath, true, "<--->") { }
        public CommonSyntaxProvider(List<string> KeyWordList, List<char> SpecialCaracters, bool CaseSensitive)
        {
            KeyWords = KeyWordList;
            specials = SpecialCaracters;
            caseSens = CaseSensitive;
        }
        public CommonSyntaxProvider(KeyWordFunction GetKeyWords, SpecCharsFunction GetSpecChars, bool CaseSensitive)
        {
            KeyWords = GetKeyWords();
            specials = GetSpecChars();
            caseSens = CaseSensitive;
        }

        List<string> KeyWords;
        List<char> specials;
        bool caseSens;
        string splitSign;

        void ReadSyntax(string SyntaxFilePath)
        {
            try
            {
                StreamReader str = new StreamReader(SyntaxFilePath, Encoding.Default);
                KeyWords = new List<string>();

                string line;
                while (!str.EndOfStream)
                {
                    line = str.ReadLine();
                    if (line == splitSign) break;
                    KeyWords.Add(line);
                }
                line = str.ReadToEnd();
                specials = new List<char>(line);
            }
            catch { }
        }

        public List<char> GetSpecials
        {
            get { return specials; }
        }
        public List<string> GetTags
        {
            get { return KeyWords; }
        }

        string Tag;
        public bool IsKnownTag(string tag)
        {
            Tag = tag;
            switch (caseSens)
            {
                case false: return KeyWords.Exists(CaseNonSensitive);
                default: return KeyWords.Exists(CaseSensitive);
            }
        }

        delegate bool CaseSensitivity(string s); // Átláthatóság miatt: a függvény szignatúrája
        public delegate List<string> KeyWordFunction();
        public delegate List<char> SpecCharsFunction();

        bool CaseSensitive(string s)
        {
            return s.Equals(Tag);
        }
        bool CaseNonSensitive(string s)
        {
            return s.ToLower().Equals(Tag.ToLower());
        }

    }
}
