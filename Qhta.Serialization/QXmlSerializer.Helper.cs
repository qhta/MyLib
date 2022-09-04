namespace Qhta.Xml.Serialization;

public partial class QXmlSerializer
{


  public static string ChangeCase(string str, SerializationCase nameCase)
  {
    switch (nameCase)
    {
      case SerializationCase.LowercaseFirstLetter:
        return FirstLetterToLower(str);
      case SerializationCase.UppercaseFirstLetter:
        return FirstLetterToUpper(str);
    }
    return str;
  }

  public static string FirstLetterToLower(string text)
  {
    if (string.IsNullOrEmpty(text))
      return text;
    char[] ss = text.ToCharArray();
    for (int i = 0; i < ss.Length; i++)
      if (Char.IsLetter(ss[0]))
        ss[i] = char.ToUpper(ss[i]);
    return new string(ss);
  }

  public static string FirstLetterToUpper(string text)
  {
    if (string.IsNullOrEmpty(text))
      return text;
    char[] ss = text.ToCharArray();
    for (int i=0; i<ss.Length; i++)
      if (Char.IsLetter(ss[0]))
        ss[i] = char.ToLower(ss[i]);
    return new string(ss);
  }

  public static bool IsFirstLetterLower(string text)
  {
    if (string.IsNullOrEmpty(text))
      return false;
    foreach (var ch in text)
      if (char.IsLetter(ch) && !Char.IsUpper(ch))
        return false;
    return true;
  }

  public static bool IsFirstLetterUpper(string text)
  {
    if (string.IsNullOrEmpty(text))
      return false;
    foreach (var ch in text)
      if (char.IsLetter(ch) && !Char.IsUpper(ch))
        return false;
    return true;
  }
}