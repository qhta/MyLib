using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.ConsoleTesting
{
  //
  // Summary:
  //     A collection of helper classes to test various conditions within unit tests.
  //     If the condition being tested is not met, an exception is thrown.
  public sealed class Assert
  {
    private static Assert that;

    //
    // Summary:
    //     Gets the singleton instance of the Assert functionality.
    //
    // Remarks:
    //     Users can use this to plug-in custom assertions through C# extension methods.
    //     For instance, the signature of a custom assertion provider could be "public static
    //     void IsOfType<T>(this Assert assert, object obj)" Users could then use a syntax
    //     similar to the default assertions which in this case is "Assert.That.IsOfType<Dog>(animal);"
    //     More documentation is at "https://github.com/Microsoft/testfx-docs".
    public static Assert That
    {
      get
      {
        if (that == null)
        {
          that = new Assert();
        }

        return that;
      }
    }

    private Assert()
    {
    }

    //
    // Summary:
    //     Tests whether the specified condition is true and throws an exception if the
    //     condition is false.
    //
    // Parameters:
    //   condition:
    //     The condition the test expects to be true.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if condition is false.
    public static void IsTrue(bool condition)
    {
      IsTrue(condition, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the specified condition is true and throws an exception if the
    //     condition is false.
    //
    // Parameters:
    //   condition:
    //     The condition the test expects to be true.
    //
    //   message:
    //     The message to include in the exception when condition is false. The message
    //     is shown in test results.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if condition is false.
    public static void IsTrue(bool condition, string message)
    {
      IsTrue(condition, message, null);
    }

    //
    // Summary:
    //     Tests whether the specified condition is true and throws an exception if the
    //     condition is false.
    //
    // Parameters:
    //   condition:
    //     The condition the test expects to be true.
    //
    //   message:
    //     The message to include in the exception when condition is false. The message
    //     is shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if condition is false.
    public static void IsTrue(bool condition, string message, params object[] parameters)
    {
      if (!condition)
      {
        HandleFail("Assert.IsTrue", message, parameters);
      }
    }

    //
    // Summary:
    //     Tests whether the specified condition is false and throws an exception if the
    //     condition is true.
    //
    // Parameters:
    //   condition:
    //     The condition the test expects to be false.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if condition is true.
    public static void IsFalse(bool condition)
    {
      IsFalse(condition, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the specified condition is false and throws an exception if the
    //     condition is true.
    //
    // Parameters:
    //   condition:
    //     The condition the test expects to be false.
    //
    //   message:
    //     The message to include in the exception when condition is true. The message is
    //     shown in test results.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if condition is true.
    public static void IsFalse(bool condition, string message)
    {
      IsFalse(condition, message, null);
    }

    //
    // Summary:
    //     Tests whether the specified condition is false and throws an exception if the
    //     condition is true.
    //
    // Parameters:
    //   condition:
    //     The condition the test expects to be false.
    //
    //   message:
    //     The message to include in the exception when condition is true. The message is
    //     shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if condition is true.
    public static void IsFalse(bool condition, string message, params object[] parameters)
    {
      if (condition)
      {
        HandleFail("Assert.IsFalse", message, parameters);
      }
    }

    //
    // Summary:
    //     Tests whether the specified object is null and throws an exception if it is not.
    //
    // Parameters:
    //   value:
    //     The object the test expects to be null.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if value is not null.
    public static void IsNull(object value)
    {
      IsNull(value, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the specified object is null and throws an exception if it is not.
    //
    // Parameters:
    //   value:
    //     The object the test expects to be null.
    //
    //   message:
    //     The message to include in the exception when value is not null. The message is
    //     shown in test results.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if value is not null.
    public static void IsNull(object value, string message)
    {
      IsNull(value, message, null);
    }

    //
    // Summary:
    //     Tests whether the specified object is null and throws an exception if it is not.
    //
    // Parameters:
    //   value:
    //     The object the test expects to be null.
    //
    //   message:
    //     The message to include in the exception when value is not null. The message is
    //     shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if value is not null.
    public static void IsNull(object value, string message, params object[] parameters)
    {
      if (value != null)
      {
        HandleFail("Assert.IsNull", message, parameters);
      }
    }

    //
    // Summary:
    //     Tests whether the specified object is non-null and throws an exception if it
    //     is null.
    //
    // Parameters:
    //   value:
    //     The object the test expects not to be null.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if value is null.
    public static void IsNotNull(object value)
    {
      IsNotNull(value, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the specified object is non-null and throws an exception if it
    //     is null.
    //
    // Parameters:
    //   value:
    //     The object the test expects not to be null.
    //
    //   message:
    //     The message to include in the exception when value is null. The message is shown
    //     in test results.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if value is null.
    public static void IsNotNull(object value, string message)
    {
      IsNotNull(value, message, null);
    }

    //
    // Summary:
    //     Tests whether the specified object is non-null and throws an exception if it
    //     is null.
    //
    // Parameters:
    //   value:
    //     The object the test expects not to be null.
    //
    //   message:
    //     The message to include in the exception when value is null. The message is shown
    //     in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if value is null.
    public static void IsNotNull(object value, string message, params object[] parameters)
    {
      if (value == null)
      {
        HandleFail("Assert.IsNotNull", message, parameters);
      }
    }

    //
    // Summary:
    //     Tests whether the specified objects both refer to the same object and throws
    //     an exception if the two inputs do not refer to the same object.
    //
    // Parameters:
    //   expected:
    //     The first object to compare. This is the value the test expects.
    //
    //   actual:
    //     The second object to compare. This is the value produced by the code under test.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected does not refer to the same object as actual.
    public static void AreSame(object expected, object actual)
    {
      AreSame(expected, actual, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the specified objects both refer to the same object and throws
    //     an exception if the two inputs do not refer to the same object.
    //
    // Parameters:
    //   expected:
    //     The first object to compare. This is the value the test expects.
    //
    //   actual:
    //     The second object to compare. This is the value produced by the code under test.
    //
    //   message:
    //     The message to include in the exception when actual is not the same as expected.
    //     The message is shown in test results.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected does not refer to the same object as actual.
    public static void AreSame(object expected, object actual, string message)
    {
      AreSame(expected, actual, message, null);
    }

    //
    // Summary:
    //     Tests whether the specified objects both refer to the same object and throws
    //     an exception if the two inputs do not refer to the same object.
    //
    // Parameters:
    //   expected:
    //     The first object to compare. This is the value the test expects.
    //
    //   actual:
    //     The second object to compare. This is the value produced by the code under test.
    //
    //   message:
    //     The message to include in the exception when actual is not the same as expected.
    //     The message is shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected does not refer to the same object as actual.
    public static void AreSame(object expected, object actual, string message, params object[] parameters)
    {
      if (expected != actual)
      {
        string message2 = message;
        if (expected is ValueType && actual is ValueType)
        {
          message2 = string.Format(CultureInfo.CurrentCulture, FrameworkMessages.AreSameGivenValues, new object[1]
          {
            (message == null) ? string.Empty : ReplaceNulls(message)
          });
        }

        HandleFail("Assert.AreSame", message2, parameters);
      }
    }

    //
    // Summary:
    //     Tests whether the specified objects refer to different objects and throws an
    //     exception if the two inputs refer to the same object.
    //
    // Parameters:
    //   notExpected:
    //     The first object to compare. This is the value the test expects not to match
    //     actual.
    //
    //   actual:
    //     The second object to compare. This is the value produced by the code under test.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected refers to the same object as actual.
    public static void AreNotSame(object notExpected, object actual)
    {
      AreNotSame(notExpected, actual, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the specified objects refer to different objects and throws an
    //     exception if the two inputs refer to the same object.
    //
    // Parameters:
    //   notExpected:
    //     The first object to compare. This is the value the test expects not to match
    //     actual.
    //
    //   actual:
    //     The second object to compare. This is the value produced by the code under test.
    //
    //   message:
    //     The message to include in the exception when actual is the same as notExpected.
    //     The message is shown in test results.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected refers to the same object as actual.
    public static void AreNotSame(object notExpected, object actual, string message)
    {
      AreNotSame(notExpected, actual, message, null);
    }

    //
    // Summary:
    //     Tests whether the specified objects refer to different objects and throws an
    //     exception if the two inputs refer to the same object.
    //
    // Parameters:
    //   notExpected:
    //     The first object to compare. This is the value the test expects not to match
    //     actual.
    //
    //   actual:
    //     The second object to compare. This is the value produced by the code under test.
    //
    //   message:
    //     The message to include in the exception when actual is the same as notExpected.
    //     The message is shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected refers to the same object as actual.
    public static void AreNotSame(object notExpected, object actual, string message, params object[] parameters)
    {
      if (notExpected == actual)
      {
        HandleFail("Assert.AreNotSame", message, parameters);
      }
    }

    //
    // Summary:
    //     Tests whether the specified values are equal and throws an exception if the two
    //     values are not equal. Different numeric types are treated as unequal even if
    //     the logical values are equal. 42L is not equal to 42.
    //
    // Parameters:
    //   expected:
    //     The first value to compare. This is the value the tests expects.
    //
    //   actual:
    //     The second value to compare. This is the value produced by the code under test.
    //
    // Type parameters:
    //   T:
    //     The type of values to compare.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected is not equal to actual.
    public static void AreEqual<T>(T expected, T actual)
    {
      AreEqual(expected, actual, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the specified values are equal and throws an exception if the two
    //     values are not equal. Different numeric types are treated as unequal even if
    //     the logical values are equal. 42L is not equal to 42.
    //
    // Parameters:
    //   expected:
    //     The first value to compare. This is the value the tests expects.
    //
    //   actual:
    //     The second value to compare. This is the value produced by the code under test.
    //
    //   message:
    //     The message to include in the exception when actual is not equal to expected.
    //     The message is shown in test results.
    //
    // Type parameters:
    //   T:
    //     The type of values to compare.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected is not equal to actual.
    public static void AreEqual<T>(T expected, T actual, string message)
    {
      AreEqual(expected, actual, message, null);
    }

    //
    // Summary:
    //     Tests whether the specified values are equal and throws an exception if the two
    //     values are not equal. Different numeric types are treated as unequal even if
    //     the logical values are equal. 42L is not equal to 42.
    //
    // Parameters:
    //   expected:
    //     The first value to compare. This is the value the tests expects.
    //
    //   actual:
    //     The second value to compare. This is the value produced by the code under test.
    //
    //   message:
    //     The message to include in the exception when actual is not equal to expected.
    //     The message is shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Type parameters:
    //   T:
    //     The type of values to compare.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected is not equal to actual.
    public static void AreEqual<T>(T expected, T actual, string message, params object[] parameters)
    {
      if (!object.Equals(expected, actual))
      {
        string message2 = (actual == null || expected == null || actual.GetType().Equals(expected.GetType())) ? string.Format(CultureInfo.CurrentCulture, FrameworkMessages.AreEqualFailMsg, new object[3]
        {
          (message == null) ? string.Empty : ReplaceNulls(message),
          ReplaceNulls(expected),
          ReplaceNulls(actual)
        }) : string.Format(CultureInfo.CurrentCulture, FrameworkMessages.AreEqualDifferentTypesFailMsg, (message == null) ? string.Empty : ReplaceNulls(message), ReplaceNulls(expected), expected.GetType().FullName, ReplaceNulls(actual), actual.GetType().FullName);
        HandleFail("Assert.AreEqual", message2, parameters);
      }
    }

    //
    // Summary:
    //     Tests whether the specified values are unequal and throws an exception if the
    //     two values are equal. Different numeric types are treated as unequal even if
    //     the logical values are equal. 42L is not equal to 42.
    //
    // Parameters:
    //   notExpected:
    //     The first value to compare. This is the value the test expects not to match actual.
    //
    //   actual:
    //     The second value to compare. This is the value produced by the code under test.
    //
    // Type parameters:
    //   T:
    //     The type of values to compare.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected is equal to actual.
    public static void AreNotEqual<T>(T notExpected, T actual)
    {
      AreNotEqual(notExpected, actual, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the specified values are unequal and throws an exception if the
    //     two values are equal. Different numeric types are treated as unequal even if
    //     the logical values are equal. 42L is not equal to 42.
    //
    // Parameters:
    //   notExpected:
    //     The first value to compare. This is the value the test expects not to match actual.
    //
    //   actual:
    //     The second value to compare. This is the value produced by the code under test.
    //
    //   message:
    //     The message to include in the exception when actual is equal to notExpected.
    //     The message is shown in test results.
    //
    // Type parameters:
    //   T:
    //     The type of values to compare.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected is equal to actual.
    public static void AreNotEqual<T>(T notExpected, T actual, string message)
    {
      AreNotEqual(notExpected, actual, message, null);
    }

    //
    // Summary:
    //     Tests whether the specified values are unequal and throws an exception if the
    //     two values are equal. Different numeric types are treated as unequal even if
    //     the logical values are equal. 42L is not equal to 42.
    //
    // Parameters:
    //   notExpected:
    //     The first value to compare. This is the value the test expects not to match actual.
    //
    //   actual:
    //     The second value to compare. This is the value produced by the code under test.
    //
    //   message:
    //     The message to include in the exception when actual is equal to notExpected.
    //     The message is shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Type parameters:
    //   T:
    //     The type of values to compare.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected is equal to actual.
    public static void AreNotEqual<T>(T notExpected, T actual, string message, params object[] parameters)
    {
      if (object.Equals(notExpected, actual))
      {
        string message2 = string.Format(CultureInfo.CurrentCulture, FrameworkMessages.AreNotEqualFailMsg, new object[3]
        {
          (message == null) ? string.Empty : ReplaceNulls(message),
          ReplaceNulls(notExpected),
          ReplaceNulls(actual)
        });
        HandleFail("Assert.AreNotEqual", message2, parameters);
      }
    }

    //
    // Summary:
    //     Tests whether the specified objects are equal and throws an exception if the
    //     two objects are not equal. Different numeric types are treated as unequal even
    //     if the logical values are equal. 42L is not equal to 42.
    //
    // Parameters:
    //   expected:
    //     The first object to compare. This is the object the tests expects.
    //
    //   actual:
    //     The second object to compare. This is the object produced by the code under test.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected is not equal to actual.
    public static void AreEqual(object expected, object actual)
    {
      AreEqual(expected, actual, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the specified objects are equal and throws an exception if the
    //     two objects are not equal. Different numeric types are treated as unequal even
    //     if the logical values are equal. 42L is not equal to 42.
    //
    // Parameters:
    //   expected:
    //     The first object to compare. This is the object the tests expects.
    //
    //   actual:
    //     The second object to compare. This is the object produced by the code under test.
    //
    //   message:
    //     The message to include in the exception when actual is not equal to expected.
    //     The message is shown in test results.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected is not equal to actual.
    public static void AreEqual(object expected, object actual, string message)
    {
      AreEqual(expected, actual, message, null);
    }

    //
    // Summary:
    //     Tests whether the specified objects are equal and throws an exception if the
    //     two objects are not equal. Different numeric types are treated as unequal even
    //     if the logical values are equal. 42L is not equal to 42.
    //
    // Parameters:
    //   expected:
    //     The first object to compare. This is the object the tests expects.
    //
    //   actual:
    //     The second object to compare. This is the object produced by the code under test.
    //
    //   message:
    //     The message to include in the exception when actual is not equal to expected.
    //     The message is shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected is not equal to actual.
    public static void AreEqual(object expected, object actual, string message, params object[] parameters)
    {
      Assert.AreEqual<object>(expected, actual, message, parameters);
    }

    //
    // Summary:
    //     Tests whether the specified objects are unequal and throws an exception if the
    //     two objects are equal. Different numeric types are treated as unequal even if
    //     the logical values are equal. 42L is not equal to 42.
    //
    // Parameters:
    //   notExpected:
    //     The first object to compare. This is the value the test expects not to match
    //     actual.
    //
    //   actual:
    //     The second object to compare. This is the object produced by the code under test.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected is equal to actual.
    public static void AreNotEqual(object notExpected, object actual)
    {
      AreNotEqual(notExpected, actual, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the specified objects are unequal and throws an exception if the
    //     two objects are equal. Different numeric types are treated as unequal even if
    //     the logical values are equal. 42L is not equal to 42.
    //
    // Parameters:
    //   notExpected:
    //     The first object to compare. This is the value the test expects not to match
    //     actual.
    //
    //   actual:
    //     The second object to compare. This is the object produced by the code under test.
    //
    //   message:
    //     The message to include in the exception when actual is equal to notExpected.
    //     The message is shown in test results.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected is equal to actual.
    public static void AreNotEqual(object notExpected, object actual, string message)
    {
      AreNotEqual(notExpected, actual, message, null);
    }

    //
    // Summary:
    //     Tests whether the specified objects are unequal and throws an exception if the
    //     two objects are equal. Different numeric types are treated as unequal even if
    //     the logical values are equal. 42L is not equal to 42.
    //
    // Parameters:
    //   notExpected:
    //     The first object to compare. This is the value the test expects not to match
    //     actual.
    //
    //   actual:
    //     The second object to compare. This is the object produced by the code under test.
    //
    //   message:
    //     The message to include in the exception when actual is equal to notExpected.
    //     The message is shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected is equal to actual.
    public static void AreNotEqual(object notExpected, object actual, string message, params object[] parameters)
    {
      Assert.AreNotEqual<object>(notExpected, actual, message, parameters);
    }

    //
    // Summary:
    //     Tests whether the specified floats are equal and throws an exception if they
    //     are not equal.
    //
    // Parameters:
    //   expected:
    //     The first float to compare. This is the float the tests expects.
    //
    //   actual:
    //     The second float to compare. This is the float produced by the code under test.
    //
    //   delta:
    //     The required accuracy. An exception will be thrown only if actual is different
    //     than expected by more than delta.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected is not equal to actual.
    public static void AreEqual(float expected, float actual, float delta)
    {
      AreEqual(expected, actual, delta, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the specified floats are equal and throws an exception if they
    //     are not equal.
    //
    // Parameters:
    //   expected:
    //     The first float to compare. This is the float the tests expects.
    //
    //   actual:
    //     The second float to compare. This is the float produced by the code under test.
    //
    //   delta:
    //     The required accuracy. An exception will be thrown only if actual is different
    //     than expected by more than delta.
    //
    //   message:
    //     The message to include in the exception when actual is different than expected
    //     by more than delta. The message is shown in test results.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected is not equal to actual.
    public static void AreEqual(float expected, float actual, float delta, string message)
    {
      AreEqual(expected, actual, delta, message, null);
    }

    //
    // Summary:
    //     Tests whether the specified floats are equal and throws an exception if they
    //     are not equal.
    //
    // Parameters:
    //   expected:
    //     The first float to compare. This is the float the tests expects.
    //
    //   actual:
    //     The second float to compare. This is the float produced by the code under test.
    //
    //   delta:
    //     The required accuracy. An exception will be thrown only if actual is different
    //     than expected by more than delta.
    //
    //   message:
    //     The message to include in the exception when actual is different than expected
    //     by more than delta. The message is shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected is not equal to actual.
    public static void AreEqual(float expected, float actual, float delta, string message, params object[] parameters)
    {
      if (float.IsNaN(expected) || float.IsNaN(actual) || float.IsNaN(delta))
      {
        string message2 = string.Format(CultureInfo.CurrentCulture, FrameworkMessages.AreEqualDeltaFailMsg, (message == null) ? string.Empty : ReplaceNulls(message), expected.ToString(CultureInfo.CurrentCulture.NumberFormat), actual.ToString(CultureInfo.CurrentCulture.NumberFormat), delta.ToString(CultureInfo.CurrentCulture.NumberFormat));
        HandleFail("Assert.AreEqual", message2, parameters);
      }

      if (Math.Abs(expected - actual) > delta)
      {
        string message3 = string.Format(CultureInfo.CurrentCulture, FrameworkMessages.AreEqualDeltaFailMsg, (message == null) ? string.Empty : ReplaceNulls(message), expected.ToString(CultureInfo.CurrentCulture.NumberFormat), actual.ToString(CultureInfo.CurrentCulture.NumberFormat), delta.ToString(CultureInfo.CurrentCulture.NumberFormat));
        HandleFail("Assert.AreEqual", message3, parameters);
      }
    }

    //
    // Summary:
    //     Tests whether the specified floats are unequal and throws an exception if they
    //     are equal.
    //
    // Parameters:
    //   notExpected:
    //     The first float to compare. This is the float the test expects not to match actual.
    //
    //   actual:
    //     The second float to compare. This is the float produced by the code under test.
    //
    //   delta:
    //     The required accuracy. An exception will be thrown only if actual is different
    //     than notExpected by at most delta.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected is equal to actual.
    public static void AreNotEqual(float notExpected, float actual, float delta)
    {
      AreNotEqual(notExpected, actual, delta, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the specified floats are unequal and throws an exception if they
    //     are equal.
    //
    // Parameters:
    //   notExpected:
    //     The first float to compare. This is the float the test expects not to match actual.
    //
    //   actual:
    //     The second float to compare. This is the float produced by the code under test.
    //
    //   delta:
    //     The required accuracy. An exception will be thrown only if actual is different
    //     than notExpected by at most delta.
    //
    //   message:
    //     The message to include in the exception when actual is equal to notExpected or
    //     different by less than delta. The message is shown in test results.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected is equal to actual.
    public static void AreNotEqual(float notExpected, float actual, float delta, string message)
    {
      AreNotEqual(notExpected, actual, delta, message, null);
    }

    //
    // Summary:
    //     Tests whether the specified floats are unequal and throws an exception if they
    //     are equal.
    //
    // Parameters:
    //   notExpected:
    //     The first float to compare. This is the float the test expects not to match actual.
    //
    //   actual:
    //     The second float to compare. This is the float produced by the code under test.
    //
    //   delta:
    //     The required accuracy. An exception will be thrown only if actual is different
    //     than notExpected by at most delta.
    //
    //   message:
    //     The message to include in the exception when actual is equal to notExpected or
    //     different by less than delta. The message is shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected is equal to actual.
    public static void AreNotEqual(float notExpected, float actual, float delta, string message, params object[] parameters)
    {
      if (Math.Abs(notExpected - actual) <= delta)
      {
        string message2 = string.Format(CultureInfo.CurrentCulture, FrameworkMessages.AreNotEqualDeltaFailMsg, (message == null) ? string.Empty : ReplaceNulls(message), notExpected.ToString(CultureInfo.CurrentCulture.NumberFormat), actual.ToString(CultureInfo.CurrentCulture.NumberFormat), delta.ToString(CultureInfo.CurrentCulture.NumberFormat));
        HandleFail("Assert.AreNotEqual", message2, parameters);
      }
    }

    //
    // Summary:
    //     Tests whether the specified doubles are equal and throws an exception if they
    //     are not equal.
    //
    // Parameters:
    //   expected:
    //     The first double to compare. This is the double the tests expects.
    //
    //   actual:
    //     The second double to compare. This is the double produced by the code under test.
    //
    //   delta:
    //     The required accuracy. An exception will be thrown only if actual is different
    //     than expected by more than delta.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected is not equal to actual.
    public static void AreEqual(double expected, double actual, double delta)
    {
      AreEqual(expected, actual, delta, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the specified doubles are equal and throws an exception if they
    //     are not equal.
    //
    // Parameters:
    //   expected:
    //     The first double to compare. This is the double the tests expects.
    //
    //   actual:
    //     The second double to compare. This is the double produced by the code under test.
    //
    //   delta:
    //     The required accuracy. An exception will be thrown only if actual is different
    //     than expected by more than delta.
    //
    //   message:
    //     The message to include in the exception when actual is different than expected
    //     by more than delta. The message is shown in test results.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected is not equal to actual.
    public static void AreEqual(double expected, double actual, double delta, string message)
    {
      AreEqual(expected, actual, delta, message, null);
    }

    //
    // Summary:
    //     Tests whether the specified doubles are equal and throws an exception if they
    //     are not equal.
    //
    // Parameters:
    //   expected:
    //     The first double to compare. This is the double the tests expects.
    //
    //   actual:
    //     The second double to compare. This is the double produced by the code under test.
    //
    //   delta:
    //     The required accuracy. An exception will be thrown only if actual is different
    //     than expected by more than delta.
    //
    //   message:
    //     The message to include in the exception when actual is different than expected
    //     by more than delta. The message is shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected is not equal to actual.
    public static void AreEqual(double expected, double actual, double delta, string message, params object[] parameters)
    {
      if (double.IsNaN(expected) || double.IsNaN(actual) || double.IsNaN(delta))
      {
        string message2 = string.Format(CultureInfo.CurrentCulture, FrameworkMessages.AreEqualDeltaFailMsg, (message == null) ? string.Empty : ReplaceNulls(message), expected.ToString(CultureInfo.CurrentCulture.NumberFormat), actual.ToString(CultureInfo.CurrentCulture.NumberFormat), delta.ToString(CultureInfo.CurrentCulture.NumberFormat));
        HandleFail("Assert.AreEqual", message2, parameters);
      }

      if (Math.Abs(expected - actual) > delta)
      {
        string message3 = string.Format(CultureInfo.CurrentCulture, FrameworkMessages.AreEqualDeltaFailMsg, (message == null) ? string.Empty : ReplaceNulls(message), expected.ToString(CultureInfo.CurrentCulture.NumberFormat), actual.ToString(CultureInfo.CurrentCulture.NumberFormat), delta.ToString(CultureInfo.CurrentCulture.NumberFormat));
        HandleFail("Assert.AreEqual", message3, parameters);
      }
    }

    //
    // Summary:
    //     Tests whether the specified doubles are unequal and throws an exception if they
    //     are equal.
    //
    // Parameters:
    //   notExpected:
    //     The first double to compare. This is the double the test expects not to match
    //     actual.
    //
    //   actual:
    //     The second double to compare. This is the double produced by the code under test.
    //
    //   delta:
    //     The required accuracy. An exception will be thrown only if actual is different
    //     than notExpected by at most delta.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected is equal to actual.
    public static void AreNotEqual(double notExpected, double actual, double delta)
    {
      AreNotEqual(notExpected, actual, delta, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the specified doubles are unequal and throws an exception if they
    //     are equal.
    //
    // Parameters:
    //   notExpected:
    //     The first double to compare. This is the double the test expects not to match
    //     actual.
    //
    //   actual:
    //     The second double to compare. This is the double produced by the code under test.
    //
    //   delta:
    //     The required accuracy. An exception will be thrown only if actual is different
    //     than notExpected by at most delta.
    //
    //   message:
    //     The message to include in the exception when actual is equal to notExpected or
    //     different by less than delta. The message is shown in test results.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected is equal to actual.
    public static void AreNotEqual(double notExpected, double actual, double delta, string message)
    {
      AreNotEqual(notExpected, actual, delta, message, null);
    }

    //
    // Summary:
    //     Tests whether the specified doubles are unequal and throws an exception if they
    //     are equal.
    //
    // Parameters:
    //   notExpected:
    //     The first double to compare. This is the double the test expects not to match
    //     actual.
    //
    //   actual:
    //     The second double to compare. This is the double produced by the code under test.
    //
    //   delta:
    //     The required accuracy. An exception will be thrown only if actual is different
    //     than notExpected by at most delta.
    //
    //   message:
    //     The message to include in the exception when actual is equal to notExpected or
    //     different by less than delta. The message is shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected is equal to actual.
    public static void AreNotEqual(double notExpected, double actual, double delta, string message, params object[] parameters)
    {
      if (Math.Abs(notExpected - actual) <= delta)
      {
        string message2 = string.Format(CultureInfo.CurrentCulture, FrameworkMessages.AreNotEqualDeltaFailMsg, (message == null) ? string.Empty : ReplaceNulls(message), notExpected.ToString(CultureInfo.CurrentCulture.NumberFormat), actual.ToString(CultureInfo.CurrentCulture.NumberFormat), delta.ToString(CultureInfo.CurrentCulture.NumberFormat));
        HandleFail("Assert.AreNotEqual", message2, parameters);
      }
    }

    //
    // Summary:
    //     Tests whether the specified strings are equal and throws an exception if they
    //     are not equal. The invariant culture is used for the comparison.
    //
    // Parameters:
    //   expected:
    //     The first string to compare. This is the string the tests expects.
    //
    //   actual:
    //     The second string to compare. This is the string produced by the code under test.
    //
    //   ignoreCase:
    //     A Boolean indicating a case-sensitive or insensitive comparison. (true indicates
    //     a case-insensitive comparison.)
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected is not equal to actual.
    public static void AreEqual(string expected, string actual, bool ignoreCase)
    {
      AreEqual(expected, actual, ignoreCase, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the specified strings are equal and throws an exception if they
    //     are not equal. The invariant culture is used for the comparison.
    //
    // Parameters:
    //   expected:
    //     The first string to compare. This is the string the tests expects.
    //
    //   actual:
    //     The second string to compare. This is the string produced by the code under test.
    //
    //   ignoreCase:
    //     A Boolean indicating a case-sensitive or insensitive comparison. (true indicates
    //     a case-insensitive comparison.)
    //
    //   message:
    //     The message to include in the exception when actual is not equal to expected.
    //     The message is shown in test results.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected is not equal to actual.
    public static void AreEqual(string expected, string actual, bool ignoreCase, string message)
    {
      AreEqual(expected, actual, ignoreCase, message, null);
    }

    //
    // Summary:
    //     Tests whether the specified strings are equal and throws an exception if they
    //     are not equal. The invariant culture is used for the comparison.
    //
    // Parameters:
    //   expected:
    //     The first string to compare. This is the string the tests expects.
    //
    //   actual:
    //     The second string to compare. This is the string produced by the code under test.
    //
    //   ignoreCase:
    //     A Boolean indicating a case-sensitive or insensitive comparison. (true indicates
    //     a case-insensitive comparison.)
    //
    //   message:
    //     The message to include in the exception when actual is not equal to expected.
    //     The message is shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected is not equal to actual.
    public static void AreEqual(string expected, string actual, bool ignoreCase, string message, params object[] parameters)
    {
      AreEqual(expected, actual, ignoreCase, CultureInfo.InvariantCulture, message, parameters);
    }

    //
    // Summary:
    //     Tests whether the specified strings are equal and throws an exception if they
    //     are not equal.
    //
    // Parameters:
    //   expected:
    //     The first string to compare. This is the string the tests expects.
    //
    //   actual:
    //     The second string to compare. This is the string produced by the code under test.
    //
    //   ignoreCase:
    //     A Boolean indicating a case-sensitive or insensitive comparison. (true indicates
    //     a case-insensitive comparison.)
    //
    //   culture:
    //     A CultureInfo object that supplies culture-specific comparison information.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected is not equal to actual.
    public static void AreEqual(string expected, string actual, bool ignoreCase, CultureInfo culture)
    {
      AreEqual(expected, actual, ignoreCase, culture, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the specified strings are equal and throws an exception if they
    //     are not equal.
    //
    // Parameters:
    //   expected:
    //     The first string to compare. This is the string the tests expects.
    //
    //   actual:
    //     The second string to compare. This is the string produced by the code under test.
    //
    //   ignoreCase:
    //     A Boolean indicating a case-sensitive or insensitive comparison. (true indicates
    //     a case-insensitive comparison.)
    //
    //   culture:
    //     A CultureInfo object that supplies culture-specific comparison information.
    //
    //   message:
    //     The message to include in the exception when actual is not equal to expected.
    //     The message is shown in test results.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected is not equal to actual.
    public static void AreEqual(string expected, string actual, bool ignoreCase, CultureInfo culture, string message)
    {
      AreEqual(expected, actual, ignoreCase, culture, message, null);
    }

    //
    // Summary:
    //     Tests whether the specified strings are equal and throws an exception if they
    //     are not equal.
    //
    // Parameters:
    //   expected:
    //     The first string to compare. This is the string the tests expects.
    //
    //   actual:
    //     The second string to compare. This is the string produced by the code under test.
    //
    //   ignoreCase:
    //     A Boolean indicating a case-sensitive or insensitive comparison. (true indicates
    //     a case-insensitive comparison.)
    //
    //   culture:
    //     A CultureInfo object that supplies culture-specific comparison information.
    //
    //   message:
    //     The message to include in the exception when actual is not equal to expected.
    //     The message is shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if expected is not equal to actual.
    public static void AreEqual(string expected, string actual, bool ignoreCase, CultureInfo culture, string message, params object[] parameters)
    {
      CheckParameterNotNull(culture, "Assert.AreEqual", "culture", string.Empty);
      if (CompareInternal(expected, actual, ignoreCase, culture) != 0)
      {
        string message2 = (ignoreCase || CompareInternal(expected, actual, ignoreCase, culture) != 0) ? string.Format(CultureInfo.CurrentCulture, FrameworkMessages.AreEqualFailMsg, new object[3]
        {
          (message == null) ? string.Empty : ReplaceNulls(message),
          ReplaceNulls(expected),
          ReplaceNulls(actual)
        }) : string.Format(CultureInfo.CurrentCulture, FrameworkMessages.AreEqualCaseFailMsg, new object[3]
        {
          (message == null) ? string.Empty : ReplaceNulls(message),
          ReplaceNulls(expected),
          ReplaceNulls(actual)
        });
        HandleFail("Assert.AreEqual", message2, parameters);
      }
    }

    //
    // Summary:
    //     Tests whether the specified strings are unequal and throws an exception if they
    //     are equal. The invariant culture is used for the comparison.
    //
    // Parameters:
    //   notExpected:
    //     The first string to compare. This is the string the test expects not to match
    //     actual.
    //
    //   actual:
    //     The second string to compare. This is the string produced by the code under test.
    //
    //   ignoreCase:
    //     A Boolean indicating a case-sensitive or insensitive comparison. (true indicates
    //     a case-insensitive comparison.)
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected is equal to actual.
    public static void AreNotEqual(string notExpected, string actual, bool ignoreCase)
    {
      AreNotEqual(notExpected, actual, ignoreCase, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the specified strings are unequal and throws an exception if they
    //     are equal. The invariant culture is used for the comparison.
    //
    // Parameters:
    //   notExpected:
    //     The first string to compare. This is the string the test expects not to match
    //     actual.
    //
    //   actual:
    //     The second string to compare. This is the string produced by the code under test.
    //
    //   ignoreCase:
    //     A Boolean indicating a case-sensitive or insensitive comparison. (true indicates
    //     a case-insensitive comparison.)
    //
    //   message:
    //     The message to include in the exception when actual is equal to notExpected.
    //     The message is shown in test results.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected is equal to actual.
    public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, string message)
    {
      AreNotEqual(notExpected, actual, ignoreCase, message, null);
    }

    //
    // Summary:
    //     Tests whether the specified strings are unequal and throws an exception if they
    //     are equal. The invariant culture is used for the comparison.
    //
    // Parameters:
    //   notExpected:
    //     The first string to compare. This is the string the test expects not to match
    //     actual.
    //
    //   actual:
    //     The second string to compare. This is the string produced by the code under test.
    //
    //   ignoreCase:
    //     A Boolean indicating a case-sensitive or insensitive comparison. (true indicates
    //     a case-insensitive comparison.)
    //
    //   message:
    //     The message to include in the exception when actual is equal to notExpected.
    //     The message is shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected is equal to actual.
    public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, string message, params object[] parameters)
    {
      AreNotEqual(notExpected, actual, ignoreCase, CultureInfo.InvariantCulture, message, parameters);
    }

    //
    // Summary:
    //     Tests whether the specified strings are unequal and throws an exception if they
    //     are equal.
    //
    // Parameters:
    //   notExpected:
    //     The first string to compare. This is the string the test expects not to match
    //     actual.
    //
    //   actual:
    //     The second string to compare. This is the string produced by the code under test.
    //
    //   ignoreCase:
    //     A Boolean indicating a case-sensitive or insensitive comparison. (true indicates
    //     a case-insensitive comparison.)
    //
    //   culture:
    //     A CultureInfo object that supplies culture-specific comparison information.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected is equal to actual.
    public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, CultureInfo culture)
    {
      AreNotEqual(notExpected, actual, ignoreCase, culture, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the specified strings are unequal and throws an exception if they
    //     are equal.
    //
    // Parameters:
    //   notExpected:
    //     The first string to compare. This is the string the test expects not to match
    //     actual.
    //
    //   actual:
    //     The second string to compare. This is the string produced by the code under test.
    //
    //   ignoreCase:
    //     A Boolean indicating a case-sensitive or insensitive comparison. (true indicates
    //     a case-insensitive comparison.)
    //
    //   culture:
    //     A CultureInfo object that supplies culture-specific comparison information.
    //
    //   message:
    //     The message to include in the exception when actual is equal to notExpected.
    //     The message is shown in test results.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected is equal to actual.
    public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, CultureInfo culture, string message)
    {
      AreNotEqual(notExpected, actual, ignoreCase, culture, message, null);
    }

    //
    // Summary:
    //     Tests whether the specified strings are unequal and throws an exception if they
    //     are equal.
    //
    // Parameters:
    //   notExpected:
    //     The first string to compare. This is the string the test expects not to match
    //     actual.
    //
    //   actual:
    //     The second string to compare. This is the string produced by the code under test.
    //
    //   ignoreCase:
    //     A Boolean indicating a case-sensitive or insensitive comparison. (true indicates
    //     a case-insensitive comparison.)
    //
    //   culture:
    //     A CultureInfo object that supplies culture-specific comparison information.
    //
    //   message:
    //     The message to include in the exception when actual is equal to notExpected.
    //     The message is shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if notExpected is equal to actual.
    public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, CultureInfo culture, string message, params object[] parameters)
    {
      CheckParameterNotNull(culture, "Assert.AreNotEqual", "culture", string.Empty);
      if (CompareInternal(notExpected, actual, ignoreCase, culture) == 0)
      {
        string message2 = string.Format(CultureInfo.CurrentCulture, FrameworkMessages.AreNotEqualFailMsg, new object[3]
        {
          (message == null) ? string.Empty : ReplaceNulls(message),
          ReplaceNulls(notExpected),
          ReplaceNulls(actual)
        });
        HandleFail("Assert.AreNotEqual", message2, parameters);
      }
    }

    //
    // Summary:
    //     Tests whether the specified object is an instance of the expected type and throws
    //     an exception if the expected type is not in the inheritance hierarchy of the
    //     object.
    //
    // Parameters:
    //   value:
    //     The object the test expects to be of the specified type.
    //
    //   expectedType:
    //     The expected type of value.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if value is null or expectedType is not in the inheritance hierarchy of
    //     value.
    public static void IsInstanceOfType(object value, Type expectedType)
    {
      IsInstanceOfType(value, expectedType, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the specified object is an instance of the expected type and throws
    //     an exception if the expected type is not in the inheritance hierarchy of the
    //     object.
    //
    // Parameters:
    //   value:
    //     The object the test expects to be of the specified type.
    //
    //   expectedType:
    //     The expected type of value.
    //
    //   message:
    //     The message to include in the exception when value is not an instance of expectedType.
    //     The message is shown in test results.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if value is null or expectedType is not in the inheritance hierarchy of
    //     value.
    public static void IsInstanceOfType(object value, Type expectedType, string message)
    {
      IsInstanceOfType(value, expectedType, message, null);
    }

    //
    // Summary:
    //     Tests whether the specified object is an instance of the expected type and throws
    //     an exception if the expected type is not in the inheritance hierarchy of the
    //     object.
    //
    // Parameters:
    //   value:
    //     The object the test expects to be of the specified type.
    //
    //   expectedType:
    //     The expected type of value.
    //
    //   message:
    //     The message to include in the exception when value is not an instance of expectedType.
    //     The message is shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if value is null or expectedType is not in the inheritance hierarchy of
    //     value.
    public static void IsInstanceOfType(object value, Type expectedType, string message, params object[] parameters)
    {
      if ((object)expectedType == null || value == null)
      {
        HandleFail("Assert.IsInstanceOfType", message, parameters);
      }

      TypeInfo typeInfo = value.GetType().GetTypeInfo();
      if (!expectedType.GetTypeInfo().IsAssignableFrom(typeInfo))
      {
        string message2 = string.Format(CultureInfo.CurrentCulture, FrameworkMessages.IsInstanceOfFailMsg, new object[3]
        {
          (message == null) ? string.Empty : ReplaceNulls(message),
          expectedType.ToString(),
          value.GetType().ToString()
        });
        HandleFail("Assert.IsInstanceOfType", message2, parameters);
      }
    }

    //
    // Summary:
    //     Tests whether the specified object is not an instance of the wrong type and throws
    //     an exception if the specified type is in the inheritance hierarchy of the object.
    //
    // Parameters:
    //   value:
    //     The object the test expects not to be of the specified type.
    //
    //   wrongType:
    //     The type that value should not be.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if value is not null and wrongType is in the inheritance hierarchy of
    //     value.
    public static void IsNotInstanceOfType(object value, Type wrongType)
    {
      IsNotInstanceOfType(value, wrongType, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the specified object is not an instance of the wrong type and throws
    //     an exception if the specified type is in the inheritance hierarchy of the object.
    //
    // Parameters:
    //   value:
    //     The object the test expects not to be of the specified type.
    //
    //   wrongType:
    //     The type that value should not be.
    //
    //   message:
    //     The message to include in the exception when value is an instance of wrongType.
    //     The message is shown in test results.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if value is not null and wrongType is in the inheritance hierarchy of
    //     value.
    public static void IsNotInstanceOfType(object value, Type wrongType, string message)
    {
      IsNotInstanceOfType(value, wrongType, message, null);
    }

    //
    // Summary:
    //     Tests whether the specified object is not an instance of the wrong type and throws
    //     an exception if the specified type is in the inheritance hierarchy of the object.
    //
    // Parameters:
    //   value:
    //     The object the test expects not to be of the specified type.
    //
    //   wrongType:
    //     The type that value should not be.
    //
    //   message:
    //     The message to include in the exception when value is an instance of wrongType.
    //     The message is shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if value is not null and wrongType is in the inheritance hierarchy of
    //     value.
    public static void IsNotInstanceOfType(object value, Type wrongType, string message, params object[] parameters)
    {
      if ((object)wrongType == null)
      {
        HandleFail("Assert.IsNotInstanceOfType", message, parameters);
      }

      if (value != null)
      {
        TypeInfo typeInfo = value.GetType().GetTypeInfo();
        if (wrongType.GetTypeInfo().IsAssignableFrom(typeInfo))
        {
          string message2 = string.Format(CultureInfo.CurrentCulture, FrameworkMessages.IsNotInstanceOfFailMsg, new object[3]
          {
            (message == null) ? string.Empty : ReplaceNulls(message),
            wrongType.ToString(),
            value.GetType().ToString()
          });
          HandleFail("Assert.IsNotInstanceOfType", message2, parameters);
        }
      }
    }

    //
    // Summary:
    //     Throws an AssertFailedException.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Always thrown.
    public static void Fail()
    {
      Fail(string.Empty, null);
    }

    //
    // Summary:
    //     Throws an AssertFailedException.
    //
    // Parameters:
    //   message:
    //     The message to include in the exception. The message is shown in test results.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Always thrown.
    public static void Fail(string message)
    {
      Fail(message, null);
    }

    //
    // Summary:
    //     Throws an AssertFailedException.
    //
    // Parameters:
    //   message:
    //     The message to include in the exception. The message is shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Always thrown.
    public static void Fail(string message, params object[] parameters)
    {
      HandleFail("Assert.Fail", message, parameters);
    }

    //
    // Summary:
    //     Throws an AssertInconclusiveException.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertInconclusiveException:
    //     Always thrown.
    public static void Inconclusive()
    {
      Inconclusive(string.Empty, null);
    }

    //
    // Summary:
    //     Throws an AssertInconclusiveException.
    //
    // Parameters:
    //   message:
    //     The message to include in the exception. The message is shown in test results.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertInconclusiveException:
    //     Always thrown.
    public static void Inconclusive(string message)
    {
      Inconclusive(message, null);
    }

    //
    // Summary:
    //     Throws an AssertInconclusiveException.
    //
    // Parameters:
    //   message:
    //     The message to include in the exception. The message is shown in test results.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertInconclusiveException:
    //     Always thrown.
    public static void Inconclusive(string message, params object[] parameters)
    {
      string text = string.Empty;
      if (!string.IsNullOrEmpty(message))
      {
        text = ((parameters != null) ? string.Format(CultureInfo.CurrentCulture, ReplaceNulls(message), parameters) : ReplaceNulls(message));
      }

      throw new AssertInconclusiveException(string.Format(CultureInfo.CurrentCulture, FrameworkMessages.AssertionFailed, new object[2]
      {
        "Assert.Inconclusive",
        text
      }));
    }

    //
    // Summary:
    //     Static equals overloads are used for comparing instances of two types for reference
    //     equality. This method should not be used for comparison of two instances for
    //     equality. This object will always throw with Assert.Fail. Please use Assert.AreEqual
    //     and associated overloads in your unit tests.
    //
    // Parameters:
    //   objA:
    //     Object A
    //
    //   objB:
    //     Object B
    //
    // Returns:
    //     False, always.
    public new static bool Equals(object objA, object objB)
    {
      Fail(FrameworkMessages.DoNotUseAssertEquals);
      return false;
    }

    //
    // Summary:
    //     Tests whether the code specified by delegate action throws exact given exception
    //     of type T (and not of derived type) and throws
    //     AssertFailedException
    //     if code does not throws exception or throws exception of type other than T.
    //
    // Parameters:
    //   action:
    //     Delegate to code to be tested and which is expected to throw exception.
    //
    // Type parameters:
    //   T:
    //     Type of exception expected to be thrown.
    //
    // Returns:
    //     The exception that was thrown.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if action does not throws exception of type T.
    public static T ThrowsException<T>(Action action) where T : Exception
    {
      return ThrowsException<T>(action, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the code specified by delegate action throws exact given exception
    //     of type T (and not of derived type) and throws
    //     AssertFailedException
    //     if code does not throws exception or throws exception of type other than T.
    //
    // Parameters:
    //   action:
    //     Delegate to code to be tested and which is expected to throw exception.
    //
    //   message:
    //     The message to include in the exception when action does not throws exception
    //     of type T.
    //
    // Type parameters:
    //   T:
    //     Type of exception expected to be thrown.
    //
    // Returns:
    //     The exception that was thrown.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if action does not throws exception of type T.
    public static T ThrowsException<T>(Action action, string message) where T : Exception
    {
      return ThrowsException<T>(action, message, null);
    }

    //
    // Summary:
    //     Tests whether the code specified by delegate action throws exact given exception
    //     of type T (and not of derived type) and throws
    //     AssertFailedException
    //     if code does not throws exception or throws exception of type other than T.
    //
    // Parameters:
    //   action:
    //     Delegate to code to be tested and which is expected to throw exception.
    //
    // Type parameters:
    //   T:
    //     Type of exception expected to be thrown.
    //
    // Returns:
    //     The exception that was thrown.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if action does not throws exception of type T.
    public static T ThrowsException<T>(Func<object> action) where T : Exception
    {
      return ThrowsException<T>(action, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the code specified by delegate action throws exact given exception
    //     of type T (and not of derived type) and throws
    //     AssertFailedException
    //     if code does not throws exception or throws exception of type other than T.
    //
    // Parameters:
    //   action:
    //     Delegate to code to be tested and which is expected to throw exception.
    //
    //   message:
    //     The message to include in the exception when action does not throws exception
    //     of type T.
    //
    // Type parameters:
    //   T:
    //     Type of exception expected to be thrown.
    //
    // Returns:
    //     The exception that was thrown.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if action does not throws exception of type T.
    public static T ThrowsException<T>(Func<object> action, string message) where T : Exception
    {
      return ThrowsException<T>(action, message, null);
    }

    //
    // Summary:
    //     Tests whether the code specified by delegate action throws exact given exception
    //     of type T (and not of derived type) and throws
    //     AssertFailedException
    //     if code does not throws exception or throws exception of type other than T.
    //
    // Parameters:
    //   action:
    //     Delegate to code to be tested and which is expected to throw exception.
    //
    //   message:
    //     The message to include in the exception when action does not throws exception
    //     of type T.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Type parameters:
    //   T:
    //     Type of exception expected to be thrown.
    //
    // Returns:
    //     The exception that was thrown.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if action does not throw exception of type T.
    public static T ThrowsException<T>(Func<object> action, string message, params object[] parameters) where T : Exception
    {
      return ThrowsException<T>(delegate
      {
        action();
      }, message, parameters);
    }

    //
    // Summary:
    //     Tests whether the code specified by delegate action throws exact given exception
    //     of type T (and not of derived type) and throws
    //     AssertFailedException
    //     if code does not throws exception or throws exception of type other than T.
    //
    // Parameters:
    //   action:
    //     Delegate to code to be tested and which is expected to throw exception.
    //
    //   message:
    //     The message to include in the exception when action does not throws exception
    //     of type T.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Type parameters:
    //   T:
    //     Type of exception expected to be thrown.
    //
    // Returns:
    //     The exception that was thrown.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if action does not throws exception of type T.
    public static T ThrowsException<T>(Action action, string message, params object[] parameters) where T : Exception
    {
      string empty = string.Empty;
      if (action == null)
      {
        throw new ArgumentNullException("action");
      }

      if (message == null)
      {
        throw new ArgumentNullException("message");
      }

      try
      {
        action();
      }
      catch (Exception ex)
      {
        if (!typeof(T).Equals(((object)ex).GetType()))
        {
          empty = string.Format(CultureInfo.CurrentCulture, FrameworkMessages.WrongExceptionThrown, ReplaceNulls(message), typeof(T).Name, ((object)ex).GetType().Name, ex.Message, ex.StackTrace);
          HandleFail("Assert.ThrowsException", empty, parameters);
        }

        return (T)ex;
      }

      empty = string.Format(CultureInfo.CurrentCulture, FrameworkMessages.NoExceptionThrown, new object[2]
      {
        ReplaceNulls(message),
        typeof(T).Name
      });
      HandleFail("Assert.ThrowsException", empty, parameters);
      return null;
    }

    //
    // Summary:
    //     Tests whether the code specified by delegate action throws exact given exception
    //     of type T (and not of derived type) and throws
    //     AssertFailedException
    //     if code does not throws exception or throws exception of type other than T.
    //
    // Parameters:
    //   action:
    //     Delegate to code to be tested and which is expected to throw exception.
    //
    // Type parameters:
    //   T:
    //     Type of exception expected to be thrown.
    //
    // Returns:
    //     The System.Threading.Tasks.Task executing the delegate.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if action does not throws exception of type T.
    public static async Task<T> ThrowsExceptionAsync<T>(Func<Task> action) where T : Exception
    {
      return await ThrowsExceptionAsync<T>(action, string.Empty, null);
    }

    //
    // Summary:
    //     Tests whether the code specified by delegate action throws exact given exception
    //     of type T (and not of derived type) and throws
    //     AssertFailedException
    //     if code does not throws exception or throws exception of type other than T.
    //
    // Parameters:
    //   action:
    //     Delegate to code to be tested and which is expected to throw exception.
    //
    //   message:
    //     The message to include in the exception when action does not throws exception
    //     of type T.
    //
    // Type parameters:
    //   T:
    //     Type of exception expected to be thrown.
    //
    // Returns:
    //     The System.Threading.Tasks.Task executing the delegate.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if action does not throws exception of type T.
    public static async Task<T> ThrowsExceptionAsync<T>(Func<Task> action, string message) where T : Exception
    {
      return await ThrowsExceptionAsync<T>(action, message, null);
    }

    //
    // Summary:
    //     Tests whether the code specified by delegate action throws exact given exception
    //     of type T (and not of derived type) and throws
    //     AssertFailedException
    //     if code does not throws exception or throws exception of type other than T.
    //
    // Parameters:
    //   action:
    //     Delegate to code to be tested and which is expected to throw exception.
    //
    //   message:
    //     The message to include in the exception when action does not throws exception
    //     of type T.
    //
    //   parameters:
    //     An array of parameters to use when formatting message.
    //
    // Type parameters:
    //   T:
    //     Type of exception expected to be thrown.
    //
    // Returns:
    //     The System.Threading.Tasks.Task executing the delegate.
    //
    // Exceptions:
    //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
    //     Thrown if action does not throws exception of type T.
    public static async Task<T> ThrowsExceptionAsync<T>(Func<Task> action, string message, params object[] parameters) where T : Exception
    {
      _ = string.Empty;
      if (action == null)
      {
        throw new ArgumentNullException("action");
      }

      if (message == null)
      {
        throw new ArgumentNullException("message");
      }

      string message2;
      try
      {
        await action();
      }
      catch (Exception ex)
      {
        if (!typeof(T).Equals(((object)ex).GetType()))
        {
          message2 = string.Format(CultureInfo.CurrentCulture, FrameworkMessages.WrongExceptionThrown, ReplaceNulls(message), typeof(T).Name, ((object)ex).GetType().Name, ex.Message, ex.StackTrace);
          HandleFail("Assert.ThrowsException", message2, parameters);
        }

        return (T)ex;
      }

      message2 = string.Format(CultureInfo.CurrentCulture, FrameworkMessages.NoExceptionThrown, new object[2]
      {
        ReplaceNulls(message),
        typeof(T).Name
      });
      HandleFail("Assert.ThrowsException", message2, parameters);
      return null;
    }

    //
    // Summary:
    //     Replaces null characters ('\0') with "\\0".
    //
    // Parameters:
    //   input:
    //     The string to search.
    //
    // Returns:
    //     The converted string with null characters replaced by "\\0".
    //
    // Remarks:
    //     This is only public and still present to preserve compatibility with the V1 framework.
    public static string ReplaceNullChars(string input)
    {
      if (string.IsNullOrEmpty(input))
      {
        return input;
      }

      return input.Replace("\0", "\\0");
    }

    //
    // Summary:
    //     Helper function that creates and throws an AssertionFailedException
    //
    // Parameters:
    //   assertionName:
    //     name of the assertion throwing an exception
    //
    //   message:
    //     message describing conditions for assertion failure
    //
    //   parameters:
    //     The parameters.
    internal static void HandleFail(string assertionName, string message, params object[] parameters)
    {
      string text = string.Empty;
      if (!string.IsNullOrEmpty(message))
      {
        text = ((parameters != null) ? string.Format(CultureInfo.CurrentCulture, ReplaceNulls(message), parameters) : ReplaceNulls(message));
      }

      throw new AssertFailedException(string.Format(CultureInfo.CurrentCulture, FrameworkMessages.AssertionFailed, new object[2]
      {
        assertionName,
        text
      }));
    }

    //
    // Summary:
    //     Checks the parameter for valid conditions
    //
    // Parameters:
    //   param:
    //     The parameter.
    //
    //   assertionName:
    //     The assertion Name.
    //
    //   parameterName:
    //     parameter name
    //
    //   message:
    //     message for the invalid parameter exception
    //
    //   parameters:
    //     The parameters.
    internal static void CheckParameterNotNull(object param, string assertionName, string parameterName, string message, params object[] parameters)
    {
      if (param == null)
      {
        HandleFail(assertionName, string.Format(CultureInfo.CurrentCulture, FrameworkMessages.NullParameterToAssert, new object[2]
        {
          parameterName,
          message
        }), parameters);
      }
    }

    //
    // Summary:
    //     Safely converts an object to a string, handling null values and null characters.
    //     Null values are converted to "(null)". Null characters are converted to "\\0".
    //
    // Parameters:
    //   input:
    //     The object to convert to a string.
    //
    // Returns:
    //     The converted string.
    internal static string ReplaceNulls(object input)
    {
      if (input == null)
      {
        return FrameworkMessages.Common_NullInMessages.ToString();
      }

      string text = input.ToString();
      if (text == null)
      {
        return FrameworkMessages.Common_ObjectString.ToString();
      }

      return ReplaceNullChars(text);
    }

    private static int CompareInternal(string expected, string actual, bool ignoreCase, CultureInfo culture)
    {
      return string.Compare(expected, actual, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
    }
  }


}
