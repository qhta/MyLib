using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Qhta.Parsing
{
  public class Tokenizer_Base
  {
    public event EventHandler<Token> OnEmitToken;

    protected virtual void EmitToken(Token token)
    {
      OnEmitToken?.Invoke(this, token);
    }

    public void Tokenize(string input)
    {
      int i = 0;
      while (i<input.Length)
      {
        var ch = input[i];
        if (IsWhitespace(ch))
        {
          var whitespaces = GetWhitespaces(input, ref i);
          EmitToken(new Token { Kind=TokenKind.Whitespaces, String=whitespaces });
        }
        else
        if (IsLetter(ch))
        {
          var letters = GetLetters(input, ref i);
          EmitToken(new Token { Kind=TokenKind.Letters, String=letters });
        }
        else
        if (IsDigit(ch))
        {
          var digits = GetDigits(input, ref i);
          EmitToken(new Token { Kind=TokenKind.Digits, String=digits });
        }
        else
        if (IsSymbol(ch))
        {
          EmitToken(new Token { Kind=TokenKind.Symbol, String=new string(ch,1) });
          i++;
        }
        else
        {
          EmitToken(new Token { Kind=TokenKind.Other, String=new string(ch, 1) });
          i++;
        }
      }
    }

    protected virtual bool IsWhitespace(char ch)
    {
      return Char.IsWhiteSpace(ch);
    }

    protected virtual string GetWhitespaces(string input, ref int pos)
    {
      for (int i=pos; i<input.Length; i++)
      {
        if (!IsWhitespace(input[i]))
        {
          if (i>pos)
            return input.Substring(pos, i-pos);
          break;
        }
      }
      return null;
    }

    protected virtual bool IsLetter(char ch)
    {
      return Char.IsLetter(ch);
    }

    protected virtual string GetLetters(string input, ref int pos)
    {
      for (int i = pos; i<input.Length; i++)
      {
        if (!IsLetter(input[i]))
        {
          if (i>pos)
            return input.Substring(pos, i-pos);
          break;
        }
      }
      return null;
    }

    protected virtual bool IsDigit(char ch)
    {
      return Char.IsDigit(ch);
    }

    protected virtual string GetDigits(string input, ref int pos)
    {
      for (int i = pos; i<input.Length; i++)
      {
        if (!IsDigit(input[i]))
        {
          if (i>pos)
            return input.Substring(pos, i-pos);
          break;
        }
      }
      return null;
    }

    protected virtual bool IsSymbol(char ch)
    {
      return Char.IsSymbol(ch) || Char.IsPunctuation(ch);
    }

    protected virtual bool IsOther(char ch)
    {
      return !IsWhitespace(ch) && !IsLetter(ch) && !IsDigit(ch) && !IsSymbol(ch);
    }

  }
  
}
