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

        #region Watcher Configuration Data

        public const string cfgSQLServer = @"POTOKTEST";
        public const string cfgSQLDatabase = "wacTest";
        public const string cfgSQLUsername = "sa";
        public const string cfgSQLPassword = "WacAttack9";
        public const string cfgSQLTable = "testFameUploads";
        public const string cfgWatchDir = @"E:\projects\fame uploads\upload_drop";
        public const string runWorker = "true";

        public static string connectionString = $"Server='{cfgSQLServer}';"
                                              + $"Database='{cfgSQLDatabase}';"
                                              + $"User Id='{cfgSQLUsername}';" 
                                              + $"Password='{cfgSQLPassword}';";

        #endregion

        //This method is called when a File Creation is detected
        public static void OnChanged(object source, FileSystemEventArgs e)
        {
  
            Console.WriteLine(e.Name + " has been " + e.ChangeType + " to FAME.  SQL server has been updated. ");
            Console.WriteLine(' ');

            #region Building SQL Query 
            string docFilePath = e.FullPath;
            string docFileName = e.Name;
            string docFileType = @"TV Show";
            DateTime docUploadTime = DateTime.Now;
            double docFileSize = 12.34;
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

                        + "([fileDirectoryPath], [fileName], [fk_fileType], [fk_fileFarmID], [fk_fileUploader], [fileTimestamp], [fileSize]) " 

                        + "VALUES("
                        
                        + $" '{docFilePath}', '{docFileName}', '{docFileType}', 'TEST-12345', 'fameAutomation', '{docUploadTime}', '{docFileSize}'" 
                        
                        + ");";

                        SqlCommand query = new SqlCommand(queryString, conn);
                        queryResult = query.ExecuteNonQuery();
                    conn.Close();
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Oh, there was a problem! Exception: ");
                    Console.WriteLine(' ');
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(' ');
                    Console.WriteLine(ex.InnerException);
                    LogEvent(ex.Message, EventLogEntryType.Error);
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
            while (true)  { Thread.Sleep(5000); Console.WriteLine("Worker Thread Status: Working"); Console.WriteLine(' '); }
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

        }
    }
}
