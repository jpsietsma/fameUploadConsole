using System;
using System.IO;
using System.Threading;
using System.Data.SqlClient;
using FameDocumentUploaderSvc;
using System.Timers;

namespace fameUploadConsole
{
    
    public class Program
    {

        public static FileSystemWatcher fameWatcher = new FileSystemWatcher(Configuration.cfgWatchDir);
        public static string wacDocUploader = @"FAMEDocUploader";

#region Function Definitions...

        //This method is called when a File Creation is detected
        public static void OnFileDropped(object source, FileSystemEventArgs e)
        {

            String[] nameParts = e.Name.Split('_');
            string wacDocType = nameParts[0];
            string wacFarmID = nameParts[1];
            string wacContractorName = nameParts[1];
            string wacDocTypeSectorFolderCode = string.Empty;            

            string fileSubPath = null;
            string finalFilePath = null;            

            int pk1 = 0; //represents the pk_participant or pk_farmBusiness
            int? pk2 = null; //If doctype is ASR then represents pk_asrAg, if doctype is WFP2 then represents pk_form_wfp2
            int? pk3 = null; //???

            //Determine if document is a valid Contractor or participant by checking nameparts[1] for either farmID or ContractorName in database
            //if (Directory.Exists(Configuration.wacFarmHome + wacFarmID))
            //{
            //    validWACFarmID = true;
            //    validWacContractor = false;
            //}
            //else if (Directory.Exists(Configuration.wacContractorHome + wacContractorName))
            //{
            //    validWacContractor = true;
            //    validWACFarmID = false;
            //}
            //else
            //{
            //    validWACFarmID = false;
            //    validWacContractor = false;

            //    FameLibrary.WriteFameLog(e, "error", "invalidFarmID");
            //}

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

                        fileSubPath = @"Final Documentation\ASRs";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, e.Name);

                        //Write success messages to FAME log and Windows Event log
                        FameLibrary.WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        FameLibrary.LogWindowsEvent(DateTime.Now.ToString() + " - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Build and Send email notification of successful upload
                        if (Configuration.enableSendingUploadEmail)
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
                        
                        fileSubPath = @"Final Documentation\Nutrient Mgmt\Nm Credits";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, e.Name);

                        //Write success messages to FAME log and Windows Event log
                        FameLibrary.WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        FameLibrary.LogWindowsEvent(DateTime.Now.ToString() + " - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (Configuration.enableSendingUploadEmail)
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

                        fileSubPath = @"Final Documentation\Nutrient Mgmt\Nm Plans";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, e.Name);

                        //Write success messages to FAME log and Windows Event log
                        FameLibrary.WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        FameLibrary.LogWindowsEvent(DateTime.Now.ToString() + " - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (Configuration.enableSendingUploadEmail)
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

                        fileSubPath = @"Final Documentation\WFP-0,1,2 COS\WFP-0";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, e.Name);

                        //Write success messages to FAME log and Windows Event log
                        FameLibrary.WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        FameLibrary.LogWindowsEvent(" - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (Configuration.enableSendingUploadEmail)
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

                        fileSubPath = @"Final Documentation\WFP-0,1,2 COS\WFP-1";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, e.Name);

                        //Write success messages to FAME log and Windows Event log
                        FameLibrary.WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        FameLibrary.LogWindowsEvent(" - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (Configuration.enableSendingUploadEmail)
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

                        fileSubPath = @"Final Documentation\WFP-0,1,2 COS\WFP-2";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, e.Name);

                        //Write success messages to FAME log and Windows Event log
                        FameLibrary.WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        FameLibrary.LogWindowsEvent(" - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (Configuration.enableSendingUploadEmail)
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

                        fileSubPath = @"AEM";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, e.Name);

                        //Write success messages to FAME log and Windows Event log
                        FameLibrary.WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        FameLibrary.LogWindowsEvent(" - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (Configuration.enableSendingUploadEmail)
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

                        fileSubPath = @"AEM\Tier1";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, e.Name);

                        //Write success messages to FAME log and Windows Event log
                        FameLibrary.WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        FameLibrary.LogWindowsEvent(" - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (Configuration.enableSendingUploadEmail)
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

                        fileSubPath = @"AEM\Tier2";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, e.Name);

                        //Write success messages to FAME log and Windows Event log
                        FameLibrary.WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        FameLibrary.LogWindowsEvent(" - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (Configuration.enableSendingUploadEmail)
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

                        fileSubPath = @"Correspondence";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, e.Name);

                        //Write success messages to FAME log and Windows Event log
                        FameLibrary.WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        FameLibrary.LogWindowsEvent(" - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (Configuration.enableSendingUploadEmail)
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

                        fileSubPath = @"Final Documentation\WFP-0,1,2 COS\COS";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, e.Name);

                        //Write success messages to FAME log and Windows Event log
                        FameLibrary.WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        FameLibrary.LogWindowsEvent(" - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (Configuration.enableSendingUploadEmail)
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

                        wacDocTypeSectorFolderCode = "A_OVERFORM";

                        fileSubPath = $@"Procurement\CREP";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, e.Name);

