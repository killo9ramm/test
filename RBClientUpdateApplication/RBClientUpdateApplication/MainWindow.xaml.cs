using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NLog;
using RBServer.Debug_classes;
using System.IO;
using System.Xml;
using NLog.Config;
using NLog.Targets;
using RBClientUpdateApplication.Updation;
using RBClient.Classes.CustomClasses;
using System.Diagnostics;
using Config_classes;

namespace RBClientUpdateApplication
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Logger log = LogManager.GetCurrentClassLogger();       
        

        public MainWindow()
        {
            NlogConfig();
            InitializeComponent();

            Root.LogEvent += ErrorLogging;
            Root.TraceEvent += TraceLogging;
            DebugPanel.ErrorOccured += ErrorLogging;

            Loaded+=new RoutedEventHandler(MainWindow_Loaded);
            Root.Log("Упдайтер запущен!");

             tabControl1.SelectionChanged+=(s, e) =>
            {
                if (UpdateManager.Started == true && tabControl1.SelectedItem==tabItem1)
                {
                    Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                }
            };
        }

        private void NlogConfig()
        {
            #region log xml

            //            string xml = @"<nlog>
//                   <targets>
//                     <target name='console' type='Console' layout='${message}' />
//                   </targets>
//
//                   <rules>
//                     <logger name='*' minlevel='Error' writeTo='console' />
//
//                   </rules>
//                 </nlog>";

//            StringReader sr = new StringReader(xml);
//            XmlReader xr = XmlReader.Create(sr);

//            XmlLoggingConfiguration config = new XmlLoggingConfiguration(xr, null);

//            //config.LoggingRules
            //            LogManager.Configuration = config;
            #endregion

            LoggingConfiguration config = new LoggingConfiguration();

                FileTarget fileTarget = new FileTarget();
                config.AddTarget("file", fileTarget);
                fileTarget.FileName = "RBUpdater.log";
                fileTarget.ArchiveFileName = "RBUpdater.{#####}.txt";
                fileTarget.ArchiveAboveSize = 202400; // 20mb
                fileTarget.ArchiveNumbering = ArchiveNumberingMode.Sequence;
                fileTarget.ConcurrentWrites = true;
                fileTarget.KeepFileOpen = false;
                fileTarget.Layout = "${longdate} | ${level} | ${message}";
                LoggingRule rule2 = new LoggingRule("*", LogLevel.Info, fileTarget);
                config.LoggingRules.Add(rule2);
                LogManager.Configuration = config;
        }

        private static void ErrorLogging(object sender,MessageEventArgs em)
        {
            log.Error(em.Message);
        }

        private static void TraceLogging(object sender, MessageEventArgs em)
        {
            log.Info(em.Message);
        }

        private void MainWindow_Loaded(object sender,EventArgs e)
        {
            try
            {

                //сформировать по ресторанам классы

               resto_list.ItemsSource = Root.ItemsList.Select<UpdateItem, RestoranControl>(a => a.RestControl);

                //прицепить события к items
               Root.ItemsList.ForEach(a=>a.RestControl.RestoranChecked+=multiple_restoran_checked);

                Root.LogEvent+=new Root.MessageEventHandler(Root_LogEvent);
                Root.TraceEvent+= new Root.MessageEventHandler(Root_LogEvent);
                //открыть окно настроек
                //SettingsWindow settWimdow = new SettingsWindow();
                //settWimdow.ShowDialog();

            }
            catch (Exception exp)
            {
                Root.Log(exp, "Не удалось загрузить главное окно");
            }
        }

        private void Root_LogEvent(object sender, MessageEventArgs e)
        { 
            System.Windows.Application.Current.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal,
            (System.Threading.ThreadStart)delegate { txtblk_logPanel.Text += e.Message + "\r\n"; scrl_1.ScrollToBottom(); });
        }



        private void multiple_restoran_checked(object sender, EventArgs e)
        {
            if (resto_list.SelectedItems.Count == 0)
                return;
            bool flag=(sender as RestoranControl).IsChecked;

            resto_list.SelectedItems.OfType<RestoranControl>().ToList().ForEach(a => a.IsChecked = flag);

        
        }

        private void Update_btn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("После нажатия ОК запустится обновление на сеть!","Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (Root.ItemsList.Where(a => a.RestControl.IsChecked).ToList().Count > 0)
                {
                    //сохранить в свойства
                    string str="";
                    Root.ItemsList.Where(a => a.RestControl.IsChecked).ToList().ForEach(a =>
                    {
                        str += a.Teremok_1c_name + ";";
                    });
                    Properties.Settings.Default.SelectedItems=str;

                    
                    tabItem2.Visibility = Visibility.Visible;
                    tabControl1.SelectedItem = tabItem2;

                    Clear_tabItem2();

                    UpdateManager.Start(this);

                }
                else
                {
                    MessageBox.Show("Не выбрано ни одного ресторана!", "Предупреждение", MessageBoxButton.OK);
                }
            }
        }

        private void Clear_tabItem2()
        {
            lveiw_restPanel.Items.Clear();
            txtblk_logPanel.Text = "";
            progress1.Value = 0;
            progress1.Foreground = new SolidColorBrush(Colors.Green);
        }

        private void check_all_btn_Click(object sender, RoutedEventArgs e)
        {
            Root.ItemsList.ForEach(a => a.RestControl.IsChecked = true);
        }

        private void release_btn_Click(object sender, RoutedEventArgs e)
        {
            Root.ItemsList.ForEach(a => a.RestControl.IsChecked = false);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //открыть окно настроек
            SettingsWindow settWimdow = new SettingsWindow();
            settWimdow.ShowDialog();
            UpdateList();
        }

        private void MenuClose_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void copy_RBU_chbx_Checked(object sender, RoutedEventArgs e)
        {
            Root.RBUCopy = (bool)copy_RBU_chbx.IsChecked;
        }

        private void btn_file_upload_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("После нажатия ОК запустится обновление на сеть!", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (Root.ItemsList.Where(a => a.RestControl.IsChecked).ToList().Count > 0)
                {
                    //сохранить в свойства
                    string str = "";
                    Root.ItemsList.Where(a => a.RestControl.IsChecked).ToList().ForEach(a =>
                    {
                        str += a.Teremok_1c_name + ";";
                    });
                    Properties.Settings.Default.SelectedItems = str;


                    tabItem2.Visibility = Visibility.Visible;
                    tabControl1.SelectedItem = tabItem2;

                    Clear_tabItem2();

                    string filename = "";
                    //скопировать файл в темовую папку
                    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                    // Display OpenFileDialog by calling ShowDialog method 

                    Nullable<bool> result = dlg.ShowDialog();

                    if (result == true)
                    {
                        // Open document 
                        filename = dlg.FileName;
                    }

                    if (filename != "")
                    {
                        FileInfo fi = new FileInfo(filename);
                        ArhiveManager.CreateTempFolder();

                        new CustomAction((o) =>
                        {
                            File.Copy(filename, System.IO.Path.Combine(ArhiveManager.TempFolder.FullName, fi.Name));
                            ArhiveManager.ArchiveState = ArhivStates.Ready;
                            ArhiveManager.Archive = new FileInfo(System.IO.Path.Combine(ArhiveManager.TempFolder.FullName, fi.Name));
                            UpdateManager.StartUploadFile(this);
                        }, null).Start();
                    }
                }
                else
                {
                    MessageBox.Show("Не выбрано ни одного ресторана!", "Предупреждение", MessageBoxButton.OK);
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)chbx_filter.IsChecked)
            {
                List<t_Teremok> teremok_list = Root.RBM.t_Teremok.Where<t_Teremok>(a => a.teremok_use_ARM).OrderBy(a => a.teremok_name).ToList();
                Root.CreateMainItemsList(teremok_list);
                resto_list.ItemsSource = Root.ItemsList.Select<UpdateItem, RestoranControl>(a => a.RestControl);
            }
            else
            {
                string t_city = ConfigClass.GetProperty("City").ToString();
                List<t_Teremok> teremok_list = Root.RBM.t_Teremok.Where<t_Teremok>(a => a.teremok_use_ARM && ((bool)a.deleted != true || a.deleted == null) && (a.teremok_address != "" || a.teremok_address != null)
                    && a.teremok_city == t_city)
                .OrderBy(a => a.teremok_name).ToList();
                Root.CreateMainItemsList(teremok_list);
                resto_list.ItemsSource = Root.ItemsList.Select<UpdateItem, RestoranControl>(a => a.RestControl);
            }
        }
        private void UpdateList()
        {
            string t_city = ConfigClass.GetProperty("City").ToString();
            List<t_Teremok> teremok_list = Root.RBM.t_Teremok.Where<t_Teremok>(a => a.teremok_use_ARM && ((bool)a.deleted != true || a.deleted == null) && (a.teremok_address != "" || a.teremok_address != null)
                && a.teremok_city == t_city)
            .OrderBy(a => a.teremok_name).ToList();
            Root.CreateMainItemsList(teremok_list);
            resto_list.ItemsSource = Root.ItemsList.Select<UpdateItem, RestoranControl>(a => a.RestControl);
        }

        private void butt_sort_ver_Click(object sender, RoutedEventArgs e)
        {
            string t_city = ConfigClass.GetProperty("City").ToString();
            List<t_Teremok> teremok_list = Root.RBM.t_Teremok.Where<t_Teremok>(a => a.teremok_use_ARM && ((bool)a.deleted != true || a.deleted == null) && (a.teremok_address != "" || a.teremok_address != null)
                && a.teremok_city == t_city)
            .OrderBy(a => a.teremok_address).ToList();
            Root.CreateMainItemsList(teremok_list);
            resto_list.ItemsSource = Root.ItemsList.Select<UpdateItem, RestoranControl>(a => a.RestControl);
        }

        private void butt_sort_name_Click(object sender, RoutedEventArgs e)
        {
            string t_city = ConfigClass.GetProperty("City").ToString();
            List<t_Teremok> teremok_list = Root.RBM.t_Teremok.Where<t_Teremok>(a => a.teremok_use_ARM && ((bool)a.deleted != true || a.deleted == null) && (a.teremok_address != "" || a.teremok_address != null)
                && a.teremok_city == t_city)
            .OrderBy(a => a.teremok_name).ToList();
            Root.CreateMainItemsList(teremok_list);
            resto_list.ItemsSource = Root.ItemsList.Select<UpdateItem, RestoranControl>(a => a.RestControl);
        }

        private void butt_search_Click(object sender, RoutedEventArgs e)
        {
            var list = resto_list.ItemsSource.OfType<RestoranControl>().ToList();
            try
            {
                if (list != null && list.Count > 0 && txtbx_search.Text != "")
                {
                    RestoranControl item = null;
                    if (resto_list.SelectedItem != null)
                    {
                        var selectedItem = resto_list.SelectedItem as RestoranControl;

                        var temp = list.Where(a => a.ControlHeader.IndexOf(txtbx_search.Text, StringComparison.OrdinalIgnoreCase) != -1
                            && list.IndexOf(selectedItem) < list.IndexOf(a));
                        if (temp.Count()==0)
                        {
                            item = list.Where(a => a.ControlHeader.IndexOf(txtbx_search.Text, StringComparison.OrdinalIgnoreCase) != -1).First();
                        }
                        else
                        {
                            item = temp.First();
                        }
                    }
                    else
                    {
                        item = list.Where(a => a.ControlHeader.IndexOf(txtbx_search.Text, StringComparison.OrdinalIgnoreCase) != -1).First();
                    }

                    if (item != null)
                    {
                        resto_list.SelectedItem = item;
                        resto_list.ScrollIntoView(item);
                    }
                }
            }catch(Exception ex)
            {
            }
        }

        private void txtbx_search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                butt_search_Click(sender, e);
            }
        }
    }
}
