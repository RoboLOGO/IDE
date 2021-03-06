﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Editor.Exeptions;
using Editor;

namespace Robopreter
{
    public sealed class Parser
    {
        Dictionary<string, DeclareFunction> FuncDef = new Dictionary<string, DeclareFunction>();

        int Index;
        IList<Token> Tokens;
        Stmt result = new Nop();

        public int Line
        {
            get
            {
                if (Tokens.Count == Index)
                {
                    if (Index == 0) return 1;
                    return Tokens[Index - 1].Line;
                }
                return Tokens[Index].Line;
            }
        }

        public int Column
        {
            get
            {
                if (Tokens.Count == Index)
                {
                    if (Index == 0) return 1;
                    return Tokens[Index - 1].Column;
                }
                return Tokens[Index].Column;
            }
        }

        public string File
        {
            get
            {
                if (Tokens.Count == Index)
                {
                    if (Index == 0) return "main";
                    return Tokens[Index - 1].File;
                }
                return Tokens[Index].File;
            }
        }

        public Stmt Result
        {
            get
            {
                return result;
            }
        }

        public Parser(IList<Token> Tokens)
        {
            this.Tokens = Tokens;
            Index = 0;
            result = this.ParseStmt();

            if (Index != Tokens.Count)
            {     // Parser exited before EOF
                throw new RPExeption(20, App.Current.TryFindResource("x_nexpend").ToString(), Line, Column);
            }
        }

