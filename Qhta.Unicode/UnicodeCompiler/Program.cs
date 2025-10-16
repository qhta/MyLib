using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnicodeCompiler
{
  public class UnicodePoint
  {
    public string Code { get; set; }
    public string Description { get; set; }

    public UnicodePoint(string code, string description)
    {
      Code = code;
      Description = description;
    }
  }

  public class UnicodeToken
  {
    public string Code { get; set; }
    public string TokenSequence { get; set; }

    public UnicodeToken(string code, string tokenSequence)
    {
      Code = code;
      TokenSequence = tokenSequence;
    }
  }

  public class Compiler
  {
    private HashSet<string> _knownPhrases;

    public Compiler(string phrasesFilePath)
    {
      _knownPhrases = LoadKnownPhrases(phrasesFilePath);
    }

    public List<UnicodeToken> Compile(List<UnicodePoint> unicodePoints)
    {
      var tokens = new List<UnicodeToken>();

      foreach (var point in unicodePoints)
      {
        var tokenSequence = GenerateTokenSequence(point.Description);
        tokens.Add(new UnicodeToken(point.Code, tokenSequence));
      }

      return tokens;
    }

    private string GenerateTokenSequence(string description)
    {
      var words = description.Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
      var tokenList = new List<string>();

      for (int i = 0; i < words.Length; i++)
      {
        string phrase = words[i];
        int j = i;

        // Try to find the longest matching phrase
        while (j + 1 < words.Length && _knownPhrases.Contains(phrase + " " + words[j + 1]))
        {
          phrase += " " + words[j + 1];
          j++;
        }

        if (j > i) // If a phrase was found
        {
          tokenList.Add($"<{phrase}>");
          i = j; // Skip the grouped words
        }
        else
        {
          tokenList.Add(words[i]);
        }
      }

      return string.Join(" ", tokenList).Substring(0, Math.Min(255, string.Join(" ", tokenList).Length));
    }

    private HashSet<string> LoadKnownPhrases(string filePath)
    {
      var phrases = new HashSet<string>();

      foreach (var line in File.ReadLines(filePath).Skip(1)) // Skip the header
      {
        var parts = line.Split('\t');
        if (parts.Length == 2)
        {
          phrases.Add(parts[0].Trim());
        }
      }

      return phrases;
    }
  }

  class Program
  {
    static void Main(string[] args)
    {
      // Load Unicode points from the file
      var unicodePoints = LoadUnicodePoints("Data/UnicodePoints.txt");

      // Initialize the compiler with known phrases
      var compiler = new Compiler("Data/KnownPhrases.txt");

      // Compile the Unicode points into tokens
      var tokens = compiler.Compile(unicodePoints);

      // Output the tokens
      foreach (var token in tokens)
      {
        Console.WriteLine($"{token.Code}: {token.TokenSequence}");
      }
    }

    private static List<UnicodePoint> LoadUnicodePoints(string filePath)
    {
      var unicodePoints = new List<UnicodePoint>();

      foreach (var line in File.ReadLines(filePath).Skip(1)) // Skip the header
      {
        var parts = line.Split('\t');
        if (parts.Length == 2)
        {
          unicodePoints.Add(new UnicodePoint(parts[0], parts[1]));
        }
      }

      return unicodePoints;
    }
  }
}
