using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using TinyMetroWpfLibrary.LogUtil;
namespace TinyMetroWpfLibrary.Utility
{
    public static class IOHelper
    {
        private static ILogService _logger = new FileLogService(typeof(IOHelper));

        public static bool MoveDirectory(string src, string dest)
        {
            if (string.IsNullOrWhiteSpace(src) || string.IsNullOrWhiteSpace(dest))
            {
                _logger.Warn("Src or Dest directory is invalid, can't move.");
                return false;
            }
            if (!Directory.Exists(src))
            {
                _logger.Warn("Src directory doesn't exist.");
                return false;
            }
            if (Path.GetFullPath(src).Equals(Path.GetFullPath(dest)))
            {
                _logger.Info("The Src and Dest directories are the same.");
                return true;
            }
            CopyDirectory(src, dest);
            DeleteDirectoryCheck(src);
            _logger.Info(string.Format("Directory moved successfully from {0} to {1}", src, dest));
            return true;
        }
        public static bool DeleteDirectoryCheck(string dir)
        {
            try
            {
                if (!Directory.Exists(dir))
                {
                    return true;
                }
                Directory.Delete(dir, true);
                int counter = 0;
                while (Directory.Exists(dir))
                {
                    Thread.Sleep(1);
                    if (++counter >= 1000)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.Assert(false); 
                _logger.Error(null, ex);
                return false;
            }
            
        }
        public static void CreateDirectoryCheck(string dir)
        {
            try
            {
                Directory.CreateDirectory(dir);
                int counter = 0;
                while (!Directory.Exists(dir))
                {
                    Thread.Sleep(1);
                    if (++counter >= 1000)
                    {
                        break;
                    }
                }
                _logger.Info(string.Format("Directory created: {0}", dir));
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.Assert(false);
                _logger.Error(null, ex);
            }
           
        }
        public static bool DeleteFileCheck(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                {
                    return true;
                }
                File.Delete(filePath);
                int counter = 0;
                while (File.Exists(filePath))
                {
                    Thread.Sleep(1);
                    if (++counter >= 1000)
                    {
                        return false;
                    }
                }
                _logger.Info(string.Format("File deleted: {0}", filePath));
                return true;
            }
            catch(Exception ex)
            {
                //System.Diagnostics.Debug.Assert(false); 
                _logger.Error(null, ex);
                return false;
            }
        }

        public static bool DeleteFile(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                {
                    return true;
                }
                File.Delete(filePath);
                _logger.Info(string.Format("DeleteFile Success filePath:{0}", filePath));
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("DeleteFile", ex);
                return false;
            }
        }

