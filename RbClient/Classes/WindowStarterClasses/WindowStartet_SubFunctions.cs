using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RBClient.Classes.WindowMarochOtch;
using RBClient.WPF.ViewModels;
using RBClient.Classes.InternalClasses.Models;
using RBClient.WPF.UserControls.Documents;
using RBClient.WPF.UserControls.Documents.Components;
using RBClient.Classes.DocumentClasses;
using RBClient.Classes.WindowsProgress;
using System.Diagnostics;
using RBClient.Classes.CustomClasses;
using System.Windows.Threading;
using System.Threading;
using RBClient.ru.teremok.msk;
using RBClient.WinForms.Views;
using RBClient.WinForms.ViewModels;

namespace RBClient.Classes
{
    public partial class WindowStarter
    {
        #region табель
        public static void PlanSmenOpen()
        {
            //проверить есть ли табель за данный месяц

            StaticConstants.Tabel_Opened_Date = DateTime.Now;


            //проверка условия открытия табеля
            //!проверка даты следующего и текущего месяца

            t_Doc tabel_doc = StaticConstants.CBData.GetDocumentByMonthFirst(28,
                StaticConstants.Tabel_Opened_Date.Month);

            if (tabel_doc == null)
            {
                CreateAndOpenNewTabel();
            }
            else
            {
                OpenExistedTabel(tabel_doc);
            }
        }

        public static bool Проверить_открыт_ли_табель_ранее_и_активировать()
        {
            return Проверить_открыт_ли_документ_ранее_и_активировать<TabelView1>();
        }

        public static void OpenExistedTabel(t_Doc tabel_doc)
        {

            //получить документ из вебсервиса
            PlanDocument tabel_web_plan = null;


//#if(DEB)
//            OrderClass new_tabel = OrderClass.CreateOrderClass(tabel_doc);

//            OpenTabel(new_tabel, false);
//#else
            //получить документ с вебсервиса
            tabel_web_plan = GetTabelFromWebService(tabel_doc.doc_guid);

            //проверить если отличается то обновить
            if (tabel_web_plan!=null && CheckTabelIsUpdatedIn1C(tabel_web_plan, tabel_doc))
            {
                UpdateTabelInDB(tabel_web_plan, tabel_doc);
            }
            OrderClass new_tabel = OrderClass.CreateOrderClass(tabel_doc);

            //!!проверка заблокировать ли табель
            
            OpenTabel(new_tabel,!IsEnabledCondition(new_tabel));
//#endif
        }


        /// <summary>
        /// Открываем табель
        /// </summary>
        /// <param name="tabel"></param>
        public static void OpenTabel(OrderClass tabel)
        {
            tabel.CreateDetails(true);

            TabelView1 tw1 = new TabelView1(tabel);
            tw1.MdiParent=StaticConstants.MainWindow;
            tw1.Dock = DockStyle.Fill;
            tw1.Show();
        }

        /// <summary>
        /// Открываем табель
        /// </summary>
        /// <param name="tabel"></param>
        public static void OpenTabel(OrderClass tabel, bool isDisabled)
        {
            tabel.CreateDetails(true);

            TabelView1 tw1 = new TabelView1(tabel);
            tw1.MdiParent = StaticConstants.MainWindow;
            tw1.Dock = DockStyle.Fill;

            tw1.IsDocumentTabelBlocked = isDisabled;
            tw1.Show();
        }

        public static OrderClass CreateTabelOrderClass(DateTime date)
        {
            t_Doc new_doc = new t_Doc()
            {
                doc_teremok_id = StaticConstants.Teremok_ID_int,
                doc_type_id = 28,
                doc_guid = Guid.NewGuid().ToString(),
                doc_datetime = StaticConstants.Tabel_Opened_Date,
                doc_status_id = StaticConstants.CBData.GetStatusID(28, DocStatusType.New)
            };
            new_doc.Create();
            OrderClass tabel_order = OrderClass.CreateOrderClass(new_doc.doc_id);
            return tabel_order;
        }

