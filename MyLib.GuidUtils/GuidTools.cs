using System;
using System.Security.Cryptography;
using System.Text;

namespace MyLib.GuidUtils
{
  /// <summary>
  /// Klasa narzędziowa dla typu <c>Guid</c>
  /// </summary>
  static public class GuidTools
  {
    /// <summary>
    /// Domieszanie łańcucha do podanego <c>Guida</c>
    /// </summary>
    public static Guid Mix (Guid orig, string extra)
    {
      Byte[] origBytes = orig.ToByteArray();
      Byte[] extraBytes = Encoding.ASCII.GetBytes (extra);
      for (int i = 0; i < Math.Max (origBytes.Length, extraBytes.Length); i++)
        origBytes[i % origBytes.Length] ^= extraBytes[i % extraBytes.Length];
      return new Guid (origBytes);
    }

    const string key = "qwertyuiop";

    /// <summary>
    /// Utworzenie identyfikatora GUID z łańcucha znaków przez funkcję haszującą.
    /// Odwzorowanie jest jednoznaczne
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static Guid HashGuid(string input)
    {
      byte[] hash;
      using (HMACMD5 md5 = new HMACMD5(Encoding.Default.GetBytes(key)))
      {
        hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
      }
      Guid result = new Guid(hash);
      return result;
    }

    /// <summary>
    /// Utworzenie identyfikatora GUID z ciągu bajtów przez funkcję haszującą.
    /// Odwzorowanie jest jednoznaczne
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static Guid HashGuid(byte[] input)
    {
      byte[] hash;
      using (HMACMD5 md5 = new HMACMD5(Encoding.Default.GetBytes(key)))
      {
        hash = md5.ComputeHash(input);
      }
      Guid result = new Guid(hash);
      return result;
    }

  }
}