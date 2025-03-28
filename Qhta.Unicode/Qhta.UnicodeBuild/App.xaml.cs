using System.Configuration;
using System.Data;
using System.Windows;

using Qhta.Unicode.Models;

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

    using (var context = new MyDbContext())
    {
      var data = context.UcdBlocks.ToList();
      foreach (var item in data)
      {
        // Process your data
      }
    }
  }
}

