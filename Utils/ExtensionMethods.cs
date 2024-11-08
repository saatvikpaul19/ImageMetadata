using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterGrade.Utils
{
    public static class ExtensionMethods
    {
        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");

            IEnumerable<FileInfo> files = Enumerable.Empty<FileInfo>();

            foreach (string ext in extensions)
            {
                var _files = dir.GetFiles(ext);
                files = files.Concat(dir.GetFiles(ext));
            }

            return files;
        }

        public static List<string> GetFilesByExtensions2(string directory, params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");

            List<string> files = new();

            foreach (var typ in extensions)
            {
                var _files = Directory.GetFiles(directory, typ);
                files.AddRange(Directory.GetFiles(directory, typ));
            }

            return files;
        }
    }

}
