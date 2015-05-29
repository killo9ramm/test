using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using RBServer.Debug_classes;
using System.Net.Mail;
using RBClient.Classes.CustomClasses;

namespace RBClient.Classes
{
    public static class ExtensionClass
    {

        #region DirectoryInfo Extensions

        /// <summary>
        /// Создает или возвращает директорию если она существует
        /// </summary>
        /// <param name="dir_path"></param>
        /// <returns></returns>
        public static DirectoryInfo CreateOrReturn(this DirectoryInfo dir_path)
        {
            if (!dir_path.Exists)
                dir_path.Create();
            return dir_path;
        }

        /// <summary>
        /// Создает или возвращает поддиректорию если она существует
        /// </summary>
        /// <param name="dir_path"></param>
        /// <param name="dir_name"></param>
        /// <returns></returns>
        public static DirectoryInfo CreateOrReturnSubDirectory(this DirectoryInfo dir_path,string dir_name)
        {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(dir_path.FullName, dir_name));
            return dir.CreateOrReturn();
        }

        /// <summary>
        /// Создает или возвращает директорию если она существует
        /// </summary>
        /// <param name="dir_path"></param>
        /// <returns></returns>
        public static DirectoryInfo CreateOrReturn(this DirectoryInfo dir_path,ref bool Create)
        {
            if (!dir_path.Exists)
            {
                dir_path.Create();
                Create = true;
            }
            return dir_path;
        }

