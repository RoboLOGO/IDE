using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Editor
{
    class LogoRun
    {
        public LogoRun() { }

        List<string> line = new List<string>();
        List<Command> com;

       

        public List<Command> Run(string source)
        {
            com = new List<Command>();
            ReadLine(source);
            for (int i = 0; i < line.Count; i++) Separate(i);
            return com;
        }

        private void ReadLine(string sourceCode)
        {
            string[] lines = Regex.Split(sourceCode, "\r\n");
            for(int i = 0; i < lines.Length; i++) line.Add(lines[i]);

        }

        private void Separate(int sor)
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

                if (line[sor][i] != ' ' && cmd.param) cmd.word += line[sor][i];
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
                        com.Add((Command)cmd.Clone());
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
                        com.Add((Command)cmd.Clone());
                        cmd.param_value = null;
                        cmd.param = true;
                        cmd.word = "";
                    }
                }
            }

        }




        private string[] commands = {
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
                "várj"
            };
    }
}
