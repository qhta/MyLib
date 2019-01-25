using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Qhta.CodeMetrics
{
  public class FileSignatureChecker
  {
    public FileSignatureChecker() { }

    public bool ShouldAnalyzeKeywords { get; set; }

    public Dictionary<string, KeywordsSet> KeywordsByLanguage = new Dictionary<string, KeywordsSet>();

    public void CheckInDirectory(string path, string filemask, bool recursive, Dictionary<string, Signature> result)
    {
      if (Directory.Exists(path))
      {
        var files = (filemask==null) ? Directory.GetFiles(path) : Directory.GetFiles(path, filemask);
        foreach (var fname in files)
        {
          //if (filemask!=null)
          //  Debug.WriteLine($"File: {fname}");
          var ext = Path.GetExtension(fname).ToLower();
          Signature signature;
          if (!String.IsNullOrEmpty(ext))
          {
            ext = ext.Substring(1);
            signature = GetSignature(fname);
            if (signature==null)
              continue;
            var sign = signature.Data;
            if (!result.ContainsKey(ext))
            {
              signature.FilesCount=1;
              if (ShouldAnalyzeKeywords && KeywordsByLanguage.TryGetValue(ext, out var keyset))
              {
                signature.Keywords = keyset.Clone();
                CountKeywords(fname, signature.Keywords);
              }
              result.Add(ext, signature);
            }
            else
            {
              signature = result[ext];
              signature.FilesCount+=1;
              var sign0 = signature.Data;
              if (sign0!=null)
              {
                var n = sign0.EqualsCount(sign);
                if (sign==null || n<2)
                  result[ext].Data=null;
                else
                {
                  if (n<sign0.Length)
                    result[ext].Data=sign0.Take(n).ToArray();
                }
              }
              if (ShouldAnalyzeKeywords && signature.Keywords!=null)
                CountKeywords(fname, signature.Keywords);
            }
          }
        }
        if (recursive)
        {
          var directories = Directory.GetDirectories(path);
          foreach (var dirname in directories)
          {
            Debug.WriteLine($"Scanning dir: {dirname}");
            CheckInDirectory(dirname, filemask, recursive, result);
          }
        }
      }
    }

    const int minLength = 8;
    public static Signature GetSignature(string filename)
    {
      var sign = new byte[64];
      using (var stream = File.OpenRead(filename))
      {
        if (stream.Length<minLength)
          return null;
        if (stream.Length<sign.Length)
          sign = new byte[stream.Length];
        stream.Read(sign, 0, sign.Length);
      }

      if (IsText(sign))
      {
        using (var reader = File.OpenText(filename))
        {
          string line = reader.ReadLine();
          int n = 100;
          while (line=="" && --n>0)
            line = reader.ReadLine();
          if (line.StartsWith("<?xml"))
            line = "<?xml";
          var lineBytes = Encoding.UTF8.GetBytes(line);
          if (IsText(lineBytes))
            return new Signature { IsText=true, Data=lineBytes };
        }
      }
      return new Signature { Data=sign };
    }

    public static bool IsText(byte[] sign)
    {
      if (sign[0]==0xEF && sign[1]==0xBB && sign[2]==0xBF)
      { // UTF-8 signature
        return true;
      }
      foreach (var b in sign)
      {
        if (!(b>=32 && b<127 || b=='\t' || b=='\r' || b=='\n'))
          return false;
      }
      return true;
    }

    public void LoadKeywords(string ext, string filename)
    {
      var keyset = LoadKeywords(filename);
      KeywordsByLanguage.Add(ext, keyset);
    }

    public KeywordsSet LoadKeywords(string filename)
    {
      using (var reader = File.OpenText(filename))
      {
        var str = reader.ReadToEnd();
        var keywords = str.Split('\n');
        var keyset = new KeywordsSet();
        foreach (var s in keywords.Select(item => item.Trim()))
          keyset.Add(s, 0);
        return keyset;
      }
    }

    public void CountKeywords (string filename, KeywordsSet keywords)
    {
      using (var reader = File.OpenText(filename))
      {
        var str = reader.ReadToEnd();
        foreach (var key in keywords.Keys.ToList())
        {
          int n = 0;
          int k = str.IndexOf(key);
          while (k>0)
          {
            n++;
            if (k<str.Length-1)
              k = str.IndexOf(key, k+1);
            else
              break;
          }
          keywords[key] +=n;
        }
      }
    }
  }
}

