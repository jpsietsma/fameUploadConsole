using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Data.SqlClient;
using FameDocumentUploaderSvc;
using System.Timers;
using System.Text.RegularExpressions;

namespace fameUploadConsole
{
    
    public class Program
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
            string wacContractorName = nameParts[1];
            string wacDocTypeSectorFolderCode = string.Empty;            

            string wacFarmHome = Configuration.wacFarmHome;
            string wacContractorHome = Configuration.wacContractorHome;
            bool sendUploadEmails = Configuration.enableSendingUploadEmail;

            string fileSubPath = null;
            string finalFilePath = null;
            bool validWACDocType = false;
            bool validWACFarmID = false;
            bool validContractorDoc = false;
            bool validWacContractor = false;
            

            int pk1 = 0; //represents the pk_participant or pk_farmBusiness
            int? pk2 = null; //If doctype is ASR then represents pk_asrAg, if doctype is WFP2 then represents pk_form_wfp2
            int? pk3 = null; //???

            double docFileSize = new FileInfo(docFilePath).Length;

            //Determine if document is a valid Contractor or farm document by checking nameparts[1] for either farmID or ContractorName

            if (Directory.Exists(Configuration.wacFarmHome + wacFarmID))
            {
                validWACFarmID = true;
                validWacContractor = false;
            }
            else if (Directory.Exists(Configuration.wacContractorHome + wacContractorName))
            {
                validWacContractor = true;
                validWACFarmID = false;
            }
            else
            {
                validWACFarmID = false;
                validWacContractor = false;

                WriteFameLog(e, "error", "invalidFarmID");
            }


