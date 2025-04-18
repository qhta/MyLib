namespace Qhta.UnicodeBuild.Helpers;

public record EnumItem<T> where T : struct, Enum
{
  public T Id { get; set; }
  public string Name { get; set; } = null!;
}