using System;
using System.Runtime.InteropServices;

namespace Qhta.WPF.Utils
{
  public static partial class ClipboardUtils
  {
    [DllImport("user32.dll")]
    static extern bool OpenClipboard(IntPtr hWndNewOwner);
    [DllImport("user32.dll")]
    static extern bool EmptyClipboard();
    [DllImport("user32.dll")]
    static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
    [DllImport("user32.dll")]
    static extern bool CloseClipboard();
    [DllImport("gdi32.dll")]
    static extern IntPtr CopyEnhMetaFile(IntPtr hemfSrc, IntPtr hNULL);
    [DllImport("gdi32.dll")]
    static extern bool DeleteEnhMetaFile(IntPtr hemf);
  }
}
