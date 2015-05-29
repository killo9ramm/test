using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebService1;
using CustomLogger;
using RBClient.Classes;
using RBMModel;
using System.Diagnostics;

namespace ReportClasses
{
    class ReportHelper : LoggerBase
    {
        public string PreparereportString(List<string> items)
        {
            string query = "";
            query = items.Aggregate(query, (a, b) => a + "<td " + style + ">" + b + "</td>");
            return query;
        }
        public string PreparereportStringTh(List<string> items)
        {
            string query = "";
            query = items.Aggregate(query, (a, b) => a + "<th " + style + ">" + b + "</th>");
            return query;
        }

        public string PrepareTable(string content, string style)
        {
            string query = PrepareTag("table",content,style);
                //String.Format("<table " + style + ">{0}</table>", content);
            return query;
        }

        public string PrepareHtml(string content, string style)
        {
            string query = PrepareTag("html", content, style);
            return query;
        }
        public string PrepareTr(string content, string style)
        {
            string query = PrepareTag("tr", content, style);
            return query;
        }

        public string PrepareTag(string tagName, string content, string style)
        {
            string query = String.Format("<{0} " + style + ">{1}</{0}>",tagName, content);
            return query;
        }

        public string PreparereportString(ReportItem item)
        {
            string query = "<td>" + item.teremok.teremok_name + "</td>";
            query += "<td>" + item.Zdata.terem_1c + "</td>";
            query += "<td>" + item.Zdata.num_kkm + "</td>";
            //query+= "<td>" + item.Zdata.worked + "</td>";
            query += "<td>" + item.Zdata.datetime + "</td>";
            query += "<td>" + item.Zdata.lasttime_online + "</td>";
            query += "<td>" + item.Zdata.date_recieved + "</td>";
            return query;
        }

        public string prepareSingleLine(kkm_terem_stat item, t_Web_kkm_z_info stat)
        {
            string html = String.Format("<tr bgcolor=\"{0}\">", stat.worked == true ? "#FE642E" : "#3ADF00");
            List<string> mes_list = new List<string>();
            if (stat.worked==true)
            {
                mes_list = new List<string>() {item.teremok.teremok_name,item.teremok.teremok_1C
                ,item.kassa.kkm_id.ToString(),
                            String.Format("Не получен z-отчет. Последний раз касса была доступна {0}. Продажи по кассе {1}.{2}",
                            stat.lasttime_online,
                            stat.worked==true?"производились":"не производились",
                            ((DateTime)stat.lasttime_online).Date==DateTime.Now.Date
                            ?"Отчет кассовой программой не был выгружен либо ошибка арм. Нужно заново выгрузить отчет."
                            :"На точке не запущен арм или касса не включена."
                            )};
            }
            else
            {
                mes_list = new List<string>() {item.teremok.teremok_name,item.teremok.teremok_1C
                ,item.kassa.kkm_id.ToString(),
                            String.Format("Не получен z-отчет. Последний раз касса была доступна {0}. Продажи по кассе {1}.",
                            stat.lasttime_online,
                            stat.worked==true?"производились":"не производились")};
            }
                
            html += PreparereportString(mes_list);
            html += "</tr>";
            return html;
        }

        public string Test()
        {
            return "";
        }

        public string ПодготовитьТаблицуПоОтсутствующимРесторанам(List<kkm_terem_stat> items)
        {
            string html = "Отсутствие ресторанов в рбм";
            if (items.NotNullOrEmpty())
            {
                html += "<tr " + style + "><th " + style + ">Название теремка</th><th>Название теремка в 1С</th><th " + style + ">Номер кассы</th><th " + style + ">Статус кассы</th>";

                foreach (var item in items)
                {
                    if (item.statistics.NotNullOrEmpty())
                    {
                        var stat = item.statistics.Last();
                        html +=String.Format("<tr bgcolor=\"{0}\">", stat.worked == true ? "#FE642E" : "#3ADF00");
                        List<string> mes_list = new List<string>() {"Нет в БД",item.kassa.teremok_1c
                            ,item.kassa.kkm_id.ToString(),
                        String.Format("Последний раз касса была доступна {0}. Продажи по кассе {1}.",
                            stat.lasttime_online,
                           stat.worked==true?"производились":"не производились")};
                        html += PreparereportString(mes_list);
                        html += "</tr>";
                    }
                    else
                    {

                        html += "<tr bgcolor=\"#FACC2E\">";
                        List<string> mes_list = new List<string>() {"Нет в БД",item.kassa.teremok_1c
                            ,item.kassa.kkm_id.ToString(),
                        String.Format("Последний раз касса была доступна {0}. Текущий статус кассы за неделю - {1}.",
                            item.kassa.date_state_updated,
                            item.kassa.isOnline7==true?"работает":"не работает")};
                        html += PreparereportString(mes_list);
                        html += "</tr>";
                    }
                }
                return String.Format("<table  " + style + ">{0}</table>", html);
            }
            return html;
        }

