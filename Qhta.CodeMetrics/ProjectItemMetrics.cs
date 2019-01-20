namespace Qhta.CodeMetrics
{
  public class ProjectItemMetrics
  {
    public string Name { get; set; }
    public LineMetrics Lines { get; set; } = new LineMetrics();
  }
}
