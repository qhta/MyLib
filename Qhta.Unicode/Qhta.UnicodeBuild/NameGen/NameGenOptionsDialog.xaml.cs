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

  /// <summary>
  /// Dependency property for the <see cref="KnownNumeralsFile"/> property.
  /// </summary>
  public static DependencyProperty KnownNumeralsFileProperty =
    DependencyProperty.Register(nameof(KnownNumeralsFile), typeof(string), typeof(NameGenOptionsDialog),
      new FrameworkPropertyMetadata(null));

  /// <summary>
  /// Code points count property to display in the dialog.
  /// </summary>
  public string KnownNumeralsFile
  {
    get => (string)GetValue(KnownNumeralsFileProperty);
    set => SetValue(KnownNumeralsFileProperty, value);
  }

  private void BrowseKnownNumeralsFile_OnClick(object sender, RoutedEventArgs e)
  {
    var openFileDialog = new Microsoft.Win32.OpenFileDialog
    {
      Title = Strings.KnownNumeralsFileSelection,
      Filter = Strings.CsvFilesFilter,
      InitialDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!, "Resources"),
      DefaultExt = ".csv",
      FileName = KnownNumeralsFile,
      Multiselect = false
    };

    if (openFileDialog.ShowDialog() == true)
    {
      KnownNumeralsFile = openFileDialog.FileName;
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
