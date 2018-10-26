using System;
using System.Collections.Generic;
using System.Reflection;
using Qhta.DbUtils;
using Qhta.DbUtils.SqlServer;

namespace DbUtils.Core.Test
{
  class Program
  {
    static void Main(string[] args)
    {
      var sqlEngine = new MSSqlEngine();
      var knownDLLs = new string[]
      {
        @"D:\Dane\VS\Projects\MyLib\bin\Qhta.DbUtils.SqlServer.dll",
      };
      var engineClasses = DbUtilities.EnumerateEngineClasses(knownDLLs);
      
      foreach (var engineClass in engineClasses)
      {
        Console.WriteLine($"EngineClass = {engineClass.Type.AssemblyQualifiedName}");
        var files = engineClass.Type.Assembly.GetFiles();
        Console.WriteLine("Files: ");
        foreach (var file in files)
          Console.WriteLine($"  {file.Name}");
        var serverType = ServerType.Local;
        if (engineClass.Instance.CanEnumerateServerInstances(serverType))
        {
          Console.WriteLine("Servers:");
          var servers = engineClass.Instance.EnumerateServers(serverType);
          foreach (var server in servers)
          {
            Console.WriteLine($"  local server = {server.Name}");
          }
        }
        else
          Console.WriteLine("Can't enumerate local servers");
      }
      Console.WriteLine("Done");
      Console.ReadLine();
    }
  }
}