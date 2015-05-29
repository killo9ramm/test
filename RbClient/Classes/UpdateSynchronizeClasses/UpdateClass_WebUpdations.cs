using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBClient.ru.teremok.msk;
using System.IO;
using RBClient.Classes.InternalClasses.Models;
using Debug_classes;

namespace RBClient.Classes.CustomClasses
{
    partial class UpdateClass
    {
        #region рекламные документы
        internal void ОбновитьРекламныеДокументы(object o)
        {
            Docs structure = (Docs)o;
            if (structure.Type == null) return;

            DirectoryInfo adv_dir = GetAdvDocDir();

            AddWebDocStructure(structure, adv_dir);
            RemoveWebDocStructure(structure, adv_dir);

            SyncronizeAdvertisment(adv_dir);
        }

        private void SyncronizeAdvertisment(DirectoryInfo adv)
        {
            DirectoryInfo video_dir = GetAdvVideoDir();
            DirectoryInfo image_dir = GetAdvImageDir();

            UpdateAdv(adv, video_dir, (o) => o.Extension == ".avi", VideoUpdateClass.sendVideoFilesToKkm);
            UpdateAdv(adv, image_dir, (o) => o.Extension != ".avi", ImageUpdateClass.sendVideoFilesToKkm);
        }

        public DirectoryInfo GetAdvDocDir()
        {
            return prepareDirectoryInRoot(StaticConstants.ADV_DOC_FOLDER, false);
        }

        private void UpdateAdv(DirectoryInfo advdir, DirectoryInfo destdir,Func<FileInfo,bool> filrter, Action kkmUpdation)
        {
            destdir.ClearDirectory();

            foreach (var a in advdir.GetFiles())
            {
                if (filrter(a))
                {
                    var o=StaticConstants.CBData._GetStringAssociation_(a.Name);
                    if(o!=null)
                    {
                        System.IO.File.Copy(a.FullName,
                           Path.Combine(destdir.FullName, o.prop_value + a.Extension));
                    }
                }
            }

            kkmUpdation();
        }

        private void UpdateImageAdv(Docs structure, Func<Docs, Docs> filter, DirectoryInfo dir, Action kkmUpdation)
        {
            Docs image_structure = filter(structure);
            AddWebDocStructure(image_structure, dir);
            RemoveWebDocStructure(image_structure, dir);
            kkmUpdation();
        }

        private Docs AdvVideoFilter(Docs structure)
        {
            Docs d=new Docs();
            d.Category=structure.Category;
            d.Docs1=structure.Docs1.Where(a=>a.FileName.EndsWith(".avi")).ToArray();
            foreach (var a in d.Docs1)
            {
                a.FileName = a.ScreenName + a.FileName.ExtensionFileName();
            }
            return d;
        }

        private Docs AdvImageFilter(Docs structure)
        {
            Docs d = new Docs();
            d.Category = structure.Category;
            d.Docs1 = structure.Docs1.Where(a => a.FileName.EndsWith(".jpg")).ToArray();
            foreach (var a in d.Docs1)
            {
                a.FileName = a.ScreenName+a.FileName.ExtensionFileName();
            }
            return d;
        }

        public DirectoryInfo GetAdvVideoDir()
        {
            return VideoUpdateClass.working_dir;
        }

        public DirectoryInfo GetAdvImageDir()
        {
            return ImageUpdateClass.working_dir;
        }
        #endregion
        #region внутренние документы
        public void ОбновитьВнутренниеДокументы(object o)
        {
            Docs structure = (Docs)o;

            if (structure.Type == null) return;

            DirectoryInfo video_dir = GetInnerDocsDir();

            AddWebDocStructure(structure, video_dir);

            RemoveWebDocStructure(structure, video_dir);

//          UnzipInnerDocsArchives(video_dir);

           // ConvertInnerDocsToHtml(video_dir);
        }

