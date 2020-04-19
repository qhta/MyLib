using System;
using System.Collections.Generic;
using System.Linq;

namespace Qhta.TextUtils
{
  public static class StringUtils
  {

    public static string TitleCase(this string str)
    {
      var chars = str.ToCharArray();
      for (int i = 0; i < chars.Length; i++)
        if (i == 0)
          chars[i] = Char.ToUpper(chars[i]);
        else
          chars[i] = Char.ToLower(chars[i]);
      return new string(chars);
    }

    public static string CamelCase(this string str)
    {
      var ss = str.Split(' ');
      for (int i = 0; i < ss.Length; i++)
        ss[i] = ss[i].TitleCase();
      return string.Join("", ss);
    }

    public static string[] SplitCamelCase(this string str)
    {
      var ss = new List<string>();
      var chars = new List<char>();
      for (int i = 0; i < str.Length; i++)
      {
        if (Char.IsUpper(str[i]))
        {
          if (chars.Count > 0 && (!Char.IsUpper(chars.Last()) || (i<str.Length-1 && Char.IsLower(str[i+1]))))
          {
            var s = new String(chars.ToArray());
            ss.Add(s);
            chars.Clear();
          }
        }
        chars.Add(str[i]);
      }
      if (chars.Count > 0)
      {
        var s = new String(chars.ToArray());
        ss.Add(s);
        chars.Clear();
      }
      return ss.ToArray();
    }

    public static string DeCamelCase(this string str)
    {
      var ss = str.SplitCamelCase();
      for (int i = 0; i < ss.Length; i++)
      {
        if (ss[i] != ss[i].ToUpper())
        {
          if (i == 0)
            ss[i] = ss[i].TitleCase();
          else
            ss[i] = ss[i].ToLower();
        }
      }
      return string.Join(" ", ss);
    }

    public static string NumberToText(this double number)
    {
      Int64 intPart = (Int64)number;
      double frac = number - intPart;

      string result = NumberToText(intPart);
      if (frac!=0)
      {
        int cents = (int)frac*100;
        result += " " + cents.ToString() + "/100";
      }
      return result;
    }

    public static string NumberToText(this int number)
    {
      return NumberToText((Int64)number);
    }

    public static string NumberToText(this Int64 number)
    {
      if (number == 0)
        return "zero";

      if (number < 0)
        return "minus " + NumberToText(Math.Abs(number));

      string words = "";
                  // 9223372036854775808
      if ((number / 1000000000000000000) > 0)
      {
        words += NumberToText(number / 1000000000000000000) + " quintillion ";
        number %= 1000000000000000000;
      }
      
      if ((number / 1000000000000000) > 0)
      {
        words += NumberToText(number / 1000000000000000) + " quadrillion ";
        number %= 1000000000000000;
      }

      if ((number / 1000000000000) > 0)
      {
        words += NumberToText(number / 1000000000000) + " trillion ";
        number %= 1000000000000;
      }

      if ((number / 1000000000) > 0)
      {
        words += NumberToText(number / 1000000000) + " billion ";
        number %= 1000000000;
      }

      if ((number / 1000000) > 0)
      {
        words += NumberToText(number / 1000000) + " million ";
        number %= 1000000;
      }

      if ((number / 1000) > 0)
      {
        words += NumberToText(number / 1000) + " thousand ";
        number %= 1000;
      }

      if ((number / 100) > 0)
      {
        words += NumberToText(number / 100) + " hundred ";
        number %= 100;
      }

      if (number > 0)
      {
        if (words != "")
          words += "and ";

        var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
        var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

        if (number < 20)
          words += unitsMap[number];
        else
        {
          words += tensMap[number / 10];
          if ((number % 10) > 0)
            words += "-" + unitsMap[number % 10];
        }
      }

      return words;
    }
  }
}
