using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using RBClient.Classes;
using CustomLogger;
using System.IO;
using System.Threading;

namespace WebService1
{
    /// <summary>
    /// Сводное описание для Service1
    /// </summary>
    [WebService(Namespace = "url:armservice")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Чтобы разрешить вызывать веб-службу из скрипта с помощью ASP.NET AJAX, раскомментируйте следующую строку. 
    // [System.Web.Script.Services.ScriptService]
    public partial class Service1 : System.Web.Services.WebService
    {
        internal static Logger logger = new Logger();
        internal static Logger servicelogger = new Logger("Classes.txt");

        #region trash
        [WebMethod]
        public string HelloWorld()
        {
            HelperClass hc = new HelperClass();
            var rep = hc.GetReport(new DateTime(2015, 2, 12, 0, 0, 0), 11);
            return "Hello World";
        }
        [WebMethod]
        public String Mywm(string fn, string ln)
        {
            return String.Format("dsfsdf={0} dsfdsf={1}", fn, ln);
        }

        #endregion

        public void Log(string message)
        {
            
                logger.Log(message);
            
        }
        
        public void Log(Exception ex, string message)
        {
           
                logger.Log(ex, message);
            
        }

        public Service1()
        {
            //servicelogger.Log("Service1 class crearted "+DateTime.Now.ToString());
        }

        #region rename_host_functions
        [WebMethod]
        public String UpdateKKmHostName(int kkm_num, string kkm_hostname,string kkm_suffix)
        {
            RBMEntities rbm = null;
            try
            {
                rbm = new RBMEntities();
                IEnumerable<t_kkm> kkms = rbm.t_kkm.Where(a => a.kkm_id == kkm_num);
                if (kkms.NotNullOrEmpty())
                {
                    t_kkm kkm = kkms.First();
                    kkm.domain_name = kkm_hostname;
                    kkm.domain_main_suffix = kkm_suffix;
                    kkm.date_updated = DateTime.Now;

                    rbm.SaveChanges();
                }
                else
                {
                    Log("There is no such kkm " + kkm_num + Serializer.JsonSerialize(new object[3] { kkm_num, kkm_hostname, kkm_suffix }));
                    return "There is no such kkm " + kkm_num;
                }
                return "1";
            }
            finally
            {
                CloseDB(rbm);
            }
        }
        #endregion

        #region notifications
        [WebMethod]
        public String SendNotification(string _header, string _message, int _type)
        {
            RBMEntities rbm = null;
            try
            {
                t_WebInfo tw = new t_WebInfo();
                tw.header = _header;
                tw.message = _message;
                tw.type = _type;
                tw.date = DateTime.Now;
                tw.ip = HttpContext.Current.Request.UserHostAddress;

                rbm = new RBMEntities();
                rbm.AddTot_WebInfo(tw);
                rbm.SaveChanges();
                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                CloseDB(rbm);
            }
        }
        #endregion

        #region z_environment_functions

        /// <summary>
        /// Отправить информацию о работе кассы в течении дня
        /// </summary>
        /// <param name="terem_1c">теремок</param>
        /// <param name="kkm_name">номер кассы</param>
        /// <param name="date">текущая дата</param>
        /// <param name="worked">торговала ли касса</param>
        /// <param name="lasttime_online">последнее время когда касса была в сети</param>
        /// <returns></returns>
        [WebMethod]
        public string SendKKmZInfo(string terem_1c, string kkm_name, DateTime date,bool worked,DateTime lasttime_online)
        {
            RBMEntities rbm = null;
            try
            {
                if(String.IsNullOrEmpty(terem_1c)||String.IsNullOrEmpty(kkm_name))
                {
                    return "1";
                }

                rbm = new RBMEntities();
                bool isOnline = Касса_онлайн(lasttime_online);

                List<t_Web_kkm_z_info> twkzi=new List<t_Web_kkm_z_info>();
                if (Есть_ли_запись_о_кассе_сегодня_без_z_отчета
                    (ref twkzi, rbm, terem_1c, kkm_name, date))
                {
                    twkzi.ForEach(a =>
                    {
                        if(worked)  a.worked = worked;
                        a.is_online = isOnline;
                        a.lasttime_online = lasttime_online;
                        a.date_recieved = DateTime.Now;
                    });
                }
                else
                {
                        if (Касса_в_сети(lasttime_online))
                        {
                            t_Web_kkm_z_info twkz = new t_Web_kkm_z_info()
                            {
                                terem_1c = terem_1c,
                                num_kkm = kkm_name,
                                datetime = date,
                                worked = worked,
                                is_online=isOnline,
                                lasttime_online = lasttime_online,
                                date_recieved = DateTime.Now
                            };
                            
                            rbm.AddTot_Web_kkm_z_info(twkz);
                        }
                        else
                        {
                            //выход
                        }
                }
                
                rbm.SaveChanges();
               // Log("SendKKmZInfo good" + Serializer.JsonSerialize(new object[5] { terem_1c, kkm_name, date, worked, lasttime_online }));
                return "1";
            }
            catch (Exception ex)
            {
                string message = "error " + ex.Message;

                if (ex.InnerException != null)
                {
                    message += " inner error " + ex.InnerException.Message;
                }
                Log("SendKKmZInfo error:" + message + " object: " + Serializer.JsonSerialize(new object[5] { terem_1c, kkm_name, date, worked, lasttime_online }));
                //Log("SendKKmZInfo error" +Serializer.JsonSerialize(new object[5] { terem_1c,  kkm_name,  date, worked, lasttime_online}));
                return message;
            }
            finally
            {
                CloseDB(rbm);
            }
        }

        /// <summary>
        /// Отправить информацию о работе кассы в течении дня + номер смены
        /// </summary>
        /// <param name="terem_1c">теремок</param>
        /// <param name="kkm_name">номер кассы</param>
        /// <param name="date">текущая дата</param>
        /// <param name="worked">торговала ли касса</param>
        /// <param name="lasttime_online">последнее время когда касса была в сети</param>
        /// <returns></returns>
        [WebMethod]
        public string SendKKmZInfoSM(string terem_1c, string kkm_name, DateTime date, bool worked, DateTime lasttime_online,int shift_num)
        {
            RBMEntities rbm = null;
            try
            {
                if (String.IsNullOrEmpty(terem_1c) || String.IsNullOrEmpty(kkm_name))
                {
                    return "1";
                }

                rbm = new RBMEntities();
                bool isOnline = Касса_онлайн(lasttime_online);

                List<t_Web_kkm_z_info> twkzi = new List<t_Web_kkm_z_info>();
                if (Есть_ли_запись_о_кассе_сегодня_без_z_отчета_по_данной_смене
                    (ref twkzi, rbm, terem_1c, kkm_name, date, shift_num))
                {
                    twkzi.ForEach(a =>
                    {
                        if (worked)
                        {
                            a.worked = worked;
                        }
                        a.is_online = isOnline;
                        a.lasttime_online = lasttime_online;
                        a.date_recieved = DateTime.Now;
                    });
                }
                else
                {
                    if (Есть_ли_запись_о_кассе_сегодня_без_z_отчета(ref twkzi, rbm, terem_1c, kkm_name, date))
                    {
                        twkzi = twkzi.Where(a => a.shift_num == null).ToList();
                        if (twkzi.NotNullOrEmpty())
                        {
                            twkzi.ForEach(a =>
                            {
                                if (worked)
                                {
                                    a.worked = worked;
                                }
                                a.is_online = isOnline;
                                a.lasttime_online = lasttime_online;
                                a.date_recieved = DateTime.Now;
                                a.shift_num = shift_num;
                            });
                        }
                        else
                        {
                            t_Web_kkm_z_info twkz = Create_t_Web_kkm_z_info(terem_1c, kkm_name, date, worked, lasttime_online, shift_num, isOnline);
                            rbm.AddTot_Web_kkm_z_info(twkz);
                        }
                    }
                    else
                    {
                        if (Касса_в_сети(lasttime_online))
                        {
                            t_Web_kkm_z_info twkz = Create_t_Web_kkm_z_info(terem_1c, kkm_name, date, worked, lasttime_online, shift_num, isOnline);
                            rbm.AddTot_Web_kkm_z_info(twkz);
                        }
                        else
                        {
                            //выход
                        }
                    }
                }

                rbm.SaveChanges();
                // Log("SendKKmZInfo good" + Serializer.JsonSerialize(new object[5] { terem_1c, kkm_name, date, worked, lasttime_online }));
                return "1";
            }
            catch (Exception ex)
            {
                string message = "error " + ex.Message;

                if (ex.InnerException != null)
                {
                    message += " inner error " + ex.InnerException.Message;
                }
                Log("SendKKmZInfo error:" + message + " object: " + Serializer.JsonSerialize(new object[5] { terem_1c, kkm_name, date, worked, lasttime_online }));
                //Log("SendKKmZInfo error" +Serializer.JsonSerialize(new object[5] { terem_1c,  kkm_name,  date, worked, lasttime_online}));
                return message;
            }
            finally
            {
                CloseDB(rbm);
            }
        }

        private t_Web_kkm_z_info Create_t_Web_kkm_z_info(string terem_1c, string kkm_name, DateTime date, bool worked, DateTime lasttime_online, int shift_num, bool isOnline)
        {
            t_Web_kkm_z_info twkz = new t_Web_kkm_z_info()
            {
                terem_1c = terem_1c,
                num_kkm = kkm_name,
                datetime = date,
                worked = worked,
                is_online = isOnline,
                lasttime_online = lasttime_online,
                date_recieved = DateTime.Now,
                shift_num = shift_num
            };
            return twkz;
        }

        /// <summary>
        /// Уведомление о поступлении z-отчета с кассы
        /// </summary>
        /// <param name="terem_1c">теремок</param>
        /// <param name="kkm_name">номер кассы</param>
        /// <param name="date">текущая дата</param>
        /// <param name="zfile">имя файла Z-отчета</param>
        /// <param name="zdate">дата z-отчета</param>
        /// <returns></returns>
        [WebMethod]
        public string SendKKmZInfoZ(string terem_1c, string kkm_name, DateTime date, string zfile, DateTime zdate)
        {
            RBMEntities rbm = null;
            try
            {
                rbm = new RBMEntities();

                t_Web_kkm_z_info twkzi = null;
                if (Есть_ли_уже_информация_по_z_отчету_сегодня(ref twkzi, rbm, terem_1c, kkm_name,
                    date, zfile))
                {
                    twkzi.zdate = zdate;
                    twkzi.date_recieved = DateTime.Now;
                }
                else
                {
                    twkzi = Найти_запись_о_работе_кассы_сегодня_без_z_отчета(rbm, terem_1c, kkm_name, zdate);
                    if (twkzi == null)
                    {
                        twkzi = new t_Web_kkm_z_info()
                        {
                            terem_1c = terem_1c,
                            num_kkm = kkm_name,
                            datetime = date,
                            zfile = zfile,
                            zdate = zdate,
                            date_recieved = DateTime.Now
                        };
                        rbm.AddTot_Web_kkm_z_info(twkzi);
                    }
                    else
                    {
                        twkzi.zfile = zfile; twkzi.zdate = zdate; twkzi.date_recieved = DateTime.Now;
                    }
                }

                //if (date.Date == zdate.Date)
                //{
                //    twkzi.worked = true;
                //}
                
                rbm.SaveChanges();
                //Log("SendKKmZInfoZ good" + Serializer.JsonSerialize(new object[4] { terem_1c, kkm_name, zfile, zdate.ToString() }));
                return "1";
            }
            catch (Exception ex)
            {
                string message = "error " + ex.Message;

                if (ex.InnerException != null)
                {
                    message += " inner error " + ex.InnerException.Message;
                }
                Log("SendKKmZInfoZ error:" + message + " object: " + Serializer.JsonSerialize(new object[5] { terem_1c, kkm_name, date, zfile, zdate }));
                return message;
            }
            finally
            {
                CloseDB(rbm);
            }
        }

        /// <summary>
        /// Уведомление о поступлении z-отчета с кассы + номер смены
        /// </summary>
        /// <param name="terem_1c">теремок</param>
        /// <param name="kkm_name">номер кассы</param>
        /// <param name="date">текущая дата</param>
        /// <param name="zfile">имя файла Z-отчета</param>
        /// <param name="zdate">дата z-отчета</param>
        /// <returns></returns>
        [WebMethod]
        public string SendKKmZInfoZSM(string terem_1c, string kkm_name, DateTime date, string zfile, DateTime zdate, int shift_num)
        {
            RBMEntities rbm = null;
            try
            {
                rbm = new RBMEntities();
                

                t_Web_kkm_z_info twkzi = null;
                if (Есть_ли_уже_информация_по_z_отчету_сегодня(ref twkzi, rbm, terem_1c, kkm_name,
                    date, zfile))
                {
                    twkzi.zdate = zdate;
                    twkzi.date_recieved = DateTime.Now;
                }
                else
                {
                    twkzi = Найти_запись_о_работе_кассы_сегодня_без_z_отчета_со_сменой(rbm, terem_1c, kkm_name, zdate,shift_num);
                    if (twkzi == null)
                    {
                        twkzi = new t_Web_kkm_z_info()
                        {
                            terem_1c = terem_1c,
                            num_kkm = kkm_name,
                            datetime = date,
                            zfile = zfile,
                            zdate = zdate,
                            shift_num=shift_num,
                            date_recieved = DateTime.Now
                        };
                        rbm.AddTot_Web_kkm_z_info(twkzi);
                    }
                    else
                    {
                        twkzi.zfile = zfile; twkzi.zdate = zdate; twkzi.date_recieved = DateTime.Now;
                    }
                }

                rbm.SaveChanges();
                //Log("SendKKmZInfoZ good" + Serializer.JsonSerialize(new object[4] { terem_1c, kkm_name, zfile, zdate.ToString() }));
                return "1";
            }
            catch (Exception ex)
            {
                string message = "error " + ex.Message;

                if (ex.InnerException != null)
                {
                    message += " inner error " + ex.InnerException.Message;
                }
                Log("SendKKmZInfoZ error:" + message + " object: " + Serializer.JsonSerialize(new object[5] { terem_1c, kkm_name, date, zfile, zdate }));
                return message;
            }
            finally
            {
                CloseDB(rbm);
            }
        }

        /// <summary>
        /// Подтверждение что z-отчет был отправлен
        /// </summary>
        /// <param name="terem_1c">теремок</param>
        /// <param name="kkm_name">номер кассы</param>
        /// <param name="date">текущая дата</param>
        /// <param name="zfile">имя файла z-отчета</param>
        /// <returns></returns>
        [WebMethod]
        public string ConfirmZreportSended(string terem_1c, string kkm_name, DateTime date, string zfile,bool sended)
        {
            RBMEntities rbm = null;
            try
            {
                rbm = new RBMEntities();
                t_Web_kkm_z_info twkzi = null;

                if (Есть_ли_уже_информация_по_z_отчету(ref twkzi, rbm, terem_1c, kkm_name,zfile))
                {
                    if (twkzi.zsended != null && (bool)twkzi.zsended)
                    {
                        var twkzi1 = new t_Web_kkm_z_info();

                        
                        twkzi1.is_online = twkzi.is_online;
                        twkzi1.num_kkm = twkzi.num_kkm;
                        twkzi1.terem_1c = twkzi.terem_1c;

                        twkzi1.datetime = date;

                        twkzi1.zdate = twkzi.zdate;
                        twkzi1.zfile = twkzi.zfile;
                        twkzi1.zsended = twkzi.zsended;
                        twkzi1.date_recieved = DateTime.Now;
                        

                        rbm.AddTot_Web_kkm_z_info(twkzi1);
                        rbm.SaveChanges();
                    }
                    else
                    {
                        twkzi.zsended = sended;
                        twkzi.date_recieved = DateTime.Now;
                        rbm.SaveChanges();
                    }
                }
                else
                {
                    twkzi = new t_Web_kkm_z_info();
                    twkzi.terem_1c = terem_1c;
                    twkzi.datetime = DateTime.Now;
                    twkzi.num_kkm = kkm_name;    
                    twkzi.zfile = zfile;
                    twkzi.zsended = sended;
                    twkzi.date_recieved = DateTime.Now;

                    rbm.AddTot_Web_kkm_z_info(twkzi);
                    rbm.SaveChanges();
                }

                return "1";
            }
            catch (Exception ex)
            {
                string message = "error " + ex.Message;

                if (ex.InnerException != null)
                {
                    message += " inner error " + ex.InnerException.Message;
                }
                Log("ConfirmZreportSended error:"+message+" object:" + Serializer.JsonSerialize(new object[5] { terem_1c,  kkm_name,  date,  zfile, sended }));
                return message;
            }
            finally
            {
                CloseDB(rbm);
            }
        }

        /// <summary>
        /// Метод получения z-отчета с вебсервиса
        /// </summary>
        /// <param name="terem_1c"></param>
        /// <param name="kkm_name"></param>
        /// <param name="date"></param>
        /// <param name="zfile"></param>
        /// <param name="shift_num"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [WebMethod]
        public string RecieveZReport(string terem_1c, int kkm_num, DateTime zdate, string zfile,int shift_num,byte[] data)
        {
            RBMEntities rbm = null;
            try
            {
                rbm = new RBMEntities();
                
                //rbm.t_Web_ZReport.Where(a=>
                //    a.terem_1c==terem_1c
                //    && a.kkm_num==kkm_num
                //    && a.zdate==zdate
                //    && a.shift_num==shift_num
                //    && a.zfile==zfile
                //    );

                t_Web_ZReport tzrep = new t_Web_ZReport()
                {
                     terem_1c=terem_1c,
                     kkm_num=kkm_num,
                     zdate=zdate,
                     date_recieved=DateTime.Now,
                     zfile=zfile,
                     shift_num=shift_num,
                     zdata=data
                };
                rbm.AddTot_Web_ZReport(tzrep);
                rbm.SaveChanges();

                return "1";
            }
            catch (Exception ex)
            {
                string message = "error " + ex.Message;

                if (ex.InnerException != null)
                {
                    message += " inner error " + ex.InnerException.Message;
                }
                Log("RecieveZReport error:" + message + " object:" + Serializer.JsonSerialize(new object[4] { terem_1c, kkm_num, zdate, zfile }));
                return message;
            }
            finally
            {
                CloseDB(rbm);
            }
        }

       
        /// <summary>
        /// Метод получения списка файлов на кассе
        /// </summary>
        /// <param name="terem_1c"></param>
        /// <param name="kkm_num"></param>
        /// <param name="type"></param>
        /// <param name="folder"></param>
        /// <param name="file_list"></param>
        /// <returns></returns>
        [WebMethod]
        public string RecieveLogFilesList(string terem_1c, int kkm_num, string type, string folder, DateTime check_time, Fileinfo[] kkm_files)
        {
            RBMEntities rbm = null;
            try
            {
                rbm = new RBMEntities();


                var actual_file_list = rbm.t_Web_FilesLog.Where(a =>
                    a.terem_1c.Equals(terem_1c, StringComparison.OrdinalIgnoreCase)
                    && a.kkm_num == kkm_num
                    && a.type == type
                    && a.folder == folder
                    && (a.date_deleted == null)
                    ).ToList();


                var actual_file_list2 = rbm.t_Web_FilesLog.Where(a =>
                    a.terem_1c.Equals(terem_1c, StringComparison.OrdinalIgnoreCase)
                    && a.kkm_num == kkm_num
                    && a.type == type
                    && a.folder == folder
                    && a.date_deleted > check_time
                    ).ToList();
                actual_file_list.AddRange(actual_file_list2);
                



                //добавляем отсуствующие файлы
                foreach (var file in kkm_files)
                {
                    if (actual_file_list.ContainsByProp((a) => a.afile == file.filename && a.length==file.length))
                    {
                        //файл уже есть
                    }
                    else
                    {
                        //файла еще нет
                        t_Web_FilesLog record = Create_t_Web_FilesLog(file);
                        record.terem_1c = terem_1c;
                        record.kkm_num = kkm_num;
                        record.type = type;
                        record.folder = folder;
                        record.date_recieved = check_time;
                        
                        rbm.AddTot_Web_FilesLog(record);
                        
                    }
                }

                //помечаем удаленные
                foreach (var record in actual_file_list)
                {
                    if (!kkm_files.ContainsByProp(a => a.filename == record.afile && a.length == record.length))
                    {
                        //файла нет
                        record.date_deleted = check_time;
                    }
                }
                
                rbm.SaveChanges();
                
                return "1";
            }
            catch (Exception ex)
            {
                string message = "error " + ex.Message;

                if (ex.InnerException != null)
                {
                    message += " inner error " + ex.InnerException.Message;
                }
                Log("RecieveLogFilesList error:" + message + " terem=" + terem_1c+" kkm="+kkm_num);
                return message;
            }
            finally
            {
                CloseDB(rbm);
            }
        }


        #endregion


        [WebMethod]
        public string SetTeremokVersion(string terem_1c, string version)
        {
            RBMEntities rbm = null;
            try
            {
                rbm = new RBMEntities();

                var teremok = rbm.t_Teremok.Where(a =>
                    a.teremok_1C.Equals(terem_1c, StringComparison.OrdinalIgnoreCase)
                    );

                if (teremok.NotNullOrEmpty())
                {
                    teremok.ToList().ForEach(a => a.teremok_address = version);
                    rbm.SaveChanges();
                }
                return "1";
            }
            catch (Exception ex)
            {
                string message = "SetTeremokVersion error " + ex.Message;

                if (ex.InnerException != null)
                {
                    message += " inner error " + ex.InnerException.Message;
                }
                Log("SetTeremokVersion error:" + message + " terem=" + terem_1c);
                return message;
            }
            finally
            {
                CloseDB(rbm);
            }
        }


        public void CloseDB(RBMEntities rbm)
        {
            if (rbm != null)
            {
                rbm.Dispose();
                rbm = null;
            }
        }
       
        public void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        #region test_functions

        [WebMethod]
        public void Test()
        {
            RBMEntities rbm=new RBMEntities();
//            id	terem_1c	num_kkm	zfile	zdate	datetime	worked	lasttime_online	date_recieved	zsended	is_online	shift_num
//671068	UNIVE	42	NULL	NULL	2015-04-03 07:35:56.640	1	2015-04-03 15:41:55.797	2015-04-03 15:42:11.037	NULL	1	780
            var list=new List<t_Web_kkm_z_info>();
            Есть_ли_запись_о_кассе_сегодня_без_z_отчета(ref list, rbm, "kunce", "286", DateTime.Now);

            CloseDB(rbm);
        }

        [WebMethod]
        public string Test_SendKKmZInfoZ(string terem_1c, string kkm_name, DateTime date, string zfile, DateTime zdate)
        {
            
            RBMEntities rbm = null;
            try
            {
                rbm = new RBMEntities();

                t_Web_Test_table twt = new t_Web_Test_table()
                {
                    terem = terem_1c,
                    kkm = kkm_name,
                    datetime = date,
                    zfile = zfile,
                    zdate = zdate,
                    date_recieved = DateTime.Now
                };

                //t_Web_kkm_z_info twkzi = null;
                //if (Есть_ли_уже_информация_по_z_отчету_сегодня(ref twkzi, rbm, terem_1c, kkm_name,
                //    date, zfile))
                //{
                //    twkzi.zdate = zdate;
                //    twkzi.date_recieved = DateTime.Now;
                //}
                //else
                //{
                //    twkzi = Найти_запись_о_работе_кассы_сегодня_без_z_отчета(rbm, terem_1c, kkm_name, zdate);
                //    if (twkzi == null)
                //    {
                //        twkzi = new t_Web_kkm_z_info()
                //        {
                //            terem_1c = terem_1c,
                //            num_kkm = kkm_name,
                //            datetime = date,
                //            zfile = zfile,
                //            zdate = zdate,
                //            date_recieved = DateTime.Now
                //        };
                //        rbm.AddTot_Web_kkm_z_info(twkzi);
                //    }
                //    else
                //    {
                //        twkzi.zfile = zfile; twkzi.zdate = zdate; twkzi.date_recieved = DateTime.Now;
                //    }
                //}

                ////if (date.Date == zdate.Date)
                ////{
                ////    twkzi.worked = true;
                ////}
                rbm.AddTot_Web_Test_table(twt);
                rbm.SaveChanges();
                //Log("SendKKmZInfoZ good" + Serializer.JsonSerialize(new object[4] { terem_1c, kkm_name, zfile, zdate.ToString() }));
                return "1";
            }
            catch (Exception ex)
            {
                string message = "error " + ex.Message;

                if (ex.InnerException != null)
                {
                    message += " inner error " + ex.InnerException.Message;
                }
                Log("SendKKmZInfoZ error:" + message + " object: " + Serializer.JsonSerialize(new object[5] { terem_1c, kkm_name, date, zfile, zdate }));
                return message;
            }
            finally
            {
                CloseDB(rbm);
            }
        }

        [WebMethod]
        public string test__SendKKmZInfoZSMC(string terem_1c, string kkm_name, DateTime date, string zfile, DateTime zdate, int shift_num,int count)
        {
            RBMEntities rbm = null;
            try
            {
                rbm = new RBMEntities();

                t_Web_kkm_z_info twkzi = null;
                if (Есть_ли_уже_информация_по_z_отчету_сегодня(ref twkzi, rbm, terem_1c, kkm_name,
                    date, zfile))
                {
                    twkzi.zdate = zdate;
                    twkzi.date_recieved = DateTime.Now;
                }
                else
                {
                    twkzi = Найти_запись_о_работе_кассы_сегодня_без_z_отчета_со_сменой(rbm, terem_1c, kkm_name, zdate, shift_num);
                    if (twkzi == null)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            t_Web_Test_table twt = new t_Web_Test_table()
                            {
                                terem = terem_1c,
                                kkm = kkm_name,
                                datetime = date,
                                zfile = zfile,
                                zdate = zdate,
                                date_recieved = DateTime.Now
                            };
                            rbm.AddTot_Web_Test_table(twt);
                        }
                    }
                    else
                    {

                    }
                }


                rbm.SaveChanges();
                //Log("SendKKmZInfoZ good" + Serializer.JsonSerialize(new object[4] { terem_1c, kkm_name, zfile, zdate.ToString() }));
                return "1";
            }
            catch (Exception ex)
            {
                string message = "error " + ex.Message;

                if (ex.InnerException != null)
                {
                    message += " inner error " + ex.InnerException.Message;
                }
                Log("SendKKmZInfoZ error:" + message + " object: " + Serializer.JsonSerialize(new object[5] { terem_1c, kkm_name, date, zfile, zdate }));
                return message;
            }
            finally
            {
                CloseDB(rbm);
            }
        }

