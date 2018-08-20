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

    public Aircraft()
    {
        this.acreg = this.actype = "";
    }

    public Aircraft(String acreg, String actype)
    {
        this.acreg = acreg;
        this.actype = actype;
    }
}