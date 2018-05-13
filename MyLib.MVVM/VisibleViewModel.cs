using System;
using System.Collections.Generic;
using System.Text;

namespace MyLib.MVVM
{
  public class VisibleViewModel : ViewModel, IVisible, IExpandable, ISelectable
  {
    public virtual bool IsVisible
    {
      get => _IsVisible;
      set
      {
        if (_IsVisible!=value)
        {
          _IsVisible = value;
          NotifyPropertyChanged("IsVisible");
        }
      }
    }
    protected bool _IsVisible = false;

    public virtual bool HasRowDetails
    {
      get => _HasRowDetails;
      set
      {
        if (_HasRowDetails!=value)
        {
          _HasRowDetails = value;
          NotifyPropertyChanged("HasRowDetails");
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
          NotifyPropertyChanged("IsExpanded");
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
          NotifyPropertyChanged("IsSelected");
        }
      }
    }
    private bool _IsSelected = false;
  }

}
