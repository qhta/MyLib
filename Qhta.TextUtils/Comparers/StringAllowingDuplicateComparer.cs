namespace Qhta.TextUtils
{
  /// <summary>
  /// Static string comparer allowing duplicates. Can be used to avoid multiple instance creation.
  /// </summary>
  public static class StringAllowingDuplicateComparer
  {
    /// <summary>
    /// Singular instance.
    /// </summary>
    public static DuplicateAllowingComparer<string> Instance
    {
      get;
    } = new();
  }
}