        Stmt ParseStmt(bool in_dec = false)
        {
            Sequence result = new Sequence();
            Stmt stmt = new Nop();
            bool breaked = false;
            if (Index == Tokens.Count)
            {     // Already got EOF?
                throw new RPExeption(20, App.Current.TryFindResource("x_nexpend").ToString(), Line, Column);
            }
            while (Index < Tokens.Count && !breaked)
            {
                switch (Config.In(Tokens[Index]).Name)
                {
                    case "Forward":
                        Index++;
                        var stmtForward = new Forward();
                        stmtForward.Expression = ParseExpr();
                        stmt = stmtForward;
                        break;
                    case "Backward":
                        Index++;
                        var stmtBackward = new Backward();
                        stmtBackward.Expression = ParseExpr();
                        stmt = stmtBackward;
                        break;
                    case "Right":
                        Index++;
                        var stmtRight = new Right();
                        stmtRight.Expression = ParseExpr();
                        stmt = stmtRight;
                        break;
                    case "Left":
                        Index++;
                        var stmtLeft = new Left();
                        stmtLeft.Expression = ParseExpr();
                        stmt = stmtLeft;
                        break;
                    case "PenUp":
                        Index++;
                        stmt = new PenUp();
                        break;
                    case "PenDown":
                        Index++;
                        stmt = new PenDown();
                        break;
                    case "Home":
                        Index++;
                        stmt = new Home();
                        break;
                    case "Clear":
                        Index++;
                        stmt = new Clear();
                        break;
                    case "DeclareFunction":
                        if (in_dec)
                        {        // Are we already declarating a function?
                            throw new RPExeption(21, App.Current.TryFindResource("x_cdfif").ToString(), Line, Column);
                        }
                        var stmtDeclareFunction = new DeclareFunction();
                        // Read function name
                        Index++;
                        if (Index < Tokens.Count && Tokens[Index].isStr)
                        {
                            stmtDeclareFunction.Identity = (string)Tokens[Index];
                        }
                        else
                        {
                            throw new RPExeption(22, App.Current.TryFindResource("x_expfn").ToString(), Line, Column);
                        }
                        // Read parameters if any
                        Index++;
                        while (Index + 1 < Tokens.Count && Tokens[Index].Equals(Scanner.ValueOf) && Tokens[Index + 1].isStr)
                        {
                            stmtDeclareFunction.Parameters.Add((string)Tokens[Index + 1]);
                            Index += 2;
                        }
                        if (Index == Tokens.Count)
                        {
                            throw new RPExeption(23, App.Current.TryFindResource("x_expfb").ToString(), Line, Column);
                        }
                        // Add function to the functions list
                        FuncDef.Add(stmtDeclareFunction.Identity, stmtDeclareFunction);
                        // Parse body
                        stmtDeclareFunction.Body = ParseStmt(true);
                        // End of function
                        if (Index == Tokens.Count || Tokens[Index].isStr && Config.Primitives.ContainsKey((string)Tokens[Index]) && Config.Primitives[(string)Tokens[Index]] == "End")
                        {
                            throw new RPExeption(24, App.Current.TryFindResource("x_expendttd").ToString(), Line, Column);
                        }
                        Index++;
                        stmt = stmtDeclareFunction;
                        break;
                    case "Output":
                        Index++;
                        var stmtOutput = new Output();
                        stmtOutput.Expression = ParseExpr();
                        stmt = stmtOutput;
                        break;
                    case "Stop":
                        Index++;
                        stmt = new Stop();
                        break;
                    case "IfElse":
                        var stmtIfElse = new IfElse();
                        // Parse the condition
                        Index++;
                        stmtIfElse.Condition = ParseExpr();
                        // Parse the true branch
                        if (Index == Tokens.Count || !Tokens[Index].Equals(Scanner.BodyStart))
                        {
                            throw new RPExeption(25, App.Current.TryFindResource("x_expzbb").ToString(), Line, Column);
                        }
                        Index++;
                        stmtIfElse.True = ParseStmt(in_dec);
                        if (Index == Tokens.Count || !Tokens[Index].Equals(Scanner.BodyEnd))
                        {
                            throw new RPExeption(26, App.Current.TryFindResource("x_expzab").ToString(), Line, Column);
                        }
                        // Parse the false branch if any
                        Index++;
                        if (Index < Tokens.Count && Tokens[Index].Equals(Scanner.BodyStart))
                        {
                            Index++;
                            stmtIfElse.False = ParseStmt(in_dec);
                            if (Index == Tokens.Count || !Tokens[Index].Contains.Equals(Scanner.BodyEnd))
                            {
                                throw new RPExeption(26, App.Current.TryFindResource("x_expzab").ToString(), Line, Column);
                            }
                            Index++;
                        }
                        else
                        {
                            stmtIfElse.False = new Nop();
                        }
                        stmt = stmtIfElse;
                        break;
                    case "Loop":
                        var stmtLoop = new Loop();
                        // Parse expression
                        Index++;
                        stmtLoop.Repeat = ParseExpr();
                        // Parse the body
                        if (Index == Tokens.Count || !Tokens[Index].Equals(Scanner.BodyStart))
                        {
                            throw new RPExeption(25, App.Current.TryFindResource("x_expzbb").ToString(), Line, Column);
                        }
                        Index++;
                        stmtLoop.Body = ParseStmt(in_dec);
                        if (Index == Tokens.Count || !Tokens[Index].Equals(Scanner.BodyEnd))
                        {
                            throw new RPExeption(26, App.Current.TryFindResource("x_expzab").ToString(), Line, Column);
                        }
                        Index++;
                        stmt = stmtLoop;
                        break;
                    case "While":
                        var stmtWhile = new While();
                        Index++;
                        if (Index == Tokens.Count || !Tokens[Index].Equals(Scanner.BodyStart))
                        {
                            throw new RPExeption(30, App.Current.TryFindResource("x_expzbtce").ToString(), Line, Column);
                        }
                        Index++;
                        stmtWhile.Condition = ParseExpr();
                        if (Index == Tokens.Count || !Tokens[Index].Equals(Scanner.BodyEnd))
                        {
                            throw new RPExeption(31, App.Current.TryFindResource("x_expzatce").ToString(), Line, Column);
                        }
                        Index++;
                        // Parse the body
                        if (Index == Tokens.Count || !Tokens[Index].Equals(Scanner.BodyStart))
                        {
                            throw new RPExeption(25, App.Current.TryFindResource("x_expzbb").ToString(), Line, Column);
                        }
                        Index++;
                        stmtWhile.Body = ParseStmt(in_dec);
                        if (Index == Tokens.Count || !Tokens[Index].Equals(Scanner.BodyEnd))
                        {
                            throw new RPExeption(26, App.Current.TryFindResource("x_expzab").ToString(), Line, Column);
                        }
                        Index++;
                        stmt = stmtWhile;
                        break;
                    case "Forever":
                        var stmtForever = new Forever();
                        Index++;
                        // Parse the body
                        if (Index == Tokens.Count || !Tokens[Index].Equals(Scanner.BodyStart))
                        {
                            throw new RPExeption(25, App.Current.TryFindResource("x_expzbb").ToString(), Line, Column);
                        }
                        Index++;
                        stmtForever.Body = ParseStmt(in_dec);
                        if (Index == Tokens.Count || !Tokens[Index].Equals(Scanner.BodyEnd))
                        {
                            throw new RPExeption(26, App.Current.TryFindResource("x_expzab").ToString(), Line, Column);
                        }
                        Index++;
                        stmt = stmtForever;
                        break;
                    case "Print":
                        Index++;
                        var stmtPrint = new Print();
                        stmtPrint.Text = ParseExpr();
                        stmt = stmtPrint;
                        break;
                    case "DeclareVariable":
                        var stmtDeclareVariable = new DeclareVariable();
                        Index += 2;
                        if (Index < Tokens.Count && Tokens[Index - 1].Equals(Scanner.AsIs))
                        {
                            stmtDeclareVariable.Identity = (string)Tokens[Index];
                        }
                        else
                        {
                            throw new RPExeption(27, App.Current.TryFindResource("x_expvarname").ToString(), Line, Column);
                        }
                        // Do the math
                        Index++;
                        stmtDeclareVariable.Expression = ParseExpr();
                        stmt = stmtDeclareVariable;
                        break;
                    default:
                        if (Tokens[Index].isStr)
                        {
                            if (Tokens[Index].Equals("language"))
                            {
                                Index++;
                                Config.Load();
                                Index++;
                                continue;
                            }
                            else if (Tokens[Index].Equals(Scanner.BodyEnd))
                            {
                                breaked = true;
                                continue;
                            }
                            else if (Tokens[Index].Equals(Config.Primitives["End"]))
                            {
                                if (!in_dec)
                                    throw new RPExeption(23, App.Current.TryFindResource("x_expfb").ToString(), Line, Column);
                                else breaked = true;
                                continue;
                            }
                            else if (!FuncDef.ContainsKey((string)Tokens[Index]))
                            {
                                throw new RPExeption(28, App.Current.TryFindResource("x_unknid").ToString() + (string)Tokens[Index] + "'", Line, Column);
                            }
                            else
                            {
                                var stmtCallFunction = new CallFunction();
                                stmtCallFunction.Identity = (string)Tokens[Index];
                                var P = FuncDef[(string)Tokens[Index]].Parameters.Count;
                                int i = 0;
                                Index++;
                                while (Index < Tokens.Count && i < P)
                                {
                                    stmtCallFunction.Parameters.Add(ParseExpr());
                                    i++;
                                }
                                if (Index == Tokens.Count && i < P)
                                {
                                    throw new RPExeption(29, App.Current.TryFindResource("x_fparnm").ToString(), Line, Column);
                                }
                                stmt = stmtCallFunction;
                            }
                        }
                        else
                        {
                            throw new RPExeption(99, App.Current.TryFindResource("x_unkncomm").ToString(), Line, Column);
                        }
                        break;
                }
                result.Statements.Add(stmt);
            }
            return result;
        }

