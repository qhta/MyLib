using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;

namespace Qhta.UnicodeBuild.ViewModels;

/// <summary>
/// Specialized collection for managing Unicode code points.
/// </summary>
public class UcdCodePointsCollection() : OrderedObservableCollection<UcdCodePointViewModel>((item) => item.Id)
{
  /// <summary>
  /// Adds a new <see cref="UcdCodePointViewModel"/> to the collection based on the specified <see
  /// cref="UcdCodePoint"/>.
  /// </summary>
  /// <remarks>This method creates a new <see cref="UcdCodePointViewModel"/> from the provided <see
  /// cref="UcdCodePoint"/> and adds it to the collection. The method also updates an internal dictionary with the new
  /// view model, using the <see cref="UcdCodePoint.Id"/> as the key.</remarks>
  /// <param name="ws">The <see cref="UcdCodePoint"/> instance to be added to the collection.</param>
  /// <returns>The <see cref="UcdCodePointViewModel"/> created from the specified <see cref="UcdCodePoint"/>.</returns>
  public UcdCodePointViewModel Add(UcdCodePoint ws)
  {
    var vm = new UcdCodePointViewModel(ws);
    base.Add(vm);
    IntDictionary[ws.Id] = vm;
    return vm;
  }

  private readonly Dictionary<int, UcdCodePointViewModel> IntDictionary = new Dictionary<int, UcdCodePointViewModel>();

  /// <summary>
  /// Finds a <see cref="UcdCodePointViewModel"/> by its ID.
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  public UcdCodePointViewModel? FindById(int id)
  {
    return IntDictionary.GetValueOrDefault(id);
  }

  /// <summary>
  /// Indicates whether the collection is currently busy with an operation, such as loading or processing data.
  /// </summary>
  public bool IsBusy
  {
    get => _isBusy;
    set
    {
      if (value != _isBusy)
      {
        _isBusy = value;
        base.NotifyPropertyChanged(nameof(IsBusy));
      }
    }
  }
  private bool _isBusy;

  /// <summary>
  /// Progress value indicating the current state of a long-running operation, such as loading or processing data.
  /// </summary>
  public int ProgressValue
  {
    get => _progressValue;
    set
    {
      if (value != _progressValue)
      {
        _progressValue = value;
        base.NotifyPropertyChanged(nameof(ProgressValue));
      }
    }
  }
  private int _progressValue;

  /// <summary>
  /// Status message providing additional information about the current operation or state of the collection.
  /// </summary>
  public string? StatusMessage
  {
    get => _statusMessage;
    set
    {
      _statusMessage = value;
      base.NotifyPropertyChanged(nameof(StatusMessage));
    }
  }
  private string? _statusMessage;

}