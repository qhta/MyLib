using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace TestMultiSelectListView
{
  public class ListItemViewModel : INotifyPropertyChanged
  {
    public ListItemViewModel(string displayName)
    {
      DisplayName = displayName;
    }

    private bool _isSelected;

    public string DisplayName { get; set; }

    public virtual bool IsSelected
    {
      get { return _isSelected; }
      set
      {
        _isSelected = value;
        OnPropertyChanged(() => IsSelected);
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
    {
      var body = propertyExpression.Body as MemberExpression;
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(body.Member.Name));
    }

    public override string ToString()
    {
      return $"ListItemViewModel {DisplayName}";
    }
  }
}