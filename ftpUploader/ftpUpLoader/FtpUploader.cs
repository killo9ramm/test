using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ftp;
using CustomLogger;
using System.IO;
using RBClient.Classes;

namespace ftpUpLoader
{
    class FtpUploader : LoggerBase
    {
        FtpSession m_client;
        string server;
        string port;
        string username;
        string password;

        public FtpUploader(string server,string port,string username,string password)
        {
            m_client = new FtpSession();

            this.server = server; m_client.Server = server;
            this.port = port; m_client.Port = Convert.ToInt32(port);
            this.username = username;
            this.password = password;
        }

        public void Connect()
        {
             Connect(username,password);
        }

        public void Close()
        {
            m_client.Close();
        }
        private void Connect(string _username,string _password)
        {
            m_client.Connect(_username, password); 
        }

        public bool SendFile(FileInfo file,List<string> remote_folders,bool useConnection)
        {
            try
            {
                if(useConnection)Connect(username, password);

                if (remote_folders == null || remote_folders.Count == 0) throw new ArgumentOutOfRangeException("remote_folders", "Внутренняя структура папок не должна быть пуста");

                if (m_client.CurrentDirectory.Name != remote_folders.Last())
                {
                    remote_folders.ForEach(a =>
                    {
                        Ftp.FtpDirectory ftp_dir = m_client.CurrentDirectory.FindSubdirectory(a);
                        if (null != ftp_dir)
                        {
                            m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory(a);
                        }
                        else
                        {
                            m_client.CurrentDirectory.CreateSubdir(a);
                            m_client.CurrentDirectory.Refresh();
                            m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory(a);
                        }
                    });
                }

                //получить текущую директорию

                //отправить файл
                SendFileResuming(m_client, file);
                if (useConnection) m_client.Close();
                return true;
            }catch(Exception ex)
            {
                Log(ex, "Не получилось отправить файл " + file +" объект "+ Serializer.JsonSerialize(m_client));
                return false;
            }
        }
        public bool SendFile(List<FileInfo> files)
        {
            return false;
        }

        public List<string> GetFileList(List<string> remote_folders)
        {
            string folders = "";
            try
            {
                foreach (string dir in remote_folders)
                {
                    m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory(dir.ToLower());
                    folders=Path.Combine(folders,dir);
                }

                VbEnumableCollection items = m_client.CurrentDirectory.Files;

                List<string> file_names = new List<string>();
                foreach (FtpFile a in items)
                {
                    file_names.Add(a.Name);
                }

                return file_names;
            }
            catch (Exception ex)
            {
                Log(ex, "Не удалось получить список файлов из папок "+folders);
                return null;
            }
        }


    public FileInfo GetFile(String localPath, String name,bool deleteOnftp)
        {
            GetFileResuming(m_client,localPath,name);
            if (File.Exists(localPath))
            {
                if (deleteOnftp)
                {
                    m_client.CurrentDirectory.RemoveFile(name);
                }
                return new FileInfo(localPath);
            }
            return null;
        }

    private void SendFileResuming(FtpSession m_client, FileInfo _fi)
        {
            Log("SendFileResuming [_fi=" + _fi + "]");
            try
            {
                long offset = 0;
                string tmpName = _fi.Name + ".tmp";

                Boolean needToTransfer = true;
                // проверяем есть ли старый файл
                FtpFile oldFtpFile = m_client.CurrentDirectory.FindFile(_fi.Name);
                if (oldFtpFile != null)
                {
                    if (oldFtpFile.Size < _fi.Length)
                    {
                        m_client.CurrentDirectory.RemoveItem(oldFtpFile);
                    }
                    else
                    {
                        needToTransfer = false;
                    }
                }
                else
                {
                    Log("oldFtpFile==null");
                }
                if (needToTransfer)
                {
                    // проверяем есть ли временный файл
                    FtpFile ftpFile = m_client.CurrentDirectory.FindFile(tmpName);
                    if (ftpFile != null)
                    {
                        // продолжаем с места остановки
                        offset = ftpFile.Size;
                    }
                    else
                    {
                        Log("1. ftpFile==null");
                    }
                    if (offset < _fi.Length)
                    {
                        m_client.CurrentDirectory.PutFile(_fi.FullName, tmpName, offset);
                    }
                    // находим временный файл
                    m_client.CurrentDirectory = m_client.CurrentDirectory;

                    ftpFile = m_client.CurrentDirectory.FindFile(tmpName);
                    if (ftpFile != null)
                    {
                        // переименовываем в нормальное имя
                        m_client.CurrentDirectory.RenameSubitem(ftpFile, _fi.Name);
                    }
                    else
                    {
                        Log("2. ftpFile==null");
                    }
                }
                else
                {
                    Log("needToTransfer==false");
                }
            }
            catch (Exception exp)
            {
                Log(exp.ToString());
                throw(new Exception("Передаем исключение дальше",exp));
            }
        }

    private void GetFileResuming(FtpSession m_client, String localPath, String name)
{
    Log("GetFileResuming [localPath='" + localPath + "', name='" + name + "'");
    try
    {
        FtpFile ftpFile = m_client.CurrentDirectory.FindFile(name);
        if (ftpFile != null)
        {
            long offset = 0;
            string tmpName = localPath + ".tmp";
            Log("tmpName='" + tmpName + "'");

            Boolean needToTransfer = true;
            // проверяем есть ли старый файл
            FileInfo fiOld = new FileInfo(localPath);
            if (fiOld.Exists)
            {
                if (fiOld.Length < ftpFile.Size)
                {
                    fiOld.Delete();
                }
                else
                {
                    needToTransfer = false;
                }
            }
            else
            {
                Log("fiOld is not exist");
            }

            if (needToTransfer)
            {
                // проверяем есть ли временный файл
                FileInfo fiTmp = new FileInfo(tmpName);
                if (fiTmp.Exists)
                {
                    offset = fiTmp.Length;
                }
                else
                {
                    Log("1. fiTmp is not exist");
                }
                if (offset < ftpFile.Size)
                {
                    m_client.CurrentDirectory.GetFile(tmpName, name, offset);
                }
                // находим временный файл
                fiTmp = new FileInfo(tmpName);
                if (fiTmp.Exists)
                {
                    // переименовываем в нормальное имя
                    fiTmp.MoveTo(localPath);
                }
                else
                {
                    Log("2. fiTmp is not exist");
                }
            }
            else
            {
                Log("needToTransfer==false");
            }
        }
        else
        {
            Log("ftpFile==null");
        }
    }
    catch (Exception exp)
    {
        Log("Exception: " + exp);
        throw exp;
    }
}


    }
}