using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RBClient.Classes.CustomClasses;
using System.Data;
using Config_classes;
using RBClient.Classes.InternalClasses.Models;
using RBClient.ru.teremok.msk;

namespace RBClient.Classes
{
    partial class RbClientGlobalStaticMethods
    {
        public static ModelClass ConvertFromWeb(ShiftType a)
        {
            return new t_ShiftType()
            {
                type_guid = a.Guid,
                type_name = a.Name,
                type_value = a.Value,
                type_color = a.Color,
                deleted=a.Deleted
            };
        }
        public static ModelClass ConvertFromWeb(Employee a)
        {
            return new t_Employee()
            {
                employee_name = a.Name,
                employee_1C = a.Guid,
                employee_FunctionName = a.JobFunctionName,
                employee_FunctionGuid = a.JobFunctionGuid,
                employee_WorkField = a.Working,
                deleted=a.Deleted
            };
        }
        public static ModelClass ConvertFromWeb(Department a)
        {
             return new t_Teremok() { 
                 teremok_id = a.Id, 
                 teremok_1C = a.Code,
                 teremok_name = a.Name, 
                 teremok_dep = a.Type, 
                 teremok_guid = a.Guid,
                 deleted=a.Deleted
             };
        }
        public static ModelClass ConvertFromWeb(Schedule a)
        {
            t_WorkTeremok tw = new t_WorkTeremok()
            {
                teremok_id = CParam.TeremokId,
                teremok_day = a.Date.Day.ToString(),
                teremok_month = a.Date.Month.ToString(),
                teremok_year = a.Date.Year.ToString(),
            };
            if (a.StartTime.Value.Hour != 0)
                tw.teremok_firstTime = a.StartTime.Value;
            else
            {
                DateTime d = a.StartTime.Value;
                tw.teremok_firstTime = new DateTime(d.Year, d.Month, d.Day, 9, 0, 0);
            }
            if (a.EndTime.Value.Hour != 0)
                tw.teremok_lastTime = a.EndTime.Value;
            else
            {
                DateTime d = a.StartTime.Value;
                tw.teremok_lastTime = new DateTime(d.Year, d.Month, d.Day, 22, 0, 0);
            }
            return tw;
        }
        public static ModelClass ConvertFromWeb(ShiftPattern a)
        {
            //t_ButtonTemplate bt = new t_ButtonTemplate();
            //bt.btn_guid = a.Guid;
            //bt.btn_name = a.Name;
            //bt.bnt_value = a.Value.ToString() + ((int)a.Time).ToString() == "0" ? "" : ((int)a.Time).ToString();
            //bt.btn_color = a.Color;
            //bt.btn_hint = ((int)a.Time).ToString();
            //bt.btn_IsUsed = a.IsUsed;
            //bt.btn_Order = a.ButtonOrder;
            //bt.btn_SmenaType = a.ShiftTypeGuid;
            //bt.deleted = a.Deleted;

            return new t_ButtonTemplate()
            {
                btn_guid = a.Guid,
                btn_name = a.Name,
                btn_hint = ((double)a.Time).ToString(),
                bnt_value = a.Value.ToString() + ((double)a.Time == 0 ? "" : ((double)a.Time).ToString()),
                btn_color = a.Color,
                btn_IsUsed = a.IsUsed,
                btn_Order = a.ButtonOrder,
                btn_SmenaType = a.ShiftTypeGuid,
                deleted=a.Deleted
            };
        }
        public static ModelClass ConvertFromWeb(Responsibility a)
        {
            return new t_Responsibility()
            {
                res_guid = a.Guid,
                res_name = a.Name,
                deleted=a.Deleted
            };
        }

        public static List<t_Shifts_Allowed> ConvertListFromWeb(Responsibility a)
        {
            if (!a.AllowedShiftType.NotNullOrEmpty()) return null;
            List<t_Shifts_Allowed> list = list=new List<t_Shifts_Allowed>();

            foreach (var b in a.AllowedShiftType)
            {
                list.Add(new t_Shifts_Allowed() { res_guid = a.Guid, shift_guid = b });
            }
            return list;
        }

        /// <summary>
        /// Вызов функции на вебсервисе
        /// </summary>
        /// <param name="webservice"></param>
        /// <param name="web_func_name"></param>
        /// <param name="param_s"></param>
        /// <returns></returns>
        public static object CallWebFunction(System.Web.Services.Protocols.SoapHttpClientProtocol webservice, string web_func_name, object[] param_s)
        {
            try
            {
                Log(String.Format("Try to call webservice {0} func name is {1}",webservice,web_func_name));
                object results = StaticHelperClass.CallMethod(webservice, web_func_name, param_s);
                return results;
            }
            catch (Exception exp)
            {
                Log(exp, String.Format("Error call webservice {0} func name is {1}",webservice,web_func_name));
                return null;
            }
        }
    }
}
