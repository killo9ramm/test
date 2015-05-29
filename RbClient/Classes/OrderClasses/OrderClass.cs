using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBClient.Classes.InternalClasses.Models;
using RBClient.WPF.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections;
using System.Xml.Linq;
using CustomLogger;
using RBClient.Classes.CustomClasses;
using RBClient.Classes.OrderClasses;

namespace RBClient.Classes.DocumentClasses
{
    /// <summary>
    /// Класс обертка для классов модели
    /// </summary>
    [Serializable]
    public class ModelItemClass : LoggerBaseMdi
    {
        /// <summary>
        /// Id записи в таблице
        /// </summary>
        public int Id
        {
            get{
                if (_table != null)
                {
                    return (int)StaticHelperClass.ReturnClassItemValue(_table, 0);
                }
                else { return 0; }
            }
        }

        /// <summary>
        /// Свойство-класс таблицы
        /// </summary>
        private ModelClass _table=null;
        public ModelClass t_table { get { return _table; } }

        /// <summary>
        /// Свойство хэш код, для сравнения и обновления записи
        /// </summary>
        private long _hashCode = -1;
        public long HashCode
        {
            get { return _hashCode; }
            set
            {
                _hashCode = value;
            }
        }

        public override int GetHashCode()
        {
            return (int)_hashCode;
        }

        /// <summary>
        /// Метод переопределения строкового отображения
        /// </summary>
        public override string ToString()
        {
            if (this.t_table is t_OrderRetReason)
            {
                return (this.t_table as t_OrderRetReason).orr_name;
            }
            if (this.t_table is t_Nomenclature)
            {
                return (this.t_table as t_Nomenclature).nome_name;
            }
            if (this.t_table is t_OrderRetReason)
            {
                return (this.t_table as t_OrderRetReason).orr_name;
            }
            if (this.t_table is t_Employee)
            {
                return (this.t_table as t_Employee).employee_name;
            }
            if (this.t_table is t_Responsibility)
            {
                return (this.t_table as t_Responsibility).res_name;
            }

            return base.ToString();
        }

        public string StringName
        {
            get
            {
                if (this.t_table is t_Responsibility)
                {
                    return (this.t_table as t_Responsibility).res_name;
                }

                return this.ToString();
            }
        }
        /// <summary>
        /// проверяем изменились ли данные и если изменились то обновляем в базе
        /// </summary>
        public virtual void DBUpdate(bool removeEmptyEntries)
        {
            if (Id == 0)
            {
                DBCreate();
            }

            if (HashCode != -1)
            {
                long new_hash = StaticHelperClass.GetHashCodeByFields(t_table);
                if (new_hash != -1 && HashCode != new_hash)
                {
                    //t_table.UpdateOle();
                    t_table.Update(removeEmptyEntries);
                    //t_table.Update();
                    _hashCode = new_hash;
                }
            }
        }

        /// <summary>
        /// проверяем изменились ли данные и если изменились то обновляем в базе
        /// </summary>
        public virtual void DBUpdate()
        {
            if (Id == 0)
            {
                DBCreate();
            }

            if (HashCode != -1)
            {
                long new_hash = StaticHelperClass.GetHashCodeByFields(t_table);
                if(new_hash!=-1 && HashCode!=new_hash)
                {
                    //t_table.UpdateOle();
                    //t_table.Update(true);
                    t_table.Update();
                    _hashCode = new_hash;
                }
            }
        }

