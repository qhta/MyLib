using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.TestHelper
{
  public class TestWriter : TextWriter
  {
    public List<string> Lines { get; init; } = new List<string>();
    public object LockObject { get; init;}

    public TestWriter(object lockObject): base()
    {
      LockObject = lockObject;
    }

    public override void WriteLine(string line)
    {
      line = line.Replace("\n", "\\n");
      Lines.Add(line);
    }

    public override void Flush()
    {
      lock (LockObject)
      {
        foreach (var line in Lines)
        {
          Debug.WriteLine(line);
          Console.WriteLine(line);
        }
        Debug.Flush();
        Lines.Clear();
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        Flush();
      }
    }

    public override Encoding Encoding => Encoding.UTF8;
  }
}
