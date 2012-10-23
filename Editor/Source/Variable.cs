using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    class Variable
    {
        public Variable() { }
        public Variable(string name, int value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; set; }
        public int Value { get; set; }
    }
}
