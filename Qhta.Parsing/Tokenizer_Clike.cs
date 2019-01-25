using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Qhta.Parsing
{
  public class Tokenizer_Clike
  {
    enum TokenizerState
    {
      Initial,
      Identifier,
      Number,
      HexNumber,
      FloatNumber,
      Symbol,
      CharLiteral,
      StringLiteral,
      Error,
    }

    Token buffer = null;

    TokenizerState state = TokenizerState.Initial;

    public event EventHandler<Token> OnEmitToken;

    protected virtual void EmitToken(Token token)
    {
      OnEmitToken?.Invoke(this, token);
    }

    public virtual void Flush()
    {
      if (buffer!=null)
      {
        switch (state)
        {
          case TokenizerState.Initial:
            EmitToken(buffer);
            break;
          case TokenizerState.Identifier:
            EmitToken(buffer);
            break;
          case TokenizerState.Number:
            buffer.Kind=TokenKind.Number;
            buffer.Value = ParseNumber(buffer.String);
            EmitToken(buffer);
            break;
          case TokenizerState.HexNumber:
            buffer.Kind=TokenKind.HexNumber;
            buffer.Value = ParseHexNumber(buffer.String);
            EmitToken(buffer);
            break;
          case TokenizerState.FloatNumber:
            buffer.Kind=TokenKind.FloatNumber;
            buffer.Value = ParseFloatNumber(buffer.String);
            EmitToken(buffer);
            break;
          case TokenizerState.Symbol:
            buffer.Kind=TokenKind.Symbol;
            EmitToken(buffer);
            break;
          case TokenizerState.CharLiteral:
            buffer.Kind=TokenKind.CharLiteral;
            EmitToken(buffer);
            break;
          case TokenizerState.StringLiteral:
            buffer.Kind=TokenKind.StringLiteral;
            EmitToken(buffer);
            break;
          case TokenizerState.Error:
            return;
        }
      }
      state = TokenizerState.Initial;
    }

    protected virtual object ParseNumber(string str)
    {
      if (Int32.TryParse(str, out Int32 n))
        return n;
      else
      if (UInt32.TryParse(str, out UInt32 un))
        return un;
      else
      if (Int64.TryParse(str, out Int64 l))
        return l;
      else
      if (UInt64.TryParse(str, out UInt64 ul))
        return ul;
      return null;
    }

    protected virtual object ParseHexNumber(string str)
    {
      if (Int32.TryParse(str, out Int32 n))
        return n;
      else
      if (UInt32.TryParse(str, out UInt32 un))
        return un;
      else
      if (Int64.TryParse(str, out Int64 l))
        return l;
      else
      if (UInt64.TryParse(str, out UInt64 ul))
        return ul;
      return null;
    }

    protected virtual object ParseFloatNumber(string str)
    {
      if (double.TryParse(str, out double x))
        return x;
      else
      if (decimal.TryParse(str, out decimal d))
        return d;
      return null;
    }

    public virtual void MarkError(Token token)
    {
      state = TokenizerState.Error;
    }

    protected void AcceptToken(Token token)
    {
      switch (token.Kind)
      {
        case TokenKind.Whitespaces:
          AcceptWhitespaces(token);
          break;
        case TokenKind.Letters:
          AcceptLetters(token);
          break;
        case TokenKind.Digits:
          AcceptDigits(token);
          break;
        case TokenKind.Symbol:
          AcceptSymbol(token);
          break;
        case TokenKind.Other:
          AcceptOther(token);
          break;
      }
    }

    protected void AcceptWhitespaces(Token token)
    {
      EmitToken(token);
    }

    protected void AcceptLetters(Token token)
    {
      switch (state)
      {
        case TokenizerState.Initial:
          buffer = token;
          state = TokenizerState.Identifier;
          break;
        case TokenizerState.Identifier:
          buffer = token;
          break;
        case TokenizerState.Number:
          if (buffer.String=="0" && token.String?.ToLower()=="x")
          {
            state = TokenizerState.HexNumber;
            buffer.String += token.String;
          }
          else if (token.String?.ToLower()=="e")
          {
            state = TokenizerState.FloatNumber;
            buffer.String += token.String;
          }
          else
          {
            MarkError(token);
          }
          break;
        case TokenizerState.HexNumber:
          foreach (var ch in token.String.Select(item=>Char.ToLower(item)))
          {
            if (ch<'a' || ch>'f')
            {
              MarkError(token);
              return;
            }
          }
          buffer.String += token.String;
          break;
        case TokenizerState.FloatNumber:
          MarkError(token);
          break;
        case TokenizerState.Symbol:
          Flush();
          buffer = token;
          state = TokenizerState.Number;
          break;
        case TokenizerState.CharLiteral:
        case TokenizerState.StringLiteral:
          buffer.String += token.String;
          break;
        default:
          MarkError(token);
          break;
      }
    }

    protected void AcceptDigits(Token token)
    {
      switch (state)
      {
        case TokenizerState.Initial:
          buffer = token;
          state = TokenizerState.Number;
          break;
        case TokenizerState.Identifier:
          buffer.String += token.String;
          break;
        case TokenizerState.Number:
        case TokenizerState.HexNumber:
        case TokenizerState.FloatNumber:
          buffer.String += token.String;
          break;
        case TokenizerState.Symbol:
          Flush();
          buffer = token;
          state = TokenizerState.Number;
          break;
        case TokenizerState.CharLiteral:
        case TokenizerState.StringLiteral:
          buffer.String += token.String;
          break;
        default:
          MarkError(token);
          break;
      }
    }

    protected void AcceptSymbol(Token token)
    {
      var ch = token.String[0];
      switch (state)
      {
        case TokenizerState.Initial:
          if (ch=='_')
          {
            buffer = token;
            state=TokenizerState.Identifier;
          }
          else
          if (ch=='\'')
          {
            buffer = token;
            state=TokenizerState.CharLiteral;
          }
          else
          if (ch=='\"')
          {
            buffer = token;
            state=TokenizerState.StringLiteral;
          }
          else
          {
            buffer = token;
            state=TokenizerState.Symbol;
          }
          break;
        case TokenizerState.Identifier:
          if (ch=='_')
          {
            buffer.String += token.String;
            state=TokenizerState.Identifier;
          }
          break;
        case TokenizerState.Number:
          if (ch=='.')
          {
            buffer.String += token.String;
            state=TokenizerState.FloatNumber;
          }
          else
          {
            Flush();
            AcceptSymbol(token);
          }
          break;
        case TokenizerState.FloatNumber:
          if (ch=='.' && !buffer.String.Contains('.'))
          {
            buffer.String += token.String;
          }
          if ((ch=='-' || ch=='+') && (buffer.String.EndsWith("e") || buffer.String.EndsWith("E")))
          {
            buffer.String += token.String;
          }
          else
          {
            Flush();
            AcceptSymbol(token);
          }
          break;
        case TokenizerState.HexNumber:
          Flush();
          AcceptSymbol(token);
          break;
        case TokenizerState.Symbol:
          if (!Check2CharSymbol(token))
          {
            Flush();
            AcceptSymbol(token);
          }
          break;
        case TokenizerState.CharLiteral:
          if (token.String=="\'" && !CheckEscape(token))
          {
            buffer.String+=token.String;
            Flush();
          }
          else
          {
            buffer.String+=token.String;
          }
          break;
        case TokenizerState.StringLiteral:
          if (token.String=="\"" && !CheckEscape(token))
          {
            buffer.String+=token.String;
            Flush();
          }
          else
          {
            buffer.String+=token.String;
          }
          break;
        default:
          MarkError(token);
          break;
      }
    }

    protected virtual bool Check2CharSymbol(Token token)
    {
      return false;
    }

    protected virtual bool CheckVerbatim(Token token)
    {
      return false;
    }

    protected virtual bool CheckEscape(Token token)
    {
      return false;
    }

    protected void AcceptOther(Token token)
    {
      MarkError(token);
    }

  }


}
