using System.Data;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

namespace Qhta.Unicode;

/// <summary>
/// An index of codePoints with specific char name. Char name is a single-word string.
/// </summary>
public class CharNameIndex : Dictionary<string, CodePoint>
{
  private UnicodeData Ucd = null!;

  /// <summary>
  /// Initializes index.from a UnicodeData object.
  /// </summary>
  /// <param name="ucd"></param>
  public void Initialize(UnicodeData ucd)
  {
    Ucd = ucd;
    CreateShortNamesToFile("CharNames.txt");
  }

  /// <summary>
  /// Add a code point to the this.
  /// </summary>
  /// <param name="charName">Name is a single-word string</param>
  /// <param name="codePoint"></param>
  private void Add(string charName, int codePoint)
  {
    if (this.TryGetValue(charName, out var value))
    {
      throw new DuplicateNameException($"CharName \"{charName}\" already exists");
    }
    else
    {
      base.Add(charName, codePoint);
    }
  }

  private void CreateShortNamesToFile(string filename)
  {
    foreach (CharInfo charInfo in Ucd.Values.OrderBy(item => (int)item.CodePoint))
    {
      var charName = CreateShortName(charInfo);
      if (charName is not null)
      {
        if (this.TryGetValue(charName, out var existingCodePoint))
        {
          var oldName2 = (charInfo.OldName != null) ? CreateShortNameFromString(charInfo.OldName) : null;
          if (oldName2 != null && oldName2 != charName)
            this.Add(oldName2, charInfo.CodePoint);
          else
          {
            var charInfo1 = Ucd[existingCodePoint];
            var oldName1 = (charInfo1.OldName != null) ? CreateShortNameFromString(charInfo1.OldName.ToString()) : null;
            if (oldName1 != null && oldName1 != charName)
            {
              this.Remove(charName);
              Add(oldName1, existingCodePoint);
              Add(charName, charInfo.CodePoint);
            }
            else
              throw new DuplicateNameException($"CharName \"{charName}\" already exists");
          }
        }
        else
          Add(charName, charInfo.CodePoint);
      }
    }
    using (var writer = File.CreateText(filename))
    {
      foreach (var item in this.OrderBy(item => (int)item.Value))
      {
        writer.WriteLine($"{item.Value};{item.Key}");
      }
    }
  }

  /// <summary>
  /// Create a short name for a character.
  /// </summary>
  /// <param name="charInfo"></param>
  /// <returns></returns>
  public static string? CreateShortName(CharInfo charInfo)
  {
    var sb = new StringBuilder();
    string longName = charInfo.Name;
    string? charName = null;
    if (longName[0] == '<' || charInfo.CodePoint == 0x020)
    {
      if (charInfo.Aliases != null)
      {
        var alias = charInfo.Aliases.FirstOrDefault(item => item.Type == NameAliasType.Abbreviation);
        charName = alias?.Name!;
        //Debug.WriteLine($"{charInfo.CodePoint};{charName}");
        if (alias != null)
          return alias!.Name;
      }
      charName = CreateShortNameFromString(longName);
      //Debug.WriteLine($"{charInfo.CodePoint};{charName}");
      return charName;
    }
    charName = CreateShortNameFromString(longName);
    //Debug.WriteLine($"{charInfo.CodePoint};{charName}");
    return charName;
  }

  /// <summary>
  /// Create a short name for a long character name.
  /// </summary>
  /// <param name="longName"></param>
  /// <returns></returns>
  public static string CreateShortNameFromString(string longName)
  {
    var sb = new StringBuilder();
    var ss = longName.Split([' ', ',']);
    //var wasCapital = false;
    for (int i = 0; i < ss.Length; i++)
    {
      var word = ss[i].Replace('-', '_');
      sb.Append(word.ToLower());
    }
    return sb.ToString();
  }
}
