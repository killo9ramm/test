using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RBClient.Classes
{
    class FileSystemHelper
    {
        public static DirectoryInfo GetADvVideoDir()
        {
            return prepareDirectoryInRoot(StaticConstants.ADV_VIDEO_FOLDER, false);
        }

        public static DirectoryInfo GetEducVideoDir()
        {
            return prepareDirectoryInRoot(StaticConstants.EDUC_VIDEO_FOLDER, false);
        }

        public static DirectoryInfo GetInnerDocsDir()
        {
            return prepareDirectoryInRoot(StaticConstants.INNER_DOCS_FOLDER, false);
        }

        public static DirectoryInfo prepareDirectoryInRoot(string dirName, bool clear)
        {
            DirectoryInfo _folder = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dirName));
            _folder.CreateOrReturn();

            if (clear && !_folder.IsEmpty())
            {
                _folder.DeleteOldFilesInDir(0);
                _folder.GetDirectories().ToList().ForEach(a => a.Delete(true));
            }
            return _folder;
        }
    }
}
