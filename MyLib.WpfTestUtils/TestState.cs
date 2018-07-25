using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.WpfTestUtils
{
  public enum TestState
  {
    /// <summary>
    ///    The test is pending.
    /// </summary>
    Pending,
    /// <summary>
    ///    The test is currently running.
    /// </summary>
    InProgress,
    /// <summary>
    ///    The test passed.
    /// </summary>
    Passed,
    /// <summary>
    /// The test failed.
    /// </summary>
    Failed,
    /// <summary>
    ///  Assert.Inconclusive was raised.
    /// </summary>
    Inconclusive,
    /// <summary>
    ///  Internal exception occurred
    /// </summary>
    Error,
    /// <summary>
    /// The test was run too long
    /// </summary>
    Timeout,
    /// <summary>
    /// The test was interrupted by the user
    /// </summary>
    Aborted,
    /// <summary>
    /// The test was cancelled due to preceding test methods failure
    /// </summary>
    Cancelled,
    /// <summary>
    ///  The state is unknown
    /// </summary>
    Unknown
  }
}
