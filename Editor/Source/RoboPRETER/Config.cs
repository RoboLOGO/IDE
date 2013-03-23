using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Windows;
using Editor;

namespace Robopreter {
    public sealed class Config
    {
        public static List<ID> IDs = new List<ID>();
        public static Dictionary<string, string> Primitives = new Dictionary<string, string>();
        public static char DecimalPoint = '.';

        public static Type In(Token t)
        {
            if (!t.isStr) return typeof(void);
            int i = 0;
            while (i < IDs.Count && IDs[i].Name != (string)t) i++;
            if (i == IDs.Count) return typeof(Nop);
            return IDs[i].For;
        }

        public static void Load()
        {
            IDs.Clear();
            Primitives.Clear();

            //Parancsok
            IDs.Add(new ID(App.Current.TryFindResource("kw_kor").ToString(), Type.GetType("Robopreter.Circle")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_korr").ToString(), Type.GetType("Robopreter.Circle")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_elore").ToString(), Type.GetType("Robopreter.Forward")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_elorer").ToString(), Type.GetType("Robopreter.Forward")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_hatra").ToString(), Type.GetType("Robopreter.Backward")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_hatrar").ToString(), Type.GetType("Robopreter.Backward")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_jobbra").ToString(), Type.GetType("Robopreter.Right")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_jobbrar").ToString(), Type.GetType("Robopreter.Right")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_balra").ToString(), Type.GetType("Robopreter.Left")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_balrar").ToString(), Type.GetType("Robopreter.Left")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_haza").ToString(), Type.GetType("Robopreter.Home")));
            //IDs.Add(new ID(App.Current.TryFindResource("kw_xpoz").ToString(), Type.GetType("Robopreter.Xpos")));
            //IDs.Add(new ID(App.Current.TryFindResource("kw_xpozr").ToString(), Type.GetType("Robopreter.YPos")));
            //IDs.Add(new ID(App.Current.TryFindResource("kw_ypoz").ToString(), Type.GetType("Robopreter.YPos")));
            //IDs.Add(new ID(App.Current.TryFindResource("kw_ypozr").ToString(), Type.GetType("Robopreter.YPos")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_torolkep").ToString(), Type.GetType("Robopreter.Clear")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_torolkepr").ToString(), Type.GetType("Robopreter.Clear")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_tollatfel").ToString(), Type.GetType("Robopreter.PenUp")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_tollatfelr").ToString(), Type.GetType("Robopreter.PenUp")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_tollatle").ToString(), Type.GetType("Robopreter.PenDown")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_tollatler").ToString(), Type.GetType("Robopreter.PenDown")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_ha").ToString(), Type.GetType("Robopreter.IfElse")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_ismetles").ToString(), Type.GetType("Robopreter.Loop")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_ismetlesr").ToString(), Type.GetType("Robopreter.Loop")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_amig").ToString(), Type.GetType("Robopreter.While")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_eljaras").ToString(), Type.GetType("Robopreter.DeclareFunction")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_eljarasr").ToString(), Type.GetType("Robopreter.DeclareFunction")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_tanuld").ToString(), Type.GetType("Robopreter.DeclareFunction")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_keszit").ToString(), Type.GetType("Robopreter.DeclareVariable")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_orokke").ToString(), Type.GetType("Robopreter.Forever")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_kiir").ToString(), Type.GetType("Robopreter.Print")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_intveletlen").ToString(), Type.GetType("Robopreter.InitRandom")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_veletlen").ToString(), Type.GetType("Robopreter.Random")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_stop").ToString(), Type.GetType("Robopreter.Stop")));
            IDs.Add(new ID(App.Current.TryFindResource("kw_eredmeny").ToString(), Type.GetType("Robopreter.Output")));

            //primitívek
            Primitives.Add("True", App.Current.TryFindResource("kw_igaz").ToString());
            Primitives.Add("False", App.Current.TryFindResource("kw_hamis").ToString());
            Primitives.Add("End", App.Current.TryFindResource("kw_vege").ToString());

            //tizedespont
            DecimalPoint = App.Current.TryFindResource("kw_decimal").ToString()[0];

        }
    }

    public class Error {
        public int Id;
        public string Description;
        public bool InFile = true;

        public Error() { }

        public Error(int Id, string Description, bool InFile = true) {
            this.Id = Id;
            this.Description = Description;
            this.InFile = InFile;
        }
    }

    public class ID {
        public string Name;
        public Type For;

        public ID() { }

        public ID(string Name, Type For) {
            this.Name = Name;
            this.For = For;
        }
    }
}
