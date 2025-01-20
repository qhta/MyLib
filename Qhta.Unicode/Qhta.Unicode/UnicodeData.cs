using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.Unicode;
public class UnicodeData: Dictionary<int, CharInfo>
{
  public UnicodeData(string ucdFilePath)
  {
    string[] lines = System.IO.File.ReadAllLines(ucdFilePath);
    foreach (string line in lines)
    {
      string[] parts = line.Split(';');
      CharInfo charInfo = new CharInfo
      {
        CodePoint = int.Parse(parts[0],NumberStyles.HexNumber),
        Name = parts[1],
        Category = parts[2],
        CanonicalCombiningClass = byte.Parse(parts[3]),
        BidiClass = parts[4],
        DecompositionType = parts[5],
        NumericValue = parts[6],
        NumericType = parts[7],
        BidiMirrored = parts[9] == "Y",
        Unicode1Name = parts[10],
        ISOComment = parts[11],
        SimpleUppercaseMapping = parts[12],
        SimpleLowercaseMapping = parts[13],
        SimpleTitlecaseMapping = parts[14],
      };
      Add(charInfo.CodePoint, charInfo);
    }
    NameWordIndex = new NameWordIndex(this);
  }

  public int LoadAliases(string filePath)
  {
    int count = 0;
    string[] lines = System.IO.File.ReadAllLines(filePath);
    foreach (string line in lines)
    {
      if (line.StartsWith("#") || line.Trim().Length==0)
        continue;
      count++;
      string[] parts = line.Split(';');
      int codePoint = int.Parse(parts[0], NumberStyles.HexNumber);
      string alias = parts[1];
      NameAliasType type = Enum.Parse<NameAliasType>(parts[2], true);
      if (TryGetValue(codePoint, out CharInfo? charInfo))
      {
        var aliases = charInfo.Aliases ??= new List<NameAlias>();
        aliases.Add(new NameAlias { CodePoint = codePoint, Alias = alias, Type = type });
      }
    }
    NameWordIndex.LoadAliases(this);
    return count;
  }

  public NameWordIndex NameWordIndex { get; }
}
