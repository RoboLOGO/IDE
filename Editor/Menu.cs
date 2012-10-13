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
        SaveFileDialog sfd;
        StreamWriter stw;
        public OpenFileDialog dlg;
        //mentés
        public void Save(string s)
        {
            if (sfd == null) sfd = new SaveFileDialog();
            if (sfd.FileName == String.Empty) sfd.ShowDialog();
            try
            {
                stw = new StreamWriter(sfd.FileName, false, Encoding.UTF8);
                stw.Write(s);
                stw.Flush();
            }
            catch { }
            finally { stw.Close(); }
        }
        //mentés másként
        public void SaveAs(string s)
        {
            if (sfd == null) sfd = new SaveFileDialog();
            sfd.ShowDialog();
            try
            {
                stw = new StreamWriter(File.OpenWrite(sfd.FileName), Encoding.UTF8);
                stw.Write(s);
                stw.Flush();

            }
            catch { }
            finally { stw.Close(); }
        }
        //megnyitás
        public bool? Open()
        {
            dlg = new OpenFileDialog();
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";
            bool? result = dlg.ShowDialog();
            //fájlnév
            return result;
        }
        #region --Kép Mentés--
        public void Image_Save(Canvas canvas, Window window)
        {
            sfd = new SaveFileDialog();
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
        public void run(Turtle turtle, Canvas canvas)
        {
            if (turtle == null)
            {
                turtle = new Turtle(canvas);
            }
            Logo_Run runn = new Logo_Run();
            runn.Read_line(sfd.FileName);
            for (int i = 0; i < runn.line.Count; i++)
            {
                runn.Spearate(i);
            }
            runn.Draww(turtle, runn.comm);
        }
        //képernyő törlés
        public void clear(Turtle turtle)
        {
            if (turtle != null)
                turtle.Clean();
        }
    }
}
