using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robopreter
{
    public enum Parancsok
    {
        balra, jobbra, elore, hatra, tollatfel, tollatle, haza, torol
    }
    public class Parancs
    {
        public Parancsok parancs;
        public object ertek;
    }
}
