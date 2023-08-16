namespace MS.Internal.Data
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Globalization;
  using System.Runtime.CompilerServices;
  using System.Threading;
  using System.Windows;
  using System.Windows.Data;

  internal class CollectionViewProxy : CollectionView, IEditableCollectionViewAddNewItem, IEditableCollectionView, ICollectionViewLiveShaping, IItemProperties
  {
    private IndexedEnumerable _indexer;
    private ObservableCollection<string> _liveFilteringProperties;
    private ObservableCollection<string> _liveGroupingProperties;
    private ObservableCollection<string> _liveSortingProperties;
    private ICollectionView _view;

    public event EventHandler CurrentChanged
    {
      add
      {
        this.PrivateCurrentChanged += value;
      }
      remove
      {
        this.PrivateCurrentChanged -= value;
      }
    }

    public event CurrentChangingEventHandler CurrentChanging
    {
      add
      {
        this.PrivateCurrentChanging += value;
      }
      remove
      {
        this.PrivateCurrentChanging -= value;
      }
    }

    [field: CompilerGenerated]
    private event EventHandler PrivateCurrentChanged;

    [field: CompilerGenerated]
    private event CurrentChangingEventHandler PrivateCurrentChanging;

    internal CollectionViewProxy(ICollectionView view) : base(view.SourceCollection)
    {
      this._view = view;
      view.CollectionChanged += new NotifyCollectionChangedEventHandler(this._OnViewChanged);
      view.CurrentChanging += new CurrentChangingEventHandler(this._OnCurrentChanging);
      view.CurrentChanged += new EventHandler(this._OnCurrentChanged);
      INotifyPropertyChanged changed = view as INotifyPropertyChanged;
      if (changed != null)
      {
        changed.PropertyChanged += new PropertyChangedEventHandler(this._OnPropertyChanged);
      }
    }

    private void _OnCurrentChanged(object sender, EventArgs args)
    {
      if (this.PrivateCurrentChanged != null)
      {
        this.PrivateCurrentChanged(this, args);
      }
    }

    private void _OnCurrentChanging(object sender, CurrentChangingEventArgs args)
    {
      if (this.PrivateCurrentChanging != null)
      {
        this.PrivateCurrentChanging(this, args);
      }
    }

    private void _OnPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
      this.OnPropertyChanged(args);
    }

    private void _OnViewChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      this.OnCollectionChanged(args);
    }

    public override bool Contains(object item)
    {
      return this.ProxiedView.Contains(item);
    }

    public override IDisposable DeferRefresh()
    {
      return this.ProxiedView.DeferRefresh();
    }

    public override void DetachFromSourceCollection()
    {
      if (this._view != null)
      {
        this._view.CollectionChanged -= new NotifyCollectionChangedEventHandler(this._OnViewChanged);
        this._view.CurrentChanging -= new CurrentChangingEventHandler(this._OnCurrentChanging);
        this._view.CurrentChanged -= new EventHandler(this._OnCurrentChanged);
        INotifyPropertyChanged changed = this._view as INotifyPropertyChanged;
        if (changed != null)
        {
          changed.PropertyChanged -= new PropertyChangedEventHandler(this._OnPropertyChanged);
        }
        this._view = null;
      }
      base.DetachFromSourceCollection();
    }

    internal void GetCollectionChangedSources(int level, Action<int, object, bool?, List<string>> format, List<string> sources)
    {
      format(level, this, false, sources);
      if (this._view != null)
      {
        format(level + 1, this._view, true, sources);
        object sourceCollection = this._view.SourceCollection;
        if (sourceCollection != null)
        {
          format(level + 2, sourceCollection, null, sources);
        }
      }
    }

    protected override IEnumerator GetEnumerator()
    {
      return this.ProxiedView.GetEnumerator();
    }

    public override object GetItemAt(int index)
    {
      if (index < 0)
      {
        throw new ArgumentOutOfRangeException("index");
      }
      return this.EnumerableWrapper[index];
    }

    public override int IndexOf(object item)
    {
      return this.EnumerableWrapper.IndexOf(item);
    }

    public override bool MoveCurrentTo(object item)
    {
      return this.ProxiedView.MoveCurrentTo(item);
    }

    public override bool MoveCurrentToFirst()
    {
      return this.ProxiedView.MoveCurrentToFirst();
    }

    public override bool MoveCurrentToLast()
    {
      return this.ProxiedView.MoveCurrentToLast();
    }

    public override bool MoveCurrentToNext()
    {
      return this.ProxiedView.MoveCurrentToNext();
    }

    public override bool MoveCurrentToPosition(int position)
    {
      return this.ProxiedView.MoveCurrentToPosition(position);
    }

    public override bool MoveCurrentToPrevious()
    {
      return this.ProxiedView.MoveCurrentToPrevious();
    }

    public override bool PassesFilter(object item)
    {
      if ((this.ProxiedView.CanFilter && (this.ProxiedView.Filter != null)) && ((item != CollectionView.NewItemPlaceholder) && (item != ((IEditableCollectionView)this).CurrentAddItem)))
      {
        return this.ProxiedView.Filter(item);
      }
      return true;
    }

    public override void Refresh()
    {
      IndexedEnumerable enumerable = Interlocked.Exchange<IndexedEnumerable>(ref this._indexer, null);
      if (enumerable != null)
      {
        enumerable.Invalidate();
      }
      this.ProxiedView.Refresh();
    }

    object IEditableCollectionView.AddNew()
    {
      IEditableCollectionView proxiedView = this.ProxiedView as IEditableCollectionView;
      if (proxiedView != null)
      {
        return proxiedView.AddNew();
      }
      object[] args = new object[] { "AddNew" };
      throw new InvalidOperationException(System.Windows.SR.Get("MemberNotAllowedForView", args));
    }

    void IEditableCollectionView.CancelEdit()
    {
      IEditableCollectionView proxiedView = this.ProxiedView as IEditableCollectionView;
      if (proxiedView != null)
      {
        proxiedView.CancelEdit();
      }
      else
      {
        object[] args = new object[] { "CancelEdit" };
        throw new InvalidOperationException(System.Windows.SR.Get("MemberNotAllowedForView", args));
      }
    }

    void IEditableCollectionView.CancelNew()
    {
      IEditableCollectionView proxiedView = this.ProxiedView as IEditableCollectionView;
      if (proxiedView != null)
      {
        proxiedView.CancelNew();
      }
      else
      {
        object[] args = new object[] { "CancelNew" };
        throw new InvalidOperationException(System.Windows.SR.Get("MemberNotAllowedForView", args));
      }
    }

    void IEditableCollectionView.CommitEdit()
    {
      IEditableCollectionView proxiedView = this.ProxiedView as IEditableCollectionView;
      if (proxiedView != null)
      {
        proxiedView.CommitEdit();
      }
      else
      {
        object[] args = new object[] { "CommitEdit" };
        throw new InvalidOperationException(System.Windows.SR.Get("MemberNotAllowedForView", args));
      }
    }

    void IEditableCollectionView.CommitNew()
    {
      IEditableCollectionView proxiedView = this.ProxiedView as IEditableCollectionView;
      if (proxiedView != null)
      {
        proxiedView.CommitNew();
      }
      else
      {
        object[] args = new object[] { "CommitNew" };
        throw new InvalidOperationException(System.Windows.SR.Get("MemberNotAllowedForView", args));
      }
    }

    void IEditableCollectionView.EditItem(object item)
    {
      IEditableCollectionView proxiedView = this.ProxiedView as IEditableCollectionView;
      if (proxiedView != null)
      {
        proxiedView.EditItem(item);
      }
      else
      {
        object[] args = new object[] { "EditItem" };
        throw new InvalidOperationException(System.Windows.SR.Get("MemberNotAllowedForView", args));
      }
    }

    void IEditableCollectionView.Remove(object item)
    {
      IEditableCollectionView proxiedView = this.ProxiedView as IEditableCollectionView;
      if (proxiedView != null)
      {
        proxiedView.Remove(item);
      }
      else
      {
        object[] args = new object[] { "Remove" };
        throw new InvalidOperationException(System.Windows.SR.Get("MemberNotAllowedForView", args));
      }
    }

    void IEditableCollectionView.RemoveAt(int index)
    {
      IEditableCollectionView proxiedView = this.ProxiedView as IEditableCollectionView;
      if (proxiedView != null)
      {
        proxiedView.RemoveAt(index);
      }
      else
      {
        object[] args = new object[] { "RemoveAt" };
        throw new InvalidOperationException(System.Windows.SR.Get("MemberNotAllowedForView", args));
      }
    }

    object IEditableCollectionViewAddNewItem.AddNewItem(object newItem)
    {
      IEditableCollectionViewAddNewItem proxiedView = this.ProxiedView as IEditableCollectionViewAddNewItem;
      if (proxiedView != null)
      {
        return proxiedView.AddNewItem(newItem);
      }
      object[] args = new object[] { "AddNewItem" };
      throw new InvalidOperationException(System.Windows.SR.Get("MemberNotAllowedForView", args));
    }

    public override bool CanFilter
    {
      get
      {
        return this.ProxiedView.CanFilter;
      }
    }

    public override bool CanGroup
    {
      get
      {
        return this.ProxiedView.CanGroup;
      }
    }

    public override bool CanSort
    {
      get
      {
        return this.ProxiedView.CanSort;
      }
    }

    public override int Count
    {
      get
      {
        return this.EnumerableWrapper.Count;
      }
    }

    public override CultureInfo Culture
    {
      get
      {
        return this.ProxiedView.Culture;
      }
      set
      {
        this.ProxiedView.Culture = value;
      }
    }

    public override object CurrentItem
    {
      get
      {
        return this.ProxiedView.CurrentItem;
      }
    }

    public override int CurrentPosition
    {
      get
      {
        return this.ProxiedView.CurrentPosition;
      }
    }

    private IndexedEnumerable EnumerableWrapper
    {
      get
      {
        if (this._indexer == null)
        {
          IndexedEnumerable enumerable = new IndexedEnumerable(this.ProxiedView, new Predicate<object>(this.PassesFilter));
          Interlocked.CompareExchange<IndexedEnumerable>(ref this._indexer, enumerable, null);
        }
        return this._indexer;
      }
    }

    public override Predicate<object> Filter
    {
      get
      {
        return this.ProxiedView.Filter;
      }
      set
      {
        this.ProxiedView.Filter = value;
      }
    }

    public override ObservableCollection<GroupDescription> GroupDescriptions
    {
      get
      {
        return this.ProxiedView.GroupDescriptions;
      }
    }

    public override ReadOnlyObservableCollection<object> Groups
    {
      get
      {
        return this.ProxiedView.Groups;
      }
    }

    public override bool IsCurrentAfterLast
    {
      get
      {
        return this.ProxiedView.IsCurrentAfterLast;
      }
    }

    public override bool IsCurrentBeforeFirst
    {
      get
      {
        return this.ProxiedView.IsCurrentBeforeFirst;
      }
    }

    public override bool IsEmpty
    {
      get
      {
        return this.ProxiedView.IsEmpty;
      }
    }

    public ICollectionView ProxiedView
    {
      get
      {
        return this._view;
      }
    }

    public override SortDescriptionCollection SortDescriptions
    {
      get
      {
        return this.ProxiedView.SortDescriptions;
      }
    }

    public override IEnumerable SourceCollection
    {
      get
      {
        return base.SourceCollection;
      }
    }

    bool ICollectionViewLiveShaping.CanChangeLiveFiltering
    {
      get
      {
        ICollectionViewLiveShaping proxiedView = this.ProxiedView as ICollectionViewLiveShaping;
        if (proxiedView == null)
        {
          return false;
        }
        return proxiedView.CanChangeLiveFiltering;
      }
    }

    bool ICollectionViewLiveShaping.CanChangeLiveGrouping
    {
      get
      {
        ICollectionViewLiveShaping proxiedView = this.ProxiedView as ICollectionViewLiveShaping;
        if (proxiedView == null)
        {
          return false;
        }
        return proxiedView.CanChangeLiveGrouping;
      }
    }

    bool ICollectionViewLiveShaping.CanChangeLiveSorting
    {
      get
      {
        ICollectionViewLiveShaping proxiedView = this.ProxiedView as ICollectionViewLiveShaping;
        if (proxiedView == null)
        {
          return false;
        }
        return proxiedView.CanChangeLiveSorting;
      }
    }

    bool? ICollectionViewLiveShaping.IsLiveFiltering
    {
      get
      {
        ICollectionViewLiveShaping proxiedView = this.ProxiedView as ICollectionViewLiveShaping;
        if (proxiedView == null)
        {
          return null;
        }
        return proxiedView.IsLiveFiltering;
      }
      set
      {
        ICollectionViewLiveShaping proxiedView = this.ProxiedView as ICollectionViewLiveShaping;
        if (proxiedView != null)
        {
          proxiedView.IsLiveFiltering = value;
        }
        else
        {
          object[] args = new object[] { "IsLiveFiltering", "CanChangeLiveFiltering" };
          throw new InvalidOperationException(System.Windows.SR.Get("CannotChangeLiveShaping", args));
        }
      }
    }

    bool? ICollectionViewLiveShaping.IsLiveGrouping
    {
      get
      {
        ICollectionViewLiveShaping proxiedView = this.ProxiedView as ICollectionViewLiveShaping;
        if (proxiedView == null)
        {
          return null;
        }
        return proxiedView.IsLiveGrouping;
      }
      set
      {
        ICollectionViewLiveShaping proxiedView = this.ProxiedView as ICollectionViewLiveShaping;
        if (proxiedView != null)
        {
          proxiedView.IsLiveGrouping = value;
        }
        else
        {
          object[] args = new object[] { "IsLiveGrouping", "CanChangeLiveGrouping" };
          throw new InvalidOperationException(System.Windows.SR.Get("CannotChangeLiveShaping", args));
        }
      }
    }

    bool? ICollectionViewLiveShaping.IsLiveSorting
    {
      get
      {
        ICollectionViewLiveShaping proxiedView = this.ProxiedView as ICollectionViewLiveShaping;
        if (proxiedView == null)
        {
          return null;
        }
        return proxiedView.IsLiveSorting;
      }
      set
      {
        ICollectionViewLiveShaping proxiedView = this.ProxiedView as ICollectionViewLiveShaping;
        if (proxiedView != null)
        {
          proxiedView.IsLiveSorting = value;
        }
        else
        {
          object[] args = new object[] { "IsLiveSorting", "CanChangeLiveSorting" };
          throw new InvalidOperationException(System.Windows.SR.Get("CannotChangeLiveShaping", args));
        }
      }
    }

    ObservableCollection<string> ICollectionViewLiveShaping.LiveFilteringProperties
    {
      get
      {
        ICollectionViewLiveShaping proxiedView = this.ProxiedView as ICollectionViewLiveShaping;
        if (proxiedView != null)
        {
          return proxiedView.LiveFilteringProperties;
        }
        if (this._liveFilteringProperties == null)
        {
          this._liveFilteringProperties = new ObservableCollection<string>();
        }
        return this._liveFilteringProperties;
      }
    }

    ObservableCollection<string> ICollectionViewLiveShaping.LiveGroupingProperties
    {
      get
      {
        ICollectionViewLiveShaping proxiedView = this.ProxiedView as ICollectionViewLiveShaping;
        if (proxiedView != null)
        {
          return proxiedView.LiveGroupingProperties;
        }
        if (this._liveGroupingProperties == null)
        {
          this._liveGroupingProperties = new ObservableCollection<string>();
        }
        return this._liveGroupingProperties;
      }
    }

    ObservableCollection<string> ICollectionViewLiveShaping.LiveSortingProperties
    {
      get
      {
        ICollectionViewLiveShaping proxiedView = this.ProxiedView as ICollectionViewLiveShaping;
        if (proxiedView != null)
        {
          return proxiedView.LiveSortingProperties;
        }
        if (this._liveSortingProperties == null)
        {
          this._liveSortingProperties = new ObservableCollection<string>();
        }
        return this._liveSortingProperties;
      }
    }

    bool IEditableCollectionView.CanAddNew
    {
      get
      {
        IEditableCollectionView proxiedView = this.ProxiedView as IEditableCollectionView;
        return ((proxiedView != null) && proxiedView.CanAddNew);
      }
    }

    bool IEditableCollectionView.CanCancelEdit
    {
      get
      {
        IEditableCollectionView proxiedView = this.ProxiedView as IEditableCollectionView;
        return ((proxiedView != null) && proxiedView.CanCancelEdit);
      }
    }

    bool IEditableCollectionView.CanRemove
    {
      get
      {
        IEditableCollectionView proxiedView = this.ProxiedView as IEditableCollectionView;
        return ((proxiedView != null) && proxiedView.CanRemove);
      }
    }

    object IEditableCollectionView.CurrentAddItem
    {
      get
      {
        IEditableCollectionView proxiedView = this.ProxiedView as IEditableCollectionView;
        if (proxiedView != null)
        {
          return proxiedView.CurrentAddItem;
        }
        return null;
      }
    }

    object IEditableCollectionView.CurrentEditItem
    {
      get
      {
        IEditableCollectionView proxiedView = this.ProxiedView as IEditableCollectionView;
        if (proxiedView != null)
        {
          return proxiedView.CurrentEditItem;
        }
        return null;
      }
    }

    bool IEditableCollectionView.IsAddingNew
    {
      get
      {
        IEditableCollectionView proxiedView = this.ProxiedView as IEditableCollectionView;
        return ((proxiedView != null) && proxiedView.IsAddingNew);
      }
    }

    bool IEditableCollectionView.IsEditingItem
    {
      get
      {
        IEditableCollectionView proxiedView = this.ProxiedView as IEditableCollectionView;
        return ((proxiedView != null) && proxiedView.IsEditingItem);
      }
    }

    NewItemPlaceholderPosition IEditableCollectionView.NewItemPlaceholderPosition
    {
      get
      {
        IEditableCollectionView proxiedView = this.ProxiedView as IEditableCollectionView;
        if (proxiedView != null)
        {
          return proxiedView.NewItemPlaceholderPosition;
        }
        return NewItemPlaceholderPosition.None;
      }
      set
      {
        IEditableCollectionView proxiedView = this.ProxiedView as IEditableCollectionView;
        if (proxiedView != null)
        {
          proxiedView.NewItemPlaceholderPosition = value;
        }
        else
        {
          object[] args = new object[] { "NewItemPlaceholderPosition" };
          throw new InvalidOperationException(System.Windows.SR.Get("MemberNotAllowedForView", args));
        }
      }
    }

    bool IEditableCollectionViewAddNewItem.CanAddNewItem
    {
      get
      {
        IEditableCollectionViewAddNewItem proxiedView = this.ProxiedView as IEditableCollectionViewAddNewItem;
        return ((proxiedView != null) && proxiedView.CanAddNewItem);
      }
    }

    ReadOnlyCollection<ItemPropertyInfo> IItemProperties.ItemProperties
    {
      get
      {
        IItemProperties proxiedView = this.ProxiedView as IItemProperties;
        if (proxiedView != null)
        {
          return proxiedView.ItemProperties;
        }
        return null;
      }
    }
  }
}
