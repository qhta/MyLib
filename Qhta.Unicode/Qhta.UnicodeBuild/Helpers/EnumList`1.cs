using System.Security.Cryptography.X509Certificates;

namespace Qhta.UnicodeBuild.Helpers;

public class EnumList<T>: List<EnumItem<T>> where T : struct, Enum
{
  public EnumList()
  {
    foreach (var item in Enum.GetValues(typeof(T)).Cast<T>())
    {
      Add(new EnumItem<T>
      {
        Id = item,
        Name = item.ToString()!
      });
    }
  }

  public static Array EnumValues => Enum.GetValues(typeof(T));
}