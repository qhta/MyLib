using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Microsoft.EntityFrameworkCore.SqlServer.Migrations;
using Microsoft.EntityFrameworkCore.Storage;

namespace MyLib.EFTools
{
  public static class MigrationBuilderExtensions
  {
    public static CreateTableBuilder<TColumns> CreateTableIfNotExists<TColumns>(this MigrationBuilder migrationBuilder,
      string name, Func<ColumnsBuilder, TColumns> columns, string schema = null,
      Action<CreateTableBuilder<TColumns>> constraints = null)
    {
      var op = migrationBuilder.CreateTable(name, columns, schema, constraints);
      return op;
      //      Microsoft.EntityFrameworkCore.Migrations.MigrationCommand
      //"IF(NOT EXISTS(" +
      //"SELECT *" +
      //" FROM INFORMATION_SCHEMA.TABLES" +
      //" WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Roles'))" +
      //      return false;
    }
    public static void MyMigrate(this DatabaseFacade databaseFacade)
    {
      var migrator = databaseFacade.GetRelationalService<IMigrator>();
      migrator.Migrate(null);
    }


    private static TService GetRelationalService<TService>(this IInfrastructure<IServiceProvider> databaseFacade)
    {
      Check_NotNull<IInfrastructure<IServiceProvider>>(databaseFacade, "databaseFacade");
      TService service = databaseFacade.Instance.GetService<TService>();
      if (service == null)
      {
        throw new InvalidOperationException("RelationalStrings.RelationalNotInUse");
      }
      return service;
    }

    public static T GetService<T>(this IServiceProvider provider)
    {
      if (provider == null)
      {
        throw new ArgumentNullException("provider");
      }
      if (typeof(T)==typeof(IMigrator))
      {
        var migrator = (T)provider.GetService((Type)typeof(T));
        Debug.WriteLine(migrator);
        return migrator;
      }
      var result = (T)provider.GetService((Type)typeof(T));
      return result;
    }




    //[ContractAnnotation("value:null => halt")]
    public static T Check_NotNull<T>(T value, string parameterName)
    {
      if (value == null)
      {
        throw new ArgumentNullException(parameterName);
      }
      return value;
    }

  }

  public class MigratorExtension: IMigrator
  {
    public MigratorExtension(IMigrator migrator)
    {
      Migrator = migrator;
    }

    private IMigrator Migrator;

    public void Migrate(string targetMigration = null)
    {
      Migrator.Migrate(targetMigration);
    }

    public Task MigrateAsync(string targetMigration = null, CancellationToken cancellationToken = default(CancellationToken))
    {
      return Migrator.MigrateAsync(targetMigration, cancellationToken);
    }

    public string GenerateScript(string fromMigration = null, string toMigration = null, bool idempotent = false)
    {
      return Migrator.GenerateScript(fromMigration, toMigration, idempotent);
    }


  }

  public class MyMigrator : Migrator
  {
    public MyMigrator(IMigrationsAssembly migrationsAssembly, 
      IHistoryRepository historyRepository, 
      IDatabaseCreator databaseCreator,
      IMigrationsSqlGenerator migrationsSqlGenerator, 
      IRawSqlCommandBuilder rawSqlCommandBuilder, 
      IMigrationCommandExecutor migrationCommandExecutor, 
      IRelationalConnection connection, 
      ISqlGenerationHelper sqlGenerationHelper, 
      IDiagnosticsLogger<DbLoggerCategory.Migrations> logger, 
      IDatabaseProvider databaseProvider) : 
        base(migrationsAssembly, historyRepository, databaseCreator, migrationsSqlGenerator, 
        rawSqlCommandBuilder, migrationCommandExecutor, connection, sqlGenerationHelper, logger, databaseProvider)
    {
    }

    protected override IReadOnlyList<MigrationCommand> GenerateUpSql(Migration migration)
    {
      return base.GenerateUpSql(migration);
    }
  }
}
