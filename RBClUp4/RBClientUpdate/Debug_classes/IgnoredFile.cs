using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBServer.Debug_classes
{
    internal partial class DebugPanel
    {
        internal class IgnoredFile
        {
            internal static string extension = ".ignored";

            internal static void MakeFileIgnored(FileInfo _file)
            {
                _file.MoveTo(_file.FullName+extension);
                DebugPanel.OnErrorOccured("Неверный файл в структуре " + _file.FullName + " теперь игнорируется");
            }

            internal static void MakeFileIgnored(string _file)
            {
                FileInfo file = new FileInfo(_file);
                file.MoveTo(_file+extension);
                DebugPanel.OnErrorOccured("Неверный файл в структуре "+file.FullName+" теперь игнорируется");
            }

            internal static bool IsIgnored(FileInfo _file)
            {
                System.IO.FileInfo file = new System.IO.FileInfo(_file.FullName);
                if (file.Extension == extension)
                {
                    return true;
                }
                return false;
            }
            internal static bool IsIgnored(string _file)
            {
                System.IO.FileInfo file = new System.IO.FileInfo(_file);
                if (file.Extension == extension)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
