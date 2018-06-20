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
        //Paths to our error logs, transfer logs, and system logs
       public static string errorLogPath = @"E:\projects\fame uploads\logs\error-logs\" + DateTime.Now.ToString("MM-dd-yyyy") + "_error.log";
       public static string transferLogPath = @"E:\projects\fame uploads\logs\transfer-logs\" + DateTime.Now.ToString("MM-dd-yyyy") + "_transfer.log";
       public static string sysLogPath = @"E:\projects\fame uploads\logs\system-logs\" + DateTime.Now.ToString("MM-dd-yyyy") + "_system.log";

#region SQL/Watcher Configuration Data
        public static FileSystemWatcher fameWatcher = new FileSystemWatcher(cfgWatchDir);

        //SQL configuration details
        public const string cfgSQLServer = @"POTOKTEST";
        public const string cfgSQLDatabase = "wacTest";
        public const string cfgSQLUsername = "sa";
        public const string cfgSQLPassword = "WacAttack9";
        public const string cfgSQLTable = "testFameUploads";

        //Directory path to monitor for uploads
        public const string cfgWatchDir = @"E:\projects\fame uploads\upload_drop";

        public static bool runWorker = true;
        public static string connectionString = $"Server='{cfgSQLServer}';"
                                              + $"Database='{cfgSQLDatabase}';"
                                              + $"User Id='{cfgSQLUsername}';" 
                                              + $"Password='{cfgSQLPassword}';";

#endregion

#region Function Definitions...

        //This method is called when a File Creation is detected
        public static void OnChanged(object source, FileSystemEventArgs e)
        {

            string docFilePath = e.FullPath;
            string docFileName = e.Name;
            String[] nameParts = docFileName.Split('_');
            string wacDocType = nameParts[0];
            string wacFarmID = nameParts[1];

            string wacFarmHome = @"E:\Projects\fame uploads\Farms\";
            string fileSubPath = null;
            string finalFilePath = null;

            bool validWACDocType;
            bool validWACFarmID;

            DateTime docUploadTime = DateTime.Now;
            double docFileSize = new FileInfo(docFilePath).Length;

            if (Directory.Exists(wacFarmHome + wacFarmID))
            {
                validWACFarmID = true;
            }
            else
            {
                validWACFarmID = false;
                WriteFameLog(e, "error", "invalidFarmID");
            }

            switch (wacDocType)
            {

                case "ASR":
                    {
                        validWACDocType = true;
                        fileSubPath = @"Final Documentation\ASRs";
                        finalFilePath = wacFarmHome + wacFarmID + @"\" + fileSubPath + @"\" + docFileName;
                        WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded");

                        Console.WriteLine(e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");
                        Console.WriteLine(' ');
                        break;
                    }

                case "NMP":
                    {
                        validWACDocType = true;
                        fileSubPath = @"Final Documentation\Nutrient Mgmt";
                        finalFilePath = wacFarmHome + wacFarmID + @"\" + fileSubPath + @"\" + docFileName;
                        WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded");

                        Console.WriteLine(e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");
                        Console.WriteLine(' ');
                        break;
                    }

                case "WFP0":
                case "WFP1":
                case "WFP2":
                    {
                        validWACDocType = true;
                        fileSubPath = @"Final Documentation\WFP-0,WFP-1,WFP-2";
                        finalFilePath = wacFarmHome + wacFarmID + @"\" + fileSubPath + @"\" + docFileName;
                        WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded");

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
                        WriteFameLog(e, "error", "invalidDocType");

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid Document Type: {0} has been added.  Document WILL NOT be uploaded", nameParts[0]);
                        Console.WriteLine(' ');
                        Console.ResetColor();
                        break;
                    }

            }

            //Check if file has valid farm ID and document type
            //If user drops a valid document type, then add it to database
            if (validWACFarmID && validWACDocType)
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
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Oh, there was a problem! Exception: ");
                        Console.WriteLine(' ');
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(' ');
                        Console.WriteLine(ex.InnerException);
                        LogEvent(ex.Message, EventLogEntryType.Error);
                        Console.ResetColor();
                    }
                }
            }
        }

        public static void CheckLogFiles(string logType)
        {
            if (logType == "error")
            {
                if (!File.Exists(errorLogPath))
                {
                    using (FileStream fs = File.Create(errorLogPath))
                    {
                        LogEvent(DateTime.Now.ToString() + " - Daily Error Log Does not exist, the file has been created", EventLogEntryType.Warning);
                    }
                }
            }

            if (logType == "transfer")
            {
                if (!File.Exists(transferLogPath))
                {
                    using (FileStream fs = File.Create(transferLogPath))
                    {
                        LogEvent(DateTime.Now.ToString() + " - Daily transfer Log Does not exist, the file has been created.", EventLogEntryType.Warning);
                    }
                }
            }

            if (logType == "system")
            {
                if (!File.Exists(sysLogPath))
                {
                    using (FileStream fs = File.Create(sysLogPath))
                    {
                        LogEvent(DateTime.Now.ToString() + " - Daily System Log Does not exist, the file has been created.", EventLogEntryType.Warning);
                    }
                }
            }
        }

        //logs event to the Windows Event Log as event type information
        public static void LogEvent(string message)
        {
            string eventSource = "FAME Document Upload Watcher";
            DateTime dt = new DateTime();
            dt = System.DateTime.UtcNow;
            message = dt.ToLocalTime() + ": " + message;

            EventLog.WriteEntry(eventSource, message);
        }

        //logs event to the windows event log as supplied specific event type
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

        //Write to the FAME uploader program logs, using specific log types
        public static void WriteFameLog(FileSystemEventArgs arg, string logType, string errSub = "notice", string addmsg = "")
        {

            DateTime dt = new DateTime();
            dt = System.DateTime.UtcNow;
            string message = dt.ToLocalTime().ToString() + ": ";

            switch (logType)
            {
                case "error":
                    {
                        CheckLogFiles("error");

                            if (errSub == "invalidFarmID")
                            {
                                using (System.IO.StreamWriter file = new System.IO.StreamWriter(errorLogPath, true))
                                {
                                    message += "Invalid Farm ID - " + arg.Name + " - upload cancelled.";
                                    file.WriteLine(message);
                                }

                            }
                            else if(errSub == "invalidDocType")
                            {
                                using (System.IO.StreamWriter file = new System.IO.StreamWriter(errorLogPath, true))
                                {
                                    message += "Invalid Document Type - " + arg.Name + " - upload cancelled.";
                                    file.WriteLine(message);
                                }
                            }

                        break;
                    }
                case "notice":
                    {
                        CheckLogFiles("transfer");

                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(transferLogPath, true))
                        {
                            message += addmsg;
                            file.WriteLine(message);
                        }

                        break;
                    }
                case "status":
                    {
                        CheckLogFiles("system");

                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(sysLogPath, true))
                        {
                            message += addmsg;
                            file.WriteLine(message);
                        }

                        break;
                    }
            }
        }

        //Writes to the FAME uploader system log
        public static void WriteFameLog(string msg)
        {
            CheckLogFiles("system");
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(sysLogPath, true))
            {
                file.WriteLine(msg);
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
