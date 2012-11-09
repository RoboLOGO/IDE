using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robopreter {

    public abstract class Stmt { }

    public abstract class Expr {
        public object Return;
        public abstract double? Calc();
    }


    public class Forward : Stmt {   // Move our turtle forward
        public Expr Expression;     // How much?
    }

    public class Home : Stmt    {   
        public Expr Expression;    
    }

    public class Clear : Stmt
    {
        public Expr Expression;
    }

    public class Backward : Stmt {  // Put the turtle in reverse
        public Expr Expression;     // How much?
    }

    public class Left : Stmt {      // Turn left
        public Expr Expression;     // Yeah, but how much?
    }

    public class Right : Stmt {     // Turn right
        public Expr Expression;     // How much so?
    }

    public class PenUp : Stmt { }   // Release the pen

    public class PenDown : Stmt { } // Hold the pen down

    public class Loop : Stmt {      // Its the 'repeat' statement
        public Expr Repeat;         // Repeat how many times?
        public Stmt Body;           // What to repeat
    }

    public class While : Stmt {     // Its the 'while' statement
        public Expr Condition;      // Repeat while its true
        public Stmt Body;           // What to repeat
    }

    public class Forever : Stmt {
        public Stmt Body;           // infinite loop
    }

    public class IfElse : Stmt {    // Its an 'ifelse' statement
        public Expr Condition;      // Test condition
        public Stmt True;           // Do if true
        public Stmt False;          // Do if false
    }

    public class DeclareVariable : Stmt {   // Let's declare a variable with 'make' statement
        public string Identity;             // Variable name to assign
        public Expr Expression;             // Variable value to interpret
    }

    public class Print : Stmt {
        public Expr Text;
    }

    public class DeclareFunction : Stmt {                       // We want a to declare a function

        public string Identity;                                 // What will be the name?
        public List<string> Parameters = new List<string>();    // Parameters to declare
        public Stmt Body;                                       // Body of the function
    }

    public class CallFunction : Stmt {                    // Calling a function
        public string Identity;                           // Its name
        public List<Expr> Parameters = new List<Expr>();  // Its parameters to give
    }

    public class InitRandom : Stmt {
        public static readonly List<string> ID = new List<string>();
    }
    public class Random : Stmt {    // Randomize number
        public int Number;          // Between 0 and (Number - 1)
    }

    public class Sequence : Stmt {      // Batch same level commands
        public List<Stmt> Statements = new List<Stmt>();
    }

    public class Output : Stmt {        // Return from a function
        public Expr Expression;         // Expression to be calculated and returned
    }

    public class Stop : Stmt { }        // STOP THE PROGRAM!!!!444

    public class Nop : Stmt { }         // The no operation procedure

    public class StringLiteral : Expr {
        public string Value = "";
        public override double? Calc() {
            Return = Value;
            return null; 
        }
    }

    public class IntLiteral : Expr {
        public int Value = 0;
        public override double? Calc() {
            Return = Value;
            return (double)Value; 
        }
    }

    public class BoolLiteral : Expr {
        public bool Value = false;
        public override double? Calc() {
            Return = Value;
            return Value ? 1 : 0;
        }
    }

    public class DblLiteral : Expr {
        public double Value = 0;
        public override double? Calc() { 
            Return = Value;
            return Value; 
        }
    }

    public class BinaryExpression : Expr {
        public Expr Left;
        public Expr Right;
        public Operator BinaryOperator;
        public override double? Calc() {
            double? ret = null;

            Left.Calc();    // calc left
            Right.Calc();   // calc right

            if(Left.Return is string || Right.Return is string) {
                Return = "";
                if(Left.Return is string) Return += (string)Left.Return;
                if(Right.Return is string) Return += (string)Right.Return;
            }
            if(Return is string && (Left.Return is bool || Right.Return is bool)) {
                if(Left.Return is bool) Return += (bool)Left.Return ? Config.Primitives["True"] : Config.Primitives["False"];
                if(Right.Return is bool) Return += (bool)Right.Return ? Config.Primitives["True"] : Config.Primitives["False"];
            }
            if(Return is string && (Left.Return is int || Right.Return is int || Left.Return is double || Right.Return is double)) {
                if(Left.Return is int) Return += Left.Return.ToString();
                if(Right.Return is int) Return += Right.Return.ToString();
            }

            if(Return is string || Right.Return == null || Left.Return == null) return ret;   // If string found exit, don't calc!

            bool isLeftBool = false, isRightBool = false;

            double dLeft = Left.Return is int || Left.Return is double ? Convert.ToDouble(Left.Return) : 0;
            double dRight = Right.Return is int || Right.Return is double ? Convert.ToDouble(Right.Return) : 0;

            if(Left.Return is bool) isLeftBool = true;
            if(Right.Return is bool) isRightBool = true;

            bool bLeft = Left.Return is bool ? Convert.ToBoolean(Left.Return) : false;
            bool bRight = Right.Return is bool ? Convert.ToBoolean(Right.Return) : false;

            switch(BinaryOperator.GetType().Name) {
            case "Add":
                if(!isLeftBool && !isRightBool) Return = dLeft + dRight;
                break;
            case "Sub":
                if(!isLeftBool && !isRightBool) Return = dLeft - dRight;
                break;
            case "Mul":
                if(!isLeftBool && !isRightBool) Return = dLeft * dRight;
                break;
            case "Div":
                if(!isLeftBool && !isRightBool) Return = dLeft / dRight;
                break;
            case "Eq":
                if(isLeftBool && isRightBool) Return = bLeft == bRight;
                else if(!isLeftBool && !isRightBool) Return = dLeft == dRight;
                break;
            case "NotEq":
                if(isLeftBool && isRightBool) Return = bLeft != bRight;
                else if(!isLeftBool && !isRightBool) Return = dLeft != dRight;
                break;
            case "Smaller":
                if(!isLeftBool && !isRightBool) Return = dLeft < dRight;
                break;
            case "SmallerEq":
                if(!isLeftBool && !isRightBool) Return = dLeft <= dRight;
                break;
            case "Bigger":
                if(!isLeftBool && !isRightBool) Return = dLeft > dRight;
                break;
            case "BiggerEq":
                if(!isLeftBool && !isRightBool) Return = dLeft >= dRight;
                break;
            case "And":
                if(isLeftBool && isRightBool) Return = bLeft && bRight;
                break;
            case "Or":
                if(isLeftBool && isRightBool) Return = bLeft || bRight;
                break;
            case "Xor":
                if(isLeftBool && isRightBool) Return = bLeft != bRight;
                break;
            }
            try {
                ret = Convert.ToDouble(Return);
                if(ret % 1 == 0 && !(Return is bool)) Return = Convert.ToInt32(Return);
            } catch { }
            if(Return is bool) ret = (bool)Return ? 1d : 0d;
            return ret;
        }
    }

    public class UnaryExpression : Expr {
        public Expr Right;
        public Operator UnaryOperator;
        public override double? Calc() {
            double? ret = null;

            Right.Calc();   // calc right

            if(Right.Return is string) {
                Return = (string)Right.Return;
                return ret;   // If string found exit, don't calc!
            }

            Return = Right.Return;

            if(Return == null) return ret;

            switch(UnaryOperator.GetType().Name) {
            case "Minus":
                if(Return is double) Return = -((double)Return);
                else if(Return is int) Return = -((int)Return);
                break;
            case "Not":
                if(Return is bool) Return = !((bool)Return);
                break;
            }
            try {
                ret = Convert.ToDouble(Return);
                if(ret % 1 == 0 && !(Return is bool)) Return = Convert.ToInt32(Return);
            } catch { }
            if(Return is bool) ret = (bool)Return ? 1d : 0d;
            return ret;
        }
    }

    public class Variable : Expr {
        public string Identity;
        public override double? Calc() {
            var param = Interpreter.stmtStack.Count > 0 ?  Interpreter.stmtStack.Peek() : null;
            if(param != null && param.ContainsKey(Identity)) {
                Return = param[Identity];
            } else if(Interpreter.VarDecl.ContainsKey(Identity)) {
                Return = Interpreter.VarDecl[Identity];
            }
            if(Return is bool) return (bool)Return ? 1d : 0d;
            if(Return is int || Return is double) return Convert.ToDouble(Return);
            return null;
        }
    }

    public class ExprFunction : Expr {                    // Calling a function
        public string Identity;                           // Its name
        public List<Expr> Parameters = new List<Expr>();  // Its parameters to give
        public override double? Calc() {
            double? ret = null;
            if(Interpreter.FuncDecl.ContainsKey(Identity)) {
                Return = Interpreter.Call(Identity, Parameters);
                if(Return is bool) ret = (bool)Return ? 1d : 0d;
                if(Return is int || Return is double) ret = Convert.ToDouble(Return);
            }
            return ret;
        }
    }
}
