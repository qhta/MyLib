using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlServerCe;

namespace MyLib.DbUtils.SqlCE
{

  public static class SqlCeUtils
  {
    public static void EnsureVersion40(this SqlCeEngine engine, string source, string destination)
    {
      SQLCEVersion fileversion = DetermineVersion(source);
      if (fileversion == SQLCEVersion.SQLCE20)
        throw new ApplicationException("Unable to upgrade from 2.0 to 4.0");

      if (SQLCEVersion.SQLCE40 > fileversion)
      {
        engine.Upgrade(String.Format("Data Source=\"{0}\"", destination));
      }
    }
    private enum SQLCEVersion
    {
      SQLCE20 = 0,
      SQLCE30 = 1,
      SQLCE35 = 2,
      SQLCE40 = 3
    }
    private static SQLCEVersion DetermineVersion(string filename)
    {
      var versionDictionary = new Dictionary<int, SQLCEVersion>
      {
        { 0x73616261, SQLCEVersion.SQLCE20},
        { 0x002dd714, SQLCEVersion.SQLCE30},
        { 0x00357b9d, SQLCEVersion.SQLCE35},
        { 0x003d0900, SQLCEVersion.SQLCE40}
      };
      int versionLONGWORD = 0;
      try
      {
        using (var fs = new FileStream(filename, FileMode.Open))
        {
          fs.Seek(16, SeekOrigin.Begin);
          using (BinaryReader reader = new BinaryReader(fs))
          {
            versionLONGWORD = reader.ReadInt32();
          }
        }
      }
      catch
      {
        throw;
      }
      if (versionDictionary.ContainsKey(versionLONGWORD))
      {
        return versionDictionary[versionLONGWORD];
      }
      else
      {
        throw new ApplicationException("Unable to determine database file version");
      }
    }
  }
}