        /// <summary>
        /// проверяем изменились ли данные и если изменились то обновляем в базе
        /// </summary>
        public virtual void DBUpdate(Action act)
        {
            if (Id == 0)
            {
                DBCreate();
            }

            if (HashCode != -1)
            {
                long new_hash = StaticHelperClass.GetHashCodeByFields(t_table);
                if (new_hash != -1 && HashCode != new_hash)
                {
                    t_table.Update();
                    _hashCode = new_hash;
                    try
                    {
                        act();
                    }catch(Exception ex){
                        MDIParentMain.Log(ex, "Action in DBUpdate error");
                        if (ex.InnerException != null)
                        {
                            MDIParentMain.Log(ex.InnerException, "Action in DBUpdate innererror");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Создает запись в базе данных
        /// </summary>
        public virtual void DBCreate()
        {
            if (Id == 0)
            {
                _table.Create();
                _hashCode = StaticHelperClass.GetHashCodeByProperties(t_table);
            }
        }
        /// <summary>
        /// Удаляем запись из базы
        /// </summary>
        /// <param name="deleteDecendants">Флаг показывает удалять ли связанные таблицы</param>
        public virtual void DBDelete(bool deleteDecendants)
        {
            if (Id != 0)
            {
                if (deleteDecendants)
                {
                    _table.DeleteJoinTablesMass();
                }
                _table.Delete();
                _table = null;
            }
        }
        /// <summary>
        /// Удаляет из базы только текущую запись
        /// </summary>
        public virtual void DBDelete()
        {
            if (Id != 0)
            {
                _table.Delete();
                _table = null;
            }
        }

       

        private ModelItemClass() { }
        public ModelItemClass(ModelClass model) { _table = model; _hashCode = StaticHelperClass.GetHashCodeByProperties(t_table); }


        private int clear_parameters_length=100;
        /// <summary>
        /// Очищаем в базе указанный список свойств
        /// </summary>
        /// <param name="list"></param>
        internal void Clear(List<string> list)
        {
            
            int count=(list.Count/clear_parameters_length)+1;
            foreach(var a in Enumerable.Range(0,count))
            {
                var o = list.Where((oo, i) => i >= a * clear_parameters_length 
                    && i < (a + 1) * clear_parameters_length).ToList();
                _table.Clear(o);
            }
        }
    }

    [Serializable]
    public class OrderClass : ModelItemClass
    {
        public enum DocumentStatusTypes { New = 1, Sending = 2, Sended = 3, Recieved = 4, NRecieved = 5 }
        public enum DocumentSendTypes { Ftp = 1, Webservice = 2 }

        private OrderClass(t_Doc CurrentDoc) : base(CurrentDoc) { }
        /// <summary>
        /// Создать класс документа
        /// </summary>
        /// <param name="id">идентификатор документа</param>
        /// <returns></returns>
        public static OrderClass CreateOrderClass(int id)
        {
            t_Doc doc = new t_Doc().SelectFirst<t_Doc>("doc_id=" + id);
            return CreateOrderClass(doc);
        }
        /// <summary>
        /// Создать класс документа
        /// </summary>
        /// <param name="CurrentDoc">Модель документа</param>
        /// <returns></returns>
        public static OrderClass CreateOrderClass(t_Doc CurrentDoc)
        {
            if (CurrentDoc != null)
            {
                OrderClass order = new OrderClass(CurrentDoc);
                t_DocTypeRef doc_type = new t_DocTypeRef().SelectFirst<t_DocTypeRef>("doctype_id=" + CurrentDoc.doc_type_id);
                order._header = doc_type.doctype_name;
                order._sendType = doc_type.sendtype_type;
                order.OrderDescription = String.Format("{0} №{1}",WindowStarter.GetCurrentDocName(CurrentDoc.doc_type_id),CurrentDoc.doc_id);
                return order;
            }
            return null;
        }


        /// <summary>
        /// Модель документа
        /// </summary>
        public t_Doc CurrentDocument
        {
            get
            {
                if (base.t_table is t_Doc)
                    return (t_Doc)base.t_table;
                else return null;
            }
        }

        /// <summary>
        /// Заголовок документа
        /// </summary>
        public string Header
        {
            get { return _header; }
        }
        private string _header = "";

        public string OrderDescription { get; set; }

        /// <summary>
        /// Тип отправки документа
        /// </summary>
        public int? SendType
        {
            get
            {
                return _sendType;
            }
        }
        private int? _sendType = null;

        [System.ComponentModel.DefaultValue(false)]
        public bool AutoCreateView{get;set;}

        /// <summary>
        /// Детали документа
        /// </summary>
        public Dictionary<Type, List<ModelItemClass>> DetailsDictionary;
        public Dictionary<Type, Hashtable> HDetailsDictionary;

        public Comparison<ViewModelItem> ViewSorter=null;
        public List<ViewModelItem> ViewsCollection{get;set;}
        public Dictionary<ViewModelItem, ModelItemClass> ViewsDictionary;

        ObservableCollection<DetailsClass> DetailsList
        {
            get
            {
                return _detailsList;
            }
        }//повесить события добавления элемента

        private ObservableCollection<DetailsClass> _detailsList;

        public event EventHandler UpdateViewSourceEvent;

        public bool IsDocumentBlocked { get; set; } //если документ не текущий

        public void AddViewItem(ViewModelItem vmi, ModelItemClass model)
        {
            ViewsCollection.Add(vmi);
            ViewsDictionary.Add(vmi, model);
        }

        public void RemoveViewItem(ViewModelItem vmi, ModelItemClass model)
        {
            ViewsCollection.Remove(vmi);
            ViewsDictionary.Remove(vmi);
        }

        public void InsertViewItem(ViewModelItem vmi, ModelItemClass model, Func<ModelItemClass,ModelItemClass,bool> comp)
        {
            var present_item = from c in ViewsCollection where comp(c.modelitem, vmi.modelitem) select c;
            if (present_item.NotNullOrEmpty())
            {
                ViewsCollection.Insert(ViewsCollection.IndexOf(present_item.Last()) + 1, vmi);
            }
            else
            {
                ViewsCollection.Add(vmi);
            }

            ViewsDictionary.Add(vmi, model);
        }

        public void SortViews()
        {
            if (ViewSorter != null)
            {
                ViewsCollection.Sort(ViewSorter);
            }
        }

        public void AddViewItem(ViewModelItem vmi, ModelItemClass model, object comparison)
        {
            ViewsCollection.Add(vmi);
            if(comparison is bool)
                if ((bool)comparison)
                {
                    SortViews();
                }
            if (comparison is Comparison<ViewModelItem>)
            {
                ViewsCollection.Sort((Comparison<ViewModelItem>)comparison);
            }
            ViewsDictionary.Add(vmi, model);
        }

        public ViewModelItem AddDetail(ModelItemClass detail,bool CreateInDb)
        {
            if (DetailsDictionary != null)
            {
                if (detail != null && detail.t_table != null)
                {
                    List<ModelItemClass> list = DetailsDictionary[detail.t_table.GetType()];
                    if (list != null)
                    {
                        if (CreateInDb)
                        {
                            detail.t_table.Create();
                        }
                        list.Add(detail);
                    }

                    if (AutoCreateView)
                    {
                        ViewModelItem vmi = OrderFactory.ConstructView(this, detail);
                        AddViewItem(vmi, detail);
                        UpdateViewSource(ViewsCollection);
                        return vmi;
                    }
                }
            }
            return null;
        }

        public void RemoveDetail(ModelItemClass detail, bool RemoveInDb)
        {
            if (DetailsDictionary != null)
            {
                if (detail != null && detail.t_table != null)
                {
                    List<ModelItemClass> list = DetailsDictionary[detail.t_table.GetType()];
                    if (list != null && list.Contains(detail))
                    {
                        if (RemoveInDb)
                        {
                            detail.t_table.Delete();
                        }
                        list.Remove(detail);
                    }
                }
            }
        }

        public void RemoveViewDetail(ViewModelItem detail, bool RemoveInDb)
        {
            if (DetailsDictionary != null)
            {
                ModelItemClass model = detail.modelitem;
                if (detail != null && model.t_table != null)
                {
                    List<ModelItemClass> list = DetailsDictionary[model.t_table.GetType()];
                    if (list != null && list.Contains(model))
                    {
                        RemoveDetail(detail.modelitem,RemoveInDb);
                    }

                    if (AutoCreateView)
                    {
                        RemoveViewItem(detail, model);
                        UpdateViewSource(ViewsCollection);
                    }
                }
            }
        }

        public ViewModelItem AddDetail(ModelItemClass detail)
        {
            if (DetailsDictionary != null)
            {
                if(detail!=null && detail.t_table!=null)
                {
                    List<ModelItemClass> list = DetailsDictionary[detail.t_table.GetType()];
                    if (list != null)
                    {
                        list.Add(detail);
                    }

                    if (AutoCreateView)
                    {
                        ViewModelItem vmi=OrderFactory.ConstructView(this, detail);
                        AddViewItem(vmi, detail);
                        UpdateViewSource(ViewsCollection);
                        return vmi;
                    }
                }
            }
            return null;
        }

        public ViewModelItem InsertDetail(ModelItemClass detail, Func<ModelItemClass, ModelItemClass, bool> comp)
        {
            if (DetailsDictionary != null)
            {
                if (detail != null && detail.t_table != null)
                {
                    List<ModelItemClass> list = DetailsDictionary[detail.t_table.GetType()];
                    
                    var present_item=from c in list where comp(c,detail) select c;
                    
                    if(present_item.NotNullOrEmpty())
                    {
                        list.Insert(list.IndexOf(present_item.Last()) + 1, detail);
                    }else
                    {
                        if (list != null)
                        {
                            list.Add(detail);
                        }
                    }
                    
                    if (AutoCreateView)
                    {
                        ViewModelItem vmi = OrderFactory.ConstructView(this, detail);
                        InsertViewItem(vmi, detail,comp);
                        UpdateViewSource(ViewsCollection);
                        return vmi;
                    }
                }
            }
            return null;
        }

        public void UpdateViewSource(object items_Source)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(items_Source);
            view.Refresh();
        }

        public void CreateDetails(bool createView)
        {
            AutoCreateView=createView;
            OrderFactory.ConstructDetails(this);
        }

        public ModelItemClass CreateOrderLineFromTemplate(ModelClass model)
        {
            ModelItemClass model_item=OrderFactory.ConstructDetail(this, model);
            return model_item;
        }

        public string WriteXml()
        {
            XElement elem=OrderFactory.ConstructXML(this);
            if (elem != null)
            {
                return elem.ToString();
            }
            return "";
        }


        public Hashtable GetHDetailTable<T>() where T : ModelClass
        {
            return HDetailsDictionary[typeof(T)];
        }

        public object GetHDetail<T>(object key) where T : ModelClass
        {
            object result = null;

            new TryAction1((o) => result = HDetailsDictionary[typeof(T)][key], null, (ex) => Log(ex, "GetHDetail error")).Start();

            return result;
        }

        public List<ModelItemClass> GetDetailTable<T>() where T : ModelClass
        {
            List<ModelItemClass> result=null;

            new TryAction1((o) => result = DetailsDictionary[typeof(T)], null, (ex) => Log(ex, "GetDetailTable error")).Start();

            return result;
        }

        internal void Document_SetAsNew()
        {
            StaticConstants.CBData.SetDocStatus(CurrentDocument, (int)DocumentStatusTypes.New);
        }

        internal void Document_SetAsSending()
        {
            StaticConstants.CBData.SetDocStatus(CurrentDocument, (int)DocumentStatusTypes.Sending,true);
        }

        internal void Document_SetAsSended()
        {
            StaticConstants.CBData.SetDocStatus(CurrentDocument, (int)DocumentStatusTypes.Sended,true);
            SerializationHipManager.Instance.UnValidSerializedDocument(this);
        }

        internal void Document_SaveCurrentDocumentState(OrderStateClass orderState)
        {
            if (orderState == null) { return;}

            orderState.order = this;

            if (SendType == (int)DocumentSendTypes.Webservice)
            {
                SerializationHipManager.Instance.SaveToHip(orderState);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal void Document_MarkToSending()
        {
            if (SendType == (int)DocumentSendTypes.Ftp)
            {
                throw new NotImplementedException();
            }
            else
            {
                //webservice task
                //найти сериализованное состояние
                t_SerializationHip docstate = SerializationHipManager.Instance.FindDocState(this);

                //создать задчу на отправку
                t_WebServiceTask wtask = StaticConstants.WebService1cManager.EnqeueTask(docstate);

                Document_SetAsSending();
            }
        }
    }

    public class DetailsClass : ModelItemClass//,IDetailsClass
    {
        private DetailsClass(ModelClass _c) : base(_c) { }
        /// <summary>
        /// Создать класс деталей
        /// </summary>
        /// <param name="id">идентификатор документа</param>
        /// <returns></returns>
        public static DetailsClass CreateDetailsClass(ModelClass detail_model)
        {
            if (detail_model != null)
            {
                DetailsClass cla_ss = new DetailsClass(detail_model);
                return cla_ss;
            }
            return null;
        }

        //public static DetailsClass CreateDetailsClass(ModelClass detail_model,bool autoCreateView)
        //{
        //    if (detail_model != null)
        //    {
        //        DetailsClass cla_ss = new DetailsClass(detail_model);
        //        return cla_ss;
        //    }
        //    return null;
        //}
        ///// <summary>
        ///// Создать класс деталей документа
        ///// </summary>
        ///// <param name="CurrentDoc">Модель документа</param>
        ///// <returns></returns>
        //public static DetailsClass CreateDetailsClass(ModelClass detail_model,Type decendant_model_type,string query)
        //{
        //    if (detail_model != null)
        //    {
        //        DetailsClass cla_ss = new DetailsClass(detail_model);
                
        //        //получить детали
        //        object o = Activator.CreateInstance(decendant_model_type);
        //        object o1 = StaticHelperClass.ExecuteMethodByName(o, "SelectReflection", new object[] { query });
        //        List<ModelClass> models = (o1 as List<object>).OfType<ModelClass>().ToList();
        //        //cla_ss._details = models;

        //        return cla_ss;
        //    }
        //    return null;
        //}

        [System.ComponentModel.DefaultValue(false)]
        public bool AutoCreateView{get;set;}

        private ObservableCollection<DetailsClass> _details;
        public ObservableCollection<DetailsClass> Details { get { return _details;} }

        public virtual string ToString()
        {
            return base.ToString();
        }
    }
}
