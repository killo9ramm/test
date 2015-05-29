using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using RBClient.Classes.CustomClasses;
using System.Windows.Forms;
using CustomLogger;
using RBClient.Classes;

namespace ftpUpLoader
{
    class TaskManager : LoggerBase
    {
        private AutoResetEvent are = new AutoResetEvent(false);

        private DirectoryInfo dir1,dir2;
        Thread th;

        private bool _IsWorking = false;
        public bool IsWorking
        {
            get
            {
                return _IsWorking;
            }
        }

        public TaskManager(DirectoryInfo dir_files,DirectoryInfo dir_kkm)
        {
            dir1 = dir_files; dir2 = dir_kkm;
            th = new Thread(UpdateMenu);
            th.IsBackground = true;
            th.Start();
        }

        public void Go()
        {
            are.Set();
        }

        public void UpdateMenu()
        {
            try
            {
                are.WaitOne();
                Thread.Sleep(3000);
                _IsWorking = true;

                CustomAction ca = new CustomAction(o =>
                {
                    dir1.Refresh();
                    if (dir1 == null || dir1.Exists == false || dir1.GetFiles().Count() == 0)
                    {
                        throw new Exception("Не удалось создать задачу обновления. Директория с файлами меню пустая или отсутствует " + dir1.FullName);
                    }
                }, null); ca.LogEvent = Log; ca.Start();
                if (ca.State != StateEnum.SuccessfulComplete)
                {
                    Log("Не удалось создать задачу обновления. Директория с файлами меню пустая или отсутствует " + dir1.FullName);
                    _IsWorking = false;
                    return;
                }

                CustomAction ca1 = new CustomAction(o =>
                {
                    dir2.Refresh();
                    if (dir2 == null || dir2.Exists == false)
                    {
                        throw new Exception("Не удалось создать задачу обновления. Директория для файлов меню отсутствует " + dir2.FullName);
                    }
                }, null); ca1.LogEvent = Log; ca1.Start();

                if (ca.State != StateEnum.SuccessfulComplete)
                {
                    Log("Не удалось создать задачу обновления. Директория для файлов меню отсутствует " + dir2.FullName);
                    _IsWorking = false;
                    return;
                }
                LoadMenu();
            }
            catch(ThreadAbortException ex)
            {
                Log(ex,"Процесс обновления меню завершен принудительно");
            }
            catch(Exception ex)
            {
                Log(ex,"Произошла ошибка во время обновления меню");
            }
        }
        private void LoadMenu()
        {
            Log("Открываем окно меню");
            MessageBox.Show("Необходимо обновить меню","");

            Log("Приступаем к обновлению меню");

            th.IsBackground = false;
            dir1.GetFiles().ToList().ForEach(a =>
            {
                new CustomAction(o =>
                {
                    a.CopyTo(Path.Combine(dir2.FullName, a.Name), true);
                    Log("Скопировали файл "+a.Name+" в "+dir2.FullName);
                }, null).Start();
            });
            Log("Завершили обновление меню");
            Dispose();
        }
        private void Dispose()
        {
            th = null;
            _IsWorking = false;
            are.Close();
        }
        public void Abort()
        {
            th.Abort();
            Dispose();
        }

    }
}
