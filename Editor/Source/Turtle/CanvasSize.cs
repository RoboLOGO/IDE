using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    class CanvasSize
    {
        static CanvasSize self = null;

        int _height;
        public int Height
        {
            get { return _height; }
            set { if (value >= 0) _height = value; }
        }

        int _width;
        public int Width
        {
            get { return _width; }
            set { if (value >= 0)  _width = value; }
        }

        private CanvasSize()
        {
            Height = 600;
            Width = 800;
        }

        public static CanvasSize GetCanvasSize()
        {
            if (self == null) self = new CanvasSize();
            return self;
        }


    }
}
