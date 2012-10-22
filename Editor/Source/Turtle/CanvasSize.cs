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
            set
            {
                if (value >= 0)
                {
                    _height = value;
                }
            }

            get
            {
                return _height;
            }
        }

        int _width;
        public int Width
        {
            set
            {
                if (value >= 0)
                {
                    _width = value;
                }
            }

            get
            {
                return _width;
            }
        }

        private CanvasSize()
        {
            Height = 600;
            Width = 800;
        }

        public static CanvasSize GetCanvasSize()
        {
            if (self == null)
            {
                self = new CanvasSize();
            }
            return self;
        }


    }
}
