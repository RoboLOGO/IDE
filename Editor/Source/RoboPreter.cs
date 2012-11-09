using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Robopreter;

namespace Editor
{
    class RoboPreter
    {
        public RoboPreter() {
            Config.Load();
        }

        public List<Parancs> Run(string input)
        {

            Scanner scanner = new Scanner(new StringReader(input));
            Parser parser = new Parser(scanner.Tokens);
            Interpreter interpreter = new Interpreter(parser.Result, false);
            Interpreter.Run();
            return Interpreter.Out;
        }
    }
}
