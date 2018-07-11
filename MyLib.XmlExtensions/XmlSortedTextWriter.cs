using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Xml
{

  /// <summary>
  /// Klasa zastępująca <c>TextWriter</c> sortująca atrybuty.
  /// Atrybuty <c>xmlns</c> podawane są w pierwszej kolejności.
  /// W drugiej - zdefiniowane przez <see cref="AttributeOrder"/>.
  /// Cała reszta w kolejności alfabetycznej.
  /// </summary>
  public class XmlSortedTextWriter: XmlTextWriter
  {
    #region konstruktory
    /// <summary>
    ///  Kreator z wejściowym strumieniem i kodowaniem.
    ///  Zastępuje kreator odziedziczony po klasie <c>XmlTextWriter</c>.
    /// </summary>
    public XmlSortedTextWriter(Stream w, Encoding encoding) : base(w, encoding) {}

    /// <summary>
    ///  Kreator z wejściową nazwą pliku i kodowaniem.
    ///  Zastępuje kreator odziedziczony po klasie <c>XmlTextWriter</c>.
    /// </summary>
    public XmlSortedTextWriter(String filename, Encoding encoding) : base(filename, encoding) { }

    /// <summary>
    ///  Kreator z wejściowym parametrem klasy <c>TextWriter</c>.
    ///  Zastępuje kreator odziedziczony po klasie <c>XmlTextWriter</c>.
    /// </summary>
    public XmlSortedTextWriter(TextWriter aWriter): base(aWriter) 
    {
      _Writer = aWriter;
    } 
    #endregion

    #region statyczne procecury kreacji - na wzór klasy <c>XmlWriter</>

    /// <summary>
    ///   Tworzy instancję klasy <see cref="XmlSortedTextWriter"/> dla podanej nazwy pliku.
    /// </summary>
    public new static XmlWriter Create(string outputFileName)
    {
      return Create(outputFileName, null);
    }

    /// <summary>
    ///   Tworzy instancję klasy <see cref="XmlSortedTextWriter"/> dla podanej nazwy pliku
    ///   i ustawień zapisu w XML.
    /// </summary>
    public new static XmlWriter Create(string outputFileName, XmlWriterSettings settings)
    {
      if (outputFileName == null)
      {
        throw new ArgumentNullException("outputFileName");
      }
      if (settings == null)
      {
        settings = new XmlWriterSettings();
      }
      FileStream fs = null;
      try
      {
        fs = new FileStream(outputFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
        return CreateWriterImpl(fs, settings.Encoding, true, settings);
      }
      catch
      {
        if (fs != null)
        {
          fs.Close();
        }
        throw;
      }
    }

    /// <summary>
    ///   Tworzy instancję klasy <see cref="XmlSortedTextWriter"/> dla podanego strumienia.
    /// </summary>
    public new static XmlWriter Create(Stream output)
    {
      return Create(output, null);
    }

    /// <summary>
    ///   Tworzy instancję klasy <see cref="XmlSortedTextWriter"/> dla podanego strumienia
    ///   i ustawień zapisu w XML.
    /// </summary>
    public new static XmlWriter Create(Stream output, XmlWriterSettings settings)
    {
      if (output == null)
      {
        throw new ArgumentNullException("output");
      }
      if (settings == null)
      {
        settings = new XmlWriterSettings();
      }
      return CreateWriterImpl(output, settings.Encoding, settings.CloseOutput, settings);
    }

    /// <summary>
    ///   Tworzy instancję klasy <see cref="XmlSortedTextWriter"/> dla podanego parametru klasy <c>TextWriter</c>.
    /// </summary>
    public new static XmlWriter Create(TextWriter output)
    {
      return Create(output, null);
    }

    /// <summary>
    ///   Tworzy instancję klasy <see cref="XmlSortedTextWriter"/> dla podanego parametru klasy <c>TextWriter</c>
    ///   i ustawień zapisu w XML.
    /// </summary>
    public new static XmlWriter Create(TextWriter output, XmlWriterSettings settings)
    {
      if (output == null)
      {
        throw new ArgumentNullException("output");
      }
      if (settings == null)
      {
        settings = new XmlWriterSettings();
      }
      return CreateWriterImpl(output, settings);
    }

    /// <summary>
    ///   Tworzy instancję klasy <see cref="XmlSortedTextWriter"/> dla podanego parametru klasy <c>StreamBuilder</c>.
    /// </summary>
    public new static XmlWriter Create(StringBuilder output)
    {
      return Create(output, null);
    }

    /// <summary>
    ///   Tworzy instancję klasy <see cref="XmlSortedTextWriter"/> dla podanego parametru klasy <c>StreamBuilder</c>
    ///   i ustawień zapisu w XML.
    /// </summary>
    public new static XmlWriter Create(StringBuilder output, XmlWriterSettings settings)
    {
      if (output == null)
      {
        throw new ArgumentNullException("output");
      }
      if (settings == null)
      {
        settings = new XmlWriterSettings();
      }
      return CreateWriterImpl(new StringWriter(output, CultureInfo.InvariantCulture), settings);
    }

    /// <summary>
    ///   Tworzy instancję klasy <see cref="XmlSortedTextWriter"/> 
    ///   nadpisującą obiekt klasy <c>XmlWriter</c>.
    /// </summary>
    public new static XmlWriter Create(XmlWriter output)
    {
      return Create(output, null);
    }

    /// <summary>
    ///   Tworzy instancję klasy <see cref="XmlSortedTextWriter"/> 
    ///   nadpisującą obiekt klasy <c>XmlWriter</c>.
    ///   Uwzględnia ustawienia zapisu w XML.
    /// </summary>
    public new static XmlWriter Create(XmlWriter output, XmlWriterSettings settings)
    {
      if (output == null)
      {
        throw new ArgumentNullException("output");
      }
      if (settings == null)
      {
        settings = new XmlWriterSettings();
      }

      // ponieważ metoda AddConformanceWrapper jest prywatna w klasie <c>XmlTextWriter</c>,
      // więc jej wywołanie następuje przez mechanizm refleksji typów.
      MethodInfo aMethod = typeof(XmlSortedTextWriter).GetMethod("AddConformanceWrapper",
        BindingFlags.NonPublic | BindingFlags.Static);
      XmlWriter result = (XmlWriter)aMethod.Invoke(null, new object[] { output, output.Settings, settings });
      return result;
      // return AddConformanceWrapper(output, output.Settings, settings);
    }

    /// <summary>
    /// Wspólna metoda utworzenia instancji klasy <see cref="XmlSortedTextWriter"/>
    /// </summary>
    private static XmlWriter CreateWriterImpl(Stream output, Encoding encoding, bool closeOutput, XmlWriterSettings settings)
    {
      Debug.Assert(output != null);
      Debug.Assert(encoding != null);
      Debug.Assert(settings != null);

      XmlSortedTextWriter writer = new XmlSortedTextWriter(output, encoding);
      writer.SetSettings(settings);

      // ponieważ chcemy zapewnić dostęp z zewnątrz do właściwości <see cref="NamespaceManage"/>
      // więc nie możemy utworzyć nadpisującej instancji klasy <c>XmlWellFormedWriter</c>
      //writer = new XmlWellFormedWriter(writer, settings);
      return writer;
    }

    private static XmlWriter CreateWriterImpl(TextWriter output, XmlWriterSettings settings)
    {
      Debug.Assert(output != null);
      Debug.Assert(settings != null);

      XmlSortedTextWriter writer = new XmlSortedTextWriter(output);
      writer.SetSettings(settings);

      // ponieważ chcemy zapewnić dostęp z zewnątrz do właściwości <see cref="NamespaceManage"/>
      // więc nie możemy utworzyć nadpisującej instancji klasy <c>XmlWellFormedWriter</c>
      //writer = new XmlWellFormedWriter(writer, settings);
      return writer;
    }
    #endregion

    /// <summary>
    /// Wewnętrzny <c>TextWriter</c> potrzebny do wypisania znacznika UTF-8
    /// </summary>
    private TextWriter _Writer;

    private XmlWriterSettings fSettings;
    /// <summary>
    /// Ustawienia sposobu zapisu w XML. 
    /// Umożliwiają zapis niektórych ustawień w klasie <c>XmlTextWriter</c>
    /// </summary>
    public override XmlWriterSettings Settings
    {
      get { return fSettings; }
    }

    /// <summary>
    /// Metoda zapisu ustawień. Konieczna, bo dziedziczona właściwość 
    /// <see cref="XmlWriterSettings"/> nie ma metody zapisu.
    /// </summary>
    public void SetSettings(XmlWriterSettings value)
    {
      if (value.Indent)
        base.Formatting = Formatting.Indented;
      else
        base.Formatting = Formatting.None;
      base.Namespaces = false;
      FieldInfo aEncoding = typeof(XmlTextWriter).GetField("encoding", BindingFlags.NonPublic | BindingFlags.Instance);
      aEncoding.SetValue(this, value.Encoding);
      fSettings = value;
    }

    /// <summary>
    ///  Przechowuje predefiniowane przestrzenie nazw.
    /// </summary>
    public XmlNamespaceManager NamespaceManager;

    /// <summary>
    ///  Przechowuje w kolejności nazwy atrybutów.
    /// </summary>
    public String[] AttributeOrder = new string[0];

    /// <summary>
    ///  Przechowuje nazwy atrybutów ukrywanych.
    /// </summary>
    public String[] HiddenAttributes = new string[0];

    /// <summary>
    ///   Lista przechowywanych atrybutów sortowanych wg klucza, którego głównym składnikiem jest nazwa atrybutu.
    /// </summary>
    /// 
    private SortedList<string, XmlAttributeData> Attributes = new SortedList<string, XmlAttributeData>();

    /// <summary>
    /// Bieżąco wypełniany atrybut.
    /// </summary>
    private XmlAttributeData currentAttribute;

    /// <summary>
    ///  Umożliwia wyszukanie prefiksu na podstawie URL w predefiniowanej przestrzeni nazw.
    /// </summary>
    public override string LookupPrefix(string ns)
    {
      if (NamespaceManager == null)
        return null;
      return NamespaceManager.LookupPrefix(ns);
    }

    /// <summary>
    ///  Rozpoczęcie dokumentu - wypisywany znacznik UTF-8 i standardowa deklaracja pliku XML.
    /// </summary>
    public override void WriteStartDocument()
    {
      if (_Writer!=null)
        _Writer.Write("\uFEFF");
      base.WriteStartDocument();
    }

    /// <summary>
    ///  Rozpoczęcie dokumentu - wypisywany znacznik UTF-8 i standardowa deklaracja pliku XML 
    ///  (z atrybutem <c>standalone</c>).
    /// </summary>
    public override void WriteStartDocument(bool standalone)
    {
      if (_Writer != null)
        _Writer.Write("\uFEFF");
      base.WriteStartDocument(standalone);
    }

    /// <summary>Stan - wewnątrz tworzenia atrybutu - atrybut jest zapamiętywany na liście.</summary>
    bool inAttr;

    /// <summary>Stan - wewnątrz wyrzucania atrybutów z listy.</summary>
    bool inFlush;

    /// <summary>Stan - dla metody <c>WriteStartElement</c> - jest to pierwszy, główny element.</summary>
    bool isRoot = true;

    /// <summary>
    /// Nadpisana metoda rozpoczęcia elementu. Jeśli to jest pierwszy element,
    /// to wypisywana jest deklaracja pliku XML oraz wszystkie predefiniowane
    /// przestrzenie nazw.
    /// </summary>
    public override void WriteStartElement(string prefix, string localName, string ns)
    {
      if (WriteState == WriteState.Start && !Settings.OmitXmlDeclaration)
        WriteStartDocument();
      FlushAttributes();
      base.WriteStartElement(null, PrepareName(prefix, localName, ns), null);
      if (isRoot)
      {
        inFlush = true;
        //base.WriteAttributeString("xml", "space", null, "preserve");
        if (NamespaceManager != null)
        {
          foreach (string nm in NamespaceManager)
          {
            if (nm != "xmlns" && nm != "xml")
            {
              string aName = "xmlns";
              if (!String.IsNullOrEmpty(nm))
                aName += ":" + nm;
              base.WriteAttributeString(aName, NamespaceManager.LookupNamespace(nm));
            }
          }
        }
        inFlush = false;
        isRoot = false;
      }
    }

    /// <summary>
    /// Nadpisana metoda zakończenia elementu.
    /// </summary>
    public override void WriteEndElement()
    {
      if (Attributes.Count != 0)
        FlushAttributes();
      base.WriteEndElement();
    }

    /// <summary>
    /// Nadpisana metoda rozpoczęcia atrybutu
    /// </summary>
    public override void WriteStartAttribute(string prefix, string localName, string ns)
    {
      if (inFlush)
        base.WriteStartAttribute(null, PrepareName(prefix, localName, ns), null);
      else
      {
        inAttr = true;
        currentAttribute = new XmlAttributeData(prefix, localName, ns);
        Attributes.Add(currentAttribute.GetKey(AttributeOrder), currentAttribute);
      }
    }

    /// <summary>
    /// Nadpisana metoda zakończenia atrybutu
    /// </summary>
    public override void WriteEndAttribute()
    {
      if (inFlush)
        base.WriteEndAttribute();
      else
      {
        currentAttribute = null;
        inAttr = false;
      }
    }

    /// <summary>
    /// Nadpisana metoda pisania tekstu - może to być tekst atrybutu lub zawartość elementu.
    /// </summary>
    public override void WriteString(string text)
    {
      if (inFlush)
        base.WriteString(text);
      else if (inAttr && currentAttribute!=null)
      {
        currentAttribute.Value = text;
      }
      else
      {
        if (Attributes.Count != 0)
          FlushAttributes();
        base.WriteString(text);
      }
    }

    /// <summary>
    /// Pomocnicza metoda przygotowania nazwy elementu lub atrybutu.
    /// </summary>
    /// <remarks>
    /// Ponieważ zmuszamy <c>XmlTextWriter</c> do wypisania przestrzeni nazw
    /// na początku, to w kolejnych elementach i atrybutach prefiks i przestrzeń
    /// nazw muszą być puste. Stąd konieczność samodzielnego dodawania prefiksu
    /// do nazwy lokalnej na podstawie URL wyszukanego w predefiniowanej
    /// przestrzeni nazw.
    /// </remarks>
    protected string PrepareName(string prefix, string localName, string ns)
    {
      if (ns != null && String.IsNullOrEmpty(prefix))
        prefix = LookupPrefix(ns);
      string aName = prefix;
      if (!String.IsNullOrEmpty(aName) && !String.IsNullOrEmpty(localName))
        aName += ":";
      aName += localName;
      return aName;
    }

    /// <summary>
    /// Pomocnicza metoda wyrzucenia atrybutów z posortowanej listy atrybutów.
    /// </summary>
    protected void FlushAttributes()
    {
      inFlush = true;
      foreach (XmlAttributeData attr in Attributes.Values)
      {
        if (!(attr.Prefix == "xmln" ||
          attr.Prefix == "xml" && attr.LocalName == "space"))
        {
          if (!HiddenAttributes.Contains(attr.LocalName))
            base.WriteAttributeString(attr.Prefix, attr.LocalName, attr.NamespaceURI, attr.Value);
        }
      }
      XmlAttributeData spaceAttribute = (from attr in Attributes.Values
                                     where (attr.Prefix == "xml" && attr.LocalName == "space")
                                     select attr).FirstOrDefault ();
      if (spaceAttribute!=null)
        base.WriteAttributeString (spaceAttribute.Prefix, spaceAttribute.LocalName, 
          spaceAttribute.NamespaceURI, spaceAttribute.Value);
      Attributes.Clear ();
      inFlush = false;
    }

  }
}
