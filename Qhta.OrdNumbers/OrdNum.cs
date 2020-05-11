using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Qhta.OrdNumbers
{

  //============================================================================
  /// <class refID="ORDNUM_001"/>
  /// <summary>
  ///   Klasa reprezentująca liczbę porządkową składającą się z segmentów.
  ///   Segmenty są liczbami całkowitymi z zakresu od 0 do 16777215 oddzielonymi kropkami.
  ///   Każdy segment może mieć wariant oznaczany literą od "a" do "z".
  ///   Wariant "a" ma wartość 1, "z" ma wartość 26.
  ///   Jeśli segment ma więcej wariantów niż 26, to kolejne warianty są oznaczane jako
  ///   "aa", "ab", "ac".."iu" (maksymalna wartość wariantu to 255).
  /// </summary>
  [TypeConverter(typeof(OrdNumTypeConverter))]
  public sealed class OrdNum : IEnumerable<OrdNumSegment>, IComparable, ICollection<OrdNumSegment>
  {

    #region komunikaty błędów

    internal const string sDigitNotAllowedAfterLetter = "Digit not allowed after letter in ordinal number \"{0}\"";
    internal const string sInvalidCharInOrdNumString = "Invalid char '{0:c}' in ordinal number \"{1}\"";
    internal const string sInvalidOrdNumString = "Invalid ordinal number \"{0}\" string";
    internal const string sOrdNumVariantTooHigh = "OrdNum Variant string value \"{0}\" too high";
    internal const string sTwoLettersShouldBeSeparated =
                                 "Two letters in ordinal number \"{0}\" should be separated by decimal number";
    internal const string sCantConvertToIntValue = "Can't convert ord number \"{0}\" to integer value";
    internal const string sCantConvertToRealValue = "Can't convert ord number \"{0}\" to real value";
    internal const string sCantConvertToDecimalValue = "Can't convert ord number \"{0}\" to decimal value";
    internal const string sCantDecrementEmptyOrdNum = "Can't decrement empty ord number";

    #endregion

    /// <summary>Wewnętrzna tablica segmentów</summary>
    private OrdNumSegment[] fValues = new OrdNumSegment[0];

    #region konstruktory

    /// <summary>Konstruktor domyślny</summary>
    public OrdNum() { }

    /// <summary>Konstruktor kopiujący</summary>
    public OrdNum(OrdNum a)
    {
      fValues = new OrdNumSegment[a.Count];
      for (int i = 0; i<a.Count; i++)
        fValues[i] = new OrdNumSegment(a[i]);

    }

    /// <summary>Konstruktor z wartością całkowitą (0..255)</summary>
    public OrdNum(int value) { SetAsInt(value); }

    /// <summary>Konstruktor z wartością rzeczywistą (0..255).(0..255)</summary>
    public OrdNum(double value) { SetAsReal(value); }

    /// <summary>Konstruktor z wartością dziesiętną (0..255).(0..255)</summary>
    public OrdNum(decimal value) { FromDecimal(value); }

    /// <summary>Konstruktor z wartością tekstową</summary>
    public OrdNum(string value) { SetAsString(value); }

    #endregion

    #region konwersja liczby porządkowe - liczby całkowite

    /// <summary>Czy ma wartość całkowitą?</summary>
    public bool HasIntValue() { return (fValues.Length == 1) && (fValues[0].Variant == 0); }

    /// <summary>Wartość całkowita liczby porządkowej</summary>
    public int AsInt { get { return GetAsInt(); } set { SetAsInt(value); } }

    /// <summary>Wpisuje wartość całkowitą do liczby porządkowej</summary>
    public void SetAsInt(int value)
    {
      fValues = new OrdNumSegment[1];
      fValues[0] = new OrdNumSegment(value);
    }

    /// <summary>Podaje wartość całkowitą liczby porządkowej</summary>
    public int GetAsInt()
    {
      if ((fValues.Length == 1) && (fValues[0].Variant == 0))
        return (fValues[0].Value);
      else
        throw new InvalidOperationException(String.Format(sCantConvertToIntValue, ToString()));
    }

    #endregion

    #region konwersja liczby porządkowe - liczby rzeczywiste

    /// <summary>Czy ma wartość rzeczywistą?</summary>
    public bool HasRealValue()
    {
      if (fValues.Length > 0)
      {
        if (fValues[0].Variant != 0)
          return false;
        if (fValues.Length > 2)
          return false;
        if (fValues[0].Variant != 0)
          return false;
        if (fValues.Length == 2)
          if (fValues[1].Variant != 0)
            return false;
        return true;
      }
      else
        return false;
    }

    /// <summary>Wartość rzeczywista liczby porządkowej</summary>
    public double AsReal { get { return GetAsReal(); } set { SetAsReal(value); } }

    /// <summary>Wpisuje wartość rzeczywistą do liczby porządkowej</summary>
    public void SetAsReal(double value)
    {
      fValues = new OrdNumSegment[2];
      fValues[0] = new OrdNumSegment((int)value);
      value = value - (int)value;
      string s = String.Format(CultureInfo.InvariantCulture, "{0:f8}", value);
      int n = s.IndexOf('.');
      s = s.Substring(n + 1, 8);
      for (int i = s.Length - 1; i > 0; i--)
        if (s[i] == '0')
          s = s.Remove(i);
        else
          break;
      n = Int32.Parse(s);
      fValues[1] = new OrdNumSegment(n);
    }

    /// <summary>Podaje wartość rzeczywistą liczby porządkowej</summary>
    public double GetAsReal()
    {
      if (fValues.Length > 0)
      {
        double result = fValues[0].Value;
        for (int i = 1; i < fValues.Length; i++)
        {
          result += fValues[i].Value / Math.Pow(100, i);
        }
        return result;
      }
      else
        throw new InvalidOperationException(String.Format(sCantConvertToRealValue, ToString()));
    }
    #endregion

    #region konwersja liczby porządkowe - liczby dziesiętne

    /// <summary>Czy ma wartość dziesiętną?</summary>
    public bool HasDecimalValue()
    {
      return HasRealValue();
    }

    /// <summary>Wartość dziesiętna liczby porządkowej</summary>
    public decimal DecimalValue { get { return ToDecimal(); } set { FromDecimal(value); } }

    /// <summary>Wpisuje wartość dziesiętną do liczby porządkowej</summary>
    public void FromDecimal(decimal value)
    {
      fValues = new OrdNumSegment[2];
      fValues[0]=(new OrdNumSegment((int)value));
      value = value - (int)value;
      string s = String.Format(CultureInfo.InvariantCulture, "{0:f28}", value);
      int n = s.IndexOf('.');
      s = s.Substring(n + 1, 9);
      for (int i = s.Length - 1; i > 0; i--)
        if (s[i] == '0')
          s = s.Remove(i);
        else
          break;
      n = Int32.Parse(s);
      fValues[1] = (new OrdNumSegment(n));
    }

    /// <summary>Podaje wartość dziesiętną liczby porządkowej</summary>
    public decimal ToDecimal()
    {
      decimal result;
      if (fValues.Length > 0)
      {
        if (fValues[0].Variant != 0)
          throw new InvalidOperationException(String.Format(sCantConvertToDecimalValue, ToString()));
        result = fValues[0].Value;
        if (fValues.Length > 2)
          throw new InvalidOperationException(String.Format(sCantConvertToDecimalValue, ToString()));
        if (fValues.Length == 2)
        {
          if (fValues[1].Variant != 0)
            throw new InvalidOperationException(String.Format(sCantConvertToDecimalValue, ToString()));
          string s = fValues[0].Value.ToString() + '.' + fValues[1].Value;
          result = Decimal.Parse(s, CultureInfo.InvariantCulture);
        }
        return result;
      }
      else
        throw new InvalidOperationException(String.Format(sCantConvertToDecimalValue, ToString()));
    }
    #endregion

    #region konwersja liczby porządkowe - tekst

    /// <summary>Czy tekst jest poprawnie sformatowaną liczbą porządkową?</summary>
    public static bool IsValid(string value)
    {
      if (String.IsNullOrEmpty(value))
        return false;
      string[] ss = value.Split('.');
      for (int i = 0; i < ss.Length; i++)
      {
        string s = ss[i];
        for (int j = 0; j<s.Length; j++)
        {
          char c = s[j];
          if ((c >= '0') && (c <= '9'))
          {
            // ok
          }
          else if ((c >= 'A') && (c <= 'Z') || (c >= 'a') && (c<='z'))
          {
            if (j < s.Length - 1)
              return false;
          }
          else
            return false;
        }
      }

      return true;
    }


    /// <summary>Wartość tekstowa liczby porządkowej</summary>
    public string StrValue { get { return ToString(); } set { SetAsString(value); } }

    /// <summary>Wpisuje wartość tekstową do liczby porządkowej</summary>
    public void SetAsString(string value)
    {
      fValues = new OrdNumSegment[0];
      if (String.IsNullOrEmpty(value))
        return;
      string[] ss = value.Split('.');
      fValues = new OrdNumSegment[ss.Length];
      for (int i = 0; i < ss.Length; i++)
      {
        string s = ss[i];
        string sn = "";
        string sa = "";
        int mode = 0;
        foreach (char c in s)
        {
          if ((c >= '0') && (c <= '9'))
          {
            if (mode < 0) // was letter
              throw new InvalidOperationException(String.Format(sDigitNotAllowedAfterLetter, value));
            mode = 1; // switch to digit mode
            sn += c;
          }
          else
            if ((c >= 'A') && (c <= 'Z') || (c >= 'a') && (c <= 'z'))
          {
            mode = -1; // switch to letter mode
            sa += c;
          }
          else
            throw new InvalidOperationException(String.Format(sInvalidCharInOrdNumString, c, value));
        }
        if (sn != "")
        {
          int n = Int32.Parse(sn);
          if (sa == "")
            sa = null;
          fValues[i] = (new OrdNumSegment(n, sa));
        }
        else
          throw new InvalidOperationException(String.Format(sInvalidOrdNumString, value));
      }
    }

    /// <summary>Podaje wartość tekstową liczby porządkowej</summary>
    public override string ToString()
    {
      string result = "";
      foreach (OrdNumSegment seg in fValues)
      {
        if (result != "")
          result += '.';
        result += seg.Value + seg.VariantStr;
      }
      return result;
    }
    #endregion

    #region niejawne operatory konwersji
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    /// <summary>Niejawna konwersja na liczbę całkowitą</summary>
    public static implicit operator int(OrdNum a)
    {
      if (IsNullOrEmpty(a))
        return 0;
      return a.AsInt;
    }
    /// <summary>Niejawna konwersja na liczbę rzeczywistą</summary>
    public static implicit operator double(OrdNum a)
    {
      if (IsNullOrEmpty(a))
        return 0;
      return a.AsReal;
    }
    /// <summary>Niejawna konwersja na liczbę dziesiętną</summary>
    public static implicit operator decimal(OrdNum a)
    {
      if (IsNullOrEmpty(a))
        return 0;
      return a.DecimalValue;
    }
    /// <summary>Niejawna konwersja na tekst</summary>
    public static implicit operator string(OrdNum a)
    {
      if (IsNullOrEmpty(a))
        return null;
      return a.StrValue;
    }

    /// <summary>Niejawna konwersja z liczby całkowitej</summary>
    public static implicit operator OrdNum(int n) { return new OrdNum(n); }
    /// <summary>Niejawna konwersja z liczby rzeczywistej</summary>
    public static implicit operator OrdNum(double x) { return new OrdNum(x); }
    /// <summary>Niejawna konwersja z liczby dziesiętnej</summary>
    public static implicit operator OrdNum(decimal d) { return new OrdNum(d); }
    /// <summary>Niejawna konwersja z tekstu</summary>
    public static implicit operator OrdNum(string s)
    { if (!String.IsNullOrEmpty(s)) return new OrdNum(s); else return null; }
    #endregion

    #region operatory porównania i nierówności
    /// <summary>Porównanie dwóch liczb porządkowych</summary>
    public static bool operator ==(OrdNum a, OrdNum b) { return a?.CompareTo(b)==0; }
    /// <summary>Operator nierówności dwóch liczb porządkowych</summary>
    public static bool operator !=(OrdNum a, OrdNum b) { return a?.CompareTo(b)!=0; }
    /// <summary>Operator większości dwóch liczb porządkowych</summary>
    public static bool operator >(OrdNum a, OrdNum b) { return a?.CompareTo(b)>0; }
    /// <summary>Operator większości lub równości dwóch liczb porządkowych</summary>
    public static bool operator >=(OrdNum a, OrdNum b) { return a?.CompareTo(b) >= 0; }
    /// <summary>Operator większości dwóch liczb porządkowych</summary>
    public static bool operator <(OrdNum a, OrdNum b) { return a?.CompareTo(b) < 0; }
    /// <summary>Operator większości lub równości dwóch liczb porządkowych</summary>
    public static bool operator <=(OrdNum a, OrdNum b) { return a?.CompareTo(b) <= 0; }

    /// <summary>Porównanie z liczbą całkowitą</summary>
    public static bool operator ==(OrdNum a, int n) { return a?.Equals(n)==true; }
    /// <summary>Operator nierówności liczby porządkowej z liczbą całkowitą</summary>
    public static bool operator !=(OrdNum a, int n) { return a?.Equals(n)==false; }
    /// <summary>Porównanie z liczbą rzeczywistą</summary>
    public static bool operator ==(OrdNum a, double x) { return a?.Equals(x)==true; }
    /// <summary>Operator nierówności liczby porządkowej z liczbą rzeczywistą</summary>
    public static bool operator !=(OrdNum a, double x) { return a?.Equals(x)==false; }
    /// <summary>Porównanie z liczbą dziesiętną</summary>
    public static bool operator ==(OrdNum a, decimal d) { return a?.Equals(d)==true; }
    /// <summary>Operator nierówności liczby porządkowej z liczbą dziesiętną</summary>
    public static bool operator !=(OrdNum a, decimal d) { return a?.Equals(d)==false; }
    /// <summary>Porównanie z wartością tekstową</summary>
    public static bool operator ==(OrdNum a, string s)
    {
      if (IsNullOrEmpty(a))
        return (String.IsNullOrEmpty(s));
      return a.Equals(s);
    }
    /// <summary>Operator nierówności liczby porządkowej z wartością tekstową</summary>
    public static bool operator !=(OrdNum a, string s)
    {
      if (IsNullOrEmpty(a))
        return !(String.IsNullOrEmpty(s));
      return !a.Equals(s);
    }
    #endregion

    #region inne procedury i operatory

    /// <summary>
    /// Porównanie dwóch łańcuchów traktowanych jako liczby porządkowe.
    /// </summary>
    /// <remarks>
    /// Łańcuch pusty jest mniejszy od jakiegokolwiek innego, z wyjątkiem innego pustego.
    /// </remarks>
    /// <returns>-1 gdy pierwszy jest mniejszy od drugiego, 0 gdy równy, +1 gdy większy</returns>
    public static int Compare(string s1, string s2)
    {
      if (String.IsNullOrEmpty(s1))
      {
        if (String.IsNullOrEmpty(s2))
          return 0;
        else
          return -1;
      }
      else if (String.IsNullOrEmpty(s2))
      {
        if (String.IsNullOrEmpty(s1))
          return 0;
        else
          return +1;
      }
      else
        return Compare(new OrdNum(s1), new OrdNum(s2));
    }

    /// <summary>
    /// Porównanie dwóch liczb porządkowych.
    /// </summary>
    /// <returns>-1 gdy pierwsza jest mniejsza od drugiej, 0 gdy równa, +1 gdy większa</returns>
    public static int Compare(OrdNum v1, OrdNum v2, bool strict = true)
    {
      int n1 = v1.Count;
      int n2 = v2.Count;
      for (int i = 0; i < n1 && i < n2; i++)
      {
        if (v1[i].Value > v2[i].Value)
          return +1;
        else if (v1[i].Value < v2[i].Value)
          return -1;
        else if (strict)
        {
          if (v1[i].Variant > v2[i].Variant)
            return +1;
          else if (v1[i].Variant < v2[i].Variant)
            return -1;
        }
        else if (v1[i].Variant != 0 || v2[i].Variant != 0)
          return 0;
      }
      if (n1 > n2)
        return +1;
      else if (n1 < n2)
        return -1;
      else
        return 0;
    }

    /// <summary>Porównanie wartości porządkowej z innym obiektem</summary>
    /// <param name="o">wartość porównywana</param>
    /// <returns>-1 gdy dana wartość jest mniejsza od tej drugiej, 0 gdy równa, +1 gdy większa</returns>
    public int CompareTo(object o)
    {
      if (o == null)
        return 1;
      if (o is OrdNum)
        return Compare(this, (OrdNum)o);
      else if (o is int)
        return Compare(this, new OrdNum((int)o));
      else if (o is double)
        return Compare(this, new OrdNum((double)o));
      if (o is Decimal)
        return Compare(this, new OrdNum((decimal)o));
      if (o is String)
        return Compare(this, new OrdNum((string)o));
      else
        throw new InvalidOperationException("Can't compare OrdNum to "+o.GetType().Name);
    }

    /// <summary>Porównanie wartości porządkowej z inną wartością porządkową</summary>
    /// <param name="o">wartość porównywana</param>
    /// <returns>-1 gdy dana wartość jest mniejsza od tej drugiej, 0 gdy równa, +1 gdy większa</returns>
    public int CompareTo(OrdNum o)
    {
      return Compare(this, o);
    }

    /// <summary>Porównanie wartości porządkowej z dowolną inną wartością</summary>
    /// <param name="o">wartość porównywana (obiekt)</param>
    public override bool Equals(Object o)
    {
      if (o == null)
        return (object)this == null;
      if (o is OrdNum)
        return Compare(this, (OrdNum)o) == 0;
      else if (o is int)
        return ToString() == ((int)o).ToString();
      else if (o is double)
        return HasRealValue() && (GetAsReal() == (double)o);
      if (o is Decimal)
        return HasDecimalValue() && (ToDecimal() == (decimal)o);
      if (o is String)
        return ToString() == (string)o;
      else
        return base.Equals(o);
    }

    /// <summary>
    /// Sprawdzenie, czy wartość porządkowa jest pusta.
    /// </summary>
    public static bool IsNullOrEmpty(OrdNum value)
    {
      return (value as object)== null || value.Count == 0;
    }

    /// <summary>
    /// Sprawdzenie, czy wartość porządkowa rozpoczyna się od innej wartości
    /// </summary>
    /// <param name="v2"></param>
    /// <param name="strict">czy brać pod uwagę ostatni wariant</param>
    /// <returns></returns>
    public bool StartsWith(OrdNum v2, bool strict=true)
    {
      int k = this.Count;
      int n = v2.Count;
      if (k<n)
        return false;
      for (int i = 0; i<n; i++)
        if (!this[i].Equals(v2[i],strict && i==n-1))
          return false;
      return true;
    }

    /// <summary>
    /// Podaje liczbę porządkową utworzoną z pierwszych n segmentów danej liczby
    /// </summary>
    /// <param name="n">liczba segmentów do podania; gdy przekracza aktualną, to podaje bez zmian</param>
    /// <returns>nowoutworzona liczba porządkowa</returns>
    public OrdNum FirstSegments(int n)
    {
      int k = this.Count;
      if (k<n)
        return new OrdNum(this);
      OrdNum result = new OrdNum();
      result.fValues = new OrdNumSegment[n];
      for (int i = 0; i<n; i++)
        result.fValues[i] = new OrdNumSegment(this[i]);
      return result;
    }

    /// <summary>
    /// Podaje liczbę porządkową utworzoną z ostatnich n segmentów danej liczby
    /// </summary>
    /// <param name="n">liczba segmentów do podania; gdy przekracza aktualną, to kopiuje bez zmian</param>
    /// <returns>nowoutworzona liczba porządkowa</returns>
    public OrdNum LastSegments(int n)
    {
      int k = this.Count-n;
      if (k<0)
        return new OrdNum(this);
      OrdNum result = new OrdNum();
      result.fValues = new OrdNumSegment[n];
      for (int i = 0; i<n; i++)
        result.fValues[i] = new OrdNumSegment(this[k+i]);
      return result;
    }

    /// <summary>
    /// Podaje liczbę porządkową utworzoną z ostatnich n segmentów danej liczby począwszy od segmentu k
    /// </summary>
    /// <param name="n">liczba segmentów do podania; gdy k+n przekracza aktualną liczbę segmentów, to podaje do końca</param>
    /// <returns>nowoutworzona liczba porządkowa</returns>
    public OrdNum MidSegments(int k, int n=int.MaxValue)
    {
      if (k+n>this.Count)
        n = this.Count-k;
      OrdNum result = new OrdNum();
      result.fValues = new OrdNumSegment[n];
      for (int i = 0; i<n; i++)
        result.fValues[i] = new OrdNumSegment(this[k+i]);
      return result;
    }

    /// <summary>Dopisanie liczby całkowitej do liczby porządkowej. Dodawany jest nowy segment</summary>
    public static OrdNum operator |(OrdNum a, int n)
    {
      OrdNum result = new OrdNum();
      result.fValues = new OrdNumSegment[a.Count+1];
      a.fValues.CopyTo(result.fValues, 0);
      result.fValues[a.Count] = new OrdNumSegment(n);
      return result;
    }

    /// <summary>Dopisanie wartości tekstowej do liczby porządkowej. Dodawany jest nowy segment</summary>
    public static OrdNum operator |(OrdNum a, string s)
    {
      OrdNum result = new OrdNum();
      result.fValues = new OrdNumSegment[a.Count + 1];
      a.fValues.CopyTo(result.fValues, 0);
      result.fValues[a.Count] = new OrdNumSegment(s);
      return result;
    }

    /// <summary>Ustawia wariant na ostatnim segmencie liczby porządkowej</summary>
    /// <param name="a">wejściowa liczba porządkowa</param>
    /// <param name="n">wartość wariantu 1='a', 2='b' itd.</param>
    public static OrdNum operator &(OrdNum a, byte n)
    {
      OrdNum result;
      if (a.Count > 0)
        result = new OrdNum(a);
      else
        result = new OrdNum(0);
      result.fValues[result.Count - 1].Variant = n;
      return result;
    }

    /// <summary>Ustawia wariant na ostatnim segmencie liczby porządkowej</summary>
    /// <param name="a">wejściowa liczba porządkowa</param>
    /// <param name="s">wartość tekstowa wariantu "a"=1, "b"=2 itd.</param>
    public static OrdNum operator &(OrdNum a, string s)
    {
      byte n = OrdNumSegment.StrToVariant(s);
      OrdNum result;
      if (a.Count > 0)
        result = new OrdNum(a);
      else
        result = new OrdNum(0);
      result.fValues[result.Count - 1].Variant = n;
      return result;
    }

    /// <summary>
    ///   Zwiększa wartość ostatniego segmentu o 1.
    /// </summary>
    public static OrdNum operator ++(OrdNum a)
    {
      if (a.Count > 0)
      {
        OrdNum result = new OrdNum(a);
        result.fValues[result.Count-1].Value+=1;
        return result;
      }
      else
        return new OrdNum(1);
    }

    /// <summary>
    ///   Zwiększa wartość ostatniego segmentu o n.
    /// </summary>
    public static OrdNum operator +(OrdNum a, int n)
    {
      if (a.Count > 0)
      {
        OrdNum result = new OrdNum(a);
        result.fValues[result.Count - 1].Value += n;
        return result;
      }
      else
        return new OrdNum(1);
    }

    /// <summary>
    ///   Zmniejsza wartość ostatniego segmentu o 1.
    /// </summary>
    public static OrdNum operator --(OrdNum a)
    {
      if (a.Count > 0)
      {
        OrdNum result = new OrdNum(a);
        result.fValues[result.Count - 1].Value-=1;
        return result;
      }
      else
        throw new InvalidOperationException(sCantDecrementEmptyOrdNum);
    }

    /// <summary>
    ///   Zmniejsza wartość ostatniego segmentu o n.
    /// </summary>
    public static OrdNum operator -(OrdNum a, int n)
    {
      if (a.Count > 0)
      {
        OrdNum result = new OrdNum(a);
        result.fValues[result.Count - 1].Value -= n;
        return result;
      }
      else
        throw new InvalidOperationException(sCantDecrementEmptyOrdNum);
    }

    /// <summary>
    ///   Zwiększa wariant ostatniego segmentu o n.
    /// </summary>
    public static OrdNum operator *(OrdNum a, byte n)
    {
      OrdNum result;
      if (a.Count > 0)
        result = new OrdNum(a);
      else
        result = new OrdNum(0);
      result.fValues[result.Count - 1].Variant += n;
      return result;
    }

    /// <summary>
    ///   Zmniejsza wariant ostatniego segmentu o n.
    /// </summary>
    public static OrdNum operator /(OrdNum a, byte n)
    {
      OrdNum result;
      if (a.Count > 0)
        result = new OrdNum(a);
      else
        throw new InvalidOperationException(sCantDecrementEmptyOrdNum);
      result.fValues[result.Count - 1].Variant -= n;
      return result;
    }

    /// <summary>Funkcja mieszania</summary>
    public override int GetHashCode() { return ToString().GetHashCode(); }

    IEnumerator IEnumerable.GetEnumerator() { return fValues.GetEnumerator(); }
    IEnumerator<OrdNumSegment> IEnumerable<OrdNumSegment>.GetEnumerator() { return fValues.GetEnumerator() as IEnumerator<OrdNumSegment>; }

    public void Add(OrdNumSegment item)
    {
      ((ICollection<OrdNumSegment>)fValues).Add(item);
    }

    public void Clear()
    {
      ((ICollection<OrdNumSegment>)fValues).Clear();
    }

    public bool Contains(OrdNumSegment item)
    {
      return ((ICollection<OrdNumSegment>)fValues).Contains(item);
    }

    public void CopyTo(OrdNumSegment[] array, int arrayIndex)
    {
      ((ICollection<OrdNumSegment>)fValues).CopyTo(array, arrayIndex);
    }

    public bool Remove(OrdNumSegment item)
    {
      return ((ICollection<OrdNumSegment>)fValues).Remove(item);
    }

    /// <summary>Liczba segmentów</summary>
    public int Count { get { return fValues.Length; } }

    /// <summary>Długość liczona w wartościach i wariantach</summary>
    public int Length
    {
      get
      {
        int result = fValues.Length;
        foreach (OrdNumSegment aSegment in fValues)
          if (aSegment.Variant > 0)
            result++;
        return result;
      }
    }

    public bool IsReadOnly => ((ICollection<OrdNumSegment>)fValues).IsReadOnly;
    public OrdNum Parent
    {
      get
      {
        var s = this.ToString();
        int n = s.Length;
        while (n>0 && Char.IsLetter(s[n-1]))
        {
          n--;
        }
        if (n<s.Length)
          return s.Substring(0, n);
        n = s.LastIndexOf('.');
        if (n>0)
          return s.Substring(0, n);
        return new OrdNum();
      }
    }

    /*
/// <summary>Czy dana liczba zaczyna się od innej liczby</summary>
public bool StartsWith(OrdNum a)
{
for (int i = 0; i <= fValue.Length - 1; i++)
{
if (i >= a.fValue.Length)
return true;
OrdNumSegment thisSeg = fValue[i];
OrdNumSegment otherSeg = a.fValue[i];
if (thisSeg.Value != otherSeg.Value)
return false;
if (i == fValue.Length - 1)
{
if (thisSeg.Variant == 0)
{
if (otherSeg.Variant != 0)
return false;
}
else
if (thisSeg.Variant != otherSeg.Variant)
return false;
}
else
{
if (thisSeg.Variant != otherSeg.Variant)
return false;
}
}
return true;
}
*/
    /// <summary>Dostęp do pojedynczego segmentu</summary>
    public OrdNumSegment this[int n] { get { return fValues[n]; } }
    #endregion
  }

  //======================================================================
  /// <class refID="ORDNUM_002"/>
  /// <summary>
  ///   Pojedynczy segment liczby porządkowej
  /// </summary>
  public struct OrdNumSegment: IComparable
  {
    public static int MaxValue = 0xFFFFFF;
    static int Bias = 24;

    /// <summary>
    /// Wartość całkowita segmentu
    /// </summary>
    private int fValue;

    /// <summary>Konstruktor kopiujący</summary>
    public OrdNumSegment(OrdNumSegment a) { fValue = a.fValue; }

    /// <summary>Konstruktor z wartością całkowitą</summary>
    public OrdNumSegment(int value) { fValue = value & MaxValue; }

    /// <summary>Konstruktor z wartością tekstową</summary>
    public OrdNumSegment(string s)
    {
      int n = 0;
      for (int i = 0; i<s.Length; i++)
      {
        char c = s[i];
        if ((c >= '0') && (c <= '9'))
          n = n * 1 + (c - '0');
        else
        {
          s=s.Substring(i, s.Length-i);
          break;
        }
      }
      fValue = n & MaxValue | (StrToVariant(s) << Bias);
    }

    /// <summary>Konstruktor z wartością całkowitą i wariantem w postaci liczby całkowitej</summary>
    public OrdNumSegment(int value, byte variant)
    {
      fValue = value & MaxValue | (variant << Bias);
    }

    /// <summary>Konstruktor z wartością całkowitą i wariantem w postaci tekstu: "a"=1.</summary>
    public OrdNumSegment(int value, string variant)
    {
      fValue = value & MaxValue | (StrToVariant(variant) << Bias);
    }

    /// <summary>Wartość całkowita segmentu</summary>
    public int Value { get { return fValue & MaxValue; } set { fValue = value & MaxValue | (fValue & ~MaxValue); } }

    /// <summary>Wartość całkowita wariantu (0 - gdy brak)</summary>
    public byte Variant { get { return (byte)(fValue >> Bias); } set { fValue = (value << Bias) | (fValue & MaxValue); } }

    /// <summary>Wartość tekstowa wariantu (<c>null</c> - gdy brak)</summary>
    public string VariantStr { get { return VariantToStr(Variant); } set { Variant = StrToVariant(value); } }

    /// <summary>
    ///   Konwersja wariantu na łańcuch. 0 = <c>null</c>, 1 = "a", 26 = "a", 27 = "aa", 28 = "ab" ... 255 = "iu"
    /// </summary>
    public static string VariantToStr(byte variant)
    {
      if (variant == 0)
        return null;
      if (variant >= 1 && variant <= 26)
        return new String(new[] { (char)(variant-1 +'a') });
      byte high = (byte)((variant-1)/26);
      byte low = (byte)((variant-1) % 26);
      return new String(new[] { (char)(high +'a'), (char)(low + 'a') });
    }

    /// <summary>
    ///   Konwersja łańcucha na wariant. <c>null</c> = 0, "a" = 1, "z" = 26, "aa" = 27 ... "iu" = 255
    /// </summary>
    public static byte StrToVariant(string str)
    {
      byte result;
      if (String.IsNullOrEmpty(str))
        result = 0;
      else
      {
        str = str.ToLowerInvariant();
        int val = 0;
        char ch = str[0];
        if (ch>='a' && ch<='z')
          val = ch-'a' + 1;
        else
          throw new InvalidOperationException(string.Format(OrdNum.sInvalidOrdNumString, str));
        if (str.Length>1)
        {
          ch = str[1];
          val = (val-1)*26;
          if (ch>='a' && ch<='z')
            val += ch-'a' + 1;
          else
            throw new InvalidOperationException(string.Format(OrdNum.sInvalidOrdNumString, str));
        }
        if (val>255)
          throw new InvalidOperationException(string.Format(OrdNum.sOrdNumVariantTooHigh, str));
        result = (byte)(val);
      }
      return result;
    }

    /// <summary>
    /// Porównanie dwóch segmentów.
    /// </summary>
    /// <param name="v2">drugi segment do porównania</param>
    /// <param name="strict"> czy brać pod uwagę wariant segmentu</param>
    /// <returns>-1 gdy dany segment jest mniejszy od drugiego, 0 gdy równy, +1 gdy większy</returns>
    public int CompareTo(OrdNumSegment v2, bool strict = true)
    {
      var v1 = this;
      if (v1.Value > v2.Value)
        return +1;
      else if (v1.Value < v2.Value)
        return -1;
      else if (strict)
      {
        if (v1.Variant > v2.Variant)
          return +1;
        else if (v1.Variant < v2.Variant)
          return -1;
      }
      return 0;
    }

    /// <summary>Porównanie segmentu z innym obiektem. Gdy argument jest liczbą całkowitą lub łańcuchem, 
    /// to przed porównaniem dokonuje konwersji na segment</summary>
    /// <param name="o">wartość porównywana</param>
    /// <returns>-1 gdy dany segment jest mniejszy od drugiego, 0 gdy równy, +1 gdy większy</returns>
    public int CompareTo(object o)
    {
      if (o == null)
        return 1;
      if (o is OrdNumSegment)
        return this.CompareTo((OrdNumSegment)o);
      if (o is int)
        return this.CompareTo(new OrdNumSegment((int)o));
      if (o is String)
        return this.CompareTo(new OrdNumSegment((string)o));
      else
        throw new InvalidOperationException("Can't compare OrdNumSegment to "+o.GetType().Name);
    }

    /// <summary>
    /// Porównanie dwóch segmentów.
    /// </summary>
    /// <param name="v2">drugi segment do porównania</param>
    /// <param name="strict"> czy brać pod uwagę wariant segmentu</param>
    /// <returns>true gdy dany segment jest równy drugiemu</returns>
    public bool Equals(OrdNumSegment v2, bool strict=true)
    {
      var cmp = CompareTo(v2, strict);
      return cmp==0;
    }

    #region implementacja interfejsu IComparable
    public override bool Equals(object obj)
    {
      if (!(obj is OrdNumSegment))
      {
        return false;
      }

      var segment = (OrdNumSegment)obj;
      return fValue==segment.fValue;
    }

    public override int GetHashCode()
    {
      return fValue.GetHashCode();
    }
    #endregion

    #region operatory porównania i nierówności
    /// <summary>Porównanie dwóch liczb porządkowych</summary>
    public static bool operator ==(OrdNumSegment a, OrdNumSegment b) { return a.CompareTo(b)==0; }
    /// <summary>Operator nierówności dwóch liczb porządkowych</summary>
    public static bool operator !=(OrdNumSegment a, OrdNumSegment b) { return a.CompareTo(b)!=0; }
    /// <summary>Operator większości dwóch liczb porządkowych</summary>
    public static bool operator >(OrdNumSegment a, OrdNumSegment b) { return a.CompareTo(b)>0; }
    /// <summary>Operator większości lub równości dwóch liczb porządkowych</summary>
    public static bool operator >=(OrdNumSegment a, OrdNumSegment b) { return a.CompareTo(b) >= 0; }
    /// <summary>Operator większości dwóch liczb porządkowych</summary>
    public static bool operator <(OrdNumSegment a, OrdNumSegment b) { return a.CompareTo(b) < 0; }
    /// <summary>Operator większości lub równości dwóch liczb porządkowych</summary>
    public static bool operator <=(OrdNumSegment a, OrdNumSegment b) { return a.CompareTo(b) <= 0; }
    #endregion

    public override string ToString()
    {
      return this.Value + this.VariantStr;
    }
  }

  /// <summary>
  /// Konwerter dla potrzeb serializacji
  /// </summary>
  public class OrdNumTypeConverter : TypeConverter
  {

    /// <summary>
    /// Konwerter może zamienić łańcuch na liczbę porządkową
    /// </summary>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      if (sourceType == typeof(string))
        return true;
      return base.CanConvertFrom(context, sourceType);
    }

    /// <summary>
    /// Konwersja łańcucha na liczbę porządkową
    /// </summary>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if (value is string)
        return new OrdNum((string)value);
      return base.ConvertFrom(context, culture, value);
    }

  }

}
