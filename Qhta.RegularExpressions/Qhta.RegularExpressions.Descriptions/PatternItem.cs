using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.RegularExpressions.Descriptions
{
  public class PatternItem
  {
    public string Str { get; set; }

    public string Description { get; set; }

    public bool? IsOK { get; internal set; }

    public override bool Equals(object obj)
    {
      bool result = true;

      if (obj is PatternItem other)
      {
        if (this.Str != other.Str)
        {
          result = false;
        }
        string description1 = Prepare(this.Description);
        string description2 = Prepare(other.Description);
        if (!description1.Equals(description2, StringComparison.CurrentCultureIgnoreCase))
        {
          result = false;
        }
        this.IsOK = result;
        return result;
      }
      return false;
    }

    private static readonly string[] NonComparedWords = new string[]
    {
      "a", "an", "the",
      "marks", "mark",
      "character", "characters",
      "either", "all", "any",
      "or", "and",
      "occurrences", "occurrence",
      "occurences", "occurence",
      "of", "at",
      "Match", "Start",
    };

    private static string Prepare(string description)
    {
      var chars = description.ToCharArray().ToList();
      for (int i = chars.Count - 1; i >= 0; i--)
      {
        var ch = chars[i];
        if (ch == '-')
          chars[i] = ' ';
        else if (ch=='"')
          chars.RemoveAt(i);
        else
        if (Char.IsPunctuation(ch))
          chars.RemoveAt(i);
      }
      description = new string(chars.ToArray());
      var words = description.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
      for (int i = words.Count - 1; i >= 0; i--)
      {
        var word = words[i];
        if (NonComparedWords.Contains(word))
          words.RemoveAt(i);
      }
      return String.Join(" ", words);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}
