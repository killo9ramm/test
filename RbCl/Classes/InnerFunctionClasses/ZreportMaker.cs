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
        public static Regex dx_reg=new Regex(@"DX(\d+[.]\d+)");

        public static Regex x_reg = new Regex(@"X(\d+[.]\d+)");

        /// <summary>
        ///  Создаем z отчеты и перемещает в папку
        /// </summary>
        /// <param name="files">Первоначальный список файлов</param>
        /// <param name="dest_dir">Папка назначения</param>
        /// <returns></returns>
        public List<FileInfo> makeZreports(List<FileInfo> files,string name_prefix, DirectoryInfo dest_dir, DirectoryInfo back_dir)//,bool remove_form_files,Action<List<FileInfo>> before_creation)
        {
            List<FileInfo> ZReports = new List<FileInfo>();
            List<FileInfo> DXList = files.Where(a =>dx_reg.IsMatch(a.Name)).ToList();
            List<FileInfo> operated_files = new List<FileInfo>();


            DXList.ForEach(a =>
            {
                string num_name = dx_reg.Match(a.Name).Groups[1].Value;

                List<FileInfo> files_to_z = files.Where(b => b.Name.IndexOf(num_name) != -1 && b.Name!="T"+num_name).ToList();
                if (ZFilesIsRight(files_to_z, num_name))
                {
                    FileInfo zrep = makeZreport(files_to_z, name_prefix + num_name + ".zip", dest_dir);
                    operated_files.AddRange(files_to_z);
                    ZReports.Add(zrep);
                }
                else
                {
                    string filess="";
                    files_to_z.ForEach(b=>filess+=b.Name+" ");
                    Log("Не правильные файлы для z-репорта " + filess);
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
        /// Проверить правильность файлов для z-отчета
        /// </summary>
        /// <param name="file_list">Список файлов</param>
        /// <param name="num_name">Имя файлов</param>
        /// <returns></returns>
        private bool ZFilesIsRightMin(List<FileInfo> file_list, string num_name)
        {
            if (file_list.Where(a => a.Name == "X" + num_name).Count() != 1) return false;
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

        /// <summary>
        /// Находим другие файлы от z-отчета 
        /// </summary>
        /// <param name="a">файл x</param>
        /// <returns>Список файлов для z-отчета</returns>
        public List<FileInfo> findAnotherFilesOnX(FileInfo a)
        {
            
            List<FileInfo> files_of_z = new List<FileInfo>();
            try{

            DirectoryInfo dir = a.Directory;


            var info = GetXInfoFromXname(a.Name);
            var xdate = getXDateFromInfo(info);
            

            string z_base = "*" + a.Name.Substring(1, 2) + "*"+a.Name.Substring(7);

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
                }catch(Exception ex)
                {
                    Log(ex, "findAnotherFilesOnX loop error");
                    return false;
                }
            }));

            if (files_of_z.Count == 0)
                return null;
            }catch(Exception ex)
            {
                Log(ex, "findAnotherFilesOnX error");
            }
            return files_of_z;
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
        /// Возвращает информацию по z-отчету
            ///1: X
            ///2: 15
            ///3: 03
            ///4: 01
            ///5: 216
            ///6: 537
        /// </summary>
        /// <param name="zrep_file"></param>
        /// <returns></returns>
        private static string[] GetXInfoFromXname(string zrep_file)
        {
            string regg = StaticConstants.RBINNER_CONFIG.GetProperty<string>("x_report_name_reg"
                , @"(\S)(\d{2})(\d{2})(\d{2})(\S+?)[.](\d+)");
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

        /// <summary>
        /// Подготовить файлы для зашивки в z-репорт
        /// </summary>
        /// <param name="x_file">имя x-файла</param>
        /// <returns></returns>
        internal List<FileInfo> prepareZpackage(FileInfo x_file)
        {
            List<FileInfo> files = findAnotherFilesOnX(x_file);
            //Log("++1");

            files = files.Where(c =>
            { return (!c.Name.StartsWith("T") && !c.Name.StartsWith("DT")); }).ToList();
           // Log("++2");
            List<FileInfo> y_files = files.Where(a => a.Name.StartsWith("Y")).ToList();
            //Log("++3");
            //Log("files: "+Serializer.JsonSerialize(files.Select(a => a.Name)));
            //Log("yfiles: " + Serializer.JsonSerialize(y_files.Select(a => a.Name)));
            if (y_files.Count > 1)
            {
                var temp = y_files.Where(a => isYFiscalNull(a));
                //y_files.RemoveAll(a => isYFiscalNull(a));
                //FileInfo y_file_right = y_files.OrderBy(x => x.Name).ThenByDescending(x => x.Length).First();
                //y_files.Remove(y_file_right);
                files.RemoveAll(x => temp.Contains(x));
            }
            //Log("++4");
            return files;
        }

        private bool isYFiscalNull(FileInfo yfile)
        {
            if (yfile == null) return false;
            try
            {
                var info = getYFileContent(yfile);
                if (double.Parse(info[2]) == 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log(ex,String.Format("isYFiscalNull error yfileName={0} yfilecontent='{1}'",yfile.Name,File.ReadAllText(yfile.FullName)));
                return true;
            }
        }

        private string[] getYFileContent(FileInfo yfile)
        {
            string text = File.ReadAllText(yfile.FullName);
            string regg = StaticConstants.RBINNER_CONFIG.GetProperty<string>("y_file_content"
                , @"(?s)(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(.+)");
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

        public void LogKkmFiles_BeforeExchane(string outPath)
        {

        }
        public void LogKkmFiles_BeforeExchane(DirectoryInfo outPath)
        {

        }
    }
    
}
