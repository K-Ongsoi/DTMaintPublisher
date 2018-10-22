<%@ WebHandler Language="C#" Class="DocUploadHandler" %>

using System;
using System.Web;
using Newtonsoft.Json;

public class UploadStats
{
    public String uploadStatus;
    public String uploadLog;
    public String fullFileName;
}

public class DocUploadHandler : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        String fullFilePath = "UploadFiles/";
        UploadStats stat = new UploadStats();

        HttpPostedFile fileData = context.Request.Files[0];

        try
        {
            String folderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            System.IO.Directory.CreateDirectory(context.Server.MapPath("UploadFiles/" + folderName));
            fullFilePath = "UploadFiles/" + folderName + "/" + fileData.FileName;
            fileData.SaveAs(context.Server.MapPath(fullFilePath));
            stat.fullFileName = context.Server.MapPath(fullFilePath);
            stat.uploadStatus = "Ok";
            stat.uploadLog = fileData.FileName + " was uploaded successfully.\n";
        }
        catch (Exception e)
        {
            stat.uploadStatus = "Error";
            stat.uploadLog = e.Message;
        }
        finally
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(stat));
        }
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}