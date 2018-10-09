using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for RevisionCode
/// </summary>
public class RevisionCode
{
    public String revisionCode;
    public String revisionText;

    public RevisionCode()
    {
        //
        // TODO: Add constructor logic here
        //
        this.revisionCode = this.revisionText = "";
    }

    public RevisionCode(String value, String text)
    {
        this.revisionCode = value;
        this.revisionText = text;
    }
}