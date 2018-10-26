using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for RfcResult
/// </summary>
public class RfcResult
{
    public bool result;
    public String message;
    public String docNumber;
    public String docType;
    public String docPart;
    public String docVersion;

    public RfcResult()
    {
        this.result = false;
        this.message = "";
    }
}