        /// <summary>
        /// Создает и открывает новый табель
        /// При создании учитывает StaticConstants.Tabel_Opened_Date
        /// </summary>
        public static void CreateAndOpenNewTabel()
        {
            t_Doc tabel_new_tdoc = StaticConstants.CBData._CreateTDoc
                      (28, StaticConstants.Teremok_ID_int, Guid.NewGuid().ToString(),
                      StaticConstants.Tabel_Opened_Date, DocStatusType.New);

            OrderClass new_tabel = OrderClass.CreateOrderClass(tabel_new_tdoc);

            OpenTabel(new_tabel,false);
        }

        public static PlanDocument GetTabelFromWebService(string doc_guid)
        {
            ARMWeb systemService = StaticConstants.WebService;
            t_PropValue updationFlag = StaticConstants.CBData.
                _GetWebServiceLastExchangeFlag("GetDocument", StaticConstants.Teremok_ID_int);
            int uflag = int.Parse(updationFlag.prop_value);
            PlanDocument[] plan = systemService.GetDocument(Convert.ToInt32(CParam.TeremokId), ref uflag);
            updationFlag.prop_value = uflag.ToString();
            updationFlag.Update();
            return plan.WhereFirst(a => a.ID == doc_guid);
        }

        public static bool CheckTabelIsUpdatedIn1C(PlanDocument tabel_web_plan, t_Doc tabel_doc)
        {
            string hash = StaticConstants.CBData._GetTabelHash(tabel_web_plan);
            return tabel_doc.doc_hash != hash;
        }

        public static void UpdateTabelInDB(PlanDocument tabel_web_plan, t_Doc tabel_doc)
        {
            int docID = tabel_doc.doc_id;
            StaticConstants.CBData.DeleteMark(docID);
            int countGetDocument = 0;

            if (tabel_web_plan.ArrayWorkEmployee.NotNullOrEmpty())
            {
                if (tabel_web_plan.ArrayWorkEmployee.NotNullOrEmpty())
                {
                    foreach (var a in tabel_web_plan.ArrayWorkEmployee)
                    {
                        //пишем сотрудника
                        StaticConstants.CBData.ImportMarkEmp(a.Name.ToString(), a.Responsibility_Name.ToString(),
                            a.Responsibility.ToString(), countGetDocument, docID);
                        countGetDocument++;
                        DayWork[] dw = a.ArrayDayWork;
                        foreach (var c in dw)
                        {
                            if (c.Value == "" && c.Name == "") continue;
                            StaticConstants.CBData.ImportMarkEmpDetails(c.Number.ToString(),
                                c.SmenaType.ToString(), c.Value.ToString(), c.Name.ToString(),
                                c.FirstTime.Value.ToShortTimeString(), c.LastTime.Value.ToShortTimeString(),
                                a.Name.ToString());
                        }
                    }
                }
            }
        }

        #endregion

        public static bool Проверить_открыт_ли_документ_ранее_и_активировать<T>() where T : Form
        {
            foreach (Form _f in StaticConstants.MainWindow.MdiChildren)
            {
                if (_f is T)
                {
                    _f.Activate();
                    return true;
                }
            }
            return false;
        }
        public static bool Проверить_открыт_ли_документ_ранее_и_активировать(Type T)
        {
            foreach (Form _f in StaticConstants.MainWindow.MdiChildren)
            {
                if (_f.GetType().Equals(T))
                {
                    _f.Activate();
                    return true;
                }
            }
            return false;
        }

        public static bool Проверить_открыт_ли_документ_ранее_и_активировать(t_Doc doc)
        {
            Type T = GetCurrentDocView(doc.doc_type_id);
            return Проверить_открыт_ли_документ_ранее_и_активировать(T);
        }
    }
}
