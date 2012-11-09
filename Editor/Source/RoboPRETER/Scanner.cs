using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using Editor;

namespace Robopreter {
    public sealed class Scanner {
        private List<Token> Result;
        private int Line, Column;
        private string file;

        public List<Token> Tokens {
            get {
                return Result;
            }
        }

        public static char[] TerminateSym = new char[] { ' ', '[', ']', '+', '-', '*', '/', '~', '"', '(', ')', '<', '>', '='};

        public const string Add = "Add";
        public const string Subtract = "Sub";
        public const string Multiply = "Mul";
        public const string Divide = "Div";
        public const string AsIs = "AsIs";
        public const string ValueOf = "ValueOf";
        public const string Equal = "Eq";
        public const string NotEqual = "NotEq";
        public const string Bigger = "Bigger";
        public const string BiggerEqual = "BiggerEq";
        public const string Smaller = "Smaller";
        public const string SmallerEqual = "SmallerEq";
        public const string BracketStart = "(";
        public const string BracketEnd = ")";
        public const string BodyStart = "[";
        public const string BodyEnd = "]";
        public const string And = "And";
        public const string Or = "Or";
        public const string Xor = "Xor";
        public const string Not = "Not";

        public Scanner(TextReader Input, string file = "main") {
            Result = new List<Token>();
            this.file = file;
            Line = 1;
            Column = 1;
            Scan(Input);
            Input.Close();
        }

