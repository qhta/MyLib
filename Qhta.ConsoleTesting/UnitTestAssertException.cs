
namespace Qhta.ConsoleTesting
{
  using System;

  public abstract class UnitTestAssertException : Exception
  {
    protected UnitTestAssertException()
    {
    }

    protected UnitTestAssertException(string msg) : base(msg)
    {
    }

    protected UnitTestAssertException(string msg, Exception ex) : base(msg, ex)
    {
    }
  }
}
