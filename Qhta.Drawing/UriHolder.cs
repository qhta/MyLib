namespace System
{
  using System;
  using System.Runtime.InteropServices;

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct UriHolder
  {
    internal Uri BaseUri;
    internal Uri OriginalUri;
  }
}