        public static void CreateFileCheck(string filePath)
        {
            try
            {
                File.Create(filePath).Close();
                int counter = 0;
                while (!File.Exists(filePath))
                {
                    Thread.Sleep(1);
                    if (++counter >= 1000)
                    {
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.Assert(false);
                _logger.Error(null, ex);
            }
          
        }
        public static FileInfo GetOnlyFile(string parentDir, string regexPattern = null)
        {
            if (string.IsNullOrWhiteSpace(parentDir) || !Directory.Exists(parentDir))
            {
                return null;
            }
            DirectoryInfo di = new DirectoryInfo(parentDir);
            foreach (var item in di.EnumerateFiles())
            {
                
                if (string.IsNullOrWhiteSpace(regexPattern))
                {
                    return item;
                }
                if (Regex.IsMatch(item.Name, regexPattern))
                {
                    return item;
                }
            }
            return null;
        }
        public static DirectoryInfo GetOnlyDirectory(string parentDir, string exceptNamePattern = null)
        {
            if (string.IsNullOrWhiteSpace(parentDir) || !Directory.Exists(parentDir))
            {
                return null;
            }
            DirectoryInfo di = new DirectoryInfo(parentDir);
            foreach (var item in di.EnumerateDirectories())
            {
                if (string.IsNullOrEmpty(exceptNamePattern) || !Regex.IsMatch(item.Name, exceptNamePattern))
                {
                    return item;
                }
            }
            return null;
        }
        public static void ClearDirectory(string dir)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dir))
                {
                    return;
                }
                DirectoryInfo di = new DirectoryInfo(dir);
                if (!di.Exists)
                {
                    return;
                }
                foreach (var item in di.EnumerateDirectories())
                {
                    //item.Delete(true);
                    DeleteDirectoryCheck(item.FullName);
                }
                foreach (var item in di.EnumerateFiles())
                {
                    //item.Delete();
                    DeleteFileCheck(item.FullName);
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.Assert(false);
                _logger.Error(null, ex);
            }
           
        }
        public static void CreateDirectory(string directory)
        {
            try
            {
                string dirpath = directory.Substring(0, directory.LastIndexOf('\\'));
                string[] pathes = dirpath.Split('\\');
                if (pathes.Length > 1)
                {
                    string path = pathes[0];
                    for (int i = 1; i < pathes.Length; i++)
                    {
                        path += "\\" + pathes[i];
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.Assert(false);
                _logger.Error(null, ex);
            }
            
        }
        public static string CopyFileToDestDirectory(string filePath, string destDirectory, bool overwrite)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return null;
                }
                if (!Directory.Exists(destDirectory))
                {
                    CreateDirectoryCheck(destDirectory);
                }
                string fileName = Path.GetFileName(filePath);
                string destFilePath = Path.Combine(destDirectory, fileName);
                File.Copy(filePath, destFilePath, overwrite);
                return destFilePath;
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return null;
            }
           
        }
        public static string CopyFileToDestDirectory(string filePath, string destDirectory)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return null;
                }

                if (!Directory.Exists(destDirectory))
                {
                    //CreateDirectory(destDirectory);
                    CreateDirectoryCheck(destDirectory);
                }

                string fileName = Path.GetFileName(filePath);
                string destFilePath = Path.Combine(destDirectory, fileName);
                destFilePath = DupesNameUtility.GetNewPathForDupes(destFilePath);
                File.Copy(filePath, destFilePath);
                return destFilePath;
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return null;
            }
        }

        public static void DeleteDirectoryByExclude(string directory, string excludeDir = null)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(directory);
                if (dir.Exists)
                {
                    DirectoryInfo[] childs = dir.GetDirectories();
                    foreach (DirectoryInfo child in childs)
                    {
                        if (child.Name == excludeDir)
                        {
                            continue;
                        }
                        child.Delete(true);
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.Assert(false);
                _logger.Error(null, ex);
            }
            
        }
        /// <summary>
        /// FileUtility.CopyDirectory(@"D:\temp\a", @"D:\temp\b");
        /// a is an existing folder.
        /// b can be existing, can be not.
        /// a and b are top levels. they have the same structure after copying.
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="destPath"></param>
        public static void CopyDirectory(string srcPath, string destPath)
        {
            try
            {
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                    _logger.Debug(string.Format("Directory created at: {0}", destPath));
                }
                destPath = Regex.Replace(destPath, "[\\/]+$", "");
                IEnumerable<string> directoryList = Directory.EnumerateDirectories(srcPath);
                foreach (var item in directoryList)
                {
                    string subDirName = Path.GetFileName(item);
                    string subDir = string.Format(@"{0}\{1}", destPath, subDirName);
                    Directory.CreateDirectory(subDir);
                    CopyDirectory(item, subDir);
                }
                IEnumerable<string> fileList = Directory.EnumerateFiles(srcPath);
                foreach (var item in fileList)
                {
                    string fileName = Path.GetFileName(item);
                    string destFileName = string.Format(@"{0}\{1}", destPath, fileName);
                    File.Copy(item, destFileName);
                }
                _logger.Debug(string.Format("Directory copied from {0} to {1}", srcPath, destPath));
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.Assert(false);
                _logger.Error(null, ex);
            }
        }
        public static void TryDeleteDirectory(string path, int maxErrorTime)
        {
            if (Directory.Exists(path))
            {
                int errorTime = 0;
                while (true)
                {
                    try
                    {
                        Directory.Delete(path, true);
                        break;
                    }
                    catch (Exception)
                    {
                        ++errorTime;
                        if (errorTime >= maxErrorTime)
                        {
                            throw;
                        }
                    }
                }

            }
        }


        public static bool IsFileMatchImageExtension(string filePath)
        {
            Regex imageFilenameRegex = new Regex(@"(.*?)\.(BMP|DIB|RLE|JPG|JPEG|JPE|JFIF|GIF|TIF|TIFF|PNG|)$", RegexOptions.IgnoreCase);
            return imageFilenameRegex.IsMatch(filePath);
        }

        public static bool IsFileMatchImageInfoExtension(string filePath)
        {
            Regex filenameRegex = new Regex(@"(.*?)\.(AFP)$", RegexOptions.IgnoreCase);
            return filenameRegex.IsMatch(filePath);
        }


        public static string GetInfoFilePathByOtherFile(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            string filename = Path.GetFileNameWithoutExtension(filePath);
            string extension = "."+Constants.FLOOR_PLAN_INFO_EXT;
            return Path.Combine(directory, (filename + extension));
        }

        public static string GetRenamFilePath(string filePath, string newName)
        {
            string directory = Path.GetDirectoryName(filePath);
            string extension = Path.GetExtension(filePath);
            string newFileName = string.Format("{0}{1}", newName, extension);
            return Path.Combine(directory, newFileName);
        }

        public static void CopyFile(string src, string dest, bool overwrite = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(src) || !File.Exists(src))
                {
                    return;
                }
                if (!Directory.Exists(Path.GetDirectoryName(dest)))
                {
                    CreateDirectoryCheck(Path.GetDirectoryName(dest));
                }
                File.Copy(src, dest, overwrite);
                _logger.Info(string.Format("File copied from {0} to {1}", src, dest));
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
            }
        }

        public static bool CheckImageValidation(string imagePath)
        {
            System.Drawing.Image image = null;

            try
            {
                image = System.Drawing.Image.FromFile(imagePath);
            }
            catch(Exception ex)
            {
                _logger.Error("CheckImageValidation", ex);
                return false;
            }

            if (image == null)
            {
                return false;
            }

            return true;
        }

        public static string GetAllFileNameUnderDir(string dir)
        {
            string result = string.Empty;
            try
            {
                do
                {

                    DirectoryInfo theFolder = new DirectoryInfo(dir);
                    if (!theFolder.Exists)
                    {
                        break;
                    }
                    DirectoryInfo[] dirInfo = theFolder.GetDirectories();
                    foreach (DirectoryInfo NextFolder in dirInfo)
                    {
                        FileInfo[] fileInfo = NextFolder.GetFiles();
                        foreach (FileInfo NextFile in fileInfo)
                        {
                            result = result + NextFile.Name + "  ";
                        }
                    }
                } while (false);
            }
            catch (Exception ex)
            {
                _logger.Warn("GetAllFileNameUnderDir", ex);
            }
           
            return result;
        }
    }
}
