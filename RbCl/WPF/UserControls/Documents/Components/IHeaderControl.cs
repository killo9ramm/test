using RBClient.Classes;
using RBClient.Classes.DocumentClasses;
using RBClient.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfControls = System.Windows.Controls;

namespace RBClient.WPF.UserControls.Documents.Components
{
    public interface IHeaderControl : ISelfControl
    {
        void AddInnerGrid(IMainGrid InnerControl);
    }

    public interface IMainGrid : ISelfControl
    {
        void OnAddNomenclatureEvent(ModelItemClass mic);
        void OnSearchNomenclatureEvent(ModelItemClass mic);
        ModelItemClass CurrentItem { get; set; }
    }
    public interface ISelfControl
    {
        WpfControls.UserControl SelfControl { get; set; }
    }
}
