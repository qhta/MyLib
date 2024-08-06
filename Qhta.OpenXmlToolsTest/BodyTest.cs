using DocumentFormat.OpenXml.Packaging;

using Range = Qhta.OpenXmlTools.Range;

namespace Qhta.OpenXmlToolsTest;


public class BodyTest

{
  public void BodyReadTest(string filename)
  {
    using (var wordDoc = DXPack.WordprocessingDocument.Open(filename, false))
    {
      //BodyRangeReadTest(wordDoc);
      //BodyRangeParagraphsReadTest(wordDoc);
      //BodyRangeTablesReadTest(wordDoc);
      //BodyRangeStdBlocksReadTest(wordDoc);
      //BodyRangeElementTypesCountTest(wordDoc);
      //BodyRangeElementTypesAndSubtypesCountTest(wordDoc);
      BodyReadTextTest(wordDoc);
    }
  }

  public void BodyWriteTest(string sourceFilename, string filename)
  {
    //using (var sourceDoc = WordprocessingDocument.Open(sourceFilename, false))
    //using (var wordDoc = WordprocessingDocument.Create(filename, WordprocessingDocumentType.Document))
    //{
    //  BodyWriteTest(sourceDoc, wordDoc);
    //}
    //using (var wordDoc = WordprocessingDocument.Open(filename, false))
    //{
    //  BodyReadTest(wordDoc);
    //}
  }


  public void BodyRangeReadTest(DXPack.WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Body range read test:");

    var docRange = wordDoc.GetBody().GetRange();
    var elements = docRange.GetElements();
    var count = elements.Count();
    Console.WriteLine($"Body range elements count = {count}");
    foreach (var element in elements)
    {
      Console.WriteLine($"{element.AsString(1, 2)}");
    }
    Console.WriteLine();
  }

  public void BodyRangeParagraphsReadTest(DXPack.WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Body range paragraphs read test:");

    var docRange = wordDoc.GetBody().GetRange();
    var elements = docRange.GetParagraphs();
    var count = elements.Count();
    Console.WriteLine($"Body range paragraphs count = {count}");
    foreach (var element in elements)
    {
      Console.WriteLine($"{element.AsString(1, 2)}");
    }
    Console.WriteLine();
  }

  public void BodyRangeTablesReadTest(DXPack.WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Body range tables read test:");

    var docRange = wordDoc.GetBody().GetRange();
    var elements = docRange.GetTables();
    var count = elements.Count();
    Console.WriteLine($"Body range tables count = {count}");
    foreach (var element in elements)
    {
      Console.WriteLine($"{element.AsString(1, 3)}");
    }
    Console.WriteLine();
  }

  public void BodyRangeStdBlocksReadTest(DXPack.WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Body range sdt blocks read test:");

    var docRange = wordDoc.GetBody().GetRange();
    var elements = docRange.GetElements<DXW.SdtBlock>();
    var count = elements.Count();
    Console.WriteLine($"Body range sdt blocks count = {count}");
    foreach (var element in elements)
    {
      Console.WriteLine($"{element.AsString(1, 3)}");
    }
    Console.WriteLine();
  }


  public void BodyRangeElementTypesCountTest(DXPack.WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Body range element types count test:");

    var range = wordDoc.GetBody().GetRange();
    var elements = range.GetElements().GroupBy(e => e.GetType().Name)
      .ToDictionary(e => e.Key, e => e.Count())
      .OrderByDescending(e => e.Value).ToArray();
    var count = elements.Count();
    Console.WriteLine($"Body range element types count = {count}");
    foreach (var element in elements)
    {
      var typeName = element.Key;
      if (!typeName.EndsWith("s"))
        typeName += "s";
      Console.WriteLine($"{typeName} count = {element.Value}");

    }
    Console.WriteLine();
  }

  public void BodyRangeElementTypesAndSubtypesCountTest(DXPack.WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Body range element types and subtypes count test:");

    var range = wordDoc.GetBody().GetRange();
    var typeInfos = GetTypeInfos(range);
    PrintTypeInfo(typeInfos,0);
  }

