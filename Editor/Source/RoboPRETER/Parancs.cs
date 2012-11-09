using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robopreter
{
    public enum Commands
    {
        balra, jobbra, elore, hatra, tollatfel, tollatle, haza, torol
    }
    public class Command
    {
        public Commands comm;
        public object value;
    }
}
