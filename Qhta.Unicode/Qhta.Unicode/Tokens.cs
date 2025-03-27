using System.Diagnostics;

namespace Qhta.Unicode;

/// <summary>
/// A list of tokens for a line of text
/// </summary>
public class Tokens : List<string>
{
  /// <summary>
  /// Default constructor for empty list
  /// </summary>
  [DebuggerStepThrough]
  public Tokens()
  {
  }

  /// <summary>
  /// Constructor with initial strings
  /// </summary>
  /// <param name="strings"></param>
  [DebuggerStepThrough]
  public Tokens(IEnumerable<string> strings)
  {
    this.AddRange(strings);
  }


  /// <summary>
  /// Gets the tokens from a start index to the end of the list
  /// </summary>
  /// <param name="start"></param>
  /// <returns></returns>
  [DebuggerStepThrough]
  public Tokens Substring(int start) => start <= this.Count - 1
    ? new Tokens(this.GetRange(start, this.Count - start))
    : new Tokens();

  /// <summary>
  /// Gets the tokens converted to upper case
  /// </summary>
  /// <returns></returns>
  [DebuggerStepThrough]
  public Tokens ToUpper() => new Tokens(this.Select(s => s.ToUpper()));

  /// <summary>
  /// Gets the tokens converted to lower case
  /// </summary>
  /// <returns></returns>
  [DebuggerStepThrough]
  public Tokens ToLower() => new Tokens(this.Select(s => s.ToLower()));

  /// <summary>
  /// Gets the tokens from a start index in the list for a given length
  /// </summary>
  /// <param name="start"></param>
  /// <param name="length"></param>
  /// <returns></returns>
  [DebuggerStepThrough]
  public Tokens Substring(int start, int length) => new Tokens(this.GetRange(start, length));


  /// <summary>
  /// Add a string to the start of the list
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  [DebuggerStepThrough]
  public Tokens Prepend(string str)
  {
    var tokens = new Tokens(new string[] { str });
    tokens.AddRange(this);
    return tokens;
  }

  /// <summary>
  /// Add strings to the start of the list
  /// </summary>
  /// <param name="strs"></param>
  /// <returns></returns>
  [DebuggerStepThrough]
  public Tokens Prepend(params string[] strs)
  {
    var tokens = new Tokens(strs);
    tokens.AddRange(this);
    return tokens;
  }


  /// <summary>
  /// Add a string to the end of the list
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  [DebuggerStepThrough]
  public Tokens Append(string str)
  {
    var tokens = new Tokens(this);
    tokens.Add(str);
    return tokens;
  }

  /// <summary>
  /// Add strings to the end of the list
  /// </summary>
  /// <param name="strs"></param>
  /// <returns></returns>
  [DebuggerStepThrough]
  public Tokens Append(params string[] strs)
  {
    var tokens = new Tokens(this);
    tokens.AddRange(strs);
    return tokens;
  }

  /// <summary>
  /// Checks if the first token is the same as the given string
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  [DebuggerStepThrough]
  public bool StartsWith(string str) => this.Count > 0 && this.First() == str.Trim();

  /// <summary>
  /// Checks if the first tokens are the same as the given strings
  /// </summary>
  /// <param name="strs"></param>
  /// <returns></returns>
  [DebuggerStepThrough]
  public bool StartsWith(params string[] strs)
  {
    if (this.Count < strs.Length)
      return false;
    for (int i = 0; i < strs.Length; i++)
      if (this[i] != strs[i].Trim())
        return false;
    return true;
  }

  /// <summary>
  /// Checks if the last token is the same as the given string
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  [DebuggerStepThrough]
  public bool EndsWith(string str) => this.Count > 0 && this.Last() == str.Trim();

  /// <summary>
  /// Checks if the last tokens ares the same as the given strings
  /// </summary>
  /// <param name="strs"></param>
  /// <returns></returns>
  [DebuggerStepThrough]
  public bool EndsWith(params string[] strs)
  {
    if (this.Count < strs.Length)
      return false;
    for (int i = 0; i < strs.Length; i++)
        if (this[this.Count - 1 - i] != strs[^i].Trim())
          return false;
    return true;
  }

  /// <summary>
  /// Checks if the list contains the given string
  /// </summary>
  /// <returns></returns>
  [DebuggerStepThrough]
  public override string ToString() => string.Join(" ", this);
}