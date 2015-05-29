using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RBClient.Classes;
using System.IO;

namespace WebService1
{
    public struct ReportItem
    {
        public t_Web_kkm_z_info Zdata;
        public t_Teremok teremok;
    }
    public class HelperClass
    {
        public List<ReportItem> GetReport(DateTime dt,int check_hour)
        {
            var date1 = new DateTime(dt.Year,dt.Month,dt.Day,0,0,0);
            var date2 = new DateTime(dt.Year, dt.Month, dt.Day+1, 11, 0, 0);
            var date3=new DateTime(dt.Year, dt.Month, dt.Day+1, 0, 0, 0);

            var MainList = GetMainList(date1,date2);
            var WorkingRestoList = GetWorkingTeremok("2");

            var Results=new List<t_Web_kkm_z_info>();


            foreach (var terem in WorkingRestoList)
            {
                var a=CheckTeremok_on_Zreport(MainList, terem);
                if (a.NotNullOrEmpty())
                {
                    Results.AddRange(a);
                }
            }

            var FinalResult = CheckPhisicallyLoadedFiles(@"\\backup01\Backup\MSKAPP\RBServer\mskapp\FTP_b4", Results,date1,date2);

            if (FinalResult.NotNullOrEmpty())
            {
                List<ReportItem> ResultList = new List<ReportItem>();
                foreach (var a in FinalResult)
                {
                    var tereM=WorkingRestoList.WhereFirst(b=>b.teremok_1C==b.teremok_1C);
                    var repItem = new ReportItem() { teremok = tereM, Zdata = a };
                    ResultList.Add(repItem);
                }
                return ResultList;
            }

            return null;
        }

        private bool ZFileExist(DirectoryInfo dir, t_Web_kkm_z_info kkm, DateTime date1, DateTime date2)
        {
            try{
            var dirs=dir.GetDirectories(kkm.terem_1c).ToList().First();
            var files = dirs.GetFiles("*.zip",SearchOption.AllDirectories).ToList();
            var zfiles = files.Where(a => a.CreationTime > date1 && a.CreationTime < date2 && a.Name.IndexOf(kkm.num_kkm + ".") != -1).ToList();

            if (zfiles.NotNullOrEmpty())
            {
                return true;
            }
            return false;
            }catch(Exception ex)
            {
                return false;
            }
            return false;
        }

        private List<t_Web_kkm_z_info> CheckPhisicallyLoadedFiles(string Folder, List<t_Web_kkm_z_info> Results, DateTime date1, DateTime date2)
        {
            DirectoryInfo dir = new DirectoryInfo(Folder);
            List<t_Web_kkm_z_info> Result=new List<t_Web_kkm_z_info>();
            foreach (var a in Results)
            {
                if (!ZFileExist(dir,a,date1,date2))
                {
                    Result.Add(a);
                }
            }
            return Result;
        }

        private List<t_Web_kkm_z_info> CheckTeremok_on_Zreport(List<t_Web_kkm_z_info> MainList, t_Teremok terem)
        {
            var Results=new List<t_Web_kkm_z_info>();
            var WorkedKkms = GetWorkingKkms(MainList, terem);
            foreach (var kkm in WorkedKkms)
            {
               var a=CheckKkm_on_Zreport(MainList, kkm);
                if(a!=null) Results.Add(a);
            }
            return Results;
        }

        private t_Web_kkm_z_info CheckKkm_on_Zreport(List<t_Web_kkm_z_info> MainList, t_Web_kkm_z_info kkm)
        {
            var zreportcount = GetKkmZReport(MainList,kkm);
            if (!zreportcount.NotNullOrEmpty())
            {
                return kkm;
            }
            return null;
        }

        private List<t_Web_kkm_z_info> GetKkmZReport(List<t_Web_kkm_z_info> MainList,t_Web_kkm_z_info kkm)
        {
            var zreports = (from a in MainList
                                     where a.num_kkm == kkm.num_kkm && a.zfile.NotNullOrEmpty() && a.terem_1c==kkm.terem_1c
                                     select a).ToList();
            return zreports;
        }

        private List<t_Web_kkm_z_info> GetWorkingKkms(List<t_Web_kkm_z_info> MainLisT, t_Teremok terem)
        {
            var worked_kkms_count = (from a in MainLisT
                                    where a.terem_1c==terem.teremok_1C //select a).ToList(); 
                                                                        && a.worked==true select a).ToList();
            return worked_kkms_count;
        }

        private List<t_Teremok> GetWorkingTeremok(string t_city)
        {
            RBMEntities rbm = new RBMEntities();
            try
            {
                var result = rbm.t_Teremok.Where<t_Teremok>(a => a.teremok_use_ARM && ((bool)a.deleted != true || a.deleted == null) && (a.teremok_address != "" || a.teremok_address != null)
                    && a.teremok_city == t_city).ToList();
                return result;
            }
            finally
            {
                rbm.Dispose();
            }
        }

        private List<t_Web_kkm_z_info> GetMainList(DateTime date1, DateTime date2)
        {
            RBMEntities rbm = new RBMEntities();
            try
            {
                var result = rbm.t_Web_kkm_z_info.Where(a => a.date_recieved > date1 && a.date_recieved < date2).ToList();
                return result;
            }
            finally
            {
                rbm.Dispose();
            }
        }



    }
    public class Fileinfo
    {
        public string filename;
        public long length;
        public byte[] fileInfo;
    }
}