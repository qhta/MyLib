using MS.Internal.WindowsBase;
using System;
using System.Globalization;

namespace System.Drawing
{

  public class TokenizerHelper
  {
    private char _quoteChar;
    private char _argSeparator;
    private string _str;
    private int _strLen;
    private int _charIndex;
    internal int _currentTokenIndex;
    internal int _currentTokenLength;
    private bool _foundSeparator;

    internal TokenizerHelper(string str, IFormatProvider formatProvider)
    {
      char numericListSeparator = GetNumericListSeparator(formatProvider);
      this.Initialize(str, '\'', numericListSeparator);
    }

    internal TokenizerHelper(string str, char quoteChar, char separator)
    {
      this.Initialize(str, quoteChar, separator);
    }

    internal string GetCurrentToken()
    {
      return ((this._currentTokenIndex >= 0) ? this._str.Substring(this._currentTokenIndex, this._currentTokenLength) : null);
    }

    internal static char GetNumericListSeparator(IFormatProvider provider)
    {
      char ch = ',';
      NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
      if ((instance.NumberDecimalSeparator.Length > 0) && (ch == instance.NumberDecimalSeparator[0]))
      {
        ch = ';';
      }
      return ch;
    }

    private void Initialize(string str, char quoteChar, char separator)
    {
      this._str = str;
      this._strLen = (str == null) ? 0 : str.Length;
      this._currentTokenIndex = -1;
      this._quoteChar = quoteChar;
      this._argSeparator = separator;
      while ((this._charIndex < this._strLen) && char.IsWhiteSpace(this._str, this._charIndex))
      {
        this._charIndex++;
      }
    }

    internal void LastTokenRequired()
    {
      if (this._charIndex != this._strLen)
      {
        object[] args = new object[] { this._charIndex, this._str };
        throw new InvalidOperationException(SR.Get("TokenizerHelperExtraDataEncountered", args));
      }
    }

    internal bool NextToken()
    {
      return this.NextToken(false);
    }

    internal bool NextToken(bool allowQuotedToken)
    {
      return this.NextToken(allowQuotedToken, this._argSeparator);
    }

    internal bool NextToken(bool allowQuotedToken, char separator)
    {
      this._currentTokenIndex = -1;
      this._foundSeparator = false;
      if (this._charIndex >= this._strLen)
      {
        return false;
      }
      char c = this._str[this._charIndex];
      int num = 0;
      if (allowQuotedToken && (c == this._quoteChar))
      {
        num++;
        this._charIndex++;
      }
      int num2 = this._charIndex;
      int num3 = 0;
      while (this._charIndex < this._strLen)
      {
        c = this._str[this._charIndex];
        if (num <= 0)
        {
          if (char.IsWhiteSpace(c) || (c == separator))
          {
            if (c == separator)
            {
              this._foundSeparator = true;
            }
            break;
          }
        }
        else if (c == this._quoteChar)
        {
          num--;
          if (num == 0)
          {
            this._charIndex++;
            break;
          }
        }
        this._charIndex++;
        num3++;
      }
      if (num > 0)
      {
        object[] objArray1 = new object[] { this._str };
        throw new InvalidOperationException(SR.Get("TokenizerHelperMissingEndQuote", objArray1));
      }
      this.ScanToNextToken(separator);
      this._currentTokenIndex = num2;
      this._currentTokenLength = num3;
      if (this._currentTokenLength >= 1)
      {
        return true;
      }
      object[] args = new object[] { this._charIndex, this._str };
      throw new InvalidOperationException(SR.Get("TokenizerHelperEmptyToken", args));
    }

    internal string NextTokenRequired()
    {
      if (this.NextToken(false))
      {
        return this.GetCurrentToken();
      }
      object[] args = new object[] { this._str };
      throw new InvalidOperationException(SR.Get("TokenizerHelperPrematureStringTermination", args));
    }

    internal string NextTokenRequired(bool allowQuotedToken)
    {
      if (this.NextToken(allowQuotedToken))
      {
        return this.GetCurrentToken();
      }
      object[] args = new object[] { this._str };
      throw new InvalidOperationException(SR.Get("TokenizerHelperPrematureStringTermination", args));
    }

    private void ScanToNextToken(char separator)
    {
      if (this._charIndex < this._strLen)
      {
        char c = this._str[this._charIndex];
        if ((c != separator) && !char.IsWhiteSpace(c))
        {
          object[] args = new object[] { this._charIndex, this._str };
          throw new InvalidOperationException(SR.Get("TokenizerHelperExtraDataEncountered", args));
        }
        int num = 0;
        while (true)
        {
          if (this._charIndex < this._strLen)
          {
            c = this._str[this._charIndex];
            if (c == separator)
            {
              this._foundSeparator = true;
              num++;
              this._charIndex++;
              if (num > 1)
              {
                object[] objArray2 = new object[] { this._charIndex, this._str };
                throw new InvalidOperationException(SR.Get("TokenizerHelperEmptyToken", objArray2));
              }
              continue;
            }
            if (char.IsWhiteSpace(c))
            {
              this._charIndex++;
              continue;
            }
          }
          if ((num <= 0) || (this._charIndex < this._strLen))
          {
            break;
          }
          object[] args = new object[] { this._charIndex, this._str };
          throw new InvalidOperationException(SR.Get("TokenizerHelperEmptyToken", args));
        }
      }
    }

    internal bool FoundSeparator
    {
      get
      {
        return this._foundSeparator;
      }
    }
  }
}
