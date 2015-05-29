using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBClientUpdateApplication
{
    public class UpdateItem
    {
        private t_Teremok a;

        public string Teremok_1c_name;
        public string Teremok_name;
        public RestoranControl RestControl;
        public UpdateItemControl UpdateItemControl;

        public UpdateItem(t_Teremok a)
        {
            // TODO: Complete member initialization
            this.a = a;

            Teremok_1c_name = a.teremok_1C;
            Teremok_name = a.teremok_name;

            RestControl = new RestoranControl()
            {
                 ControlHeader=a.teremok_name,
            };
            RestControl.textBox1.Text = a.teremok_address;
        }
    }
}