  private void PrintTypeInfo(IEnumerable<MyTypeInfo> typeInfos, int indent)
  {
    var indentStr = new string(' ', indent * 2);
    foreach (var info in typeInfos)
    {
      Console.WriteLine($"{indentStr}{info.TypeName} count = {info.Count}");
      PrintTypeInfo(info.Subtypes, indent+1);
    }
  }

  private record MyTypeInfo
  {
    public string TypeName { get; init; } = null!;
    public int Count { get; set; }
    public List<MyTypeInfo> Subtypes { get; init; } = new();
  }

  private List<MyTypeInfo> GetTypeInfos(Range range)
  {
    var elements = range.GetElements().GroupBy(e => e.Prefix+":"+e.LocalName)
          .ToDictionary(e => e.Key, e => e.ToArray())
          .ToArray();
    var result = new List<MyTypeInfo>();
    foreach (var keyValuePair in elements)
    {
      var typeName = keyValuePair.Key;
      var count = keyValuePair.Value.Length;
      var subtypes = new Dictionary<string, MyTypeInfo>();
      foreach (var element in keyValuePair.Value)
      {
        if (element is DX.OpenXmlCompositeElement compositeElement && element.HasChildren)
        {
          var elementRange = compositeElement.GetRange();
          var typeInfo = GetTypeInfos(elementRange);
          foreach (var subTypeInfo in typeInfo)
          {
            var subTypeName = subTypeInfo.TypeName;
            if (!subtypes.TryGetValue(subTypeName, out var info))
            {
              info = new MyTypeInfo { TypeName = subTypeName };
              subtypes.Add(subTypeName, info);
            }
            info.Count += subTypeInfo.Count;
            MergeTypeInfos(info, subTypeInfo.Subtypes);
          }
        }
        //else
        //{
        //  var subTypeName = subElement.Prefix + ":" + subElement.LocalName;
        //  if (!subtypes.TryGetValue(subTypeName, out var info))
        //    subtypes[typeName] = new MyTypeInfo { TypeName = subTypeName };
        //  else
        //    info.Count++;
        //}
      }
      result.Add(new MyTypeInfo{TypeName =typeName, Count = count, Subtypes = subtypes.Values.ToList() });
    }
    return result;

  }

  private static void MergeTypeInfos(MyTypeInfo info, List<MyTypeInfo> subTypeInfo)
  {
    foreach (var subInfo in subTypeInfo)
    {
      var typeName = subInfo.TypeName;
      if (!TryGetValue(info.Subtypes, typeName, out var infoSubInfo))
      {
        infoSubInfo = new MyTypeInfo{TypeName = typeName};
        info.Subtypes.Add(infoSubInfo);
      }
      infoSubInfo.Count += subInfo.Count;
      MergeTypeInfos(infoSubInfo, subInfo.Subtypes);
    }
  }

  private static bool TryGetValue(List<MyTypeInfo> baseList, string typeName, out MyTypeInfo info)
  {
    foreach (var item in baseList)
    {
      if (item.TypeName == typeName)
      {
        info = item;
        return true;
      }
    }
    info = null!;
    return false;
  }

  public void BodyReadTextTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Body read text test:");
    var wordBody = wordDoc.GetBody();
    var wordText = wordBody.GetRange().GetText();
    Console.WriteLine($"Word text length = {wordText.Length}");
    Console.WriteLine(wordText);

  }

  //public void BodyWriteTest(WordprocessingDocument sourceDoc, WordprocessingDocument targetDoc)
  //{
  //  Console.WriteLine("Body write test:");
  //  var sourceBody = sourceDoc.GetBody();
  //  var targetBody = targetDoc.GetBody();
  //  foreach (var styleName in sourceBody.GetNames(ItemFilter.All))
  //  {
  //    var sourceStyle = sourceBody.GetStyle(styleName);
  //    if (sourceStyle == null)
  //      throw new Exception($"Source style \"{styleName}\" not found");
  //    Console.WriteLine($"{styleName} copied");
  //    var targetStyle = (Style)sourceStyle.CloneNode(true);
  //    targetBody.SetStyle(targetStyle);
  //  }
  //  Console.WriteLine();
  //}

}
