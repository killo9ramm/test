using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;            
using System.Runtime.InteropServices;   
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace RBClient
{
    /// <summary>
    /// Скрытая форма, которую мы используем, чтобы получить Windows сообщения о флэш-накопители
    /// </summary>
    internal class DetectorForm : Form
    {
        private Label label1;
        private DriveDetector mDetector = null;

        /// <summary>
        /// Настройка скрытой формы
        /// </summary>
        /// <param name="detector">DriveDetector объект, который будет получать уведомления о дисках USB</param>
        public DetectorForm(DriveDetector detector)
        {
            mDetector = detector;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ShowInTaskbar = false;
            this.ShowIcon = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Load += new System.EventHandler(this.Load_Form);
            this.Activated += new EventHandler(this.Form_Activated);
        }

        private void Load_Form(object sender, EventArgs e)
        {            
            InitializeComponent();
            this.Size = new System.Drawing.Size(5, 5);
        }

        private void Form_Activated(object sender, EventArgs e)
        {
            this.Visible = false;
        }

       
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (mDetector != null)
            {
                mDetector.WndProc(ref m);
            }
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(314, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Невидемая форма буээээ!";
            // 
            // DetectorForm
            // 
            this.ClientSize = new System.Drawing.Size(360, 80);
            this.Controls.Add(this.label1);
            this.Name = " ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }   // class DetectorForm


    //Обработчик событий
    public delegate void DriveDetectorEventHandler(Object sender, DriveDetectorEventArgs e);
    
    /// <summary>
    ///Передаем событие
    /// 
    /// </summary>
    public class DriveDetectorEventArgs : EventArgs 
    {
        public DriveDetectorEventArgs()
        {
            Cancel = false;
            Drive = "";
            HookQueryRemove = false;
        }

       
        public bool Cancel;
        public string Drive;   
        public bool HookQueryRemove;

    }    
  
    class DriveDetector : IDisposable 
    {
       
        public event DriveDetectorEventHandler DeviceArrived;
        public event DriveDetectorEventHandler DeviceRemoved;
        public event DriveDetectorEventHandler QueryRemove;
             
        public DriveDetector()
        {
            DetectorForm  frm = new DetectorForm(this);
            frm.Show();
            Init(frm, null);
        }

        public DriveDetector(Control control)
        {
            Init(control, null);
        }

        public DriveDetector(Control control, string FileToOpen)
        {
            Init(control, FileToOpen);
        }

        private void Init(Control control, string fileToOpen)
        {
            mFileToOpen = fileToOpen;
            mFileOnFlash = null;
            mDeviceNotifyHandle = IntPtr.Zero;
            mRecipientHandle = control.Handle;
            mDirHandle = IntPtr.Zero; 
            mCurrentDrive = "";
        }

	    public bool IsQueryHooked
	    {
		    get
            {
                if (mDeviceNotifyHandle == IntPtr.Zero)
                    return false;
                else
                    return true;
            }
	    }

        public string HookedDrive
        {
            get
            {
                return mCurrentDrive;
            }
        }

        public FileStream OpenedFile
        {
            get
            {
                return mFileOnFlash;
            }
        }

        public bool EnableQueryRemove(string fileOnDrive)
        {
            if (fileOnDrive == null || fileOnDrive.Length == 0)
                throw new ArgumentException(" ");
            
            if ( fileOnDrive.Length == 2 && fileOnDrive[1] == ':' )
                fileOnDrive += '\\';   

            if (mDeviceNotifyHandle != IntPtr.Zero)
            {
                RegisterForDeviceChange(false, null);
            }

            if (Path.GetFileName(fileOnDrive).Length == 0 ||!File.Exists(fileOnDrive))
                mFileToOpen = null;     
            else
                mFileToOpen = fileOnDrive;

            RegisterQuery(Path.GetPathRoot(fileOnDrive));
            if (mDeviceNotifyHandle == IntPtr.Zero)
                return false; 

            return true;
        }

        public void DisableQueryRemove()
        {
            if (mDeviceNotifyHandle != IntPtr.Zero)
            {
                RegisterForDeviceChange(false, null);
            }
        }


        public void Dispose()
        {
            RegisterForDeviceChange(false, null);
        }


        #region WindowProc
 
        public void WndProc(ref Message m)
        {
            int devType;
            char c;

            if (m.Msg == WM_DEVICECHANGE)
            {
                
                switch (m.WParam.ToInt32())
                {

                      
                    case DBT_DEVICEARRIVAL:

                        devType = Marshal.ReadInt32(m.LParam, 4);
                        if (devType == DBT_DEVTYP_VOLUME)
                        {
                            DEV_BROADCAST_VOLUME vol;
                            vol = (DEV_BROADCAST_VOLUME)
                                Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_VOLUME));

                           
                            c = DriveMaskToLetter(vol.dbcv_unitmask);


                            DriveDetectorEventHandler tempDeviceArrived = DeviceArrived;
                            if ( tempDeviceArrived != null )
                            {
                                DriveDetectorEventArgs e = new DriveDetectorEventArgs();
                                e.Drive = c + ":\\";
                                tempDeviceArrived(this, e);
                                
                               
                                if (e.HookQueryRemove)
                                {
                                    if (mDeviceNotifyHandle != IntPtr.Zero)
                                    {
                                        RegisterForDeviceChange(false, null);
                                    }
                                        
                                   RegisterQuery(c + ":\\");
                                }
                            } 
                            
                        }
                        break;



                     
                    case DBT_DEVICEQUERYREMOVE:

                        devType = Marshal.ReadInt32(m.LParam, 4);
                        if (devType == DBT_DEVTYP_HANDLE)
                        {
                           
                            DriveDetectorEventHandler tempQuery = QueryRemove;
                            if (tempQuery != null)
                            {
                                DriveDetectorEventArgs e = new DriveDetectorEventArgs();
                                e.Drive = mCurrentDrive;   
                                tempQuery(this, e);

                               
                                if (e.Cancel)
                                {                                    
                                    m.Result = (IntPtr)BROADCAST_QUERY_DENY;
                                }
                                else
                                {                                  
                                    RegisterForDeviceChange(false, null); 
                                }

                           }                          
                        }
                        break;

                    case DBT_DEVICEREMOVECOMPLETE:

                        devType = Marshal.ReadInt32(m.LParam, 4);
                        if (devType == DBT_DEVTYP_VOLUME)
                        {
                            devType = Marshal.ReadInt32(m.LParam, 4);
                            if (devType == DBT_DEVTYP_VOLUME)
                            {
                                DEV_BROADCAST_VOLUME vol;
                                vol = (DEV_BROADCAST_VOLUME)
                                    Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_VOLUME));
                                c = DriveMaskToLetter(vol.dbcv_unitmask);

                                DriveDetectorEventHandler tempDeviceRemoved = DeviceRemoved;
                                if (tempDeviceRemoved != null)
                                {
                                    DriveDetectorEventArgs e = new DriveDetectorEventArgs();
                                    e.Drive = c + ":\\";
                                    tempDeviceRemoved(this, e);
                                }
                            }
                        }
                        break;
                }

            }

        }
        #endregion



        #region  Private Area

      
        private IntPtr mDirHandle = IntPtr.Zero;

        private FileStream mFileOnFlash = null;

        private string mFileToOpen;
      
        private IntPtr mDeviceNotifyHandle;

        private IntPtr mRecipientHandle;

        private string mCurrentDrive;   

        private const int DBT_DEVTYP_DEVICEINTERFACE = 5;
        private const int DBT_DEVTYP_HANDLE = 6;
        private const int BROADCAST_QUERY_DENY = 0x424D5144;
        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000; 
        private const int DBT_DEVICEQUERYREMOVE = 0x8001;  
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004; 
        private const int DBT_DEVTYP_VOLUME = 0x00000002; 

        private void RegisterQuery(string drive)
        {
            bool register = true;

            if (mFileToOpen == null)
            {
                         
            }
            else
            {                
                if (mFileToOpen.Contains(":"))
                {
                    string tmp = mFileToOpen.Substring(3);
                    string root = Path.GetPathRoot(drive);
                    mFileToOpen = Path.Combine(root, tmp);
                }
                else
                    mFileToOpen = Path.Combine(drive, mFileToOpen);
            }


            try
            {               
                if (mFileToOpen == null)  
                    mFileOnFlash = null;
                else
                    mFileOnFlash = new FileStream(mFileToOpen, FileMode.Open);
            }
            catch (Exception)
            {                
                register = false;
            }


            if (register)
            {
                if (mFileOnFlash == null)
                    RegisterForDeviceChange(drive);
                else
                    
                    RegisterForDeviceChange(true, mFileOnFlash.SafeFileHandle);

                mCurrentDrive = drive;
            }


        }


        
        private void RegisterForDeviceChange(string dirPath)
        {
            IntPtr handle = Native.OpenDirectory(dirPath);
            if (handle == IntPtr.Zero)
            {
                mDeviceNotifyHandle = IntPtr.Zero;
                return;
            }
            else
                mDirHandle = handle;    

            DEV_BROADCAST_HANDLE data = new DEV_BROADCAST_HANDLE();
            data.dbch_devicetype = DBT_DEVTYP_HANDLE;
            data.dbch_reserved = 0;
            data.dbch_nameoffset = 0;
            //data.dbch_data = null;
            //data.dbch_eventguid = 0;
            data.dbch_handle = handle;
            data.dbch_hdevnotify = (IntPtr)0;
            int size = Marshal.SizeOf(data);
            data.dbch_size = size;
            IntPtr buffer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(data, buffer, true);

            mDeviceNotifyHandle = Native.RegisterDeviceNotification(mRecipientHandle, buffer, 0);

        }

        private void RegisterForDeviceChange(bool register, SafeFileHandle fileHandle)
        {
            if (register)
            {
                DEV_BROADCAST_HANDLE data = new DEV_BROADCAST_HANDLE();
                data.dbch_devicetype = DBT_DEVTYP_HANDLE;
                data.dbch_reserved = 0;
                data.dbch_nameoffset = 0;
                //data.dbch_data = null;
                //data.dbch_eventguid = 0;
                data.dbch_handle = fileHandle.DangerousGetHandle();  
                data.dbch_hdevnotify = (IntPtr)0;
                int size = Marshal.SizeOf(data);
                data.dbch_size = size;
                IntPtr buffer = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(data, buffer, true);

                mDeviceNotifyHandle = Native.RegisterDeviceNotification(mRecipientHandle, buffer, 0);
            }
            else
            {
                if (mDirHandle != IntPtr.Zero)
                {
                    Native.CloseDirectoryHandle(mDirHandle);
                    //    string er = Marshal.GetLastWin32Error().ToString();
                }

                // unregister
                if (mDeviceNotifyHandle != IntPtr.Zero)
                {
                    Native.UnregisterDeviceNotification(mDeviceNotifyHandle);                                        
                }
                

                mDeviceNotifyHandle = IntPtr.Zero;
                mDirHandle = IntPtr.Zero;
               
                mCurrentDrive = "";
                if (mFileOnFlash != null)
                {
                    mFileOnFlash.Close();
//                    mFileOnFlashOpenDirectory = null;
                    mFileOnFlash = null;
                }
            }

        }

      
        private static char DriveMaskToLetter(int mask)
        {
            char letter;
            string drives = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            // 1 = A
            // 2 = B
            // 4 = C...
            int cnt = 0;
            int pom = mask / 2;     
            while (pom != 0)
            {              
                pom = pom / 2;
                cnt++;
            }

            if (cnt < drives.Length)
                letter = drives[cnt];
            else
                letter = '?';

            return letter;
        }

      /*
        private string GetAnyFile(string drive)
        {
            string file = "";
            // First try files in the root
            string[] files = Directory.GetFiles(drive);
            if (files.Length == 0)
            {
                // if no file in the root, search whole drive
                files = Directory.GetFiles(drive, "*.*", SearchOption.AllDirectories);
            }
                
            if (files.Length > 0)
                file = files[0];        // get the first file

            // return empty string if no file found
            return file;
        }*/
        #endregion


        #region Native Win32 API
            
        private class Native
        {
            const uint GENERIC_READ = 0x80000000;
            const uint OPEN_EXISTING = 3;
            const uint FILE_SHARE_READ = 0x00000001;
            const uint FILE_SHARE_WRITE = 0x00000002;
            const uint FILE_ATTRIBUTE_NORMAL = 128;
            const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
            static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, uint Flags);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern uint UnregisterDeviceNotification(IntPtr hHandle);

            [DllImport("kernel32", SetLastError = true)]
            static extern IntPtr CreateFile(
                  string FileName,             
                  uint DesiredAccess,              
                  uint ShareMode,                    
                  uint SecurityAttributes,          
                  uint CreationDisposition,           
                  uint FlagsAndAttributes,            
                  int hTemplateFile                   
                  );


            [DllImport("kernel32", SetLastError = true)]
            static extern bool CloseHandle(
                  IntPtr hObject  
                  );

     
            static public IntPtr OpenDirectory(string dirPath)
            {        
                IntPtr handle = CreateFile(
                      dirPath,
                      GENERIC_READ,
                      FILE_SHARE_READ | FILE_SHARE_WRITE,
                      0,
                      OPEN_EXISTING,
                      FILE_FLAG_BACKUP_SEMANTICS | FILE_ATTRIBUTE_NORMAL,
                      0);

                if ( handle == INVALID_HANDLE_VALUE)
                    return IntPtr.Zero;
                else
                    return handle;
            }


            public static bool CloseDirectoryHandle(IntPtr handle)
            {
                return CloseHandle(handle);
            }
        }

        
        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_HANDLE
        {
            public int dbch_size;
            public int dbch_devicetype;
            public int dbch_reserved;
            public IntPtr dbch_handle;
            public IntPtr dbch_hdevnotify;
            public Guid dbch_eventguid;
            public long dbch_nameoffset;
            //public byte[] dbch_data[1]; // = new byte[1];
            public byte dbch_data;
            public byte dbch_data1; 
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_VOLUME
        {
            public int dbcv_size;
            public int dbcv_devicetype;
            public int dbcv_reserved;
            public int dbcv_unitmask;
        }
        #endregion

    }
}
