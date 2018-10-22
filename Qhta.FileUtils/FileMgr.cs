using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using IO = System.IO;


namespace Qhta.FileUtils
{
  /// <summary>
  /// Klasa zarządzająca plikami i katalogami
  /// </summary>
  public static class FileMgr
  {
//#region potrzebne elementy WinAPI
//    /// <summary>
//    /// Struktura potrzebna do pobierania dodatkowej informacji o pliku
//    /// </summary>
//    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
//    private struct SHFILEINFO
//    {
//      public IntPtr hIcon;
//      public int iIcon;
//      public Int32 dwAttributes;
//      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
//      public string szDisplayName;
//      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
//      public string szTypeName;
//    };

//    /// <summary>
//    /// Procedura WinAPI do pobierania dodatkowej informacji o pliku
//    /// </summary>
//    /// <param name="pszPath"></param>
//    /// <param name="dwFileAttributes"></param>
//    /// <param name="psfi"></param>
//    /// <param name="cbFileInfo"></param>
//    /// <param name="uFlags"></param>
//    /// <returns></returns>
//    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
//    private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

//    /// <summary>
//    /// Flagi do pobierania dodatkowej informacji o pliku
//    /// </summary>
//    [Flags]
//    private enum SHGFI : int
//    {
//      /// <summary>get icon</summary>
//      Icon = 0x000000100,
//      /// <summary>get display name</summary>
//      DisplayName = 0x000000200,
//      /// <summary>get type name</summary>
//      TypeName = 0x000000400,
//      /// <summary>get attributes</summary>
//      Attributes = 0x000000800,
//      /// <summary>get icon location</summary>
//      IconLocation = 0x000001000,
//      /// <summary>return exe type</summary>
//      ExeType = 0x000002000,
//      /// <summary>get system icon index</summary>
//      SysIconIndex = 0x000004000,
//      /// <summary>put a link overlay on icon</summary>
//      LinkOverlay = 0x000008000,
//      /// <summary>show icon in selected state</summary>
//      Selected = 0x000010000,
//      /// <summary>get only specified attributes</summary>
//      Attr_Specified = 0x000020000,
//      /// <summary>get large icon</summary>
//      LargeIcon = 0x000000000,
//      /// <summary>get small icon</summary>
//      SmallIcon = 0x000000001,
//      /// <summary>get open icon</summary>
//      OpenIcon = 0x000000002,
//      /// <summary>get shell size icon</summary>
//      ShellIconSize = 0x000000004,
//      /// <summary>pszPath is a pidl</summary>
//      PIDL = 0x000000008,
//      /// <summary>use passed dwFileAttribute</summary>
//      UseFileAttributes = 0x000000010,
//      /// <summary>apply the appropriate overlays</summary>
//      AddOverlays = 0x000000020,
//      /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
//      OverlayIndex = 0x000000040,
//    }

//    [DllImport("gdi32.dll", SetLastError = true)]
//    private static extern bool DeleteObject(IntPtr hObject);

 
 
//    [DllImport("kernel32.dll", SetLastError = true)]
//    private static extern IntPtr LoadLibrary(string fileName);
//    [DllImport("kernel32.dll", SetLastError = true)]
//    private static extern bool FreeLibrary(IntPtr hModule);

//    [DllImport("user32.dll", SetLastError = true)]
//    private static extern IntPtr LoadIcon(IntPtr hModule, Int32 id);
//    [DllImport("user32.dll", SetLastError = true)]
//    private static extern bool DestroyIcon(IntPtr hIcon);
//    [DllImport("kernel32.dll", SetLastError = true)]
//    private static extern IntPtr FindResource(IntPtr hModule, Int32 id, Int32 type);
//#endregion

    /// <summary>
    /// Wyliczenie urządzeń dostępnych na maszynie
    /// </summary>
    public static IEnumerable<DriveInfo> EnumerateDrives()
    {
      ObservableCollection<DriveInfo> result = new ObservableCollection<DriveInfo>();
      GetDrives(result);
      return result as IEnumerable<DriveInfo>;
    }

    /// <summary>
    /// Asynchroniczne wyliczenie urządzeń dostępnych na maszynie
    /// </summary>
    public static ObservableCollection<DriveInfo> EnumerateDrivesAsync(NotifyCollectionChangedEventHandler changeHandler)
    {
      ObservableCollection<DriveInfo> result = new ObservableCollection<DriveInfo>();
      result.CollectionChanged += changeHandler;
      Thread aThread = new Thread (GetDrives);
      aThread.Name = "EnumerateDrivesAsync";
      aThread.Start(result);
      return result;
    }

