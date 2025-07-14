using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;

namespace Qhta.UnicodeBuild.ViewModels;

/// <summary>
/// A collection of writing system mappings, which associates Unicode code point ranges with writing systems.
/// </summary>
public class WritingSystemMappingCollection() : OrderedObservableCollection<WritingSystemMappingViewModel>((item) => item.Range)
{
  /// <summary>
  /// Adds a new writing system mapping to the collection.
  /// </summary>
  /// <param name="sm"></param>
  /// <returns></returns>
  public WritingSystemMappingViewModel Add(WritingSystemMapping sm)
  {
    var sv = new WritingSystemMappingViewModel(sm);
    base.Add(sv);
    return sv;
  }

  /// <summary>
  /// Finds a writing system mapping by its code point.
  /// </summary>
  /// <param name="cp"></param>
  /// <returns></returns>
  public WritingSystemMappingViewModel? FindWritingSystem(int cp)
  {
    return this.FirstOrDefault(item => item.Range.Contains(cp));
  }
}