using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.TextUtils
{
  public static class TextUtils
  {
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
