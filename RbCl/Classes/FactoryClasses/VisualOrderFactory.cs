using RBClient.Classes.InternalClasses.Models;
using RBClient.WPF.UserControls.Documents;
using RBClient.WPF.UserControls.Documents.Components;
using RBClient.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfControls=System.Windows.Controls;

namespace RBClient.Classes.DocumentClasses
{
    class VisualOrderFactory
    {
        internal static System.Windows.Controls.UserControl ConstructMainView(OrderClass order)
        {
            switch (order.CurrentDocument.doc_type_id)
            {
                case 16:
                    #region возврат
                    {
                        System.Windows.Controls.UserControl mainControl = VisualOrderFactory.ConstructReturnView(order);
                        return mainControl;
                    }
                    #endregion
                    break;
                case 37:
                    #region Смена и обеды
                    {
                        System.Windows.Controls.UserControl mainControl = VisualOrderFactory.ConstructSmenaDinnerView(order);
                        return mainControl;
                    }
                    #endregion
                    break;
                default:
                    break;
            }
            return null;
        }

        private static WpfControls.UserControl ConstructSmenaDinnerView(OrderClass order)
        {
            RBClient.WPF.UserControls.MarOtchCentarPanel wpfctl = new RBClient.WPF.UserControls.MarOtchCentarPanel(order);
            return wpfctl;
        }

        private static System.Windows.Controls.UserControl ConstructReturnView(OrderClass order)
        {
            //собрать документ
            IHeaderControl frmGridHeader = CreateHeaderedHostControl(order);//сделать хедер
            IMainGrid wpdDocGrid = CreatePlainInnerGrid(order);//сделать главный список
            frmGridHeader.AddInnerGrid(wpdDocGrid);//добавить в хедер и прицепить события добавления и поиска номенклатуры
            return frmGridHeader.SelfControl;
        }

        private static IHeaderControl CreateHeaderedHostControl(OrderClass order)
        {
            return new FormGridHeader(order);
        }

        private static IMainGrid CreatePlainInnerGrid(OrderClass orderClass)
        {
            WpdDocGrid wpdDocGrid = new WpdDocGrid(orderClass);
          //  WpfGroupedList wpdDocGrid = new WpfGroupedList(typeof(WpdDocGrid), orderClass); 
            return wpdDocGrid;
        }
    }
}
