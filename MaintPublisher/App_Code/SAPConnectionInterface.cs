using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAP.Middleware.Connector;
using SAP.Middleware;
using System.IO;

/// <summary>
/// Summary description for SAPConnectionInterface
/// </summary>
public class SAPConnectionInterface
{
    private RfcDestination rfcDestination;

    public bool checkConnection()
    {
        bool result = false;
        try
        {
            rfcDestination = RfcDestinationManager.GetDestination(System.Configuration.ConfigurationManager.AppSettings["SAP_SYSTEMNAME"]);
            if (rfcDestination != null)
            {
                rfcDestination.Ping();
                result = true;
            }            
        }
        catch (Exception ex)
        {
            result = false;
            throw new Exception("Connection Failure Error: " + ex.Message);           
        }
        return result;
    }

    public DMSDocument getDocumentDetail(String docNo, String docPart, String docVersion)
    {
        try
        {
            if (rfcDestination == null)
            {
                rfcDestination = RfcDestinationManager.GetDestination(System.Configuration.ConfigurationManager.AppSettings["SAP_SYSTEMNAME"]);
            }

            RfcRepository rfcRepo = rfcDestination.Repository;
            IRfcFunction document = rfcRepo.CreateFunction("ZPMEN158_DMS_GETDETAIL");
            document.SetValue("IM_DOCUMENTTYPE", "MRI");
            document.SetValue("IM_DOCUMENTNUMBER", docNo);
            document.SetValue("IM_DOCUMENTPART", docPart);
            document.SetValue("IM_DOCUMENTVERSION", docVersion); 
            document.Invoke(rfcDestination);

            IRfcStructure result = document.GetStructure("EX_RETURN");
            if (result.GetString("TYPE")!=null && result.GetString("TYPE").Trim().Length > 0 && !result.GetString("NUMBER").Equals("000"))
            {
                throw new Exception("Getting document detail error: " + result.GetString("MESSAGE"));
            }

            DMSDocument doc = new DMSDocument();
            IRfcStructure resultDoc = document.GetStructure("EX_DOCUMENT");
            doc.docType = resultDoc.GetString("DOCUMENTTYPE");
            doc.docNo = resultDoc.GetString("DOCUMENTNUMBER");
            doc.docVersion = resultDoc.GetString("DOCUMENTVERSION");
            doc.docPart = resultDoc.GetString("DOCUMENTPART");
            doc.description = resultDoc.GetString("DESCRIPTION");
            doc.userName = resultDoc.GetString("USERNAME");
            doc.status = resultDoc.GetString("STATUSEXTERN");
            doc.labCode = resultDoc.GetString("LABORATORY");

            DocCharacteristics charact = new DocCharacteristics();
            charact.acModel = new List<string>();
            charact.acZone = new List<string>();
            charact.mpSource = new List<string>();
            charact.taskSection = new List<string>();
            charact.tgAircraftEff = new List<string>();
            charact.inspectSpecial = new List<string>();
            charact.accessPanel = new List<string>();
            charact.mpdOffset = new List<string>();
            charact.mpdLimitInterval = new List<string>();
            charact.mpdOffsetSample = new List<string>();
            charact.mpdIntervalSample = new List<string>();
            charact.tgTaskOffset = new List<string>();
            charact.tgTaskLimit = new List<string>();
            charact.tgTaskOffsetSample = new List<string>();
            charact.tgTaskLimitSample = new List<string>();
            charact.mpdEngineEff = new List<string>();
            charact.mpdAircraftEff = new List<string>();
            charact.mpdReference = new List<string>();
            charact.otherReference = new List<string>();
            charact.tgIntervalOffsetNote = new List<string>();
            charact.tgDepending = new List<string>();

            IRfcTable tabCharact = document.GetTable("ET_CHARACT");
            foreach (IRfcStructure chars in tabCharact)
            {
                String charName = chars.GetString("CHARNAME");
                switch (charName)
                {
                    case "Z_AC_MODEL": charact.acModel.Add(chars.GetString("CHARVALUE")); break;
                    case "Z_AC_ZONE": charact.acZone.Add(chars.GetString("CHARVALUE")); break;
                    case "Z_ACCESS_PANEL_NUMBER": charact.accessPanel.Add(chars.GetString("CHARVALUE")); break; 
                    case "Z_ACTK_FINDING_REPORT_REQ": charact.inspReportReq = chars.GetString("CHARVALUE"); break;
                    case "Z_DETCD": charact.detecCode = chars.GetString("CHARVALUE"); break;
                    case "Z_DOC_REVDATE": charact.revDate = chars.GetString("CHARVALUE"); break;
                    case "Z_ENGINEERING_NOTE": charact.engineeringNote = chars.GetString("CHARVALUE"); break;
                    case "Z_INSP_SPECIAL1": charact.inspectSpecial.Add(chars.GetString("CHARVALUE")); break;
                    case "Z_MP_SOURCE": charact.mpSource.Add(chars.GetString("CHARVALUE")); break;
                    case "Z_MP_TASK_MH": charact.mpTaskMH = chars.GetString("CHARVALUE"); break;
                    case "Z_MP_TASK_NUMBER": charact.mpTaskNumber = chars.GetString("CHARVALUE"); break;
                    case "Z_MP_TASK_STATUS": charact.mpTaskStatus = chars.GetString("CHARVALUE"); break;
                    case "Z_MP_TG_TASK_NUMBER": charact.tgTaskNumber = chars.GetString("CHARVALUE"); break;
                    case "Z_MPD_APPLICABILITY_AC": charact.mpdAircraftEff.Add(chars.GetString("CHARVALUE")); break;
                    case "Z_MPD_APPLICABILITY_ENG": charact.mpdEngineEff.Add(chars.GetString("CHARVALUE")); break;
                    case "Z_MPD_OFFSET": charact.mpdOffset.Add(chars.GetString("CHARVALUE")); break;
                    case "Z_MPD_LIMIT-INTERVAL": charact.mpdLimitInterval.Add(chars.GetString("CHARVALUE")); break;
                    case "Z_MPD_LIMIT-INTERVAL_SAMPLE": charact.mpdIntervalSample.Add(chars.GetString("CHARVALUE")); break;
                    case "Z_MPD_OFFSET_SAMPLE": charact.mpdOffsetSample.Add(chars.GetString("CHARVALUE")); break;
                    case " Z_MPD_LIMIT-INTERVAL_SAMPLE": charact.mpdIntervalSample.Add(chars.GetString("CHARVALUE")); break;
                    case "Z_MPD_REFERENCE_DOCUMENT": charact.mpdReference.Add(chars.GetString("CHARVALUE")); break;                    
                    case "Z_MPD_REV_NUMBER": charact.revNumber = chars.GetString("CHARVALUE"); break;                    
                    case "Z_REASON": charact.reason = chars.GetString("CHARVALUE"); break;
                    case "Z_REFERENCE_DOCUMENT": charact.otherReference.Add(chars.GetString("CHARVALUE")); break;
                    case "Z_SAM_PERCENT": charact.samplingPercent = chars.GetString("CHARVALUE");  break;
                    case "Z_TASK_SECTION": charact.taskSection.Add(chars.GetString("CHARVALUE")); break;
                    case "Z_TASK_REV_CODE": charact.revCode = chars.GetString("CHARVALUE"); break;
                    case "Z_TG_APPLICABILITY_AC": charact.tgAircraftEff.Add(chars.GetString("CHARVALUE")); break;
                    case "Z_TG_DEPENDING": charact.tgDepending.Add(chars.GetString("CHARVALUE")); break;
                    case "Z_TG_INT_OFFSET_NOTE": charact.tgIntervalOffsetNote.Add(chars.GetString("CHARVALUE")); break;
                    case "Z_TG_LIMIT-INTERVAL": charact.tgTaskLimit.Add(chars.GetString("CHARVALUE")); break;
                    case "Z_TG_LIMIT-INTERVAL_SAMPLE": charact.tgTaskLimitSample.Add(chars.GetString("CHARVALUE")); break;
                    case "Z_TG_OFFSET": charact.tgTaskOffset.Add(chars.GetString("CHARVALUE")); break;
                    case "Z_TG_OFFSET_SAMPLE": charact.tgTaskOffsetSample.Add(chars.GetString("CHARVALUE")); break;                  
                    case "Z_VALID_FROM": charact.validFrom = chars.GetString("CHARVALUE"); break;
                    case "Z_WORK_TYPE": charact.workType = chars.GetString("CHARVALUE"); break;
                    default:
                        break;
                } 
            }
            if (charact.revCode == null) charact.revCode = "";
            if (charact.mpSource.Count == 0) charact.mpSource.Add("");
            if (charact.taskSection.Count == 0) charact.taskSection.Add("");
            if (charact.workType == null) charact.workType = "";
            if (charact.inspectSpecial.Count == 0) charact.inspectSpecial.Add("");
            if (charact.detecCode == null) charact.detecCode = "";
            if (charact.inspReportReq == null) charact.inspReportReq = "";
            doc.characteristics = charact;

            doc.attachments = new List<dmsFile>();
            IRfcTable attachments = document.GetTable("ET_DOCUMENTFILES");
            foreach (IRfcStructure attachment in attachments)
            {
                String origType = attachment.GetString("ORIGINALTYPE");
                String storageCAT = attachment.GetString("STORAGECATEGORY");
                String appl = attachment.GetString("WSAPPLICATION");
                String attchFile = attachment.GetString("DOCFILE");
                String applID = attachment.GetString("APPLICATION_ID");
                String fileID = attachment.GetString("FILE_ID");
                char checkedIN = attachment.GetChar("CHECKEDIN");
                char activeVersion = attachment.GetChar("ACTIVE_VERSION");

                dmsFile attch_file = new dmsFile();
                attch_file.originalType = origType;
                attch_file.storageCategory = storageCAT;                  
                attch_file.application = appl;
                attch_file.origFile = attchFile;
                attch_file.docFile = attchFile.Substring(attchFile.LastIndexOf('\\') + 1); ;
                attch_file.applicationID = applID;
                attch_file.fileID = fileID;
                attch_file.checkedIN = checkedIN.ToString();
                attch_file.activeVersion = activeVersion.ToString();
                doc.attachments.Add(attch_file);
            }
            return doc;
        }
        catch (Exception ex)
        {
            throw new Exception("List laboratories error: " + ex.Message);
        }
    }

