using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using RBClient.WPF.UserControls;
using RBClient.Classes.DocumentClasses;

namespace RBClient.Classes.WindowMarochOtch
{
    public partial class FormMarochOtch : FormHost
    {
        //public event FormClosingEventHandler FormClosingRequested;

        public FormMarochOtch(OrderClass order)
            : base(order)
        {
            ChildControl.IsEnabled = !order.IsDocumentBlocked;
            FormClosingRequested += ((MarOtchCentarPanel)ChildControl).FormBeforeClose;
        }
    }
}
