using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using ZippArhiveHelper;
using CustomLogger;
using RBClient.Classes;

namespace ZreportWork
{
    class ZreportMaker : LoggerBase
    {
        static Regex dx_reg=new Regex(@"DX(\d+[.]\d+)");
        static Regex x_reg = new Regex(@"^X(\d+[.]\d+)");
        private const string Y_FILE_CONTENT = @"(?s)(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(.+)";
        private const string X_REPORT_NAME_REG = @"(\S)(\d{2})(\d{2})(\d{2})(\S+?)[.](\d+)";

        private static string[] GetXInfoFromXname(string zrep_file)
        {
            string regg = X_REPORT_NAME_REG;
            Regex reg = new Regex(regg);
            if (reg.IsMatch(zrep_file))
            {
                string[] st = new string[6];
                Match m = reg.Match(zrep_file);
                st[0] = m.Groups[1].Value;
                st[1] = m.Groups[2].Value;
                st[2] = m.Groups[3].Value;
                st[3] = m.Groups[4].Value;
                st[4] = m.Groups[5].Value;
                st[5] = m.Groups[6].Value;


                //1: X
                //2: 15
                //3: 03
                //4: 01
                //5: 216
                //6: 537

                return st;
            }
            return null;
        }
        private DateTime getXDateFromInfo(string[] info)
        {
            var xdate = getDateFromInfo(info);
            if (xdate != null)
                return (DateTime)xdate;
            else
            {
                string message = "Ошибка парсинга x файла " + Serializer.JsonSerialize(info);
                Log(message);
                throw new Exception(message);
            }
        }
        private DateTime? getDateFromInfo(string[] info)
        {
            try
            {
                DateTime xdate = new DateTime(int.Parse("20" + info[1]), int.Parse(info[2]), int.Parse(info[3]));
                return xdate;
            }
            catch (Exception ex)
            {
                string message = "Ошибка парсинга файла z-отчета" + Serializer.JsonSerialize(info);
                Log(message);
                return null;
            }
        }

