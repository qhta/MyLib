namespace Qhta.WPF.Utils;

/// <summary>
/// Interface that defines CanCopy and CopyToClipboard funtions.
/// </summary>
public interface IClipboardMate
{
  /// <summary>
  /// Specifies whether the object can copy its content to clipboard using any of the specified data formats.
  /// </summary>
  /// <param name="dataFormats"></param>
  /// <returns></returns>
  bool CanCopy(out string[] dataFormats);

  /// <summary>
  /// Generates DataObject array with content to copy do clipboard using the specified data formats.
  /// Some of the items can be null.
  /// </summary>
  /// <param name="dataFormats"></param>
  /// <returns></returns>
  DataObject?[] CopyToClipboard(string[] dataFormats);
}
