using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace System.Drawing
{
  public static class KnownColors
  {
    private static Dictionary<uint, SolidBrush> s_solidBrushCache = new Dictionary<uint, SolidBrush>();
    private static Dictionary<string, KnownColor> s_knownArgbColors = new Dictionary<string, KnownColor>();

    static KnownColors()
    {
      foreach (KnownColor color in Enum.GetValues(typeof(KnownColor)))
      {
        string str = string.Format("#{0,8:X8}", (uint)color);
        s_knownArgbColors[str] = color;
      }
    }

    internal static KnownColor ArgbStringToKnownColor(string argbString)
    {
      KnownColor color;
      string key = argbString.Trim().ToUpper(CultureInfo.InvariantCulture);
      return (!s_knownArgbColors.TryGetValue(key, out color) ? KnownColor.UnknownColor : color);
    }

    public static SolidBrush ColorStringToKnownBrush(string s)
    {
      if (s != null)
      {
        KnownColor color = ColorStringToKnownColor(s);
        if (color != KnownColor.UnknownColor)
        {
          return SolidBrushFromUint((uint)color);
        }
      }
      return null;
    }

    internal static KnownColor ColorStringToKnownColor(string colorString)
    {
      return (KnownColor)Enum.Parse(typeof(KnownColor), colorString);
    }
    //{
    //  if (colorString != null)
    //  {
    //    char ch;
    //    string str = colorString.ToUpper(CultureInfo.InvariantCulture);
    //    switch (str.Length)
    //    {
    //      case 3:
    //        if (str.Equals("RED"))
    //        {
    //          return (KnownColor)(-65536);
    //        }
    //        if (!str.Equals("TAN"))
    //        {
    //          break;
    //        }
    //        return (KnownColor)(-2968436);

    //      case 4:
    //        ch = str[0];
    //        switch (ch)
    //        {
    //          case 'A':
    //            if (!str.Equals("AQUA"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-16711681);

    //          case 'B':
    //            if (!str.Equals("BLUE"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-16776961);

    //          case 'C':
    //            if (!str.Equals("CYAN"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-16711681);

    //          case 'D':
    //          case 'E':
    //          case 'F':
    //            break;

    //          case 'G':
    //            if (str.Equals("GOLD"))
    //            {
    //              return (KnownColor)(-10496);
    //            }
    //            if (!str.Equals("GRAY"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-8355712);

    //          default:
    //            switch (ch)
    //            {
    //              case 'L':
    //                if (!str.Equals("LIME"))
    //                {
    //                  break;
    //                }
    //                return (KnownColor)(-16711936);

    //              case 'N':
    //                if (!str.Equals("NAVY"))
    //                {
    //                  break;
    //                }
    //                return (KnownColor)(-16777088);

    //              case 'P':
    //                if (str.Equals("PERU"))
    //                {
    //                  return (KnownColor)(-3308225);
    //                }
    //                if (str.Equals("PINK"))
    //                {
    //                  return (KnownColor)(-16181);
    //                }
    //                if (!str.Equals("PLUM"))
    //                {
    //                  break;
    //                }
    //                return (KnownColor)(-2252579);

    //              case 'S':
    //                if (!str.Equals("SNOW"))
    //                {
    //                  break;
    //                }
    //                return (KnownColor)(-1286);

    //              case 'T':
    //                if (!str.Equals("TEAL"))
    //                {
    //                  break;
    //                }
    //                return (KnownColor)(-16744320);

    //              default:
    //                break;
    //            }
    //            break;
    //        }
    //        break;

    //      case 5:
    //        ch = str[0];
    //        switch (ch)
    //        {
    //          case 'A':
    //            if (!str.Equals("AZURE"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-983041);

    //          case 'B':
    //            if (str.Equals("BEIGE"))
    //            {
    //              return (KnownColor)(-657956);
    //            }
    //            if (str.Equals("BLACK"))
    //            {
    //              return ~KnownColor.Transparent;
    //            }
    //            if (!str.Equals("BROWN"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-5952982);

    //          case 'C':
    //            if (!str.Equals("CORAL"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-32944);

    //          case 'D':
    //          case 'E':
    //          case 'F':
    //          case 'H':
    //          case 'J':
    //          case 'M':
    //          case 'N':
    //            break;

    //          case 'G':
    //            if (!str.Equals("GREEN"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-16744448);

    //          case 'I':
    //            if (!str.Equals("IVORY"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-16);

    //          case 'K':
    //            if (!str.Equals("KHAKI"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-989556);

    //          case 'L':
    //            if (!str.Equals("LINEN"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-331546);

    //          case 'O':
    //            if (!str.Equals("OLIVE"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-8355840);

    //          default:
    //            if (ch == 'W')
    //            {
    //              if (str.Equals("WHEAT"))
    //              {
    //                return (KnownColor)(-663885);
    //              }
    //              if (str.Equals("WHITE"))
    //              {
    //                return (KnownColor)(-1);
    //              }
    //            }
    //            break;
    //        }
    //        break;

    //      case 6:
    //        ch = str[0];
    //        if (ch == 'B')
    //        {
    //          if (str.Equals("BISQUE"))
    //          {
    //            return (KnownColor)(-6972);
    //          }
    //        }
    //        else if (ch == 'I')
    //        {
    //          if (str.Equals("INDIGO"))
    //          {
    //            return (KnownColor)(-11861886);
    //          }
    //        }
    //        else
    //        {
    //          switch (ch)
    //          {
    //            case 'M':
    //              if (!str.Equals("MAROON"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-8388608);

    //            case 'O':
    //              if (str.Equals("ORANGE"))
    //              {
    //                return (KnownColor)(-23296);
    //              }
    //              if (!str.Equals("ORCHID"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-2461482);

    //            case 'P':
    //              if (!str.Equals("PURPLE"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-8388480);

    //            case 'S':
    //              if (str.Equals("SALMON"))
    //              {
    //                return (KnownColor)(-360334);
    //              }
    //              if (str.Equals("SIENNA"))
    //              {
    //                return (KnownColor)(-6270419);
    //              }
    //              if (!str.Equals("SILVER"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-4144960);

    //            case 'T':
    //              if (!str.Equals("TOMATO"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-40121);

    //            case 'V':
    //              if (!str.Equals("VIOLET"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-1146130);

    //            case 'Y':
    //              if (!str.Equals("YELLOW"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-256);

    //            default:
    //              break;
    //          }
    //        }
    //        break;

    //      case 7:
    //        ch = str[0];
    //        if (ch > 'M')
    //        {
    //          if (ch == 'O')
    //          {
    //            if (str.Equals("OLDLACE"))
    //            {
    //              return (KnownColor)(-133658);
    //            }
    //          }
    //          else if (ch != 'S')
    //          {
    //            if ((ch == 'T') && str.Equals("THISTLE"))
    //            {
    //              return (KnownColor)(-2572328);
    //            }
    //          }
    //          else if (str.Equals("SKYBLUE"))
    //          {
    //            return (KnownColor)(-7876885);
    //          }
    //        }
    //        else
    //        {
    //          switch (ch)
    //          {
    //            case 'C':
    //              if (!str.Equals("CRIMSON"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-2354116);

    //            case 'D':
    //              if (str.Equals("DARKRED"))
    //              {
    //                return (KnownColor)(-7667712);
    //              }
    //              if (!str.Equals("DIMGRAY"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-9868951);

    //            case 'E':
    //            case 'G':
    //              break;

    //            case 'F':
    //              if (!str.Equals("FUCHSIA"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-65281);

    //            case 'H':
    //              if (!str.Equals("HOTPINK"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-38476);

    //            default:
    //              if ((ch != 'M') || !str.Equals("MAGENTA"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-65281);
    //          }
    //        }
    //        break;

    //      case 8:
    //        ch = str[0];
    //        if (ch > 'H')
    //        {
    //          if (ch == 'L')
    //          {
    //            if (str.Equals("LAVENDER"))
    //            {
    //              return (KnownColor)(-1644806);
    //            }
    //          }
    //          else if (ch == 'M')
    //          {
    //            if (str.Equals("MOCCASIN"))
    //            {
    //              return (KnownColor)(-6987);
    //            }
    //          }
    //          else if (ch == 'S')
    //          {
    //            if (str.Equals("SEAGREEN"))
    //            {
    //              return (KnownColor)(-13726889);
    //            }
    //            if (str.Equals("SEASHELL"))
    //            {
    //              return (KnownColor)(-2578);
    //            }
    //          }
    //        }
    //        else if (ch == 'C')
    //        {
    //          if (str.Equals("CORNSILK"))
    //          {
    //            return (KnownColor)(-1828);
    //          }
    //        }
    //        else if (ch != 'D')
    //        {
    //          if ((ch == 'H') && str.Equals("HONEYDEW"))
    //          {
    //            return (KnownColor)(-983056);
    //          }
    //        }
    //        else
    //        {
    //          if (str.Equals("DARKBLUE"))
    //          {
    //            return (KnownColor)(-16777077);
    //          }
    //          if (str.Equals("DARKCYAN"))
    //          {
    //            return (KnownColor)(-16741493);
    //          }
    //          if (str.Equals("DARKGRAY"))
    //          {
    //            return (KnownColor)(-5658199);
    //          }
    //          if (str.Equals("DEEPPINK"))
    //          {
    //            return (KnownColor)(-60269);
    //          }
    //        }
    //        break;

    //      case 9:
    //        switch (str[0])
    //        {
    //          case 'A':
    //            if (!str.Equals("ALICEBLUE"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-984833);

    //          case 'B':
    //            if (!str.Equals("BURLYWOOD"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-2180985);

    //          case 'C':
    //            if (str.Equals("CADETBLUE"))
    //            {
    //              return (KnownColor)(-10510688);
    //            }
    //            if (!str.Equals("CHOCOLATE"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-2987746);

    //          case 'D':
    //            if (str.Equals("DARKGREEN"))
    //            {
    //              return (KnownColor)(-16751616);
    //            }
    //            if (!str.Equals("DARKKHAKI"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-4343957);

    //          case 'F':
    //            if (!str.Equals("FIREBRICK"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-5103070);

    //          case 'G':
    //            if (str.Equals("GAINSBORO"))
    //            {
    //              return (KnownColor)(-2302756);
    //            }
    //            if (!str.Equals("GOLDENROD"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-2448096);

    //          case 'I':
    //            if (!str.Equals("INDIANRED"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-3318692);

    //          case 'L':
    //            if (str.Equals("LAWNGREEN"))
    //            {
    //              return (KnownColor)(-8586240);
    //            }
    //            if (str.Equals("LIGHTBLUE"))
    //            {
    //              return (KnownColor)(-5383962);
    //            }
    //            if (str.Equals("LIGHTCYAN"))
    //            {
    //              return (KnownColor)(-2031617);
    //            }
    //            if (str.Equals("LIGHTGRAY"))
    //            {
    //              return (KnownColor)(-2894893);
    //            }
    //            if (str.Equals("LIGHTPINK"))
    //            {
    //              return (KnownColor)(-18751);
    //            }
    //            if (!str.Equals("LIMEGREEN"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-13447886);

    //          case 'M':
    //            if (str.Equals("MINTCREAM"))
    //            {
    //              return (KnownColor)(-655366);
    //            }
    //            if (!str.Equals("MISTYROSE"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-6943);

    //          case 'O':
    //            if (str.Equals("OLIVEDRAB"))
    //            {
    //              return (KnownColor)(-9728477);
    //            }
    //            if (!str.Equals("ORANGERED"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-47872);

    //          case 'P':
    //            if (str.Equals("PALEGREEN"))
    //            {
    //              return (KnownColor)(-6751336);
    //            }
    //            if (!str.Equals("PEACHPUFF"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-9543);

    //          case 'R':
    //            if (str.Equals("ROSYBROWN"))
    //            {
    //              return (KnownColor)(-4419697);
    //            }
    //            if (!str.Equals("ROYALBLUE"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-12490271);

    //          case 'S':
    //            if (str.Equals("SLATEBLUE"))
    //            {
    //              return (KnownColor)(-9807155);
    //            }
    //            if (str.Equals("SLATEGRAY"))
    //            {
    //              return (KnownColor)(-9404272);
    //            }
    //            if (!str.Equals("STEELBLUE"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-12156236);

    //          case 'T':
    //            if (!str.Equals("TURQUOISE"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-12525360);

    //          default:
    //            break;
    //        }
    //        break;

    //      case 10:
    //        ch = str[0];
    //        if (ch > 'P')
    //        {
    //          if (ch != 'S')
    //          {
    //            if ((ch == 'W') && str.Equals("WHITESMOKE"))
    //            {
    //              return (KnownColor)(-657931);
    //            }
    //          }
    //          else if (str.Equals("SANDYBROWN"))
    //          {
    //            return (KnownColor)(-744352);
    //          }
    //        }
    //        else
    //        {
    //          switch (ch)
    //          {
    //            case 'A':
    //              if (!str.Equals("AQUAMARINE"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-8388652);

    //            case 'B':
    //              if (!str.Equals("BLUEVIOLET"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-7722014);

    //            case 'C':
    //              if (!str.Equals("CHARTREUSE"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-8388864);

    //            case 'D':
    //              if (str.Equals("DARKORANGE"))
    //              {
    //                return (KnownColor)(-29696);
    //              }
    //              if (str.Equals("DARKORCHID"))
    //              {
    //                return (KnownColor)(-6737204);
    //              }
    //              if (str.Equals("DARKSALMON"))
    //              {
    //                return (KnownColor)(-1468806);
    //              }
    //              if (str.Equals("DARKVIOLET"))
    //              {
    //                return (KnownColor)(-7077677);
    //              }
    //              if (!str.Equals("DODGERBLUE"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-14774017);

    //            case 'E':
    //            case 'F':
    //            case 'H':
    //            case 'I':
    //            case 'J':
    //            case 'K':
    //              break;

    //            case 'G':
    //              if (!str.Equals("GHOSTWHITE"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-460545);

    //            case 'L':
    //              if (str.Equals("LIGHTCORAL"))
    //              {
    //                return (KnownColor)(-1015680);
    //              }
    //              if (!str.Equals("LIGHTGREEN"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-7278960);

    //            case 'M':
    //              if (!str.Equals("MEDIUMBLUE"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-16777011);

    //            default:
    //              if (ch == 'P')
    //              {
    //                if (str.Equals("PAPAYAWHIP"))
    //                {
    //                  return (KnownColor)(-4139);
    //                }
    //                if (str.Equals("POWDERBLUE"))
    //                {
    //                  return (KnownColor)(-5185306);
    //                }
    //              }
    //              break;
    //          }
    //        }
    //        break;

    //      case 11:
    //        ch = str[0];
    //        if (ch > 'N')
    //        {
    //          if (ch == 'S')
    //          {
    //            if (str.Equals("SADDLEBROWN"))
    //            {
    //              return (KnownColor)(-7650029);
    //            }
    //            if (str.Equals("SPRINGGREEN"))
    //            {
    //              return (KnownColor)(-16711809);
    //            }
    //          }
    //          else if (ch != 'T')
    //          {
    //            if ((ch == 'Y') && str.Equals("YELLOWGREEN"))
    //            {
    //              return (KnownColor)(-6632142);
    //            }
    //          }
    //          else if (str.Equals("TRANSPARENT"))
    //          {
    //            return KnownColor.Transparent;
    //          }
    //        }
    //        else
    //        {
    //          switch (ch)
    //          {
    //            case 'D':
    //              if (str.Equals("DARKMAGENTA"))
    //              {
    //                return (KnownColor)(-7667573);
    //              }
    //              if (!str.Equals("DEEPSKYBLUE"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-16728065);

    //            case 'E':
    //              break;

    //            case 'F':
    //              if (str.Equals("FLORALWHITE"))
    //              {
    //                return (KnownColor)(-1296);
    //              }
    //              if (!str.Equals("FORESTGREEN"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-14513374);

    //            case 'G':
    //              if (!str.Equals("GREENYELLOW"))
    //              {
    //                break;
    //              }
    //              return (KnownColor)(-5374161);

    //            default:
    //              if (ch != 'L')
    //              {
    //                if ((ch == 'N') && str.Equals("NAVAJOWHITE"))
    //                {
    //                  return (KnownColor)(-8531);
    //                }
    //              }
    //              else
    //              {
    //                if (str.Equals("LIGHTSALMON"))
    //                {
    //                  return (KnownColor)(-24454);
    //                }
    //                if (str.Equals("LIGHTYELLOW"))
    //                {
    //                  return (KnownColor)(-32);
    //                }
    //              }
    //              break;
    //          }
    //        }
    //        break;

    //      case 12:
    //        ch = str[0];
    //        if (ch <= 'D')
    //        {
    //          if (ch != 'A')
    //          {
    //            if ((ch == 'D') && str.Equals("DARKSEAGREEN"))
    //            {
    //              return (KnownColor)(-7357297);
    //            }
    //          }
    //          else if (str.Equals("ANTIQUEWHITE"))
    //          {
    //            return (KnownColor)(-332841);
    //          }
    //        }
    //        else if (ch == 'L')
    //        {
    //          if (str.Equals("LIGHTSKYBLUE"))
    //          {
    //            return (KnownColor)(-7876870);
    //          }
    //          if (str.Equals("LEMONCHIFFON"))
    //          {
    //            return (KnownColor)(-1331);
    //          }
    //        }
    //        else if (ch == 'M')
    //        {
    //          if (str.Equals("MEDIUMORCHID"))
    //          {
    //            return (KnownColor)(-4565549);
    //          }
    //          if (str.Equals("MEDIUMPURPLE"))
    //          {
    //            return (KnownColor)(-7114533);
    //          }
    //          if (str.Equals("MIDNIGHTBLUE"))
    //          {
    //            return (KnownColor)(-15132304);
    //          }
    //        }
    //        break;

    //      case 13:
    //        ch = str[0];
    //        if (ch == 'D')
    //        {
    //          if (str.Equals("DARKSLATEBLUE"))
    //          {
    //            return (KnownColor)(-12042869);
    //          }
    //          if (str.Equals("DARKSLATEGRAY"))
    //          {
    //            return (KnownColor)(-13676721);
    //          }
    //          if (str.Equals("DARKGOLDENROD"))
    //          {
    //            return (KnownColor)(-4684277);
    //          }
    //          if (str.Equals("DARKTURQUOISE"))
    //          {
    //            return (KnownColor)(-16724271);
    //          }
    //        }
    //        else if (ch == 'L')
    //        {
    //          if (str.Equals("LIGHTSEAGREEN"))
    //          {
    //            return (KnownColor)(-14634326);
    //          }
    //          if (str.Equals("LAVENDERBLUSH"))
    //          {
    //            return (KnownColor)(-3851);
    //          }
    //        }
    //        else if (ch == 'P')
    //        {
    //          if (str.Equals("PALEGOLDENROD"))
    //          {
    //            return (KnownColor)(-1120086);
    //          }
    //          if (str.Equals("PALETURQUOISE"))
    //          {
    //            return (KnownColor)(-5247250);
    //          }
    //          if (str.Equals("PALEVIOLETRED"))
    //          {
    //            return (KnownColor)(-2396013);
    //          }
    //        }
    //        break;

    //      case 14:
    //        ch = str[0];
    //        switch (ch)
    //        {
    //          case 'B':
    //            if (!str.Equals("BLANCHEDALMOND"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-5171);

    //          case 'C':
    //            if (!str.Equals("CORNFLOWERBLUE"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-10185235);

    //          case 'D':
    //            if (!str.Equals("DARKOLIVEGREEN"))
    //            {
    //              break;
    //            }
    //            return (KnownColor)(-11179217);

    //          default:
    //            if (ch != 'L')
    //            {
    //              if ((ch == 'M') && str.Equals("MEDIUMSEAGREEN"))
    //              {
    //                return (KnownColor)(-12799119);
    //              }
    //            }
    //            else
    //            {
    //              if (str.Equals("LIGHTSLATEGRAY"))
    //              {
    //                return (KnownColor)(-8943463);
    //              }
    //              if (str.Equals("LIGHTSTEELBLUE"))
    //              {
    //                return (KnownColor)(-5192482);
    //              }
    //            }
    //            break;
    //        }
    //        break;

    //      case 15:
    //        if (str.Equals("MEDIUMSLATEBLUE"))
    //        {
    //          return (KnownColor)(-8689426);
    //        }
    //        if (str.Equals("MEDIUMTURQUOISE"))
    //        {
    //          return (KnownColor)(-12004916);
    //        }
    //        if (!str.Equals("MEDIUMVIOLETRED"))
    //        {
    //          break;
    //        }
    //        return (KnownColor)(-3730043);

    //      case 0x10:
    //        if (!str.Equals("MEDIUMAQUAMARINE"))
    //        {
    //          break;
    //        }
    //        return (KnownColor)(-10039894);

    //      case 0x11:
    //        if (!str.Equals("MEDIUMSPRINGGREEN"))
    //        {
    //          break;
    //        }
    //        return (KnownColor)(-16713062);

    //      case 20:
    //        if (!str.Equals("LIGHTGOLDENRODYELLOW"))
    //        {
    //          break;
    //        }
    //        return (KnownColor)(-329006);

    //      default:
    //        break;
    //    }
    //  }
    //  return KnownColor.UnknownColor;
    //}

    public static bool IsKnownSolidBrush(SolidBrush scp)
    {
      Dictionary<uint, SolidBrush> dictionary = s_solidBrushCache;
      lock (dictionary)
      {
        return s_solidBrushCache.ContainsValue(scp);
      }
    }

    internal static string MatchColor(string colorString, out bool isKnownColor, out bool isNumericColor, out bool isContextColor, out bool isScRgbColor)
    {
      string str = colorString.Trim();
      if ((((str.Length == 4) || ((str.Length == 5) || (str.Length == 7))) || (str.Length == 9)) && (str[0] == '#'))
      {
        isNumericColor = true;
        isScRgbColor = false;
        isKnownColor = false;
        isContextColor = false;
        return str;
      }
      isNumericColor = false;
      if (!str.StartsWith("sc#", StringComparison.Ordinal))
      {
        isScRgbColor = false;
      }
      else
      {
        isNumericColor = false;
        isScRgbColor = true;
        isKnownColor = false;
        isContextColor = false;
      }
      if (!str.StartsWith("ContextColor ", StringComparison.OrdinalIgnoreCase))
      {
        isContextColor = false;
        isKnownColor = true;
        return str;
      }
      isContextColor = true;
      isScRgbColor = false;
      isKnownColor = false;
      return str;
    }

    public static SolidBrush SolidBrushFromUint(uint argb)
    {
      SolidBrush brush = null;
      Dictionary<uint, SolidBrush> dictionary = s_solidBrushCache;
      lock (dictionary)
      {
        if (!s_solidBrushCache.TryGetValue(argb, out brush))
        {
          brush = new SolidBrush(Color.FromArgb((int)argb));
          //brush.Freeze();
          s_solidBrushCache[argb] = brush;
        }
      }
      return brush;
    }

  }
}
