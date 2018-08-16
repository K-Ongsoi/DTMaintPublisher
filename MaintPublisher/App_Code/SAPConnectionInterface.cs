using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAP.Middleware.Connector;

/// <summary>
/// Summary description for SAPConnectionInterface
/// </summary>
public class SAPConnectionInterface
{
    private RfcDestination rfcDestination;

    public bool checkConnection()
    {
        bool result = false;
        try
        {
            rfcDestination = RfcDestinationManager.GetDestination(System.Configuration.ConfigurationManager.AppSettings["SAP_SYSTEMNAME"]);
            if (rfcDestination != null)
            {
                rfcDestination.Ping();
                result = true;
            }            
        }
        catch (Exception ex)
        {
            result = false;
            throw new Exception("Connection Failure Error: " + ex.Message);           
        }
        return result;
    }
}