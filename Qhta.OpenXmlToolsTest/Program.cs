namespace Qhta.OpenXmlToolsTest;

internal class Program
{
  static void Main(string[] args)
  {
    //var propertiesTest = new PropertiesTest();
    //propertiesTest.DocumentPropertiesReadTest("Test.docx");
    //propertiesTest.DocumentPropertiesWriteTest("WriteTest.docx");
    var settingsTest = new SettingsTest();
    settingsTest.SettingsReadTest("Test.docx");
    settingsTest.SettingsWriteTest("WriteTest.docx");
  }

}
