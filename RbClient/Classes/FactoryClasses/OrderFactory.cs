using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBClient.Classes.InternalClasses.Models;
using RBClient.WPF.ViewModels;
using RBClient.WinForms.ViewModels;
using System.Collections;
using System.Xml.Linq;
using System.Reflection;

namespace RBClient.Classes.DocumentClasses
{
    class OrderFactory
    {
        public static void ConstructDetails(OrderClass order)
        {
            switch (order.CurrentDocument.doc_type_id)
            {
                case 37:
                    {
                        #region Смена и обеды(укороченный при необходимости дополнить)
                        order.DetailsDictionary = new Dictionary<Type, List<ModelItemClass>>();
                        AddDetail(typeof(t_Marotch), "doc_id=" + order.CurrentDocument.doc_id, order);

                        AddDetail(typeof(t_Employee), "employee_WorkField=-1 AND Deleted=0", order);
                        

                        if (order.AutoCreateView)
                        {
                            order.HDetailsDictionary = new Dictionary<Type, Hashtable>();   
                            AddHDetail<t_ButtonTemplate>("btn_guid", "Deleted=0 AND btn_IsUsed=-1", order);
                            AddHDetail<t_Responsibility>("res_guid", "Deleted=0", order);
                            AddHDetail<t_ShiftType>("type_guid", "Deleted=0", order);
                            AddDetail(typeof(t_Shifts_Allowed), "", order);
                            ConstructViews(order);
                        }
                        #endregion
                    }

                    break;
                case 16:
                    #region возврат
                    {
                        order.DetailsDictionary = new Dictionary<Type, List<ModelItemClass>>();

                        AddDetail(typeof(t_Order2ProdDetails), "opd_doc_id=" + order.CurrentDocument.doc_id, order);

                        order.HDetailsDictionary = new Dictionary<Type, Hashtable>();
                        AddHDetail<t_GroupRef>("group_id", "deleted=0 AND doc_type_id=" + order.CurrentDocument.doc_type_id, order);

                        if (order.AutoCreateView)
                        {

                            Type t1 = typeof(t_Nomenclature);
                            List<ModelItemClass> nomenclatures = new List<ModelItemClass>();
                            List<int> all_nomes2teremok = new t_Nome2Teremok().Select<t_Nome2Teremok>().Where(a => a.n2t_nt_id == order.CurrentDocument.doc_type_id)
                                .Select(a => a.n2t_nome_id).ToList();
                            SelectModels(t1, "").ForEach(a =>
                                {
                                    if (all_nomes2teremok.Contains(((t_Nomenclature)a).nome_id))
                                    {
                                        nomenclatures.Add(new ModelItemClass(a));
                                    }
                                });
                            order.DetailsDictionary.Add(t1, nomenclatures);

                           
                           
                            AddHDetail<t_Nome2Teremok>("n2t_id", "n2t_nt_id=" + order.CurrentDocument.doc_type_id + " AND n2t_teremok_id=" + StaticConstants.Teremok_ID, order);
                            AddHDetail<t_OrderRetReason>("orr_id", "", order);
                            ConstructViews(order);
                        }
                    }
                    #endregion
                    break;
                case 28:
                    {
                        #region табель
                        order.DetailsDictionary = new Dictionary<Type, List<ModelItemClass>>();
                        AddDetail(typeof(t_MarkItems), "mark_doc_id=" + order.CurrentDocument.doc_id, order);
                        if (order.AutoCreateView)
                        {
                            AddDetail(typeof(t_Employee), "employee_WorkField=-1 AND Deleted=0", order);
                            order.HDetailsDictionary = new Dictionary<Type, Hashtable>();
                            AddHDetail<t_MarkItems>("mark_id", "mark_doc_id=" + order.CurrentDocument.doc_id, order);
                            AddHDetail<t_Employee>("employee_1C", "employee_WorkField=-1 AND Deleted=0", order);
                            AddHDetail<t_ButtonTemplate>("btn_guid", "Deleted=0 AND btn_IsUsed=-1", order);
                            AddHDetail<t_Responsibility>("res_guid", "Deleted=0", order);
                            AddHDetail<t_ShiftType>("type_guid", "Deleted=0", order);
                            AddHDetail<t_WorkTeremok>(new List<string>() { "teremok_id",
                            "teremok_day","teremok_month","teremok_year"}, "teremok_id='" + StaticConstants.Teremok_ID + "' AND " +
                                "teremok_month='" + order.CurrentDocument.doc_datetime.Month.ToString() +
                                "' AND  teremok_year='" + order.CurrentDocument.doc_datetime.Year.ToString() + "'", order);
                            AddDetail(typeof(t_Shifts_Allowed), "", order);

                            ConstructViews(order);
                        }
                        #endregion
                    }
                    break;
                case 19:
                    {
                        #region инкассация
                        order.DetailsDictionary = new Dictionary<Type, List<ModelItemClass>>();
                        AddDetail(typeof(t_Inkass), "ink_doc_id=" + order.CurrentDocument.doc_id, order);
                        if (order.AutoCreateView)
                        {
                            order.HDetailsDictionary = new Dictionary<Type, Hashtable>();
                            AddHDetail<t_Inkass_TypeRef>("ri_guid", "deleted=0", order);
                            ConstructViews(order);
                        }
                        #endregion
                    }
                    break;
                default:
                    break;
            }
        }

