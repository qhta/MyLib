﻿using System.Globalization;

namespace System.Windows.Media
{
  public static class SR
  {
    private static System.Resources.ResourceManager _resourceManager
      = new System.Resources.ResourceManager("ExceptionStringTable", typeof(SR).Assembly);

    internal static string Get(string id)
    {
      string str = _resourceManager.GetString(id);
      if (str == null)
      {
        str = _resourceManager.GetString("Unavailable");
      }
      return str;
    }

    internal static string Get(string id, params object[] args)
    {
      string format = _resourceManager.GetString(id);
      if (format == null)
      {
        format = _resourceManager.GetString("Unavailable");
      }
      else if ((args != null) && (args.Length != 0))
      {
        format = string.Format(CultureInfo.CurrentCulture, format, args);
      }
      return format;
    }

    internal static System.Resources.ResourceManager ResourceManager
    {
      get
      {
        return _resourceManager;
      }
    }
  }
}