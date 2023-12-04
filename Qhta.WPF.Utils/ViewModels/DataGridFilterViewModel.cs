
namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// ViewModel for filter of DataGrid
/// </summary>
public class DataGridFilterViewModel: FilterViewModel, IObjectOwner
{
  /// <summary>
  /// Default constructor
  /// </summary>
  public DataGridFilterViewModel(): base() { }

  /// <summary>
  /// Result of the FilterDialog
  /// </summary>
  public FilterResultOperation DialogResult { get; set; } 

  /// <summary>
  /// Names of columns to select.
  /// </summary>
  public IEnumerable<string>? ColumnNames => Columns?.Select(item=>item.ColumnName);


  /// <inheritdoc/>
  public override FilterViewModel? EditedInstance
  {
    get { return _EditedInstance; }
    set
    {
      if (_EditedInstance != value)
      {
        _EditedInstance = value;
        NotifyPropertyChanged(nameof(EditedInstance));
      }
    }
  }
  private FilterViewModel? _EditedInstance;

  /// <summary>
  /// Returns the Filter component.
  /// </summary>
  public object? GetComponent(string propName)
  {
    if (propName!=nameof(EditedInstance))
      throw new InvalidOperationException($"You can get only {nameof(EditedInstance)} component");
    return EditedInstance;
  }

  /// <summary>
  /// Changes the Filter component to new instance.
  /// </summary>
  public bool ChangeComponent(string propName, object? newComponent)
  {
    if (propName != nameof(EditedInstance))
      throw new InvalidOperationException($"To change and Filter object an old object mus be equal");
    if (newComponent is FilterViewModel newFilter)
      EditedInstance = newFilter;
    else
    if (newComponent == null)
      EditedInstance = null;
    else
      throw new InvalidOperationException($"New Filter object must be a {nameof(FilterViewModel)}");
    return true;
  }

  /// <summary>
  /// Changes the Instance component to new instance.
  /// </summary>
  public bool ChangeComponent(object? oldComponent, object? newComponent)
  {
    //if (oldComponent != Filter)
    //  throw new InvalidOperationException($"To change and Filter object an old object mus be equal");
    if (newComponent is FilterViewModel newFilter)
      EditedInstance = newFilter;
    else
    if (newComponent == null)
      EditedInstance = null;
    else
      throw new InvalidOperationException($"New Filter object must be a {nameof(FilterViewModel)}");
    return true;
  }

  /// <inheritdoc/>
  public override FilterViewModel CreateCopy()
  {
    throw new NotImplementedException();
  }

  /// <inheritdoc/>
  public override IFilter? CreateFilter()
  {
    return EditedInstance?.CreateFilter();
  }

  /// <inheritdoc/>
  public override void ClearFilter()
  {
    throw new NotImplementedException();
  }
}
