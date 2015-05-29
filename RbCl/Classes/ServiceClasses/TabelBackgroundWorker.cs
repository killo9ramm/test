using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using RBClient.ru.teremok.msk;
using System.Net;
using RBClient.Classes.InternalClasses.Models;

namespace RBClient.Classes
{
    public class TabelBackgroundWorker 
    {
        BackgroundWorker backWorker;

        public FormMark2 parent;

        public List<string[]> sortList;  

        public TabelBackgroundWorker()
        {
            if (backWorker != null) backWorker.Dispose();

            backWorker = new BackgroundWorker();
            backWorker.DoWork += Do_Work;
            backWorker.RunWorkerCompleted += work_completed;    
        }

        public void Start()
        {
            backWorker.RunWorkerAsync();
        }
        

        public static TabelBackgroundWorker CurrentWorker=null;

        public static TabelBackgroundWorker CreateWorker(int doc_id, int datess, DateTime date)
        {
            CurrentWorker=new TabelBackgroundWorker();
            CurrentWorker._doc_id = doc_id;
            CurrentWorker.dates = datess;
            CurrentWorker._date = date;
            return CurrentWorker;
        }

        private int dates;
        private int _doc_id;
        private DateTime _date;

        private void Do_Work(object sender, DoWorkEventArgs e)
        {
            //заблокировать кнопки табеля
            lock_buttons();

            send_document_to_webservice();

            StaticConstants.MainGridUpdate();
        }


        private void send_document_to_webservice()
        {
            try
            {
#if(!DEB)
                if (StaticConstants.IsDocumentTabelBlocked != true)
#else
                    if(true)
#endif
                {
                    MainProgressReport.Instance.ReportProgress("Начинаем отправку табеля № " + _doc_id, 0);

                    t_Doc cdoc = new t_Doc().SelectFirst<t_Doc>("doc_id=" + _doc_id);

                    CBData _data = new CBData();
                    ARMWeb systemService = StaticConstants.WebService;
                    
                    BlokDoc _bk = new BlokDoc { Date = StaticConstants.Tabel_Opened_Date, ID = _data.returnDocGuid(_doc_id) };
                    BlokResult _bkr = systemService.IsBlocked(Convert.ToInt32(CParam.TeremokId), _bk, false);

                    WorkEmployee[] arrWe = new WorkEmployee[_data.CountMark(_doc_id)];
                    string docGuid = _data.returnDocGuid(_doc_id);
                    DataTable all = _data.allMark(_doc_id);

                    MainProgressReport.Instance.ReportProgress(25);

                    for (int a = 0; a < sortList.Count; a++)
                    {
                        //if (sortList[a] == null) break;
                        string _name_=((string[])sortList[a])[0];

                        for (int b = 0; b < arrWe.Length; b++)
                        {
                            string _name = all.Rows[b].ItemArray[0].ToString();
                            if (_name_ == _name)
                            {
                                string _resp = all.Rows[b].ItemArray[1].ToString();
                                DayWork[] _allDwrk = myDayArr1(b);

                                WorkEmployee we = new WorkEmployee { Name = _name, Responsibility = _resp, ArrayDayWork = _allDwrk };
                                arrWe[a] = we;
                                break;
                            }
                        }
                    }

                    MainProgressReport.Instance.ReportProgress(50);

                    PlanDocument pd = new PlanDocument { Date = _date, ID = docGuid.ToString(), ArrayWorkEmployee = arrWe };

                    string hash = FormMark2.GetTabelHash(pd);

                    try
                    {
                        MainProgressReport.Instance.ReportProgress(75);
                        string result = "0";

                        if (cdoc.doc_hash == hash)
                        {
                            result = "1";
                        }
                        else
                        {
                            result = systemService.PutDocument(Convert.ToInt32(CParam.TeremokId), pd);
                        }
                        


                        if (result == "1")
                        {
                            t_Doc doc=_data.DocUpdateDocStateNew(_doc_id, 3, "Отправлено ");
                            doc.doc_hash = hash;
                            doc.Update();
                        }
                        StaticConstants.WebServiceExchanger.RemoveTask(StaticConstants.Teremok_ID, _doc_id);

                        MainProgressReport.Instance.ReportProgress("Отправлен табель № " + _doc_id,100);
                    }
                    catch(WebException ex)
                    {
                        StaticConstants.WebServiceExchanger.MakeTask(StaticConstants.Teremok_ID,_doc_id,pd);
                        MDIParentMain.Log(ex," не удалось отправить документ "+_doc_id+" по вебсервису");
                        MainProgressReport.Instance.ReportProgress("Не удалось отправить табель № " + _doc_id, 100);
                    }
                    catch (Exception ex)
                    {
                        StaticConstants.WebServiceExchanger.MakeTask(StaticConstants.Teremok_ID, _doc_id, pd);
                        MDIParentMain.Log(ex, " не удалось отправить документ " + _doc_id + " по вебсервису");
                        MainProgressReport.Instance.ReportProgress("Не удалось отправить табель № " + _doc_id, 100);
                    }
                }
                else
                {
                    MDIParentMain.log.Debug("Документ табеля заблокирован!!");
                }

                //убираем флаг блокировки
                StaticConstants.IsDocumentTabelBlocked = false;
            }
            catch (Exception exp)
            {
                MDIParentMain.Log(exp,"Ошибка выгрузки табеля на вебсервис: ");
                MainProgressReport.Instance.ReportProgress("Не удалось отправить табель № " + _doc_id, 100);
            }
        }