    public List<Laboratory> listLaboratories()
    {
        try
        {
            if (rfcDestination == null)
            {
                rfcDestination = RfcDestinationManager.GetDestination(System.Configuration.ConfigurationManager.AppSettings["SAP_SYSTEMNAME"]);
            }

            RfcRepository rfcRepo = rfcDestination.Repository;
            IRfcFunction createFunc = rfcRepo.CreateFunction("ZPM_LIST_LABORATORY");
            createFunc.Invoke(rfcDestination);

            IRfcTable laboratories = createFunc.GetTable("IT_RESULT");
            List<Laboratory> labs = new List<Laboratory>();
            foreach (IRfcStructure laboratory in laboratories)
            {
                Laboratory lab = new Laboratory(laboratory.GetString("LABOR"), laboratory.GetString("LBTXT"));
                labs.Add(lab);
            }
            return labs;
        }
        catch (Exception ex)
        {
            throw new Exception("List laboratories error: " + ex.Message);
        }        
    }

    public RfcResult createMRIDocument(DMSDocument doc, MRIClass mri)
    {
        RfcResult result = new RfcResult();
        try
        {
            if (rfcDestination == null)
            {
                rfcDestination = RfcDestinationManager.GetDestination(System.Configuration.ConfigurationManager.AppSettings["SAP_SYSTEMNAME"]);
            }

            RfcRepository rfcRepo = rfcDestination.Repository;

            IRfcStructure document = rfcRepo.GetStructureMetadata("BAPI_DOC_DRAW2").CreateStructure();
            document.SetValue("DOCUMENTTYPE", doc.docType);
            document.SetValue("DOCUMENTNUMBER", doc.docNo);
            document.SetValue("DOCUMENTPART", doc.docPart);
            //document.SetValue("DOCUMENTVERSION", doc.docVersion);            

            document.SetValue("DESCRIPTION", doc.description);
            document.SetValue("USERNAME", doc.userName);
            //document.SetValue("STATUSINTERN", doc.status);
            document.SetValue("LABORATORY", doc.laboratory);          

            //IRfcStructure mriClass = rfcRepo.GetStructureMetadata("ZPMS158_DMS_MRI").CreateStructure();
            //mriClass.SetValue("Z_AC_MODEL", mri.acModel);
            //mriClass.SetValue("Z_MPD_REV_NUMBER", mri.revNumber);
            //mriClass.SetValue("Z_DOC_REVDATE", mri.revDate);
            //mriClass.SetValue("Z_TASK_REV_CODE", mri.revCode);
            //mriClass.SetValue("Z_AC_ZONE", mri.acZone);
            //mriClass.SetValue("Z_MP_SOURCE", mri.mpSource);
            //mriClass.SetValue("Z_TASK_SECTION", mri.taskSection);
            //mriClass.SetValue("Z_SAM_PERCENT", mri.samplingPercent);
            //mriClass.SetValue("Z_WORK_TYPE", mri.workType);
            //mriClass.SetValue("Z_INSP_SPECIAL1", mri.inspectSpecial);
            //mriClass.SetValue("Z_MP_TASK_NUMBER", mri.mpTaskNumber);
            //mriClass.SetValue("Z_MP_TG_TASK_NUMBER", mri.tgTaskNumber);
            //if (mri.validFrom != null && mri.validFrom.Trim().Length > 0)
            //{
            //    DateTime validFrom = DateTime.ParseExact(mri.validFrom, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            //    mriClass.SetValue("Z_VALID_FROM", validFrom.ToString("dd.MM.yyyy"));
            //}
            //mriClass.SetValue("Z_MP_TASK_STATUS", mri.mpTaskStatus);
            //mriClass.SetValue("Z_MP_TASK_MH", mri.mpTaskMH);            
            //mriClass.SetValue("Z_ACCESS_PANEL_NUMBER", mri.accessPanel);
            //mriClass.SetValue("Z_MPD_OFFSET", mri.mpdOffset);
            //mriClass.SetValue("Z_MPD_LIMIT_INTERVAL", mri.mpdLimitInterval);
            //mriClass.SetValue("Z_MPD_OFFSET_SAMPLE", mri.mpdOffsetSample);
            //mriClass.SetValue("Z_MPD_LIMIT_INTERVAL_SAMPLE", mri.mpdIntervalSample);
            //mriClass.SetValue("Z_TG_OFFSET", mri.tgTaskOffset);
            //mriClass.SetValue("Z_TG_LIMIT_INTERVAL", mri.tgTaskLimit);
            //mriClass.SetValue("Z_TG_OFFSET_SAMPLE", mri.tgTaskOffsetSample);
            //mriClass.SetValue("Z_TG_LIMIT_INTERVAL_SAMPLE", mri.tgTaskLimitSample);
            //mriClass.SetValue("Z_MPD_APPLICABILITY_ENG", mri.mpdEngineEff);
            //mriClass.SetValue("Z_MPD_APPLICABILITY_AC", mri.mpdAircraftEff);
            //mriClass.SetValue("Z_TG_APPLICABILITY_AC", mri.tgAircraftEff);
            //mriClass.SetValue("Z_MPD_REFERENCE_DOCUMENT", mri.mpdReference);
            //mriClass.SetValue("Z_REFERENCE_DOCUMENT", mri.otherReference);
            //mriClass.SetValue("Z_DETCD", mri.detecCode);
            //mriClass.SetValue("Z_ACTK_FINDING_REPORT_REQ", mri.inspReportReq);
            //mriClass.SetValue("Z_TG_INT_OFFSET_NOTE", mri.tgIntervalOffsetNote);
            //mriClass.SetValue("Z_TG_DEPENDING", mri.tgDepending);
            //mriClass.SetValue("Z_REASON", mri.reason);
            //mriClass.SetValue("Z_ENGINEERING_NOTE", mri.engineeringNote);                                 

            IRfcFunction createFunc = rfcRepo.CreateFunction("ZPMEN158_DMS_CREATE");
            createFunc.SetValue("IM_DOCUMENT", document);
            //createFunc.SetValue("IM_CLASS_MRI", mriClass);

            IRfcTable allocTab = createFunc.GetTable("LT_ALLOC");
            allocTab.Append();
            allocTab.SetValue("CLASSTYPE", "017");
            allocTab.SetValue("CLASSNAME", "Z_MRI");
            
            IRfcTable mriClass = createFunc.GetTable("LT_CHARACT");

            foreach (String acModel in mri.acModel)
            {
                if (acModel != null && acModel.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_AC_MODEL");
                    mriClass.SetValue("CHARVALUE", acModel);
                }
            }

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_MPD_REV_NUMBER");
            mriClass.SetValue("CHARVALUE", mri.revNumber);

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_DOC_REVDATE");
            mriClass.SetValue("CHARVALUE", mri.revDate);

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_TASK_REV_CODE");
            mriClass.SetValue("CHARVALUE", mri.revCode);

            foreach (String acZone in mri.acZone)
            {
                if (acZone != null && acZone.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_AC_ZONE");                    

                    if (acZone.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", acZone.Substring(11)); ;
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", acZone);
                    }
                }
            }

            foreach (String mpSource in mri.mpSource)
            {
                if (mpSource != null && mpSource.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_MP_SOURCE");
                    if (mpSource.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", mpSource.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", mpSource);
                    }
                }
            }

            foreach (String taskSection in mri.taskSection)
            {
                if (taskSection != null && taskSection.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_TASK_SECTION");
                    if (taskSection.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", taskSection.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", taskSection);
                    }
                }
            }

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_SAM_PERCENT");
            mriClass.SetValue("CHARVALUE", mri.samplingPercent);

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_WORK_TYPE");
            mriClass.SetValue("CHARVALUE", mri.workType);

