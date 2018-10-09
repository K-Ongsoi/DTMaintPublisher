using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for WorkType
/// </summary>
public class WorkType
{
    public String workValue;
    public String workText;

    public WorkType()
    {
        //
        // TODO: Add constructor logic here
        //
        this.workValue = this.workText = "";
    }

    public WorkType(String value, String text)
    {
        this.workValue = value;
        this.workText = text;
    }
}