                        //Write success messages to FAME log and Windows Event log
                        FameLibrary.WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        FameLibrary.LogWindowsEvent(DateTime.Now.ToString() + " - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Build and Send email notification of successful upload
                        if (Configuration.enableSendingUploadEmail)
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

                        fileSubPath = @"Final Documentation\O&Ms";

                        finalFilePath = FameLibrary.BuildUploadFilePath(false, wacFarmID, fileSubPath, e.Name);

                        //Write success messages to FAME log and Windows Event log
                        FameLibrary.WriteFameLog(e, "notice", " ", e.Name + " has been successfully uploaded to " + finalFilePath);
                        FameLibrary.LogWindowsEvent(" - " + e.Name + " has been " + e.ChangeType + " to FAME.  Database has been updated. ");

                        //Send email notification of successful upload
                        if (Configuration.enableSendingUploadEmail)
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

                        //Set the document type sector folder code as per the db
                        wacDocTypeSectorFolderCode = "CONT_INS";

                        //Set the subfolder where the document type will reside
                        fileSubPath = $@"General Liability";

                        //Build the final path for where the file should be uploaded
                        finalFilePath = FameLibrary.BuildUploadFilePath(true, wacContractorName, fileSubPath, e.Name);

                        //Attempt to process the upload request and move the file
                        FameLibrary.ProcessUploadAttempt(e, finalFilePath);

                        //Add document information to FAME database
                        FameLibrary.AddFameDoc(finalFilePath, e.Name, wacDocTypeSectorFolderCode, pk1, pk2, pk3, wacDocUploader, out string errorMessage);

                        break;
                    }

                case "WORKCOMP":
                    {
                        //Get the participant ID for the contractor which document belongs
                        pk1 = FameLibrary.GetParticipantIDFromContractor(wacContractorName);
                        pk2 = null;
                        pk3 = null;

                        //Set the document type sector folder code as per the db
                        wacDocTypeSectorFolderCode = "CONT_COMP";

                        //Set the subfolder where the document type will reside
                        fileSubPath = @"Workers Comp";

                        //Build the final path for where the file should be uploaded
                        finalFilePath = FameLibrary.BuildUploadFilePath(true, wacContractorName, fileSubPath, e.Name);

                        //Attempt to process the upload request and move the file
                        FameLibrary.ProcessUploadAttempt(e, finalFilePath);

                        //Add document information to FAME database
                        FameLibrary.AddFameDoc(finalFilePath, e.Name, wacDocTypeSectorFolderCode, pk1, pk2, pk3, wacDocUploader, out string errorMessage);

                        break;
                    }

                case "IRSW9F":
                case "IRSW9":
                    {
                        //Get the participant ID for the contractor which document belongs
                        pk1 = FameLibrary.GetParticipantIDFromContractor(wacContractorName);
                        pk2 = null;
                        pk3 = null;

                        //Set the document type sector folder code as per the db
                        wacDocTypeSectorFolderCode = "CONT_IRS";

                        //Set the subfolder where the document type will reside
                        fileSubPath = @"W-9";

                        //Build the destination path for where the file should be uploaded
                        finalFilePath = FameLibrary.BuildUploadFilePath(true, wacContractorName, fileSubPath, e.Name);

                        //Attempt to process the upload request and move the file
                        FameLibrary.ProcessUploadAttempt(e, finalFilePath);

                        //Add document information to FAME database
                        FameLibrary.AddFameDoc(finalFilePath, e.Name, wacDocTypeSectorFolderCode, pk1, pk2, pk3, wacDocUploader, out string errorMessage);

                        break;
                    }
                #endregion

                default:
                    {

                        //Write invalid document errors to FAME error log and Windows Event log
                        FameLibrary.WriteFameLog("error", "Invalid FarmID, Contractor, or Document Type.  Document will not be uploaded.");
                        FameLibrary.LogWindowsEvent("Invalid FarmID, Contractor, or Document Type.  Document will not be uploaded.");

                        Console.WriteLine($@"Unknown Document Type: { nameParts[0] } has been detected.  Document WILL NOT be uploaded");
                        Console.WriteLine();

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

        }
        
#endregion

        public static void Main(string[] args)
        {
            //Create and start new thread for timer to allow program to wait for incoming files
            Thread timerThread = new Thread(new ThreadStart(FameLibrary.ExecuteWorkerThread));

            //Timer to control mailflow, default every 30 minutes
            System.Timers.Timer MailTimer = new System.Timers.Timer(Configuration.cfgMailTimer);

            //Register events to listen for: Created only
            fameWatcher.Created += new FileSystemEventHandler(OnFileDropped);

            //Check log messages at predefined intervals and send emails if necessary.            
            MailTimer.Elapsed += new ElapsedEventHandler(FameLibrary.MailTimer_Tick);

            //This begins the actual file monitoring
            FameLibrary.ToggleMonitoring(true, fameWatcher);
            timerThread.Start();
            MailTimer.Start();

        }
    }
}
