using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBClient.Classes
{
    partial class ErrorNotifications
    {
        public static void Notificate(object o)
        {
            if (o is String)
            {
                Notificate((string)o);
            }
        }
        public static void Notificate(string o)
        {
            try
            {
                Web_notificate_success(StaticConstants.Main_Teremok_1cName, o);
            }
            catch (Exception ex)
            {
                Log("Notificate error " + ex.Message);
            }
        }

        public static string Notificate(string header,string o)
        {
            try
            {
                return Web_notificate_success(StaticConstants.Teremok_1cName + " " + header, o);
            }
            catch (Exception ex)
            {
                Log("Notificate error " + ex.Message);
                return ex.Message;
            }
        }

        public static string Notificate(string header, string o,int priority)
        {
            try
            {
                return Web_notificate_success(StaticConstants.Teremok_1cName + " " + header, o,priority);
            }
            catch (Exception ex)
            {
                Log("Notificate error " + ex.Message);
                return ex.Message;
            }
        }

        public static string Notificate_error(string header, string o)
        {
            try
            {
                return Web_notificate_error(StaticConstants.Teremok_1cName + " " + header, o);
            }
            catch (Exception ex)
            {
                Log("Notificate error " + ex.Message);
                return ex.Message;
            }
        }

        public static void NotificateKKSInstall(string o)
        {
            try
            {
                Web_notificate_success("KKSInstall", o);
                //SendMail(
                //    StaticConstants.RBINNER_CONFIG.GetProperty<string>("admin_email", "krigel.s@teremok.ru"),
                //    "Автоматическая установка пакетов на кассы. " + StaticConstants.Teremok_Name,
                //    o);
            }
            catch (Exception ex)
            {
                Log("NotificateKKSInstall error " + ex.Message);
            }
        }

        internal static void NotificateAdminPkInstall(string o)
        {
            try
            {
                Web_notificate_success("AdminPkInstall", o);
                //SendMail(
                //    StaticConstants.RBINNER_CONFIG.GetProperty<string>("admin_email", "krigel.s@teremok.ru"),
                //    "Автоматическая установка пакетов на пк администратора. " + StaticConstants.Teremok_Name,
                //    o);
            }
            catch (Exception ex)
            {
                Log("NotificateAdminPkInstall error "+ex.Message);
            }
        }
    }
}