        public static void AddDetail(Type detail_type,string query,OrderClass order)
        {
            try
            {
                List<ModelItemClass> details = new List<ModelItemClass>();
                SelectModels(detail_type, query)
                    .ForEach(a => details.Add(new ModelItemClass(a)));
                order.DetailsDictionary.Add(detail_type, details);
            }catch(Exception ex)
            {
                MDIParentMain.Log(ex, "Не удалось добавить детали!!! Типа "+detail_type.ToString());
            }
        }
        public static void AddHDetail<T>(string key_property, string query, OrderClass order) where T:ModelClass
        {
            try
            {
                Hashtable details = new Hashtable();
                SelectModels(typeof(T), query)
                    .ForEach(a =>
                    {
                        try
                        {
                            details.Add(StaticHelperClass.ReturnClassItemValue(a, key_property), a);
                        }
                        catch (Exception ex)
                        {
                            MDIParentMain.Log(ex,"Ошибка деталей OrderClass "+query);
                        }
                    }
                        );
                order.HDetailsDictionary.Add(typeof(T), details);
            }
            catch (Exception ex)
            {
                MDIParentMain.Log(ex, "Не удалось добавить детали!!! Типа " + typeof(T).ToString());
            }
        }
        public static void AddHDetail<T>(List<string> key_properties, string query, OrderClass order) where T : ModelClass
        {
            try
            {
                Hashtable details = new Hashtable();
                SelectModels(typeof(T), query)
                    .ForEach(a => 
                        {
                        string key="";
                        key_properties.ForEach(b => key += StaticHelperClass.ReturnClassItemValue(a, b).ToString());
                        details.Add(key, a);
                        });
                order.HDetailsDictionary.Add(typeof(T), details);
            }
            catch (Exception ex)
            {
                MDIParentMain.Log(ex, "Не удалось добавить детали!!! Типа " + typeof(T).ToString());
            }
        }

        public static ModelItemClass ConstructDetail(OrderClass order,ModelClass model)
        {
            ModelItemClass mic = null;
            switch (order.CurrentDocument.doc_type_id)
            {
                case 16:
                    if (model is t_Nome2Teremok)
                    {
                        t_Nome2Teremok nome2ter = model as t_Nome2Teremok;
                        if (nome2ter != null)
                        {
                            t_Order2ProdDetails or2pd = ModelsTransformation.Transform(nome2ter);
                            if (or2pd == null) return null;
                            or2pd.opd_doc_id = order.CurrentDocument.doc_id;
                            mic = new ModelItemClass(or2pd);

                        }
                    }
                    if (model is t_Nomenclature)
                    {
                        t_Nomenclature nomencre = model as t_Nomenclature;
                        if (nomencre != null)
                        {
                            t_Order2ProdDetails or2pd = ModelsTransformation.Transform(
                                                            ModelsTransformation.Transform(nomencre,
                                                                                            order.CurrentDocument.doc_type_id,
                                                                                            order.CurrentDocument.doc_teremok_id));
                            if (or2pd == null) return null;
                            or2pd.opd_doc_id = order.CurrentDocument.doc_id;
                            mic = new ModelItemClass(or2pd);
                        }
                    }
                    break;
                

                case 28:
                    #region
                    if (model is t_Employee)
                    {
                        t_Employee nome2ter = model as t_Employee;
                        if (nome2ter != null)
                        {
                            t_MarkItems or2pd = ModelsTransformation.Transform(nome2ter);
                            if (or2pd == null) return null;
                            or2pd.mark_doc_id = order.CurrentDocument.doc_id;
                            mic = new ModelItemClass(or2pd);
                        }
                    }
#endregion
                    break;

                case 19:
                    #region
                    if (model is t_Inkass)
                    {
                        t_Inkass nome2ter = model as t_Inkass;
                        if (nome2ter != null)
                        {
                            nome2ter.id = 0;
                            nome2ter.istemplate = false;
                            nome2ter.ink_doc_id = order.CurrentDocument.doc_id;
                            mic = new ModelItemClass(nome2ter);
                        }
                    }
#endregion
                    break;
            }

            return mic;
        }

        private static object TempObject = null;

        public static void ConstructViews(OrderClass order)
        {
            switch (order.CurrentDocument.doc_type_id)
            {
                case 16:
                    {
                        __CreateViews(order, typeof(t_Order2ProdDetails));
                    }
                    break;

                case 28:
                    {
                        __CreateViews(order, typeof(t_MarkItems));
                    }
                    break;
                case 19:
                    {
                        __CreateViews(order, typeof(t_Inkass));
                    }
                    break;
                default:
                    break;
            }
        }

