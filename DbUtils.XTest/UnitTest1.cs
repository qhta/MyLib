using Qhta.DbUtils;
using Qhta.DbUtils.SqlServer;
using System;
using Xunit;
using Xunit.Abstractions;

namespace DbUtils.XTest
{
  public class UnitTest1
  {
    private readonly ITestOutputHelper output;

    public UnitTest1(ITestOutputHelper output)
    {
      this.output = output;
    }

    [Fact]
    public void TestEnumerateMSSQLServers()
    {
      var sqlEngine = new MSSqlEngine(); // dummy
      var serverType = ServerType.Local;
      MSSqlEngine engine = new MSSqlEngine();
      Assert.True(engine.CanEnumerateServerInstances(serverType));
      var servers = engine.EnumerateServers(serverType);
      Assert.NotEmpty(servers);
      foreach (var server in servers)
      {
        var databases = sqlEngine.EnumerateDatabases(server);
        Assert.NotEmpty(databases);
        this.output.WriteLine($"Databases in local server \"{server.Name}\":");
        foreach (var database in databases)
          this.output.WriteLine($" {database.DbName}");
      }
    }
  }
}