    /// <summary>
    /// Wyliczenie urządzeń dostępnych na maszynie. Metoda wywoływana w osobnym wątku
    /// </summary>
    /// <param name="data">obserwowalna kolekcja elementów</param>
    public static void GetDrives(object data)
    {
      ObservableCollection<DriveInfo> result = data as ObservableCollection<DriveInfo>;

      IO.DriveInfo[] infos = IO.DriveInfo.GetDrives();
      foreach (IO.DriveInfo drive in infos)
      {
        DriveInfo _file = new DriveInfo();
        _file.Name = drive.Name;
        _file.Path = drive.Name;
        //_file.Description = GetDisplayName(drive.Name);
        result.Add(_file);
      }
    }

    /// <summary>
    /// Pobranie informacji o pliku dla podanej ścieżki
    /// </summary>
    /// <param name="path">ścieżka wejściowa</param>
    /// <param name="rootDirectory">główny katalog</param>
    /// <returns></returns>
    public static FileInfo GetInfo(string path, string rootDirectory=null)
    {
      FileInfo result;
      if (String.IsNullOrEmpty(path))
      {
        if (String.IsNullOrEmpty(rootDirectory))
          return new MachineInfo { Name = Environment.MachineName };
      }
      string absPath = IO.Path.Combine(rootDirectory ?? "", path ?? "");
      IO.FileAttributes attributes = IO.File.GetAttributes(absPath);
      if (attributes.HasFlag(IO.FileAttributes.Directory))
        result = new DirInfo();
      else
        result = new FileInfo();
      result.Name = IO.Path.GetFileName(absPath);
      result.Path = RelativePath(absPath, rootDirectory);
      if (result.Path == rootDirectory)
        result.Path = "";
      return result;
    }

    /// <summary>
    /// pomocnicza procedura do przekazywania parametrów do metody <see cref="GetDirectories"/>
    /// </summary>
    struct GetDirectoriesParam
    {
      public ObservableCollection<DirInfo> Collection;
      public string Path;
      public string RootDirectory;
    }

    /// <summary>
    /// Wyliczenie katalogów z podanej ścieżki
    /// </summary>
    public static IEnumerable<DirInfo> EnumerateDirectories(string path, string rootDirectory="")
    {
      ObservableCollection<DirInfo> result = new ObservableCollection<DirInfo>();
      GetDirectories(new GetDirectoriesParam{ Collection = result, Path = path, RootDirectory = rootDirectory });
      return result as IEnumerable<DirInfo>;
    }

    /// <summary>
    /// Asynchroniczne wyliczenie katalogów z podanej ścieżki
    /// </summary>
    public static ObservableCollection<DirInfo> EnumerateDirectoriesAsync(string path, string rootDirectory,
      NotifyCollectionChangedEventHandler changeHandler, ObservableCollection<DirInfo> result)
    {
      if (result==null)
        result = new ObservableCollection<DirInfo>();
      result.CollectionChanged += changeHandler;
      Thread aThread = new Thread(GetDirectories);
      aThread.Name=String.Format("EnumerateDirectoriesAsync({0})",path);
      Debug.WriteLine(String.Format("Starting thread {0}", aThread.Name));
      aThread.Start(new GetDirectoriesParam{ Collection=result, Path = path, RootDirectory = rootDirectory });
      return result;
    }

    /// <summary>
    /// Wyliczenie katalogów z określonej ścieżki. Metoda wywoływana w osobnym wątku
    /// </summary>
    /// <param name="data">zbiorcza struktura umożliwiająca przekazywanie parametrów w postaci jednego obiektu typu <see cref="GetDirectoriesParam"/></param>
    public static void GetDirectories(object data)
    {
      GetDirectoriesParam param = (GetDirectoriesParam)data;
      ObservableCollection<DirInfo> result = param.Collection;
      string path = param.Path;
      string rootDirectory = param.RootDirectory;
      IO.DirectoryInfo info = new IO.DirectoryInfo(IO.Path.Combine(rootDirectory ?? "", path ?? ""));
      if (!info.Exists)
        return;
      IO.DirectoryInfo[] infos = info.GetDirectories();
      foreach (IO.DirectoryInfo file in infos)
      {
        DirInfo _file = new DirInfo();
        _file.Name = file.Name;
        _file.Path = RelativePath(IO.Path.Combine(path ?? "", file.Name), rootDirectory);
        _file.ModifiedAt = file.LastWriteTime;

        _file.IconName = "Folder";
#if ICONMGR
        if (!IconMgr.HasIcon(_file.IconName))
        {
          Icon icon = IconMgr.GetAssociatedIcon(file.FullName);
          icon.Name = _file.IconName;
          IconMgr.AddIcon(icon);
        }
#endif
        result.Add(_file);
      }
    }

