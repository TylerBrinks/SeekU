using System.Configuration;
using System.IO;
using System.Reflection;

namespace SeekU.FileIO
{
    internal static class FileUtility
    {
        private static readonly string BaseDirectory;

        static FileUtility()
        {
            if (ConfigurationManager.AppSettings["SeekU.FileIO.BaseDirectory"] != null)
            {
                BaseDirectory = ConfigurationManager.AppSettings["SeekU.FileIO.BaseDirectory"];
            }
            else
            {
                var codeBase = Assembly.GetExecutingAssembly().Location;
                BaseDirectory = Path.GetDirectoryName(codeBase);
            }
        }

        public static string GetEventDirectory()
        {
            return Path.Combine(BaseDirectory, "Events");
        }

        public static string GetSnapshotDirectory()
        {
            return Path.Combine(BaseDirectory, "Snapshots");
        }
    }
}
