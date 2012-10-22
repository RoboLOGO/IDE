using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Editor
{
    class Pen
    {
        //a toll állapota
        bool drawing;

        //a toll színe
        Color _color;
        public Color Color
        {
            get { return _color; }
            set 
            {
                if (value != null) _color = value;
                else throw new Exception("Error: invalid pen color");
            }
        }

        //a toll mérete
        int _pensize;
        public int Pensize
        {
            get { return _pensize; }
            set 
            {
                if (value > 0) _pensize = value;
                else throw new Exception("Error: invalid pen size");
            }
        }

        public Pen()
        {
            Reset();
        }
        public Pen(Color color) : this()
        {
            this.Color = color;
        }
        public Pen(Color color, int pensize) : this(color)
        {
            this.Pensize = pensize;
        }

        //Kezdő állapotba állítja a tollat
        public void Reset()
        {
            drawing = true;
            Color = Colors.Black;
            Pensize = 1;
        }

        //Tollat felemeli, nem rajzol
        public void PenUp()
        {
            drawing = false;
        }

        //Tollat leteszi, rajzol
        public void PenDown()
        {
            drawing = true;
        }

        //Toll állapotát adja vissza
        public bool IsDown
        {
            get { return drawing; }
        }
        
    }
}
