using System;
using System.Collections.Generic;
using System.Text;

namespace MyLib.WpfUtils
{
  public interface IClipboardMate
  {
    bool CanCopy { get; }
    void CopyToClipboard();
    bool CanCut { get; }
    void CutToClipboard();
    bool CanPaste { get; }
    void PasteFromClipboard();
  }
}
