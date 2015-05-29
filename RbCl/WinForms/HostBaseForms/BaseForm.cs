using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RBClient.Classes.DocumentClasses;

namespace RBClient.WinForms
{
    public partial class BaseForm : Form
    {
        public BaseForm()
        {
            InitializeComponent();
        }

        public bool IsDocumentBlocked = false;
        public OrderClass Order;
        //запрещаем перемещение окна
        protected override void WndProc(ref Message message)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;

            switch (message.Msg)
            {
                case WM_SYSCOMMAND:
                    int command = message.WParam.ToInt32() & 0xfff0;
                    if (command == SC_MOVE)
                        return;
                    break;
            }

            base.WndProc(ref message);
        }

        protected void Log(string message)
        {
            MDIParentMain.Log(message);
        }
        protected void Log(Exception ex, string message)
        {
            MDIParentMain.Log(ex, message);
        }
        protected void SafeLoad(EventHandler ev, object sender, EventArgs e)
        {
            try
            {
                ev(sender, e);
            }
            catch (Exception ex)
            {
                MDIParentMain.Log(ex, "Загрузка не удалась " + this.GetType().Name);
            }
        }
    }
}
