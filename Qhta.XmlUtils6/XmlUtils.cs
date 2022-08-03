using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace Qhta.XmlUtils
{
  public static class XmlUtils
  {
    public static string IndentXml(this string xml)
    {
      try
      {
        var output = new MemoryStream();
        using (var writer = new XmlTextWriter(output, Encoding.UTF8) { Formatting=Formatting.Indented })
        {
          var document = new XmlDocument();
          document.LoadXml(xml);
          document.WriteTo(writer);
          writer.Flush();
          output.Seek(0, SeekOrigin.Begin);
          var tr = new StreamReader(output);
          var result = tr.ReadToEnd();
          return result;
        }
      }
      catch(Exception ex)
      {
        Debug.WriteLine(ex.Message);
        return xml;
      }
    }
  }
}
