using MyLib.TypeUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace TestTypeUtils
{
    
    
    /// <summary>
    ///This is a test class for TypeUtilsTest and is intended
    ///to contain all TypeUtilsTest Unit Tests
    ///</summary>
  [TestClass()]
  public class TypeUtilsTest
  {


    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }

    #region Additional test attributes
    // 
    //You can use the following additional attributes as you write your tests:
    //
    //Use ClassInitialize to run code before running the first test in the class
    //[ClassInitialize()]
    //public static void MyClassInitialize(TestContext testContext)
    //{
    //}
    //
    //Use ClassCleanup to run code after all tests in a class have run
    //[ClassCleanup()]
    //public static void MyClassCleanup()
    //{
    //}
    //
    //Use TestInitialize to run code before running each test
    //[TestInitialize()]
    //public void MyTestInitialize()
    //{
    //}
    //
    //Use TestCleanup to run code after each test has run
    //[TestCleanup()]
    //public void MyTestCleanup()
    //{
    //}
    //
    #endregion

/*
    [TestMethod()]
    public void NullElementTypeTest()
    {
      Type aType = typeof(Array);
      Type actual = TypeUtils.GetElementType(aType);
      Assert.AreEqual(null, actual);
    }
*/
    [TestMethod()]
    public void ArrayElementTypeTest()
    {
      int[] aVar = new int[0] { };
      Type aType = aVar.GetType();
      Type actual = TypeUtils.GetElementType(aType);
      Assert.AreEqual(typeof(int), actual);
    }


    [TestMethod()]
    public void ListElementTypeTest()
    {
      IntList aVar = new IntList();
      Type aType = aVar.GetType();
      Type actual = TypeUtils.GetElementType(aType);
      Assert.AreEqual(typeof(int), actual);
    }
  
  }

  public class IntList : List<int>
  {
    public IntList() : base() { }
  }
}
