using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace RBClient
{
    public partial class FormCopyFromFlash : Form
    {
        public string m_drive = "";
        ArrayList _array = new ArrayList();

        public FormCopyFromFlash()
        {
            InitializeComponent();
        }

        private void FormCopyFromFlash_Load(object sender, EventArgs e)
        {
          this.SetBounds((Screen.GetBounds(this).Width / 4) - (this.Width / 4),
          (Screen.GetBounds(this).Height / 2) - (this.Height / 2),
          this.Width, this.Height); 
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_import_Click(object sender, EventArgs e)
        {
            if (m_drive != null)
            {
                DirectoryInfo _dir_folder = new DirectoryInfo(m_drive + "\\\\");
                WalkDirectoryTree(_dir_folder);
            }
        }

        public void WalkDirectoryTree(DirectoryInfo root)
        {
            try
            {
                _array.Add(root);
                foreach (var dirInfo in root.GetDirectories())               
                WalkDirectoryTree(dirInfo);
                 DirectoryInfo _dir_sf = new DirectoryInfo(root.FullName);
                 foreach (FileInfo _file in _dir_sf.GetFiles())
                 {
                     if (_file.Name.ToLower().EndsWith("avi"))
                     {
                         if (!Directory.Exists(CParam.AppFolder + "\\video\\"))
                         {
                             Directory.CreateDirectory(CParam.AppFolder + "\\video\\");
                             File.Copy(_file.FullName, CParam.AppFolder + "\\video\\" + _file.Name, true);
                         }
                     }
                     if (_file.Name.ToLower().EndsWith("bmp") || _file.Name.ToLower().EndsWith("png"))
                     {
                         if (!Directory.Exists(CParam.AppFolder + "\\img\\"))
                         {
                             Directory.CreateDirectory(CParam.AppFolder + "\\img\\");
                             File.Copy(_file.FullName, CParam.AppFolder + "\\img\\" + _file.Name, true);
                         }
                     }
                     if (_file.Name.ToLower().EndsWith("mht"))
                     {
                         if (!Directory.Exists(CParam.AppFolder + "\\Help\\"))
                         {
                             Directory.CreateDirectory(CParam.AppFolder + "\\Help\\");
                             File.Copy(_file.FullName, CParam.AppFolder + "\\Help\\" + _file.Name, true);
                         }
                     }
                     else // если это шаблоны и меню. 
                     {
                         if (!Directory.Exists(CParam.AppFolder + "\\Inbox\\"))
                         {
                             Directory.CreateDirectory(CParam.AppFolder + "\\Inbox\\");
                             File.Copy(_file.FullName, CParam.AppFolder + "\\Inbox\\" + _file.Name, true);
                         }
                     }
                 }
            }
            catch (Exception _exp)
            {
                throw (_exp);
            }
        }
    }
}
