namespace Qhta.ConsoleTesting
{
  using System;

  public class AssertInconclusiveException : UnitTestAssertException
  {
    public AssertInconclusiveException()
    {
    }

    public AssertInconclusiveException(string msg) : base(msg)
    {
    }

    public AssertInconclusiveException(string msg, Exception ex) : base(msg, ex)
    {
    }
  }
}
