using System.Windows;

namespace Qhta.WPF.Utils
{
  public interface IClipboardMate
  {
    bool CanCopy(out string[] dataFormats);
    DataObject[] CopyToClipboard(string[] dataFormats);
  }
}
