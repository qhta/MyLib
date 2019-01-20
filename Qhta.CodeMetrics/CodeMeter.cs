using System;
using System.IO;
using System.Linq;
using EnvDTE;

namespace Qhta.CodeMetrics
{
  public class CodeMeter
  {
    public int CountLinesInSolution(Solution solution, out SolutionMetrics solutionMetrics)
    {
      solutionMetrics = new SolutionMetrics { Name = solution.FullName };
      int count = 0;
      foreach (Project project in solution.Projects)
      {
        count += CountLinesInProject(project, out var projectMetrics);
        solutionMetrics.Add(projectMetrics);
      }
      return count;
    }

    public int CountLinesInProject(Project project, out ProjectMetrics projectMetrics)
    {
      projectMetrics = new ProjectMetrics { Name = project.Name };
      int count = 0;
      foreach (ProjectItem projectItem in project.ProjectItems)
      {
        if (projectItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile)
        {
          ProjectItemMetrics metrics = new ProjectItemMetrics { Name = projectItem.Name };

          for (short i = 0; i < projectItem.FileCount; i++)
          {
            var filename = projectItem.FileNames[i];
            count +=CountLinesInFile(filename, metrics.Lines);
          }
          if (metrics.Lines.TotalLines>0)
          {
            projectMetrics.Add(metrics);
          }
        }
      }
      return count;
    }

    public int CountLinesInFile(string filename, out LineMetrics metrics)
    {
      metrics = new LineMetrics();
      return CountLinesInFile(filename, metrics);
    }

    public int CountLinesInFile(string filename, LineMetrics metrics)
    {
      var ext = Path.GetExtension(filename).ToLower();
      //Debug.WriteLine(ext);
      if (ext==".cs" || ext==".cpp" || ext==".h")
      {
        //Debug.WriteLine(filename);
        using (TextReader reader = File.OpenText(filename))
        {
          var str = reader.ReadToEnd();
          var lines = str.Split(new char[] { '\n' });
          metrics.TotalLines = lines.Count();
          bool inComment = false;
          Int64 meaningfulLinesLength = 0;
          foreach (var line in lines.Select(item => item.Trim()))
          {
            if (String.IsNullOrEmpty(line))
              metrics.EmptyLines+=1;
            else
            {
              for (int i=0; i<line.Length; i++)
              {
                char ch = line[i];
                char ch1;
                if (inComment)
                {
                  for (; i<line.Length-1; i++)
                  {
                    ch = line[i];
                    ch1 = line[i+1];
                    if (ch=='*' && ch1=='/')
                    {
                      inComment = false;
                      if (i==line.Length-2)
                        metrics.CommentLines += 1;
                      break;
                    }
                  }
                }
                else
                if (Char.IsLetterOrDigit(ch))
                {
                  metrics.MeaningfulLines += 1;
                  meaningfulLinesLength += line.Length;
                  break;
                }
                else if (ch=='/' && i<line.Length-1)
                {
                  ch1 = line[++i];
                  if (ch1=='/')
                  {
                    metrics.CommentLines += 1;
                    break;
                  }
                  else 
                  if (ch1=='*')
                  {
                    inComment = true;
                    for (i+=1; i<line.Length-1; i++)
                    {
                      ch = line[i];
                      ch1 = line[i+1];
                      if (ch=='*' && ch1=='/')
                      {
                        inComment = false;
                        break;
                      }
                    }
                  }
                }
              }
              if (inComment)
                metrics.CommentLines += 1;
            }
          }
          metrics.MeanLineLength = (int)(meaningfulLinesLength / metrics.MeaningfulLines);
        }
      }
      return metrics.MeaningfulLines;
    }

    //public bool IsTextFile(string filename)
    //{

    //}
  }
}
