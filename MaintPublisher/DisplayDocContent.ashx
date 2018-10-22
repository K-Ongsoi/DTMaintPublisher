<%@ WebHandler Language="C#" Class="DisplayDocContent" %>

using System;
using System.Web;
using System.IO;

public class DisplayDocContent : IHttpHandler {

    public bool ByteArrayToFile(string fileName, byte[] byteArray)
    {
        try
        {
            using (var fs = new System.IO.FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(byteArray, 0, byteArray.Length);
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception caught in process: {0}", ex);
            return false;
        }
    }

    public void ProcessRequest (HttpContext context) {
        String fileID = context.Request.Params["FILEID"];

        SAPConnectionInterface sapHelper = new SAPConnectionInterface();
        String contentType = "";
        String outFile = "";
        byte[] arr_content = sapHelper.getDocumentContent(fileID, out contentType, out outFile);

        context.Response.AddHeader("Content-Disposition", "attachment; filename="+ outFile);
        switch (contentType)
        {
            case "application/word": contentType = "application/msword"; break;
            case "appication/msword" : contentType = "application/msword"; break;
            case "application/excel" : contentType = "application/vnd.ms-excel"; break;
            case "application/msexcel" : contentType = "application/vnd.ms-excel"; break;
            default: break;
        }
        context.Response.ContentType = contentType;
        context.Response.OutputStream.Write(arr_content, 0, arr_content.Length);
        ByteArrayToFile("H:\\TESTPDF.PDF", arr_content);
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}