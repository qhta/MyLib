namespace Qhta.WPF.Utils;

public static partial class ClipboardUtils
{
  /// <summary>
  /// Copies contents of the source object to the clipboard.
  /// Source object must implement <see cref="IClipboardMate"/> interface.
  /// The method first checks whether the source object can copy content in any data format.
  /// Then it gets data objects array and not null objects are copied to the clipboard.
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
  public static bool CopyToClipboard(IClipboardMate source)
  {
    bool success = true;
    Clipboard.Clear();
    if (source.CanCopy(out string[] dataFormats))
    {
      var dataObjects = source.CopyToClipboard(dataFormats);
      foreach (var dataObject in dataObjects)
        if (dataObject != null)
          CopyToClipboard(dataObject);
      return success;
    }
    return false;
  }

  /// <summary>
  /// Helper method to copy data object to clipboard.
  /// If the data object contains EnhancedMetafile data format, then its intPtr is copied to clipboard.
  /// Otherwise simple copy is performed.
  /// </summary>
  /// <param name="dataObject"></param>
  static void CopyToClipboard(DataObject dataObject)
  {
    if (dataObject.GetDataPresent(DataFormats.EnhancedMetafile))
    {
      IntPtr hEMF2 = (IntPtr)dataObject.GetData(DataFormats.EnhancedMetafile);
      if (OpenClipboard(new IntPtr(0)))
      {
        if (EmptyClipboard())
        {
          IntPtr hRes = SetClipboardData(14 /*CF_ENHMETAFILE*/, hEMF2);
          CloseClipboard();
        }
      }
    }
    else
      Clipboard.SetDataObject(dataObject, true);
  }
}
