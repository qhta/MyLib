namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// View model stored and shown in FilterDialog.
/// </summary>
public abstract class ColumnFilterViewModel : ViewModel
{
  /// <summary>
  ///  Specifies what to do with a column filter.
  /// </summary>
  public ColumnFilterOperation Operation
  {
    get => _Operation;
    set
    {
      if (_Operation != value)
      {
        _Operation = value;
        NotifyOpChanged();
      }
    }
  }
  private ColumnFilterOperation _Operation;

  /// <summary>
  /// Specifies whether Edit operation is enabled. If not, then Add operation is enabled.
  /// </summary>
  public bool EditOpEnabled { get; set; }

  #region Individual boolean properties for Operation used in view.

  /// <summary>
  /// Specifies whether Operation is Add or Edit.
  /// </summary>
  public bool DefaultOp
  {
    get => Operation == ColumnFilterOperation.Add || Operation == ColumnFilterOperation.Edit;
    set
    {
      if (value)
      {
        if (EditOpEnabled)
          Operation = ColumnFilterOperation.Edit;
        else
          Operation = ColumnFilterOperation.Add;
      }
    }
  }


  /// <summary>
  /// Specifies whether Operation is Add.
  /// </summary>
  public bool AddOp { get => Operation == ColumnFilterOperation.Add; set { if (value) Operation = ColumnFilterOperation.Add; } }

  /// <summary>
  /// Specifies whether Operation is Edit.
  /// </summary>
  public bool EditOp { get => Operation == ColumnFilterOperation.Edit; set { if (value) Operation = ColumnFilterOperation.Edit; } }

  /// <summary>
  ///  Specifies whether Operation is Clear.
  /// </summary>
  public bool ClearOp
  {
    get => Operation == ColumnFilterOperation.Clear;
    set
    {
      if (value)
      { 
        Operation = ColumnFilterOperation.Clear;
        ClearFilter();
      }
    }
  }

  private void NotifyOpChanged()
  {
    NotifyPropertyChanged(nameof(Operation));
    NotifyPropertyChanged(nameof(DefaultOp));
    NotifyPropertyChanged(nameof(AddOp));
    NotifyPropertyChanged(nameof(EditOp));
    NotifyPropertyChanged(nameof(ClearOp));
  }
  #endregion

  /// <summary>
  /// Default constructor.
  /// </summary>
  public ColumnFilterViewModel() { }

  /// <summary>
  /// Copying constructor that copies the data of the view model is needed
  /// because we need to edit its copy and after Cancel button is pressed
  /// the previous content remains unchanged.
  /// </summary>
  public ColumnFilterViewModel(ColumnFilterViewModel other)
  {
    EditOpEnabled = true;
    DefaultOp = true;
  }

  /// <summary>
  /// This method must create a copy of the original instance;
  /// </summary>
  /// <returns></returns>
  public abstract ColumnFilterViewModel CreateCopy();

  /// <summary>
  /// Creates Predicate basing on current properties.
  /// </summary>
  /// <returns>Predicate that takes a property value from the object.</returns>
  public abstract ColumnFilter? CreateFilter(PropertyInfo propInfo);

  /// <summary>
  /// Clear filter properties.
  /// </summary>
  public abstract void ClearFilter();
}