        /// <summary>
        /// Создает или возвращает поддиректорию если она существует
        /// </summary>
        /// <param name="dir_path"></param>
        /// <param name="dir_name"></param>
        /// <returns></returns>
        public static DirectoryInfo CreateOrReturnSubDirectory(this DirectoryInfo dir_path, string dir_name,ref bool Create)
        {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(dir_path.FullName, dir_name));
            return dir.CreateOrReturn(ref Create);
        }


        /// <summary>
        /// Проверяет пуста ли папка
        /// </summary>
        /// <param name="dir_path">папка</param>
        /// <returns></returns>
        public static bool IsEmpty(this DirectoryInfo dir_path)
        {
            if (dir_path.GetFiles().Count() > 0)
                return false;
            if (dir_path.GetDirectories().Count() > 0)
                return false;
            return true;
        }


        /// <summary>
        /// Ищет файлы в папке
        /// </summary>
        /// <param name="dir_path">папка</param>
        /// <param name="recursive">поиск в подпапках</param>
        /// <returns></returns>
        public static List<FileInfo> FindFiles(this DirectoryInfo dir_path,string filename, bool recursive)
        {
            List<FileInfo> list = new List<FileInfo>();
            if (recursive)
            {
                list = dir_path.GetFiles("*.*", SearchOption.AllDirectories).ToList().Where(a => a.Name.ToLower() == filename.ToLower()).ToList();
            }
            else
            {
                list = dir_path.GetFiles("*.*", SearchOption.TopDirectoryOnly).ToList().Where(a => a.Name.ToLower() == filename.ToLower()).ToList();
            }

            if (list == null || list.Count == 0)
                return null;
            else
                return list;
        }

        /// <summary>
        /// Ищет первый файл в папке
        /// </summary>
        /// <param name="dir_path">папка</param>
        /// <param name="recursive">поиск в подпапках</param>
        /// <returns></returns>
        public static FileInfo FindFile(this DirectoryInfo dir_path, string filename, bool recursive)
        {
            List<FileInfo> list = new List<FileInfo>();
            if (recursive)
            {
                list = dir_path.GetFiles("*.*",SearchOption.AllDirectories).ToList().Where(a => a.Name.ToLower() == filename.ToLower()).ToList();
            }
            else
            {
                list = dir_path.GetFiles("*.*", SearchOption.TopDirectoryOnly).ToList().Where(a => a.Name.ToLower() == filename.ToLower()).ToList();
            }
             
            if (list == null || list.Count == 0)
                return null;
            else
                return list.First();
        }

        /// <summary>
        /// Ищет первую папку в папке
        /// </summary>
        /// <param name="dir_path">папка</param>
        /// <param name="recursive">поиск в подпапках</param>
        /// <returns></returns>
        public static DirectoryInfo FindDirectory(this DirectoryInfo dir_path, string foldername, bool recursive)
        {
            try
            {
                foreach (DirectoryInfo dir in dir_path.GetDirectories().ToList())
                {
                    try
                    {
                        if (dir.Name.ToLower() == foldername.ToLower())
                        {
                            return dir;
                        }
                        else
                        {
                            if (recursive)
                            {
                                DirectoryInfo dir1 = dir.FindDirectory(foldername, recursive);
                                if (dir1 != null)
                                    return dir1;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            #region trash
            //List<DirectoryInfo> list = new List<DirectoryInfo>();
            //if (recursive)
            //{
            //    list = dir_path.GetDirectories("*", SearchOption.AllDirectories).ToList()
            //        .Where(a => a.Name.ToLower() == foldername.ToLower()).ToList();
            //}
            //else
            //{
            //    list = dir_path.GetDirectories("*", SearchOption.TopDirectoryOnly).ToList()
            //        .Where(a => a.Name.ToLower() == foldername.ToLower()).ToList();
            //}

            //if (list == null || list.Count == 0)
            //    return null;
            //else
            //    return list.First();
            #endregion
        }

        /// <summary>
        /// Ищет все папки в папке
        /// </summary>
        /// <param name="dir_path">папка</param>
        /// <param name="recursive">поиск в подпапках</param>
        /// <returns></returns>
        public static List<DirectoryInfo> FindDirectories(this DirectoryInfo dir_path, string foldername, bool recursive)
        {
            List<DirectoryInfo> list = new List<DirectoryInfo>();

            foreach (DirectoryInfo dir in dir_path.GetDirectories().ToList())
            {
                try
                {
                    if (dir.Name.ToLower() == foldername.ToLower())
                    {
                        list.Add(dir);
                    }
                    else
                    {
                        if (recursive)
                        {
                            List<DirectoryInfo> dir1 = dir.FindDirectories(foldername, recursive);
                            if (dir1 != null)
                                list.AddRange(dir1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            if (list.Count == 0)
                return null;
            else return list;
            #region trash
            //if (recursive)
            //{
            //    list = dir_path.GetDirectories("*", SearchOption.AllDirectories).ToList()
            //        .Where(a => a.Name.ToLower() == foldername.ToLower()).ToList();
            //}
            //else
            //{
            //    list = dir_path.GetDirectories("*", SearchOption.TopDirectoryOnly).ToList()
            //        .Where(a => a.Name.ToLower() == foldername.ToLower()).ToList();
            //}

            //if (list == null || list.Count == 0)
            //    return null;
            //else
            //    return list;
            #endregion
        }

        /// <summary>
        /// Копирует внутренние файлы в папку
        /// </summary>
        /// <param name="dir_path"></param>
        /// <param name="folder_path"></param>
        public static void CopyInternalFilesToDir(this DirectoryInfo dir_path,string folder_path)
        {
            List<FileInfo> file_list = dir_path.GetFiles().ToList();
            DirectoryInfo destFolder=folder_path.CreateOrReturnDirectory();
            file_list.ForEach(a => a.CopyTo(destFolder.FullName));
        }


        /// <summary>
        /// Возвращает путь к указанному файлу в папке
        /// </summary>
        /// <param name="dir_path">директория</param>
        /// <param name="file_name">искомый файл</param>
        public static string PFileIn(this DirectoryInfo dir_path, string file_name)
        {
            return Path.Combine(dir_path.FullName, file_name);
        }

        /// <summary>
        /// Перемещает внутренние файлы в папку c перезаписью
        /// </summary>
        /// <param name="dir_path"></param>
        /// <param name="folder_path"></param>
        public static void MoveInternalFilesToDir(this DirectoryInfo dir_path, string folder_path)
        {
            List<FileInfo> file_list = dir_path.GetFiles().ToList();
            DirectoryInfo destFolder = folder_path.CreateOrReturnDirectory();
            file_list.ForEach(a =>
            {
                try
                {
                    File.Copy(a.FullName, Path.Combine(folder_path, a.Name), true);
                    a.Delete();
                }catch(Exception ex){
                }
            });
        }

        /// <summary>
        /// Копирует внутренние файлы в папку
        /// </summary>
        /// <param name="dir_path"></param>
        /// <param name="folder_path"></param>
        /// <param name="flag">Показывает перезаписывать ли файлы</param>
        public static void CopyInternalFilesToDir(this DirectoryInfo dir_path, string folder_path,bool flag)
        {
            List<FileInfo> file_list = dir_path.GetFiles().ToList();
            DirectoryInfo destFolder = folder_path.CreateOrReturnDirectory(flag);
            file_list.ForEach(a => a.CopyTo(Path.Combine(destFolder.FullName,a.Name)));
        }

        /// <summary>
        /// Вернуть алиас или ip ресурса
        /// </summary>
        /// <param name="dir_path"></param>
        /// <returns></returns>
        public static string GetGlobalRoot(this DirectoryInfo dir_path)
        {
            DirectoryInfo d = dir_path.Root;
            if (d == null) d = dir_path;
            
            string result=d.FullName.Substring(0,d.FullName.IndexOf(@"\",3));

            return result;
        }

        /// <summary>
        /// Перемещает внутренние файлы в папку
        /// </summary>
        /// <param name="dir_path"></param>
        /// <param name="folder_path"></param>
        /// <param name="flag">Показывает перезаписывать ли файлы</param>
        public static void MoveInternalFilesToDir(this DirectoryInfo dir_path, string folder_path,bool flag)
        {
            List<FileInfo> file_list = dir_path.GetFiles().ToList();
            DirectoryInfo destFolder = folder_path.CreateOrReturnDirectory(flag);
            file_list.ForEach(a => a.MoveTo(destFolder.FullName));
        }

        /// <summary>
        /// Находит папку родитель с указанными именем
        /// </summary>
        /// <param name="dir_path"></param>
        /// <param name="dirTofind"></param>
        /// <returns></returns>
        public static DirectoryInfo FindParentDir(this DirectoryInfo dir_path, string dirTofind)
        {
            DirectoryInfo dir=dir_path.Parent;
            int i = 0;
            while (dir != null)
            {
                MDIParentMain.Log(String.Format("Ищем директорию {0} находимся в {1}",dirTofind,dir.Name));
                i++;
                if (dir.Name.ToLower() == dirTofind.ToLower())
                {
                    break;
                }
                dir = dir.Parent;
                if (i == 100) return null;
            }
            return dir;
        }

        /// <summary>
        /// Очистить директорию
        /// </summary>
        /// <param name="dir">отчищаемая папка</param>
        public static void ClearDirectory(this DirectoryInfo dir)
        {
            dir.DeleteOldFilesInDir(0);
            dir.GetDirectories().ToList().ForEach(a => a.Delete(true));
        }

        /// <summary>
        /// Удаляет из папки старые файлы
        /// </summary>
        /// <param name="dir_path">Папка</param>
        /// <param name="desired_folder_size">Желаемый размер в байтах</param>
        public static void DeleteOldFilesInDir(this DirectoryInfo dir_path, int desired_folder_size)
        {
            List<FileInfo> fi_list = dir_path.GetFiles().ToList();
            long folder_size=0;
            fi_list.ForEach(a => folder_size += a.Length);

            if (folder_size > desired_folder_size)
            {
                fi_list.Sort((a, b) => a.CreationTime.CompareTo(b.CreationTime));
                foreach(FileInfo c in fi_list)
                {
                    c.Delete(); fi_list = dir_path.GetFiles().ToList();
                    folder_size = 0;
                    fi_list.ForEach(a => folder_size += a.Length);
                    if (folder_size < desired_folder_size) break;
                }
            }
            
        }

        /// <summary>
        /// Проверяем, что в директориях одинаковые файлы
        /// </summary>
        /// <param name="dir1"></param>
        /// <param name="dir2"></param>
        /// <returns></returns>
        public static bool FolderHasSameFiles(this DirectoryInfo dir1, DirectoryInfo dir2)
        {
            List<FileInfo> fl1 = dir1.GetFiles().ToList(); fl1.Sort((a, b) => a.Name.CompareTo(b.Name));
            List<FileInfo> fl2 = dir2.GetFiles().ToList(); fl2.Sort((a, b) => a.Name.CompareTo(b.Name));

            if (fl1.Count != fl2.Count) return false;

            for (int i = 0; i < fl1.Count; i++)
            {
                if (fl1[i].Name != fl2[i].Name) return false;
            }

            return true;
        }

        /// <summary>
        /// Возвращает или создает нужную директорию в исходной
        /// </summary>
        /// <param name="dir_name"></param>
        /// <returns></returns>
        public static DirectoryInfo GetDecendantDirectory(this DirectoryInfo parent_dir, string dir_name)
        {
            DirectoryInfo s_treport_dir = new DirectoryInfo(Path.Combine(parent_dir.FullName, dir_name)).CreateOrReturn();
            return s_treport_dir;
        }

        /// <summary>
        /// копирует содержимое указанной директории в директорию
        /// </summary>
        /// <param name="dest_folder">папка назначения</param>
        /// <param name="old_dir"></param>
        public static void CopyDirectory(this DirectoryInfo old_dir,string dest_folder)
        {
            old_dir.GetFiles("*.*", SearchOption.AllDirectories).ToList().ForEach(b =>
            {
                new CustomAction((oo) =>
                {
                    string rel_path =
                        b.FullName.Substring(b.FullName.IndexOf(old_dir.FullName) + old_dir.FullName.Length);
                    string folders = "";
                    DebugPanel.getFileName(rel_path, ref folders, dest_folder);
                    string file_path = dest_folder + rel_path;
                    b.CopyTo(file_path, true);
                }, null).Start();
            });
        }


        #endregion

        #region String Extensions

        /// <summary>
        /// Возвращает или создает директорию
        /// </summary>
        /// <param name="dir_path">путь</param>
        /// <returns></returns>
        public static DirectoryInfo CreateOrReturnDirectory(this string dir_path)
        {
            DirectoryInfo dir = new DirectoryInfo(dir_path);
            if (!dir.Exists)
                dir.Create();
            return dir;
        }

        /// <summary>
        /// Возвращает или создает директорию
        /// </summary>
        /// <param name="dir_path">путь</param>
        /// <param name="clearDir">если директория не пустая, то удалить все файлы из нее</param>
        /// <returns></returns>
        public static DirectoryInfo CreateOrReturnDirectory(this string dir_path,bool clearDir)
        {
            DirectoryInfo dir = new DirectoryInfo(dir_path);
            if (!dir.Exists)
                dir.Create();
            else
            {
                if (clearDir)
                {
                    dir.Delete(true);
                    dir = dir_path.CreateOrReturnDirectory();
                }
            }
            return dir;
        }

        /// <summary>
        /// Удаляет файл если он существует
        /// </summary>
        /// <param name="file_name"></param>
        public static void DeleteFileIfExist(this string file_name)
        {
            FileInfo fi=new FileInfo(file_name);
            if (fi.Exists)
            {
                fi.Delete();
            }
        }

        /// <summary>
        /// Вырезает из файла символы после .
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string BaseFileName(this string fileName)
        {
            string baseFileName = fileName;
            if (fileName.LastIndexOf(".") != -1)
            {
                baseFileName=fileName.Substring(0,fileName.LastIndexOf("."));
            }
            return baseFileName;
        }

        /// <summary>
        /// Проверяет строку на пустоту
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool NotNullOrEmpty(this string str)
        {
            if (str != null && str != "")
                return true;
            return false;
        }

        /// <summary>
        /// Возвращает или строку или пустоту
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReturnOrEmpty(this string str)
        {
            if (str==null)
            {
                return "";
            }
            return str;
        }
        #endregion

        #region FileInfo Extensions 

        /// <summary>
        /// Переименует файл
        /// </summary>
        /// <param name="file">файл</param>
        /// <param name="new_filename">новое имя</param>
        public static void RenameFile(this FileInfo file, string new_filename)
        {
            DirectoryInfo dir = file.Directory;
            file.CopyTo(Path.Combine(dir.FullName, new_filename), true);
            file.Delete();
        }

        /// <summary>
        /// Возвращает имя файла без расширения
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string BaseFileName(this FileInfo file)
        {
            string baseFileName = file.Name.Replace(file.Extension, "");
            return baseFileName;
        }

        /// <summary>
        /// Проверяет файлы на равенство
        /// </summary>
        /// <param name="file"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        public static bool IsFilesEqual(this FileInfo file,FileInfo file2)
        {
            bool flag = false;
            if (!file2.Exists) return flag;
            if (file.Name == file2.Name && file.Length == file2.Length)
                flag = true;
            return flag;
        }


        /// <summary>
        /// Перемещает файл с заменой
        /// </summary>
        /// <param name="file">заменяемый файл</param>
        /// <param name="path_to_folder">путь к папке</param>
        public static void MoveWithReplase(this FileInfo file, string path_to_folder)
        {
            if (path_to_folder == file.Directory.FullName) return;
            DirectoryInfo dir = new DirectoryInfo(path_to_folder);
            if (!dir.Exists) throw new ArgumentException("Не существует указанной папки", path_to_folder);

            string dest_file = Path.Combine(path_to_folder, file.Name);
            if (File.Exists(dest_file))
            {
                File.Delete(dest_file);
            }
                File.Move(file.FullName, dest_file);
        }

        public static void RemoteCopyFileWithDirCreation(this FileInfo b,
            DirectoryInfo packageFolder,
            DirectoryInfo kkm_package_directory)
        {
            string rel_path =
              b.FullName.Substring(b.FullName.IndexOf(packageFolder.Name)+ packageFolder.Name.Length+1);
            string folders = "";
            DebugPanel.getFileName(rel_path, ref folders, kkm_package_directory.FullName);
            string file_path = Path.Combine(kkm_package_directory.FullName, rel_path);
            b.CopyTo(file_path, true);
        }

        /// <summary>
        /// Заменяет путь рута на указанный
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReplaceRootDirectoryOnDisk(this FileInfo file, string path)
        {
            string st=file.FullName;
            return st.Replace(file.Directory.Root.FullName, path);
        }

        #endregion

        #region Dictionary Extensions

        /// <summary>
        /// Проверяет по ключу есть ли в словаре данный элемент, если нет то создает
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="Z"></typeparam>
        /// <param name="dict"></param>
        /// <param name="tkey"></param>
        /// <param name="arg1"></param>
        /// <param name="makeU"></param>
        /// <returns></returns>
        public static U CreateOrreturnElement<T,U,Z>(this Dictionary<T,U> dict,T tkey,Z arg1,Func<Z,U> makeU) 
            where U : new()
        {
            if (dict.ContainsKey(tkey))
            {
                return dict[tkey];
            }
            else
            {
                U new_elem = makeU(arg1);
                dict.Add(tkey, new_elem);
                return new_elem;
            }
        }

        #endregion

        #region List<T> extension
        public static void AddRangeNotNull<T>(this List<T> collect,IEnumerable<T> col1)
        {
            if (col1.NotNullOrEmpty())
            {
                collect.AddRange(col1);
            }
        }
        #endregion

        #region IEnumerable Extension
        public static TSource WhereFirst<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) 
        {
            if (source.Where(predicate).Count() > 0)
            {
                return source.Where(predicate).First();
            }
            else
            {
                return default(TSource);
            }
        }

        /// <summary>
        /// Определяет не пустая ли коллекция
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool NotNullOrEmpty<T>(this IEnumerable<T> collection)// where T : object
        {
            if (collection != null && collection.Count() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Безопасный foreach позволяющий удалять данные из себя же
        /// </summary>
        /// <typeparam name="TSource">тип коллекции</typeparam>
        /// <param name="source">коллекция</param>
        /// <param name="predicate">действие</param>
        //public static void ForEachSafe<TSource>(this IEnumerable<TSource> source, Action<TSource> predicate)
        //{
        //    if (source == null || source.Count() == 0) return;
            
        //    IEnumerable<TSource> source1=
        //    if (source.Where(predicate).Count() > 0)
        //    {
        //        return source.Where(predicate).First();
        //    }
        //    else
        //    {
        //        return default(TSource);
        //    }
        //}
        #endregion

        #region Datagridview Extension
        
            public static void DoubleBuffered(this DataGridView dgv, bool setting)
            {
                Type dgvType = dgv.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(dgv, setting, null);
            }
       
        #endregion

        #region MailMessage
            private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
            private static readonly Type MailWriter = typeof(SmtpClient).Assembly.GetType("System.Net.Mail.MailWriter");
            private static readonly ConstructorInfo MailWriterConstructor = MailWriter.GetConstructor(Flags, null, new[] { typeof(Stream) }, null);
            private static readonly MethodInfo CloseMethod = MailWriter.GetMethod("Close", Flags);
            private static readonly MethodInfo SendMethod = typeof(MailMessage).GetMethod("Send", Flags);

            /// <summary>
            /// A little hack to determine the number of parameters that we
            /// need to pass to the SaveMethod.
            /// </summary>
            private static readonly bool IsRunningInDotNetFourPointFive = SendMethod.GetParameters().Length == 3;

            /// <summary>
            /// The raw contents of this MailMessage as a MemoryStream.
            /// </summary>
            /// <param name="self">The caller.</param>
            /// <returns>A MemoryStream with the raw contents of this MailMessage.</returns>
            public static MemoryStream RawMessage(this MailMessage self)
            {
                var result = new MemoryStream();
                var mailWriter = MailWriterConstructor.Invoke(new object[] { result });
                SendMethod.Invoke(self, Flags, null, IsRunningInDotNetFourPointFive ? new[] { mailWriter, true, true } : new[] { mailWriter, true }, null);
                result = new MemoryStream(result.ToArray());
                CloseMethod.Invoke(mailWriter, Flags, null, new object[] { }, null);
                return result;
            }
        #endregion
    }
}