    /// <summary>
    /// pomocnicza procedura do przekazywania parametrów do metody <see cref="GetFiles"/>
    /// </summary>
    struct GetFilesParam
    {
      public ObservableCollection<FileInfo> Collection;
      public string Path;
      public string RootDirectory;
    }

    /// <summary>
    /// Wyliczenie plików z podanej ścieżki
    /// </summary>
    public static IEnumerable<FileInfo> EnumerateFiles(string path, string rootDirectory = "")
    {
      ObservableCollection<FileInfo> result = new ObservableCollection<FileInfo>();
      GetFiles(new GetFilesParam { Collection = result, Path = path, RootDirectory = rootDirectory });
      return result as IEnumerable<FileInfo>;
    }
    
    /// <summary>
    /// Asynchroniczne wyliczenie katalogów z podanej ścieżki
    /// </summary>
    public static ObservableCollection<FileInfo> EnumerateFilesAsync(string path, string rootDirectory,
      NotifyCollectionChangedEventHandler changeHandler, ObservableCollection<FileInfo> result)
    {
      if (result==null)
        result = new ObservableCollection<FileInfo>();
      result.CollectionChanged += changeHandler;
      Thread aThread = new Thread(GetFiles);
      aThread.Name = String.Format("EnumerateFilesAsync({0})", path);
      aThread.Start(new GetFilesParam { Collection = result, Path = path, RootDirectory = rootDirectory });
      return result;
    }

    /// <summary>
    /// Wyliczenie katalogów z określonej ścieżki. Metoda wywoływana w osobnym wątku
    /// </summary>
    /// <param name="data">zbiorcza struktura umożliwiająca przekazywanie parametrów w postaci jednego obiektu typu <see cref="GetDirectoriesParam"/></param>
    public static void GetFiles(object data)
    {
      GetFilesParam param = (GetFilesParam)data;
      ObservableCollection<FileInfo> result = param.Collection;
      string path = param.Path;
      string rootDirectory = param.RootDirectory;
      IO.DirectoryInfo info = new IO.DirectoryInfo(IO.Path.Combine(rootDirectory ?? "", path ?? ""));
      if (!info.Exists)
        return;

      IO.FileInfo[] files = info.GetFiles();
      foreach (IO.FileInfo file in files)
      {
        FileInfo _file = new FileInfo();
        _file.Name = file.Name;
        _file.Path = RelativePath(IO.Path.Combine(path ?? "", file.Name ?? ""), rootDirectory);
        _file.Size = file.Length;// / 1024;
        _file.ModifiedAt = file.LastWriteTime;


        //_file.Description = GetFileType(file.FullName);
        _file.IconName = IO.Path.GetExtension(file.FullName);
        if (_file.IconName.StartsWith("."))
          _file.IconName = _file.IconName.Substring(1);
#if ICONMGR
        if (!String.IsNullOrEmpty(_file.IconName))
        {
          if (!IconMgr.HasIcon(_file.IconName))
          {
            Icon icon;
            string filename = file.FullName.ToLower();
            string ext = IO.Path.GetExtension(filename);
            if (ext == ".png")
            {
              icon = IconMgr.GetIconFromPng(filename);
              icon.Name = IO.Path.Combine(path, IO.Path.GetFileName(filename));
              _file.IconName = icon.Name;
            }
            else if (ext == ".ico")
            {
              icon = IconMgr.GetIconFromIco(filename);
              icon.Name = IO.Path.Combine(path, IO.Path.GetFileName(filename));
              _file.IconName = icon.Name;
            }
            else
            {
              icon = IconMgr.GetAssociatedIcon(filename);
              icon.Name = _file.IconName;
            }
            IconMgr.AddIcon(icon);
          }
        }
#endif
        result.Add(_file);
      }
    }

    ///// <summary>
    ///// Pobieranie nazwy przyjaznej z pliku
    ///// </summary>
    ///// <param name="filename"></param>
    ///// <returns></returns>
    //private static string GetDisplayName(string filename)
    //{
    //  SHFILEINFO fileInfo = new SHFILEINFO();
    //  int cbFileInfo = Marshal.SizeOf(fileInfo);
    //  SHGetFileInfo(filename, 0, ref fileInfo, (uint)cbFileInfo, (uint)SHGFI.DisplayName);
    //  return fileInfo.szDisplayName;
    //}