        //private void ConvertInnerDocsToHtml(DirectoryInfo video_dir)
        //{
        //    List<FileInfo> zipList = video_dir.GetFiles("*.doc*", SearchOption.AllDirectories).ToList();

        //    foreach (var a in zipList)
        //    {
        //        DirectoryInfo dir = new DirectoryInfo(a.FullName.Replace(a.Extension, ""));
        //        dir.CreateOrReturn();

        //        if (!WordAutomation.ConvertFromDocToHtml(a, dir))
        //        {
        //            throw new Exception("cannot convert to html "+a.FullName);
        //        }
        //    }
        //}

        private void UnzipInnerDocsArchives(DirectoryInfo video_dir)
        {
            List<FileInfo> zipList=video_dir.GetFiles("*.zip", SearchOption.AllDirectories).ToList();

            foreach (var a in zipList)
            {
                DirectoryInfo dir = new DirectoryInfo(a.FullName.Replace(a.Extension,""));
                dir.CreateOrReturn();
                
                ZipHelper.ExtractZipFile(a.FullName,null, dir.FullName);
            }
        }

        public DirectoryInfo GetInnerDocsDir()
        {
            return prepareDirectoryInRoot(StaticConstants.INNER_DOCS_FOLDER, false);
        }
#endregion

        #region Обучающее видео

        public void ОбновитьОбучающееВидео(object o)
        {
            try
            {
                Docs structure = (Docs)o;

                if (structure.Type == null) return;
                //получить папку обучающего видео
                DirectoryInfo video_dir = GetEducVideoDir();

                AddWebDocStructure(structure, video_dir);

                RemoveWebDocStructure(structure, video_dir);
            }
            catch (Exception ex)
            {
                Log("Не удалось обновить структуру обучающего видео!");
                throw ex;
            }
            finally
            {
                Dispose();
            }
        }

        private void AddWebDocStructure(Docs structure, DirectoryInfo video_dir)
        {
            foreach (var a in structure.Docs1.ToList())
            {
                if (IsDirectory(a))
                {
                    bool Create = false;
                    DirectoryInfo dir = video_dir.CreateOrReturnSubDirectory(ReturnDirSystemName(a),
                        ref Create);

                    //проверка если уже есть ассоциация с директорией
                    CheckAndCreateNewDirNameAssociation(a);
                    
                    if (a.Docs1!=null && a.Docs1.Length > 0)
                    {
                        AddWebDocStructure(a, dir);
                    }
                    continue;
                }
                if (IsFile(a))
                {
                    FileInfo fi = FindFile(video_dir,a);
                    if (fi == null || fi.Extension==".tmp")
                    {
                        CAction.CreateCA(() =>
                        {
                            fi = GetFileFromFtp(video_dir, a, false); //файл скопирован
                        }, "GetFileFromFtp "+a.FileName).Start();

                        if (fi == null)
                        {
                            Log("Ftp Файл " + a.FileName + " не загружен!");
                        }
                        else
                        {
                            CheckAndCreateNewNameAssociation(a);
                        }
                    }
                    else
                    {
                        CheckAndCreateNewNameAssociation(a);
                        if (fi.Directory.FullName != video_dir.FullName)
                        {
                            CAction.CreateCA(() =>
                            {
                                fi.CopyTo(Path.Combine(video_dir.FullName, a.FileName), true);
                            }, "Copy file "+a.FileName).Start();
                        }
                    }
                }
            }
        }

        private void RemoveWebDocStructure(Docs structure, DirectoryInfo video_dir)
        {
            List<Docs> sdocs=structure.Docs1.ToList();
            List<string> file_names = 
                sdocs.Where(a => IsFile(a)).Select(a => a.FileName).ToList();

            RemoveUnwantedFiles(file_names,video_dir);

            List<Docs> directories = sdocs.Where(a => IsDirectory(a)).ToList();

            List<string> new_file_elements_list=GetElementsList(structure);

            RemoveUnwantedDirs(directories, video_dir, new_file_elements_list);

            foreach (Docs directory in directories)
            {
                DirectoryInfo dir = video_dir.FindDirectory(ReturnDirSystemName(directory), false);
                if (dir != null)
                    RemoveWebDocStructure(directory, dir);
            }
        }

