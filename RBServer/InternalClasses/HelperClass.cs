using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RBClient.Classes;
using System.IO;
using RBMModel;
using System.Collections;
using CustomLogger;

namespace WebService1
{
    public struct ReportItem
    {
        public t_Web_kkm_z_info Zdata;
        public t_Teremok teremok;
    }
    public class HelperClass : LoggerBase,IDisposable
    {
        public void Dispose()
        {
            DisposeBase();
        }
        #region good
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

        private bool ZFileExist(DirectoryInfo dir, string resto,string file_name, DateTime date1, DateTime date2)
        {
            try
            {
                var dirs = dir.GetDirectories(resto).ToList().First();
                var files = dirs.GetFiles("*.*", SearchOption.AllDirectories).ToList();
                var zfiles = files.Where(a => a.CreationTime >=date1 && a.CreationTime <=date2 
                    && a.Name.IndexOf(file_name) != -1).ToList();

                if (zfiles.NotNullOrEmpty())
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
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
                                    where a.terem_1c.Equals(terem.teremok_1C,StringComparison.OrdinalIgnoreCase)// select a).ToList(); 
                                                                        && a.worked==true select a).ToList();
            return worked_kkms_count;
        }

        public List<t_Teremok> GetWorkingTeremok(string t_city)
        {
            RBMEntities rbm = new RBMEntities();
            try
            {
                var result = rbm.t_Teremok.Where<t_Teremok>(a => a.teremok_use_ARM && ((bool)a.deleted != true || a.deleted == null)
                    && (a.teremok_address != "" || a.teremok_address != null)
                    && a.teremok_city == t_city).ToList();
                return result;
            }
            finally
            {
                rbm.Dispose();
            }
        }

        private List<t_Web_kkm_z_info> GetWorkedKkm(DateTime date_start, DateTime date_end)
        {
            RBMEntities rbm = new RBMEntities();
            try
            {
                var result = rbm.t_Web_kkm_z_info.Where(a => a.date_recieved >= date_start && a.date_recieved <= date_end 
                    && a.worked==true).ToList();
                return result;
            }
            finally
            {
                rbm.Dispose();
            }
        }

