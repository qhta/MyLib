namespace System.Windows.Data
{
    using MS.Internal;
    using MS.Internal.Data;
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Markup;

    public class CollectionViewSource : DependencyObject, ISupportInitialize, IWeakEventListener
    {
        private CultureInfo _culture;
        private DataSourceProvider _dataProvider;
        private int _deferLevel;
        private FilterStub _filterStub;
        private ObservableCollection<GroupDescription> _groupBy;
        private bool _hasMultipleInheritanceContexts;
        private DependencyObject _inheritanceContext;
        private bool _isInitializing;
        private bool _isViewInitialized;
        private ObservableCollection<string> _liveFilteringProperties;
        private ObservableCollection<string> _liveGroupingProperties;
        private ObservableCollection<string> _liveSortingProperties;
        private DependencyProperty _propertyForInheritanceContext;
        private SortDescriptionCollection _sort = new SortDescriptionCollection();
        private int _version;
        public static readonly DependencyProperty CanChangeLiveFilteringProperty = CanChangeLiveFilteringPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey CanChangeLiveFilteringPropertyKey = DependencyProperty.RegisterReadOnly("CanChangeLiveFiltering", typeof(bool), typeof(CollectionViewSource), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty CanChangeLiveGroupingProperty = CanChangeLiveGroupingPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey CanChangeLiveGroupingPropertyKey = DependencyProperty.RegisterReadOnly("CanChangeLiveGrouping", typeof(bool), typeof(CollectionViewSource), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty CanChangeLiveSortingProperty = CanChangeLiveSortingPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey CanChangeLiveSortingPropertyKey = DependencyProperty.RegisterReadOnly("CanChangeLiveSorting", typeof(bool), typeof(CollectionViewSource), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty CollectionViewTypeProperty = DependencyProperty.Register("CollectionViewType", typeof(Type), typeof(CollectionViewSource), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(CollectionViewSource.OnCollectionViewTypeChanged)), new ValidateValueCallback(CollectionViewSource.IsCollectionViewTypeValid));
        internal static readonly CollectionViewSource DefaultSource = new CollectionViewSource();
        //private static readonly UncommonField<FilterEventHandler> FilterHandlersField = new UncommonField<FilterEventHandler>();
        public static readonly DependencyProperty IsLiveFilteringProperty = IsLiveFilteringPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey IsLiveFilteringPropertyKey = DependencyProperty.RegisterReadOnly("IsLiveFiltering", typeof(bool?), typeof(CollectionViewSource), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty IsLiveFilteringRequestedProperty = DependencyProperty.Register("IsLiveFilteringRequested", typeof(bool), typeof(CollectionViewSource), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(CollectionViewSource.OnIsLiveFilteringRequestedChanged)));
        public static readonly DependencyProperty IsLiveGroupingProperty = IsLiveGroupingPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey IsLiveGroupingPropertyKey = DependencyProperty.RegisterReadOnly("IsLiveGrouping", typeof(bool?), typeof(CollectionViewSource), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty IsLiveGroupingRequestedProperty = DependencyProperty.Register("IsLiveGroupingRequested", typeof(bool), typeof(CollectionViewSource), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(CollectionViewSource.OnIsLiveGroupingRequestedChanged)));
        public static readonly DependencyProperty IsLiveSortingProperty = IsLiveSortingPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey IsLiveSortingPropertyKey = DependencyProperty.RegisterReadOnly("IsLiveSorting", typeof(bool?), typeof(CollectionViewSource), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty IsLiveSortingRequestedProperty = DependencyProperty.Register("IsLiveSortingRequested", typeof(bool), typeof(CollectionViewSource), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(CollectionViewSource.OnIsLiveSortingRequestedChanged)));
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(object), typeof(CollectionViewSource), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(CollectionViewSource.OnSourceChanged)), new ValidateValueCallback(CollectionViewSource.IsSourceValid));
        public static readonly DependencyProperty ViewProperty = ViewPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey ViewPropertyKey = DependencyProperty.RegisterReadOnly("View", typeof(ICollectionView), typeof(CollectionViewSource), new FrameworkPropertyMetadata(null));

        public event FilterEventHandler Filter
        {
            add
            {
                FilterEventHandler a = FilterHandlersField.GetValue(this);
                if (a != null)
                {
                    a = (FilterEventHandler) Delegate.Combine(a, value);
                }
                else
                {
                    a = value;
                }
                FilterHandlersField.SetValue(this, a);
                this.OnForwardedPropertyChanged();
            }
            remove
            {
                FilterEventHandler source = FilterHandlersField.GetValue(this);
                if (source != null)
                {
                    source = (FilterEventHandler) Delegate.Remove(source, value);
                    if (source == null)
                    {
                        FilterHandlersField.ClearValue(this);
                    }
                    else
                    {
                        FilterHandlersField.SetValue(this, source);
                    }
                }
                this.OnForwardedPropertyChanged();
            }
        }

        public CollectionViewSource()
        {
            this._sort.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnForwardedCollectionChanged);
            this._groupBy = new ObservableCollection<GroupDescription>();
            this._groupBy.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnForwardedCollectionChanged);
        }

        internal override void AddInheritanceContext(DependencyObject context, DependencyProperty property)
        {
            if (!this._hasMultipleInheritanceContexts && (this._inheritanceContext == null))
            {
                this._propertyForInheritanceContext = property;
            }
            else
            {
                this._propertyForInheritanceContext = null;
            }
            InheritanceContextHelper.AddInheritanceContext(context, this, ref this._hasMultipleInheritanceContexts, ref this._inheritanceContext);
        }

        private void ApplyPropertiesToView(ICollectionView view)
        {
            if ((view != null) && (this._deferLevel <= 0))
            {
                ICollectionViewLiveShaping shaping = view as ICollectionViewLiveShaping;
                using (view.DeferRefresh())
                {
                    int num;
                    int count;
                    Predicate<object> filterWrapper;
                    if (this.Culture != null)
                    {
                        view.Culture = this.Culture;
                    }
                    if (view.CanSort)
                    {
                        view.SortDescriptions.Clear();
                        num = 0;
                        count = this.SortDescriptions.Count;
                        while (num < count)
                        {
                            view.SortDescriptions.Add(this.SortDescriptions[num]);
                            num++;
                        }
                    }
                    else if (this.SortDescriptions.Count > 0)
                    {
                        object[] args = new object[] { view };
                        throw new InvalidOperationException(System.Windows.SR.Get("CannotSortView", args));
                    }
                    if (FilterHandlersField.GetValue(this) != null)
                    {
                        filterWrapper = this.FilterWrapper;
                    }
                    else
                    {
                        filterWrapper = null;
                    }
                    if (view.CanFilter)
                    {
                        view.Filter = filterWrapper;
                    }
                    else if (filterWrapper != null)
                    {
                        object[] objArray2 = new object[] { view };
                        throw new InvalidOperationException(System.Windows.SR.Get("CannotFilterView", objArray2));
                    }
                    if (view.CanGroup)
                    {
                        view.GroupDescriptions.Clear();
                        num = 0;
                        count = this.GroupDescriptions.Count;
                        while (num < count)
                        {
                            view.GroupDescriptions.Add(this.GroupDescriptions[num]);
                            num++;
                        }
                    }
                    else if (this.GroupDescriptions.Count > 0)
                    {
                        object[] objArray3 = new object[] { view };
                        throw new InvalidOperationException(System.Windows.SR.Get("CannotGroupView", objArray3));
                    }
                    if (shaping != null)
                    {
                        ObservableCollection<string> liveSortingProperties;
                        if (shaping.CanChangeLiveSorting)
                        {
                            shaping.IsLiveSorting = new bool?(this.IsLiveSortingRequested);
                            liveSortingProperties = shaping.LiveSortingProperties;
                            liveSortingProperties.Clear();
                            if (this.IsLiveSortingRequested)
                            {
                                foreach (string str in this.LiveSortingProperties)
                                {
                                    liveSortingProperties.Add(str);
                                }
                            }
                        }
                        this.CanChangeLiveSorting = shaping.CanChangeLiveSorting;
                        this.IsLiveSorting = shaping.IsLiveSorting;
                        if (shaping.CanChangeLiveFiltering)
                        {
                            shaping.IsLiveFiltering = new bool?(this.IsLiveFilteringRequested);
                            liveSortingProperties = shaping.LiveFilteringProperties;
                            liveSortingProperties.Clear();
                            if (this.IsLiveFilteringRequested)
                            {
                                foreach (string str2 in this.LiveFilteringProperties)
                                {
                                    liveSortingProperties.Add(str2);
                                }
                            }
                        }
                        this.CanChangeLiveFiltering = shaping.CanChangeLiveFiltering;
                        this.IsLiveFiltering = shaping.IsLiveFiltering;
                        if (shaping.CanChangeLiveGrouping)
                        {
                            shaping.IsLiveGrouping = new bool?(this.IsLiveGroupingRequested);
                            liveSortingProperties = shaping.LiveGroupingProperties;
                            liveSortingProperties.Clear();
                            if (this.IsLiveGroupingRequested)
                            {
                                foreach (string str3 in this.LiveGroupingProperties)
                                {
                                    liveSortingProperties.Add(str3);
                                }
                            }
                        }
                        this.CanChangeLiveGrouping = shaping.CanChangeLiveGrouping;
                        this.IsLiveGrouping = shaping.IsLiveGrouping;
                    }
                    else
                    {
                        this.CanChangeLiveSorting = false;
                        bool? nullable = null;
                        this.IsLiveSorting = nullable;
                        this.CanChangeLiveFiltering = false;
                        nullable = null;
                        this.IsLiveFiltering = nullable;
                        this.CanChangeLiveGrouping = false;
                        this.IsLiveGrouping = null;
                    }
                }
            }
        }

        private void BeginDefer()
        {
            this._deferLevel++;
        }

        public IDisposable DeferRefresh()
        {
            return new DeferHelper(this);
        }

        private void EndDefer()
        {
            int num = this._deferLevel - 1;
            this._deferLevel = num;
            if (num == 0)
            {
                this.EnsureView();
            }
        }

        private void EnsureView()
        {
            this.EnsureView(this.Source, this.CollectionViewType);
        }

        private void EnsureView(object source, Type collectionViewType)
        {
            if (!this._isInitializing && (this._deferLevel <= 0))
            {
                DataSourceProvider provider = source as DataSourceProvider;
                if (provider != this._dataProvider)
                {
                    if (this._dataProvider != null)
                    {
                        DataChangedEventManager.RemoveHandler(this._dataProvider, new EventHandler<EventArgs>(this.OnDataChanged));
                    }
                    this._dataProvider = provider;
                    if (this._dataProvider != null)
                    {
                        DataChangedEventManager.AddHandler(this._dataProvider, new EventHandler<EventArgs>(this.OnDataChanged));
                        this._dataProvider.InitialLoad();
                    }
                }
                if (provider != null)
                {
                    source = provider.Data;
                }
                ICollectionView view = null;
                if (source != null)
                {
                    ViewRecord record = DataBindEngine.CurrentDataBindEngine.GetViewRecord(source, this, collectionViewType, true, delegate (object x) {
                        BindingExpressionBase bindingExpressionBase = BindingOperations.GetBindingExpressionBase(this, SourceProperty);
                        if (bindingExpressionBase == null)
                        {
                            return null;
                        }
                        return bindingExpressionBase.GetSourceItem(x);
                    });
                    if (record != null)
                    {
                        view = record.View;
                        this._isViewInitialized = record.IsInitialized;
                        if (this._version != record.Version)
                        {
                            this.ApplyPropertiesToView(view);
                            record.Version = this._version;
                        }
                    }
                }
                base.SetValue(ViewPropertyKey, view);
            }
        }

        internal static System.Windows.Data.CollectionView GetDefaultCollectionView(object source, bool createView, Func<object, object> GetSourceItem = null)
        {
            if (!IsValidSourceForView(source))
            {
                return null;
            }
            ViewRecord record = DataBindEngine.CurrentDataBindEngine.GetViewRecord(source, DefaultSource, null, createView, GetSourceItem);
            if (record == null)
            {
                return null;
            }
            return (System.Windows.Data.CollectionView) record.View;
        }

        internal static System.Windows.Data.CollectionView GetDefaultCollectionView(object source, DependencyObject d, Func<object, object> GetSourceItem = null)
        {
            System.Windows.Data.CollectionView view = GetDefaultCollectionView(source, true, GetSourceItem);
            if ((view != null) && (view.Culture == null))
            {
                XmlLanguage language = (d != null) ? ((XmlLanguage) d.GetValue(FrameworkElement.LanguageProperty)) : null;
                if (language == null)
                {
                    return view;
                }
                try
                {
                    view.Culture = language.GetSpecificCulture();
                }
                catch (InvalidOperationException)
                {
                }
            }
            return view;
        }

        public static ICollectionView GetDefaultView(object source)
        {
            return GetOriginalView(GetDefaultCollectionView(source, true, null));
        }

        private static ICollectionView GetOriginalView(ICollectionView view)
        {
            for (CollectionViewProxy proxy = view as CollectionViewProxy; proxy != null; proxy = view as CollectionViewProxy)
            {
                view = proxy.ProxiedView;
            }
            return view;
        }

        private static bool IsCollectionViewTypeValid(object o)
        {
            Type c = (Type) o;
            if (c != null)
            {
                return typeof(ICollectionView).IsAssignableFrom(c);
            }
            return true;
        }

        public static bool IsDefaultView(ICollectionView view)
        {
            if (view != null)
            {
                object sourceCollection = view.SourceCollection;
                return (GetOriginalView(view) == LazyGetDefaultView(sourceCollection));
            }
            return true;
        }

        internal bool IsShareableInTemplate()
        {
            return false;
        }

        private static bool IsSourceValid(object o)
        {
            if (((o != null) && !(o is IEnumerable)) && (!(o is IListSource) && !(o is DataSourceProvider)))
            {
                return false;
            }
            return !(o is ICollectionView);
        }

        private static bool IsValidSourceForView(object o)
        {
            return ((o is IEnumerable) || (o is IListSource));
        }

        private static ICollectionView LazyGetDefaultView(object source)
        {
            return GetOriginalView(GetDefaultCollectionView(source, false, null));
        }

        protected virtual void OnCollectionViewTypeChanged(Type oldCollectionViewType, Type newCollectionViewType)
        {
        }

        private static void OnCollectionViewTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CollectionViewSource source = (CollectionViewSource) d;
            Type oldValue = (Type) e.OldValue;
            Type newValue = (Type) e.NewValue;
            if (!source._isInitializing)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("CollectionViewTypeIsInitOnly"));
            }
            source.OnCollectionViewTypeChanged(oldValue, newValue);
            source.EnsureView();
        }

        private void OnDataChanged(object sender, EventArgs e)
        {
            this.EnsureView();
        }

        private void OnForwardedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnForwardedPropertyChanged();
        }

        private void OnForwardedPropertyChanged()
        {
            this._version++;
            this.ApplyPropertiesToView(this.View);
        }

        private static void OnIsLiveFilteringRequestedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CollectionViewSource) d).OnForwardedPropertyChanged();
        }

        private static void OnIsLiveGroupingRequestedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CollectionViewSource) d).OnForwardedPropertyChanged();
        }

        private static void OnIsLiveSortingRequestedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CollectionViewSource) d).OnForwardedPropertyChanged();
        }

        protected virtual void OnSourceChanged(object oldSource, object newSource)
        {
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CollectionViewSource source = (CollectionViewSource) d;
            source.OnSourceChanged(e.OldValue, e.NewValue);
            source.EnsureView();
        }

        protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            return false;
        }

        internal override void RemoveInheritanceContext(DependencyObject context, DependencyProperty property)
        {
            InheritanceContextHelper.RemoveInheritanceContext(context, this, ref this._hasMultipleInheritanceContexts, ref this._inheritanceContext);
            this._propertyForInheritanceContext = null;
        }

        void ISupportInitialize.BeginInit()
        {
            this._isInitializing = true;
        }

        void ISupportInitialize.EndInit()
        {
            this._isInitializing = false;
            this.EnsureView();
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            return this.ReceiveWeakEvent(managerType, sender, e);
        }

        private bool WrapFilter(object item)
        {
            FilterEventArgs e = new FilterEventArgs(item);
            FilterEventHandler handler = FilterHandlersField.GetValue(this);
            if (handler != null)
            {
                handler(this, e);
            }
            return e.Accepted;
        }

        [ReadOnly(true)]
        public bool CanChangeLiveFiltering
        {
            get
            {
                return (bool) base.GetValue(CanChangeLiveFilteringProperty);
            }
            private set
            {
                base.SetValue(CanChangeLiveFilteringPropertyKey, value);
            }
        }

        [ReadOnly(true)]
        public bool CanChangeLiveGrouping
        {
            get
            {
                return (bool) base.GetValue(CanChangeLiveGroupingProperty);
            }
            private set
            {
                base.SetValue(CanChangeLiveGroupingPropertyKey, value);
            }
        }

        [ReadOnly(true)]
        public bool CanChangeLiveSorting
        {
            get
            {
                return (bool) base.GetValue(CanChangeLiveSortingProperty);
            }
            private set
            {
                base.SetValue(CanChangeLiveSortingPropertyKey, value);
            }
        }

        internal System.Windows.Data.CollectionView CollectionView
        {
            get
            {
                ICollectionView view = (ICollectionView) base.GetValue(ViewProperty);
                if ((view != null) && !this._isViewInitialized)
                {
                    object source = this.Source;
                    DataSourceProvider provider = source as DataSourceProvider;
                    if (provider != null)
                    {
                        source = provider.Data;
                    }
                    if (source != null)
                    {
                        ViewRecord record = DataBindEngine.CurrentDataBindEngine.GetViewRecord(source, this, this.CollectionViewType, true, null);
                        if (record != null)
                        {
                            record.InitializeView();
                            this._isViewInitialized = true;
                        }
                    }
                }
                return (System.Windows.Data.CollectionView) view;
            }
        }

        public Type CollectionViewType
        {
            get
            {
                return (Type) base.GetValue(CollectionViewTypeProperty);
            }
            set
            {
                base.SetValue(CollectionViewTypeProperty, value);
            }
        }

        [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
        public CultureInfo Culture
        {
            get
            {
                return this._culture;
            }
            set
            {
                this._culture = value;
                this.OnForwardedPropertyChanged();
            }
        }

        internal override int EffectiveValuesInitialSize
        {
            get
            {
                return 3;
            }
        }

        private Predicate<object> FilterWrapper
        {
            get
            {
                if (this._filterStub == null)
                {
                    this._filterStub = new FilterStub(this);
                }
                return this._filterStub.FilterWrapper;
            }
        }

        public ObservableCollection<GroupDescription> GroupDescriptions
        {
            get
            {
                return this._groupBy;
            }
        }

        internal override bool HasMultipleInheritanceContexts
        {
            get
            {
                return this._hasMultipleInheritanceContexts;
            }
        }

        internal override DependencyObject InheritanceContext
        {
            get
            {
                return this._inheritanceContext;
            }
        }

        [ReadOnly(true)]
        public bool? IsLiveFiltering
        {
            get
            {
                return (bool?) base.GetValue(IsLiveFilteringProperty);
            }
            private set
            {
                base.SetValue(IsLiveFilteringPropertyKey, value);
            }
        }

        public bool IsLiveFilteringRequested
        {
            get
            {
                return (bool) base.GetValue(IsLiveFilteringRequestedProperty);
            }
            set
            {
                base.SetValue(IsLiveFilteringRequestedProperty, value);
            }
        }

        [ReadOnly(true)]
        public bool? IsLiveGrouping
        {
            get
            {
                return (bool?) base.GetValue(IsLiveGroupingProperty);
            }
            private set
            {
                base.SetValue(IsLiveGroupingPropertyKey, value);
            }
        }

        public bool IsLiveGroupingRequested
        {
            get
            {
                return (bool) base.GetValue(IsLiveGroupingRequestedProperty);
            }
            set
            {
                base.SetValue(IsLiveGroupingRequestedProperty, value);
            }
        }

        [ReadOnly(true)]
        public bool? IsLiveSorting
        {
            get
            {
                return (bool?) base.GetValue(IsLiveSortingProperty);
            }
            private set
            {
                base.SetValue(IsLiveSortingPropertyKey, value);
            }
        }

        public bool IsLiveSortingRequested
        {
            get
            {
                return (bool) base.GetValue(IsLiveSortingRequestedProperty);
            }
            set
            {
                base.SetValue(IsLiveSortingRequestedProperty, value);
            }
        }

        public ObservableCollection<string> LiveFilteringProperties
        {
            get
            {
                if (this._liveFilteringProperties == null)
                {
                    this._liveFilteringProperties = new ObservableCollection<string>();
                    this._liveFilteringProperties.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnForwardedCollectionChanged);
                }
                return this._liveFilteringProperties;
            }
        }

        public ObservableCollection<string> LiveGroupingProperties
        {
            get
            {
                if (this._liveGroupingProperties == null)
                {
                    this._liveGroupingProperties = new ObservableCollection<string>();
                    this._liveGroupingProperties.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnForwardedCollectionChanged);
                }
                return this._liveGroupingProperties;
            }
        }

        public ObservableCollection<string> LiveSortingProperties
        {
            get
            {
                if (this._liveSortingProperties == null)
                {
                    this._liveSortingProperties = new ObservableCollection<string>();
                    this._liveSortingProperties.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnForwardedCollectionChanged);
                }
                return this._liveSortingProperties;
            }
        }

        internal DependencyProperty PropertyForInheritanceContext
        {
            get
            {
                return this._propertyForInheritanceContext;
            }
        }

        public SortDescriptionCollection SortDescriptions
        {
            get
            {
                return this._sort;
            }
        }

        public object Source
        {
            get
            {
                return base.GetValue(SourceProperty);
            }
            set
            {
                base.SetValue(SourceProperty, value);
            }
        }

        [ReadOnly(true)]
        public ICollectionView View
        {
            get
            {
                return GetOriginalView(this.CollectionView);
            }
        }

        private class DeferHelper : IDisposable
        {
            private CollectionViewSource _target;

            public DeferHelper(CollectionViewSource target)
            {
                this._target = target;
                this._target.BeginDefer();
            }

            public void Dispose()
            {
                if (this._target != null)
                {
                    CollectionViewSource source = this._target;
                    this._target = null;
                    source.EndDefer();
                }
                GC.SuppressFinalize(this);
            }
        }

        private class FilterStub
        {
            private Predicate<object> _filterWrapper;
            private WeakReference _parent;

            public FilterStub(CollectionViewSource parent)
            {
                this._parent = new WeakReference(parent);
                this._filterWrapper = new Predicate<object>(this.WrapFilter);
            }

            private bool WrapFilter(object item)
            {
                CollectionViewSource target = (CollectionViewSource) this._parent.Target;
                if (target != null)
                {
                    return target.WrapFilter(item);
                }
                return true;
            }

            public Predicate<object> FilterWrapper
            {
                get
                {
                    return this._filterWrapper;
                }
            }
        }
    }
}

