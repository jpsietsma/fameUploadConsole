using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Data.SqlClient;
using FameDocumentUploaderSvc;
using System.Timers;
using System.DirectoryServices;

namespace fameUploadConsole
{
    
    class Program
    {

        public static FileSystemWatcher fameWatcher = new FileSystemWatcher(Configuration.cfgWatchDir);
        public static string wacDocUploader;

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

            //Checks 'wacFarmHome' to see if Farm ID is valid by searching for folder with corresponding farm ID
            if (Directory.Exists(wacFarmHome + wacFarmID))
            {
                validWACFarmID = true;
            }
            else
            {
                validWACFarmID = false;
                WriteFameLog(e, "error", "invalidFarmID");
            }


            //Check to see if the supplied document type is a valid WAC document that should be stored
            switch (wacDocType)
            {

                case "ASR":
                    {
                        validWACDocType = true;
                        fileSubPath = @"Final Documentation\ASRs";
                        finalFilePath = wacFarmHome + wacFarmID + @"\" + fileSubPath + @"\" + docFileName;

                        //Write success messages to FAME log and Windows Event log
                        WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        LogEvent(DateTime.Now.ToString() + " - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Build and Send email notification of successful upload
                        FameLibrary.SendUploadedFileEmail(e, finalFilePath, DateTime.Now);
                        Console.WriteLine(FameLibrary.GetADEmail(@"WAC\jsietsma"));
                   
                        Console.WriteLine(e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");
                        Console.WriteLine(' ');
                        break;
                    }

                case "NMP":
                    {
                        validWACDocType = true;
                        fileSubPath = @"Final Documentation\Nutrient Mgmt";
                        finalFilePath = wacFarmHome + wacFarmID + @"\" + fileSubPath + @"\" + docFileName;

                        //Write success messages to FAME log and Windows Event log
                        WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        LogEvent(DateTime.Now.ToString() + " - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        FameLibrary.SendUploadedFileEmail(e, finalFilePath, DateTime.Now);

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

                        //Write success messages to FAME log and Windows Event log
                        WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        LogEvent(" - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        FameLibrary.SendUploadedFileEmail(e, finalFilePath, DateTime.Now);

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
                case "OM":
                case "PAPP":
                case "PPD":
                case "TIER1":
                case "TIER2":
                case "WFPSUBF":
                case "WORKCOMP":
                default:
                    {
                        validWACDocType = false;

                        //Write invalid document errors to FAME error log and Windows Event log
                        LogEvent("Invalid Document Type: " + nameParts[0] + ". " + nameParts[1] + " was NOT uploaded", EventLogEntryType.Error);
                        WriteFameLog(e, "error", "invalidDocType");

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid Document Type: {0} has been detected.  Document WILL NOT be uploaded", nameParts[0]);
                        Console.WriteLine(' ');
                        Console.ResetColor();
                        break;
                    }

            }

            //Compare security logs to file drop name and time and pull file uploader from Windows Security event logs
            if (EventLog.SourceExists("Security"))
            {
                EventLog log = new EventLog() { Source = "Microsoft Windows security auditing.", Log = "Security" };

                foreach (EventLogEntry entry in log.Entries)
                {
                    if (entry.Message.Contains(@"E:\Projects\fame uploads\upload_drop") && entry.Message.Contains("0x80") && !entry.Message.Contains("desktop.ini"))
                    {
                        wacDocUploader = FameLibrary.GetUploadUserName(entry.Message, e.Name);

                    }

                }
            }
            else
            {
                WriteFameLog("Specified event source: 'security' does not exist");
                LogEvent("Specified event source: 'security' does not exist.", EventLogEntryType.Error);
            }

            //Check if file has valid farm ID and document type
            //If user drops a valid document type, then add it to database
            if (validWACFarmID && validWACDocType)
            {
                using (SqlConnection conn = new SqlConnection(Configuration.connectionString))
                {
                    try
                    {
                        conn.Open();
                        int queryResult;
                        string queryString = "INSERT INTO "

                            + Configuration.cfgSQLDatabase + ".dbo." + Configuration.cfgSQLTable

                            + "([fileDirectoryPath], [fileName], [fk_fileType], [fk_fileFarmID], [fk_fileUploader], [fileTimestamp], [fileSize]) "

                            + "VALUES("

                            + $" '{finalFilePath}', '{docFileName}', '{wacDocType}', '{wacFarmID}', '{wacDocUploader}', '{docUploadTime}', '{docFileSize}'"

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

        //Runs when the mailtimer ticks.  Used for summary emails 
        public static void MailTimer_Tick(object source, ElapsedEventArgs e)
        {
            
        }

        public static void CheckLogFiles(string logType)
        {
            if (logType == "error")
            {
                if (!File.Exists(Configuration.errorLogPath))
                {
                    using (FileStream fs = File.Create(Configuration.errorLogPath))
                    {
                        LogEvent(DateTime.Now.ToString() + " - Daily Error Log Does not exist, the file has been created", EventLogEntryType.Warning);
                    }
                }
            }

            if (logType == "transfer")
            {
                if (!File.Exists(Configuration.transferLogPath))
                {
                    using (FileStream fs = File.Create(Configuration.transferLogPath))
                    {
                        LogEvent(DateTime.Now.ToString() + " - Daily transfer Log Does not exist, the file has been created.", EventLogEntryType.Warning);
                    }
                }
            }

            if (logType == "system")
            {
                if (!File.Exists(Configuration.sysLogPath))
                {
                    using (FileStream fs = File.Create(Configuration.sysLogPath))
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
                WriteFameLog(" - FAME upload monitoring has successfully started");

            } else
            {

                fameWatcher.EnableRaisingEvents = status;
                LogEvent("FAME upload monitoring has been stopped", EventLogEntryType.Warning);
                WriteFameLog(" - FAME upload monitoring has been stopped.  No files will be uploaded until it has been restarted.");

            }

        }

        //Write to the FAME uploader program logs, using specific log types
        public static void WriteFameLog(FileSystemEventArgs arg, string logType, string errSub = "notice", string addmsg = "")
        {

            DateTime dt = new DateTime();
            dt = System.DateTime.UtcNow;
            string message = dt.ToLocalTime().ToString(@"HH:mm:sstt") + " - ";

            switch (logType)
            {
                case "error":
                    {
                        CheckLogFiles("error");

                            if (errSub == "invalidFarmID")
                            {
                                using (StreamWriter file = new StreamWriter(Configuration.errorLogPath, true))
                                {
                                    message += "Invalid Farm ID - " + (arg.Name).Split('_')[1] + " - upload cancelled.";
                                    file.WriteLine(message);
                                }

                            }
                            else if(errSub == "invalidDocType")
                            {
                                using (StreamWriter file = new StreamWriter(Configuration.errorLogPath, true))
                                {
                                    message += "Invalid Document Type - " + (arg.Name).Split('_')[0] + " - upload cancelled.";
                                    file.WriteLine(message);
                                }
                            }

                        break;
                    }
                case "notice":
                    {
                        CheckLogFiles("transfer");

                        using (StreamWriter file = new StreamWriter(Configuration.transferLogPath, true))
                        {
                            message += addmsg;
                            file.WriteLine(message);
                        }

                        break;
                    }
                case "status":
                    {
                        CheckLogFiles("system");

                        using (StreamWriter file = new StreamWriter(Configuration.sysLogPath, true))
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
            string message = DateTime.Now.ToString(@"HH:mm:sstt");
            message += msg;

            CheckLogFiles("system");
            using (StreamWriter file = new StreamWriter(Configuration.sysLogPath, true))
            {
                file.WriteLine(message);
            }

        }

#endregion

        public static void Main(string[] args)
        {
            //Create and start new thread for timer to allow program to wait for incoming files
            Thread timerThread = new Thread(new ThreadStart(ExecuteWorkerThread));

            //Timer to control mailflow
            System.Timers.Timer MailTimer = new System.Timers.Timer(30000);



            //Register events to listen for, Created, Changed, Renamed, Deleted
            fameWatcher.Created += new FileSystemEventHandler(OnChanged);

            MailTimer.Elapsed += new ElapsedEventHandler(MailTimer_Tick);
            
            //This begins the actual file monitoring
            ToggleMonitoring(true);
            timerThread.Start();
            MailTimer.Start();

        }
    }
}
