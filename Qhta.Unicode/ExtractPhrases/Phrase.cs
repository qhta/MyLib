namespace ExtractPhrases;

public class Phrase : List<string>, IEquatable<Phrase>
{
  public Phrase() : base() { }

  public Phrase(IEnumerable<string> collection) : base(collection) { }

  public Phrase(string str) : base(str.Split(' ', StringSplitOptions.RemoveEmptyEntries)) { }

  public bool IsEmpty()
   => this.Count == 0;

  public bool Contains(Phrase other)
  {
    if (other.Count > this.Count) return false;
    for (int i = 0; i <= this.Count - other.Count; i++)
    {
      bool match = true;
      for (int j = 0; j < other.Count; j++)
      {
        if (this[i + j] != other[j])
        {
          match = false;
          break;
        }
      }
      if (match) return true;
    }
    return false;
  }

  public Phrase Remove(Phrase other)
  {
    if (other.Count > this.Count) return other;
    for (int i = 0; i <= this.Count - other.Count; i++)
    {
      bool match = true;
      for (int j = 0; j < other.Count; j++)
      {
        if (this[i + j] != other[j])
        {
          match = false;
          break;
        }
      }
      if (match)
      {
        var result = new Phrase();
        for (int k = 0; k < this.Count; k++)
          if (k<i || k>=i+other.Count)
            result.Add(this[k]);
        return result;
      }
    }
    return other;
  }

  public override int GetHashCode()
  {
    int hash = 17;
    foreach (var word in this)
    {
      hash = hash * 31 + word.GetHashCode();
    }
    return hash;
  }

  public override bool Equals(object? obj)
  {
    if (obj is Phrase phrase)
      return Equals(phrase);
    if (obj is string str)
      return Equals(new Phrase(str));
    return false;
  }

  public bool Equals(Phrase? other)
  {
    if (other == null) return false;
    if (this.Count != other.Count) return false;
    for (int i = 0; i < this.Count; i++)
    {
      if (this[i] != other[i]) return false;
    }
    return true;
  }

  public bool Equals(string? str)
  {
    if (str == null) return false;
    return Equals(new Phrase(str));
    return false;
  }

  public override string ToString()
  {
    return String.Join(" ", this);
  }
}