        private List<t_Web_kkm_z_info> GetZReports(DateTime date_start, DateTime date_end)
        {
            RBMEntities rbm = new RBMEntities();
            try
            {
                var result = rbm.t_Web_kkm_z_info.Where(a => a.date_recieved > date_start && a.date_recieved <= date_end 
                    && !String.IsNullOrEmpty(a.zfile)).ToList();
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

        public List<ReportItem> GetMskReport(DateTime dt, int check_hour, int start_zwait_hour, int resto_open_hour)
        {
            return GetReportOld(dt, check_hour, start_zwait_hour, resto_open_hour, "2", @"\\backup01\Backup\MSKAPP\RBServer\mskapp\FTP_b4");
        }
        public List<kkm_terem_stat> GetSpbReport(DateTime dt, int check_hour, int start_zwait_hour, int resto_open_hour)
        {
            return GetReport(dt, check_hour, start_zwait_hour, resto_open_hour, "1", @"\\backup01\Backup\MSKAPP\RBServer\mskapp\FTP_b4");
        }

        public List<kkm_terem_stat> GetMskReportNew(DateTime dt, int check_hour, int start_zwait_hour, int resto_open_hour)
        {
            return GetReportNew(dt, check_hour, start_zwait_hour, resto_open_hour, "2", @"\\backup01\Backup\MSKAPP\RBServer\mskapp\FTP_b4");
        }

        public List<kkm_terem_stat> GetMskReportNew____Final(DateTime dt, int check_hour, int start_zwait_hour, int resto_open_hour)
        {
            return GetReportNewWeb(dt, check_hour, start_zwait_hour, resto_open_hour, "2", @"\\backup01\Backup\MSKAPP\RBServer\mskapp\FTP_b4",true,typeof(t_ZReport));
        }

        public List<kkm_terem_stat> GetSpbReportNew____Final(DateTime dt, int check_hour, int start_zwait_hour, int resto_open_hour)
        {
            return GetReportNewWeb(dt, check_hour, start_zwait_hour, resto_open_hour, "1", @"\\backup01\Backup\MSKAPP\RBServer\mskapp\FTP_b4", false, typeof(t_Web_ZReport));
        }

        #endregion

        public List<ReportItem> GetReportOld(DateTime dt, int check_hour, int start_zwait_hour, int resto_open_hour, string city, string bakupfolder)
        {
            var date_start_zwait = new DateTime(dt.Year, dt.Month, dt.Day, start_zwait_hour, 0, 0);
            var date_stop_zwait = new DateTime(dt.Year, dt.Month, dt.Day + 1, check_hour, 0, 0);
            var date_start_day_kkm_worked = new DateTime(dt.Year, dt.Month, dt.Day, resto_open_hour, 0, 0);
            var date_stop_day_kkm_worked = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

            var workedkkms = GetWorkedKkm(date_start_day_kkm_worked, date_stop_day_kkm_worked);

            var MainList = GetMainList(date_start_zwait, date_stop_zwait);
            var WorkingRestoList = GetWorkingTeremok(city);

            var Results = new List<t_Web_kkm_z_info>();


            foreach (var terem in WorkingRestoList)
            {
                var a = CheckTeremok_on_Zreport(MainList, terem);
                if (a.NotNullOrEmpty())
                {
                    Results.AddRange(a);
                }
            }
            var FinalResult = Results;
            //CheckPhisicallyLoadedFiles(bakupfolder, Results, date_start_zwait, date_stop_zwait);

            if (FinalResult.NotNullOrEmpty())
            {
                List<ReportItem> ResultList = new List<ReportItem>();
                foreach (var a in FinalResult)
                {
                    var tereM = WorkingRestoList.WhereFirst(b => b.teremok_1C.Equals(a.terem_1c, StringComparison.OrdinalIgnoreCase));
                    var repItem = new ReportItem() { teremok = tereM, Zdata = a };
                    ResultList.Add(repItem);
                }
                return ResultList;
            }

            return null;
        }

        public List<kkm_terem_stat> GetReport(DateTime dt, int check_hour, int start_zwait_hour, int resto_open_hour, string city, string bakupfolder)
        {
            var date_start_zwait = new DateTime(dt.Year, dt.Month, dt.Day, start_zwait_hour, 0, 0);
            var z_report_date = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            var date_stop_zwait = new DateTime(dt.Year, dt.Month, dt.Day, check_hour, 0, 0).AddDays(1);
            var date_start_day_kkm_worked = new DateTime(dt.Year, dt.Month, dt.Day, resto_open_hour, 0, 0);
            var date_stop_day_kkm_worked = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

            DateTime dt_one_week = dt.AddDays(-7);

            var working_teremoks = GetWorkingTeremok(city);

            var getkkmsOverLast7days = GetWorkedKkm7(date_stop_zwait, dt_one_week);

            List<t_kkm> _List = ПолучитьСписокРабочихКасс(dt_one_week, city);
            
            List<kkm_terem_stat> nZ = new List<kkm_terem_stat>();

            ПолучитьСписокКассZОтчетов(_List, ref nZ, z_report_date);
            
            СобратьСтатистику(nZ, z_report_date, date_stop_zwait);

            return nZ;
        }

        public List<kkm_terem_stat> GetReportNewWeb(DateTime dt, int check_hour, int start_zwait_hour, int resto_open_hour,
            string city, string bakupfolder,bool bringInCorrectState, Type zreport_table)
        {
            
            #region date region
            var date_start_zwait = new DateTime(dt.Year, dt.Month, dt.Day, start_zwait_hour, 0, 0);
            var z_report_date = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            var date_stop_zwait = new DateTime(dt.Year, dt.Month, dt.Day, check_hour, 0, 0).AddDays(1);
            var date_start_day_kkm_worked = new DateTime(dt.Year, dt.Month, dt.Day, resto_open_hour, 0, 0);
            var date_stop_day_kkm_worked = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

            DateTime dt_one_week = dt.AddDays(-3);
            #endregion

            var working_teremoks = GetWorkingTeremok(city);


            #region correctness kkm check раскомментировать
            if (bringInCorrectState)
            {
                var getkkmsOverLast7days = GetWorkedKkm7(date_stop_zwait, dt_one_week);
                ПривестиСписокКассВСоответствиеСПолученнымиДанными(getkkmsOverLast7days);
            }
            #endregion

            List<t_kkm> _List = ПолучитьСписокРабочихКасс(dt_one_week, city);

            List<kkm_terem_stat> reportItems = new List<kkm_terem_stat>();

            foreach (var kkm in _List)
            {
                //проверить работала ли касса
                if (IsKkmWorked(kkm, z_report_date))
                {
                    //касса работала
                    //получить все смены
                    List<t_Web_kkm_z_info> smenas = TakeKkmShifts(kkm, z_report_date);

                    //проверить все ли смены закрыты
                    List<t_Web_kkm_z_info> opened_smenas = CheckIfSmenasClosed(smenas, zreport_table);

                    if (opened_smenas.NotNullOrEmpty())
                    {
                        foreach (var osmena in opened_smenas)
                        {
                            kkm_terem_stat kts = new kkm_terem_stat(kkm, osmena);
                            reportItems.Add(kts);
                        }
                    }
                }
                else
                {
                    //касса не работала
                    kkm_terem_stat err_kkm = new kkm_terem_stat(kkm);
                    reportItems.Add(err_kkm);
                }
            }
            СобратьСтатистику(reportItems, z_report_date, date_stop_zwait);
            return reportItems;
        }

        private List<t_Web_kkm_z_info> CheckIfSmenasClosed(List<t_Web_kkm_z_info> smenas, Type zreport_table)
        {
            List<t_Web_kkm_z_info> result = new List<t_Web_kkm_z_info>();
            foreach (var smena in smenas)
            {
                if (!IsSmenaClosed(smena,zreport_table))
                {
                    result.Add(smena);
                }
            }
            return result;
        }

        private bool IsSmenaClosed(t_Web_kkm_z_info smena, Type zreport_table)
        {
            bool flag = false;
            if (zreport_table == typeof(t_ZReport))
            {
                flag=CheckSmenaClosedIn_T_Zreport(smena);
                
            }
            else
            {
                flag = CheckSmenaClosedIn_T_ZreportWeb(smena);
            }

            return flag;
        }

        private bool CheckSmenaClosedIn_T_Zreport(t_Web_kkm_z_info smena)
        {
            bool result = false;
            GetFromBase(() =>
            {
                DateTime date1 = new DateTime(smena.datetime.Value.Year, smena.datetime.Value.Month, smena.datetime.Value.Day, 0, 0, 0).AddDays(-1);
                DateTime date2 = date1.AddDays(1);
                var temp = from a in Rbmbase.t_ZReport
                           where
                                a.t_Teremok.teremok_1C.Equals(smena.terem_1c, StringComparison.OrdinalIgnoreCase) &&
                                a.z_kkm_id == smena.num_kkm &&
                                a.z_date >= date1 && 
                                a.z_date <= date2
                           select a;

                var temp1 = temp.ToList().Where(a => ZReport.GetShiftNum(a.z_file) == smena.shift_num);

                if (temp1.NotNullOrEmpty())
                {
                    result = true;
                }
            });
            return result;
        }

        private bool CheckSmenaClosedIn_T_ZreportWeb(t_Web_kkm_z_info smena)
        {
            bool result = false;
            GetFromBase(() =>
            {
                DateTime date1 = new DateTime(smena.datetime.Value.Year, smena.datetime.Value.Month, smena.datetime.Value.Day, 0, 0, 0).AddDays(-1);
                DateTime date2 = new DateTime(smena.datetime.Value.Year, smena.datetime.Value.Month, smena.datetime.Value.Day, 23, 59, 59).AddDays(1);

                int kkm_num = int.Parse(smena.num_kkm);
                var temp = from a in Rbmbase.t_Web_ZReport
                           where
                                a.terem_1c.Equals(smena.terem_1c, StringComparison.OrdinalIgnoreCase) &&
                                a.shift_num==smena.shift_num&&
                                a.kkm_num == kkm_num &&
                                a.zdate>=date1 &&
                                a.zdate<=date2
                           select a;

                if (temp.NotNullOrEmpty())
                {
                    result = true;
                }
            });
            return result;
        }

        private List<t_Web_kkm_z_info> TakeKkmShifts(t_kkm kkm, DateTime z_report_date)
        {
            List<t_Web_kkm_z_info> result = new List<t_Web_kkm_z_info>();
            GetFromBase(() =>
            {
                string kkm_id = kkm.kkm_id.ToString();

                DateTime date1 = new DateTime(z_report_date.Year, z_report_date.Month, z_report_date.Day, 0, 0, 0);
                DateTime date2 = new DateTime(z_report_date.Year, z_report_date.Month, z_report_date.Day, 23, 59, 59);

                var temp = from a in Rbmbase.t_Web_kkm_z_info
                           where
                                a.terem_1c.Equals(kkm.teremok_1c, StringComparison.OrdinalIgnoreCase) &&

                                a.shift_num != null &&
                                a.lasttime_online!=null &&
                                a.num_kkm == kkm_id &&
                                a.datetime >= date1 &&
                                a.datetime <= date2
                           select a;

                if (temp.NotNullOrEmpty())
                {
                    result = temp.ToList();
                }
            });
            return result;
        }

        internal RBMEntities Rbmbase;

        private bool IsKkmWorked(t_kkm kkm, DateTime z_report_date)
        {
            bool IsKkmWorked=false;
            GetFromBase(() =>
            {
                string kkm_id=kkm.kkm_id.ToString();

                DateTime date1=new DateTime(z_report_date.Year,z_report_date.Month,z_report_date.Day,0,0,0);
                DateTime date2=new DateTime(z_report_date.Year,z_report_date.Month,z_report_date.Day,23,59,59);

                var result=Rbmbase.t_Web_kkm_z_info.Where(a => a.terem_1c.Equals(kkm.teremok_1c, StringComparison.OrdinalIgnoreCase) &&
                    a.num_kkm == kkm_id &&
                    a.worked==true &&
                    a.datetime >=date1 &&
                    a.datetime <=date2);
                if (result.NotNullOrEmpty())
                {
                    IsKkmWorked = true;
                }
            });
            return IsKkmWorked;
        }

        public void GetFromBase(Action act)
        {
            CheckBase();
            try
            {
                act();
            }catch(Exception ex)
            {
                Log(ex,"GetFromBase error");
            }
        }

        public void CheckBase()
        {
            if (Rbmbase == null)
            {
                Rbmbase = new RBMEntities();
            }
        }

        public void DisposeBase()
        {
            if (Rbmbase != null)
            {
                Rbmbase.Dispose();
            }
        }

        public List<kkm_terem_stat> GetReportNew(DateTime dt, int check_hour, int start_zwait_hour, int resto_open_hour,
            string city, string bakupfolder)
        {
            var date_start_zwait = new DateTime(dt.Year, dt.Month, dt.Day, start_zwait_hour, 0, 0);
            var z_report_date=new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            var date_stop_zwait = new DateTime(dt.Year, dt.Month, dt.Day, check_hour, 0, 0).AddDays(1);
            var date_start_day_kkm_worked = new DateTime(dt.Year, dt.Month, dt.Day, resto_open_hour, 0, 0);
            var date_stop_day_kkm_worked = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);

            //DateTime dt_one_week = dt.AddDays(-7);
            DateTime dt_one_week = dt.AddDays(-3);

            var working_teremoks = GetWorkingTeremok(city);

            var getkkmsOverLast7days = GetWorkedKkm7(date_stop_zwait, dt_one_week);

            ПривестиСписокКассВСоответствиеСПолученнымиДанными(getkkmsOverLast7days);

            List<t_kkm> _List = ПолучитьСписокРабочихКасс(dt_one_week,city);

            List<kkm_terem_stat> rZ = new List<kkm_terem_stat>();
            List<kkm_terem_stat> nZ = new List<kkm_terem_stat>();

            Получить2СпискаПолученныхНеполученныхZОтчетовВt_Zreport(_List, ref rZ, ref nZ,z_report_date);
            //List<kkm_terem_stat> temp = ПроверитьНаФизическоеНаличиеФайла(rZ, bakupfolder, z_report_date, date_stop_zwait);
            List<kkm_terem_stat> temp = new List<kkm_terem_stat>();

            temp.AddRange(nZ);

            //пока не обновлю арм вычленить уличные точки
            //List<string> ult = new List<string>() { "grval", "", "shepk", "vorob", "yarma", "mayak", "lenin" };комс ярмарка  маяк
            //temp = temp.Where(a => !ult.Contains(a.kassa.teremok_1c)).ToList();
            //end


            СобратьСтатистику(temp, z_report_date, date_stop_zwait);
            return temp;
        }

        public void СобратьСтатистику(List<kkm_terem_stat> temp,DateTime z_report_date,
            DateTime date_stop_zwait)
        {
             RBMEntities rbm = new RBMEntities();
            try
            {
                foreach (var _a in temp)
                {
                    try
                    {
                        string _kkm_string = _a.kassa.kkm_id.ToString();
                        var result = rbm.t_Web_kkm_z_info.Where(a => a.date_recieved >= z_report_date 
                        && a.date_recieved <= date_stop_zwait
                        && a.terem_1c.Trim().Equals(_a.kassa.teremok_1c.Trim(), StringComparison.OrdinalIgnoreCase)
                        && a.num_kkm == _kkm_string).ToList();

                        _a.statistics = result;

                        var web_zreps = result.Where(a=>!String.IsNullOrEmpty(a.zfile)).ToList();
                        _a.z_reports_web = web_zreps;
                    }catch(Exception ex1)
                    {
                        Log(ex1, "error in СобратьСтатистику item "+Serializer.JsonSerialize(_a));
                    }
                }
                
                return;
            }
            catch(Exception ex)
            {
                Log(ex, "error in СобратьСтатистику");
            }
            finally
            {
                rbm.Dispose();
            }
        }
        

        private List<kkm_terem_stat> ПроверитьНаФизическоеНаличиеФайла(List<kkm_terem_stat> rZ, string bakupfolder, DateTime z_report_date,DateTime date2)
        {
            DirectoryInfo dir = new DirectoryInfo(bakupfolder);
            List<kkm_terem_stat> Result = new List<kkm_terem_stat>();

            DateTime dt1 = z_report_date;
            DateTime dt2 = date2;

            foreach (var a in rZ)
            {
                if (a.z_reports.NotNullOrEmpty())
                {
                    bool flag = true;
                    foreach (var z in a.z_reports)
                    {
                        bool _flag=ZFileExist(dir, a.kassa.teremok_1c, z.ZRep.z_file, dt1, dt2);
                        z.exist=_flag;
                        flag = flag && _flag;
                    }

                    if (!flag)
                    {
                        Result.Add(a);
                    }
                }
                else
                {
                    Result.Add(a);
                }
            }
            return Result;
        }       

        public List<t_kkm> ПолучитьСписокРабочихКасс(DateTime dt_one_week, string city)
        {
            RBMEntities rbm = new RBMEntities();
            try
            {
                int _city = int.Parse(city);
                return rbm.t_kkm.Where(a => a.city == _city && (bool)a.isOnline7).ToList();
                //return rbm.t_kkm.Where(a => a.city==_city&&((bool)a.isOnline7 || a.date_state_updated > dt_one_week)).ToList();
            }
            catch (Exception ex)
            {
                Log(ex, "error ПолучитьСписокРабочихКасс");
                return null;
            }
            finally
            {
                rbm.Dispose();
            }
        }

        private List<kkm_terem_stat> ПроверитьНаФизическоеНаличиеФайла(List<kkm_terem_stat> rZ)
        {
            throw new NotImplementedException();
        }

        private void ПолучитьСписокКассZОтчетов(List<t_kkm> _List,
            ref List<kkm_terem_stat> nZ, DateTime check_date)
        {
            RBMEntities rbm = new RBMEntities();
            try
            {
                foreach (var _kkm in _List)
                {
                    t_Teremok terem = rbm.t_Teremok.WhereFirst(a => a.teremok_1C.EqualsR(_kkm.teremok_1c));

                    kkm_terem_stat temp = new kkm_terem_stat()
                    {
                        kassa = _kkm,
                        teremok = terem
                    };
                        nZ.Add(temp);
                }
            }
            catch (Exception ex)
            {
                Log(ex, "error ПолучитьСписокКассZОтчетов");
                return;
            }
            finally
            {
                rbm.Dispose();
            }
        }

        public void Получить2СпискаПолученныхНеполученныхZОтчетовВt_ZreportWeb(List<t_kkm> _List, ref List<kkm_terem_stat> rZ,
            ref List<kkm_terem_stat> nZ, DateTime check_date)
        {
            RBMEntities rbm = new RBMEntities();
            try
            {
                foreach (var _kkm in _List)
                {
                    t_Teremok terem = rbm.t_Teremok.WhereFirst(a => a.teremok_1C.EqualsR(_kkm.teremok_1c));


                    var zreports = rbm.t_Web_ZReport.Where(a => a.terem_1c== _kkm.teremok_1c
                        && a.zdate.Value.Date == check_date.Date && a.kkm_num == _kkm.kkm_id).ToList();

                    kkm_terem_stat temp = new kkm_terem_stat()
                    {
                        kassa = _kkm,
                        teremok = terem,
                        z_reports = zreports.Select(a => new ZReport(a)).ToList()
                    };

                    if (zreports.NotNullOrEmpty())
                    {
                        rZ.Add(temp);
                    }
                    else
                    {
                        nZ.Add(temp);
                    }

                }
            }
            catch (Exception ex)
            {
                Log(ex, "error Получить2СпискаПолученныхНеполученныхZОтчетовВt_Zreport");
                return;
            }
            finally
            {
                rbm.Dispose();
            }
        }

        public void Получить2СпискаПолученныхНеполученныхZОтчетовВt_Zreport(List<t_kkm> _List, ref List<kkm_terem_stat> rZ,
            ref List<kkm_terem_stat> nZ,DateTime check_date)
        {
            
            RBMEntities rbm = new RBMEntities();
            try
            {
                foreach (var _kkm in _List)
                {
                    t_Teremok terem=rbm.t_Teremok.WhereFirst(a=>a.teremok_1C.EqualsR(_kkm.teremok_1c));
                    var zreports=rbm.t_ZReport.Where(a=>a.t_Teremok.teremok_id==terem.teremok_id 
                        && a.z_date==check_date && a.z_kkm ==_kkm.kkm_id).ToList();

                    kkm_terem_stat temp=new kkm_terem_stat(){ kassa=_kkm, teremok=terem,
                        z_reports=zreports.Select(a=>new ZReport(a)).ToList()};

                    if(zreports.NotNullOrEmpty())
                    {
                        rZ.Add(temp);
                    }
                    else
                    {
                        nZ.Add(temp);
                    }

                }
            }
            catch (Exception ex)
            {
                Log(ex, "error Получить2СпискаПолученныхНеполученныхZОтчетовВt_Zreport");
                return;
            }
            finally
            {
                rbm.Dispose();
            }
        }

        public void ПривестиСписокКассВСоответствиеСПолученнымиДанными(List<kkm_resto> kkmsOverLast7days)
        {
            RBMEntities rbm = new RBMEntities();
            try
            {
                DateTime dt=DateTime.Now;
                
                ОбновитьИлиДобавитьНовыеКассы(kkmsOverLast7days, rbm, dt);
                ВывестиКассыКоторыеНеВСети(kkmsOverLast7days, rbm, dt);
            }
            catch (Exception ex)
            {
                return;
            }
            finally
            {
                rbm.Dispose();
            }
        }

        public void ВывестиКассыКоторыеНеВСети(List<kkm_resto> kkmsOverLast7days, RBMEntities rbm, DateTime dt)
        {
            try
            {
                var kkms = rbm.t_kkm.ToList();
                foreach (var kkm_rest in kkms)
                {
                    var temp = kkmsOverLast7days.WhereFirst(a => a.kkm == kkm_rest.kkm_id.ToString()
                        && a.resto.EqualsR(kkm_rest.teremok_1c));

                    if (temp == null && (bool)kkm_rest.isOnline7)
                    {
                        kkm_rest.isOnline7 = false;
                        kkm_rest.date_state_updated = dt;
                    }
                }
                rbm.SaveChanges();
            }
            catch (Exception ex)
            {
                Log(ex, "не удалось ВывестиКассыКоторыеНеВСети");
            }
        }

        public void ОбновитьИлиДобавитьНовыеКассы(List<kkm_resto> kkmsOverLast7days, RBMEntities rbm, DateTime dt)
        {
            try
            {
                var kkms = rbm.t_kkm.ToList();
                
                foreach (var kkm_rest in kkmsOverLast7days)
                {
                    var tkkm = kkms.WhereFirst(a => a.kkm_uid.EqualsR(kkm_rest.kkm + kkm_rest.resto));
                    if (tkkm == null)
                    {
                        t_kkm _kkm = new t_kkm()
                        {
                            kkm_uid = kkm_rest.kkm + kkm_rest.resto,
                            kkm_id = int.Parse(kkm_rest.kkm),
                            teremok_1c = kkm_rest.resto,
                            isOnline7 = true,
                            date_state_updated = dt
                        };

                        t_Teremok ter = rbm.t_Teremok.WhereFirst(a => a.teremok_1C == _kkm.teremok_1c);
                        if (ter != null)
                        {
                            int _city = 0;
                            if (int.TryParse(ter.teremok_city, out _city))
                            {
                                _kkm.city = _city;
                            }else{
                                _kkm.city = 2;
                                }
                        }
                        rbm.AddTot_kkm(_kkm);
                    }
                    else
                    {
                        if (tkkm.isOnline7 == null || !(bool)tkkm.isOnline7)
                        {
                            tkkm.isOnline7 = true;
                            tkkm.date_state_updated = dt;
                        }
                    }
                }
                rbm.SaveChanges();
            }catch(Exception ex)
            {
                Log(ex, "не удалось ОбновитьИлиДобавитьНовыеКассы");
            }
        }

        public List<kkm_resto> GetWorkedKkm7(DateTime dt,DateTime dte)
        {
            RBMEntities rbm = new RBMEntities();
            try
            {
                DateTime date_start = dte;

               

                var res = (from a in rbm.t_Web_kkm_z_info
                           where a.date_recieved >= date_start && a.date_recieved <= dt
                           select new kkm_resto() { kkm = a.num_kkm, resto = a.terem_1c }).ToList();

                res.ForEach(a => a.GetHashCode());
                res=res.Distinct().ToList();
                res.Sort((a,b)=>a.CompareTo(b));

                return res;

            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                rbm.Dispose();
            }
        }

        public class kkm_resto
        {
            public string kkm;
            public string resto;
            public int hashcode=0;
            

            public override bool Equals(object obj)
            {
                if (kkm == ((kkm_resto)obj).kkm && resto == ((kkm_resto)obj).resto)
                    return true;
                return false;
            }
            public int CompareTo(kkm_resto b)
            {
                return (kkm + resto).CompareTo(b.kkm + b.resto);
            }
            public override int GetHashCode()
            {
                if (hashcode == 0)
                {
                    hashcode = (kkm + resto).GetHashCode();
                }
                
                return hashcode;
            }
        }
    }
    [Serializable]
    public class kkm_terem_stat
    {
        public t_Teremok teremok;
        public t_kkm kassa;
        public List<t_Web_kkm_z_info> statistics;
        public List<ZReport> z_reports;
        public List<t_Web_kkm_z_info> z_reports_web;
        
