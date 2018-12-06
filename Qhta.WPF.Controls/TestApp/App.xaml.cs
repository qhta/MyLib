using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;
using Qhta.WPF;

namespace TestApp
{
  public partial class App : Application
  {

    protected override void OnStartup(StartupEventArgs e)
    {
      var appPath = System.AppDomain.CurrentDomain.BaseDirectory;
      appPath= Path.Combine(appPath, "Temp");
      if (!Directory.Exists(appPath))
        Directory.CreateDirectory(appPath);
      var customColorsPath = Resources["CustomColorsPath"] as string;
      Debug.WriteLine($"appPath={appPath}");
      customColorsPath = Path.Combine(appPath, customColorsPath);
      Debug.WriteLine($"customColorsPath={customColorsPath}");
      KnownColors.CustomColorsPath = customColorsPath;
      bool? autoSaveCustomColors = Resources["AutoSaveCustomColors"] as bool?;
      if (autoSaveCustomColors==true)
        KnownColors.AutoSaveCustomColors = true;
    }
  }
}
