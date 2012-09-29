using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Editor
{
    internal class Logo_Run
    {
        public Logo_Run()
        {
        }
        public List<string> line = new List<string>();
        public List<Tuple<Command>> comm = new List<Tuple<Command>>();
        public struct Command
        {
            public string word;
            public int? param_value;
            public bool param;
            public Command(string _word, int? _param_value, bool _param)
            {
                word = _word;
                param_value = _param_value;
                param = _param;
            }

        }
        public void Read_line(string filename)
        {
            string s = "";
            StreamReader str = new StreamReader(filename, Encoding.UTF8);
            while (!str.EndOfStream)
            {
                s = str.ReadLine();
                line.Add(s);
            }

        }

        public void Spearate(int sor)
        {
            Command cmd = new Command("", null, true);
            string strparam = ""; ;
            string[] strs = {
                "haza",
                "xpoz",
                "ypoz",
                "törölkép",
                "tollatle",
                "tollatfel",
            };
            for (int i = 0; i < line[sor].Length; i++)
            {

                if (line[sor][i] != ' ' && cmd.param)
                {
                    cmd.word += line[sor][i];
                }
                else
                {
                    if (i + 1 >= line[sor].Length) break;
                    else
                    {
                        i++;
                        while (line[sor][i] != ' ')
                        {
                            strparam += line[sor][i];
                            i++;
                            if (i + 1 > line[sor].Length) break;
                        }
                        cmd.param_value = int.Parse(strparam);
                        comm.Add(new Tuple<Command>(cmd));
                        cmd.param_value = null;
                        cmd.param = true;
                        cmd.word = "";
                        strparam = "";
                    }
                }
                for (int j = 0; j < strs.Length; j++)
                {
                    if (cmd.word == strs[j])
                    {
                        cmd.param = false;
                        cmd.param_value = null;
                        comm.Add(new Tuple<Command>(cmd));
                        cmd.param_value = null;
                        cmd.param = true;
                        cmd.word = "";
                    }
                }
            }

        }
        public string[] commands = {
                "előre",
                "hátra",
                "jobbra",
                "balra",
                "haza",
                "xpoz",
                "ypoz",
                "törölkép",
                "tollatle",
                "tollatfel",
            };
        public void Draww(Turtle turtle, List<Tuple<Command>> c)
        {
            for (int i = 0; i < c.Count; i++)
            {
                switch (c[i].Item1.word)
                {
                    case "előre": turtle.Forward((int)c[i].Item1.param_value); break;
                    case "hátra": turtle.Backward((int)c[i].Item1.param_value); break;
                    case "jobbra": turtle.Right((int)c[i].Item1.param_value); break;
                    case "balra": turtle.Left((int)c[i].Item1.param_value); break;
                    case "haza": turtle.Home(); break;
                    case "xpoz": turtle.XPos(); break;
                    case "ypoz": turtle.YPos(); break;
                    case "törölkép": turtle.Clean(); break;
                    case "tollatle": turtle.PenDown(); break;
                    case "tollatfel": turtle.PenUp(); break;
                }
            }
        }

    }
}
