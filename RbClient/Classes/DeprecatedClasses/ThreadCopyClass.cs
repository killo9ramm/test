using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using RBClient;
using RBClient.Classes;

namespace killolib
{
    public enum CopyThreadState {Created=-1,Started=0,RetryCopy=1,Error=2,TriesEnded=3,SuccessfulComplete=4,StaticComplete=5};

    class ThreadCopyClass : IDisposable
    {
        public ThreadCopyClass()
        {
            State = CopyThreadState.Created;
        }
        public static void Log(string message)
        {
            MDIParentMain.log.Debug(message);
        }

        public static void Log(Exception exp, string message)
        {
            MDIParentMain.log.Error(message,exp);
        }

        private CopyThreadState _State;
        public CopyThreadState State
        {
            get
            {
                return _State;
            }
            set
            {
                _State=value;
            }
        }

        private static CopyThreadState _StaticState = CopyThreadState.StaticComplete;
        public static CopyThreadState StaticState
        {
            get {
                if (ThreadList.Where(a => a.ThreadState != ThreadState.Running).Count() == ThreadCount)
                {
                    _StaticState = CopyThreadState.StaticComplete;
                }

                return _StaticState;
            }
            set {
                _StaticState = value;
            }
        }
        
        private const int _Tries = 10;

        //содержит список потоков
        private static List<Thread> ThreadList=new List<Thread>();

        [System.ComponentModel.DefaultValue(false)]
        public static bool WaitForEnd { get; set; }
        
        private const bool _WaitForEnd=false;

        [System.ComponentModel.DefaultValue(0)]
        public int Tries { get; set; }

        [System.ComponentModel.DefaultValue(10)]
        public static int MaxTries { get; set; }
        
        [System.ComponentModel.DefaultValue(10)]
        public static int Timeout { get; set; }
        private const int _Timeout = 5000;
        
        private static int _ThreadCount=0;
        public static int ThreadCount { get { return _ThreadCount; } }

        public void Dispose()
        {
        }

        public static void Disposed()
        {
            Reset();
        }

        /// <summary>
        /// Переобновление класса для работы
        /// </summary>
        public static void Reset()
        {
            Log("Обновляем Копировщик");
            MaxTries = _Tries;
            Timeout = _Timeout;
            _ThreadCount = 0;
            WaitForEnd = _WaitForEnd;
            ThreadList.Clear();
        }


        /// <summary>
        /// Асинхронно копирует список файлов по очереди в одно направление в отдельном потоке
        /// </summary>
        /// <param name="file_list_queue"></param>
        /// <param name="pathToMove"></param>
        public static void ThreadCopy(IEnumerable<FileInfo> file_list_queue, string pathToMove)
        {
            System.Threading.Thread thread = new System.Threading.Thread(
                new System.Threading.ThreadStart(delegate
                {
                    foreach (FileInfo fi in file_list_queue)
                    {
                        ThreadCopyClass tcc = new ThreadCopyClass();
                        tcc.TriesCopyFileTimeout(fi,pathToMove);
                    }
                }));

            ThreadList.Add(thread);
            _ThreadCount++;
            thread.Name = "CopyThread"+_ThreadCount;
            thread.Start();
            StaticState = CopyThreadState.Started;
        }

        /// <summary>
        /// Синхронно копирует список файлов по очереди в одно направление
        /// </summary>
        /// <param name="file_info"></param>
        /// <param name="pathToMove"></param>
        public static void ThreadCopySync(IEnumerable<FileInfo> file_list_queue, string pathToMove)
        {
            foreach (FileInfo fi in file_list_queue)
            {
                ThreadCopyClass tcc = new ThreadCopyClass();
                tcc.TriesCopyFileTimeout(fi, pathToMove);
            }
        }

        /// <summary>
        /// Асинхронно Копирует один файл в одно направление в отдельном потоке
        /// </summary>
        /// <param name="file_info"></param>
        /// <param name="pathToMove"></param>
        public static void ThreadCopy(FileInfo file_info, string pathToMove)
        {
            System.Threading.Thread thread = new System.Threading.Thread(
                new System.Threading.ThreadStart(delegate
                {
                        ThreadCopyClass tcc = new ThreadCopyClass();
                        tcc.TriesCopyFileTimeout(file_info, pathToMove);   
                }));

            ThreadList.Add(thread);
            _ThreadCount++;
            thread.Name = "CopyThread" + _ThreadCount;
            thread.Start();
            StaticState = CopyThreadState.Started;
        }


