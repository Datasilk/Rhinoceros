using System.IO;

namespace Rhinoceros
{
    public static class FileSystem
    {
        public static void DeleteDirectory(string path)
        {
            var info = new DirectoryInfo(path);

            foreach(var dir in info.GetDirectories())
            {
                DeleteDirectory(dir.FullName);
            }

            foreach(var file in info.GetFiles())
            {
                file.Delete();
            }

            Directory.Delete(path);
        }
    }
}
