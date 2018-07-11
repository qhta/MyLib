using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace MyLib.FileUtils
{
  /// <summary>
  /// Plik dostępny na serwerze
  /// </summary>
  [DataContract]
  public class FileInfo
  {
    /// <summary>
    /// Nazwa pliku
    /// </summary>
    [DataMember]
    public string Name { get; set; }
    /// <summary>
    /// ścieżka pliku na serwerze
    /// </summary>
    [DataMember]
    public string Path { get; set; }
    /// <summary>
    /// Data ostatniej modyfikacji
    /// </summary>
    [DataMember]
    public DateTime ModifiedAt { get; set; }
    /// <summary>
    /// Rozmiar pliku
    /// </summary>
    [DataMember]
    public long Size { get; set; }
    /// <summary>
    /// Krótki opis pliku
    /// </summary>
    [DataMember]
    public string Description { get; set; }
    /// <summary>
    /// Nazwa ikony
    /// </summary>
    [DataMember]
    public string IconName { get; set; }
  }


  /// <summary>
  /// Katalog dostępny na serwerze
  /// </summary>
  public class DirInfo : FileInfo
  {
    /// <summary>
    /// Konstruktor inicjujący
    /// </summary>
    public DirInfo()
    {
      Items = new ObservableCollection<FileInfo>();
    }
    /// <summary>
    /// Podkatalogi i pliki zawarte w katalogu
    /// </summary>
    public ObservableCollection<FileInfo> Items { get; set; }
  }

  /// <summary>
  /// Urządzenie dostępne na serwerze
  /// </summary>
  public class DriveInfo : DirInfo
  {
    /// <summary>
    /// Konstruktor inicjujący
    /// </summary>
    public DriveInfo()
    {
      Path = "\\";
    }
  }

  /// <summary>
  /// Informacje o samym serwerze
  /// </summary>
  public class MachineInfo : FileInfo
  {
  }
}
