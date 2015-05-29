using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RBClient;
using System.IO;
using iTuner;
using System.Collections;
using System.Threading;

namespace RBClient
{
    public partial class FormFlash : Form
    {
        public string m_drive;
        public bool m_update = false;
        MDIParentMain mdi = new MDIParentMain();
        FormDoc _fd = new FormDoc();
        ArrayList _array = new ArrayList();

        public FormFlash()
        {
            InitializeComponent();          
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //записываем на флешку.
        private void button_export_Click(object sender, EventArgs e)
        {
            this.SetBounds((Screen.GetBounds(this).Width / 4) - (this.Width / 4),
            (Screen.GetBounds(this).Height / 4) - (this.Height / 4), 375, 303);

            button_import.Visible = false;
            button_export.Visible = false;
            button_export2.Visible = true;
            button_import2.Visible = false;
            checkedListBox_Tasks.Visible = true;
            groupPanel1.Size = new Size(356, 261);
            button_export2.Location = new Point(215, 157);
            button_Cancel.Location  = new Point(215, 196);

            if (m_drive != null)
            {
                progressBarX1.Visible = true;
                InitData();
            }
        }

        //копируем с флеш карты.
        private void button_import_Click(object sender, EventArgs e)
        {
            this.SetBounds((Screen.GetBounds(this).Width / 4) - (this.Width / 4),
           (Screen.GetBounds(this).Height / 4) - (this.Height / 4), 375, 303);

            button_import.Visible = false;
            button_export.Visible = false;
            button_export2.Visible = false;
            button_import2.Visible = true;
            checkedListBox_Tasks.Visible = true;
            groupPanel1.Size = new Size(356, 261);
            button_import2.Location = new Point(215, 157);
            button_Cancel.Location = new Point(215, 196);

            if (m_drive != null)
            {
                progressBarX1.Visible = true;
            }          
        }

        private void FormFlesh_Load(object sender, EventArgs e)
        {
            checkedListBox_Tasks.Visible = false;
            this.SetBounds((Screen.GetBounds(this).Width / 4) - (this.Width / 4),
            (Screen.GetBounds(this).Height / 4) - (this.Height / 4), 320, 200);

            button_import2.Visible = false;
            groupPanel1.Size = new Size(300, 155);
            button_import.Location = new Point(10, 27);
            button_export.Location = new Point(155, 27);
            button_Cancel.Location = new Point(78, 67);
        }

        private void button_export2_Click(object sender, EventArgs e)
        {
            reloadWindow();
            startJob();
        }

        private void reloadWindow()
        {
            progressBarX1.Text = "Копирую документы.";
            progressBarX1.Value = 15;
        }

        private void startJob()
        {
            ToDo();
            write();
        }

        private void write()
        {
            mdi.CheckZReportFlesh();
            progressBarX1.Value = 25;
            mdi.Exchange(m_drive);
            progressBarX1.Value = 35;
            Copy2Flesh();
            progressBarX1.Value = 75;
            Thread.Sleep(5000);
            progressBarX1.Text = "Запись данных завершена.";
            progressBarX1.Value = 100;
        }

        private void InitData()
        {
            string _d;
            try
            {
                CBData _data = new CBData();
                DataTable _dt = _data.GetDoc2Send();
                foreach (DataRow _row in _dt.Rows)
                {
                    _d = _row[0].ToString() + " №:" + _row[1].ToString() + ": (от " + _row[3].ToString() + ")";
                    checkedListBox_Tasks.Items.Add(_d, true);
                }
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        //помететь в базе, что выбранные документы готовы к отправке.
        private void ToDo()
        {
            string[] _s;
            ArrayList _array = new ArrayList();
            char[] _separator = ":".ToCharArray();
            CBData _data = new CBData();

            try
            {
                int _i = 0;
                for (; _i < checkedListBox_Tasks.Items.Count; _i++)
                {
                    if (checkedListBox_Tasks.GetItemChecked(_i))
                    {
                        _s = checkedListBox_Tasks.Items[_i].ToString().Split(_separator);
                        _data.AddTaskExchange(_s[1]);
                    }
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
            }
        }

        private void Copy2Flesh()
        {
            try
            {
                Thread.Sleep(500);
                DirectoryInfo _dir = new DirectoryInfo(CParam.AppFolder + "\\outbox\\");
                foreach (FileInfo _file in _dir.GetFiles())
                {
                    if (!Directory.Exists(m_drive + "\\outbox\\"))
                        Directory.CreateDirectory(m_drive + "\\outbox\\");
                    File.Copy(_file.FullName, m_drive + "\\outbox\\" + _file.Name, true);
                    File.Delete(CParam.AppFolder + "\\outbox\\" + _file.Name);
                }
            }
            catch
            {
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
                progressBarX1.Value = 55;
                foreach (FileInfo _file in _dir_sf.GetFiles())
                {
                    if (_file.Name.ToLower().EndsWith("avi"))               //копируем видео
                    {
                        if (!Directory.Exists(CParam.AppFolder + "\\video\\"))
                        {
                            Directory.CreateDirectory(CParam.AppFolder + "\\video\\");
                        }
                        File.Copy(_file.FullName, CParam.AppFolder + "\\video\\" + _file.Name, true);
                    }
                    if (_file.Name.ToLower().EndsWith("bmp") || _file.Name.ToLower().EndsWith("png"))       //копируем картинки
                    {
                        if (!Directory.Exists(CParam.AppFolder + "\\img\\"))
                        {
                            Directory.CreateDirectory(CParam.AppFolder + "\\img\\");
                        }
                        File.Copy(_file.FullName, CParam.AppFolder + "\\img\\" + _file.Name, true);
                    }
                    if (_file.Name.ToLower().EndsWith("mht"))           //копируем справку
                    {
                        if (!Directory.Exists(CParam.AppFolder + "\\Help\\"))
                        {
                            Directory.CreateDirectory(CParam.AppFolder + "\\Help\\");
                        }
                        File.Copy(_file.FullName, CParam.AppFolder + "\\Help\\" + _file.Name, true);
                    }
                    if (_file.Name.ToLower().EndsWith("dll"))   //копируем дллки
                    {
                        if (!Directory.Exists(CParam.AppFolder + "\\"))
                        {
                            Directory.CreateDirectory(CParam.AppFolder + "\\");
                        }
                        File.Copy(_file.FullName, CParam.AppFolder + "\\" + _file.Name, true);
                    }
                    if (!_file.Name.ToLower().EndsWith("dll") || !_file.Name.ToLower().EndsWith("mht") || !_file.Name.ToLower().EndsWith("bmp") || !_file.Name.ToLower().EndsWith("png") || !_file.Name.ToLower().EndsWith("avi")) // если это шаблоны и меню. 
                    {
                        if (!Directory.Exists(CParam.AppFolder + "\\Inbox\\"))
                        {
                            Directory.CreateDirectory(CParam.AppFolder + "\\Inbox\\");
                        }
                        File.Copy(_file.FullName, CParam.AppFolder + "\\Inbox\\" + _file.Name, true);
                    }
                }
            }
            catch (Exception _exp)
            {
                throw (_exp);
            }
        }

        private void button_import2_Click(object sender, EventArgs e)
        {
            if (m_drive != null)
            {
                progressBarX1.Text = "Копирую документы.";
                progressBarX1.Value = 15;
                DirectoryInfo _dir_folder = new DirectoryInfo(m_drive + "\\\\");
                WalkDirectoryTree(_dir_folder);
                Thread.Sleep(5000);
                progressBarX1.Text = "Запись данных завершена.";
                progressBarX1.Value = 100;
            }
        }
    }
}