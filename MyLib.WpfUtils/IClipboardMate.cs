using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace MyLib.WpfUtils
{
  public interface IClipboardMate
  {
    bool CanCopy(out string[] dataFormats);
    DataObject[] CopyToClipboard(string[] dataFormats);
  }
}
