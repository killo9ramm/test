using RBClient.Classes.DocumentClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBClient.WinForms.Views
{
    class ReturnForm : FormHost
    {
        public ReturnForm(string Header, System.Windows.Controls.UserControl uc):base(Header,uc)
        {
        }

        public ReturnForm(OrderClass order):base(order)
        {
        }
    }
}
