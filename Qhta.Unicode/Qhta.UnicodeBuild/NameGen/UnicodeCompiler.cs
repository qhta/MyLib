namespace UnicodeCompiler
{
  /// <summary>
  /// UnicodePoint represents an entry in UnicodeData.txt, containing the Unicode code point and its description.
  /// </summary>
  public class UnicodePoint
  {
    /// <summary>
    /// Code is the Unicode code point in hexadecimal format (e.g., "U+1F600").
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Description is the textual description of the Unicode code point.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Initializing constructor for UnicodePoint, taking the code and description as parameters.
    /// </summary>
    /// <param name="code">The Unicode code point in hexadecimal format (e.g., "U+1F600").</param>
    /// <param name="description">The textual description of the Unicode code point.</param>
    public UnicodePoint(string code, string description)
    {
      Code = code;
      Description = description;
    }
  }

  /// <summary>
  /// A single token in Unicode Point compilation, containing the Unicode code and the generated token sequence based on the description.
  /// </summary>
  public class UnicodeToken
  {
    /// <summary>
    /// Code is the Unicode code point in hexadecimal format (e.g., "U+1F600").
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// TokenSequence is the generated token sequence based on the Unicode point's description.
    /// </summary>
    public string TokenSequence { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="UnicodeToken"/> class with the specified code and token sequence.
    /// </summary>
    /// <param name="code">The Unicode character code.</param>
    /// <param name="tokenSequence">The token sequence representation.</param>
    public UnicodeToken(string code, string tokenSequence)
    {
      Code = code;
      TokenSequence = tokenSequence;
    }
  }

  /// <summary>
  /// A compiler of Unicode points that generates token sequences based on their descriptions, following specific rules for grouping words and limiting the length of the token sequence.
  /// </summary>
  public class Compiler
  {
    /// <summary>
    /// Main execution method that takes a list of Unicode points and generates a list of Unicode tokens, where each token contains the Unicode code and the generated token sequence based on the description.
    /// </summary>
    /// <param name="unicodePoints"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Generates token sequence from the description of a Unicode point, following specific rules for grouping words (e.g., grouping "WITH" and the following word) and ensuring that the final token sequence does not exceed 255 characters in length.
    /// </summary>
    /// <param name="description">The description of the Unicode point.</param>
    /// <returns>The generated token sequence.</returns>
    private string GenerateTokenSequence(string description)
    {
      var words = description.Split([' ', '-'], StringSplitOptions.RemoveEmptyEntries);
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

    /// <summary>
    /// Check for the words that should be grouped together based on specific rules (e.g., "WITH" and the following word should be grouped together).
    /// </summary>
    /// <param name="word1">The first word to check.</param>
    /// <param name="word2">The second word to check.</param>
    /// <returns>True if the words should be grouped together; otherwise, false.</returns>
    private bool ShouldGroup(string word1, string word2)
    {
      // Example rule: Group words if the first word is "WITH" or "AND"
      return word1 == "WITH" || word2 == "AND";
    }
  }

}