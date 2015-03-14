using System;
using System.Collections.Generic;
using System.IO;

namespace TinyMetroWpfLibrary.Utility
{
    public static class DupesNameUtility
    {
        public static string GetNewPathForDupes(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return path;
            }

            string directory = Path.GetDirectoryName(path);
            string filename = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);
            int counter = 1;
            string newFullPath;
            do
            {
                string newFilename = string.Format("{0}({1}){2}", filename, counter, extension);
                newFullPath = Path.Combine(directory, newFilename);
                counter++;
            } while (System.IO.File.Exists(newFullPath));

            return newFullPath;
        }

        public static string GetNewNameForDupes<T>(IEnumerable<T> list, Func<T, string> getPropertyString, string name)
        {
            if (!GetIsLitsContainProperty(list, getPropertyString, name))
            {
                return name;
            }

            if (name == null)
            {
                return name;
            }

            string directory = Path.GetDirectoryName(name);
            string filename = Path.GetFileNameWithoutExtension(name);
            string extension = Path.GetExtension(name);
            int counter = 1;
            string newFullPath;
            do
            {
                string newFilename = string.Format("{0}({1}){2}", filename, counter, extension);
                newFullPath = Path.Combine(directory, newFilename);
                counter++;
            } while (GetIsLitsContainProperty(list, getPropertyString, newFullPath));
            return newFullPath;
        }

        public static bool GetIsLitsContainProperty<T>(IEnumerable<T> list, Func<T, string> getPropertyString, string name)
        {
            bool isContain = false;
            if (list != null && getPropertyString != null)
            {
                foreach (var item in list)
                {
                    if (getPropertyString(item) == name)
                    {
                        isContain = true;
                        break;
                    }
                }
            }

            return isContain;
        }

    }
}
