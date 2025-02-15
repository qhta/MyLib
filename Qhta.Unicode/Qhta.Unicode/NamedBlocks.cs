namespace Qhta.Unicode;

/// <summary>
/// A list of named blocks. Defines a LoadFromFile method to load from a file.
/// </summary>
public class NamedBlocks : List<NamedBlock>
{
  /// <summary>
  /// Load a list of named blocks from a file
  /// </summary>
  /// <param name="fileName"></param>
  public void LoadFromFile(string fileName)
  {
    using (var reader = File.OpenText(fileName))
    {
      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        if (line is not null && !line.StartsWith("#"))
        {
          var parts = line.Split([';']);
          if (parts.Length > 1)
          {

            var block = new NamedBlock
            {
              Range = parts[0],
              Name = parts[1],
              KeyString = EmptyToNull(parts[2]),
              BlockType = String.IsNullOrEmpty(parts[3]) ? BlockType.Unknown : EnumHelper.ParseEnum<BlockType>(parts[3])
            };
            Add(block);
          }
        }
      }
    }
  }

  /// <summary>
  /// Try to get a named block by code point and description
  /// </summary>
  /// <param name="codePoint"></param>
  /// <param name="description"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public bool TryGetValue(CodePoint codePoint, string description, out NamedBlock? value)
  {
    foreach (var block in this)
    {
      if (block.Contains(codePoint, description))
      {
        value = block;
        return true;
      }
    }
    value = null;
    return false;
  }

  private static string? EmptyToNull(string s) => string.IsNullOrEmpty(s) ? null : s;

}