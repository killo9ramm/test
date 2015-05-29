using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Config_classes;
using System.Threading;
using System.Windows.Media;
using System.Windows.Controls;

namespace RBClientUpdateApplication.Updation
{
    public class UpdateClient
    {
        public readonly UpdateItem UpdItem;
        public Object state_obj;

        string Error="";

        private ClientStates previousState;
        public ClientStates _State;
        public ClientStates State
        {
            get { return _State; }
            set 
            {
                previousState = _State;
                _State = value;
                switch (State)
                {
                    case ClientStates.CopingRBU:
                        increase_Progress_bar();    
                        //нарисовать прогресс в юзерконтроле и в главном таскбаре
                        break;
                    case ClientStates.CopiedRBU:
                        
                        //нарисовать прогресс в юзерконтроле и в главном таскбаре
                        break;
                    case ClientStates.RBUEated:
                        
                        //нарисовать прогресс в юзерконтроле и в главном таскбаре
                        break;
                    case ClientStates.CopiedRBC:
                        //нарисовать прогресс в юзерконтроле и в главном таскбаре
                        break;
                    case ClientStates.Finished:
                        increase_Progress_bar();
                        //нарисовать прогресс в юзерконтроле и в главном таскбаре
                        //добавить завершенный элемент
                        add_completed_item_to_list();
                        break;
                    case ClientStates.Error:
                        increase_Progress_bar();
                        signalize_ui_error();
                        //нарисовать прогресс в юзерконтроле и в главном таскбаре
                        break;
                }
            }
        }

        string _PathToRest = "";
        string PathToRest
        {
            get
            {
                if (_PathToRest == "")
                {
                    _PathToRest = Path.Combine(UpdateManager.PathToFolder, Path.Combine(UpdItem.Teremok_1c_name, UpdateManager.FolderName));
                }
                return _PathToRest;
            }
        }

        public UpdateClient(UpdateItem item)
        {
            this.UpdItem = item;
        }

        internal void Start(Object state)
        {
            Log("Начинаем обработку");
            State = (ClientStates)state;
            EnterPoint();
        }

        private void EnterPoint()
        {
            try
            {
                Root.Log(UpdItem.Teremok_name + " статус " + State);
                switch (State)
                {
                    case ClientStates.CopingRBU:
                        Copy_New_RB_Updater();
                        break;
                    case ClientStates.CopiedRBU:
                        Check_RBU_Eated();
                        break;
                    case ClientStates.RBUEated:
                        Copy_New_RBClient();
                        break;

                    case ClientStates.CopiedRBC:
                        check_file_consistency();
                        break;

                    case ClientStates.Finished:
                        return;
                        break;

                    case ClientStates.Error:
                        return;
                        break;
                }
            }
            catch(Exception exp)
            {
                Log("Ошибка в потоке",exp);
            }
        }

        private void signalize_ui_error()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal,
            (System.Threading.ThreadStart)delegate {
                Brush brush = UpdateManager.mainWindow.progress1.Foreground;
                UpdateManager.mainWindow.progress1.Foreground = new SolidColorBrush(Colors.Red);
                ListViewItem lw = new ListViewItem();
                lw.Foreground = new SolidColorBrush(Colors.Red);
                lw.Content = this.UpdItem.Teremok_name + " не обновлен! Error:"+Error;
                UpdateManager.mainWindow.lveiw_restPanel.Items.Add(lw);
            });
        }

        private void increase_Progress_bar()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal,
            (System.Threading.ThreadStart)delegate { UpdateManager.mainWindow.progress1.Value+=1; });
        }

        private void add_completed_item_to_list()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal,
            (System.Threading.ThreadStart)delegate {
                ListViewItem lw = new ListViewItem();
                lw.Foreground = new SolidColorBrush(Colors.Green);
                lw.Content = this.UpdItem.Teremok_name + " обновлен!\r\n";
                UpdateManager.mainWindow.lveiw_restPanel.Items.Add(lw); 

            });
        }

        private void check_file_consistency()
        {
            //Проверить файл на целостность?

            State = ClientStates.Finished;
            EnterPoint();
        }

        private void Copy_New_RB_Updater()
        {
            try
            {
                Log("Копируем апдейтер");

                if (UpdateManager.RBUpdater_arr == null) throw new Exception("Отсутствует RBUpdater(null)");

                string pathToUpdater = Path.Combine(PathToRest, UpdateManager.RbUpdaterName);

#if(LOKING)
                lock (UpdateManager.RBUpdater_arr)
                {
                    File.WriteAllBytes(pathToUpdater, UpdateManager.RBUpdater_arr);
                }
#else

                File.WriteAllBytes(pathToUpdater, UpdateManager.RBUpdater_arr);

#endif
                State = ClientStates.CopiedRBU;
            }
            catch (Exception exp)
            {
                Log("Ошибка при попытке записать апдейтер", exp);
                State = ClientStates.Error;
                throw exp;
            }
            finally
            {
                EnterPoint();
            }
        }

        private void Check_RBU_Eated()
        {
            Thread.Sleep(UpdateManager.RBUCheckTimeout);
            
            Log("Проверяем съеден ли файл на фтп");

            string pathToUpdater = Path.Combine(PathToRest, UpdateManager.RbUpdaterName);

            if (!File.Exists(pathToUpdater))
            {
                State = ClientStates.RBUEated;
                Log("Файл на фтп съеден");
            }

            EnterPoint();
        }

        private void Copy_New_RBClient()
        {
            try
            {

                //if (UpdItem.Teremok_1c_name == "atriu")
                //{
                //    throw new Exception("Тестовая ошибка!!");
                //}

                if (UpdateManager.ArchState != ArhivStates.Ready)
                {
                    Log("RBClient архив еще не готов");
                    Thread.Sleep(UpdateManager.RBUCheckTimeout);
                    Copy_New_RBClient();
                    return;
                }

                Log("Копируем RBClient архив");
                State = ClientStates.CopingRBC;
                //string pathToUpdater = Path.Combine(PathToRest, UpdateManager.RbName);
                string pathToUpdater = Path.Combine(PathToRest, ArhiveManager.Archive.Name);

#if(LOKING)
                lock (UpdateManager.RBClient_arr)
                {
                    File.WriteAllBytes(pathToUpdater, UpdateManager.RBClient_arr);
                }
#else
                File.WriteAllBytes(pathToUpdater, UpdateManager.RBClient_arr);
#endif
                State = ClientStates.CopiedRBC;
            }
            catch (Exception exp)
            {
                Log("Ошибка при попытке записать RBClient", exp);
                State = ClientStates.Error;
                throw exp;
            }
            finally
            {
                EnterPoint();
            }
        }

        private void Log(string message)
        {
            Root.Log(message+" "+ UpdItem.Teremok_name);
        }
        private void Log(string message,Exception exp)
        {
            Root.Log(message + " " + UpdItem.Teremok_name+" error:"+exp.Message);
            Error += message + " error:" + exp.Message + "\r\n";
        }
    }
}
