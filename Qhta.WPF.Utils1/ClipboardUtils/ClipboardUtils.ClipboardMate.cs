using System;
using System.Windows;

namespace Qhta.WPF.Utils
{
  public static partial class ClipboardUtils
  {
    public static bool CopyToClipboard(IClipboardMate clipboardMate)
    {
      bool success = true;
      Clipboard.Clear();
      if (clipboardMate.CanCopy(out string[] dataFormats))
      {
        var dataObjects = clipboardMate.CopyToClipboard(dataFormats);
        foreach (var dataObject in dataObjects)
          if (dataObject!=null)
          CopyToClipboard(dataObject);
        return success;
      }
      return false;
    }

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
}
