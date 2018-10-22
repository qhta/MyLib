namespace Qhta.TextUtils
{
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
