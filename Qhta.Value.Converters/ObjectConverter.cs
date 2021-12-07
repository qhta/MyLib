using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Qhta.Value.Converters
{

  public class ObjectConverter : TypeConverter
  {

    public List<(string ass, string ns)> TypePaths { get; set; }
      = new List<(string ass, string ns)>
        (new (string ass, string ns)[] { new("System.Runtime", "System") });

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      if (destinationType == typeof(string))
        return true;
      return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      if (destinationType != typeof(string))
      {
        if (value is string str)
        {
          if (!str.Contains('"'))
            return '"' + str + '"';
          if (!str.Contains('\''))
            return '\'' + str + '\'';
          return '"' + str.Replace("\"", "\\\"") + '"';
        }
        if (value is Type typeValue)
          return typeValue.Name;
        if (value is Array arrayValue)
        {
          var arrayType = value.GetType();
          var itemType = arrayType.GetElementType();
          var typeName = itemType.Name;
          var itemStrs = new List<string>();
          foreach (var item in arrayValue)
            itemStrs.Add(ConvertToString(item));
          var result = $"{typeName}[]{{{String.Join(",", itemStrs)}}}";
          return result;
        }
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      string str = (string)value;
      if (str.StartsWith('"') && str.EndsWith('"'))
      {
        str = str.Substring(1, str.Length - 2);
        return str.Replace("\\\"", "\"");
      }
      if (str.StartsWith('\'') && str.EndsWith('\''))
      {
        str = str.Substring(1, str.Length - 2);
        return str;
      }
      Type type = null;

      var paramStr = str.ToLowerInvariant();
      if (paramStr == "null")
        return null;
      if (paramStr == "true")
        return true;
      if (paramStr == "false")
        return false;
      if (int.TryParse(paramStr, out var n))
        return n;

      int bracketPos = str.IndexOf("[");
      if (bracketPos >= 0)
      {
        string typeName = null;
        int itemsCount = -1;
        if (bracketPos > 0)
          typeName = str.Substring(0, bracketPos);
        str = str.Substring(bracketPos + 1);
        bracketPos = str.IndexOf("]");
        if (bracketPos >= 0)
        {
          if (int.TryParse(str.Substring(0, bracketPos), out n) && n >= 0)
            itemsCount = n;
          str = str.Substring(bracketPos + 1);
          type = typeof(object);
          if (typeName != null)
            type = GetType(typeName);
          if (type != null)
          {
            var bracePos = str.IndexOfAny(new char[] { '(', '{' });
            if (bracePos < 0 && itemsCount >= 0)
              return Array.CreateInstance(type, itemsCount);
            var endBrace = str[bracePos];
            if (endBrace == '(')
              endBrace = ')';
            else if (endBrace == '{')
              endBrace = '}';
            str = str.Substring(bracePos + 1).Trim();
            if (str.LastOrDefault() == endBrace)
            {
              str = str.Substring(0, str.Length - 1);
              var ss = str.Split(new char[] { ',', ';' });
              if (ss.Length > itemsCount)
                itemsCount = ss.Length;
              var resultArray = Array.CreateInstance(type, itemsCount);
              for (int i = 0; i < ss.Length; i++)
                resultArray.SetValue(ConvertFrom(context, culture, ss[i].Trim()), i);
              return resultArray;
            }
          }
        }
      }
      if (!String.IsNullOrEmpty(str))
      {
        type = GetType(str);
        if (type != null)
          return type;
      }
      return value;
    }

    private Type GetType(string name)
    {
      Type type = Assembly.GetCallingAssembly().GetType(name, false);
      if (type != null)
        return type;
      foreach (var typePath in TypePaths)
      {
        string typeName = typePath.ns + "." + name;
        Assembly assembly = Assembly.Load(typePath.ass);
        type = assembly.GetType(typeName, false);
        if (type != null)
          return type;
      }
      return type;
    }
  }
}
