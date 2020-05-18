﻿using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXMLTools
{
  public static class OpenXmlElementTools
  {
    public static OpenXmlPart GetDocumentPart(this OpenXmlElement xmlElement)
    {
      if (xmlElement == null)
      {
        return null;
      }
      if (xmlElement is Document document)
      {
        return document.MainDocumentPart;
      }

      if (xmlElement is Header header)
      {
        return header.HeaderPart;
      }

      if (xmlElement is Footer footer)
      {
        return footer.FooterPart;
      }

      return GetDocumentPart(xmlElement.Parent);
    }
  }
}