        [Flags]
        enum ExprNext
        {
            Literal = 1,
            UExpr = 2,
            BExpr = 4,
            Bracket = 8
        }

        Expr ParseExpr()
        {
            var OpStack = new Stack<Operator>();
            var CStack = new Stack<Expr>();
            ExprNext NextCanBe = ExprNext.UExpr | ExprNext.BExpr | ExprNext.Literal | ExprNext.Bracket;
            while (Index < Tokens.Count)
            {
                // token is literal (double, int, bool, string, function call)
                if (NextCanBe.HasFlag(ExprNext.Literal))
                {
                    if (Tokens[Index].isSB)
                    {
                        var stringLiteral = new StringLiteral();
                        stringLiteral.Value = ((StringBuilder)Tokens[Index]).ToString();
                        Index++;
                        CStack.Push(stringLiteral);
                        NextCanBe = ExprNext.BExpr;
                        continue;
                    }
                    else if (Tokens[Index].isInt)
                    {
                        var intLiteral = new IntLiteral();
                        intLiteral.Value = (int)Tokens[Index];
                        Index++;
                        CStack.Push(intLiteral);
                        NextCanBe = ExprNext.BExpr;
                        continue;
                    }
                    else if (Tokens[Index].isDbl)
                    {
                        var dblLiteral = new DblLiteral();
                        dblLiteral.Value = (double)Tokens[Index];
                        NextCanBe = ExprNext.BExpr;
                        Index++;
                        CStack.Push(dblLiteral);
                        continue;
                    }
                    else if (Tokens[Index].isStr && FuncDef.ContainsKey((string)Tokens[Index]))
                    {    // If function is exists
                        var function = new ExprFunction();
                        function.Identity = (string)Tokens[Index];
                        var P = FuncDef[(string)Tokens[Index]].Parameters.Count;
                        int i = 0;
                        Index++;
                        while (Index < Tokens.Count && i < P)
                        {
                            function.Parameters.Add(ParseExpr());
                            i++;
                        }
                        if (Index == Tokens.Count && i < P)
                        {
                            throw new RPExeption(29, App.Current.TryFindResource("x_fparnm").ToString(), Line, Column);
                        }
                        CStack.Push(function);
                        NextCanBe = ExprNext.BExpr;
                        continue;
                    }
                }

                // if token is not expression either break
                if (!NextCanBe.HasFlag(ExprNext.UExpr) && !NextCanBe.HasFlag(ExprNext.BExpr) && !NextCanBe.HasFlag(ExprNext.Bracket) || !Tokens[Index].isStr) break;
                // else token is an operator
                switch ((string)Tokens[Index])
                {
                    case Scanner.BracketStart:
                        if (NextCanBe.HasFlag(ExprNext.Bracket))
                        {
                            Index++;
                            OpStack.Push(new Group());
                            NextCanBe = ExprNext.UExpr | ExprNext.BExpr | ExprNext.Literal | ExprNext.Bracket;
                            continue;
                        }
                        break;
                    case Scanner.BracketEnd:
                        {
                            Index++;
                            var o = (OpStack.Count > 0) ? OpStack.Peek() : null;
                            while (OpStack.Count > 0 && !(o is Group))
                            {
                                if (o.IsBinary)
                                {
                                    var expr = new BinaryExpression();
                                    expr.BinaryOperator = OpStack.Pop();
                                    if (expr.BinaryOperator.Level >= OpStack.Peek().Level)
                                    {
                                        expr.Right = CStack.Pop();
                                        expr.Left = CStack.Pop();
                                    }
                                    else
                                    {
                                        expr.Left = CStack.Pop();
                                        expr.Right = CStack.Pop();
                                    }
                                    CStack.Push(expr);
                                }
                                else
                                {
                                    var expr = new UnaryExpression();
                                    expr.Right = CStack.Pop();
                                    expr.UnaryOperator = OpStack.Pop();
                                    CStack.Push(expr);
                                }
                                if (OpStack.Count > 0) o = OpStack.Peek();
                            }
                            if (OpStack.Count > 0)
                            {
                                OpStack.Pop();
                                NextCanBe = ExprNext.BExpr;
                            }
                            else NextCanBe = 0;
                            continue;
                        }
                    case Scanner.ValueOf:
                        if (NextCanBe.HasFlag(ExprNext.UExpr))
                        {
                            Index++;
                            var var = new Variable();
                            var.Identity = (string)Tokens[Index];
                            CStack.Push(var);
                            OpStack.Push(new ValueOf());
                            NextCanBe = ExprNext.BExpr;
                            Index++;
                            continue;
                        }
                        break;
                    case Scanner.AsIs:
                        if (NextCanBe.HasFlag(ExprNext.UExpr))
                        {
                            Index++;
                            var strL = new StringLiteral();
                            strL.Value = (string)Tokens[Index];
                            CStack.Push(strL);
                            OpStack.Push(new AsIs());
                            NextCanBe = ExprNext.BExpr;
                            Index++;
                            continue;
                        }
                        break;
                    case Scanner.And:
                    case Scanner.Or:
                    case Scanner.Xor:
                    case Scanner.Not:
                    case Scanner.Add:
                    case Scanner.Subtract:
                    case Scanner.Multiply:
                    case Scanner.Divide:
                    case Scanner.Equal:
                    case Scanner.Bigger:
                    case Scanner.BiggerEqual:
                    case Scanner.Smaller:
                    case Scanner.SmallerEqual:
                    case Scanner.NotEqual:
                        {
                            var op = Operator.Parse((string)Tokens[Index]);
                            var o = (OpStack.Count > 0) ? OpStack.Peek() : op;
                            while (OpStack.Count > 0 && op.Level >= o.Level && !(o is Group))
                            {
                                if (o.IsBinary)
                                {
                                    var expr = new BinaryExpression();
                                    expr.Right = CStack.Pop();
                                    expr.BinaryOperator = OpStack.Pop();
                                    expr.Left = CStack.Pop();
                                    CStack.Push(expr);
                                }
                                else
                                {
                                    var expr = new UnaryExpression();
                                    expr.Right = CStack.Pop();
                                    expr.UnaryOperator = OpStack.Pop();
                                    CStack.Push(expr);
                                }
                                if (OpStack.Count > 0) o = OpStack.Peek();
                            }
                            if (CStack.Count == 0 && op is Sub || Diff(CStack, OpStack) && op is Sub) op = new Minus();        // If there was no literal before or previous token was also an operator
                            OpStack.Push(op);
                            NextCanBe = ExprNext.BExpr | ExprNext.UExpr | ExprNext.Literal | ExprNext.Bracket;
                            Index++;
                            continue;
                        }
                }
                break;
            }
            while (OpStack.Count > 0)
            {
                var o = OpStack.Peek();
                if (o.IsBinary)
                {
                    var expr = new BinaryExpression();
                    expr.Right = CStack.Pop();
                    expr.BinaryOperator = OpStack.Pop();
                    expr.Left = CStack.Pop();
                    CStack.Push(expr);
                }
                else
                {
                    var expr = new UnaryExpression();
                    expr.Right = CStack.Pop();
                    expr.UnaryOperator = OpStack.Pop();
                    CStack.Push(expr);
                }
            }
            return CStack.Pop();
        }

        private bool Diff(Stack<Expr> CStack, Stack<Operator> OpStack)
        {
            int c = 0;
            foreach (var item in OpStack)
            {
                if (item.IsBinary) c++;
            }
            return CStack.Count != c + 1;
        }

    }
}
