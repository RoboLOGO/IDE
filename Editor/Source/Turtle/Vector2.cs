using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    public class Vector2
    {
        double _x;
        public double X
        {
            get { return _x; }
            set { _x = value; }
        }
        double _y;
        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public Vector2() { }
        public Vector2(Vector2 vector2)
        {
            this.X = vector2.X;
            this.Y = vector2.Y;
        }
        public Vector2(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public override String ToString()
        {
            return "<" + X.ToString() + "," + Y.ToString() + ">";
        }

        public override bool Equals(object two)
        {
            return ((Vector2)two).X == this.X && ((Vector2)two).Y == this.Y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
