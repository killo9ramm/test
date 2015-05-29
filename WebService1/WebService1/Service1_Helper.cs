using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using RBClient.Classes;
using CustomLogger;
using System.Reflection;
using System.Diagnostics;

namespace WebService1
{
    
    

    /// <summary>
    /// 
    /// </summary>
    public partial class Service1 : System.Web.Services.WebService
    {
        /// <summary>
        /// Есть ли в базе запись о кассе с отсутствием отчета
        /// </summary>
        /// <param name="rbm"></param>
        /// <param name="terem_1c"></param>
        /// <param name="kkm_name"></param>
        /// <param name="date"></param>
        /// <param name="worked"></param>
        /// <param name="lasttime_online"></param>
        /// <returns></returns>
        private bool Есть_ли_запись_о_кассе_сегодня_без_z_отчета
            (ref List<t_Web_kkm_z_info> twkzi, RBMEntities rbm, string terem_1c, string kkm_name,
            DateTime todaydate)//, string zfile, DateTime? zdate)
        {
            var timer=new ExecutionWatcher();timer.StartChecking();
            DateTime temp=todaydate.AddDays(-2);


            twkzi = rbm.t_Web_kkm_z_info.Where(
                a => a.terem_1c == terem_1c
                && a.num_kkm == kkm_name
                && String.IsNullOrEmpty(a.zfile)
                && a.datetime > temp
                ).ToList();


            twkzi = twkzi.Where(a => a.datetime.Date == todaydate.Date).ToList();

            if (twkzi.NotNullOrEmpty())
            {
             
                return true;
            }
            else
            {
             
                return false;
            }
        }

        /// <summary>
        /// Есть ли в базе запись о кассе с отсутствием отчета
        /// </summary>
        /// <param name="rbm"></param>
        /// <param name="terem_1c"></param>
        /// <param name="kkm_name"></param>
        /// <param name="date"></param>
        /// <param name="worked"></param>
        /// <param name="lasttime_online"></param>
        /// <returns></returns>
        private bool Есть_ли_запись_о_кассе_сегодня_без_z_отчета_по_данной_смене
            (ref List<t_Web_kkm_z_info> twkzi, RBMEntities rbm, string terem_1c, string kkm_name,
            DateTime todaydate,int shift_num)//, string zfile, DateTime? zdate)
        {
            DateTime temp = todaydate.AddDays(-2);

            twkzi = rbm.t_Web_kkm_z_info.Where(
                a => a.terem_1c == terem_1c
                && a.num_kkm == kkm_name
                && String.IsNullOrEmpty(a.zfile)
                && a.datetime > temp
                && a.shift_num==shift_num
                ).ToList();
            twkzi = twkzi.Where(a => a.datetime.Date == todaydate.Date).ToList();
            if (twkzi.NotNullOrEmpty())
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private bool Есть_ли_запись_о_кассе_сегодня
                   (ref List<t_Web_kkm_z_info> twkzi, RBMEntities rbm, string terem_1c, string kkm_name,
                   DateTime todaydate)//, string zfile, DateTime? zdate)
        {
            DateTime temp = todaydate.AddDays(-7);
            twkzi = rbm.t_Web_kkm_z_info.Where(

                a => a.terem_1c == terem_1c
                && a.num_kkm == kkm_name
                && a.datetime>temp

                ).ToList();
            twkzi = twkzi.Where(a => a.datetime.Date == todaydate.Date).ToList();
            if (twkzi.NotNullOrEmpty())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private t_Web_kkm_z_info Найти_запись_о_работе_кассы_сегодня_без_z_отчета
            (RBMEntities rbm, string terem_1c, string kkm_name,
            DateTime todaydate)
        {
            DateTime temp = todaydate.AddDays(-7);
            var twkzi = rbm.t_Web_kkm_z_info.Where(

                a => a.terem_1c == terem_1c
                && a.num_kkm == kkm_name
                && a.datetime >temp
                && String.IsNullOrEmpty(a.zfile));
            var twkzi1 = twkzi.WhereFirst(a => a.datetime.Date == todaydate.Date);
            return twkzi1;
        }

        private t_Web_kkm_z_info Найти_запись_о_работе_кассы_сегодня_без_z_отчета_со_сменой
            (RBMEntities rbm, string terem_1c, string kkm_name,
            DateTime todaydate,int shift_num)
        {
            DateTime temp = todaydate.AddDays(-7);
            var twkzi = rbm.t_Web_kkm_z_info.Where(

                a => a.terem_1c == terem_1c
                && a.num_kkm == kkm_name
                && a.datetime > temp
                && String.IsNullOrEmpty(a.zfile)
                && a.shift_num==shift_num
                );
            var twkzi1 = twkzi.WhereFirst(a => a.datetime.Date == todaydate.Date);
            return twkzi1;
        }

        private bool Касса_в_сети(DateTime lasttime_online)
        {
            if (DateTime.Now.Date.Equals(lasttime_online.Date))
            {
                return true;
            }
            return false;
        }

        private bool Касса_онлайн(DateTime lasttime_online)
        {
            var temp=DateTime.Now;
            if (temp.Date.Equals(lasttime_online.Date))
            {
                if (lasttime_online.Hour == temp.Hour)
                {
                    if(System.Math.Abs(temp.Minute-lasttime_online.Minute)<30)
                    return true;

                    return false;
                }
                return false;
            }
            return false;
        }

        private bool Есть_ли_уже_информация_по_z_отчету_сегодня(ref t_Web_kkm_z_info twkzi, 
            RBMEntities rbm, string terem_1c, string kkm_name,DateTime todaydate,string zfile)
        {
            DateTime temp = todaydate.AddDays(-2);

            var twkzi1 = rbm.t_Web_kkm_z_info.Where(a =>
                   a.terem_1c == terem_1c && a.num_kkm == kkm_name 
                   && a.datetime > temp
                   && a.zfile == zfile);
            twkzi = twkzi1.WhereFirst(a => a.datetime.Date == todaydate.Date);

            if (twkzi == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        private bool Есть_ли_уже_информация_по_z_отчету(ref t_Web_kkm_z_info twkzi, RBMEntities rbm, string terem_1c, string kkm_name, string zfile)
        {
            twkzi = rbm.t_Web_kkm_z_info.WhereFirst(a =>

                   a.terem_1c == terem_1c 
                   && a.num_kkm == kkm_name 
                   && a.zfile == zfile);

            if (twkzi == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public T CloneObject<T>(T obj)
        {
            T objectt = (T)ExecuteMethodByName(obj, "MemberwiseClone",
                            BindingFlags.Instance | BindingFlags.NonPublic, null);
            return objectt;
        }
        public object ExecuteMethodByName(object o, string method_name, BindingFlags flags, object[] param_s)
        {
            try
            {
                Type type = o.GetType();
                List<MethodInfo> mi_list = type.GetMethods(flags).ToList();
                MethodInfo method = null;

                foreach (MethodInfo mi in mi_list)
                {
                    if (mi.Name == method_name)
                    {
                        method = mi; break;
                    }
                }
                if (method != null)
                {
                    return method.Invoke(o, param_s);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private t_Web_FilesLog Create_t_Web_FilesLog(Fileinfo file)
        {
            t_Web_FilesLog tr = new t_Web_FilesLog();
            tr.afile = file.filename;
            if (file.fileInfo!=null && file.fileInfo.Length > 0)
            {
                tr.@object = file.fileInfo;
            }
            tr.length = file.length;

            return tr;
        }


    }
}