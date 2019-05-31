using System.Collections.Generic;

namespace Qhta.DbUtils
{
  public interface IDbEngine
  {
    /// <summary>
    /// Nazwa silnika - taka jak w klasie silnika
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Czy silnik potrafi wyliczyć instancje serwera?
    /// </summary>
    bool CanEnumerateServerInstances(ServerType serverType);

    /// <summary>
    /// Wyliczenie instancji serwera
    /// </summary>
    IEnumerable<DbServerInfo> EnumerateServers(ServerType serverType);

    // /// <summary>
    // /// Utworzenie połączenia do instancji serwera
    // /// </summary>
    // /// <param name="info">informacje potrzebne do utworzenia połączenia</param>
    // DbConnection CreateConnection(DbServerInfo info);

    // /// <summary>
    // /// Połączenie do instancji serwera. 
    // /// Jeśli połączenie jeszcze nie zostało utworzone, to jest tworzone w tym miejscu.
    // /// Jeśli połączenie jeszcze nie zostało otwarte, to jest otwierane.
    // /// </summary>
    // /// <param name="info">informacje potrzebne do utworzenia połączenia</param>
    // void ConnectTo(DbServerInfo info);


    // /// <summary>
    // /// Podanie, ew. utworzenie połączenia do bazy danych
    // /// </summary>
    // /// <param name="info">informacje potrzebne do utworzenia połączenia</param>
    // /// <returns>Połączenie do bazy danych</returns>
    // DbConnection GetConnection(DbServerInfo info);

    // /// <summary>
    // /// Podanie, ew. utworzenie połączenia do bazy danych
    // /// </summary>
    // /// <param name="info">informacje potrzebne do utworzenia połączenia</param>
    // /// <returns>Połączenie do bazy danych</returns>
    // DbConnection GetConnection(DbInfo info);

    // /// <summary>
    // /// Wyliczenie baz danych na serwerze
    // /// </summary>
    // /// <param name="info">informacje o serwerze</param>
    //IEnumerable<DbInfo> EnumerateDatabases(DbServerInfo info);

    // /// <summary>
    // /// Wyliczenie potencjalnych baz danych, których pliki są dostępne na serwerze
    // /// </summary>
    // /// <param name="info">informacje o serwerze</param>
    // IEnumerable<DbInfo> EnumeratePotentialDatabases(DbServerInfo info);

    // /// <summary>
    // /// Tworzenie połączenia do bazy danych
    // /// </summary>
    // /// <param name="info">informacje potrzebne do utworzenia połączenia</param>
    // /// <returns></returns>
    // DbConnection CreateConnection(DbInfo info);

    // /// <summary>
    // /// Tworzenie komendy SQL
    // /// </summary>
    // /// <param name="cmdText">text komendy (SQL)</param>
    // /// <param name="connection">połączenie do serwera</param>
    // /// <returns></returns>
    // DbCommand CreateCommand(string cmdText, DbConnection connection);

    // /// <summary>
    // /// Tworzenie bazy danych
    // /// </summary>
    // /// <param name="info">informacje potrzebne do utworzenia bazy danych</param>
    // void CreateDatabase(DbInfo info);

    // /// <summary>
    // /// Dołączenie istniejącej bazy danych
    // /// </summary>
    // /// <param name="info">informacje identyfikujące bazę danych</param>
    // void AttachDatabase(DbInfo info);

    // /// <summary>
    // /// Sprawdzenie istnienia bazy danych
    // /// </summary>
    // /// <param name="info">informacje definiujące bazę danych</param>
    // bool DatabaseExists(DbInfo info);

    // /// <summary>
    // /// Sprawdzenie istnienia plików bazy danych
    // /// </summary>
    // /// <param name="info">informacje definiujące bazę danych</param>
    // bool ExistsDatabaseFiles(DbInfo info);

    // /// <summary>
    // /// Usuwanie bazy danych
    // /// </summary>
    // /// <param name="info">informacje potrzebne do usunięcia bazy danych</param>
    // void DeleteDatabase(DbInfo info);

    // /// <summary>
    // /// Odłączenie bazy danych od serwera
    // /// </summary>
    // /// <param name="info">informacje identyfikujące bazę danych</param>
    // void DetachDatabase(DbInfo info);

    // /// <summary>
    // /// Usuwanie plików bazy danych
    // /// </summary>
    // /// <param name="info">informacje potrzebne do usunięcia bazy danych</param>
    // void DeleteDatabaseFiles(DbInfo info);


    // /// <summary>
    // /// Domyślne rozszerzenie nazwy pliku głównego
    // /// </summary>
    // string DefaultFileExt { get; }

    // /// <summary>
    // /// Nazwy wszystkich plików fizycznych skojarzonych z bazą danych.
    // /// Niekoniecznie wszystkie te pliki istnieją.
    // /// </summary>
    // /// <param name="info">kontekst identyfikujący bazę danych</param>
    // /// <returns></returns>
    // string[] PhysicalFilenames(DbInfo info);

    // /// <summary>
    // /// Zmiana nazwy bazy danych
    // /// </summary>
    // /// <param name="newDbName">nowa nazwa bazy danych</param>
    // /// <param name="newFileNames">nowe nazwy plików danych</param>
    // /// <param name="info">informacje potrzebne do wykonania operacji</param>
    // void RenameDatabase(DbInfo info, string newDbName, params string[] newFileNames);
    // /// <summary>
    // /// Zmiana nazwy plików bazy danych
    // /// <param name="oldFileNames">stare nazwy plików</param>
    // /// </summary>
    // /// <param name="newFileNames">nowe nazwy plików</param>
    // void RenameDatabaseFiles(string[] oldFileNames, string[] newFileNames);

    // /// <summary>
    // /// Zmiana nazwa pliku bez zmiany rozszerzenia. 
    // /// Jeśli nowa nazwa pliku nie zawiera ścieżki katalogów, 
    // /// to kopiowana jest ścieżka ze starej nazwy.
    // /// </summary>
    // /// <param name="oldFileName">stara nazwa pliku</param>
    // /// <param name="newFileName">nowa nazwa pliku (bez rozszerzenia)</param>
    // /// <returns></returns>
    // string ChangeFileName(string oldFileName, string newFileName);

    // /// <summary>
    // /// Kopiowanie bazy danych
    // /// </summary>
    // /// <param name="dbInfo">informacje o istniejącej bazie danych</param>
    // /// <param name="newDbInfo">informacje o nowej bazie danych</param>
    // void CopyDatabase(DbInfo dbInfo, DbInfo newDbInfo);

    // /// <summary>
    // /// Kopiowanie plików bazy danych
    // /// </summary>
    // /// <param name="newFileNames">nowe nazwy plików (bez rozszerzenia)</param>
    // /// <param name="info">informacje potrzebne do wykonania operacji</param>
    // void CopyDatabaseFiles(DbInfo info, string[] newFileNames);

    // /// <summary>
    // /// Wyliczenie tabel w bazie danych
    // /// </summary>
    // /// <param name="info">informacje o bazie danych</param>
    // IEnumerable<DbTableInfo> EnumerateTables(DbInfo info);

    // /// <summary>
    // /// Wyliczenie kolumn w tabeli
    // /// </summary>
    // /// <param name="info">informacje o tabeli danych</param>
    // IEnumerable<DbColumnInfo> EnumerateColumns(DbTableInfo info);

    // /// <summary>
    // /// Pobranie wszystkich danych z tabeli
    // /// </summary>
    // /// <param name="info">informacje o tabeli danych</param>
    // DataTable GetDataTable(DbTableInfo info);

  }
}
