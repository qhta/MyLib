using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.DbUtils
{
  /// <summary>
  /// Silnik (typ) bazy danych
  /// </summary>
  [DataContract]
  public enum DbEngineKind : int
  {
    /// <summary>
    /// Nieokreślony
    /// </summary>
    [EnumMember]
    Unknown,
    /// <summary>
    /// Microsoft SQL Server
    /// </summary>
    [EnumMember]
    MSSQL,
    /// <summary>
    /// Microsoft Access
    /// </summary>
    [EnumMember]
    ACCESS,
    /// <summary>
    /// Microsoft Excel
    /// </summary>
    [EnumMember]
    EXCEL,
    /// <summary>
    /// DBASE
    /// </summary>
    [EnumMember]
    DBASE,
    /// <summary>
    /// SQL Compact Edition
    /// </summary>
    [EnumMember]
    SQLCE,
    /// <summary>
    /// Online Analytical Processing
    /// </summary>
    [EnumMember]
    OLAP,
    /// <summary>
    /// Active Directory Service Interface
    /// </summary>
    [EnumMember]
    ADSI,
    /// <summary>
    /// baza danych Oracle
    /// </summary>
    [EnumMember]
    ORACLE,
    /// <summary>
    /// OleDb Simple Provider
    /// </summary>
    [EnumMember]
    OSP,
    /// <summary>
    /// Indexing Services
    /// </summary>
    [EnumMember]
    IDXS,
    /// <summary>
    /// Windows Search Data Services 
    /// </summary>
    [EnumMember]
    WSDS,
    /// <summary>
    /// Baza danych oparta o pliki XML
    /// </summary>
    [EnumMember]
    XMLDB,
    /// <summary>
    /// Wbudowana baza danych SQLite
    /// </summary>
    [EnumMember]
    SQLITE,
  }
}
