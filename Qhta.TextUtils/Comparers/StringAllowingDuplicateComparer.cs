namespace Qhta.TextUtils
{
  /// <summary>
  /// Static string comparer allowing duplicates. Can be used to avoid multiple instance creation.
  /// </summary>
  public static class StringAllowingDuplicateComparer
  {
    static DuplicateAllowingCompararer<string> instance = new DuplicateAllowingCompararer<string>();

    public static DuplicateAllowingCompararer<string> Instance
    {
      get
      {
        return instance;
      }
    }
  }
}
