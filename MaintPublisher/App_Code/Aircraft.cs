using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Aircraft
/// </summary>
public class Aircraft
{
    public String acreg;
    public String actype;
    public String displayAC;

    public Aircraft()
    {
        this.acreg = this.actype = this.displayAC = "";
    }

    public Aircraft(String acreg, String actype, String displayAC)
    {
        this.acreg = acreg;
        this.actype = actype;
        this.displayAC = displayAC;
    }
}