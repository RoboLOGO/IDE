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
using System.Threading;

namespace Editor
{
    class Menu
    {
        SQLiteHelper sqlitehelp;
        static Menu self;

        private Menu()
        {
            sqlitehelp = SQLiteHelper.GetSqlHelper;
        }
        public static Menu GetMenu
        {
            get
            {
                if (self == null) self = new Menu();
                return self;
            }
        }

        //mentés
        public void Save(string source)
        {
            sqlitehelp.SetSourceCode(source);
        }

        //mentés másként
        public void SaveAs(string source, string sourceFile)
        {
            SaveFileDialog sfd = FileDialogs.GetSaveFileDialog();
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
            OpenFileDialog ofd = FileDialogs.GetOpenFileDialog();
            bool? result = ofd.ShowDialog();
            if (result == true) sqlitehelp.OpenFile(ofd.FileName);
            return result;
        }

        #region --Kép Mentés--
        public void Image_Save(Canvas canvas, Window window)
        {
            SaveFileDialog sfd = FileDialogs.SavePngDialog();
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
        public void Run(Turtle turtle, Canvas canvas)
        {
            turtle.Clean();
            LogoRun run = new LogoRun();
            string sourceCode = sqlitehelp.GetSourceCode();
            run.Run(sourceCode);
            run.Draw(turtle);
        }

        //képernyő törlés
        public void Clear(Turtle turtle)
        {
            turtle.Clean();
        }
    }
}
