using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBServer.Debug_classes
{
    class Directory
    {
        internal static bool Exists(string source)
        {
            return System.IO.Directory.Exists(source);
        }

        internal static DirectoryInfo CreateDirectory(string source)
        {
            System.IO.DirectoryInfo dir=System.IO.Directory.CreateDirectory(source);
            return new DirectoryInfo(dir.FullName);
        }
    }
}