            //Check document type and set options as per supplied type
            switch (wacDocType)
            {

                #region WAC Participant Document Types...

                case "ASR":
                    {
                        pk1 = FameLibrary.GetFarmBusinessByFarmId(wacFarmID);
                        pk2 = null;
                        pk3 = null;

                        wacDocTypeSectorFolderCode = "A_ASR";

                        validWACDocType = true;

                        fileSubPath = @"Final Documentation\ASRs";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, docFileName);

                        //Write success messages to FAME log and Windows Event log
                        WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        LogEvent(DateTime.Now.ToString() + " - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Build and Send email notification of successful upload
                        if (sendUploadEmails)
                        {
                            FameLibrary.SendUploadedFileEmail(e, finalFilePath, DateTime.Now);
                        }
                   
                        Console.WriteLine(e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");
                        Console.WriteLine(finalFilePath);
                        Console.WriteLine();
                        break;
                    }

                case "NMCP":
                    {
                        pk1 = FameLibrary.GetFarmBusinessByFarmId(wacFarmID);
                        pk2 = null;
                        pk3 = null;

                        wacDocTypeSectorFolderCode = "A_NMCP";

                        validWACDocType = true;           
                        
                        fileSubPath = @"Final Documentation\Nutrient Mgmt\Nm Credits";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, docFileName);

                        //Write success messages to FAME log and Windows Event log
                        WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        LogEvent(DateTime.Now.ToString() + " - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (sendUploadEmails)
                        {
                            FameLibrary.SendUploadedFileEmail(e, finalFilePath, DateTime.Now);
                        }

                        Console.WriteLine(e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");
                        Console.WriteLine(finalFilePath);
                        Console.WriteLine();
                        break;
                    }

                case "NMP":
                    {
                        pk1 = FameLibrary.GetFarmBusinessByFarmId(wacFarmID);
                        pk2 = null;
                        pk3 = null;

                        wacDocTypeSectorFolderCode = "A_NMP";

                        validWACDocType = true;

                        fileSubPath = @"Final Documentation\Nutrient Mgmt\Nm Plans";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, docFileName);

                        //Write success messages to FAME log and Windows Event log
                        WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        LogEvent(DateTime.Now.ToString() + " - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (sendUploadEmails)
                        {
                            FameLibrary.SendUploadedFileEmail(e, finalFilePath, DateTime.Now);
                        }

                        Console.WriteLine(e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");
                        Console.WriteLine(finalFilePath);
                        Console.WriteLine();
                        break;
                    }

                case "WFP0":
                case "WFP-0":
                    {
                        pk1 = FameLibrary.GetFarmBusinessByFarmId(wacFarmID);
                        pk2 = null;
                        pk3 = null;

                        wacDocTypeSectorFolderCode = "A_WFP0";

                        validWACDocType = true;

                        fileSubPath = @"Final Documentation\WFP-0,1,2 COS\WFP-0";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, docFileName);

                        //Write success messages to FAME log and Windows Event log
                        WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        LogEvent(" - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (sendUploadEmails)
                        {
                            FameLibrary.SendUploadedFileEmail(e, finalFilePath, DateTime.Now);
                        }

                        Console.WriteLine(e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");
                        Console.WriteLine(finalFilePath);
                        Console.WriteLine();
                        break;
                    }

                case "WFP1":
                case "WFP-1":
                    {
                        pk1 = FameLibrary.GetFarmBusinessByFarmId(wacFarmID);
                        pk2 = null;
                        pk3 = null;

                        wacDocTypeSectorFolderCode = "A_WFP1";

                        validWACDocType = true;

                        fileSubPath = @"Final Documentation\WFP-0,1,2 COS\WFP-1";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, docFileName);

                        //Write success messages to FAME log and Windows Event log
                        WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        LogEvent(" - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (sendUploadEmails)
                        {
                            FameLibrary.SendUploadedFileEmail(e, finalFilePath, DateTime.Now);
                        }

                        Console.WriteLine(e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");
                        Console.WriteLine(finalFilePath);
                        Console.WriteLine();
                        break;
                    }

                case "WFP2":
                case "WFP-2":
                    {
                        pk1 = FameLibrary.GetFarmBusinessByFarmId(wacFarmID);
                        pk2 = null;
                        pk3 = null;

                        wacDocTypeSectorFolderCode = "A_WFP2";

                        validWACDocType = true;

                        fileSubPath = @"Final Documentation\WFP-0,1,2 COS\WFP-2";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, docFileName);

                        //Write success messages to FAME log and Windows Event log
                        WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        LogEvent(" - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (sendUploadEmails)
                        {
                            FameLibrary.SendUploadedFileEmail(e, finalFilePath, DateTime.Now);
                        }

                        Console.WriteLine(e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");
                        Console.WriteLine(finalFilePath);
                        Console.WriteLine();
                        break;
                    }

                case "AEM":
                    {
                        pk1 = FameLibrary.GetFarmBusinessByFarmId(wacFarmID);
                        pk2 = null;
                        pk3 = null;

                        wacDocTypeSectorFolderCode = "A_AEM";

                        validWACDocType = true;

                        fileSubPath = @"AEM";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, docFileName);

                        //Write success messages to FAME log and Windows Event log
                        WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        LogEvent(" - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (sendUploadEmails)
                        {
                            FameLibrary.SendUploadedFileEmail(e, finalFilePath, DateTime.Now);
                        }

                        Console.WriteLine(e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");
                        Console.WriteLine(finalFilePath);
                        Console.WriteLine();
                        break;
                    }

                case "TIER1":
                case "TIER-1":
                    {
                        pk1 = FameLibrary.GetFarmBusinessByFarmId(wacFarmID);
                        pk2 = null;
                        pk3 = null;

                        wacDocTypeSectorFolderCode = "A_TIER1";

                        validWACDocType = true;

                        fileSubPath = @"AEM\Tier1";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, docFileName);

                        //Write success messages to FAME log and Windows Event log
                        WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        LogEvent(" - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (sendUploadEmails)
                        {
                            FameLibrary.SendUploadedFileEmail(e, finalFilePath, DateTime.Now);
                        }

                        Console.WriteLine(e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");
                        Console.WriteLine(finalFilePath);
                        Console.WriteLine();
                        break;
                    }

                case "TIER-2":
                case "TIER2":
                    {
                        wacDocTypeSectorFolderCode = "A_TIER2";

                        validWACDocType = true;

                        fileSubPath = @"AEM\Tier2";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, docFileName);

                        //Write success messages to FAME log and Windows Event log
                        WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        LogEvent(" - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (sendUploadEmails)
                        {
                            FameLibrary.SendUploadedFileEmail(e, finalFilePath, DateTime.Now);
                        }

                        Console.WriteLine(e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");
                        Console.WriteLine(finalFilePath);
                        Console.WriteLine();
                        break;
                    }

                case "ALTR":
                    {
                        pk1 = FameLibrary.GetFarmBusinessByFarmId(wacFarmID);
                        pk2 = null;
                        pk3 = null;

                        wacDocTypeSectorFolderCode = "A_ALTR";

                        validWACDocType = true;

                        fileSubPath = @"Correspondence";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, docFileName);

                        //Write success messages to FAME log and Windows Event log
                        WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        LogEvent(" - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (sendUploadEmails)
                        {
                            FameLibrary.SendUploadedFileEmail(e, finalFilePath, DateTime.Now);
                        }

                        Console.WriteLine(e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");
                        Console.WriteLine(finalFilePath);
                        Console.WriteLine();
                        break;
                    }

                case "COS":
                    {
                        pk1 = FameLibrary.GetFarmBusinessByFarmId(wacFarmID);
                        pk2 = null;
                        pk3 = null;


                        wacDocTypeSectorFolderCode = "A_OVERFORM";

                        validWACDocType = true;

                        fileSubPath = @"Final Documentation\WFP-0,1,2 COS\COS";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, docFileName);

                        //Write success messages to FAME log and Windows Event log
                        WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        LogEvent(" - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (sendUploadEmails)
                        {
                            FameLibrary.SendUploadedFileEmail(e, finalFilePath, DateTime.Now);
                        }

                        Console.WriteLine(e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");
                        Console.WriteLine(finalFilePath);
                        Console.WriteLine();
                        break;
                    }

                case "CRP1":
                    {
                        pk1 = FameLibrary.GetFarmBusinessByFarmId(wacFarmID);
                        pk2 = null;
                        pk3 = null;

                        validContractorDoc = false;

                        wacDocTypeSectorFolderCode = "A_OVERFORM";

                        fileSubPath = $@"Procurement\CREP";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, docFileName);

                        //Write success messages to FAME log and Windows Event log
                        WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        LogEvent(DateTime.Now.ToString() + " - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Build and Send email notification of successful upload
                        if (sendUploadEmails)
                        {
                            FameLibrary.SendUploadedFileEmail(e, finalFilePath, DateTime.Now);
                        }

                        Console.WriteLine(e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");
                        Console.WriteLine();
                        break;
                    }

                case "OM":
                    {
                        pk1 = FameLibrary.GetFarmBusinessByFarmId(wacFarmID);
                        pk2 = null;
                        pk3 = null;

                        wacDocTypeSectorFolderCode = "A_FORMWAC";

                        validWACDocType = true;

                        fileSubPath = @"Final Documentation\O&Ms";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, docFileName);

                        //Write success messages to FAME log and Windows Event log
                        WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        LogEvent(" - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (sendUploadEmails)
                        {
                            FameLibrary.SendUploadedFileEmail(e, finalFilePath, DateTime.Now);
                        }

                        Console.WriteLine(e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");
                        Console.WriteLine(finalFilePath);
                        Console.WriteLine();
                        break;
                    }
                #endregion

                #region WAC Contractor Document Types...

                case "GeneralLiability":
                case "LIABILITY":
                case "CERTILIAB":
                case "CERTLIAB":
                    {
                        //Get the participant ID for the contractor which document belongs
                        pk1 = FameLibrary.GetParticipantIDFromContractor(wacContractorName);
                        pk2 = null;
                        pk3 = null;

                        validWACDocType = true;

                        //Set the document type sector folder code as per the db
                        wacDocTypeSectorFolderCode = "CONT_INS";

                        //Set the subfolder where the document type will reside
                        fileSubPath = $@"General Liability";

                        //Build the final path for where the file should be uploaded
                        finalFilePath = FameLibrary.BuildUploadFilePath(true, wacContractorName, fileSubPath, docFileName);

                        //Attempt to process the upload request and move the file
                        FameLibrary.ProcessUploadAttempt(e, finalFilePath);

                        break;
                    }

                case "WORKCOMP":
                    {
                        //Get the participant ID for the contractor which document belongs
                        pk1 = FameLibrary.GetParticipantIDFromContractor(wacContractorName);
                        pk2 = null;
                        pk3 = null;

                        validWACDocType = true;

                        //Set the document type sector folder code as per the db
                        wacDocTypeSectorFolderCode = "CONT_COMP";

                        //Set the subfolder where the document type will reside
                        fileSubPath = @"Workers Comp";

                        //Build the final path for where the file should be uploaded
                        finalFilePath = FameLibrary.BuildUploadFilePath(true, wacContractorName, fileSubPath, docFileName);

                        //Attempt to process the upload request and move the file
                        FameLibrary.ProcessUploadAttempt(e, finalFilePath);

                        break;
                    }

                case "IRSW9F":
                case "IRSW9":
                    {
                        //Get the participant ID for the contractor which document belongs
                        pk1 = FameLibrary.GetParticipantIDFromContractor(wacContractorName);
                        pk2 = null;
                        pk3 = null;

                        validWACDocType = true;

                        //Set the document type sector folder code as per the db
                        wacDocTypeSectorFolderCode = "CONT_IRS";

                        //Set the subfolder where the document type will reside
                        fileSubPath = @"W-9";

                        //Build the destination path for where the file should be uploaded
                        finalFilePath = FameLibrary.BuildUploadFilePath(true, wacContractorName, fileSubPath, docFileName);

                        //Attempt to process the upload request and move the file
                        FameLibrary.ProcessUploadAttempt(e, finalFilePath);

                        break;
                    }
                #endregion

                default:
                    {
                        validWACDocType = false;
                        validContractorDoc = false;

                        //Write invalid document errors to FAME error log and Windows Event log
                        LogEvent("Invalid Document Type: " + nameParts[0] + ". " + nameParts[1] + " was NOT uploaded", EventLogEntryType.Error);
                        WriteFameLog(e, "error", "invalidDocType");

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid Document Type: {0} has been detected.  Document WILL NOT be uploaded", nameParts[0]);
                        Console.WriteLine();
                        Console.ResetColor();
                        break;
                    }

            }

            //try
            //{
            //    //Try to Compare security logs to file drop name and time and pull file uploader from Windows Security event logs.  This requires administrator priviliges for the service or it can not read event log.
            //    if (EventLog.SourceExists("Security"))
            //    {
            //        EventLog log = new EventLog() { Source = "Microsoft Windows security auditing.", Log = "Security" };

            //        foreach (EventLogEntry entry in log.Entries)
            //        {
            //            if ((entry.Message.Contains(Configuration.wacFarmHome) || entry.Message.Contains(Configuration.wacContractorHome)) && (entry.Message.Contains("0x80")) && (!entry.Message.Contains("desktop.ini")))
            //            {
            //                wacDocUploader = FameLibrary.GetUploadUserName(entry.Message, e.Name);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        WriteFameLog("Specified event source: 'security' does not exist");
            //        LogEvent("Specified event source: 'security' does not exist.", EventLogEntryType.Error);
            //    }

            //}
            //catch (Exception ex)
            //{

            //    throw new Exception(ex.Message);
            //}



            //Check if file has a valid { farm ID and document type } or { contractor and contractor document type }
            //If user drops a valid combination, then add it to database
            if ((validWACFarmID && validWACDocType) || (validWacContractor && validWACDocType))
            {
                
                //TODO: In the future, this is where code to determine uploader will reside
                try
                {
                    wacDocUploader = @"WAC\famedocs";
                }
                catch(Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("oh no! an error occurred: " + ex.Message);
                    Console.WriteLine();
                    Console.WriteLine(ex.InnerException);
                    Console.ResetColor();
                    Console.WriteLine();

                }

                //If all checks pass, insert document information into database and move file to final path, final steps of adding document into fame
                using (SqlConnection conn = new SqlConnection(Configuration.connectionString))
                {

                    string queryString = string.Empty;

                    try
                    {
                        if (validWACFarmID)
                        {
                            queryString = "INSERT INTO "

                            + Configuration.cfgSQLDatabase + ".dbo." + Configuration.cfgSQLTable

                            + "([filename_actual], [filename_display], [fk_participantTypeSectorFolder_code], [created_by], [created], [modified_by], [modified], [PK_1], [PK_2], [PK_3]) "

                            + "VALUES("

                            + $@" '{ finalFilePath }', '{ docFileName }', '{ wacDocTypeSectorFolderCode }', '{ wacDocUploader }', '{ DateTime.Now.ToString("yyyy-MM-dd hh:MM:ss.fff") }', '{ wacDocUploader }', '{ DateTime.Now.ToString("yyyy-MM-dd hh:MM:ss.fff") }', '{ pk1 }', '{ pk2 }', '{ pk3 }' "

                            + ");";


                            conn.Open();
                            int queryResult;

                            //SqlCommand query = new SqlCommand(queryString, conn);
                            //queryResult = query.ExecuteNonQuery();
                            conn.Close();

                        }
                        else if (validWacContractor)
                        {
                            queryString = "INSERT INTO "

                            + Configuration.cfgSQLDatabase + ".dbo." + Configuration.cfgSQLTable

                            + "([filename_actual], [filename_display], [fk_participantTypeSectorFolder_code], [created_by], [created], [modified_by], [modified], [PK_1], [PK_2], [PK_3]) "

                            + "VALUES("

                            + $@" '{ finalFilePath }', '{ docFileName }', '{ wacDocTypeSectorFolderCode }', '{ wacDocUploader }', '{ DateTime.Now.ToString("yyyy-MM-dd hh:MM:ss.fff") }', '{ wacDocUploader }', '{ DateTime.Now.ToString("yyyy-MM-dd hh:MM:ss.fff") }', '{ pk1 }', '{ pk2 }', '{ pk3 }' "

                            + ");";



                            conn.Open();
                            int queryResult;

                            //SqlCommand query = new SqlCommand(queryString, conn);
                            //queryResult = query.ExecuteNonQuery();
                            conn.Close();

                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Uh-oh!  Invalid Farm Id or contractor.  Document will NOT be uploaded to FAME.");
                            WriteFameLog(e, "error", null, "Invalid Farm Id or Contractor.  Document cant be uploaded to FAME.");
                            LogEvent("Invalid Farm Id or Contractor.  Document cant be uploaded to FAME.");
                            Console.WriteLine();
                            Console.ResetColor();
                        }

                        
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Oh, there was a problem! Exception: ");
                        Console.WriteLine();
                        Console.WriteLine(ex.Message);
                        Console.WriteLine();
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

        //Check to make sure each of our error log files exist, or create them if they don't.  Write event to event log to record this, also.
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
                Thread.Sleep(3000);
                Console.WriteLine("Worker Thread Status: Working");
                Console.WriteLine();
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
            }
            else
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

                            if (errSub == "invalidFarmIDorContractor")
                            {
                                using (StreamWriter file = new StreamWriter(Configuration.errorLogPath, true))
                                {
                                    message += "Invalid Farm ID or Contractor - " + (arg.Name).Split('_')[1] + " - upload cancelled.";
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

            //Timer to control mailflow, default every 30 minutes
            System.Timers.Timer MailTimer = new System.Timers.Timer(1800000);

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