    ///// <summary>
    ///// Pobieranie rozszerzonej informacji o typie pliku
    ///// </summary>
    ///// <param name="filename"></param>
    ///// <returns></returns>
    //public static string GetFileType(string filename)
    //{
    //  SHFILEINFO fileInfo = new SHFILEINFO();
    //  int cbFileInfo = Marshal.SizeOf(fileInfo);
    //  SHGetFileInfo(filename, 0, ref fileInfo, (uint)cbFileInfo, (uint)SHGFI.TypeName);
    //  return fileInfo.szTypeName;
    //}

    /// <summary>
    /// ścieżka względna <c>path</c> względem <c>rootDirectory</c>
    /// </summary>
    /// <param name="path"></param>
    /// <param name="root"></param>
    /// <returns></returns>
    public static string RelativePath(string path, string root)
    {
      string result = path;
      if (!String.IsNullOrEmpty(root) && path!=null && path.StartsWith(root))
      {
        path.Substring(root.Length);
        if (result.Length > 1 && result[0] == '\\')
          result = result.Substring(1);
      }
      return result;
    }

    /// <summary>
    /// Utworzenie katalogu o podanej ścieżce.
    /// </summary>
    /// <param name="path">ścieżka katalogo do utworzenia</param>
    /// <param name="root">katalog główny</param>
    public static void CreateDirectory(string path, string root=null)
    {
      string fullPath = IO.Path.Combine(root ?? "", path ?? "");
      if (!IO.Directory.Exists(fullPath))
        IO.Directory.CreateDirectory(fullPath);
      else
        throw new InvalidOperationException(String.Format("Directory \"{0}\" already exists", path));
    }

    /// <summary>
    /// Zmiana nazwy/przesunięcie katalogu
    /// </summary>
    /// <param name="path">ścieżka katalogu</param>
    /// <param name="newPath">nowa ścieżka katalogu</param>
    /// <param name="root">katalog główny</param>
    /// <param name="newRoot">nowy katalog główny</param>
    public static void RenameDirectory(string path, string newPath, string root = null, string newRoot = null)
    {
      string fullPath = IO.Path.Combine(root ?? "", path ?? "");
      if (newRoot == null)
        newRoot = root;
      string newFullPath = IO.Path.Combine(newRoot ?? "", newPath ?? "");
      IO.Directory.Move(fullPath, newFullPath);
    }

    /// <summary>
    /// Zmiana nazwy/przesunięcie pliku
    /// </summary>
    /// <param name="path">ścieżka pliku</param>
    /// <param name="newPath">nowa ścieżka pliku</param>
    /// <param name="root">katalog główny</param>
    /// <param name="newRoot">nowy katalog główny</param>
    public static void RenameFile(string path, string newPath, string root = null, string newRoot = null)
    {
      string fullPath = IO.Path.Combine(root ?? "", path ?? "");
      if (newRoot == null)
        newRoot = root;
      string newFullPath = IO.Path.Combine(newRoot ?? "", newPath ?? "");
      IO.File.Move(fullPath, newFullPath);
    }

    /// <summary>
    /// Czy katalog jest pusty
    /// </summary>
    /// <param name="path">ścieżka katalogu</param>
    /// <param name="root">katalog główny</param>
    public static bool IsDirectoryEmpty(string path, string root = null)
    {
      string fullPath = IO.Path.Combine(root ?? "", path ?? "");
      foreach (string dir in IO.Directory.EnumerateDirectories(fullPath))
        return false;
      foreach (string file in IO.Directory.EnumerateFiles(fullPath))
        return false;
      foreach (string entries in IO.Directory.EnumerateFileSystemEntries(fullPath))
        return false;
      return true;
    }

    /// <summary>
    /// Usunięcie katalogu
    /// </summary>
    /// <param name="path">ścieżka katalogu</param>
    /// <param name="root">katalog główny</param>
    /// <param name="recursive">czy usunąć rekurencyjnie</param>
    public static void DeleteDirectory(string path, string root = null, bool recursive = false)
    {
      string fullPath = IO.Path.Combine(root ?? "", path ?? "");
      IO.Directory.Delete(fullPath,recursive);
    }

    /// <summary>
    /// Usunięcie pliku
    /// </summary>
    /// <param name="path">ścieżka pliku</param>
    /// <param name="root">katalog główny</param>
    public static void DeleteFile(string path, string root = null)
    {
      string fullPath = IO.Path.Combine(root ?? "", path ?? "");
      IO.File.Delete(fullPath);
    }  
  }
}