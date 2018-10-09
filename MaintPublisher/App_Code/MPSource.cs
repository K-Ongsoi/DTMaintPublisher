using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for MPSource
/// </summary>
public class MPSource
{
    public String sourceCode;
    public String sourceText;

    public MPSource()
    {
        //
        // TODO: Add constructor logic here
        //
        this.sourceCode = this.sourceText = "";
    }

    public MPSource(String value, String text)
    {
        this.sourceCode = value;
        this.sourceText = text;
    }
}