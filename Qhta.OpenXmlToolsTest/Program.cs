namespace Qhta.OpenXmlToolsTest;

internal class Program
{
  static void Main(string[] args)
  {
    //var propertiesTest = new PropertiesTest();
    //propertiesTest.DocumentPropertiesReadTest("Test.docx");
    //propertiesTest.DocumentPropertiesWriteTest("WriteTest.docx");
    //var settingsTest = new SettingsTest();
    //settingsTest.SettingsReadTest("Test.docx");
    ////settingsTest.SettingsWriteTest("WriteTest.docx");
    //var mathPropertiesTest = new MathPropertiesTest();
    //mathPropertiesTest.MathPropertiesReadTest("Test.docx");
    //mathPropertiesTest.MathPropertiesWriteTest("WriteTest.docx");
    var compatibilitySettingsTest = new CompatibilitySettingsTest();
    compatibilitySettingsTest.CompatibilitySettingsReadTest("CompatibilitySettingsTest12.docx");
    compatibilitySettingsTest.CompatibilitySettingsReadTest("CompatibilitySettingsTest15.docx");
    compatibilitySettingsTest.CompatibilitySettingsWriteTest("WriteTest.docx");
  }

}
