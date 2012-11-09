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

        public string GetFullSource()
        {
            string full = "";
            full += GetAllVar(sqlitehelp.GetAllVariablesName());
            full += GetAllMethod(sqlitehelp.GetAllMethodName());
            full += sqlitehelp.GetSourceCode();
            return full;
        }

        private string GetAllMethod(List<string> list)
        {
            string all = "";
            foreach (string name in list)
            {
                all += sqlitehelp.GetMethod(name);
            }
            return all;
        }

        private string GetAllVar(List<string> list)
        {
            string all = "";
            string mname = App.Current.TryFindResource("kw_keszit").ToString();
            foreach (string name in list)
            {
                all += mname + " \"" + name + " " + sqlitehelp.GetVariable(name) + "\r\n";
            }
            return all;
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


        //képernyő törlés
        public void Clear(Turtle turtle)
        {
            turtle.Clean();
        }
    }
}
