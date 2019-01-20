using System.Collections.Generic;

namespace Qhta.CodeMetrics
{
  public class ProjectMetrics:List<ProjectItemMetrics>
  {
    public new int Capacity { get; }

    public string Name { get; set; }
  }
}
