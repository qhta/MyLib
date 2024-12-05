using System.Text;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with OpenXml Wordprocessing plain text element.
/// </summary>
public static class PlainTextTools
{
  ///// <summary>
  ///// Get the plain text from a composite element. Joins the text of the run elements.
  ///// </summary>
  ///// <param name="element">source element</param>
  ///// <returns>joined string</returns>
  //public static string GetPlainText(this OpenXmlCompositeElement element)
  //{
  //  var stringBuilder = new StringBuilder();
  //  foreach (var node in element.ChildElements)
  //  {
  //    if (node is Run runNode)
  //      stringBuilder.Append(runNode.InnerText);
  //  }
  //  return stringBuilder.ToString();
  //}

  ///// <summary>
  ///// Get the plain text from a run element. Joins the text of the run items which are not field codes.
  ///// </summary>
  ///// <param name="element">source element</param>
  ///// <returns>joined string</returns>
  //public static string GetPlainText(this Run element)
  //{
  //  var stringBuilder = new StringBuilder();
  //  foreach (var node in element.ChildElements)
  //  {
  //    if (!(node is FieldCode))
  //      stringBuilder.Append(node.InnerText);
  //  }
  //  return stringBuilder.ToString();
  //}
}