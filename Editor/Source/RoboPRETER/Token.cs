using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robopreter {
    public class Token {
        object token;
        int line;
        int column;
        string file;

        public object Contains {
            get { return token; }
        }

        public int Line {
            get { return line; }
        }

        public int Column {
            get { return column; }
        }

        public string File {
            get { return file; }
        }

        public bool isSB {
            get { return this.token is StringBuilder; }
        }

        public bool isInt {
            get { return token is int; }
        }

        public bool isDbl {
            get { return token is double; }
        }

        public bool isNum {
            get { return token is int || token is double; }
        }

        public bool isStr {
            get { return this.token is string; }
        }

        public Token(object Token, int Line = 0, int Column = 0, string File = "main") {
            this.token = Token;
            this.line = Line;
            this.column = Column;
            this.file = File;
        }

        public override bool Equals(object obj) {
            return token.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
        
        public static implicit operator string(Token t) {
            if(t.Contains is StringBuilder) {
                return ((StringBuilder)t.Contains).ToString();
            }
            return (string)t.Contains;
        }

        public static implicit operator int(Token t) {
            return (int)t.Contains;
        }

        public static implicit operator double(Token t) {
            return (double)t.Contains;
        }

        public static implicit operator StringBuilder(Token t) {
            return (StringBuilder)t.Contains;
        }
    }
}
