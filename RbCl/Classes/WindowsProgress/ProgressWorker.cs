using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace RBClient.Classes.WindowsProgress
{
    public class ProgressWorker : Control
    {
        public string Header { get; set; }
        public int Status {get;set; }
        public  string Name { get; set; }

        public int FontSize = 0;

        private Form MdiParent;

        protected WindowProgress progrWindow;
        private BackgroundWorker bkgr;

        private Bitmap Image;
        public ProgressWorker(string Name, string _Header,Bitmap image)
        {
            Image = image;
            Header = _Header;
            this.Name = Name;

            bkgr = new BackgroundWorker();
            bkgr.WorkerSupportsCancellation = true;
            bkgr.WorkerReportsProgress = true;
            bkgr.DoWork += new DoWorkEventHandler(bkgr_Inbox_DoWork_with_image);
            bkgr.Disposed += new EventHandler(bkgr_Disposed);
        }

        public ProgressWorker(string Name, string _Header)
        {
            Header = _Header;
            this.Name = Name;

            bkgr = new BackgroundWorker();
            bkgr.WorkerSupportsCancellation = true;
            bkgr.WorkerReportsProgress = true;
            bkgr.DoWork += new DoWorkEventHandler(bkgr_Inbox_DoWork_with_Header);
            bkgr.Disposed += new EventHandler(bkgr_Disposed);
        }

        public ProgressWorker(Form MdiParent,string Name,string _Header)
        {
            Header = _Header;
            this.Name = Name;
            this.MdiParent = MdiParent;

            bkgr=new BackgroundWorker();
            bkgr.WorkerSupportsCancellation = true;
            bkgr.WorkerReportsProgress = true;
            bkgr.DoWork += new DoWorkEventHandler(bkgr_Inbox_DoWork);
            bkgr.Disposed+=new EventHandler(bkgr_Disposed);            
        }

        #region private methods
        private void bkgr_Disposed(object sender,EventArgs e)
        {
            
        }

        private void Close()
        {
            progrWindow.CClose();
        }

        private void bkgr_Inbox_ProgressChanged(object sender,ProgressChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new ProgressChangedEventHandler(bkgr_Inbox_ProgressChanged),new object[]{this,e});
                return;
            }

            progrWindow.Status = e.ProgressPercentage;
        }


        private void bkgr_Inbox_DoWork_with_image(object sender, DoWorkEventArgs e)
        {
            progrWindow = new WindowProgress();
            //  progrWindow.Parent = MdiParent;
            progrWindow.Header = Header;
            progrWindow.Status = 0;
            if (Image != null)
            {
                progrWindow.BackgroundImage = Image;
                progrWindow.Size = Image.Size;
                progrWindow.progressBar1.Width = progrWindow.Width - 25;
                progrWindow.progressBar1.Location = new Point(progrWindow.progressBar1.Location.X, progrWindow.Height - 40);
            }
            if (null != MdiParent)
            {
                progrWindow.StartPosition = FormStartPosition.Manual;
                progrWindow.Location = GetStartupPositionCenter(MdiParent, progrWindow);
            }
            else
            {
                progrWindow.StartPosition = FormStartPosition.CenterScreen;
            }

            

            progrWindow.TopMost = true;
            progrWindow.ShowInTaskbar = true;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MDIParentMain));
            progrWindow.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            progrWindow.ShowDialog();
        }

        private void bkgr_Inbox_DoWork_with_Header(object sender,DoWorkEventArgs e)
        {
            progrWindow = new WindowProgress();
            //  progrWindow.Parent = MdiParent;
            progrWindow.Header = Header;
            progrWindow.Status = 0;

            if(FontSize!=0)
            {
                //progrWindow.label1.Font.Size = FontSize;
                progrWindow.label1.Font = new Font(progrWindow.label1.Font.FontFamily,FontSize);
            }

            if (null != MdiParent)
            {
                progrWindow.StartPosition = FormStartPosition.Manual;
                progrWindow.Location = GetStartupPositionCenter(MdiParent, progrWindow);
            }
            else
            {
                progrWindow.StartPosition = FormStartPosition.CenterScreen;
            }
            progrWindow.TopMost = true;
            progrWindow.ShowInTaskbar = true;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MDIParentMain));
            progrWindow.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            progrWindow.ShowDialog();
        }

        private void bkgr_Inbox_DoWork(object sender,DoWorkEventArgs e)
        {
            progrWindow = new WindowProgress();
            //  progrWindow.Parent = MdiParent;
            progrWindow.Header = Header;

            if (FontSize != 0)
            {
                //progrWindow.label1.Font.Size = FontSize;
                progrWindow.label1.Font = new Font(progrWindow.label1.Font.FontFamily, FontSize);
            }

            progrWindow.Status = 0;
            if (null != MdiParent)
            {
                progrWindow.StartPosition = FormStartPosition.Manual;
                progrWindow.Location = GetStartupPositionCenter(MdiParent, progrWindow);
            }
            else
            {
                progrWindow.StartPosition = FormStartPosition.CenterScreen;
            }
            progrWindow.TopMost = true;
            progrWindow.ShowDialog();
        }
        #endregion

        private Point GetStartupPositionCenter(Form owner,Form child)
        {
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
	        int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;

            int owidth = owner.Width;
            int oheight = owner.Height;
            int oX = owner.Left;
            int oY = owner.Top;
            int cwidth = child.Width;
            int cheight = child.Height;

            int newX = oX + (int)((owidth - cwidth) / 2);
            int newY = oY + (int)((oheight - cheight) / 2);


            return new Point(newX, newY);
        }

        #region public methods
        public void Start()
        {
            bkgr.RunWorkerAsync();
        }
        public void Stop()
        {
            try
            {
                bkgr.CancelAsync();

                if (progrWindow.InvokeRequired)
                {
                    progrWindow.Invoke(new MethodInvoker(delegate { progrWindow.Close(); }));

                }
                else
                {
                    progrWindow.Close();
                }

                bkgr.Dispose();
            }catch(Exception ex)
            {
                MDIParentMain.Log(ex,"Не могу остановить окно прогресса");
            }
        }

        public delegate void DelegateIntPattern(int val);

        public void ReportProgress(int progress)
        {
            try
            {
                if (progrWindow.InvokeRequired)
                {
                    progrWindow.Invoke(new MethodInvoker(delegate
                        {
                            if (null != progrWindow) progrWindow.Status = progress;
                        }));
                }
            }catch{
            }
        }
        #endregion
    }
}
