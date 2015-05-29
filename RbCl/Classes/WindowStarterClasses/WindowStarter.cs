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
using RBClient.Classes.CustomWindows;
using RBClient.WinForms.Documents;
using Wpf = System.Windows.Controls;

namespace RBClient.Classes
{
    public partial class WindowStarter
    {
        #region открываем смена и обеды

        public static void Открыть_смена_и_обеды_дата(DateTime dt)
        {
            ОткрытьДокумент(dt, 37);
        }

        public static void Открыть_смена_и_обеды(int _doc_id)
        {
            ОткрытьДокумент(_doc_id, 37);
        }
        #endregion

        #region открываем планирование смен

        public static void Открыть_планирование_смен_дата(DateTime dt)
        {
            ОткрытьДокумент(dt, 28);
        }

        public static void Открыть_планирование_смен(int _doc_id)
        {
            ОткрытьДокумент(_doc_id, 28);
        }
        #endregion

        #region видео реестр
        public static void ОткрытьВидеоРеестр(Form parent)
        {
            FormDBReestr _form = new FormDBReestr();
            _form.MdiParent = parent;
            _form.Show();
        }
        #endregion

        #region открыть инкассацию
        public static void ОткрытьИнкассацию(DateTime dt)
        {
            ОткрытьДокумент(dt, 19);
        }
        #endregion

        #region открыть возврат
        internal static void ОткрытьВозврат(DateTime dt)
        {
            ОткрытьДокумент(dt, 16);
        }
        #endregion

        public static void ОткрытьДокумент(DateTime dt,int docType)
        {
            Type view = GetCurrentDocView(docType);
            if (Проверить_открыт_ли_документ_ранее_и_активировать(view)) return;
            t_Doc tabel_doc = StaticConstants.CBData.GetDocumentByDate(docType,dt);
            ОткрытьДокумент(tabel_doc,docType);
        }

        public static void ОткрытьДокумент(int _doc_id, int docType)
        {
            t_Doc tabel_doc = new t_Doc().SelectFirst<t_Doc>("doc_id=" + _doc_id);
            if (tabel_doc == null)
            {
                string doc_name = GetCurrentDocName(docType);
                WpfCustomMessageBox.Show("Документ \""+doc_name+"\" №" + _doc_id + " не найден!!!");
            }
            else
            {
                ОткрытьДокумент(tabel_doc, docType);
            }
        }

        private static void ОткрытьДокумент(t_Doc tabel_doc, int docType)
        {            
            if (tabel_doc == null)
            {
                CreateAndOpenNewDoc(docType);
            }
            else
            {
                OpenExistedDoc(tabel_doc);
            }
        }

        private static void CreateAndOpenNewDoc(int doc_type)
        {
            DateTime dt=DateTime.Now;
            
            t_Doc tabel_new_tdoc = StaticConstants.CBData.CreateDocumentInDb(doc_type, dt);

            OrderClass new_tabel = OrderClass.CreateOrderClass(tabel_new_tdoc);
            OpenDoc(new_tabel, false);
        }


        //переопределяются для разных документов

        private static void OpenDoc(OrderClass order, bool isDisabled)
        {
            TimerProgressWorker pwrkr = new TimerProgressWorker(StaticConstants.MainWindow
                , "11", "Подождите, идет загрузка документа " + order.OrderDescription + "...");
            pwrkr.Start();

            order.CreateDetails(true);
            order.IsDocumentBlocked = isDisabled;
            if (!isDisabled)
            {
                order.Document_SetAsNew();
            }
            
            object form = Activator.CreateInstance(GetCurrentDocView(order.CurrentDocument.doc_type_id), (object)order);
            StaticHelperClass.SetClassItemValue(form, "Text", String.Format("№{0} Документ \"{1}\"",order.Id,order.Header));
            StaticHelperClass.SetClassItemValue(form, "MdiParent", StaticConstants.MainWindow);
            StaticHelperClass.SetClassItemValue(form, "Dock", DockStyle.Fill);
            StaticHelperClass.SetClassItemValue(form, "IsDocumentBlocked", isDisabled);
            StaticHelperClass.ExecuteMethodByName(form, "Show", null);

            pwrkr.Stop();
        }

        public static void OpenExistedDoc(t_Doc tabel_doc)
        {
            OrderClass new_tabel = OrderClass.CreateOrderClass(tabel_doc);
            OpenDoc(new_tabel, !IsEnabledCondition(new_tabel));
        }

        
    }
}
