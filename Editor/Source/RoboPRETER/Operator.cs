using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robopreter {
    // [Operator]
    public abstract class Operator {
        protected int level;
        protected bool isBinary;

        public int Level {
            get { return level; }
        }

        public bool IsBinary {
            get { return isBinary; }
        }

        public Operator() {
            level = 10;
            isBinary = true;
        }

        public static Operator Parse(string p) {
            if(Type.GetType("Robopreter." + p).BaseType != typeof(Operator)) return null;
            return (Operator)Activator.CreateInstance(Type.GetType("Robopreter." + p));
        }
    }
    // (Expression)
    public class Group : Operator {
        public Group() { level = 1; isBinary = false; }
    }
    // -Expression2
    public class Minus : Operator {
        public Minus() { level = 2; isBinary = false; }
    }
    // !Expression
    public class Not : Operator {
        public Not() { level = 2; isBinary = false; }
    }
    // ValueOf(Variable)
    public class ValueOf : Operator {
        public ValueOf() { level = 2; isBinary = false; }
    }
    // Expression1 * Expression2
    public class Mul : Operator {
        public Mul() { level = 3; }
    }
    // Expression1 / Expression2
    public class Div : Operator {
        public Div() { level = 3; }
    }
    // Expression1 + Expression2
    public class Add : Operator {
        public Add() { level = 4; }
    }
    // Expression1 - Expression2
    public class Sub : Operator {
        public Sub() { level = 4; }
    }
    // Expression1 > Expression2
    public class Bigger : Operator {
        public Bigger() { level = 5; }
    }
    // Expression1 >= Expression2
    public class BiggerEq : Operator {
        public BiggerEq() { level = 5; }
    }
    // Expression1 < Expression2
    public class Smaller : Operator {
        public Smaller() { level = 5; }
    }
    // Expression1 <= Expression2
    public class SmallerEq : Operator {
        public SmallerEq() { level = 5; }
    }
    // Expression1 == Expression2
    public class Eq : Operator {
        public Eq() { level = 6; }
    }
    // Expression1 != Expression2
    public class NotEq : Operator {
        public NotEq() { level = 6; }
    }
    // Expression1 && Expression2
    public class And : Operator {
        public And() { level = 7; }
    }
    // Expression1 ^^ Expression2
    public class Xor : Operator {
        public Xor() { level = 8; }
    }
    // Expression1 || Expression2
    public class Or : Operator {
        public Or() { level = 9; }
    }
    // "Expression"
    public class AsIs : Operator {
        public AsIs() { level = 10; isBinary = false; }
    }
}
