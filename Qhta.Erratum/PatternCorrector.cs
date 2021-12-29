using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.Erratum
{
  public class PatternCorrector
  {
    public PatternCorrector(string filename)
    {
      Filename = filename;
      patternSets.Load(Filename);
    }

    private string Filename { get; init; }

    private PatternSetDictionary patternSets = new();

    public void Load()
    {
      lock (this)
      {
        patternSets.Load(Filename);
      }
    }


    public bool FindBestPosition(string inStr, string patStr, out int pos, out float probability)
    {
      pos = 0;
      probability = 0;
      if (patternSets.TryGetValue(patStr, out var patternSet))
      {
        var wholePattern = PatternUtils.CreatePattern(inStr);
        int maxCount = 0;
        string prePattern = null;
        int prePatternEnd = 0;
        for (int i = 0; i < wholePattern.Length - 1; i++)
          for (int n = 1; n <= 5; n++)
          {
            if (i+n<=wholePattern.Length)
            {
              var partPattern = wholePattern.Substring(i,n);
              if (patternSet.PrePatterns.TryGetValue(partPattern, out var count))
              {
                if (count > maxCount)
                {
                  maxCount = count;
                  prePattern = partPattern;
                  prePatternEnd = i+n;
                }
              }
            }
          }
        return true;
      }
      return false;
    }

    public void Clear()
    {
      patternSets.Clear();
    }

  }
}
