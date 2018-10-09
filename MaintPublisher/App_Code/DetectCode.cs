using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DetectCode
/// </summary>
public class DetectCode
{
    public String detectCode;
    public String detectText;
    public DetectCode()
    {
        //
        // TODO: Add constructor logic here
        //
        this.detectCode = this.detectText = "";
    }

    public DetectCode(String value, String text)
    {
        this.detectCode = value;
        this.detectText = text;
    }
}