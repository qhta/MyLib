using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;

using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.Windows.Shared.Resources;

namespace Qhta.UnicodeBuild;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
  protected override void OnStartup(StartupEventArgs e)
  {
    Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NNaF5cXmBCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXxcd3VSQmdYU01+X0FWYUA=");

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

  protected override void OnExit(ExitEventArgs e)
  {
    _ViewModels.Instance.Dispose();
    base.OnExit(e);
  }
}

