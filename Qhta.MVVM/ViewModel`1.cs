namespace Qhta.MVVM
{
  /// <summary>
  /// View model which has a model of a specific type.
  /// </summary>
  /// <typeparam name="ModelType"></typeparam>
  public class ViewModel<ModelType>: ViewModel
  {
    /// <summary>
    /// Specific modeled object.
    /// </summary>
    public ModelType? Model { get; set; }
  }
}
