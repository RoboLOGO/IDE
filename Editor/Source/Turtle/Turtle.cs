using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows;

namespace Editor
{
    class Turtle
    {
        Canvas canvas;
        Pen pen;
        //aktuális poz
        Vector2 pos;
        //teknős képe
        TurtleImage turtleimage;

        int _angle;
        //teknős aktuális foka
        private int angle
        {
            get { return _angle; }
            set { _angle = value % 360; }
        }


        public Turtle(Canvas canvas)
        {
            InitTurtle(canvas, Colors.Black, 1);
        }

        public Turtle(Canvas canvas, Color penColor)
        {
            InitTurtle(canvas, penColor, 1);
        }

        public Turtle(Canvas canvas, int penSize)
        {
            InitTurtle(canvas, Colors.Black, penSize);
        }

        public Turtle(Canvas canvas, Color penColor, int penSize)
        {
            InitTurtle(canvas, penColor, penSize);
        }

        private void InitTurtle(Canvas canvas, Color penColor, int penSize)
        {
            this.canvas = canvas;
            pen = new Pen(penColor, penSize);
            pos = new Vector2();
            turtleimage = new TurtleImage();
            turtleimage.CPos = canvas.Children.Add(turtleimage.GetTurtleImage);
            Canvas.SetZIndex(canvas.Children[turtleimage.CPos], 100); //a teknős mindig legfelül
            Home();
        }

        //Előre megy a teknős
        public void Forward(int distance)
        {
            DrawLine(-distance);
        }

        //Hátra megy a teknős
        public void Backward(int distance)
        {
            DrawLine(distance);
        }

        //Vonalat rajzol
        private void DrawLine(int distance)
        {
            Vector2 newpos = new Vector2();

            newpos.X = pos.X + (distance * Math.Cos(DegreeToRadian(angle)));
            newpos.Y = pos.Y + (distance * Math.Sin(DegreeToRadian(angle)));

            if (pen.IsDown)
            {
                Line line = new Line();
                line.Stroke = new SolidColorBrush(pen.Color);
                //line.SnapsToDevicePixels = true;
                //line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);

                line.X1 = pos.X;
                line.X2 = newpos.X;
                line.Y1 = pos.Y;
                line.Y2 = newpos.Y;

                line.StrokeThickness = pen.Pensize;
                canvas.Children.Add(line);
                
            }

            pos = newpos;

            TurtleMove(pos);
        }

        //Teknőst mozgatja a Canvason
        private void TurtleMove(Vector2 turtlePos)
        {
            if (turtleimage.Visible)
            {
                //Canvas.SetTop(canvas.Children[turtleimage.CPos], turtlePos.Y - turtleimage.Size / 2);
                //Canvas.SetLeft(canvas.Children[turtleimage.CPos], turtlePos.X - turtleimage.Size / 2);

                Storyboard sb = new Storyboard();
                DoubleAnimation da = new DoubleAnimation((double)canvas.Children[turtleimage.CPos].GetValue(Canvas.TopProperty), turtlePos.Y - turtleimage.Size / 2, new System.Windows.Duration(TimeSpan.FromSeconds(1)));
                DoubleAnimation db = new DoubleAnimation((double)canvas.Children[turtleimage.CPos].GetValue(Canvas.LeftProperty), turtlePos.X - turtleimage.Size / 2, new System.Windows.Duration(TimeSpan.FromSeconds(1)));
                Storyboard.SetTargetProperty(db, new PropertyPath("(Canvas.Left)"));
                Storyboard.SetTargetProperty(da, new PropertyPath("(Canvas.Top)")); // EPIC FONTOS A ZAROJELEZES
                sb.Children.Add(da);
                sb.Children.Add(db);

                (canvas.Children[turtleimage.CPos] as Image).BeginStoryboard(sb);

                //TranslateTransform translate = new TranslateTransform();
                //(canvas.Children[0] as Image).RenderTransform = translate;
                //translate.BeginAnimation(TranslateTransform.XProperty, db);
                //translate.BeginAnimation(TranslateTransform.YProperty, da);
            }
        }

        //Teknőst forgatja a Canvason
        private void TurtleRotate(int angle)
        {
            if (turtleimage.Visible)
            {
                RotateTransform rotateTransform1 = new RotateTransform(angle - 90, turtleimage.Size / 2, turtleimage.Size / 2);
                canvas.Children[turtleimage.CPos].RenderTransform = rotateTransform1;
            }
        }

        //Balra fordítja a tekőst 'angle' fokkal
        public void Left(int angle)
        {
            this.angle -= angle;
            TurtleRotate(this.angle);
        } 

        //Jobbra fordítja a tekőst 'angle' fokkal
        public void Right(int angle)
        {
            this.angle += angle;
            TurtleRotate(this.angle);
        } 

        //törli a Canvast és vissza állítja a teknőst a kezdő pontba
        public void Clean()
        {
            canvas.Children.Clear();
            turtleimage.CPos = canvas.Children.Add(turtleimage.GetTurtleImage);
            Home();
        } 

        //Visszaállítja a teknőst a Canvas közepére, (0,0) pontba
        public void Home()
        {
            pos.X = canvas.Width / 2d;
            pos.Y = canvas.Height / 2d;
            angle = 90;
            //TurtleMove(pos);
            Canvas.SetTop(canvas.Children[turtleimage.CPos], pos.Y - turtleimage.Size / 2);
            Canvas.SetLeft(canvas.Children[turtleimage.CPos], pos.X - turtleimage.Size / 2);
            TurtleRotate(angle);
        }

        //Vissza adja a teknős X pozicióját az origóhoz(teknős kezdő pontja) képest
        public int XPos()
        {
            return (int)(pos.X - (canvas.Width) / 2);
        } 

        //Vissza adja a teknős Y pozicióját az origóhoz(teknős kezdő pontja) képest
        public int YPos()
        {
            return (int)(pos.Y - (int)(canvas.Height) / 2);
        } 

        //fokból radiánt csinál
        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        //Tollat felemeli
        public void PenUp()
        {
            pen.PenUp();
        }

        //Tollat leteszi
        public void PenDown()
        {
            pen.PenDown();
        }

        //Visszadja, hogy éppen lent van e a toll, rajzol-e
        public bool PenIsDown()
        {
            return pen.IsDown;
        }

        //beállítja a toll színét
        public void PenColor(Color penColor)
        {
            if(penColor != null) pen.Color = penColor;
        }

        //beállítja a toll vastagságát
        public void PenSize(int penSize)
        {
            if (penSize > 0) pen.Pensize = penSize;
        }

        //A teknős láthatóságának kikapcsolása
        public void TurtleOff()
        {
            turtleimage.Visible = false;
        }

        //A teknős láthatóságának bekapcsolása
        public void TurtleOn()
        {
            turtleimage.Visible = true;
        }
        //Megmondja h látható e a teknős
        public bool TurtleIsVisible()
        {
            return turtleimage.Visible;
        }

        //Megállítja a teknős
        public void Wait(int millisecundum)
        {
            if (millisecundum > 0) System.Threading.Thread.Sleep(millisecundum);
        }
    }
}
