using System.IO;
using System.Reflection;
using System.Windows;

using Microsoft.Win32;

using Qhta.MVVM;
using Qhta.UnicodeBuild.ViewModels;

namespace Qhta.UnicodeBuild.Commands;


/// <summary>
/// Command to browse a name gen file.
/// </summary>
public class BrowseNameGenFileCommand : RelayCommand<WritingSystemViewModel>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="BrowseNameGenFileCommand"/> class.
  /// </summary>
  public BrowseNameGenFileCommand(): base(BrowseNameGenFileCommandExecute)
  {
  }

  private static void BrowseNameGenFileCommandExecute(WritingSystemViewModel? writingSystem)
  {
    if (writingSystem is null) return;
    var dialog = new OpenFileDialog();
    //dialog.FileName = "WritingSystems.txt"; // Default file name
    dialog.DefaultExt = ".txt"; // Default file extension
    dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension
    dialog.InitialDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Resources");

    // Show open file dialog box
    bool? result = dialog.ShowDialog();

    // Process open file dialog box results
    if (result == true)
    {
      // Open document
      string filename = dialog.FileName;
      if (!File.Exists(filename))
      {
        MessageBox.Show(String.Format(Resources.Strings.FileNotFound, filename), Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }
      writingSystem.NameGenFile = filename;
    }
  }

}