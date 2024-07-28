namespace Qhta.OpenXmlToolsTest;

internal class Program
{
  static void Main(string[] args)
  {
    var test = new PropertiesTest();
    test.DocumentPropertiesReadTest("Test.docx");
    test.DocumentPropertiesWriteTest("WriteTest.docx");
  }

}
