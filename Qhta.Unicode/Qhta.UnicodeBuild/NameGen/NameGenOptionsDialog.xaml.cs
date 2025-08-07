using System.Windows;

using Strings = Qhta.UnicodeBuild.Resources.Strings;

namespace Qhta.UnicodeBuild.NameGen;

/// <summary>
/// Interaction logic for NameGenOptionsDialog.xaml
/// </summary>
public partial class NameGenOptionsDialog : Window
{
  /// <summary>
  /// Initializes a new instance of the <see cref="NameGenOptionsDialog"/> class.
  /// </summary>
  public NameGenOptionsDialog()
  {
    InitializeComponent();
    Loaded += Window_Loaded;
  }

  private void Window_Loaded(object sender, RoutedEventArgs e)
  {
    MinHeight = Height + 30; // Prevents the window from being too small to display all controls
  }

  /// <summary>
  /// Options for the name generation method.
  /// </summary>
  public NameGenOptions NameGenOptions
  {
    get => (NameGenOptions)DataContext;
    set => DataContext = value with {};
  }

  private void BrowsePredefinedNamesFile_OnClick(object sender, RoutedEventArgs e)
  {
    var openFileDialog = new Microsoft.Win32.OpenFileDialog
    {
      Title = Strings.PredefinedNamesFileSelection,
      Filter = Strings.CsvFilesFilter,
      InitialDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!, "Resources"),
      DefaultExt = ".csv",
      FileName = NameGenOptions.PredefinedNamesFile,
      Multiselect = false
    };

    if (openFileDialog.ShowDialog() == true)
    {
      NameGenOptions.AbbreviatedWordsFile = openFileDialog.FileName;
    }
  }

  private void BrowseAbbreviatedWordsFile_OnClick(object sender, RoutedEventArgs e)
  {
    var openFileDialog = new Microsoft.Win32.OpenFileDialog
    {
      Title = Strings.AbbreviatedWordsFileSelection,
      Filter = Strings.CsvFilesFilter,
      InitialDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!, "Resources"),
      DefaultExt = ".csv",
      FileName = NameGenOptions.AbbreviatedWordsFile,
      Multiselect = false
    };

    if (openFileDialog.ShowDialog() == true)
    {
      NameGenOptions.AbbreviatedWordsFile = openFileDialog.FileName;
    }
  }

  private void OkButton_OnClick(object sender, RoutedEventArgs e)
  {
    DialogResult = true;
    Close();
  }

  private void CancelButton_OnClick(object sender, RoutedEventArgs e)
  {
    DialogResult = false;
    Close();
  }
}
