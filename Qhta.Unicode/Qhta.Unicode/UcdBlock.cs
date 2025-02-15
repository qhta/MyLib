namespace Qhta.Unicode;

/// <summary>
/// Unicode blocks enumeration
/// </summary>
public class UcdBlock(string name, CodePoint start, CodePoint end) : IEquatable<UcdBlock>
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public readonly string Name = name;
  public readonly CodePoint Start = start;
  public readonly CodePoint End = end;

  public bool Equals(UcdBlock? other)
  {
    if (other is null) return false;
    return Name == other.Name && Start.Equals(other.Start) && End.Equals(other.End);
  }

  public override bool Equals(object? obj) => Equals(obj as UcdBlock);
  public override int GetHashCode() => HashCode.Combine(Name, Start, End);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
