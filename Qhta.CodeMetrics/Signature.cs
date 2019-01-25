using System.Collections.Generic;

namespace Qhta.CodeMetrics
{
  public class Signature
  {
    public int FilesCount { get; set; }

    public bool IsText { get; set; }

    public byte[] Data { get; set; }

    public KeywordsSet Keywords { get; set; }
  }
}
