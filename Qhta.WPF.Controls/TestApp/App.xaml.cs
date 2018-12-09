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
      customColorsPath = Path.Combine(appPath, customColorsPath);
      KnownColors.CustomColorsPath = customColorsPath;
      bool? autoSaveCustomColors = Resources["AutoSaveCustomColors"] as bool?;
      if (autoSaveCustomColors==true)
        KnownColors.AutoSaveCustomColors = true;

      var customBrushesPath = Resources["CustomBrushesPath"] as string;
      customBrushesPath = Path.Combine(appPath, customBrushesPath);
      KnownBrushes.CustomBrushesPath = customBrushesPath;
      bool? autoSaveCustomBrushes = Resources["AutoSaveCustomBrushes"] as bool?;
      if (autoSaveCustomBrushes==true)
        KnownBrushes.AutoSaveCustomBrushes = true;

      System.Windows.Resources.StreamResourceInfo info = 
        Application.GetResourceStream(new Uri("/Qhta.WPF.Utils;component/Resources/ArrowPlus.cur", UriKind.Relative));
      var cursor = new System.Windows.Input.Cursor(info.Stream);
      Resources["ArrowPlus"] = cursor;
    }
  }
}
