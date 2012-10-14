using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;



namespace Editor
{
    class TurtleImage
    {
        //A teknős mérete
        int _size = 40;
        public int size
        {
            get
            {
                return _size;
            }
        }
        //A teknős képe
        Image turtle;
        //a teknős állapota
        bool _visible;
        public bool visible
        {
            get { return _visible; }
            set { _visible = value; }
        }
        //A canvas children tömbjének hanyadik eleme a teknős kép
        int _cPos;
        public int cPos
        {
            get { return _cPos; }
            set { _cPos = value; }
        }


        public TurtleImage()
        {
            turtle = new Image();
            turtle.Source = new BitmapImage(new Uri(@"Content\turtle.png", UriKind.Relative));
            turtle.Width = size;
            turtle.Height = size;
            visible = true;
        }

        //Visszadja a teknős képének referenciáját
        public Image getTurtleImage()
        {
            return turtle;
        }
        
    }
}
