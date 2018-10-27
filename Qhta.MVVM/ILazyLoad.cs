using System.Threading.Tasks;

namespace Qhta.MVVM
{
  public interface ILazyLoad
  {
    bool LoadOnInit { get; set; }
    bool IsLoading { get; }
    bool IsLoaded { get; }

    Task StartLoading();
  }
}