        private void RemoveUnwantedDirs(List<Docs> directories, DirectoryInfo video_dir, List<string> new_file_elements_list)
        {
            List<string> _names = directories
                .Where(a => IsDirectory(a)).Select(a => ReturnDirSystemName(a)).ToList();

            List<DirectoryInfo> _list = video_dir.GetDirectories().ToList();
            foreach (var a in _list)
            {
                if (!_names.Contains(a.Name))
                {
                    RemoveNameAssociation(a.Name);
                    var assocToDelete = a.GetFiles().Where(b => !new_file_elements_list.Contains(b.Name));
                    RemoveNameAssociations(assocToDelete.ToList());
                    a.Delete(true);
                }
            }
        }

        private List<string> GetElementsList(Docs structure)
        {
            List<string> ls=new List<string>();
            if(IsFile(structure))
            {
                ls.Add(structure.FileName);
            }else
            {
                if (structure.Docs1.NotNullOrEmpty())
                {
                    foreach (var a in structure.Docs1)
                    {
                        ls.AddRange(GetElementsList(a));
                    }
                }
            }
                return ls;
        }

        

        private void RemoveUnwantedDirs(List<Docs> directories, DirectoryInfo video_dir)
        {
            List<string> _names = directories
                .Where(a => IsDirectory(a)).Select(a =>ReturnDirSystemName(a)).ToList();

            List<DirectoryInfo> _list = video_dir.GetDirectories().ToList();
            foreach (var a in _list)
            {
                if (!_names.Contains(a.Name))
                {
                    RemoveNameAssociation(a.Name);
                    RemoveNameAssociations(a.GetFiles().ToList());
                    a.Delete(true);
                }
            }
        }

        private void RemoveNameAssociations(List<FileInfo> list)
        {
            foreach (var a in list)
            {
                RemoveNameAssociation(a.Name);
            }
        }

        private void RemoveNameAssociation(string a)
        {
            List<t_PropValue> prop = new t_PropValue().Select<t_PropValue>("Prop_name='" + a +
                    "' AND Prop_type='" + StaticConstants.Teremok_ID + "'");
            if(prop.NotNullOrEmpty())
            prop.ForEach(o => o.Delete());
        }

        private void RemoveUnwantedFiles(List<string> file_names, DirectoryInfo video_dir)
        {
            List<FileInfo> flist = video_dir.GetFiles().ToList();
            foreach (var a in flist)
            {
                if (!file_names.Contains(a.Name))
                {
                    RemoveNameAssociation(a.Name);
                    CAction.CreateCA(() =>
                    {
                        a.Delete();
                    }, "DeleteUnwantedFile "+a.Name).Start();
                }
            }
        }

        private string ReturnFileName(Docs a)
        {
            FileInfo f = new FileInfo(a.FileName);
            return a.ScreenName + f.Extension;
        }

        private FileInfo GetFileFromFtp(DirectoryInfo video_dir, Docs a,bool deleteOnFtp)
        {
            //скачать файл
            string repositFolder=GetRepositoryFolder();
            DownloadHelper.Logg1 = Log;
            DownloadHelper.StartDownload(repositFolder, a.FileName, video_dir,deleteOnFtp);

            FileInfo f = video_dir.FindFile(a.FileName, false);
            return f;
        }

        private void CreateNewDirNameAssociation(Docs a)
        {
            string name = ReturnDirSystemName(a);

            t_PropValue prop = new t_PropValue();
            prop.prop_name = name;
            prop.prop_type = StaticConstants.Teremok_ID;
            prop.add_prop1 = "dir";
            prop.add_prop2 = a.SortOrder.ToString();
            prop.prop_value = a.ScreenName;
            prop.Create();
        }

