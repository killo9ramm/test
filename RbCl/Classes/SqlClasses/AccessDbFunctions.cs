using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using CustomLogger;

namespace RBClient.Classes
{
    class AccessDbFunctions : LoggerBase
    {
        /// <summary>
        /// возвращает путь к базе данных из строки подключения
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        public FileInfo returnCurrentDataBase(string connString)
        {
            Regex reg = new Regex("Data Source=(.*)");
            string dataBase_path = reg.Match(connString).Groups[1].Value;
            FileInfo fi = new FileInfo(dataBase_path);
            if (fi.Exists)
            {
                return fi;
            }
            else
            {
                Exception ex=new Exception("Не удалось найти файл базы данных " + dataBase_path);
                Log(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Делаем бэкап базы данных
        /// </summary>
        /// <param name="Path">путь к базе</param>
        /// <param name="destPath">путь для бэкапа</param>
        /// <param name="removeOldDb">удалять ли старую базу?</param>
        /// <returns></returns>
        public bool BackUpDataBase(string Path, string destPath,bool removeOldDb)
        {
            try
            {
                FileInfo fi = new FileInfo(Path);
                FileInfo fib = new FileInfo(destPath);
                if (fib.Exists)
                {
                    //destPath += "_new";
                    return true;
                }

                fi.CopyTo(destPath);
                if (removeOldDb)
                {
                    fi.Delete();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log(ex);
                return false;
            }
        }


        /// <summary>
        /// Делаем бэкап базы данных
        /// </summary>
        /// <param name="Path">путь к базе</param>
        /// <param name="destPath">путь для бэкапа</param>
        /// <param name="removeOldDb">удалять ли старую базу?</param>
        /// <returns></returns>
        public bool BackUpDataBase(string Path, string destPath, bool removeOldDb,bool overwrite)
        {
            try
            {
                FileInfo fi = new FileInfo(Path);
                FileInfo fib = new FileInfo(destPath);
                if (fib.Exists && overwrite)
                {
                    if (overwrite)
                    {
                        fib.Delete();
                    }
                    else
                    {
                        return true;
                    }
                }

                fi.CopyTo(destPath);
                if (removeOldDb)
                {
                    fi.Delete();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log(ex);
                return false;
            }
        }

        /// <summary>
        /// Компактит базу аццесс
        /// </summary>
        /// <param name="dbPath_src">исходная</param>
        /// <param name="dbPath_dest">компактная</param>
        /// <returns></returns>
        public bool CompactDataBase(string dbPath_src, string dbPath_dest)
        {
            try
            {
                Microsoft.Office.Interop.Access.Dao.DBEngine objDbEngine = new Microsoft.Office.Interop.Access.Dao.DBEngine();
                objDbEngine.CompactDatabase(dbPath_src, dbPath_dest);
                return true;
            }catch(Exception ex)
            {
                Log(ex, "Не удалось скомпактнуть базу " + dbPath_src);
                return false;
            }
        }

    }
}