        private static void __CreateViews(OrderClass order,Type viewModelType)
        {
                        order.ViewsCollection = new List<ViewModelItem>();
                        order.ViewsDictionary = new Dictionary<ViewModelItem, ModelItemClass>();
                        List<ModelItemClass> view_details = order.DetailsDictionary[viewModelType];
                        view_details.ForEach(a =>
                        {
                            ViewModelItem vmi = ConstructView(order, a);
                            ConstructView(order, a);
                            if (vmi != null)
                            {
                                order.AddViewItem(vmi, a);
                            }
                        });
                        if (order.ViewSorter != null)
                        {
                            order.SortViews();
                        }
        }

        public enum ComparisonEnum{ByName=0};
        public static Comparison<ViewModelItem> ConstructComparison(OrderClass order,ViewModelItem vmi, ComparisonEnum compare_enum)
        {
             switch (order.CurrentDocument.doc_type_id)
            {
                case 16:
                        if (vmi.modelitem.t_table is t_Order2ProdDetails)
                        {
                            return (a, b) => ((OutboxReturnViewModelItem)a).NomenclatureName_bnd
                                .CompareTo(((OutboxReturnViewModelItem)b).NomenclatureName_bnd);
                        }
                    
                    break;
                default:
                    break;
            }
             return null;
        }

        //возможность вызова статического метода Create для ViewModelItem
        //public static ViewModelItem ConstructView(OrderClass order, ModelItemClass model, Type viewModelType)
        //{

        //    object o = StaticHelperClass.ExecuteTMethodByName(viewModelType, "Create", System.Reflection.BindingFlags.Public| System.Reflection.BindingFlags.Static, new object[2] { order, model });
        //    return (ViewModelItem)o;
        //}

        public static ViewModelItem ConstructView(OrderClass order, ModelItemClass model)
        {
            ViewModelItem orvmi=null;
            switch (order.CurrentDocument.doc_type_id)
            {
                case 16:
                    orvmi = OutboxReturnViewModelItem.Create(order, model);
                    break;
                case 28:
                    orvmi = MarkViewModelItem.Create(order, model);
                    break;
                case 19:
                    orvmi = InkassViewModelItem.Create(order, model);
                    break;
            }
            return orvmi;
        }

        public static List<ModelClass> SelectModels(Type t, string query)
        {
            try
            {
                object o = Activator.CreateInstance(t);
                object o1 = StaticHelperClass.ExecuteMethodByName(o, "SelectReflection", new object[] { query });
                List<ModelClass> models = (o1 as List<object>).OfType<ModelClass>().ToList();
                return models;
            }
            catch (Exception ex)
            {
                MDIParentMain.Log(ex, "SelectModels error");
                return null;
            }
        }

        public static decimal ComputeTotal(OrderClass order,ModelItemClass mi, decimal quantity)
        {
            switch (order.CurrentDocument.doc_type_id)
            {
                case 16:
                    if(mi.t_table is t_Order2ProdDetails)return quantity;
                    break;
            }
            return 0;
        }

        internal static System.Xml.Linq.XElement ConstructXML(OrderClass order)
        {
            XElement xml =null;

            switch (order.CurrentDocument.doc_type_id)
            {
                case 19:
                    xml = new XElement("DocumentElement", 
                        new XAttribute("time",
                        StaticConstants.CBData.GetDateInkass(order.CurrentDocument.doc_id)),
                        new XElement("o2p", new XAttribute("time",
                        StaticConstants.CBData.GetDateInkass(order.CurrentDocument.doc_id))),
                        new XElement("o2p", new XAttribute("timecreate",
                        StaticConstants.CBData.GetDateCreate(order.CurrentDocument.doc_id))));
                    
                    foreach (var a in order.DetailsDictionary[typeof(t_Inkass)])
                    {
                        xml.Add(CreateXml(a, new string[] { "id", "ink_name", "istemplate" }));
                    }
                    

                    return xml;
                    break;
            }
            return null;
        }

        private static XElement CreateXml(ModelItemClass model, string[] avoided_props)
        {
            var o = CreateNode(model, avoided_props);
                if (o != null)
                {
                    return o;
                }
            return null;
        }

        private static XElement CreateNode(ModelItemClass model, string[] avoided_props)
        {
            List<FieldInfo> o = StaticHelperClass.ReturnModelFieldsList(model.t_table,avoided_props);
            if (o.NotNullOrEmpty())
            {
                XElement xe = new XElement("o2p");
                foreach (var oooo in o)
                {
                    var ooo=oooo.GetValue(model.t_table);
                    if (ooo!=null)
                    {
                         xe.Add(new XAttribute(oooo.Name,
                        ooo.ToString()));
                    }
                }
                return xe;
            }
            return null;
        }

        public static System.Windows.Controls.UserControl ConstructMainView(OrderClass order)
        {
            System.Windows.Controls.UserControl mainControl = VisualOrderFactory.ConstructMainView(order);
            return mainControl;
        }
    }
}
