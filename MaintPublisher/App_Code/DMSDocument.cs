using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DMSDocument
/// </summary>
public class DocCharacteristics
{
    public List<String> acModel;
    public String revNumber;
    public String revDate;
    public String revCode;
    public List<String> acZone;
    public List<String> mpSource;
    public List<String> taskSection;
    public String samplingPercent;
    public String workType;
    public List<String> inspectSpecial;
    public String mpTaskNumber;
    public String tgTaskNumber;
    public String validFrom;
    public String mpTaskStatus;
    public String mpTaskMH;
    public List<String> accessPanel;
    public List<String> mpdOffset;
    public List<String> mpdLimitInterval;
    public List<String> mpdOffsetSample;
    public List<String> mpdIntervalSample;
    public List<String> tgTaskOffset;
    public List<String> tgTaskLimit;
    public List<String> tgTaskOffsetSample;
    public List<String> tgTaskLimitSample;
    public List<String> mpdEngineEff;
    public List<String> mpdAircraftEff;
    public List<String> tgAircraftEff;
    public List<String> mpdReference;
    public List<String> otherReference;
    public String detecCode;
    public String inspReportReq;
    public List<String> tgIntervalOffsetNote;
    public List<String> tgDepending;
    public String reason;
    public String engineeringNote;
}

public class dmsFile
{
    public String originalType;
    public String storageCategory;
    public String application;
    public String origFile;
    public String docFile;
    public String applicationID;
    public String fileID;
    public String checkedIN;
    public String activeVersion;
    public String updateStatus;
}

public class DMSDocument
{
    public String docNo;
    public String docType;
    public String docVersion;
    public String docPart;
    public String description;
    public String userName;
    public String status;
    public String labCode;
    public String laboratory;
    public DocCharacteristics characteristics;
    public List<dmsFile> attachments;
}