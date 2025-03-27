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

