namespace Qhta.WPF.Utils;

public static partial class ClipboardUtils
{
  /// <summary>
  /// Interop method to open the clipboard.
  /// </summary>
  /// <param name="hWndNewOwner"></param>
  /// <returns></returns>
  [DllImport("user32.dll")]
  static extern bool OpenClipboard(IntPtr hWndNewOwner);

  /// <summary>
  /// Interop method to empty the clipboard.
  /// </summary>
  /// <returns></returns>
  [DllImport("user32.dll")]
  static extern bool EmptyClipboard();

  /// <summary>
  /// Interop method to set the clipboard data.
  /// </summary>
  /// <param name="uFormat"></param>
  /// <param name="hMem"></param>
  /// <returns></returns>
  [DllImport("user32.dll")]
  static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

  /// <summary>
  /// Interop method to close the clipboard.
  /// </summary>
  /// <returns></returns>
  [DllImport("user32.dll")]
  static extern bool CloseClipboard();

  /// <summary>
  /// Interop method to copy enhanced metafile to the clipboard.
  /// </summary>
  /// <param name="hemfSrc"></param>
  /// <param name="hNULL"></param>
  /// <returns></returns>
  [DllImport("gdi32.dll")]
  static extern IntPtr CopyEnhMetaFile(IntPtr hemfSrc, IntPtr hNULL);

  /// <summary>
  /// Interop method to delete enhanced metafile.
  /// </summary>
  /// <param name="hemf"></param>
  /// <returns></returns>
  [DllImport("gdi32.dll")]
  static extern bool DeleteEnhMetaFile(IntPtr hemf);
}
