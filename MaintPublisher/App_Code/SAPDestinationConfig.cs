using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using SAP.Middleware.Connector;

/// <summary>
/// Summary description for SAPDestinationConfig
/// </summary>
public class SAPDestinationConfig: IDestinationConfiguration
{
    public event RfcDestinationManager.ConfigurationChangeHandler ConfigurationChanged;   

    public RfcConfigParameters GetParameters(string destinationName)
    {
        RfcConfigParameters parms = new RfcConfigParameters();

        if (destinationName == null || destinationName.Trim().Length == 0) return parms;

        switch (destinationName)
        {
            case "TED":
                        parms.Add(RfcConfigParameters.Name, destinationName);
                        parms.Add(RfcConfigParameters.AppServerHost, "siamted.dt.thaiairways.co.th");
                        parms.Add(RfcConfigParameters.SystemNumber, "00");
                        parms.Add(RfcConfigParameters.Client, "300");
                        parms.Add(RfcConfigParameters.SystemID, destinationName);
                        parms.Add(RfcConfigParameters.User, "webrfc");
                        parms.Add(RfcConfigParameters.Password, "siamrfc1");
                        parms.Add(RfcConfigParameters.Language, "EN");
                        parms.Add(RfcConfigParameters.PoolSize, "4");
                        break;
            case "TEQ":
                        parms.Add(RfcConfigParameters.Name, destinationName);
                        parms.Add(RfcConfigParameters.AppServerHost, "siamteq.dt.thaiairways.co.th");
                        parms.Add(RfcConfigParameters.SystemNumber, "02");
                        parms.Add(RfcConfigParameters.Client, "100");
                        parms.Add(RfcConfigParameters.SystemID, destinationName);
                        parms.Add(RfcConfigParameters.User, "webrfc");
                        parms.Add(RfcConfigParameters.Password, "siamrfc1");
                        parms.Add(RfcConfigParameters.Language,"EN");
                        parms.Add(RfcConfigParameters.PoolSize, "8");
                        break;
            case "TEP":
                        parms.Add(RfcConfigParameters.Name, destinationName);
                        parms.Add(RfcConfigParameters.AppServerHost, "siamtep.dt.thaiairways.co.th");
                        parms.Add(RfcConfigParameters.SystemNumber, "05");
                        parms.Add(RfcConfigParameters.Client, "100");
                        parms.Add(RfcConfigParameters.SystemID, destinationName);
                        parms.Add(RfcConfigParameters.User, "webrfc");
                        parms.Add(RfcConfigParameters.Password, "siamrfc1");
                        parms.Add(RfcConfigParameters.Language, "EN");
                        parms.Add(RfcConfigParameters.PoolSize, "16");
                        break;
            default:
                        return parms;
        }

        return parms;
    }

    public bool ChangeEventsSupported()
    {
        return false;
    }
}