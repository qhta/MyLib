﻿namespace Qhta.MVVM
{
  /// <summary>
  /// View model which has a model of a specific type.
  /// </summary>
  /// <typeparam name="ModelType"></typeparam>
  public class ViewModel<ModelType>: ViewModel
  {

    /// <summary>
    /// Initializing constructor.
    /// </summary>
    /// <param name="model"></param>
    public ViewModel(ModelType model)
    {
      Model = model;
    }

    /// <summary>
    /// Specific modeled object.
    /// </summary>
    [DataGridColumn(IsAutoGenerated = false)]
    public ModelType Model { get; set; }
  }
}
