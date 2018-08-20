using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

/// <summary>
/// Summary description for DocumentWS
/// </summary>
[WebService(Namespace = "http://thaitechnical.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class DocumentWS : System.Web.Services.WebService
{
    [WebMethod]
    public String SAPConnectionState()
    {
        SAPConnectionInterface sapConnector = new SAPConnectionInterface();
        bool result = sapConnector.checkConnection();
        return JsonConvert.SerializeObject(result);
    }

    [WebMethod]
    public String createMRIDoc(DMSDocument doc, MRIClass mriClass)
    {
        SAPConnectionInterface sapConnector = new SAPConnectionInterface();        

        RfcResult result = sapConnector.createMRIDocument(doc, mriClass);
        return JsonConvert.SerializeObject(result);
    }

    [WebMethod]
    public String GetLaboratories()
    {
        SAPConnectionInterface sapConnector = new SAPConnectionInterface();

        List<Laboratory> result = sapConnector.listLaboratories();
        return JsonConvert.SerializeObject(result);
    }

    [WebMethod]
    public string GetAircraftModel()
    {
        List<String> actypes = new List<String>();
        SAPConnectionInterface sapConnector = new SAPConnectionInterface();
        List<MRI_Charact_Values> values = sapConnector.getMRICharacteristics();

        foreach (MRI_Charact_Values value in values)
        {
            if (value.characteristic!=null && value.characteristic.Trim().Equals("Z_AC_MODEL"))
            {
                actypes.Add(value.value);
            }
        }

        return JsonConvert.SerializeObject(actypes);
    }

    [WebMethod]
    public String GetAircrafts(String model)
    {
        List<String> fleet = new List<string>();
        SAPConnectionInterface sapConnector = new SAPConnectionInterface();
        List<Aircraft> acregs = sapConnector.getAircraftDetail();
        foreach (Aircraft acreg in acregs)
        {
            if (acreg.actype!=null && acreg.actype.Trim().Equals(model.Trim()))
            {
                fleet.Add(acreg.acreg);
            }
        }
        return JsonConvert.SerializeObject(fleet);
    }

    [WebMethod]
    public String getIntervalUnit()
    {
        List<String> iUnits = new List<string>();

        iUnits.Add("FH");
        iUnits.Add("FC");
        iUnits.Add("MO");
        iUnits.Add("YR");

        return JsonConvert.SerializeObject(iUnits);
    }
}
