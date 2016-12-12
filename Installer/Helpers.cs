using System.IO;
using System.IO.Compression;
using System.Linq;
using IWshRuntimeLibrary;

namespace Installer
{
    static class Helpers
    {
        public static void CreateShortcut(string linkPath, string linkName, string targetPath, string description)
        {
            var shell = new WshShell();
            var shortcut = (IWshShortcut)shell.CreateShortcut(Path.Combine(linkPath, $"{linkName}.lnk"));

            shortcut.Description = description;
            shortcut.TargetPath = targetPath;
            shortcut.Save();
        }
        public static void ExtractArchive(this ZipArchive archive, string path, bool overwrite = false)
        {
            if (overwrite)
                archive.Entries.ToList().ForEach((ZipArchiveEntry entry) =>
                {
                    var entryPath = Path.Combine(path, entry.FullName);
                    if (System.IO.File.Exists(entryPath))
                        System.IO.File.Delete(entryPath);
                    else if (Directory.Exists(entryPath))
                        Directory.Delete(entryPath, true);
                });

            archive.ExtractToDirectory(path);
        }
        public static Stream LoadFileFromResources(string filename)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().First(name => name.Contains(filename));
            return assembly.GetManifestResourceStream(resourceName);
        }
        public static byte[] ToBytes(this Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Close();
            return bytes;
        }
    }
}