        [WebMethod]
        public string test__SendKKmZInfoZSM(string terem_1c, string kkm_name, DateTime date, string zfile, DateTime zdate, int shift_num)
        {
            RBMEntities rbm = null;
            try
            {
                rbm = new RBMEntities();

                t_Web_kkm_z_info twkzi = null;
                if (Есть_ли_уже_информация_по_z_отчету_сегодня(ref twkzi, rbm, terem_1c, kkm_name,
                    date, zfile))
                {
                    twkzi.zdate = zdate;
                    twkzi.date_recieved = DateTime.Now;
                }
                else
                {
                    twkzi = Найти_запись_о_работе_кассы_сегодня_без_z_отчета_со_сменой(rbm, terem_1c, kkm_name, zdate, shift_num);
                    if (twkzi == null)
                    {
                        t_Web_Test_table twt = new t_Web_Test_table()
                        {
                            terem = terem_1c,
                            kkm = kkm_name,
                            datetime = date,
                            zfile = zfile,
                            zdate = zdate,
                            date_recieved = DateTime.Now
                        };
                        rbm.AddTot_Web_Test_table(twt);
                    }
                    else
                    {

                    }
                }

                
                rbm.SaveChanges();
                //Log("SendKKmZInfoZ good" + Serializer.JsonSerialize(new object[4] { terem_1c, kkm_name, zfile, zdate.ToString() }));
                return "1";
            }
            catch (Exception ex)
            {
                string message = "error " + ex.Message;

                if (ex.InnerException != null)
                {
                    message += " inner error " + ex.InnerException.Message;
                }
                Log("SendKKmZInfoZ error:" + message + " object: " + Serializer.JsonSerialize(new object[5] { terem_1c, kkm_name, date, zfile, zdate }));
                return message;
            }
            finally
            {
                CloseDB(rbm);
            }
        }
        #endregion

    }
}