using System.Reflection;
using System.Windows;

using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.Windows.Shared.Resources;

namespace Qhta.UnicodeBuild;

using ResourceStrings = Qhta.UnicodeBuild.Resources.Strings;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
  /// <summary>
  /// Method that is called when the application starts.
  /// </summary>
  /// <param name="e"></param>
  protected override void OnStartup(StartupEventArgs e)
  {
    Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzkzMDU5MUAzMzMwMmUzMDJlMzAzYjMzMzAzYm5NRWxRdDZ2NU9wYnJldHk0TDFsN3lGTW04YjJqL3M2NzcvbnlHQ05DWGc9");

    //System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("pl-PL");
    //var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
    //foreach (var asm in loadedAssemblies)
    //{
    //  System.Diagnostics.Debug.WriteLine(asm.FullName);
    //}
    ResourceWrapper.SetResources(
      Assembly.GetExecutingAssembly(), // Correctly passing the assembly
      "Qhta.UnicodeBuilder.Resources.Syncfusion.SfGrid.WPF.pl"); // Namespace + resource file name
    ResourceWrapper.SetResources(
      Assembly.GetExecutingAssembly(), // Correctly passing the assembly
      "Qhta.UnicodeBuilder.Resources.Syncfusion.PropertyGrid.WPF.pl"); // Namespace + resource file name
  }

  /// <summary>
  /// Method that is called when the application exits.
  /// </summary>
  /// <param name="e"></param>
  protected override void OnExit(ExitEventArgs e)
  {
    if (_ViewModels.Instance.DbContext.ThereAreUnsavedChanges)
    {
      var result = MessageBox.Show(
        ResourceStrings.UnsavedDataChanges, ResourceStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Warning);
      if (result == MessageBoxResult.No)
        _ViewModels.Instance.DbContext.AutoSaveChanges = false;
      else
      {
        _ViewModels.Instance.DbContext.AutoSaveChanges = true;
        MessageBox.Show(
          ResourceStrings.DataSaveStarted, ResourceStrings.Info, MessageBoxButton.YesNo, MessageBoxImage.Information);
      }
    }

    _ViewModels.Instance.Dispose();
    base.OnExit(e);
  }
}

