using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconUtils
{
  [Flags]
  public enum ImageFlags
  {
    Scalable = 1,
    HasAlpha = 2,
    HasTranslucent = 4,
    PartiallyScalable = 8,
    ColorSpaceRGB = 16,
    ColorSpaceCMYK = 32,
    ColorSpaceGRAY = 64,
    ColorSpaceYCBCR = 128,
    ColorSpaceYCCK = 256,
    HasRealDPI = 4096,
    HasRealPixelSize = 8192,
    ReadOnly = 65536,
    Caching = 131072,
  }
}
