using Qhta.RegularExpressions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegExTaggerTest
{
  class Program
  {
    static void Main(string[] args)
    {
      string pattern = @"(?<name_012>subexpression[a-zA-Z0-9])";// @" ((\w++)[\s.])+";
      var tagger = new RegExTagger();
      var result = tagger.TryParseText(pattern);
      Console.WriteLine($"TaggedText = {pattern}");
      Console.WriteLine($"result = {result}");
      Dump(tagger.Items);
      Console.ReadKey();
    }


    static void Dump(RegExItems items, int level=0)
    {
      foreach (var item in items)
      {
        var s = item.ToString();
        Console.WriteLine(new string(' ', level*2)+s);
        if (item is RegExGroup group)
        {
          if (group.Name!=null)
            Console.WriteLine(new string(' ', (level+1) * 2) + $"Name=\"{group.Name}\"");
          Dump(group.Items, level + 1);
        }
        else
        if (item is RegExCharset charset)
          Dump(charset.Items, level + 1);
      }
    }
  }
}
