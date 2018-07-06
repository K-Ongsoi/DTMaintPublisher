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
// [System.Web.Script.Services.ScriptService]
public class DocumentWS : System.Web.Services.WebService
{
    [WebMethod]
    public string GetAircraftModel()
    {
        List<String> actypes = new List<String>();
        
        actypes.Add("A350-900");
        actypes.Add("A380-800");
        actypes.Add("B747-400");
        actypes.Add("B787-800");

        return JsonConvert.SerializeObject(actypes);
    }

    [WebMethod]
    public String GetAircrafts(String model)
    {
        List<String> fleet = new List<string>();

        switch (model)
        {
            case "B747-400": fleet.Add("HS-TGA"); fleet.Add("HS-TGB"); fleet.Add("HS-TGC");
                break;
            case "B787-800": fleet.Add("HS-TQA"); fleet.Add("HS-TQB"); fleet.Add("HS-TQC"); fleet.Add("HS-TQD");
                break;
            case "A350-900": fleet.Add("HS-THB"); fleet.Add("HS-THC"); fleet.Add("HS-THD"); fleet.Add("HS-THE"); fleet.Add("HS-THF"); fleet.Add("HS-THG");
                break;
            case "A380-800": fleet.Add("HS-TUA"); fleet.Add("HS-TUB");
                break;
        }

        return JsonConvert.SerializeObject(fleet);
    }
}
