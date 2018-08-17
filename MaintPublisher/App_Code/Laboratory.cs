using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Laboratory
/// </summary>
public class Laboratory
{
    public String labCode;
    public String labText;

    public Laboratory()
    {
        labCode = labText = "";
    }

    public Laboratory(String labCode, String labText)
    {
        this.labCode = labCode;
        this.labText = labText;
    }
}