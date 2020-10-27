using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Qhta.ConsoleTesting
{
  public class ConsoleTestContext
  {
    public void Write(string message)
    {
      Console.Write(message);
      Debug.Write(message);
    }

    public void Write(string format, params object[] args)
    {
      Console.Write(format, args);
      Debug.Write(string.Format(format, args));
    }

    public void WriteLine(string message)
    {
      Console.WriteLine(message);
      Debug.WriteLine(message);
    }

    public void WriteLine(string format, params object[] args)
    {
      Console.WriteLine(format, args);
      Debug.WriteLine(string.Format(format, args));
    }

    public IDictionary Properties { get => properties; }
    Dictionary<string, object> properties = new Dictionary<string, object>();

    public void AddResultFile(string fileName)
    {
      throw new NotImplementedException();
    }

    //public override DataRow DataRow
    //{
    //  get { return testContextInstance.DataRow; }
    //}

  }
}
