using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;

namespace RBClient
{
    class CStandart
    {
        public string c_hash;
        string log_file = "log_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_error.log";

        private void button1_Click(object sender, EventArgs e)
        {
            Log("(+) - Файл новый");
            Log("(-) - Файл не найден");
            Log("(!) - Файл изменен");
            Log("(*) - Путь не найден");
            Log("(?) - Прочее");
            Log(" ");
            CBData _db = new CBData();
            DirectoryInfo _dir_folder = new DirectoryInfo("C://RBС");
            WalkDirectoryTree(_dir_folder);
            DataTable _dt_item;
            _dt_item = _db.QueryTable();

            WriteLog(_dt_item);
        }

        public void WalkDirectoryTree(DirectoryInfo root)
        {
            string _name;
            string _hash_code;
            DateTime _date_time;

            OleDbConnection _conn = null;
            _conn = new OleDbConnection(CParam.ConnString);
            _conn.Open();

            try
            {
                foreach (var dirInfo in root.GetDirectories()) WalkDirectoryTree(dirInfo);
                {
                    DirectoryInfo _dir_sf = new DirectoryInfo(root.FullName);
                    foreach (FileInfo _file in _dir_sf.GetFiles())
                    {
                        CalculateHash(_file.FullName);
                        _name = _file.FullName;
                        _hash_code = c_hash;
                        _date_time = _file.CreationTime;
                        CBData _db = new CBData();

                        if (_db.FileSearch(_name))
                        {
                            if (_db.СompareStandart(_name) == _hash_code)
                            {
                                //Все нормально.
                            }
                            else
                            {
                                Log("(!) - " + _name);
                            }
                        }
                        else
                        {
                            Log("(+) - " + _name);
                        }
                    }
                }
            }
            catch (Exception _exp)
            {
                Log("(*) - " + _exp.Message);
            }
        }
 
        private void CalculateHash(string _file)
        {
            try
            {
                FileStream fs = System.IO.File.Open(_file, FileMode.Open, FileAccess.Read, FileShare.Read);
                byte[] hash = new byte[16];
                System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                hash = md5.ComputeHash(fs);
                c_hash = HashToString(hash);
            }
            catch (Exception _exp)
            {
                Log("(?) - " + _exp.Message);
            }
            finally
            {

            }
        }

        private string HashToString(byte[] hash)
        {
            string ret = "";
            for (int i = 0; i < hash.Length; i++)
            {
                ret += String.Format("{0:X2}", hash[i]);
            }
            return ret;
        }

        public void Log(string message)
        {
            TextWriter m_file = null;

            try
            {
                m_file = new StreamWriter(log_file, true, System.Text.Encoding.GetEncoding(1251));
                m_file.WriteLine(message);
            }
            catch (Exception)
            {
            }
            finally
            {
                if (m_file != null)
                    m_file.Close();
            }
        }

        protected void WriteLog(DataTable dt)
        {
            try
            {
                // формируем запись
                foreach (DataRow _row in dt.Rows)
                {
                    if (File.Exists(_row[0].ToString()))
                    {

                    }
                    else
                    {
                        Log("(-) - " + _row[0].ToString());
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void Exp(string message)
        {
            Log(message);
        }
    }
}
