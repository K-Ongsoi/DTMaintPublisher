using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DocUser
/// </summary>
public class DocUser
{
    public bool status;
    public String userID;
    public String userName;
    public String functionCode;
    public String email;

    public DocUser()
    {
        this.status = false;
        this.userID = this.userName = this.functionCode = this.email = "";
    }

    public DocUser(String userID,String userName)
    {
        this.status = true;
        this.userID = userID;
        this.userName = userName;
    }

    public DocUser(String userID, String userName, String functionCode, String email)
    {
        this.status = true;
        this.userID = userID;
        this.userName = userName;
        this.functionCode = functionCode;
        this.email = email;
    }
}