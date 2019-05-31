using System;
using System.Collections.Generic;
using System.Reflection;
using Qhta.DbUtils;
using Qhta.DbUtils.SqlServer;

namespace DbUtils.NET.Test
{
  class Program
  {
    static object sqlEngine = new MSSqlEngine();
    static string[] knownDLLs = new string[]
    {
        @"D:\Dane\VS\Projects\MyLib\bin\Qhta.DbUtils.SqlServer.dll",
    };
    static void Main(string[] args)
    {
      TestEnumerateEngines();
    }

    static void TestEnumerateEngines()
    {
      var engineClasses = DbUtilities.EnumerateEngineClasses();

      foreach (var engineClass in engineClasses)
      {
        Console.WriteLine($"EngineClass = {engineClass.Type.AssemblyQualifiedName}");
        var files = engineClass.Type.Assembly.GetFiles();
        Console.WriteLine("Files: ");
        foreach (var file in files)
          Console.WriteLine($"  {file.Name}");
      }
      Console.WriteLine("Done");
      Console.ReadLine();
    }

    static void TestEnumerateServers()
    {
      var engineClasses = DbUtilities.EnumerateEngineClasses(knownDLLs);
      
      foreach (var engineClass in engineClasses)
      {
        Console.WriteLine($"EngineClass = {engineClass.Type.AssemblyQualifiedName}");
        var serverType = ServerType.Local;
        MSSqlEngine engine = new MSSqlEngine();
        if (engine.CanEnumerateServerInstances(serverType))
        {
          Console.WriteLine("Servers:");
          var servers = engine.EnumerateServers(serverType);
          foreach (var server in servers)
          {
            Console.WriteLine($"  local server = {server.ID}{Version(server)}");
          }
        }
        else
          Console.WriteLine("Can't enumerate local servers");
      }
      Console.WriteLine("Done");
      Console.ReadLine();

      string Version(DbServerInfo server)
      {
        if (server.Version != null)
          return ", version = " + server.Version;
        return null;
      }
    }

    static void TestEnumerateDatabases()
    {
      var engineClasses = DbUtilities.EnumerateEngineClasses(knownDLLs);

      foreach (var engineClass in engineClasses)
      {
        Console.WriteLine($"EngineClass = {engineClass.Type.AssemblyQualifiedName}");
        var serverType = ServerType.Local;
        MSSqlEngine engine = new MSSqlEngine();
        if (engine.CanEnumerateServerInstances(serverType))
        {
          Console.WriteLine("Servers:");
          var servers = engine.EnumerateServers(serverType);
          foreach (var server in servers)
          {
            Console.WriteLine($"  local server = {server.ID}");
            var databases = engine.EnumerateDatabases(server);
            foreach (var database in databases)
            {
              Console.WriteLine($"    {database.DbName}");
            }
          }
        }
        else
          Console.WriteLine("Can't enumerate local servers");
      }
      Console.WriteLine("Done");
      Console.ReadLine();
    }

    static void TestEnumerateTables()
    {
      var engineClasses = DbUtilities.EnumerateEngineClasses(knownDLLs);

      foreach (var engineClass in engineClasses)
      {
        Console.WriteLine($"EngineClass = {engineClass.Type.AssemblyQualifiedName}");
        var serverType = ServerType.Local;
        MSSqlEngine engine = new MSSqlEngine();
        if (engine.CanEnumerateServerInstances(serverType))
        {
          Console.WriteLine("Servers:");
          var servers = engine.EnumerateServers(serverType);
          foreach (var server in servers)
          {
            Console.WriteLine($"  local server = {server.ID}");
            var databases = engine.EnumerateDatabases(server);
            foreach (var database in databases)
            {
              Console.WriteLine($"    {database.DbName}");
              var tables = engine.EnumerateTables(database);
              foreach (var table in tables)
              {
                Console.WriteLine($"      {table.Name}");
              }
            }
          }
        }
        else
          Console.WriteLine("Can't enumerate local servers");
      }
      Console.WriteLine("Done");
      Console.ReadLine();
    }

    static void TestEnumerateColumns()
    {
      var engineClasses = DbUtilities.EnumerateEngineClasses(knownDLLs);

      foreach (var engineClass in engineClasses)
      {
        Console.WriteLine($"EngineClass = {engineClass.Type.AssemblyQualifiedName}");
        var serverType = ServerType.Local;
        MSSqlEngine engine = new MSSqlEngine();
        if (engine.CanEnumerateServerInstances(serverType))
        {
          Console.WriteLine("Servers:");
          var servers = engine.EnumerateServers(serverType);
          foreach (var server in servers)
          {
            Console.WriteLine($"  local server = {server.ID}");
            var databases = engine.EnumerateDatabases(server);
            foreach (var database in databases)
            {
              Console.WriteLine($"    {database.DbName}");
              var tables = engine.EnumerateTables(database);
              foreach (var table in tables)
              {
                Console.WriteLine($"      {table.Name}");
                var columns = engine.EnumerateColumns(table);
                foreach (var column in columns)
                {
                  Console.WriteLine($"        {column.Name}: {TypeSpec(column)}");
                }
              }
            }
          }
        }
        else
          Console.WriteLine("Can't enumerate local servers");
      }
      Console.WriteLine("Done");
      Console.ReadLine();

      string TypeSpec(DbColumnInfo column)
      {
        var result = column.Type;
        int ctrl = 0;
        if (column.Size != null && column.Size!=0)
        {
          result += $"({column.Size}";
          ctrl++;
        }
        if (column.Precision != null && column.Precision != 0)
        {
          if (ctrl == 0)
            result += "(";
          result += $",{column.Precision}";
          ctrl++;
        }
        if (ctrl > 0)
          result += ")";
        return result;
      }
    }

  }
}