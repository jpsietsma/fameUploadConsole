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

        public static bool runWorker = true;
        public static string connectionString = $"Server='{cfgSQLServer}';"
                                              + $"Database='{cfgSQLDatabase}';"
                                              + $"User Id='{cfgSQLUsername}';" 
                                              + $"Password='{cfgSQLPassword}';";

        #endregion

        //This method is called when a File Creation is detected
        public static void OnChanged(object source, FileSystemEventArgs e)
        {

            #region Building SQL Query 
            string docFilePath = e.FullPath;
            string docFileName = e.Name;
            String[] nameParts = docFileName.Split('_');
            string wacDocType = nameParts[0];
            string wacFarmID = nameParts[1];
            string fileSubPath = null;
            string finalFilePath = null;
            bool validWACDocType = true;

            #region Switch for Document Type Verification and final file path

            switch (wacDocType)
            {

                case "ASR":
                    {
                        fileSubPath = @"Final Documentation\ASRs";
                        finalFilePath = @"J:\Farms\" + wacFarmID + @"\" + fileSubPath + @"\" + docFileName;
                        Console.WriteLine(e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");
                        Console.WriteLine(' ');
                        break;
                    }

                case "NMP":
                    {
                        fileSubPath = @"Final Documentation\Nutrient Mgmt";
                        finalFilePath = @"J:\Farms\" + wacFarmID + @"\" + fileSubPath + @"\" + docFileName;
                        Console.WriteLine(e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");
                        Console.WriteLine(' ');
                        break;
                    }

                case "WFP0":
                case "WFP1":
                case "WFP2":
                    {
                        fileSubPath = @"Final Documentation\WFP-0,WFP-1,WFP-2";
                        finalFilePath = @"J:\Farms\" + wacFarmID + @"\" + fileSubPath + @"\" + docFileName;
                        Console.WriteLine(e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");
                        Console.WriteLine(' ');
                        break;
                    }

                case "AEM":
                case "ALTR":
                case "CERTILIAB":
                case "COS":
                case "CRP1":
                case "FPD":
                case "FPF":
                case "FRP":
                case "IRSW9":
                case "IRSW9F":
                case "OM":
                case "PAPP":
                case "PPD":
                case "RFP":
                case "TIER1":
                case "TIER2":
                case "WFPSUBF":
                default:
                    {
                        validWACDocType = false;
                        LogEvent("Invalid Document Type: " + nameParts[0] + ".  File was NOT uploaded", EventLogEntryType.Error);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid Document Type: {0} has been added.  Document WILL NOT be uploaded", nameParts[0]);
                        Console.WriteLine(' ');
                        Console.ResetColor();
                        break;
                    }

            }
            #endregion

            DateTime docUploadTime = DateTime.Now;
            double docFileSize = new FileInfo(docFilePath).Length;

            #endregion

            LogEvent(e.Name + " has been " + e.ChangeType + ". ", EventLogEntryType.Information);

            //If user drops a valid document type, then add it to database
            if (validWACDocType)
            {
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

                            + $" '{finalFilePath}', '{docFileName}', '{wacDocType}', '{wacFarmID}', 'fameAutomation', '{docUploadTime}', '{docFileSize}'"

                            + ");";

                        SqlCommand query = new SqlCommand(queryString, conn);
                        queryResult = query.ExecuteNonQuery();
                        conn.Close();
                    }
                    catch (Exception ex)
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
            string eventSource = "FAME Document Upload Watcher";
            DateTime dt = new DateTime();
            dt = System.DateTime.UtcNow;
            message = dt.ToLocalTime() + ": " + message;

            EventLog.WriteEntry(eventSource, message, e);
        }

        //Executes when the timer workerThread is started
        public static void ExecuteWorkerThread()
        {
            while (true)
            {
                Thread.Sleep(5000);
                Console.WriteLine("Worker Thread Status: Working");
                Console.WriteLine(' ');
            }
        }

        //Toggles the FileSystemWatcher monitoring
        public static void ToggleMonitoring(bool status)
        {

            if (status)
            {
                fameWatcher.EnableRaisingEvents = status;
                LogEvent("FAME upload monitoring has successfully started", EventLogEntryType.Information);

            } else
            {

                fameWatcher.EnableRaisingEvents = status;
                LogEvent("FAME upload monitoring has been stopped", EventLogEntryType.Warning);

            }

        }

#endregion
        
        public static void Main(string[] args)
        {
            //Create and start new thread for timer to allow program to wait for incoming files
            Thread timerThread = new Thread(new ThreadStart(ExecuteWorkerThread));
            timerThread.Start();

            //Register the different types of file system events to listen for, Created, Changed, Renamed, Deleted
            //This launches the onChanged method we defined above.
            fameWatcher.Created += new FileSystemEventHandler(OnChanged);

            //This begins the actual file monitoring
            ToggleMonitoring(true);

        }
    }
}
