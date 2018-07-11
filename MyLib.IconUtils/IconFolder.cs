using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MyLib.FileUtils;

namespace MyLib.IconUtils
{
  /// <summary>
  /// Folder ikon. Zawiera listę ikon oraz listę folderów składowych
  /// </summary>
  [DataContract]
  [KnownType(typeof(IconFolder))]
  [KnownType(typeof(IconInfo))]
  public class IconFolder : DirInfo
  {
  }
}