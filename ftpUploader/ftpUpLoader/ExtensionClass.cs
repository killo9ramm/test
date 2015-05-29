using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

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
            file_list.ForEach(a => a.CopyTo(destFolder.FullName));
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
                i++;
                if (dir.Name.ToLower() == dirTofind.ToLower())
                {
                    break;
                }
                dir = dir_path.Parent;
                if (i == 100) return null;
            }
            return dir;
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
        #endregion

        #region FileInfo Extensions
        
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
        /// Определяет не пустая ли коллекция
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool NotNullOrEmpt(this ICollection collection)// where T : object
        {
            if (collection != null && collection.Count > 0)
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

    }
}