        private void work_completed(object sender,RunWorkerCompletedEventArgs e)
        {
            
            //разблокировать кнопки табелей
            unlock_buttons();
            
        }

        private void lock_buttons()
        {
            StaticConstants.MainWindow.button_Mark.Enabled = false;
            StaticConstants.MainWindow.button_MarkNM.Enabled = false;
        }

        private void unlock_buttons()
        {
            StaticConstants.MainWindow.button_Mark.Enabled = true;
            StaticConstants.MainWindow.button_MarkNM.Enabled = true;
        }

        private DayWork[] myDayArr1(int num)
        {
            CBData db = new CBData();
            DayWork[] myDayArr = new DayWork[dates];
            DataTable all = db.allMark(_doc_id);
            DataTable firstime = db.marktime(_doc_id);
            DataTable lasttime = db.marklasttime(_doc_id);
            DataTable Valuetime = db.valueTime(_doc_id);
            DataTable guidSmena = db.guidSmena(_doc_id);
            string DateTime = db.GetDateCreate(_doc_id);
            DateTime create = Convert.ToDateTime(DateTime);

            for (int i = 0; i < myDayArr.Length; i++)
            {
                {
                    DayWork Work = new DayWork
                    {
                        Number = 1 + i,
                        SmenaType = guidSmena.Rows[num].ItemArray[i].ToString(),
                        Value = Valuetime.Rows[num].ItemArray[i].ToString(),
                        FirstTime = Convert.ToDateTime(create.Year + "-" + AttachZeroToDate(create.Month) + "-" + AttachZeroToDate(1 + i) + " " + ToDate(firstime.Rows[num].ItemArray[i].ToString() + ":00")),
                        LastTime = Convert.ToDateTime(create.Year + "-" + AttachZeroToDate(create.Month) + "-" + AttachZeroToDate(1 + i) + " " + ToDate(lasttime.Rows[num].ItemArray[i].ToString() + ":00"))
                    };
                    myDayArr[i] = Work;
                }
            }
            return myDayArr;
        }

        private string AttachZeroToDate(int c)
        {
            if (c < 10)
                return "0" + c.ToString();
            else
                return c.ToString();
        }

        private string ToDate(string c)
        {
            if (c == "24:00:00")
                return "23:59:00";
            if (c == ":00")
                return "0:00" + c.ToString();
            else
                return c.ToString();
        }
    }
}