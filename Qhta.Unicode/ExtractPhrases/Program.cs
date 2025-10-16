using System.Collections.Generic;
using System.Diagnostics;

namespace ExtractPhrases;

internal class Program
{

  static bool RemoveLastHexadecimal(ref string str)
  {
    var pos = str.LastIndexOfAny(['-',' ']);
    if (pos > 0 && pos<str.Length-2)
    {
      var isHexadecimal = false;
      for (int i = pos + 1; i < str.Length; i++)
      {
        if (Char.IsAsciiDigit(str[i]))
          isHexadecimal = true;
        else if (!Char.IsAsciiHexDigit(str[i]))
        {
          isHexadecimal = false;
          break;
        }
      }
      if (isHexadecimal)
      {
        str = str.Substring(0, pos).TrimEnd();
        return true;
      }
    }
    return false;
  }

  static void Main(string[] args)
  {
    PhraseCollection Phrases = new();
    string[] inputLines;
    using (var input = File.OpenText(@"d:\Dane\Lines.txt"))
    {
      inputLines = input.ReadToEnd().Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);
    }

    // Process each line. Generate all phrases (continuous sequences of words) and count their occurrences.
    foreach (var line in inputLines)
    {
      if (line.StartsWith("<")) continue;
      var str = line;
      
      while (RemoveLastHexadecimal(ref str));
      if (str=="") continue;

      var parts = new List<string>();
      var pos = str.IndexOf(" WITH ");
      if (pos > 0)
      {
        parts.Add(str.Substring(0, pos));
        str = str.Substring(pos + 6);
        while (str != "")
        {
          pos = str.IndexOf(" AND ");
          if (pos > 0)
          {
            parts.Add(str.Substring(0, pos));
            str = str.Substring(pos + 5);
          }
          else
          {
            pos = str.IndexOf(" WITH ");
            if (pos > 0)
            {
              parts.Add(str.Substring(0, pos));
              str = str.Substring(pos + 6);
            }
            else
            {
              parts.Add(str);
              str = "";
            }
          }
        }
      }
      else
        parts.Add(str);

      foreach (var part in parts)
      {
        var words = part.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < words.Length; i++)
          for (int j = i; j < words.Length; j++)
          {
            var phrase = new Phrase();
            for (int k = i; k <= j; k++)
            {
              phrase.Add(words[k]);
            }
            if (!Phrases.TryAdd(phrase, 1))
              Phrases[phrase]++;
          }
      }
    }

    Console.WriteLine($"Generated {Phrases.Count} phrases");

    var removedPhrasesCount = Phrases.RemoveSingleCountPhrases();
    Console.WriteLine($"Removed {removedPhrasesCount} phrases that occur only once");
    Console.WriteLine($"{Phrases.Count} phrases left");

    removedPhrasesCount = Phrases.RemoveDuplicatedFragmentPhrases();
    Console.WriteLine($"Removed {removedPhrasesCount} phrases that are contained in another phrase with the same count");
    Console.WriteLine($"{Phrases.Count} phrases left");

    var changedPhrasesCount = 0;
    do
    {
      Console.WriteLine($"Searching phrases that contain single-word phrases");
      changedPhrasesCount = Phrases.RemoveShortPhrasesFromOthers(1);
      Console.WriteLine($"{Phrases.Count} phrases left");
    } while (changedPhrasesCount>0);

    do
    {
      Console.WriteLine($"Searching phrases that contain two-word phrases");
      changedPhrasesCount = Phrases.RemoveShortPhrasesFromOthers(2);
      Console.WriteLine($"{Phrases.Count} phrases left");
    } while (changedPhrasesCount > 0);

    // Save phrases to the file
    using (var output = File.CreateText(@"d:\Dane\Phrases.txt"))
    {
      foreach (var phrase in Phrases)
        output.WriteLine($"{phrase.Key}\t{phrase.Value}");
    }
  }
}
