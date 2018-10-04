using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Management.Common;

namespace MyLib.EFTools
{
  public class ErrorHandler
  {
    public static void HandleException(Exception ex)
    {
      //int indent = 0;
      InnerHandleException(ex);//, indent++);
      ex = ex.InnerException;
      while (ex != null)
      {
        InnerHandleException(ex);//, indent++);
        ex = ex.InnerException;
      }
    }

    private static void InnerHandleException(Exception ex)
    {
      bool @throw = true;
      if (ex.InnerException!=null)
      {
        InnerHandleException(ex.InnerException);
        return;
      }
      if (ex.InnerException == null ||
        !(ex is ExecutionFailureException) &&
        !(ex is UpdateException) &&
        !(ex is DbUpdateException))
      {
        if (ex is EntitySqlException)
        {
          string[] ss = ex.Message.Split(new char[] { '.', ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
          if (ss[0] == "Violation")
          {
            if (ss[2] == "PRIMARY" && ss[3] == "KEY")
            {
              string primaryKeyName = ss[5];
              string primaryKeyValue = ss[19];
              Debug.WriteLine(String.Format("Primary key violation. PrimaryKey name = {0}, value ={1}", primaryKeyName, primaryKeyValue));
              @throw = false;
            }
          }
        }
        if (@throw)
        {
          string exTypeName = ex.GetType().Name;
          if (exTypeName.Contains('`'))
            exTypeName = ex.GetType().Name;
          Debug.WriteLine(String.Format("{0}: {1}", exTypeName, ex.Message));
          //throw ex;
        }
      }
    }

  }
}
