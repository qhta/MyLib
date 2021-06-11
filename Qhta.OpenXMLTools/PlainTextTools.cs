using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

using System;
using System.Collections.Generic;
using System.Text;

namespace Qhta.OpenXMLTools
{
  public static class PlainTextTools
  {
    public static string GetPlainText(this OpenXmlCompositeElement element)
    {
      var stringBuilder = new StringBuilder();
      foreach (var node in element.ChildElements)
      {
        if (node is Run runNode)
          stringBuilder.Append(runNode.InnerText);
      }
      return stringBuilder.ToString();
    }

    public static string GetPlainText(this Run element)
    {
      var stringBuilder = new StringBuilder();
      foreach (var node in element.ChildElements)
      {
        if (!(node is FieldCode))
          stringBuilder.Append(node.InnerText);
      }
      return stringBuilder.ToString();
    }
  }
}
