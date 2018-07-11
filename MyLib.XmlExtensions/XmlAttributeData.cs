using System.Collections.Generic;

namespace System.Xml
{
  /// <summary>
  /// Pomocnicza klasa danych atrybutu
  /// </summary>
  public class XmlAttributeData
  {
    /// <summary>
    /// Prefiks - przed nazwą atrybutu
    /// </summary>
    public string Prefix;

    /// <summary>
    /// Lokalna nazwa atrybutu
    /// </summary>
    public string LocalName;

    /// <summary>
    /// URI przestrzeni nazw atrybutu
    /// </summary>
    public string NamespaceURI;

    /// <summary>
    /// Wartość tekstowa atrybutu.
    /// </summary>
    public string Value;

    /// <summary>
    /// Konstruktor z parametrami.
    /// </summary>
    public XmlAttributeData(string prefix, string localName, string namespaceURI)
    {
      Prefix = prefix;
      LocalName = localName;
      NamespaceURI = namespaceURI;
    }

    /// <summary>
    /// Utworzenie klucza do sortowania atrybutów.
    /// Atrybuty <c>xmlns</c> podawane są w pierwszej kolejności.
    /// W drugiej - zdefiniowane przez <paramref name="AttributeOrder"/>
    /// Cała reszta w kolejności alfabetycznej.
    /// </summary>
    public string GetKey(IEnumerable<string> AttributeOrder)
    {
      if (Prefix =="xml" || Prefix == "xmlns" || LocalName=="xmlns")
        return "!" + Prefix+":"+LocalName;
      bool found = false;
      int i=0;
      foreach (string s in AttributeOrder)
      {
        if (LocalName == s)
        {
          found = true;
          break;
        }
        i++;
      }
      if (found)
        return "#"+i + Prefix+":"+LocalName;
      return Prefix + ":" + LocalName;
    }

    /// <summary>
    /// Pomocnicza metoda zamiany na łańcuch dla ułatwienia śledzenia.
    /// </summary>
    public override string ToString()
    {
      string result = Prefix;
      if (result != null)
        result += ":";
      result += LocalName + "=\"" + Value + "\"";
      return result;
    }
  }
}
