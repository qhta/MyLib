using System.Windows;

using Strings = Qhta.UnicodeBuild.Resources.Strings;

namespace Qhta.UnicodeBuild.Views;
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
  /// Dependency property for the <see cref="CodePointsCount"/> property.
  /// </summary>
  public static DependencyProperty CodePointsCountProperty =
    DependencyProperty.Register(nameof(CodePointsCount), typeof(int), typeof(NameGenOptionsDialog),
      new FrameworkPropertyMetadata(0));

  /// <summary>
  /// Code points count property to display in the dialog.
  /// </summary>
  public int CodePointsCount
  {
    get => (int)GetValue(CodePointsCountProperty);
    set => SetValue(CodePointsCountProperty, value);
  }

  /// <summary>
  /// Dependency property for the <see cref="PredefinedNamesFile"/> property.
  /// </summary>
  public static DependencyProperty PredefinedNamesFileProperty =
    DependencyProperty.Register(nameof(PredefinedNamesFile), typeof(int), typeof(NameGenOptionsDialog),
      new FrameworkPropertyMetadata(0));

  /// <summary>
  /// Code points count property to display in the dialog.
  /// </summary>
  public string PredefinedNamesFile
  {
    get => (string)GetValue(PredefinedNamesFileProperty);
    set => SetValue(PredefinedNamesFileProperty, value);
  }

  private void BrowsePredefinedNameFile_OnClick(object sender, RoutedEventArgs e)
  {
    var openFileDialog = new Microsoft.Win32.OpenFileDialog
    {
      Title = Strings.SelectPrefefinedNamesFile,
      Filter = Strings.TextFilesFilter,
      InitialDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!, "Resources"),
      DefaultExt = ".txt",
      FileName = PredefinedNamesFile,
      Multiselect = false
    };
  }
}
