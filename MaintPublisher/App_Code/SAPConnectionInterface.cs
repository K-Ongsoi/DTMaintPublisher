using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAP.Middleware.Connector;

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
            //document.SetValue("DOCUMENTVERSION", doc.docVersion);            
            //document.SetValue("DOCUMENTPART", doc.docPart);
            document.SetValue("DESCRIPTION", doc.description);
            document.SetValue("USERNAME", doc.userName);
            //document.SetValue("STATUSINTERN", doc.status);
            document.SetValue("LABORATORY", doc.laboratory);

            IRfcStructure mriClass = rfcRepo.GetStructureMetadata("ZPMS158_DMS_MRI").CreateStructure();
            mriClass.SetValue("Z_AC_MODEL", mri.acModel);
            mriClass.SetValue("Z_MPD_REV_NUMBER", mri.revNumber);
            mriClass.SetValue("Z_DOC_REVDATE", mri.revDate);
            mriClass.SetValue("Z_TASK_REV_CODE", mri.revCode);
            mriClass.SetValue("Z_AC_ZONE", mri.acZone);
            mriClass.SetValue("Z_MP_SOURCE", mri.mpSource);
            mriClass.SetValue("Z_TASK_SECTION", mri.taskSection);
            mriClass.SetValue("Z_SAM_PERCENT", mri.samplingPercent);
            mriClass.SetValue("Z_WORK_TYPE", mri.workType);
            mriClass.SetValue("Z_INSP_SPECIAL1", mri.inspectSpecial);
            mriClass.SetValue("Z_MP_TASK_NUMBER", mri.mpTaskNumber);
            mriClass.SetValue("Z_MP_TG_TASK_NUMBER", mri.tgTaskNumber);
            mriClass.SetValue("Z_VALID_FROM", mri.validFrom);
            mriClass.SetValue("Z_MP_TASK_STATUS", mri.mpTaskStatus);
            mriClass.SetValue("Z_MP_TASK_MH", mri.mpTaskMH);
            mriClass.SetValue("Z_ACCESS_PANEL_NUMBER", mri.accessPanel);
            mriClass.SetValue("Z_MPD_OFFSET", mri.mpdOffset);
            mriClass.SetValue("Z_MPD_LIMIT_INTERVAL", mri.mpdLimitInterval);
            mriClass.SetValue("Z_MPD_OFFSET_SAMPLE", mri.mpdOffsetSample);
            mriClass.SetValue("Z_MPD_LIMIT_INTERVAL_SAMPLE", mri.mpdIntervalSample);
            mriClass.SetValue("Z_TG_OFFSET", mri.tgTaskOffset);
            mriClass.SetValue("Z_TG_LIMIT_INTERVAL", mri.tgTaskLimit);
            mriClass.SetValue("Z_TG_OFFSET_SAMPLE", mri.tgTaskOffsetSample);
            mriClass.SetValue("Z_TG_LIMIT_INTERVAL_SAMPLE", mri.tgTaskLimitSample);
            mriClass.SetValue("Z_MPD_APPLICABILITY_ENG", mri.mpdEngineEff);
            mriClass.SetValue("Z_MPD_APPLICABILITY_AC", mri.mpdAircraftEff);
            mriClass.SetValue("Z_TG_APPLICABILITY_AC", mri.tgAircraftEff);
            mriClass.SetValue("Z_MPD_REFERENCE_DOCUMENT", mri.mpdReference);
            mriClass.SetValue("Z_REFERENCE_DOCUMENT", mri.otherReference);
            mriClass.SetValue("Z_DETCD", mri.detecCode);
            mriClass.SetValue("Z_ACTK_FINDING_REPORT_REQ", mri.inspReportReq);
            mriClass.SetValue("Z_TG_INT_OFFSET_NOTE", mri.tgIntervalOffsetNote);
            mriClass.SetValue("Z_TG_DEPENDING", mri.tgDepending);
            mriClass.SetValue("Z_REASON", mri.reason);
            mriClass.SetValue("Z_ENGINEERING_NOTE", mri.engineeringNote);                                 

            IRfcFunction createFunc = rfcRepo.CreateFunction("ZPMEN158_DMS_CREATE");
            createFunc.SetValue("IM_DOCUMENT", document);
            createFunc.SetValue("IM_CLASS_MRI", mriClass);
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
            createFunc.Invoke(rfcDestination);

            IRfcStructure createResult = createFunc.GetStructure("EX_RETURN");
            char iResult = createResult.GetChar("TYPE");

            if (iResult == 'S' || iResult == 'W')
            {
                result.result = true;
                String docType = createFunc.GetString("EX_DOCTYPE");
                String docNumber = createFunc.GetString("EX_DOCNUMBER");
                String docPart = createFunc.GetString("EX_DOCPART");
                String docVersion = createFunc.GetString("EX_DOCVERSION");
                result.message = $"{docNumber}-{docType}-{docPart}-{docVersion} has been created successfully";
            }
            else
            {
                result.result = false;
                result.message = createResult.GetString("MESSAGE");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Create MRI document error: " + ex.Message);
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
                MRI_Charact_Values allowVal = new MRI_Charact_Values(val.GetString("NAME_CHAR"), val.GetString("CHAR_VALUE"));
                list.Add(allowVal);
            }
            return list;
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
                    Aircraft acreg = new Aircraft(value.value, actype);
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
}
