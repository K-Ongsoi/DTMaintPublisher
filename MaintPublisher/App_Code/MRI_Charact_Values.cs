using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for MRI_Charact_Values
/// </summary>
public class MRI_Charact_Values
{
    public String characteristic;
    public String value;
    public String description;

    public MRI_Charact_Values()
    {
        this.characteristic = this.value = this.description = "" ;
    }

    public MRI_Charact_Values(String charact, String val, String description)
    {
        this.characteristic = charact;
        this.value = val;
        this.description = description;
    }
}