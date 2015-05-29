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

namespace RBClient.Classes
{
    public partial class WindowStarter
    {
        public static bool IsEnabledCondition(OrderClass order)
        {
            switch (order.CurrentDocument.doc_type_id)
            {
                case 28:
                    return IsTabelEnabled(order);
                    break;
                case 19:
                    return SimpleEnabledCondition(order);
                    break;
            }

            return SimpleEnabledCondition(order);
            
        }

        public static bool SimpleEnabledCondition(OrderClass order)
        {
            DateTime dt = DateTime.Now;
            if (StaticConstants.CBData._GetDocStatusType(order.CurrentDocument) == 1)
            {
                return true;
            }
            else
            {
                if (dt.Date > order.CurrentDocument.doc_datetime.Date)
                {
                    return false;
                }
                else
                {
                    if (WpfCustomMessageBox.Show(
                        String.Format("Документ \"{0}\" был отправлен. Вы хотите его изменить?",order.OrderDescription)
                        ,"Внимание!", System.Windows.MessageBoxButton.OKCancel) == System.Windows.MessageBoxResult.OK)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            return false;
        }


        public static bool IsTabelEnabled(OrderClass order)
        {
            DateTime dt = DateTime.Now;
            if (dt > order.CurrentDocument.doc_datetime.AddMonths(1))
            {
                return false;
            }

#if(!DEB)
            if (IsTabelBlockedIn1C(order))
                return false;
#endif    
            return true;
        }
        public static bool IsTabelBlockedIn1C(OrderClass order)
        {
            BlokResult _bkr; BlokDoc _bk;
            string _guid = order.CurrentDocument.doc_guid;
            if (_guid == "")
            {
                throw new ArgumentException("Guid документа не может быть пустым", "order.CurrentDocument.doc_guid");
            }
            else
            {
                _bk = new BlokDoc { Date = order.CurrentDocument.doc_datetime, ID = _guid };
                _bkr = StaticConstants.WebService.IsBlocked(Convert.ToInt32(CParam.TeremokId), _bk, true);   //блокировка документа по функции вебсервиса
            }

            if (_bkr.Result == true)
            {
                return false;//табель не заблокирован 1С - блокируем его сами
            }
            if (_bkr.Result == false)
            {
                if (_bkr.Name == null)
                {
                    _bkr.Name = "";
                    return false;
                }
                if (_bkr.Name.IndexOf("web", StringComparison.OrdinalIgnoreCase)!=-1)
                {
                    return false;
                }
                
                    WpfCustomMessageBox.Show("Документ заблокирован в 1С: " + _bkr.Name.ToString() + " \nРедатирование запрещено, октрыть для чтения?\nРедактирование возможно при закрытии документа в 1С, если вам нужно его редактировать, позвоните вашему управляющему.");
                
                return true;
            }
            return true;
        }


    }
}
