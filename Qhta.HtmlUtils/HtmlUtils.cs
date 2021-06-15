using System.Text;
using HtmlAgilityPack;

namespace Qhta.HtmlUtils
{
  public static class HtmlNodeUtils 
  {
    public static HtmlNode NextElement(this HtmlNode node)
    {
      node = node.NextSibling;
      while (node != null && node.NodeType != HtmlNodeType.Element)
        node = node.NextSibling;
      return node;
    }

    private static string ToSimpleTextWithSiblings(this HtmlNode node, int fromPos)
    {
      StringBuilder sb = new StringBuilder();
      while (node != null)
      {
        string text = node.InnerText;
        text = text.DecodeHtmlEntities();
        if (fromPos > 0)
        {
          text = text.Substring(fromPos);
          fromPos = 0;
        }
        sb.Append(text);
        node = node.NextSibling;
      }
      return sb.ToString();
    }


  }
}
