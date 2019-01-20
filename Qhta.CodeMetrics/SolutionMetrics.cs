using System.Collections.Generic;

namespace Qhta.CodeMetrics
{
  public class SolutionMetrics: List<ProjectMetrics>
  {
    public new int Capacity { get; }
    public string Name { get; set; }
  }
}
