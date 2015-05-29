using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBClient.WPF.ViewModels;
using System.ComponentModel;
using RBClient.Classes.DocumentClasses;
using RBClient.Classes.InternalClasses.Models;

namespace RBClient.WinForms.ViewModels
{
    [Serializable]
    class InkassViewModelItem : ViewModelItem
    {
        public static InkassViewModelItem Create(OrderClass order, ModelItemClass modelitem)
        {
            if (modelitem.t_table is t_Inkass)
            {
                InkassViewModelItem mi = Createt_Inkass(order, modelitem);
                return mi;
            }

            return null;
        }

        public static InkassViewModelItem Createt_Inkass(OrderClass order, ModelItemClass modelitem)
        {
            try
            {
                if(modelitem.t_table is t_Inkass)
                {
                    InkassViewModelItem mi = Createt_IVMI(order, modelitem);
                    return mi;
                }
                    
                throw new NotImplementedException();
                #region tr
                //t_MarkItems db_line = (t_MarkItems)modelitem.t_table;

                //MarkViewModelItem mi = new MarkViewModelItem();
                //mi.order = order;
                //mi.modelitem = modelitem;

                //if (db_line.mark_name != RBClient.FormMark2.Itog_class.itog_1C)
                //{
                //    mi.Id = db_line.mark_id;
                //    mi._user_fio = ((t_Employee)mi.rht<t_Employee>()[db_line.mark_name]).employee_name;

                //    if (db_line.mark_res != null)
                //    {
                //        if (mi.rht<t_Responsibility>().ContainsKey(db_line.mark_res))
                //            mi._responsibility = ((t_Responsibility)mi.rht<t_Responsibility>()[db_line.mark_res]).res_guid;
                //    }

                //    mi._itog_vert = db_line.mark_total;
                //}
                //else
                //{
                //    mi._user_fio = RBClient.FormMark2.Itog_class.itog_name;
                //}

                //mi.RowId = db_line.mark_row_id;

                //for (int i = 1; i <= DateTime.DaysInMonth(order.CurrentDocument.doc_datetime.Year,
                //    order.CurrentDocument.doc_datetime.Month); i++)
                //{
                //    StaticHelperClass.SetClassItemValue(mi, "__" + i.ToString(),
                //    StaticHelperClass.ReturnClassItemValue(db_line, "mark_" + i.ToString()));
                //}
                //mi.ComputeTotal();
                //return mi;
                #endregion
            }
            catch (Exception ex)
            {
                MDIParentMain.Log("CreateMI error " + ex.Message);
                return null;
            }
        }

        private static InkassViewModelItem Createt_IVMI(OrderClass order, ModelItemClass modelitem)
        {
            try
            {
                t_Inkass db_line = (t_Inkass)modelitem.t_table;
                InkassViewModelItem mi = new InkassViewModelItem();

                mi.modelitem = modelitem;

                mi.Id = db_line.id;
                mi.ink_name = db_line.ink_name;
                mi.sum1 = db_line.sum1;
                mi.sum2 = db_line.sum2;
                mi.sum3 = db_line.sum3;
                mi.ink_type = db_line.ink_type;
                mi.currency = db_line.ink_currency;
                mi.comment = db_line.comment;

                return mi;
            }
            catch(Exception ex)
            {
                MDIParentMain.Log("Createt_IVMI error " + ex.Message);
                return null;
            }
        }

        public int Id
        {
            get;
            set;
        }

        public string ink_name;
        public string Позиция
        {
            get
            {
                return ink_name;
            }
            set
            {
                ink_name = value;
            }
        }

        public string currency;
        public string Валюта
        {
            get
            {
                return currency;
            }
            set
            {
                currency = value;
            }
        }

        private decimal? sum1;
        public decimal Sum1
        {
            get
            {
                return (decimal)sum1;

            }
            set
            {
                try
                {
                    sum1 = value;
                    SetValueToModel("sum1", sum1);
                    OnPropertyChanged("sum1");
                }
                catch (Exception ex)
                {

                }

            }
        }

        private decimal? sum2;
        public decimal Sum2
        {
            get
            {
                return (decimal)sum2;

            }
            set
            {
                try
                {
                    sum2 = value;
                    SetValueToModel("sum2", sum2);
                    OnPropertyChanged("sum2");
                }
                catch (Exception ex)
                {

                }

            }
        }

        private decimal? sum3;
        public decimal Sum3
        {
            get
            {
                return (decimal)sum3;

            }
            set
            {
                try
                {
                    sum3 = value;
                    SetValueToModel("sum3", sum3);
                    OnPropertyChanged("sum3");
                }
                catch (Exception ex)
                {

                }

            }
        }

        public string ink_type;
        public string Ink_type
        {
            get
            {
                return ink_type;
            }
            set
            {
                try
                {
                    ink_type = value;
                    SetValueToModel("ink_type", ink_type);
                    OnPropertyChanged("Ink_type");
                }
                catch (Exception ex)
                {
                    ink_type = "";
                }
            }
        }

        public string comment;
        public string Коментарий
        {
            get
            {
                return comment;
            }
            set
            {
                try
                {
                    comment = value;
                    SetValueToModel("comment", comment);
                    OnPropertyChanged("comment");
                }
                catch (Exception ex)
                {

                }
            }
        }

    }
}
