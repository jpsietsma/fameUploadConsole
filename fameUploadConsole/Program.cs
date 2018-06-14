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
        public static FileSystemWatcher fameWatcher = new FileSystemWatcher(cfgWatchDir);

        #region SQL/Watcher Configuration Data
        private const string cfgSQLServer = @"NYPLEXSERV-SDN\NYSQLLIVE01_sdn";
        private const string cfgSQLDatabase = @"sdnMedia";
        private const string cfgSQLUsername = @"sa";
        private const string cfgSQLPassword = @"A!12@lop^6";
        private const string cfgSQLTable = @"sdnSortDrive";
        private const string cfgWatchDir = @"s:\~drops\powerdrop";
        public static string runWorker = "true";
        #endregion

        //This method is called when a File Creation is detected
        public static void OnChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("File has been added!");
            Console.WriteLine(e.Name + " has been " + e.ChangeType + " and SQL will be updated. ");

            #region Building SQL Query 
            string mediaFilePath = e.FullPath;
            string mediaFileName = e.Name;
            string mediaFileType = @"TV Show";
            DateTime mediaUploadTime = DateTime.Now;
            double mediaFileSize = 12.34;
            string connectionString = "Server=" + cfgSQLServer + ";Database=" + cfgSQLDatabase + ";User Id=" + cfgSQLUsername + ";Password=" + cfgSQLPassword + ";";
            #endregion

            LogEvent(e.Name + " has been " + e.ChangeType + ". ", EventLogEntryType.Information);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    int queryResult;
                    string queryString = "INSERT INTO " 
                        + cfgSQLDatabase + ".dbo." + cfgSQLTable 
                        + "([mediaFilePath], [mediaFileName], [mediaFileType], [mediaUploadTime], [mediaFileSize]) " 
                        + "VALUES('" + mediaFilePath + "', '" + mediaFileName + "', '" + mediaFileType + "', '" + mediaUploadTime + "', '" + mediaFileSize + "');";

                        SqlCommand query = new SqlCommand(queryString, conn);
                        queryResult = query.ExecuteNonQuery();
                    conn.Close();
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Oh, there was a problem! Exception: " + ex.Message);
                    LogEvent(ex.Message, EventLogEntryType.Error);
                }
                finally
                {
                    Console.WriteLine("File has been successfully uploaded to database!");
                }
            }
        }
        
        #region Function Definitions...
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

        //Executes when the timer workerThread is started
        public static void executeWorkerThread()
        {
            while(runWorker == "true")
            {
                Thread.Sleep(15000);
            }

            fameWatcher.EnableRaisingEvents = false;
            Console.WriteLine("Monitoring has been disabled");
        }
#endregion

        public static void Main(string[] args)
        {

            //Register the different types of file system events to listen for, Created, Changed, Renamed, Deleted
            //This launches the onChanged method we defined above.
            fameWatcher.Created += new FileSystemEventHandler(OnChanged);

            //This begins the actual file monitoring
            fameWatcher.EnableRaisingEvents = true;
            Thread timerThread = new Thread(new ThreadStart(executeWorkerThread));

            timerThread.Start();

            Console.WriteLine("Keep Monitoring?  Type true or false");
            runWorker = Console.ReadLine();

            while (true) {
                Thread.Sleep(15000);
            }

        }
    }
}
