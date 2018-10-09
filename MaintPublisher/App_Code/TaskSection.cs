using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for TaskSection
/// </summary>
public class TaskSection
{
    public String taskCode;
    public String taskText;

    public TaskSection()
    {
        //
        // TODO: Add constructor logic here
        //
        this.taskCode = this.taskText = "";
    }

    public TaskSection(String value, String text)
    {
        this.taskCode = value;
        this.taskText = text;
    }
}