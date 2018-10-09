using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for FindingRequire
/// </summary>
public class FindingRequire
{
    public String requireCode;
    public String requireText;
    public FindingRequire()
    {
        //
        // TODO: Add constructor logic here
        //
        this.requireCode = this.requireText = "";
    }

    public FindingRequire(String value, String text)
    {
        this.requireCode = value;
        this.requireText = text;
    }
}