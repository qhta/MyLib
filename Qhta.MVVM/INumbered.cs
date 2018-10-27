using Qhta.OrdNumbers;

namespace Qhta.MVVM
{
  public interface INumbered
  {
    int Number { get; set; }

    OrdNum OrdNumber { get; }
  }
}