        public string ПодготовитьТаблицуДляОтчета(List<kkm_terem_stat> items,DateTime date)
        {
            string html = "Все отчеты дошли";
            if (items.NotNullOrEmpty())
            {

                html = "<tr " + style + "><th " + style + ">Название теремка</th><th " + style + ">Имя 1С</th><th " + style + ">Номер кассы</th>" +
                    "<th " + style + ">Причина отсутствия отчета</th></tr>";

                foreach (var item in items)
                {
                    
                    //нет связи
                    if (НетСвязиСКассой(item))
                    {

                        html += "<tr " + style + "bgcolor=\"#FACC2E\">";
                        List<string> mes_list = new List<string>() {item.teremok.teremok_name,item.teremok.teremok_1C
                            ,item.kassa.kkm_id.ToString(),
                        String.Format("Нет связи с кассой по сети. Последний раз касса была доступна {0}. "+
                            "",
                            LastKkmAvailable(item))};
                            //,item.kassa.isOnline7==true?"работает":"не работает")};
                        html += PreparereportString(mes_list);
                        html += "</tr>";
                    }

                    //нет z-отчета
                    if (НеПолученZОтчет(item))
                    {
                        if (item.statistics.Count == 1)
                        {
                            html += prepareSingleLine(item, item.statistics[0]);
                        }
                        if (item.statistics.Count > 1)
                        {
                            var ite = item.statistics.Where(a => ((DateTime)a.datetime).Date == date.Date).Last();
                            var ite_last = item.statistics.Last();
                            if (ite == null)
                                html += prepareSingleLine(item, ite_last);
                            else
                                ite.lasttime_online = ite_last.lasttime_online;
                            html += prepareSingleLine(item, ite);
                        }
                    }
                }
                return String.Format("<table "+style+">{0}</table>", html);
            }
            return html;
        }


        public string LastKkmNotWorkedAvailable(kkm_terem_stat item)
        {
            DateTime z_report_date = DateTime.Now;
            List<t_Web_kkm_z_info> result = new List<t_Web_kkm_z_info>();
            DateTime date1 = new DateTime(z_report_date.Year, z_report_date.Month, z_report_date.Day, 0, 0, 0).AddDays(-3);
            DateTime date2 = new DateTime(z_report_date.Year, z_report_date.Month, z_report_date.Day, 23, 59, 59).AddDays(3);

            string kkm_id = item.kassa.kkm_id.ToString();

            HelperClass hc = new HelperClass() {  Rbmbase=new RBMEntities()};
            hc.GetFromBase(() =>
            {
                var temp = from a in hc.Rbmbase.t_Web_kkm_z_info
                           where
                                a.terem_1c.Equals(item.kassa.teremok_1c, StringComparison.OrdinalIgnoreCase) &&
                                a.num_kkm == kkm_id &&
                                a.date_recieved >= date1 &&
                                a.date_recieved <= date2
                           select a;

                if (temp.NotNullOrEmpty())
                {
                    result = temp.ToList();
                }
            });
            hc.Dispose();

            if (result.NotNullOrEmpty())
            {
                result.Sort((b, a) => ((DateTime)b.date_recieved).CompareTo((DateTime)a.date_recieved));
                return result.Last().date_recieved.ToString();
            }
            else
            {
                return LastKkmAvailable(item);
            }
        }

        public string LastKkmAvailable(kkm_terem_stat item)
        {
            RBMEntities rbm=new RBMEntities();
            try
            {
                string kkm_num=item.kassa.kkm_id.ToString();
             
                var temp=DateTime.Now;
                DateTime dt = new DateTime(temp.Year, temp.Month, temp.Day, 0, 0, 0);

                var a = rbm.t_Web_kkm_z_info.Where(
                    b => b.num_kkm == kkm_num &&
                        b.terem_1c.Equals(item.kassa.teremok_1c, StringComparison.OrdinalIgnoreCase) &&
                        b.date_recieved<dt
                        )
                    .Max(b => b.id);
  
                var c = rbm.t_Web_kkm_z_info.Where(b => b.id==a);
                
                if (c.NotNullOrEmpty())
                {
                    return c.ToList().Last().date_recieved.ToString();
                }
                else
                {
                    return item.kassa.date_state_updated.ToString();
                }
            }catch(Exception ex)
            {
                return item.kassa.date_state_updated.ToString();
            }
            finally
            {
                rbm.Dispose();
            }
            
        }
        public const string style = "style=\"border-collapse: collapse;border: 1px solid black;\"";
        public bool НетСвязиСКассой(kkm_terem_stat item)
        {
            return !item.statistics.NotNullOrEmpty();
        }

        public bool НеПолученZОтчет(kkm_terem_stat item)
        {
            var k=item.statistics.Where(a => !String.IsNullOrEmpty(a.zfile));
            return k.Count()==0 && item.statistics.NotNullOrEmpty();
        }
    }
}
