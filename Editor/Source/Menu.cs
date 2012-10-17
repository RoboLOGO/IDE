using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Editor
{
    class Menu
    {


        SQLiteHelper sqlitehelp = SQLiteHelper.GetSqlHelper();

        //mentés
        public void Save(string source)
        {
            sqlitehelp.SetSourceCode(source);
        }
        //mentés másként
        public void SaveAs(string source, string sourceFile)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = ".rlsln"; 
            sfd.Filter = "RoboLOGO Solution (.rlsln)|*.rbsln";
            sfd.FileName = "Projekt1";
            bool? result = sfd.ShowDialog();
            if (result == true)
            {
                sqlitehelp.SetSourceCode(source);
                sqlitehelp.Close();
                File.Copy(sourceFile, sfd.FileName);
                sqlitehelp.Open();
            }
        }
        //megnyitás
        public bool? Open()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".rlsln"; 
            ofd.Filter = "RoboLOGO Solution (.rlsln)|*.rbsln";
            bool? result = ofd.ShowDialog();
            sqlitehelp.SetSourceCode(ofd.FileName);
            return result;
        }
        #region --Kép Mentés--
        public void Image_Save(Canvas canvas, Window window)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = ".png";
            sfd.FileName = "image";
            sfd.Filter = "Portable Network Graphics (.png)|*.png";
            sfd.ShowDialog();

            SaveCanvas(window, canvas, 96, sfd.FileName);
        }
        private static void SaveCanvas(Window window, Canvas canvas, int dpi, string filename)
        {
            Size size = new Size(canvas.RenderSize.Width, canvas.RenderSize.Height);
            canvas.Measure(size);
            canvas.Arrange(new Rect(size));

            var rtb = new RenderTargetBitmap(
                (int)canvas.RenderSize.Width, //width 
                (int)canvas.RenderSize.Height, //height 
                dpi, //dpi x 
                dpi, //dpi y 
                PixelFormats.Pbgra32 // pixelformat 
                );
            rtb.Render(canvas);

            SaveRTB2PNG(rtb, filename);
        }
        private static void SaveRTB2PNG(RenderTargetBitmap bmp, string filename)
        {
            var enc = new System.Windows.Media.Imaging.PngBitmapEncoder();
            enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bmp));

            using (var stm = System.IO.File.Create(filename))
            {
                enc.Save(stm);
            }
        }
        #endregion
        //futtatás
        public void Run(ref Turtle turtle, Canvas canvas)
        {
            if (!sqlitehelp.IsOpen())
            {
                MessageBox.Show("Nincs projekted");
                return;
            }
            if (turtle == null)
            {
                turtle = new Turtle(canvas);
            }
            LogoRun run = new LogoRun();
            string sourceCode = sqlitehelp.GetSourceCode();
            run.Run(sourceCode);
            run.Draw(turtle);
        }
        //képernyő törlés
        public void clear(Turtle turtle)
        {
            if (turtle != null)
                turtle.Clean();
        }
    }
}
