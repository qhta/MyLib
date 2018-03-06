using NLP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.TextUtils
{

  public struct Definition
  {
    public string Definiendum;
    public string Definiens;

    public Definition(string s)
    {
      string[] df = s.SplitWithTrim(':');
      if (df.Length == 2)
      {
        Definiendum = df[0];
        Definiens = df[1];
      }
      else
      {
        Definiendum = null;
        Definiens = s;
      }
    }

    public override string ToString()
    {
      if (Definiendum != null)
        return Definiendum + ": " + Definiens;
      return Definiens;
    }
  }
}
