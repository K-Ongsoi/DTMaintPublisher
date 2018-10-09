using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for InspectSpecial
/// </summary>
public class InspectSpecial
{
    public String inspectValue;
    public String inspectText;    
    
    public InspectSpecial()
    {
        //
        // TODO: Add constructor logic here
        //
        this.inspectValue = this.inspectText = "";
    }

    public InspectSpecial(String value, String text)
    {
        this.inspectValue = value;
        this.inspectText = text;
    }
}