        /// <summary>
        /// Находим другие файлы от z-отчета 
        /// </summary>
        /// <param name="a">файл x</param>
        /// <returns>Список файлов для z-отчета</returns>
        public List<FileInfo> findAnotherFilesOnX(FileInfo a)
        {

            List<FileInfo> files_of_z = new List<FileInfo>();
            try
            {

                DirectoryInfo dir = a.Directory;

                var info = GetXInfoFromXname(a.Name);
                var xdate = getXDateFromInfo(info);


                string z_base = "*" + a.Name.Substring(1, 2) + "*" + a.Name.Substring(7);

                files_of_z.AddRange(dir.GetFiles(z_base).Where(b =>
                {
                    try
                    {
                        var _info = GetXInfoFromXname(b.Name);
                        var _date = getDateFromInfo(_info);
                        if (_date == null)
                            return false;
                        else
                        {

                            double totdays = (xdate - ((DateTime)_date)).TotalDays;
                            if (Math.Abs(totdays) > 2)
                                return false;
                            else return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }));

                if (files_of_z.Count == 0)
                    return null;
            }
            catch (Exception ex)
            {
                Log(ex, "findAnotherFilesOnX error");
            }
            return files_of_z;
        }
        #region old
        //internal List<FileInfo> findAnotherFilesOnX(FileInfo a)
        //{
        //    List<FileInfo> files_of_z = new List<FileInfo>();
        //    DirectoryInfo dir = a.Directory;
        //    string z_base = "*" + a.Name.Substring(1, 4) + "*" + a.Extension;
        //    files_of_z.AddRange(dir.GetFiles(z_base));

        //    if (files_of_z.Count == 0)
        //        return null;
        //    return files_of_z;
        //}
        //internal List<FileInfo> findAnotherFilesOnX(FileInfo a, bool useShiftsNumbers)
        //{

        //    List<FileInfo> files_of_z = new List<FileInfo>();
        //    DirectoryInfo dir = a.Directory;
        //    string z_base = "";
        //    if (useShiftsNumbers)
        //    {
        //        z_base = "*" + a.Extension;

        //    }
        //    else
        //    {
        //        z_base = "*" + a.Name.Substring(1, 4) + "*" + a.Extension;
        //    }

        //    files_of_z.AddRange(dir.GetFiles(z_base));

        //    if (files_of_z.Count == 0)
        //        return null;
        //    return files_of_z;
        //}

        #endregion

        internal List<FileInfo> findAnotherFilesOnX(FileInfo a, bool useShiftsNumbers)
        {
            return findAnotherFilesOnX(a);
        }

        

        internal List<FileInfo> prepareZpackage(FileInfo x_file,bool useShiftsNumbers)
        {
            if (useShiftsNumbers)
            {
                return prepareZpackage_new(x_file, useShiftsNumbers);
            }
            else
            {
                return prepareZpackage(x_file);
            }
        }

        internal List<FileInfo> prepareZpackage_new(FileInfo x_file,bool useShiftsNumbers)
        {
            List<FileInfo> files = findAnotherFilesOnX(x_file, useShiftsNumbers);

            files = files.Where(c =>
            { return (!c.Name.StartsWith("T") && !c.Name.StartsWith("DT")); }).ToList();

            List<FileInfo> y_files = files.Where(a => a.Name.StartsWith("Y")).ToList();

            if (y_files.Count > 1)
            {
                var temp = y_files.Where(a => isYFiscalNull(a));
                files.RemoveAll(x => temp.Contains(x));
            }
            return files;
        }

        /// <summary>
        /// Подготовить файлы для зашивки в z-репорт
        /// </summary>
        /// <param name="x_file">имя x-файла</param>
        /// <returns></returns>
        internal List<FileInfo> prepareZpackage(FileInfo x_file)
        {
            List<FileInfo> files = findAnotherFilesOnX(x_file);

            files = files.Where(c =>
            { return (!c.Name.StartsWith("T") && !c.Name.StartsWith("DT")); }).ToList();

            List<FileInfo> y_files = files.Where(a => a.Name.StartsWith("Y")).ToList();

            //if (y_files.Count > 1)
            //{
            //    FileInfo y_file_right = y_files.OrderBy(x => x.Name).ThenByDescending(x => x.Length).First();
            //    y_files.Remove(y_file_right);
            //    files.RemoveAll(x => y_files.Contains(x));
            //}

            if (y_files.Count > 1)
            {
                var temp = y_files.Where(a => isYFiscalNull(a));
                files.RemoveAll(x => temp.Contains(x));
            }
            return files;
        }

        private bool isYFiscalNull(FileInfo yfile)
        {
            var info = getYFileContent(yfile);
            if (double.Parse(info[2]) == 0)
            {
                return true;
            }
            return false;
        }

        private string[] getYFileContent(FileInfo yfile)
        {
            string text = File.ReadAllText(yfile.FullName);
            string regg = Y_FILE_CONTENT;
            Regex reg = new Regex(regg);
            if (reg.IsMatch(text))
            {
                string[] st = new string[5];
                Match m = reg.Match(text);
                st[0] = m.Groups[1].Value;
                st[1] = m.Groups[2].Value;
                st[2] = m.Groups[3].Value;
                st[3] = m.Groups[4].Value;
                st[4] = m.Groups[5].Value;

                //1: 216
                //2: 539
                //3: 003272
                //4: 000512
                //5: 16079457.22

                return st;
            }
            return null;
        }

        /// <summary>
        ///  Создаем z отчеты и перемещает в папку
        /// </summary>
        /// <param name="files">Первоначальный список файлов</param>
        /// <param name="dest_dir">Папка назначения</param>
        /// <returns></returns>
        public List<FileInfo> makeZreports(List<FileInfo> files,string name_prefix, DirectoryInfo dest_dir, DirectoryInfo back_dir)//,bool remove_form_files,Action<List<FileInfo>> before_creation)
        {
            List<FileInfo> ZReports = new List<FileInfo>();
            List<FileInfo> XList = files.Where(a =>x_reg.IsMatch(a.Name)).ToList();
            List<FileInfo> operated_files = new List<FileInfo>();

            XList.ForEach(a =>
            {
                        try
                        {
                            string num_name = a.Name.Substring(1);

                            //получить нужные файлы для z-отчета
                            List<FileInfo> files_to_z = prepareZpackage(a);

                            //создать z-отчеты
                            FileInfo zrep = makeZreport(files_to_z, name_prefix + num_name + ".zip", dest_dir);

                            //копируем в outbox
                            if (zrep != null)
                            {
                                ZReports.Add(zrep);
                                operated_files.AddRange(files_to_z);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log(ex, "Ошибка создания z_отчета " + a.FullName);
                        }
             });

            if (operated_files.Count == 0)
            {
                return null;
            }
            else
            {
                if (back_dir != null)
                {
                    operated_files.ForEach(a =>
                    {
                        a.CopyTo(Path.Combine(back_dir.FullName, a.Name), true);
                        a.Delete();
                    });
                }
                return ZReports;
            }
        }


        /// <summary>
        ///  Создаем z отчеты и перемещает в папку
        /// </summary>
        /// <param name="files">Первоначальный список файлов</param>
        /// <param name="dest_dir">Папка назначения</param>
        /// <returns></returns>
        public List<FileInfo> makeZreports(List<FileInfo> files, string name_prefix, DirectoryInfo dest_dir, DirectoryInfo back_dir,bool useShiftsNumbers)//,bool remove_form_files,Action<List<FileInfo>> before_creation)
        {
            List<FileInfo> ZReports = new List<FileInfo>();
            List<FileInfo> XList = files.Where(a => x_reg.IsMatch(a.Name)).ToList();
            List<FileInfo> operated_files = new List<FileInfo>();

            XList.ForEach(a =>
            {
                try
                {
                    string num_name = a.Name.Substring(1);

                    //получить нужные файлы для z-отчета
                    List<FileInfo> files_to_z = prepareZpackage(a, useShiftsNumbers);

                    //создать z-отчеты
                    FileInfo zrep = makeZreport(files_to_z, name_prefix + num_name + ".zip", dest_dir);

                    //копируем в outbox
                    if (zrep != null)
                    {
                        ZReports.Add(zrep);
                        operated_files.AddRange(files_to_z);
                    }
                }
                catch (Exception ex)
                {
                    Log(ex, "Ошибка создания z_отчета " + a.FullName);
                }
            });

            if (operated_files.Count == 0)
            {
                return null;
            }
            else
            {
                if (back_dir != null)
                {
                    operated_files.ForEach(a =>
                    {
                        a.CopyTo(Path.Combine(back_dir.FullName, a.Name), true);
                        a.Delete();
                    });
                }
                return ZReports;
            }
        }

        /// <summary>
        /// Проверить правильность файлов для z-отчета
        /// </summary>
        /// <param name="file_list">Список файлов</param>
        /// <param name="num_name">Имя файлов</param>
        /// <returns></returns>
        private bool ZFilesIsRight(List<FileInfo> file_list,string num_name)
        {
            if (file_list.Where(a => a.Name == "X" + num_name).Count() != 1) return false;
            if (file_list.Where(a => a.Name == "DX" + num_name).Count() != 1) return false;
            if (file_list.Where(a => a.Name == "Y" + num_name).Count() != 1) return false;
            return true;
        }

        /// <summary>
        ///  Создаем z отчет
        /// </summary>
        /// <param name="files">Первоначальный список файлов</param>
        /// <param name="dest_dir">Папка назначения</param>
        /// <returns></returns>
        public FileInfo makeZreport(List<FileInfo> files,string ZreportName, DirectoryInfo dest_dir)
        {
            FileInfo zrep=ArhiveManager.CreateArchiveFile(files, ZreportName, dest_dir);
            return zrep;
        }
    }
}