        /// <summary>
        /// Копирует один файл в одно направление с несколькими попытками
        /// </summary>
        /// <param name="file_info"></param>
        /// <param name="pathToMove"></param>
        public static void ThreadCopySync(FileInfo file_info, string pathToMove)
        {
          ThreadCopyClass tcc = new ThreadCopyClass();
          tcc.TriesCopyFileTimeout(file_info, pathToMove);
        }


        /// <summary>
        /// Перемещает один файл в одно направление с несколькими попытками
        /// </summary>
        /// <param name="file_info"></param>
        /// <param name="pathToMove"></param>
        public static void ThreadMoveSync(FileInfo file_info, string pathToMove)
        {
            ThreadCopyClass tcc = new ThreadCopyClass();
            tcc.TriesMoveFileTimeout(file_info, pathToMove);
        }

        /// <summary>
        /// Асинхронно Перемещает файл в указанную папку
        /// </summary>
        /// <param name="file_info"></param>
        /// <param name="pathToMove"></param>
        public static void ThreadMove(FileInfo file_info, string pathToMove)
        {
            System.Threading.Thread thread = new System.Threading.Thread(
                new System.Threading.ThreadStart(delegate
                {
                    ThreadCopyClass tcc = new ThreadCopyClass();
                    tcc.TriesCopyFileTimeout(file_info, pathToMove);
                    file_info.Delete();
                }));

            ThreadList.Add(thread);
            _ThreadCount++;
            thread.Name = "CopyThread" + _ThreadCount;
            thread.Start();
            StaticState = CopyThreadState.Started;
        }

        /// <summary>
        /// Рекурсивный метод копирования с количеством попыток и таймаутом
        /// </summary>
        /// <param name="fileSource"></param>
        /// <param name="folderDest"></param>
        private void TriesCopyFileTimeout(FileInfo fileSource,string folderDest)
        {
            if (Tries == 0) State = CopyThreadState.Started;
            
            try
            {
                Log("копируем файл: " + fileSource.FullName + " в " + folderDest);

                string dest_file_path=Path.Combine(folderDest, fileSource.Name);
                //копировать файл
                
                if (!fileSource.IsFilesEqual(new FileInfo(dest_file_path))) 
                fileSource.CopyTo(dest_file_path, true);

                State = CopyThreadState.SuccessfulComplete;
            }
            catch(Exception exp)
            {
                State = CopyThreadState.Error;
                Log(exp, "не получилось скопировать файл:" + fileSource.FullName + " в "+folderDest+" попытка " + this.Tries+" exception "+exp.Message);
                if (this.Tries > MaxTries)
                {
                    State = CopyThreadState.TriesEnded;
                    return;
                }
                else
                {
                    Thread.Sleep(Timeout);
                    this.Tries++;
                    State = CopyThreadState.RetryCopy;
                    TriesCopyFileTimeout(fileSource, folderDest);
                    return;
                }
            }
        }


        /// <summary>
        /// Рекурсивный метод копирования с количеством попыток и таймаутом
        /// </summary>
        /// <param name="fileSource"></param>
        /// <param name="folderDest"></param>
        private void TriesMoveFileTimeout(FileInfo fileSource, string folderDest)
        {
            if (Tries == 0) State = CopyThreadState.Started;

            try
            {
                Log("копируем файл: " + fileSource.FullName + " в " + folderDest);

                string dest_file_path = Path.Combine(folderDest, fileSource.Name);
                //копировать файл

                if (!fileSource.IsFilesEqual(new FileInfo(dest_file_path)))
                {
                    fileSource.CopyTo(dest_file_path, true);
                    fileSource.Delete();
                }
                else
                {
                    fileSource.Delete();
                }

                State = CopyThreadState.SuccessfulComplete;
            }
            catch (Exception exp)
            {
                State = CopyThreadState.Error;
                Log(exp, "не получилось скопировать файл:" + fileSource.FullName + " в " + folderDest + " попытка " + this.Tries + " exception " + exp.Message);
                if (this.Tries > MaxTries)
                {
                    State = CopyThreadState.TriesEnded;
                    return;
                }
                else
                {
                    Thread.Sleep(Timeout);
                    this.Tries++;
                    State = CopyThreadState.RetryCopy;
                    TriesMoveFileTimeout(fileSource, folderDest);
                    return;
                }
            }
        }

    }
}
