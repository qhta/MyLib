namespace Qhta.ConsoleTesting
{
  using System;

  public class AssertFailedException : UnitTestAssertException
  {
    public AssertFailedException()
    {
    }

    public AssertFailedException(string msg) : base(msg)
    {
    }

    public AssertFailedException(string msg, Exception ex) : base(msg, ex)
    {
    }
  }
}
