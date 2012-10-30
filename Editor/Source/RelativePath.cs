using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Editor
{
    class RelativePath
    {
        public static string GetPath(string relativeFileName)
        {
            string fullPath = Environment.CurrentDirectory;
            string[] dirs = fullPath.Split(Path.DirectorySeparatorChar);
            List<string> rdirs = new List<string>();

            for (int i = 1; i < dirs.Length - 2; i++)
            {
                rdirs.Add(dirs[i]);
            }
            return Path.GetPathRoot(fullPath) + Path.Combine(rdirs.ToArray()) + '\\' + relativeFileName;
        }
    }
}
