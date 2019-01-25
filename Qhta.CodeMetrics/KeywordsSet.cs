using System;
using System.Collections.Generic;
using System.Text;

namespace Qhta.CodeMetrics
{
  public class KeywordsSet: Dictionary<string, int>
  {
    public KeywordsSet Clone()
    {
      var result = new KeywordsSet();
      foreach (var item in this)
        result.Add(item.Key, item.Value);
      return result;
    }
  }
}
