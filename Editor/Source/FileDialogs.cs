using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Editor
{
    class FileDialogs
    {
        static string filter = "RoboLOGO Solution (.rbsln)|*.rbsln";
        static string ext = ".rlsln";

        public static OpenFileDialog GetOpenFileDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ext;
            ofd.Filter = filter;
            return ofd;
        }

        public static SaveFileDialog GetSaveFileDialog()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = ext;
            sfd.Filter = filter;
            sfd.FileName = "Projekt";
            return sfd;
        }

        public static SaveFileDialog SavePngDialog()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = ".png";
            sfd.FileName = "image";
            sfd.Filter = "Portable Network Graphics (.png)|*.png";
            return sfd;
        }
    }
}
