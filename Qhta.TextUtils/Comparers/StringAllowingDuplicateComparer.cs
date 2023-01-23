namespace Qhta.TextUtils
{
  /// <summary>
  /// Static string comparer allowing duplicates. Can be used to avoid multiple instance creation.
  /// </summary>
  public static class StringAllowingDuplicateComparer
  {
    static DuplicateAllowingCompararer<string> instance = new DuplicateAllowingCompararer<string>();

    /// <summary>
    /// Singular instance.
    /// </summary>
    public static DuplicateAllowingCompararer<string> Instance
    {
      get
      {
        return instance;
      }
    }
  }
}
