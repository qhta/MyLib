using System;
using MyLib.DbUtils;
using MyLib.DbUtils.SqlServer;

namespace DbUtils.Core.Test
{
  class Program
  {
    static void Main(string[] args)
    {
      var sqlEngine = new MSSqlEngine();
      var engineClasses = DbUtilities.EnumerateEngineClasses();
      foreach (var engineClass in engineClasses)
      {
        Console.WriteLine($"EngineClass = {engineClass.Name}");
        var serverType = ServerType.Local;
        if (engineClass.Instance.CanEnumerateServerInstances(serverType))
        {
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
