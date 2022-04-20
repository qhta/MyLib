using Newtonsoft.Json;

using Qhta.Json.Converters;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Qhta.Json
{
  public partial class MultilangText
  {
    public string? Text { get; set; }
    public Dictionary<string, string>? MultiText { get; set; }

    public static implicit operator MultilangText(string aText)
    {
      return new MultilangText{ Text = aText };
    }

    public static implicit operator MultilangText(Dictionary<string, string> mText)
    {
      return new MultilangText { MultiText = mText };
    }


    public static implicit operator string (MultilangText value)
    {
      return value.ToString();
    }

    public override string ToString()
    {
      if (Text!=null)
        return Text;
      if (MultiText!=null)
      {
        return string.Join("", MultiText.Select(item => "{{lang|" + item.Key + "|" + item.Value+"}}"));
      }
      return null;
    }


  }
}
