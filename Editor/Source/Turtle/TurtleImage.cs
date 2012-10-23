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
        public int Size
        {
            get { return _size; }
        }

        //a teknős állapota
        bool _visible;
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        //A canvas children tömbjének hanyadik eleme a teknős kép
        int _cPos;
        public int CPos
        {
            get { return _cPos; }
            set { _cPos = value; }
        }

        public TurtleImage()
        {
            turtle = new Image();
            turtle.Source = new BitmapImage(new Uri(@"Content\turtle.png", UriKind.Relative));
            turtle.Width = this.Size;
            turtle.Height = this.Size;
            this.Visible = true;
        }

        //A teknős képe
        Image turtle;
        public Image GetTurtleImage
        {
            get { return turtle; }
        }
        
    }
}
