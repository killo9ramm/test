using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RBClient.Classes
{
	public static class ExtensionClass
	{
		public static string BaseFileName(this string fileName)
		{
			string str = fileName;
			if (fileName.LastIndexOf(".") != -1)
			{
				str = fileName.Substring(0, fileName.LastIndexOf("."));
			}
			return str;
		}

		public static string BaseFileName(this FileInfo file)
		{
			return file.Name.Replace(file.Extension, "");
		}

		public static void CopyInternalFilesToDir(this DirectoryInfo dir_path, string folder_path)
		{
			List<FileInfo> list = dir_path.GetFiles().ToList<FileInfo>();
			DirectoryInfo directoryInfo = folder_path.CreateOrReturnDirectory();
			list.ForEach((FileInfo a) => a.CopyTo(directoryInfo.FullName));
		}

		public static void CopyInternalFilesToDir(this DirectoryInfo dir_path, string folder_path, bool flag)
		{
			List<FileInfo> list = dir_path.GetFiles().ToList<FileInfo>();
			DirectoryInfo directoryInfo = folder_path.CreateOrReturnDirectory(flag);
			list.ForEach((FileInfo a) => a.CopyTo(directoryInfo.FullName));
		}

		public static DirectoryInfo CreateOrReturn(this DirectoryInfo dir_path)
		{
			if (!dir_path.Exists)
			{
				dir_path.Create();
			}
			return dir_path;
		}

		public static DirectoryInfo CreateOrReturnDirectory(this string dir_path)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(dir_path);
			if (!directoryInfo.Exists)
			{
				directoryInfo.Create();
			}
			return directoryInfo;
		}

		public static DirectoryInfo CreateOrReturnDirectory(this string dir_path, bool clearDir)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(dir_path);
			if (!directoryInfo.Exists)
			{
				directoryInfo.Create();
			}
			else if (clearDir)
			{
				directoryInfo.Delete(true);
				directoryInfo = dir_path.CreateOrReturnDirectory();
			}
			return directoryInfo;
		}

		public static U CreateOrreturnElement<T, U, Z>(this Dictionary<T, U> dict, T tkey, Z arg1, Func<Z, U> makeU)
		where U : new()
		{
			if (dict.ContainsKey(tkey))
			{
				return dict[tkey];
			}
			U u = makeU(arg1);
			dict.Add(tkey, u);
			return u;
		}

		public static void DeleteFileIfExist(this string file_name)
		{
			FileInfo fileInfo = new FileInfo(file_name);
			if (fileInfo.Exists)
			{
				fileInfo.Delete();
			}
		}

		public static void DeleteOldFilesInDir(this DirectoryInfo dir_path, int desired_folder_size)
		{
			List<FileInfo> list = dir_path.GetFiles().ToList<FileInfo>();
			long length = (long)0;
			list.ForEach((FileInfo a) => length = length + a.Length);
			if (length > (long)desired_folder_size)
			{
				list.Sort((FileInfo a, FileInfo b) => a.CreationTime.CompareTo(b.CreationTime));
				List<FileInfo>.Enumerator enumerator = list.GetEnumerator();
				try
				{
					do
					{
						if (!enumerator.MoveNext())
						{
							break;
						}
						enumerator.Current.Delete();
						list = dir_path.GetFiles().ToList<FileInfo>();
						length = (long)0;
						list.ForEach((FileInfo a) => length = length + a.Length);
					}
					while (length >= (long)desired_folder_size);
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
			}
		}

		public static DirectoryInfo FindParentDir(this DirectoryInfo dir_path, string dirTofind)
		{
			DirectoryInfo parent = dir_path.Parent;
			int num = 0;
			while (parent != null)
			{
				num++;
				if (parent.Name.ToLower() == dirTofind.ToLower())
				{
					break;
				}
				parent = dir_path.Parent;
				if (num != 100)
				{
					continue;
				}
				return null;
			}
			return parent;
		}

		public static bool FolderHasSameFiles(this DirectoryInfo dir1, DirectoryInfo dir2)
		{
			List<FileInfo> list = dir1.GetFiles().ToList<FileInfo>();
			list.Sort((FileInfo a, FileInfo b) => a.Name.CompareTo(b.Name));
			List<FileInfo> fileInfos = dir2.GetFiles().ToList<FileInfo>();
			fileInfos.Sort((FileInfo a, FileInfo b) => a.Name.CompareTo(b.Name));
			if (list.Count != fileInfos.Count)
			{
				return false;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Name != fileInfos[i].Name)
				{
					return false;
				}
			}
			return true;
		}

		public static DirectoryInfo GetDecendantDirectory(this DirectoryInfo parent_dir, string dir_name)
		{
			DirectoryInfo directoryInfo = (new DirectoryInfo(Path.Combine(parent_dir.FullName, dir_name))).CreateOrReturn();
			return directoryInfo;
		}

		public static bool IsFilesEqual(this FileInfo file, FileInfo file2)
		{
			bool flag = false;
			if (!file2.Exists)
			{
				return flag;
			}
			if (file.Name == file2.Name && file.Length == file2.Length)
			{
				flag = true;
			}
			return flag;
		}

		public static void MoveInternalFilesToDir(this DirectoryInfo dir_path, string folder_path)
		{
			List<FileInfo> list = dir_path.GetFiles().ToList<FileInfo>();
			folder_path.CreateOrReturnDirectory();
			list.ForEach((FileInfo a) => {
				try
				{
					File.Copy(a.FullName, Path.Combine(folder_path, a.Name), true);
					a.Delete();
				}
				catch (Exception exception)
				{
				}
			});
		}

		public static void MoveInternalFilesToDir(this DirectoryInfo dir_path, string folder_path, bool flag)
		{
			List<FileInfo> list = dir_path.GetFiles().ToList<FileInfo>();
			DirectoryInfo directoryInfo = folder_path.CreateOrReturnDirectory(flag);
			list.ForEach((FileInfo a) => a.MoveTo(directoryInfo.FullName));
		}

		public static TSource WhereFirst<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			if (source.Where<TSource>(predicate).Count<TSource>() <= 0)
			{
				return default(TSource);
			}
			return source.Where<TSource>(predicate).First<TSource>();
		}
	}
}