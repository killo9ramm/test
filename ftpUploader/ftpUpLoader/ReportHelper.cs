using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CustomLogger;
using RBClient.Classes.CustomClasses;
using System.Text.RegularExpressions;
using Config_classes;
using Models;

namespace ftpUpLoader
{
    class ReportHelper : LoggerBase
    {
        public  List<FileInfo> GetTReports(List<string> InPathes, List<string> OutPathes, bool deleteOldTReps)
        {
            //получить пути для касс
            List<string> kkmInList = InPathes;
            List<string> kkmOutList = OutPathes;

            List<FileInfo> t_report_list = new List<FileInfo>();

            if (kkmInList == null || kkmOutList == null)
            {
                Log("Нет касс в сети!!!");
                return null;
            }

            //Очистить файлы t-report на кассах
            if (deleteOldTReps)
            {
                kkmOutList.ForEach(a =>
                {
                    DirectoryInfo dir = new DirectoryInfo(a);
                    List<FileInfo> file_pathes = dir.GetFiles("T*", SearchOption.TopDirectoryOnly).ToList();
                    file_pathes.ForEach(b => new CustomAction((o) =>
                    {
                        b.Delete();
                    }
                    , null).Start());
                });

            }

            //закинуть туда 0.dat
            kkmInList.ForEach(a => new CustomAction((o) =>
            { string filename = o.ToString(); File.Create(System.IO.Path.Combine(a, filename)); }
            , "0.dat").Start());

            //дождаться ответа касс
            kkmOutList.ForEach(a => new CustomAction((o) =>
            {
                DirectoryInfo kkm_dir = new DirectoryInfo(a);
                List<FileInfo> file_pathes = kkm_dir.GetFiles("T*", SearchOption.TopDirectoryOnly).ToList();
                if (file_pathes == null || file_pathes.Count == 0) throw new Exception("Еще пока нет Трепорта");
                file_pathes.Sort((c, b) => b.CreationTime.CompareTo(c.CreationTime));

                t_report_list.AddRange(file_pathes);

            }, null) { Timeout = 10000 }.Start());

            if (t_report_list.Count > 0)
                return t_report_list;
            else return null;
        }

        public string GetKkmName(string z_rep_text)
        {
            string result = "";

            Regex reg = new Regex(
                   ConfigClass.GetProperty<string>("kkm_in_treport_reg", @"Касса (\d+)"));
            if (reg.IsMatch(z_rep_text))
            {
                result = reg.Match(z_rep_text).Groups[1].Value.Trim();
            }
            return result;
        }

        public void GetLastCheckInfo(string z_rep_text, ref DateTime last_check_date, ref string last_check_num)
        {
            try
            {
                Regex reg = new Regex(
                          ConfigClass.GetProperty<string>("check_reg", @"(?s)Чек\s*(\S+)\s+(\S+ \S+).*?[+]"));
                if (reg.IsMatch(z_rep_text))
                {
                    Match m = reg.Matches(z_rep_text).OfType<Match>().Last();
                    last_check_num = m.Groups[1].Value;
                    last_check_date = DateTime.Parse(m.Groups[2].Value);
                }
            }
            catch (Exception ex)
            {
                Log(ex, "ошибка в GetKkmName");
            }
        }

        public void CheckKKmWorkedToDay(DateTime ToDay, t_Kkm kkm)
        {
            if (kkm.last_check_datetime.Date == ToDay.Date)
            {
                kkm.workedToDay = true;
            }
            else
            {
                kkm.workedToDay = false;
            }
        }
    }
}