            foreach (String inspectSpecial in mri.inspectSpecial)
            {
                if (inspectSpecial != null && inspectSpecial.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_INSP_SPECIAL1");
                    if (inspectSpecial.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", inspectSpecial.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", inspectSpecial);
                    }
                }
            }

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_MP_TASK_NUMBER");
            mriClass.SetValue("CHARVALUE", mri.mpTaskNumber);

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_MP_TG_TASK_NUMBER");
            mriClass.SetValue("CHARVALUE", mri.tgTaskNumber);

            if (mri.validFrom != null && mri.validFrom.Trim().Length > 0)
            {
                DateTime validFrom = DateTime.ParseExact(mri.validFrom, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                mriClass.Append();
                mriClass.SetValue("CLASSTYPE", "017");
                mriClass.SetValue("CLASSNAME", "Z_MRI");
                mriClass.SetValue("CHARNAME", "Z_VALID_FROM");
                mriClass.SetValue("CHARVALUE", validFrom.ToString("dd.MM.yyyy"));
            }

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_MP_TASK_STATUS");
            mriClass.SetValue("CHARVALUE", mri.mpTaskStatus);

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_MP_TASK_MH");
            mriClass.SetValue("CHARVALUE", mri.mpTaskMH);

            foreach (String accessPanel in mri.accessPanel)
            {
                if (accessPanel != null && accessPanel.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_ACCESS_PANEL_NUMBER");
                    if (accessPanel.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", accessPanel.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", accessPanel);
                    }
                }
            }

            foreach (String mpdOffset in mri.mpdOffset)
            {
                if (mpdOffset != null && mpdOffset.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_MPD_OFFSET");
                    if (mpdOffset.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", mpdOffset.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", mpdOffset);
                    }
                }
            }

            foreach (String mpdLimitInterval in mri.mpdLimitInterval)
            {
                if (mpdLimitInterval != null && mpdLimitInterval.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_MPD_LIMIT-INTERVAL");
                    if (mpdLimitInterval.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", mpdLimitInterval.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", mpdLimitInterval);
                    }
                }
            }

            foreach (String mpdOffsetSample in mri.mpdOffsetSample)
            {
                if (mpdOffsetSample != null && mpdOffsetSample.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_MPD_OFFSET_SAMPLE");
                    if (mpdOffsetSample.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", mpdOffsetSample.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", mpdOffsetSample);
                    }
                }
            }

            foreach (String mpdIntervalSample in mri.mpdIntervalSample)
            {
                if (mpdIntervalSample != null && mpdIntervalSample.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_MPD_LIMIT-INTERVAL_SAMPLE");
                    if (mpdIntervalSample.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", mpdIntervalSample.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", mpdIntervalSample);
                    }
                }
            }

            foreach (String tgTaskOffset in mri.tgTaskOffset)
            {
                if (tgTaskOffset != null && tgTaskOffset.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_TG_OFFSET");
                    if (tgTaskOffset.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", tgTaskOffset.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", tgTaskOffset);
                    }
                }
            }

            foreach (String tgTaskLimit in mri.tgTaskLimit)
            {
                if (tgTaskLimit != null && tgTaskLimit.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_TG_LIMIT-INTERVAL");
                    if (tgTaskLimit.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", tgTaskLimit.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", tgTaskLimit);
                    }
                }
            }

            foreach (String tgTaskOffsetSample in mri.tgTaskOffsetSample)
            {
                if (tgTaskOffsetSample != null && tgTaskOffsetSample.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_TG_OFFSET_SAMPLE");
                    if (tgTaskOffsetSample.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", tgTaskOffsetSample.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", tgTaskOffsetSample);
                    }
                }
            }

            foreach (String tgTaskLimitSample in mri.tgTaskLimitSample)
            {
                if (tgTaskLimitSample != null && tgTaskLimitSample.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_TG_LIMIT-INTERVAL_SAMPLE");
                    if (tgTaskLimitSample.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", tgTaskLimitSample.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", tgTaskLimitSample);
                    }
                }
            }

            foreach (String mpdEngineEff in mri.mpdEngineEff)
            {
                if (mpdEngineEff != null && mpdEngineEff.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_MPD_APPLICABILITY_ENG");
                    if (mpdEngineEff.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", mpdEngineEff.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", mpdEngineEff);
                    }
                }
            }

            foreach (String mpdAircraftEff in mri.mpdAircraftEff)
            {
                if (mpdAircraftEff != null && mpdAircraftEff.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_MPD_APPLICABILITY_AC");
                    if (mpdAircraftEff.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", mpdAircraftEff.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", mpdAircraftEff);
                    }
                }
            }

            foreach (String tgAircraftEff in mri.tgAircraftEff)
            {
                if (tgAircraftEff != null && tgAircraftEff.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_TG_APPLICABILITY_AC");
                    if (tgAircraftEff.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", tgAircraftEff.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", tgAircraftEff);
                    }
                }
            }

            foreach (String mpdReference in mri.mpdReference)
            {
                if (mpdReference != null && mpdReference.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_MPD_REFERENCE_DOCUMENT");
                    if (mpdReference.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", mpdReference.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", mpdReference);
                    }
                }
            }

            foreach (String otherReference in mri.otherReference)
            {
                if (otherReference != null && otherReference.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_REFERENCE_DOCUMENT");
                    if (otherReference.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", otherReference.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", otherReference);
                    }
                }
            }

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_DETCD");
            mriClass.SetValue("CHARVALUE", mri.detecCode);

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_ACTK_FINDING_REPORT_REQ");
            mriClass.SetValue("CHARVALUE", mri.inspReportReq);

            foreach (String tgIntervalOffsetNote in mri.tgIntervalOffsetNote)
            {
                if (tgIntervalOffsetNote!=null && tgIntervalOffsetNote.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_TG_INT_OFFSET_NOTE");
                    if (tgIntervalOffsetNote.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", tgIntervalOffsetNote.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", tgIntervalOffsetNote);
                    }
                }
            }            

            foreach (String tgDepending in mri.tgDepending)
            {
                if (tgDepending !=null && tgDepending.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_TG_DEPENDING");
                    if (tgDepending.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", tgDepending.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", tgDepending);
                    }
                }
            }            

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_REASON");
            mriClass.SetValue("CHARVALUE", mri.reason);

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_ENGINEERING_NOTE");
            mriClass.SetValue("CHARVALUE", mri.engineeringNote);
            /*
                        IRfcTable longTextTab = createFunc.GetTable("LT_LONGTXT");
                        longTextTab.Append();
                        longTextTab.SetValue("DELETEVALUE", ' ');
                        longTextTab.SetValue("LANGUAGE", 'S');
                        longTextTab.SetValue("LANGUAGE_ISO", "EN");
                        longTextTab.SetValue("TEXTLINE", "TEXT LINE");
            */
              
            IRfcTable descTab = createFunc.GetTable("LT_DESC");
            descTab.Append();
            descTab.SetValue("DELETEVALUE", ' ');
            descTab.SetValue("LANGUAGE", 'E');
            descTab.SetValue("LANGUAGE_ISO", "EN");
            descTab.SetValue("DESCRIPTION", doc.description);
            descTab.SetValue("TEXTINDICATOR", ' ');            
            
            createFunc.Invoke(rfcDestination);

            IRfcStructure createResult = createFunc.GetStructure("EX_RETURN");
            char iResult = createResult.GetChar("TYPE");

            if (iResult == 'S' || iResult == 'W' || iResult == ' ')
            {
                result.result = true;
                String docType = createFunc.GetString("EX_DOCTYPE");
                String docNumber = createFunc.GetString("EX_DOCNUMBER");
                String docPart = createFunc.GetString("EX_DOCPART");
                String docVersion = createFunc.GetString("EX_DOCVERSION");
                result.docNumber = docNumber;
                result.docType = docType;
                result.docPart = docPart;
                result.docVersion = docVersion;
                result.message = $"{docNumber}-{docType}-{docPart}-{docVersion} has been created successfully";
            }
            else
            {
                result.result = false;
                result.message = createResult.GetString("MESSAGE");
            }

            if (result.result)
            {
                foreach (dmsFile attch in doc.attachments)
                {
                    if (attch.updateStatus != null && attch.updateStatus.Equals("N"))
                    {
                        IRfcFunction checkinFunc = rfcRepo.CreateFunction("CVAPI_DOC_CHECKIN");
                        checkinFunc.SetValue("PF_DOKAR", result.docType);
                        checkinFunc.SetValue("PF_DOKNR", result.docNumber);
                        checkinFunc.SetValue("PF_DOKTL", result.docPart);
                        checkinFunc.SetValue("PF_DOKVR", result.docVersion);
                        checkinFunc.SetValue("PF_CONTENT_PROVIDE", "TBL");

                        IRfcTable docFile = checkinFunc.GetTable("PT_FILES_X");
                        docFile.Append();
                        String appl = attch.application.ToUpper();
                        switch (appl)
                        {
                            case "PPT": appl = "PPT"; break;
                            case "PPTX": appl = "PPT"; break;
                            case "DOC": appl = "WWI"; break;
                            case "DOCX": appl = "WWI"; break;
                            case "JPG": appl = "GRF"; break;
                            case "PNG": appl = "GRF"; break;
                            case "GIF": appl = "GRF"; break;
                            case "XLS": appl = "XLS"; break;
                            case "XLSX": appl = "XLS"; break;
                            case "ZIP": appl = "ZIP"; break;
                            case "RAR": appl = "ZIP"; break;
                        }
                        docFile.SetValue("DAPPL", appl);
                        docFile.SetValue("STORAGE_CAT", "XD");
                        docFile.SetValue("FILENAME", attch.docFile);

                        IRfcTable contentFile = checkinFunc.GetTable("PT_CONTENT");
                        byte[] content = ReadAllBytes(attch.origFile);
                        int fileSize = content.Length;
                        int remaining = content.Length;

                        int xPos = 0;

                        int rowCount = 0;
                        while (xPos < fileSize)
                        {
                            byte[] rowData = new byte[2550];

                            contentFile.Append();
                            if (xPos + 2550 > fileSize)
                            {
                                System.Array.Copy(content, xPos, rowData, 0, fileSize - xPos);
                                contentFile.SetValue("ORBkl", fileSize - xPos);
                                contentFile.SetValue("orblk", rowData);
                            }
                            else
                            {
                                System.Array.Copy(content, xPos, rowData, 0, 2550);
                                contentFile.SetValue("ORBkl", 2550);
                                contentFile.SetValue("orblk", rowData);
                            }
                            contentFile.SetValue("appnr", '1');
                            contentFile.SetValue("orln", fileSize);
                            contentFile.SetValue("zaehl", ++rowCount);
                            xPos += 2550;
                        }
                        checkinFunc.Invoke(rfcDestination);
                        IRfcStructure message = checkinFunc.GetStructure("PSX_MESSAGE");
                        Console.WriteLine("Check-in Status: " + message.GetString("MSG_TXT"));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Create MRI document error: " + ex.Message);
        }
        return result;
    }


    private byte[] ReadAllBytes(string fileName)
    {
        byte[] buffer = null;
        using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            buffer = new byte[fs.Length];
            fs.Read(buffer, 0, (int)fs.Length);
        }
        return buffer;
    }

    public RfcResult saveMRIDocument(DMSDocument doc, MRIClass mri)
    {
        RfcResult result = new RfcResult();
        try
        {
            if (rfcDestination == null)
            {
                rfcDestination = RfcDestinationManager.GetDestination(System.Configuration.ConfigurationManager.AppSettings["SAP_SYSTEMNAME"]);
            }

            RfcRepository rfcRepo = rfcDestination.Repository;            
            IRfcFunction createFunc = rfcRepo.CreateFunction("ZPMEN158_DMS_MAINTAIN");
            createFunc.SetValue("IM_DOCUMENTTYPE", doc.docType);
            createFunc.SetValue("IM_DOCUMENTNUMBER", doc.docNo);
            createFunc.SetValue("IM_DOCUMENTPART", doc.docPart);
            createFunc.SetValue("IM_DOCUMENTVERSION", doc.docVersion);
            createFunc.SetValue("IM_LAB_OFFICE", doc.laboratory);

            IRfcTable allocTab = createFunc.GetTable("IT_ALLOC");
            allocTab.Append();
            allocTab.SetValue("CLASSTYPE", "017");
            allocTab.SetValue("CLASSNAME", "Z_MRI");

            IRfcTable mriClass = createFunc.GetTable("IT_CHARACT");
            foreach (String acModel in mri.acModel)
            {
                if (acModel != null && acModel.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_AC_MODEL");
                    mriClass.SetValue("CHARVALUE", acModel);
                }
            }

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_MPD_REV_NUMBER");
            mriClass.SetValue("CHARVALUE", mri.revNumber);

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_DOC_REVDATE");
            mriClass.SetValue("CHARVALUE", mri.revDate);

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_TASK_REV_CODE");
            mriClass.SetValue("CHARVALUE", mri.revCode);

            foreach (String acZone in mri.acZone)
            {
                if (acZone != null && acZone.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_AC_ZONE");

                    if (acZone.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", acZone.Substring(11)); ;
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", acZone);
                    }
                }
            }

            foreach (String mpSource in mri.mpSource)
            {
                if (mpSource != null && mpSource.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_MP_SOURCE");
                    if (mpSource.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", mpSource.Substring(11)); 
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", mpSource);
                    }                    
                }
            }

            foreach (String taskSection in mri.taskSection)
            {
                if (taskSection != null && taskSection.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_TASK_SECTION");
                    if (taskSection.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", taskSection.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", taskSection);
                    }
                }
            }

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_SAM_PERCENT");
            mriClass.SetValue("CHARVALUE", mri.samplingPercent);

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_WORK_TYPE");
            mriClass.SetValue("CHARVALUE", mri.workType);

            foreach (String inspectSpecial in mri.inspectSpecial)
            {
                if (inspectSpecial != null && inspectSpecial.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_INSP_SPECIAL1");
                    if (inspectSpecial.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", inspectSpecial.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", inspectSpecial);
                    }
                }
            }

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_MP_TASK_NUMBER");
            mriClass.SetValue("CHARVALUE", mri.mpTaskNumber);

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_MP_TG_TASK_NUMBER");
            mriClass.SetValue("CHARVALUE", mri.tgTaskNumber);

            if (mri.validFrom != null && mri.validFrom.Trim().Length > 0)
            {
                DateTime validFrom = DateTime.ParseExact(mri.validFrom, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                mriClass.Append();
                mriClass.SetValue("CLASSTYPE", "017");
                mriClass.SetValue("CLASSNAME", "Z_MRI");
                mriClass.SetValue("CHARNAME", "Z_VALID_FROM");
                mriClass.SetValue("CHARVALUE", validFrom.ToString("dd.MM.yyyy"));
            }

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_MP_TASK_STATUS");
            mriClass.SetValue("CHARVALUE", mri.mpTaskStatus);

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_MP_TASK_MH");
            mriClass.SetValue("CHARVALUE", mri.mpTaskMH);

            foreach (String accessPanel in mri.accessPanel)
            {
                if (accessPanel != null && accessPanel.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_ACCESS_PANEL_NUMBER");
                    if (accessPanel.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", accessPanel.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", accessPanel);
                    }                    
                }
            }

            foreach (String mpdOffset in mri.mpdOffset)
            {
                if (mpdOffset != null && mpdOffset.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_MPD_OFFSET");
                    if (mpdOffset.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", mpdOffset.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", mpdOffset);
                    }
                }
            }

            foreach (String mpdLimitInterval in mri.mpdLimitInterval)
            {
                if (mpdLimitInterval != null && mpdLimitInterval.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_MPD_LIMIT-INTERVAL");
                    if (mpdLimitInterval.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", mpdLimitInterval.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", mpdLimitInterval);
                    }
                }
            }

            foreach (String mpdOffsetSample in mri.mpdOffsetSample)
            {
                if (mpdOffsetSample != null && mpdOffsetSample.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_MPD_OFFSET_SAMPLE");
                    if (mpdOffsetSample.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", mpdOffsetSample.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", mpdOffsetSample);
                    }
                }
            }

            foreach (String mpdIntervalSample in mri.mpdIntervalSample)
            {
                if (mpdIntervalSample != null && mpdIntervalSample.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_MPD_LIMIT-INTERVAL_SAMPLE");
                    if (mpdIntervalSample.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", mpdIntervalSample.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", mpdIntervalSample);
                    }
                }
            }

            foreach (String tgTaskOffset in mri.tgTaskOffset)
            {
                if (tgTaskOffset != null && tgTaskOffset.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_TG_OFFSET");
                    if (tgTaskOffset.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", tgTaskOffset.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", tgTaskOffset);
                    }
                }
            }

            foreach (String tgTaskLimit in mri.tgTaskLimit)
            {
                if (tgTaskLimit != null && tgTaskLimit.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_TG_LIMIT-INTERVAL");
                    if (tgTaskLimit.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", tgTaskLimit.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", tgTaskLimit);
                    }
                }
            }

            foreach (String tgTaskOffsetSample in mri.tgTaskOffsetSample)
            {
                if (tgTaskOffsetSample != null && tgTaskOffsetSample.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_TG_OFFSET_SAMPLE");
                    if (tgTaskOffsetSample.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", tgTaskOffsetSample.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", tgTaskOffsetSample);
                    }
                }
            }

            foreach (String tgTaskLimitSample in mri.tgTaskLimitSample)
            {
                if (tgTaskLimitSample != null && tgTaskLimitSample.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_TG_LIMIT-INTERVAL_SAMPLE");
                    if (tgTaskLimitSample.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", tgTaskLimitSample.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", tgTaskLimitSample);
                    }
                }
            }

            foreach (String mpdEngineEff in mri.mpdEngineEff)
            {
                if (mpdEngineEff != null && mpdEngineEff.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_MPD_APPLICABILITY_ENG");
                    if (mpdEngineEff.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", mpdEngineEff.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", mpdEngineEff);
                    }
                }
            }

            foreach (String mpdAircraftEff in mri.mpdAircraftEff)
            {
                if (mpdAircraftEff != null && mpdAircraftEff.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_MPD_APPLICABILITY_AC");
                    if (mpdAircraftEff.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", mpdAircraftEff.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", mpdAircraftEff);
                    }
                }
            }

            foreach (String tgAircraftEff in mri.tgAircraftEff)
            {
                if (tgAircraftEff != null && tgAircraftEff.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_TG_APPLICABILITY_AC");
                    if (tgAircraftEff.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", tgAircraftEff.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", tgAircraftEff);
                    }
                }
            }

            foreach (String mpdReference in mri.mpdReference)
            {
                if (mpdReference != null && mpdReference.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_MPD_REFERENCE_DOCUMENT");
                    if (mpdReference.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", mpdReference.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", mpdReference);
                    }
                }
            }

            foreach (String otherReference in mri.otherReference)
            {
                if (otherReference != null && otherReference.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_REFERENCE_DOCUMENT");
                    if (otherReference.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", otherReference.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", otherReference);
                    }
                }
            }

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_DETCD");
            mriClass.SetValue("CHARVALUE", mri.detecCode);

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_ACTK_FINDING_REPORT_REQ");
            mriClass.SetValue("CHARVALUE", mri.inspReportReq);

            foreach (String tgIntervalOffsetNote in mri.tgIntervalOffsetNote)
            {
                if (tgIntervalOffsetNote != null && tgIntervalOffsetNote.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_TG_INT_OFFSET_NOTE");
                    if (tgIntervalOffsetNote.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", tgIntervalOffsetNote.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", tgIntervalOffsetNote);
                    }
                }
            }

            foreach (String tgDepending in mri.tgDepending)
            {
                if (tgDepending != null && tgDepending.Trim().Length > 0)
                {
                    mriClass.Append();
                    mriClass.SetValue("CLASSTYPE", "017");
                    mriClass.SetValue("CLASSNAME", "Z_MRI");
                    mriClass.SetValue("CHARNAME", "Z_TG_DEPENDING");
                    if (tgDepending.StartsWith("X-DELETED-X"))
                    {
                        mriClass.SetValue("CHARVALUE", tgDepending.Substring(11));
                        mriClass.SetValue("DELETEVALUE", 'X');
                    }
                    else
                    {
                        mriClass.SetValue("CHARVALUE", tgDepending);
                    }
                }
            }

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_REASON");
            mriClass.SetValue("CHARVALUE", mri.reason);

            mriClass.Append();
            mriClass.SetValue("CLASSTYPE", "017");
            mriClass.SetValue("CLASSNAME", "Z_MRI");
            mriClass.SetValue("CHARNAME", "Z_ENGINEERING_NOTE");
            mriClass.SetValue("CHARVALUE", mri.engineeringNote);

            /*
                        IRfcTable longTextTab = createFunc.GetTable("LT_LONGTXT");
                        longTextTab.Append();
                        longTextTab.SetValue("DELETEVALUE", ' ');
                        longTextTab.SetValue("LANGUAGE", 'S');
                        longTextTab.SetValue("LANGUAGE_ISO", "EN");
                        longTextTab.SetValue("TEXTLINE", "TEXT LINE");

                        IRfcTable descTab = createFunc.GetTable("LT_DESC");
                        descTab.Append();
                        descTab.SetValue("DELETEVALUE", ' ');
                        descTab.SetValue("LANGUAGE", 'S');
                        descTab.SetValue("LANGUAGE_ISO", "EN");
                        descTab.SetValue("DESCRIPTION", "DESCRIPTION");
                        descTab.SetValue("TEXTINDICATOR", 'X');            
            */

            IRfcTable descTable = createFunc.GetTable("IT_DESC");
            descTable.Append();
            descTable.SetValue("DELETEVALUE", ' ');
            descTable.SetValue("LANGUAGE", 'E');
            descTable.SetValue("LANGUAGE_ISO", "EN");
            descTable.SetValue("DESCRIPTION", doc.description);
            descTable.SetValue("TEXTINDICATOR", ' ');

            createFunc.Invoke(rfcDestination);

            IRfcStructure createResult = createFunc.GetStructure("EX_RETURN");
            char iResult = createResult.GetChar("TYPE");

            if (iResult == 'S' || iResult == 'W' || iResult == ' ')
            {
                result.result = true;
                result.docNumber = doc.docNo;
                result.docPart = doc.docPart;
                result.docType = doc.docType;
                result.docVersion = doc.docVersion;
                result.message = $"{doc.docNo}-{doc.docType}-{doc.docPart}-{doc.docVersion} has been updated successfully";
            }
            else
            {
                result.result = false;
                result.message = createResult.GetString("MESSAGE");
            }

            
            foreach (dmsFile attch in doc.attachments)
            {
                if (attch.updateStatus != null && attch.updateStatus.Equals("N"))
                {                    
                    IRfcFunction checkinFunc = rfcRepo.CreateFunction("CVAPI_DOC_CHECKIN");
                    checkinFunc.SetValue("PF_DOKAR", doc.docType);
                    checkinFunc.SetValue("PF_DOKNR", doc.docNo);
                    checkinFunc.SetValue("PF_DOKTL", doc.docPart);
                    checkinFunc.SetValue("PF_DOKVR", doc.docVersion);
                    checkinFunc.SetValue("PF_CONTENT_PROVIDE", "TBL");

                    IRfcTable docFile = checkinFunc.GetTable("PT_FILES_X");
                    docFile.Append();
                    String appl = attch.application.ToUpper();
                    switch (appl)
                    {
                        case "PPT": appl = "PPT"; break;
                        case "PPTX": appl = "PPT"; break;
                        case "DOC": appl = "WWI"; break;
                        case "DOCX": appl = "WWI"; break;
                        case "JPG": appl = "GRF"; break;
                        case "PNG": appl = "GRF"; break;
                        case "GIF": appl = "GRF"; break;
                        case "XLS": appl = "XLS"; break;
                        case "XLSX": appl = "XLS"; break;
                        case "ZIP": appl = "ZIP"; break;
                        case "RAR": appl = "ZIP"; break;
                    }
                    docFile.SetValue("DAPPL", appl);
                    docFile.SetValue("STORAGE_CAT", "XD");
                    docFile.SetValue("FILENAME", attch.docFile);

                    IRfcTable contentFile = checkinFunc.GetTable("PT_CONTENT");
                    byte[] content = ReadAllBytes(attch.origFile);
                    int fileSize = content.Length;
                    int remaining = content.Length;

                    int xPos = 0;

                    int rowCount = 0;
                    while (xPos < fileSize)
                    {
                        byte[] rowData = new byte[2550];

                        contentFile.Append();
                        if (xPos + 2550 > fileSize)
                        {
                            System.Array.Copy(content, xPos, rowData, 0, fileSize - xPos);
                            contentFile.SetValue("ORBkl", fileSize - xPos);
                            contentFile.SetValue("orblk", rowData);
                        }
                        else
                        {
                            System.Array.Copy(content, xPos, rowData, 0, 2550);
                            contentFile.SetValue("ORBkl", 2550);
                            contentFile.SetValue("orblk", rowData);
                        }
                        contentFile.SetValue("appnr", '1');
                        contentFile.SetValue("orln", fileSize);
                        contentFile.SetValue("zaehl", ++rowCount);
                        xPos += 2550;                                                
                    }
                    checkinFunc.Invoke(rfcDestination);
                    IRfcStructure message = checkinFunc.GetStructure("PSX_MESSAGE");
                    Console.WriteLine("Check-in Status: " + message.GetString("MSG_TXT"));
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Save MRI document error: " + ex.Message);
        }
        return result;
    }

    public List<MRI_Charact_Values> getMRICharacteristics()
    {
        RfcResult result = new RfcResult();
        try
        {
            List<MRI_Charact_Values> list = new List<MRI_Charact_Values>();
            if (rfcDestination == null)
            {
                rfcDestination = RfcDestinationManager.GetDestination(System.Configuration.ConfigurationManager.AppSettings["SAP_SYSTEMNAME"]);
            }

            RfcRepository rfcRepo = rfcDestination.Repository;
            IRfcFunction createFunc = rfcRepo.CreateFunction("BAPI_CLASS_GET_CHARACTERISTICS");
            createFunc.SetValue("CLASSNUM", "Z_MRI");
            createFunc.SetValue("CLASSTYPE", "017");
            createFunc.SetValue("LANGU_ISO", "EN");
            createFunc.SetValue("WITH_VALUES", "X");

            createFunc.Invoke(rfcDestination);
            IRfcTable charValues = createFunc.GetTable("CHAR_VALUES");

            
            foreach (IRfcStructure val in charValues)
            {
                MRI_Charact_Values allowVal = new MRI_Charact_Values(val.GetString("NAME_CHAR"), val.GetString("CHAR_VALUE"), val.GetString("DESCR_CVAL"));
                list.Add(allowVal);
            }
            return list;
        }
        catch (Exception e)
        {
            throw new Exception("Get MRI characteristics values error: " + e.Message);
        }
    }

    public List<DMSDocument> searchDMS(String docType, String docNo, String docPart, String docVers, String persID, String labOffice, String docStatus)
    {
        try
        {
            List<DMSDocument> list = new List<DMSDocument>();
            if (rfcDestination == null)
            {
                rfcDestination = RfcDestinationManager.GetDestination(System.Configuration.ConfigurationManager.AppSettings["SAP_SYSTEMNAME"]);
            }

            RfcRepository rfcRepo = rfcDestination.Repository;
            IRfcFunction createFunc = rfcRepo.CreateFunction("ZPMEN158_DMS_SEARCH");
            if (docType != null && docType.Trim().Length > 0)
                createFunc.SetValue("DOKAR", docType);

            if (docNo != null && docNo.Trim().Length > 0)
                createFunc.SetValue("DOKNR", docNo);

            if (docPart != null && docPart.Trim().Length > 0)
                createFunc.SetValue("DOKTL", docPart);

            if (docVers != null && docVers.Trim().Length > 0)
                createFunc.SetValue("DOKVR", docVers);

            if (persID != null && persID.Trim().Length > 0)
                createFunc.SetValue("DWNAM", persID);

            if (labOffice != null && labOffice.Trim().Length > 0)
                createFunc.SetValue("LABOR", labOffice);

            if (docStatus != null && docStatus.Trim().Length > 0)
                createFunc.SetValue("DOKST", docStatus);

            createFunc.Invoke(rfcDestination);
            IRfcTable documents = createFunc.GetTable("DOC_RESULT");


            foreach (IRfcStructure doc in documents)
            {
                DMSDocument document = new DMSDocument();
                document.docType = doc.GetString("DOKAR");
                document.docNo = doc.GetString("DOKNR");
                document.docPart = doc.GetString("DOKTL");
                document.docVersion = doc.GetString("DOKVR");
                document.userName = doc.GetString("DWNAM");
                document.labCode = doc.GetString("LABOR");
                document.laboratory = doc.GetString("LABORATORY");
                document.description = doc.GetString("DKTXT");
                document.status = doc.GetString("DOKST");
                list.Add(document);
            }
            return list;
        }
        catch (Exception e)
        {
            throw new Exception("Get MRI characteristics values error: " + e.Message);
        }
    }

    public PersData getPersonnelDetail(String userID)
    {
        try
        {
            if (rfcDestination == null)
            {
                rfcDestination = RfcDestinationManager.GetDestination(System.Configuration.ConfigurationManager.AppSettings["SAP_SYSTEMNAME"]);
            }

            RfcRepository rfcRepo = rfcDestination.Repository;
            IRfcFunction createFunc = rfcRepo.CreateFunction("ZHRBAPI_GET_PERSDETAIL");

            String persID = String.Format("{0:00000000}", Convert.ToInt32(userID.Substring(2)));
            createFunc.SetValue("PERSID", persID);
            createFunc.Invoke(rfcDestination);
            PersData pers = new PersData();
            pers.firstName = createFunc.GetString("FNAME");
            pers.lastName = createFunc.GetString("LNAME");
            pers.thaiFirstName = createFunc.GetString("TFNAME");
            pers.thaiLastName = createFunc.GetString("TLNAME");
            pers.doe = createFunc.GetString("DOE");
            pers.funcCode = createFunc.GetString("FUNC");
            pers.position = createFunc.GetString("POSI");
            return pers;
        }
        catch (Exception e)
        {
            throw new Exception("Get MRI characteristics values error: " + e.Message);
        }
    }

    public List<Aircraft>   getAircraftDetail()
    {
        List<Aircraft> acregs = new List<Aircraft>();
        List<MRI_Charact_Values> char_values = getMRICharacteristics();
        try
        {
            if (rfcDestination == null)
            {
                rfcDestination = RfcDestinationManager.GetDestination(System.Configuration.ConfigurationManager.AppSettings["SAP_SYSTEMNAME"]);
            }
                       
            RfcRepository rfcRepo = rfcDestination.Repository;
            IRfcFunction createFunc = rfcRepo.CreateFunction("ZPMFUNC_GETACTYPE");

            foreach (MRI_Charact_Values value in char_values)
            {
            
                if (value.characteristic!=null && value.characteristic.Trim().Equals("Z_TG_APPLICABILITY_AC"))
                {
                    String actype = "";
                    try
                    {
                        createFunc.SetValue("ACREG", value.value);
                        createFunc.Invoke(rfcDestination);
                        actype = createFunc.GetString("ACTYPE");
                    }catch (Exception)
                    {
                        actype = "N/A";
                    }
                    Aircraft acreg = new Aircraft(value.value, actype, value.description);
                    acregs.Add(acreg);
                }
            }
            return acregs;
        }
        catch (Exception e)
        {
            throw new Exception("Get MRI characteristics values error: " + e.Message);
        }
    }

    public List<InspectSpecial> getAllInspectSpecial()
    {
        List<InspectSpecial> inspSpecials = new List<InspectSpecial>();
        List<MRI_Charact_Values> char_values = getMRICharacteristics();
        try
        {
            inspSpecials.Add(new InspectSpecial("", ""));
            foreach (MRI_Charact_Values value in char_values)
            {
                if (value.characteristic != null && value.characteristic.Trim().Equals("Z_INSP_SPECIAL1"))
                {                                     
                    InspectSpecial inspSpecial = new InspectSpecial(value.value, value.description);
                    inspSpecials.Add(inspSpecial);
                }
            }
            return inspSpecials;
        }
        catch (Exception e)
        {
            throw new Exception("Get MRI characteristics values error: " + e.Message);
        }
    }

    public List<WorkType> getAllWorkType()
    {
        List<WorkType> workTypes = new List<WorkType>();
        List<MRI_Charact_Values> char_values = getMRICharacteristics();
        try
        {
            workTypes.Add(new WorkType("", ""));
            foreach (MRI_Charact_Values value in char_values)
            {
                if (value.characteristic != null && value.characteristic.Trim().Equals("Z_WORK_TYPE"))
                {
                    WorkType workType = new WorkType(value.value, value.description);
                    workTypes.Add(workType);
                }
            }
            return workTypes;
        }
        catch (Exception e)
        {
            throw new Exception("Get MRI characteristics values error: " + e.Message);
        }
    }

    public List<RevisionCode> getAllRevisionCode()
    {
        List<RevisionCode> revisionCodes = new List<RevisionCode>();
        List<MRI_Charact_Values> char_values = getMRICharacteristics();
        try
        {
            revisionCodes.Add(new RevisionCode("", ""));
            foreach (MRI_Charact_Values value in char_values)
            {
                if (value.characteristic != null && value.characteristic.Trim().Equals("Z_TASK_REV_CODE"))
                {
                    RevisionCode revisionCode = new RevisionCode(value.value, value.description);
                    revisionCodes.Add(revisionCode);
                }
            }
            return revisionCodes;
        }
        catch (Exception e)
        {
            throw new Exception("Get MRI characteristics values error: " + e.Message);
        }
    }

    public List<MPSource> getAllMPSource()
    {
        List<MPSource> mpSources = new List<MPSource>();
        List<MRI_Charact_Values> char_values = getMRICharacteristics();
        try
        {
            mpSources.Add(new MPSource("", ""));
            foreach (MRI_Charact_Values value in char_values)
            {            
                if (value.characteristic != null && value.characteristic.Trim().Equals("Z_MP_SOURCE"))
                {
                    MPSource mpSource = new MPSource(value.value, value.description);
                    mpSources.Add(mpSource);
                }
            }
            return mpSources;
        }
        catch (Exception e)
        {
            throw new Exception("Get MRI characteristics values error: " + e.Message);
        }
    }

    public List<TaskSection> getAllTaskSection()
    {
        List<TaskSection> taskSections = new List<TaskSection>();
        List<MRI_Charact_Values> char_values = getMRICharacteristics();
        try
        {
            taskSections.Add(new TaskSection("", ""));
            foreach (MRI_Charact_Values value in char_values)
            {
                if (value.characteristic != null && value.characteristic.Trim().Equals("Z_TASK_SECTION"))
                {
                    TaskSection taskSection = new TaskSection(value.value, value.description);
                    taskSections.Add(taskSection);
                }
            }
            return taskSections;
        }
        catch (Exception e)
        {
            throw new Exception("Get MRI characteristics values error: " + e.Message);
        }
    }

    public List<DetectCode> getAllDetectCode()
    {
        List<DetectCode> detectCodes = new List<DetectCode>();
        List<MRI_Charact_Values> char_values = getMRICharacteristics();
        try
        {
            detectCodes.Add(new DetectCode("", ""));
            foreach (MRI_Charact_Values value in char_values)
            {
                if (value.characteristic != null && value.characteristic.Trim().Equals("Z_DETCD"))
                {
                    DetectCode detectCode = new DetectCode(value.value, value.description);
                    detectCodes.Add(detectCode);
                }
            }
            return detectCodes;
        }
        catch (Exception e)
        {
            throw new Exception("Get MRI characteristics values error: " + e.Message);
        }
    }

    public byte[] getDocumentContent(String fileID,out String contentType, out String outFile)
    {
        try
        {
            List<DMSDocument> list = new List<DMSDocument>();
            if (rfcDestination == null)
            {
                rfcDestination = RfcDestinationManager.GetDestination(System.Configuration.ConfigurationManager.AppSettings["SAP_SYSTEMNAME"]);
            }

            RfcRepository rfcRepo = rfcDestination.Repository;
            IRfcFunction createFunc = rfcRepo.CreateFunction("SDOK_PHIO_LOAD_CONTENT");
            IRfcStructure docObj = createFunc.GetStructure("OBJECT_ID");
            docObj.SetValue("CLASS", "DMS_PCD1");
            docObj.SetValue("OBJID", fileID);
            createFunc.SetValue("RAW_MODE", 'X');            
            createFunc.Invoke(rfcDestination);

            IRfcTable docInfo = createFunc.GetTable("FILE_ACCESS_INFO");
            contentType = docInfo.GetString("MIMETYPE");
            outFile = docInfo.GetString("FILE_NAME");
            IRfcTable lines = createFunc.GetTable("FILE_CONTENT_BINARY");
            int size = docInfo.GetInt("FILE_SIZE");
            byte[] bStr = new byte[size];
            int xPos = 0;
            foreach (IRfcStructure line in lines)
            {
                byte[] buf = line.GetByteArray("LINE");
                if (xPos + buf.Length > size)
                {
                    System.Array.Copy(buf, 0, bStr, xPos, size - xPos);
                }
                else
                {
                    System.Array.Copy(buf, 0, bStr, xPos, buf.Length);
                }
                xPos += buf.Length;
            }
            return bStr;
        }
        catch (Exception e)
        {
            throw new Exception("Get Document Content Error: " + e.Message);
        }
    }
}
