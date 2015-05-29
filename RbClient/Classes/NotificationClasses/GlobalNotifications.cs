using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBClient.Classes.InternalClasses.Models;
using System.Drawing;

namespace RBClient.Classes
{
    class GlobalNotifications
    {
        private static GlobalNotifications _instance=null;
        public static GlobalNotifications Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GlobalNotifications();
                }
                return _instance;
            }
        }

        public GlobalNotifications()
        {
        }

        public void ShowInLabel(string message)
        {
            StaticConstants.UpdateMainInterface(StaticConstants.MainWindow, a =>
            {
                MeasureLabel(message, ((MDIParentMain)a).labelinfo);

                ((MDIParentMain)a).labelinfo.Text = message;
                ((MDIParentMain)a).labelinfo.Visible=true;
                ((MDIParentMain)a).labelinfo.BringToFront();
            });
        }

        private static void MeasureLabel(string message, DevComponents.DotNetBar.LabelX a)
        {
            int wit=200;
            if(message.Length>200)
            {
                wit = 360;
            }

            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                SizeF size = g.MeasureString(message, a.Font, wit);

                a.Width = (int)size.Width;
                a.Height = (int)size.Height + 30;
            }
        }

        public void HideLabel()
        {
            StaticConstants.UpdateMainInterface(StaticConstants.MainWindow, a =>
            {
                ((MDIParentMain)a).labelinfo.Visible = false;
            });
        }

        public List<t_InfoMessage> FindNotificationFilteredByCity(string query)
        {
            IEnumerable<t_InfoMessage> messages=new t_InfoMessage().Select<t_InfoMessage>(query + " AND department='" + CParam.AppCity.ToString()+"'");
            if (messages != null && messages.Count() > 0)
            {
                return messages.ToList();
            }
            else
            {
                messages = new t_InfoMessage().Select<t_InfoMessage>(query + " AND department is NULL");
                if (messages != null && messages.Count() > 0)
                {
                    return messages.ToList();
                }
            }
            return null;
        }

        public t_InfoMessage FindFirstNotificationFilteredByCity(string query)
        {
            List<t_InfoMessage> messages = FindNotificationFilteredByCity(query);
            if (messages != null) return messages.First();
            return null;
        }
    }
}
