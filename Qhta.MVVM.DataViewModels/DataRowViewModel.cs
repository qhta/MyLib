namespace Qhta.MVVM
{
  public abstract class DataRowViewModel: ViewModel, IExpandable, ISelectable
  {
    public abstract DataColumnViewModel[] GetColumns();
    public abstract string GetText(int col);

    public virtual bool HasRowDetails
    {
      get => _HasRowDetails;
      set
      {
        if (_HasRowDetails!=value)
        {
          _HasRowDetails = value;
          NotifyPropertyChanged(nameof(HasRowDetails));
        }
      }
    }
    protected bool _HasRowDetails = false;

    public virtual bool IsExpanded
    {
      get => _IsExpanded;
      set
      {
        if (_IsExpanded!=value)
        {
          _IsExpanded = value;
          NotifyPropertyChanged(nameof(IsExpanded));
        }
      }
    }
    protected bool _IsExpanded = false;

    public bool IsSelected
    {
      get => _IsSelected;
      set
      {
        if (_IsSelected!=value)
        {
          _IsSelected = value;
          NotifyPropertyChanged(nameof(IsSelected));
        }
      }
    }
    private bool _IsSelected = false;
  }
}
