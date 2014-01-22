using System.Configuration;
using System.IO;
using System.Reflection;

namespace SeekU.FileIO
{
    internal static class FileUtility
    {
        private static readonly string BaseDirectory;

        /// <summary>
        /// Finds the base location on disk where events and snapshots should be stored
        /// </summary>
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

        /// <summary>
        /// Gets the event directory
        /// </summary>
        /// <returns>Path to the event directory</returns>
        public static string GetEventDirectory()
        {
            return Path.Combine(BaseDirectory, "Events");
        }

        /// <summary>
        /// Gets the snapshot directory
        /// </summary>
        /// <returns>Path to the snapshot directory</returns>
        public static string GetSnapshotDirectory()
        {
            return Path.Combine(BaseDirectory, "Snapshots");
        }
    }
}