        private void CheckAndCreateNewDirNameAssociation(Docs a)
        {
            string name = ReturnDirSystemName(a);

            var o = new t_PropValue().SelectFirst<t_PropValue>(
                //String.Format("Prop_name='{0}' And Prop_type='{1}'", name, StaticConstants.Teremok_ID));
                String.Format("Prop_name='{0}'", name));

            if (o == null)
            {
                CreateNewDirNameAssociation(a);
            }
            else
            {
                bool changed = false;
                if (o.prop_value != a.ScreenName)
                {
                    o.prop_value = a.ScreenName;
                    changed=true;
                }
                if (o.add_prop2 != a.SortOrder.ToString())
                {
                    o.add_prop2 = a.SortOrder.ToString();
                    changed=true;
                }
                if(changed)
                {
                    o.Update();
                }
            }
        }

        private string ReturnDirSystemName(Docs a)
        {
            return a.Guid;
        }

        //private string ReturnDirSystemName(string dirScreemName)
        //{
        //    return Hashing.GetMd5Hash(dirScreemName);
        //}

        private void CreateNewNameAssociation(Docs a)
        {
            t_PropValue prop = new t_PropValue();
            prop.prop_name = a.FileName;
            prop.prop_type = StaticConstants.Teremok_ID;
            prop.prop_value = a.ScreenName;
            prop.add_prop2 = a.SortOrder.ToString();
            prop.Create();
        }

        private void CheckAndCreateNewNameAssociation(Docs a)
        {
            var o = new t_PropValue().SelectFirst<t_PropValue>(
                //String.Format("Prop_name='{0}' And Prop_type='{1}'", a.FileName, StaticConstants.Teremok_ID));
                String.Format("Prop_name='{0}'", a.FileName));

            if (o == null)
            {
                CreateNewNameAssociation(a);
            }
            else
            {
                bool changed = false;
                if (o.prop_value != a.ScreenName)
                {
                    o.prop_value = a.ScreenName;
                    changed = true;
                }
                if (o.add_prop2 != a.SortOrder.ToString())
                {
                    o.add_prop2 = a.SortOrder.ToString();
                    changed = true;
                }
                if (changed)
                {
                    o.Update();
                }
            }
        }

        public string GetRepositoryFolder()
        {
            string repo_path=
                StaticConstants.ReturnConfValue<string>("education_video_repository","_repository");
            return repo_path;
        }

        public FileInfo FindFile(DirectoryInfo video_dir, Docs a)
        {
            //FileInfo fi = FindFile(video_dir, a.ScreenName, (long)a.Size, false);
            FileInfo fi = FindFile(video_dir, a.FileName, (long)a.Size, false);
            if (fi == null)
            {
                DirectoryInfo _dir = GetEducVideoDir();
                //fi = FindFile(_dir, a.ScreenName, (long)a.Size, true);
                fi = FindFile(_dir, a.FileName, (long)a.Size, true);
            }
            return fi;
        }

        public DirectoryInfo GetEducVideoDir(){
            return prepareDirectoryInRoot(StaticConstants.EDUC_VIDEO_FOLDER, false);
        }
        
        public FileInfo FindFile(DirectoryInfo dir_path, string filename, long fileSize, bool recursive)
        {
            List<FileInfo> list = new List<FileInfo>();

            if (recursive)
            {
                list = dir_path.GetFiles("*.*", SearchOption.AllDirectories).ToList().Where(a =>{
                    return ((a.Name.IndexOf(filename)!= -1) &&
                        (a.Length == fileSize));
                }).ToList();
            }
            else
            {
                list = dir_path.GetFiles("*.*", SearchOption.TopDirectoryOnly).ToList().Where(a =>
                {
                    return ((a.Name.IndexOf(filename) != -1) &&
                        (a.Length == fileSize));
                }).ToList();
            }

            if (list == null || list.Count == 0)
                return null;
            else
                return list.First();
        }

        private bool IsFile(Docs a)
        {
            if (a.Type == "Element")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsDirectory(Docs a)
        {
            if (a.Type == "Category")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
