using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MyLib.WpfTestUtils
{
  public class VisualTextWriter: TextWriter
  {
    public VisualTextWriter(TextBox targetControl)
    {
      TargetControl = targetControl;
    }

    TextBox TargetControl;

    public override Encoding Encoding { get { return UnicodeEncoding.Unicode; } }

    public override void WriteLine()
    {
      TargetControl.Text = TargetControl.Text + "\n";
    }

    public override void WriteLine(string value)
    {
      TargetControl.Text = TargetControl.Text + value +"\n";
    }

    public override void Write(string value)
    {
      TargetControl.Text = TargetControl.Text + value;
    }
  }
}