        public t_Web_kkm_z_info osmena;

        public kkm_terem_stat(t_kkm _kkm)
        {
            kassa = _kkm;
            teremok = FindTeremok(_kkm);
        }

        public kkm_terem_stat()
        {
        }

        public kkm_terem_stat(t_kkm kkm, t_Web_kkm_z_info osmena)
        {
            // TODO: Complete member initialization
            this.kassa = kkm;
            this.osmena = osmena;
            teremok = FindTeremok(kkm);
        }

        public t_Teremok FindTeremok(t_kkm _kkm)
        {
            t_Teremok terem=null;
            HelperClass hc = new HelperClass();
            hc.GetFromBase(() =>
            {
                
                var result = hc.Rbmbase.t_Teremok.Where(
                    a => a.teremok_1C.Equals(_kkm.teremok_1c, StringComparison.OrdinalIgnoreCase));
                if (result.NotNullOrEmpty())
                {
                    terem = result.First();
                }

            });
            hc.Dispose();
            return terem;
        }
    }
    [Serializable]
    public class ZReport
    {
        public t_ZReport ZRep;
        public bool exist = false;

        public string terem_1c;
        public int shift_num;
        public DateTime z_date;
        public int kkm_id;
        public t_Web_ZReport a;
        
        

        private ZReport() { }

        internal ZReport(t_ZReport tz) 
        { 
            ZRep = tz;
            terem_1c = tz.t_Teremok.teremok_1C;
            shift_num = GetShiftNum(tz.z_file);
            z_date = tz.z_date;
            kkm_id = (int)tz.z_kkm;
        }

        public ZReport(t_Web_ZReport a)
        {
            this.a = a;
            terem_1c =a.terem_1c;
            shift_num = a.shift_num;
            z_date = a.zdate.Value;
            kkm_id = a.kkm_num;
        }

        internal static int GetShiftNum(string zfile)
        {
            return int.Parse(GetShiftNumStr(zfile));
        }
        internal static string GetShiftNumStr(string zfile)
        {
            string shift = zfile.Substring(zfile.IndexOf(".") + 1);
            return shift;
        }
    }
}