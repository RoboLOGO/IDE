using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Editor;

namespace Robopreter
{
    class Interpreter
    {
        static System.Random r = new System.Random();
        public static Dictionary<string, DeclareFunction> FuncDecl = new Dictionary<string, DeclareFunction>();    // deklarált utasítások
        public static Dictionary<string, object> VarDecl = new Dictionary<string, object>();     // deklarált változók
        public static Queue<Stmt> stmtQueue = new Queue<Stmt>();   // utasítás végrehajtási sor
        public static Stack<Dictionary<string, object>> stmtStack = new Stack<Dictionary<string, object>>();   // verem
        static bool response, stopped = false;

        public Interpreter(Stmt stmt, bool response = true)
        {
            stmtQueue.Enqueue(stmt);
            Interpreter.Out = new List<Parancs>();
            Interpreter.response = response;
        }

        public static List<Parancs> Out = new List<Parancs>();
        public static void Run()
        {
            while (stmtQueue.Count > 0)
            {
                var currStmt = stmtQueue.Dequeue();
                switch (currStmt.GetType().Name)
                {
                    case "Sequence":
                        var seq = (Sequence)currStmt;
                        foreach (var item in seq.Statements)
                        {
                            stmtQueue.Enqueue(item);
                        }
                        break;
                    case "Left":
                        {
                            var P = new Parancs();
                            P.parancs = Parancsok.balra;
                            ((Left)currStmt).Expression.Calc();
                            P.ertek = ((Left)currStmt).Expression.Return;
                            Out.Add(P);
                        }
                        break;
                    case "Right":
                        {
                            var P = new Parancs();
                            P.parancs = Parancsok.jobbra;
                            ((Right)currStmt).Expression.Calc();
                            P.ertek = ((Right)currStmt).Expression.Return;
                            Out.Add(P);
                        }                        
                        break;
                    case "Forward":
                        {
                            var P = new Parancs();
                            P.parancs = Parancsok.elore;
                            ((Forward)currStmt).Expression.Calc();
                            P.ertek = ((Forward)currStmt).Expression.Return;
                            Out.Add(P);
                        }
                        break;
                    case "Backward":
                        {
                            var P = new Parancs();
                            P.parancs = Parancsok.hatra;
                            ((Backward)currStmt).Expression.Calc();
                            P.ertek = ((Backward)currStmt).Expression.Return;
                            Out.Add(P);
                        }
                        wait();
                        break;
                    case "PenUp":
                        {
                            var P = new Parancs();
                            P.parancs = Parancsok.tollatfel;
                            Out.Add(P);
                        }
                        break;
                    case "PenDown":
                        {
                            var P = new Parancs();
                            P.parancs = Parancsok.tollatle;
                            Out.Add(P);
                        }
                        break;
                    case "Clear":
                        {
                            var P = new Parancs();
                            P.parancs = Parancsok.torol;
                            Out.Add(P);
                        }
                        break;
                    case "Home":
                        {
                            var P = new Parancs();
                            P.parancs = Parancsok.haza;
                            Out.Add(P);
                        }
                        break;
                    case "DeclareFunction":
                        var stmtDeclareFunction = (DeclareFunction)currStmt;
                        FuncDecl.Add(stmtDeclareFunction.Identity, stmtDeclareFunction);
                        break;
                    case "DeclareVariable":
                        var stmtDeclareVariable = (DeclareVariable)currStmt;
                        stmtDeclareVariable.Expression.Calc();
                        if (VarDecl.ContainsKey(stmtDeclareVariable.Identity))
                        {
                            VarDecl[stmtDeclareVariable.Identity] = stmtDeclareVariable.Expression.Return;
                        }
                        else
                        {
                            VarDecl.Add(stmtDeclareVariable.Identity, stmtDeclareVariable.Expression.Return);
                        }
                        break;
                    case "CallFunction":
                        var stmtCallFunction = (CallFunction)currStmt;
                        Call(stmtCallFunction.Identity, stmtCallFunction.Parameters);
                        break;
                    case "Print":
                        break;
                    case "IfElse":
                        var stmtIfElse = (IfElse)currStmt;
                        stmtIfElse.Condition.Calc();
                        try
                        {
                            if (Convert.ToBoolean(stmtIfElse.Condition.Return))
                            {
                                stmtQueue.Enqueue(stmtIfElse.True);
                            }
                            else
                            {
                                stmtQueue.Enqueue(stmtIfElse.False);
                            }
                        }
                        catch
                        {
                            App.StopError(101, "No logic expression found");
                        }
                        break;
                    case "Loop":
                        var stmtLoop = (Loop)currStmt;
                        var count = stmtLoop.Repeat.Calc();
                        if (count != null)
                        {
                            var savedQueue = stmtQueue.ToArray();
                            stmtQueue = new Queue<Stmt>();
                            for (int i = 0; i < count; i++)
                            {
                                stmtQueue.Enqueue(stmtLoop.Body);
                                Run();
                                if (stopped) break;
                                count = stmtLoop.Repeat.Calc() ?? 0;
                            }
                            stopped = false;
                            foreach (var item in savedQueue) stmtQueue.Enqueue(item);
                        }
                        break;
                    case "While":
                        {
                            var stmtWhile = (While)currStmt;
                            stmtWhile.Condition.Calc();
                            var condition = (bool)stmtWhile.Condition.Return;
                            var savedQueue = stmtQueue.ToArray();
                            stmtQueue = new Queue<Stmt>();
                            while (condition && !stopped)
                            {
                                stmtQueue.Enqueue(stmtWhile.Body);
                                Run();
                                stmtWhile.Condition.Calc();
                                condition = (bool)stmtWhile.Condition.Return;
                            }
                            stopped = false;
                            foreach (var item in savedQueue) stmtQueue.Enqueue(item);
                            break;
                        }
                    case "Forever":
                        {
                            var stmtForever = (Forever)currStmt;
                            var savedQueue = stmtQueue.ToArray();
                            stmtQueue = new Queue<Stmt>();
                            while (!stopped)
                            {
                                stmtQueue.Enqueue(stmtForever.Body);
                                Run();
                            }
                            stopped = false;
                            foreach (var item in savedQueue) stmtQueue.Enqueue(item);
                            break;
                        }
                    case "Output":
                        var stmtOutput = (Output)currStmt;
                        stmtOutput.Expression.Calc();
                        stmtStack.Peek().Add("$", stmtOutput.Expression.Return);
                        break;
                    case "Stop":
                        stmtQueue.Clear();
                        stopped = true;
                        break;
                    case "Nop":
                        break;
                }
            }
        }

        public static object Call(string Identity, List<Expr> Parameters)
        {
            if (FuncDecl.ContainsKey(Identity))
            {
                var param = new Dictionary<string, object>();
                for (int i = 0; i < FuncDecl[Identity].Parameters.Count; i++)
                {
                    if (i == Parameters.Count) break;
                    Parameters[i].Calc();
                    param.Add(FuncDecl[Identity].Parameters[i], Parameters[i].Return);
                }
                stmtStack.Push(param);
                var savedQueue = stmtQueue.ToArray();
                stmtQueue = new Queue<Stmt>();
                stmtQueue.Enqueue(FuncDecl[Identity].Body);
                Run();
                stopped = false;
                param = stmtStack.Pop();
                stmtQueue.Clear();
                foreach (var item in savedQueue) stmtQueue.Enqueue(item);
                if (param.ContainsKey("$")) return param["$"];
            }
            return null;
        }

        static void wait()
        {
            if (response) Console.ReadLine();
        }

        static string random()
        {
            var ac = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                ac.Append((char)r.Next(97, 123));
            }
            ac.Append('$');
            return ac.ToString();
        }


    }
}
