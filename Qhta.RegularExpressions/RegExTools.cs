namespace Qhta.RegularExpressions
{

  public static class RegExTools
  {
    public static RegExStatus Max(this RegExStatus w1, RegExStatus w2)
    {
      var w = (w1 >= w2) ? w1 : w2;
      return w;
    }
  }
}
