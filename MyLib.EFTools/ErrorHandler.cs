using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Management.Common;

namespace EFTools
{
  public class ErrorHandler
  {
    public static void HandleException(Exception ex)
    {
      if (ex is DbEntityValidationException)
      {
        DbEntityValidationException validationException = (DbEntityValidationException)ex;
        {
          string[] msg = ex.Message.Split('.');
          Debug.WriteLine(msg[0]);
          foreach (DbEntityValidationResult result in validationException.EntityValidationErrors)
          {
            foreach (DbValidationError error in result.ValidationErrors)
            {
              Debug.WriteLine(string.Format("{0}: {1}", error.PropertyName, error.ErrorMessage));
            }
          }
        }
      }
      else
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
        if (ex is SqlException)
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
          Debug.WriteLine(String.Format("{0}: {1}", ex.GetType().Name, ex.Message));
          //throw ex;
        }
      }
    }

  }
}
