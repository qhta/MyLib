using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Windows;

using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.ViewModels;

namespace Qhta.UnicodeBuild;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
  protected override void OnStartup(StartupEventArgs e)
  {
    Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NNaF5cXmBCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXxcd3VSQmdYU01+X0FWYUA=");
    base.OnStartup(e);
  }

  protected override void OnExit(ExitEventArgs e)
  {
    _ViewModels.Instance.Dispose();
    base.OnExit(e);
  }
}

