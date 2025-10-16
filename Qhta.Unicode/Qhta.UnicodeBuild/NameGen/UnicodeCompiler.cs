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
        if (i + 1 < words.Length && ShouldGroup(words[i], words[i + 1]))
        {
          tokenList.Add($"<{words[i]} {words[i + 1]}>");
          i++; // Skip the next word as it's already grouped
        }
        else
        {
          tokenList.Add(words[i]);
        }
      }

      return string.Join(" ", tokenList).Substring(0, Math.Min(255, string.Join(" ", tokenList).Length));
    }

    private bool ShouldGroup(string word1, string word2)
    {
      // Example rule: Group words if the first word is "WITH" or "AND"
      return word1 == "WITH" || word1 == "AND";
    }
  }

}