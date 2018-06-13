using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Data.SqlClient;

namespace fameUploadConsole
{

    class Program
    {
        public FileSystemWatcher fameWatcher = null;

        private const string cfgSQLServer = @"xxxxxxx\xxxxxxx";
        private const string cfgSQLDatabase = @"xxxxx";
        private const string cfgSQLUsername = @"xxxxx";
        private const string cfgSQLPassword = @"xxxxx";
        private const string cfgSQLTable = @"sdnSortDrive";
        private const string cfgWatchDir = @"s:\~drops\powerdrop";

        public static int queryResult;
        public static string queryString = "INSERT INTO " + cfgSQLDatabase + ".dbo." + cfgSQLTable + "([mediaFilePath], [fileName], [fk_fileTypes]) " +
            "VALUES('', '', '');";

        public static string connectionString = "Server=" + cfgSQLServer + ";Database=" + cfgSQLDatabase + ";User Id=" + cfgSQLUsername + ";Password=" + cfgSQLPassword + ";";

        //This method is called when a File Creation is detected
        public static void OnChanged(object source, FileSystemEventArgs e)
        {
            LogEvent("New file has been added!", EventLogEntryType.Information);
            Console.WriteLine("File has been added!" + e);

            #region SQL Connection and update Query
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                        SqlCommand query = new SqlCommand(queryString, conn);
                        queryResult = query.ExecuteNonQuery();
                    conn.Close();
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Oh, there was a problem! Exception: " + ex);
                }
            }
            #endregion
        }

        //logs event to the Windows Event Log as event type notification
        public static void LogEvent(string message)
        {
            string eventSource = "FAME Document Upload Watcher";
            DateTime dt = new DateTime();
            dt = System.DateTime.UtcNow;
            message = dt.ToLocalTime() + ": " + message;

            EventLog.WriteEntry(eventSource, message);
        }

        //logs event to the windows event log as a specific event type
        public static void LogEvent(string message, EventLogEntryType e)
        {
            string eventSource = "FAME Upload Watcher";
            DateTime dt = new DateTime();
            dt = System.DateTime.UtcNow;
            message = dt.ToLocalTime() + ": " + message;

            EventLog.WriteEntry(eventSource, message, e);
        }

        public static void Main(string[] args)
        {
            FileSystemWatcher fameWatcher = new FileSystemWatcher(cfgWatchDir);

            fameWatcher.IncludeSubdirectories = false;
            fameWatcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastWrite;

            //Register the different types of file system events to listen for, Created, Changed, Renamed, Deleted
            fameWatcher.Created += new FileSystemEventHandler(OnChanged);

            //This begins the actual file monitoring
            fameWatcher.EnableRaisingEvents = true;

            while (true)
            {
                Thread.Sleep(30000);
            }

        }
    }
}
