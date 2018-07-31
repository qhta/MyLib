using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.DbUtils
{
  /// <summary>
  /// Abstrakcyjna klasa wykonująca podstawowe operacje na bazach danych
  /// </summary>
  public abstract class DbEngine
  {
    /// <summary>
    /// Nazwa silnika - taka jak w klasie silnika
    /// </summary>
    public virtual string Name => this.GetType().Name;

    /// <summary>
    /// Czy silnik potrafi wyliczyć instancje serwera?
    /// </summary>
    public abstract bool CanEnumerateServerInstances { get; }

    /// <summary>
    /// Wyliczenie instancji serwera
    /// </summary>
    public virtual IEnumerable<DbServerInfo> EnumerateServers()
    {
      throw new InvalidOperationException($"{GetType().Name} cannot enumerate server instances");
    }

    /// <summary>
    /// Utworzenie połączenia do instancji serwera
    /// </summary>
    /// <param name="info">informacje potrzebne do utworzenia połączenia</param>
    public virtual DbConnection CreateConnection(DbServerInfo info)
    {
      throw new InvalidOperationException($"{GetType().Name} cannot connect to server instance");
    }

    /// <summary>
    /// Połączenie do instancji serwera. 
    /// Jeśli połączenie jeszcze nie zostało utworzone, to jest tworzone w tym miejscu.
    /// Jeśli połączenie jeszcze nie zostało otwarte, to jest otwierane.
    /// </summary>
    /// <param name="info">informacje potrzebne do utworzenia połączenia</param>
    public virtual void ConnectTo(DbServerInfo info)
    {
      var connection = info.Connection;
      if (connection==null)
        connection = CreateConnection(info);
      if (connection.State!=ConnectionState.Open)
        connection.Open();
      info.Connection=connection;
    }

    /// <summary>
    /// Podanie, ew. utworzenie połączenia do bazy danych
    /// </summary>
    /// <param name="info">informacje potrzebne do utworzenia połączenia</param>
    /// <returns>Połączenie do bazy danych</returns>
    public virtual DbConnection GetConnection(DbServerInfo info)
    {
      if (info.Connection==null)
      {
        info.Connection = CreateConnection(info);
      }
      return info.Connection;
    }

    /// <summary>
    /// Podanie, ew. utworzenie połączenia do bazy danych
    /// </summary>
    /// <param name="info">informacje potrzebne do utworzenia połączenia</param>
    /// <returns>Połączenie do bazy danych</returns>
    public virtual DbConnection GetConnection(DbInfo info)
    {
      if (info.Connection==null)
      {
        info.Connection = CreateConnection(info);
      }
      return info.Connection;
    }

    /// <summary>
    /// Wyliczenie instancji serwera
    /// </summary>
    public virtual IEnumerable<DbInfo> EnumerateDatabases(DbServerInfo info)
    {
      throw new InvalidOperationException($"{GetType().Name} cannot enumerate databases");
    }

    /// <summary>
    /// Tworzenie połączenia do bazy danych
    /// </summary>
    /// <param name="info">informacje potrzebne do utworzenia połączenia</param>
    /// <returns></returns>
    public abstract DbConnection CreateConnection(DbInfo info);

    /// <summary>
    /// Tworzenie bazy danych
    /// </summary>
    /// <param name="info">informacje potrzebne do utworzenia bazy danych</param>
    public abstract void CreateDatabase(DbInfo info);

    /// <summary>
    /// Dołączenie istniejącej bazy danych
    /// </summary>
    /// <param name="info">informacje identyfikujące bazę danych</param>
    public abstract void AttachDatabase(DbInfo info);

    /// <summary>
    /// Sprawdzenie istnienia bazy danych
    /// </summary>
    /// <param name="info">informacje definiujące bazę danych</param>
    public abstract bool DatabaseExists(DbInfo info);

    /// <summary>
    /// Sprawdzenie istnienia plików bazy danych
    /// </summary>
    /// <param name="info">informacje definiujące bazę danych</param>
    public virtual bool ExistsDatabaseFiles(DbInfo info)
    {
      for (int i = 0; i<info.FileNames.Length; i++)
        if (!System.IO.File.Exists(info.FileNames[i]))
          return false;
      return true;
    }

    /// <summary>
    /// Usuwanie bazy danych
    /// </summary>
    /// <param name="info">informacje potrzebne do usunięcia bazy danych</param>
    public abstract void DeleteDatabase(DbInfo info);

    /// <summary>
    /// Odłączenie bazy danych od serwera
    /// </summary>
    /// <param name="info">informacje identyfikujące bazę danych</param>
    public abstract void DetachDatabase(DbInfo info);

    /// <summary>
    /// Usuwanie plików bazy danych
    /// </summary>
    /// <param name="info">informacje potrzebne do usunięcia bazy danych</param>
    public virtual void DeleteDatabaseFiles(DbInfo info)
    {
      IEnumerable<string> files = PhysicalFilenames(info);
      if (files != null)
      {
        foreach (string file in files)
          SafeDeleteFile(file);
      }
    }

    /// <summary>
    /// Bezpieczne kasowanie pliku
    /// </summary>
    /// <param name="filename">nazwa pliku</param>
    /// <returns></returns>
    protected virtual void SafeDeleteFile(string filename)
    {
      if (filename != null && System.IO.File.Exists(filename))
        System.IO.File.Delete(filename);
    }

    /// <summary>
    /// Domyślne rozszerzenie nazwy pliku głównego
    /// </summary>
    public virtual string DefaultFileExt => null;

    /// <summary>
    /// Nazwy wszystkich plików fizycznych skojarzonych z bazą danych.
    /// Niekoniecznie wszystkie te pliki istnieją.
    /// </summary>
    /// <param name="info">kontekst identyfikujący bazę danych</param>
    /// <returns></returns>
    public virtual string[] PhysicalFilenames(DbInfo info)
    {
      List<string> result = new List<string>();
      if (info.FileNames!=null)
        result.AddRange(info.FileNames);
      else
      {
        result.Add(info.DbName+DefaultFileExt);
      }
      return result.ToArray();
    }

    /// <summary>
    /// Zmiana nazwy bazy danych
    /// </summary>
    /// <param name="newDbName">nowa nazwa bazy danych</param>
    /// <param name="newFileNames">nowe nazwy plików danych</param>
    /// <param name="info">informacje potrzebne do wykonania operacji</param>
    public abstract void RenameDatabase(DbInfo info, string newDbName, params string[] newFileNames);

    /// <summary>
    /// Zmiana nazwy plików bazy danych
    /// <param name="oldFileNames">stare nazwy plików</param>
    /// </summary>
    /// <param name="newFileNames">nowe nazwy plików</param>
    public virtual void RenameDatabaseFiles(string[] oldFileNames, string[] newFileNames)
    {
      for (int i = 0; i<oldFileNames.Length && i<newFileNames.Length; i++)
        if (oldFileNames[i] != newFileNames[i])
          SafeRenameFile(oldFileNames[i], ChangeFileName(oldFileNames[i], newFileNames[i]));
    }

    /// <summary>
    /// Zmiana nazwa pliku bez zmiany rozszerzenia. 
    /// Jeśli nowa nazwa pliku nie zawiera ścieżki katalogów, 
    /// to kopiowana jest ścieżka ze starej nazwy.
    /// </summary>
    /// <param name="oldFileName">stara nazwa pliku</param>
    /// <param name="newFileName">nowa nazwa pliku (bez rozszerzenia)</param>
    /// <returns></returns>
    public virtual string ChangeFileName(string oldFileName, string newFileName)
    {
      string oldExt = System.IO.Path.GetExtension(oldFileName);
      string newExt = System.IO.Path.GetExtension(newFileName);
      if (String.IsNullOrEmpty(System.IO.Path.GetDirectoryName(newFileName)))
      {
        string oldPath = System.IO.Path.GetDirectoryName(oldFileName);
        newFileName = System.IO.Path.Combine(oldPath, newFileName);
      }
      if (String.IsNullOrEmpty(newExt))
        newFileName = System.IO.Path.ChangeExtension(newFileName, oldExt);
      return newFileName; 
    }

    /// <summary>
    /// Bezpieczna zmiana nazwy pliku
    /// </summary>
    /// <param name="oldFileName">stara nazwa pliku</param>
    /// <param name="newFileName">nowa nazwa pliku</param>
    protected virtual void SafeRenameFile(string oldFileName, string newFileName)
    {
      if (newFileName != null && System.IO.File.Exists(newFileName))
      {
        System.IO.File.Delete(newFileName);
      }

      if (oldFileName != null && System.IO.File.Exists(oldFileName))
      {
        string newPath = System.IO.Path.GetDirectoryName(newFileName);
        if (!System.IO.Directory.Exists(newPath))
          System.IO.Directory.CreateDirectory(newPath);
        System.IO.File.Move(oldFileName, newFileName);
      }
    }

    /// <summary>
    /// Kopiowanie bazy danych
    /// </summary>
    /// <param name="newDbName">nowa nazwa bazy danych</param>
    /// <param name="newFileNames">nowe nazwy plików</param>
    /// <param name="info">informacje potrzebne do wykonania operacji</param>
    public abstract void CopyDatabase(DbInfo info, string newDbName, string[] newFileNames);

    /// <summary>
    /// Kopiowanie plików bazy danych
    /// </summary>
    /// <param name="newFileNames">nowe nazwy plików (bez rozszerzenia)</param>
    /// <param name="info">informacje potrzebne do wykonania operacji</param>
    public virtual void CopyDatabaseFiles(DbInfo info, string[] newFileNames)
    {
      IEnumerable<string> files = PhysicalFilenames(info);
      if (files != null)
      {
        int i = 0;
        foreach (string file in files)
          SafeCopyFile(file, ChangeFileName(file, newFileNames[i++]));
      }
    }

    /// <summary>
    /// Bezpieczne kopiowanie pliku
    /// </summary>
    /// <param name="fromFileName">nazwa pliku oryginalnego</param>
    /// <param name="toFileName">nazwa kopii pliku</param>
    /// <returns></returns>
    protected virtual void SafeCopyFile(string fromFileName, string toFileName)
    {
      if (fromFileName != null && System.IO.File.Exists(fromFileName))
      {
        string newPath = System.IO.Path.GetDirectoryName(toFileName);
        if (!System.IO.Directory.Exists(newPath))
          System.IO.Directory.CreateDirectory(newPath);
        System.IO.File.Copy(fromFileName, toFileName);
      }
    }
  }
}