        private void Scan(TextReader Input) {
            StringBuilder ac;
            char c;
            int currentCol = 0;
            while(Peek(Input, out c)) {
                currentCol = Column;
                if(char.IsWhiteSpace(c) || c == '~') {
                    if(c == '\n') {
                        Line++;
                        Column = 1;
                    } else Column++;
                    Input.Read();
                } else if(c == ';') {
                    Input.Read();
                    if(!Peek(Input, out c) || c != '_') {
                        while(Peek(Input, out c) && c != '\r' && c != '\n') Input.Read();
                    } else Input.Read();           // defined functions by the interpreter
                } else if(char.IsLetter(c) || c == '_' || c == ';') {
                    ac = new StringBuilder();
                    while(Peek(Input, out c) && (char.IsLetter(c) || char.IsDigit(c) || c == '_' || c == '?')) {
                        ac.Append(c);
                        Input.Read();
                        Column++;
                    }
                    if((ac.ToString() == "include" || ac.ToString() == "language") && Peek(Input, out c)) {
                        Input.Read();
                        string pre_name = ac.ToString();
                        string pre_val = "";
                        ac = new StringBuilder();
                        while(Peek(Input, out c) && c != '\n' && c != '\r') {
                            ac.Append(c);
                            Input.Read();
                        }
                        pre_val = ac.ToString().Trim();
                        if(pre_name == "include") {
                            try {
                                Scanner sc = new Scanner(new StreamReader(pre_val), pre_val);  // opens an include file
                                Result = sc.Tokens.Concat(Result).ToList();
                            } catch {
                                throw new Exception(2 + " File not found " +  Line + " " + Column + 3);
                            }
                        } else if(pre_name == "language") {
                            Tokens.Add(new Token("language", Line, currentCol, file));
                            Tokens.Add(new Token(pre_val, Line, Column + 2, file));
                        }
                    } else {
                        Result.Add(new Token(ac.ToString(), Line, currentCol, file));
                    }
                } else if(c == '"') {
                    Result.Add(new Token(AsIs, Line, currentCol, file));
                    Input.Read();
                    Column++;
                    ac = new StringBuilder();
                    bool escape = false;
                    while(Peek(Input, out c) && (Terminate(c) & escape || !Terminate(c) && !escape) && c != '\n') {
                        if(c == '\\' && !escape) escape = true;
                        else escape = false;
                        if(!escape) ac.Append(c);
                        Input.Read();
                        Column++;
                    }
                    Result.Add(new Token(ac, Line, currentCol, file));
                } else if(char.IsDigit(c) || c == Config.DecimalPoint) {
                    bool wasdot = false;
                    ac = new StringBuilder();
                    while(Peek(Input, out c) && (char.IsDigit(c) || !wasdot && (c == Config.DecimalPoint))) {
                        if(c == Config.DecimalPoint) {
                            c = '.';
                            wasdot = true;
                        }
                        ac.Append(c);
                        Input.Read();
                        Column++;
                    }
                    if(wasdot && c != Config.DecimalPoint) {
                        try {
                            double d = double.Parse(ac.ToString(), CultureInfo.InvariantCulture);
                            Result.Add(new Token(d, Line, currentCol, file));
                        } catch {
                            Result.Add(new Token(ac.ToString(), Line, currentCol, file));
                        }
                    } else {
                        try {
                            int i = int.Parse(ac.ToString(), CultureInfo.InvariantCulture);
                            Result.Add(new Token(i, Line, currentCol, file));
                        } catch {
                            Result.Add(new Token(ac.ToString(), Line, currentCol, file));
                        }
                    }
                } else {
                    switch(c) {
                        case '+': Result.Add(new Token(Add, Line, currentCol, file)); Input.Read(); break;
                        case '-': Result.Add(new Token(Subtract, Line, currentCol, file)); Input.Read(); break;
                        case '*': Result.Add(new Token(Multiply, Line, currentCol, file)); Input.Read(); break;
                        case '/': Result.Add(new Token(Divide, Line, currentCol, file)); Input.Read(); break;
                        case '=': Result.Add(new Token(Equal, Line, currentCol, file)); Input.Read(); break;
                        case ':': Result.Add(new Token(ValueOf, Line, currentCol, file)); Input.Read(); break;
                        case '>': 
                            Input.Read();
                            if(Peek(Input, out c) && c == '=') {
                                Result.Add(new Token(BiggerEqual, Line, currentCol, file));
                                Input.Read();
                            } else {
                                Result.Add(new Token(Bigger, Line, currentCol, file));
                            }
                            break;
                        case '<': 
                            Input.Read();
                            if(Peek(Input, out c) && c == '=') {
                                Result.Add(new Token(SmallerEqual, Line, currentCol, file));
                                Input.Read();
                            } else if(Peek(Input, out c) && c == '>') {
                                Result.Add(new Token(NotEqual, Line, currentCol, file));
                                Input.Read();
                            } else {
                                Result.Add(new Token(Smaller, Line, currentCol, file));
                            }
                            break;
                        case '&': Result.Add(new Token(And, Line, currentCol, file)); Input.Read(); break;
                        case '|': Result.Add(new Token(Or, Line, currentCol, file)); Input.Read(); break;
                        case '^': Result.Add(new Token(Xor, Line, currentCol, file)); Input.Read(); break;
                        case '!': Result.Add(new Token(Not, Line, currentCol, file)); Input.Read(); break;
                        case '(': Result.Add(new Token(BracketStart, Line, currentCol, file)); Input.Read(); break;
                        case ')': Result.Add(new Token(BracketEnd, Line, currentCol, file)); Input.Read(); break;
                        case '[': Result.Add(new Token(BodyStart, Line, currentCol, file)); Input.Read(); break;
                        case ']': Result.Add(new Token(BodyEnd, Line, currentCol, file)); Input.Read(); break;
                        default: Result.Add(new Token(c, Line, currentCol, file)); Input.Read(); break;
                    }
                    Column++;
                }
            }     
        }

        private bool Terminate(char c) {
            int i = 0;
            while(i < TerminateSym.Length && c != TerminateSym[i]) i++;
            return i != TerminateSym.Length; 
        }

        private static bool Peek(TextReader TR, out char c) {
            int i = TR.Peek();
            c = (i != -1) ? (char)i : '\0';
            return (i != -1);
        }
    }

}
