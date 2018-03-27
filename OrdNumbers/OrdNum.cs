using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace MyLib.OrdNumbers
{

  //============================================================================
  /// <class refID="ORDNUM_001"/>
  /// <summary>
  ///   Klasa reprezentująca liczbę porządkową składającą się z segmentów.
  ///   Segmenty są liczbami całkowitymi z zakresu od 0 do 255 oddzielonymi kropkami.
  ///   Każdy segment może mieć wariant oznaczany literą od a do z.
  /// </summary>
  [TypeConverter(typeof(OrdNumTypeConverter))]
  public sealed class OrdNum : IEnumerable<OrdNumSegment>, IComparable
  {

    #region komunikaty błędów

    private const string sDigitNotAllowedAfterLetter = "Digit not allowed after letter in ordinal number \"{0}\"";
    private const string sInvalidCharInOrdNumString = "Invalid char '{0:c}' in ordinal number \"{1}\"";
    private const string sInvalidOrdNumString = "Invalid ordinal number \"{0}\" string";
    private const string sTwoLettersShouldBeSeparated =
                                 "Two letters in ordinal number \"{0}\" should be separated by decimal number";
    private const string sCantConvertToIntValue = "Can't convert ord number \"{0}\" to integer value";
    private const string sCantConvertToRealValue = "Can't convert ord number \"{0}\" to real value";
    private const string sCantConvertToDecimalValue = "Can't convert ord number \"{0}\" to decimal value";
    private const string sCantDecrementEmptyOrdNum = "Can't decrement empty ord number";

    #endregion

    /// <summary>Wewnętrzna tablica segmentów</summary>
    private OrdNumSegment[] fValue = new OrdNumSegment[0];

    #region konstruktory

    /// <summary>Konstruktor domyślny</summary>
    public OrdNum() { }

    /// <summary>Konstruktor kopiujący</summary>
    public OrdNum(OrdNum a) 
    {
      fValue = new OrdNumSegment[a.Count];
      a.fValue.CopyTo(fValue, 0);
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
    public bool HasIntValue() { return (fValue.Length == 1) && (fValue[0].Variant == 0); }

    /// <summary>Wartość całkowita liczby porządkowej</summary>
    public int AsInt { get { return GetAsInt(); } set { SetAsInt(value); } }

    /// <summary>Wpisuje wartość całkowitą do liczby porządkowej</summary>
    public void SetAsInt(int value)
    {
      fValue = new OrdNumSegment[1];
      fValue[0] = new OrdNumSegment(value);
    }

    /// <summary>Podaje wartość całkowitą liczby porządkowej</summary>
    public int GetAsInt()
    {
      if ((fValue.Length == 1) && (fValue[0].Variant == 0))
        return (fValue[0].Value);
      else
        throw new InvalidOperationException(String.Format(sCantConvertToIntValue, ToString()));
    }

    #endregion

    #region konwersja liczby porządkowe - liczby rzeczywiste

    /// <summary>Czy ma wartość rzeczywistą?</summary>
    public bool HasRealValue()
    {
      if (fValue.Length > 0)
      {
        if (fValue[0].Variant != 0)
          return false;
        if (fValue.Length > 2)
          return false;
        if (fValue[0].Variant != 0)
          return false;
        if (fValue.Length == 2)
          if (fValue[1].Variant != 0)
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
      fValue = new OrdNumSegment[2];
      fValue[0] = new OrdNumSegment((int)value);
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
      fValue[1] = new OrdNumSegment(n);
    }

    /// <summary>Podaje wartość rzeczywistą liczby porządkowej</summary>
    public double GetAsReal()
    {
      if (fValue.Length > 0)
      {
        double result = fValue[0].fValue;
        for (int i = 1; i < fValue.Length; i++)
        {
          result += fValue[i].fValue / Math.Pow(100, i);
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
      fValue = new OrdNumSegment[2];
      fValue[0]=(new OrdNumSegment((int)value));
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
      fValue[1] = (new OrdNumSegment(n));
    }

    /// <summary>Podaje wartość dziesiętną liczby porządkowej</summary>
    public decimal ToDecimal()
    {
      decimal result;
      if (fValue.Length > 0)
      {
        if (fValue[0].Variant != 0)
          throw new InvalidOperationException(String.Format(sCantConvertToDecimalValue, ToString()));
        result = fValue[0].Value;
        if (fValue.Length > 2)
          throw new InvalidOperationException(String.Format(sCantConvertToDecimalValue, ToString()));
        if (fValue.Length == 2)
        {
          if (fValue[1].Variant != 0)
            throw new InvalidOperationException(String.Format(sCantConvertToDecimalValue, ToString()));
          string s = fValue[0].Value.ToString() + '.' + fValue[1].Value;
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
        for (int j=0; j<s.Length; j++)
        {
          char c = s[j];
          if ((c >= '0') && (c <= '9'))
          {
            // ok
          }
          else  if ((c >= 'A') && (c <= 'Z') || (c >= 'a') && (c<='z'))
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
      fValue = new OrdNumSegment[0];
      if (String.IsNullOrEmpty(value))
        return;
      string[] ss = value.Split('.');
      fValue = new OrdNumSegment[ss.Length];
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
          fValue[i] = (new OrdNumSegment(n, sa));
        }
        else
          throw new InvalidOperationException(String.Format(sInvalidOrdNumString, value));
      }
    }

    /// <summary>Podaje wartość tekstową liczby porządkowej</summary>
    public override string ToString()
    {
      string result = "";
      foreach (OrdNumSegment seg in fValue)
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
    public static bool operator ==(OrdNum a, OrdNum b) { return a.CompareTo(b)==0; }
    /// <summary>Operator nierówności dwóch liczb porządkowych</summary>
    public static bool operator !=(OrdNum a, OrdNum b) { return a.CompareTo(b)!=0; }
    /// <summary>Operator większości dwóch liczb porządkowych</summary>
    public static bool operator >(OrdNum a, OrdNum b) { return a.CompareTo(b)>0; }
    /// <summary>Operator większości lub równości dwóch liczb porządkowych</summary>
    public static bool operator >=(OrdNum a, OrdNum b) { return a.CompareTo(b) >= 0; }
    /// <summary>Operator większości dwóch liczb porządkowych</summary>
    public static bool operator <(OrdNum a, OrdNum b) { return a.CompareTo(b) < 0; }
    /// <summary>Operator większości lub równości dwóch liczb porządkowych</summary>
    public static bool operator <=(OrdNum a, OrdNum b) { return a.CompareTo(b) <= 0; }

    /// <summary>Porównanie z liczbą całkowitą</summary>
    public static bool operator ==(OrdNum a, int n) { return a.Equals(n); }
    /// <summary>Operator nierówności liczby porządkowej z liczbą całkowitą</summary>
    public static bool operator !=(OrdNum a, int n) { return !a.Equals(n); }
    /// <summary>Porównanie z liczbą rzeczywistą</summary>
    public static bool operator ==(OrdNum a, double x) { return a.Equals(x); }
    /// <summary>Operator nierówności liczby porządkowej z liczbą rzeczywistą</summary>
    public static bool operator !=(OrdNum a, double x) { return !a.Equals(x); }
    /// <summary>Porównanie z liczbą dziesiętną</summary>
    public static bool operator ==(OrdNum a, decimal d) { return a.Equals(d); }
    /// <summary>Operator nierówności liczby porządkowej z liczbą dziesiętną</summary>
    public static bool operator !=(OrdNum a, decimal d) { return !a.Equals(d); }
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
        else  if (v1[i].Variant != 0 || v2[i].Variant != 0)
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

    /// <summary>Dopisanie liczby całkowitej do liczby porządkowej. Dodawany jest nowy segment</summary>
    public static OrdNum operator |(OrdNum a, int n)
    {
      OrdNum result = new OrdNum();
      result.fValue = new OrdNumSegment[a.Count+1];
      a.fValue.CopyTo(result.fValue, 0);
      result.fValue[a.Count] = new OrdNumSegment(n);
      return result;
    }

    /// <summary>Dopisanie wartości tekstowej do liczby porządkowej. Dodawany jest nowy segment</summary>
    public static OrdNum operator |(OrdNum a, string s)
    {
      OrdNum result = new OrdNum();
      result.fValue = new OrdNumSegment[a.Count + 1];
      a.fValue.CopyTo(result.fValue, 0);
      result.fValue[a.Count] = new OrdNumSegment(s);
      return result;
    }

    /// <summary>Ustawia wariant na ostatnim segmencie liczby porządkowej</summary>
    /// <param name="a">wejściowa liczba porządkowa</param>
    /// <param name="n">wartość wariantu 1='a', 2='b' itd.</param>
    public static OrdNum operator &(OrdNum a, int n)
    {
      OrdNum result;
      if (a.Count > 0)
        result = new OrdNum(a);
      else
        result = new OrdNum(0);
      result.fValue[result.Count - 1].fVariant = (byte)n;
      return result;
    }

    /// <summary>Ustawia wariant na ostatnim segmencie liczby porządkowej</summary>
    /// <param name="a">wejściowa liczba porządkowa</param>
    /// <param name="s">wartość tekstowa wariantu "a"=1, "b"=2 itd.</param>
    public static OrdNum operator &(OrdNum a, string s)
    {
      int n = OrdNumSegment.StrToVariant(s);
      OrdNum result;
      if (a.Count > 0)
        result = new OrdNum(a);
      else
        result = new OrdNum(0);
      result.fValue[result.Count - 1].fVariant = (byte)n;
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
        result.fValue[result.Count-1].fValue++;
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
        result.fValue[result.Count - 1].fValue += (byte)n;
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
        result.fValue[result.Count - 1].fValue--;
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
        result.fValue[result.Count - 1].fValue -= (byte)n;
        return result;
      }
      else
        throw new InvalidOperationException(sCantDecrementEmptyOrdNum);
    }

    /// <summary>
    ///   Zwiększa wariant ostatniego segmentu o n.
    /// </summary>
    public static OrdNum operator *(OrdNum a, int n)
    {
      OrdNum result;
      if (a.Count > 0)
        result = new OrdNum(a);
      else
        result = new OrdNum(0);
      result.fValue[result.Count - 1].fVariant += (byte)n;
      return result;
    }

    /// <summary>
    ///   Zmniejsza wariant ostatniego segmentu o n.
    /// </summary>
    public static OrdNum operator /(OrdNum a, int n)
    {
      OrdNum result;
      if (a.Count > 0)
        result = new OrdNum(a);
      else
        throw new InvalidOperationException(sCantDecrementEmptyOrdNum);
      result.fValue[result.Count - 1].fVariant -= (byte)n;
      return result;
    }

    /// <summary>Funkcja mieszania</summary>
    public override int GetHashCode() { return ToString().GetHashCode(); }

    IEnumerator IEnumerable.GetEnumerator() { return fValue.GetEnumerator(); }
    IEnumerator<OrdNumSegment> IEnumerable<OrdNumSegment>.GetEnumerator() { return fValue.GetEnumerator() as IEnumerator<OrdNumSegment>; }

    /// <summary>Liczba segmentów</summary>
    public int Count { get { return fValue.Length; } }

    /// <summary>Długość liczona w wartościach i wariantach</summary>
    public int Length 
    { 
      get 
      { 
        int result=fValue.Length;
        foreach (OrdNumSegment aSegment in fValue)
          if (aSegment.Variant > 0)
            result++;
        return result;
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
    public OrdNumSegment this[int n] { get { return fValue[n]; } }
    #endregion
  }

  //======================================================================
  /// <class refID="ORDNUM_002"/>
  /// <summary>
  ///   Pojedynczy segment liczby porządkowej
  /// </summary>
  public struct OrdNumSegment
  {
    /// <summary>
    /// Wartość całkowita segmentu
    /// </summary>
    internal byte fValue;

    /// <summary>
    /// Wartość literowa wariantu segmentu.
    /// Dla zaoszczędzenia miejsca pamiętana w formie znaku ASCII
    /// </summary>
    internal byte fVariant;

    /// <summary>Konstruktor kopiujący</summary>
    public OrdNumSegment(OrdNumSegment a) { fValue = a.fValue; fVariant = a.fVariant; }

    /// <summary>Konstruktor z wartością całkowitą</summary>
    public OrdNumSegment(int value) { fValue = (byte)value; fVariant = 0; }

    /// <summary>Konstruktor z wartością tekstową</summary>
    public OrdNumSegment(string s) 
    { 
      int n=0;
      for (int i=0;i<s.Length; i++)
      {
        char c=s[i];
        if ((c >= '0') && (c <= '9'))
          n = n * 1 + (c - '0');
        else
        {
          s=s.Substring(i,s.Length-i);
          break;
        }
      }
      fValue = (byte)n; 
      fVariant = StrToVariant(s); 
    }

    /// <summary>Konstruktor z wartością całkowitą i wariantem w postaci liczby całkowitej</summary>
    public OrdNumSegment(int value, int variant) 
    { 
      fValue = (byte)value; 
      fVariant = (byte)variant; 
    }

    /// <summary>Konstruktor z wartością całkowitą i wariantem w postaci tekstu: "a"=1.</summary>
    public OrdNumSegment(int value, string variant) 
    { 
      fValue = (byte)value;
      fVariant = StrToVariant(variant); 
    }

    /// <summary>Wartość całkowita segmentu</summary>
    public int Value { get { return fValue; } set { fValue = (byte)value; } }

    /// <summary>Wartość całkowita wariantu (0 - gdy brak)</summary>
    public int Variant { get { return fVariant; } set { fVariant = (byte)value; } }

    /// <summary>Wartość tekstowa wariantu (<c>null</c> - gdy brak)</summary>
    public string VariantStr { get { return VariantToStr(fVariant); } set { fVariant = StrToVariant(value); } }

    /// <summary>
    ///   Konwersja wariantu na łańcuch. 0 = <c>null</c>, 1 = "A", 26 = "Z", 27 = "a", 58 = "z", else = "?"
    /// </summary>
    public static string VariantToStr (int variant)
    {
      if (variant == 0)
        return null;
      if (variant >= 1 && variant <= 26)
        return new String(new[]{(char)(variant - 1 +'A')});
      if (variant >= 27 && variant <= 58)
        return new String(new[] { (char)(variant -27 + 'a') });
      return "?";
    }

    /// <summary>
    ///   Konwersja łańcucha na wariant. <c>null</c> = 0, "A" = 1, "Z" = 26, "a" = 27, "z" = 58
    /// </summary>
    public static byte StrToVariant(string value)
    {
      byte result;
      if (String.IsNullOrEmpty(value))
        result = 0;
      else
      {
        int val = 0;
        char c = value[0];
        if (c>='A' && c<='Z')
          val = c - 'A';
        else
        if (c >= 'a' && c <= 'z')
          val = c - 'a'+26;
        else
          throw new InvalidOperationException(string.Format("Invalid OrdNum Variant string \"{0}\"", value));
        result = (byte)(val + 1);
      }
      return result;
    }

  }

  /// <summary>
  /// Konwerter dla potrzeb serializacji
  /// </summary>
  public class OrdNumTypeConverter: TypeConverter
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
