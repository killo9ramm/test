using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBClient.Classes.InternalClasses.Models;
using RBClient.WinForms.Views;

namespace RBClient.Classes
{
    public partial class WindowStarter
    {
        private static Type GetCurrentDocView(int docType)
        {
            switch (docType)
            {
                case 28:
                    return typeof(TabelView1);
                    break;
                case 19:
                    return typeof(InkassForm);
                    break;
                case 16:
                    return typeof(ReturnForm);
                    break;
                case 37:
                    return typeof(RBClient.Classes.WindowMarochOtch.FormMarochOtch);
                    break;
            }

            return null;
        }

        private static string GetCurrentDocName(t_Doc tabel_doc)
        {
            return StaticConstants.CBData.GetTypeDocName(tabel_doc.doc_type_id);
        }
        public static string GetCurrentDocName(int doc_type_id)
        {
            return StaticConstants.CBData.GetTypeDocName(doc_type_id);
        }
    }
}
