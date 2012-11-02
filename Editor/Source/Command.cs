using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    class Command : ICloneable
    {
        public string word;
        public int? param_value;
        public bool param;
        public Command(string word, int? param_value, bool param)
        {
            this.word = word;
            this.param_value = param_value;
            this.param = param;
        }

        public Object Clone()
        {
            return new Command(this.word, this.param_value, this.param);
        }
    }
}
