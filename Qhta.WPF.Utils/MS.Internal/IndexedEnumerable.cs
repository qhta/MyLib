namespace MS.Internal.Data
{
    using MS.Internal;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Data;

    internal class IndexedEnumerable : IEnumerable, IWeakEventListener
    {
        private int _cachedCount;
        private int _cachedIndex;
        private bool? _cachedIsEmpty;
        private object _cachedItem;
        private int _cachedVersion;
        private IEnumerator _changeTracker;
        private ICollection _collection;
        private System.Windows.Data.CollectionView _collectionView;
        private IEnumerable _enumerable;
        private IEnumerator _enumerator;
        private int _enumeratorVersion;
        private Predicate<object> _filterCallback;
        private IList _list;
        private PropertyInfo _reflectedCount;
        private MethodInfo _reflectedIndexOf;
        private PropertyInfo _reflectedItemAt;

        internal IndexedEnumerable(IEnumerable collection) : this(collection, null)
        {
        }

        internal IndexedEnumerable(IEnumerable collection, Predicate<object> filterCallback)
        {
            this._cachedIndex = -1;
            this._cachedVersion = -1;
            this._cachedCount = -1;
            this._filterCallback = filterCallback;
            this.SetCollection(collection);
            if (this.List == null)
            {
                INotifyCollectionChanged source = collection as INotifyCollectionChanged;
                if (source != null)
                {
                    CollectionChangedEventManager.AddHandler(source, new EventHandler<NotifyCollectionChangedEventArgs>(this.OnCollectionChanged));
                }
            }
        }

        private void CacheCurrentItem(int index, object item)
        {
            this._cachedIndex = index;
            this._cachedItem = item;
            this._cachedVersion = this._enumeratorVersion;
        }

        private void ClearAllCaches()
        {
            this._cachedItem = null;
            this._cachedIndex = -1;
            this._cachedCount = -1;
        }

        internal static void CopyTo(IEnumerable collection, Array array, int index)
        {
            Invariant.Assert(collection > null, "collection is null");
            Invariant.Assert(array > null, "target array is null");
            Invariant.Assert(array.Rank == 1, "expected array of rank=1");
            Invariant.Assert(index >= 0, "index must be positive");
            ICollection is2 = collection as ICollection;
            if (is2 != null)
            {
                is2.CopyTo(array, index);
            }
            else
            {
                IList list = array;
                foreach (object obj2 in collection)
                {
                    if (index >= array.Length)
                    {
                        throw new ArgumentException(System.Windows.SR.Get("CopyToNotEnoughSpace"), "index");
                    }
                    list[index] = obj2;
                    index++;
                }
            }
        }

        private void DisposeEnumerator(ref IEnumerator ie)
        {
            IDisposable disposable = ie as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
            ie = null;
        }

        private bool EnsureCacheCurrent()
        {
            int num = this.EnsureEnumerator();
            if (num != this._cachedVersion)
            {
                this.ClearAllCaches();
                this._cachedVersion = num;
            }
            return ((num == this._cachedVersion) && (this._cachedIndex >= 0));
        }

        private int EnsureEnumerator()
        {
            if (this._enumerator == null)
            {
                this.UseNewEnumerator();
            }
            else
            {
                try
                {
                    this._changeTracker.MoveNext();
                }
                catch (InvalidOperationException)
                {
                    this.UseNewEnumerator();
                }
            }
            return this._enumeratorVersion;
        }

        public IEnumerator GetEnumerator()
        {
            return new FilteredEnumerator(this, this.Enumerable, this.FilterCallback);
        }

        private bool GetNativeCount(out int value)
        {
            bool flag = false;
            value = -1;
            if (this.Collection != null)
            {
                value = this.Collection.Count;
                return true;
            }
            if (this.CollectionView != null)
            {
                value = this.CollectionView.Count;
                return true;
            }
            if (this._reflectedCount == null)
            {
                return flag;
            }
            try
            {
                value = (int) this._reflectedCount.GetValue(this.Enumerable, null);
                return true;
            }
            catch (MethodAccessException)
            {
                this._reflectedCount = null;
                return false;
            }
        }

        private bool GetNativeIndexOf(object item, out int value)
        {
            bool flag = false;
            value = -1;
            if ((this.List != null) && (this.FilterCallback == null))
            {
                value = this.List.IndexOf(item);
                return true;
            }
            if (this.CollectionView != null)
            {
                value = this.CollectionView.IndexOf(item);
                return true;
            }
            if (this._reflectedIndexOf == null)
            {
                return flag;
            }
            try
            {
                object[] parameters = new object[] { item };
                value = (int) this._reflectedIndexOf.Invoke(this.Enumerable, parameters);
                return true;
            }
            catch (MethodAccessException)
            {
                this._reflectedIndexOf = null;
                return false;
            }
        }

        private bool GetNativeIsEmpty(out bool isEmpty)
        {
            bool flag = false;
            isEmpty = true;
            if (this.Collection != null)
            {
                isEmpty = this.Collection.Count == 0;
                return true;
            }
            if (this.CollectionView != null)
            {
                isEmpty = this.CollectionView.IsEmpty;
                return true;
            }
            if (this._reflectedCount == null)
            {
                return flag;
            }
            try
            {
                isEmpty = ((int) this._reflectedCount.GetValue(this.Enumerable, null)) == 0;
                return true;
            }
            catch (MethodAccessException)
            {
                this._reflectedCount = null;
                return false;
            }
        }

        private bool GetNativeItemAt(int index, out object value)
        {
            bool flag = false;
            value = null;
            if (this.List != null)
            {
                value = this.List[index];
                return true;
            }
            if (this.CollectionView != null)
            {
                value = this.CollectionView.GetItemAt(index);
                return true;
            }
            if (this._reflectedItemAt == null)
            {
                return flag;
            }
            try
            {
                object[] objArray1 = new object[] { index };
                value = this._reflectedItemAt.GetValue(this.Enumerable, objArray1);
                return true;
            }
            catch (MethodAccessException)
            {
                this._reflectedItemAt = null;
                return false;
            }
        }

        internal int IndexOf(object item)
        {
            int num;
            if (!this.GetNativeIndexOf(item, out num))
            {
                if (this.EnsureCacheCurrent() && (item == this._cachedItem))
                {
                    return this._cachedIndex;
                }
                num = -1;
                if (this._cachedIndex >= 0)
                {
                    this.UseNewEnumerator();
                }
                for (int i = 0; this._enumerator.MoveNext(); i++)
                {
                    if (object.Equals(this._enumerator.Current, item))
                    {
                        num = i;
                        break;
                    }
                }
                if (num >= 0)
                {
                    this.CacheCurrentItem(num, this._enumerator.Current);
                    return num;
                }
                this.ClearAllCaches();
                this.DisposeEnumerator(ref this._enumerator);
            }
            return num;
        }

        internal void Invalidate()
        {
            this.ClearAllCaches();
            if (this.List == null)
            {
                INotifyCollectionChanged enumerable = this.Enumerable as INotifyCollectionChanged;
                if (enumerable != null)
                {
                    CollectionChangedEventManager.RemoveHandler(enumerable, new EventHandler<NotifyCollectionChangedEventArgs>(this.OnCollectionChanged));
                }
            }
            this._enumerable = null;
            this.DisposeEnumerator(ref this._enumerator);
            this.DisposeEnumerator(ref this._changeTracker);
            this._collection = null;
            this._list = null;
            this._filterCallback = null;
        }

        private void InvalidateEnumerator()
        {
            this._enumeratorVersion++;
            this.DisposeEnumerator(ref this._enumerator);
            this.ClearAllCaches();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.InvalidateEnumerator();
        }

        protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            return false;
        }

        private void SetCollection(IEnumerable collection)
        {
            Invariant.Assert(collection > null);
            this._enumerable = collection;
            this._collection = collection as ICollection;
            this._list = collection as IList;
            this._collectionView = collection as System.Windows.Data.CollectionView;
            if ((this.List == null) && (this.CollectionView == null))
            {
                Type type = collection.GetType();
                Type[] types = new Type[] { typeof(object) };
                MethodInfo method = type.GetMethod("IndexOf", types);
                if ((method != null) && (method.ReturnType == typeof(int)))
                {
                    this._reflectedIndexOf = method;
                }
                MemberInfo[] defaultMembers = type.GetDefaultMembers();
                for (int i = 0; i <= (defaultMembers.Length - 1); i++)
                {
                    PropertyInfo info2 = defaultMembers[i] as PropertyInfo;
                    if (info2 != null)
                    {
                        ParameterInfo[] indexParameters = info2.GetIndexParameters();
                        if ((indexParameters.Length == 1) && indexParameters[0].ParameterType.IsAssignableFrom(typeof(int)))
                        {
                            this._reflectedItemAt = info2;
                            break;
                        }
                    }
                }
                if (this.Collection == null)
                {
                    PropertyInfo property = type.GetProperty("Count", typeof(int));
                    if (property != null)
                    {
                        this._reflectedCount = property;
                    }
                }
            }
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            return this.ReceiveWeakEvent(managerType, sender, e);
        }

        private void UseNewEnumerator()
        {
            this._enumeratorVersion++;
            this.DisposeEnumerator(ref this._changeTracker);
            this._changeTracker = this._enumerable.GetEnumerator();
            this.DisposeEnumerator(ref this._enumerator);
            this._enumerator = this.GetEnumerator();
            this._cachedIndex = -1;
            this._cachedItem = null;
        }

        internal ICollection Collection
        {
            get
            {
                return this._collection;
            }
        }

        internal System.Windows.Data.CollectionView CollectionView
        {
            get
            {
                return this._collectionView;
            }
        }

        internal int Count
        {
            get
            {
                this.EnsureCacheCurrent();
                int num = 0;
                if (!this.GetNativeCount(out num))
                {
                    if (this._cachedCount >= 0)
                    {
                        return this._cachedCount;
                    }
                    num = 0;
                    foreach (object obj2 in this)
                    {
                        num++;
                    }
                    this._cachedCount = num;
                    this._cachedIsEmpty = new bool?(this._cachedCount == 0);
                }
                return num;
            }
        }

        internal IEnumerable Enumerable
        {
            get
            {
                return this._enumerable;
            }
        }

        private Predicate<object> FilterCallback
        {
            get
            {
                return this._filterCallback;
            }
        }

        internal bool IsEmpty
        {
            get
            {
                bool flag;
                if (this.GetNativeIsEmpty(out flag))
                {
                    return flag;
                }
                if (!this._cachedIsEmpty.HasValue)
                {
                    IEnumerator enumerator = this.GetEnumerator();
                    this._cachedIsEmpty = new bool?(!enumerator.MoveNext());
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                    if (this._cachedIsEmpty.Value)
                    {
                        this._cachedCount = 0;
                    }
                }
                return this._cachedIsEmpty.Value;
            }
        }

        internal object this[int index]
        {
            get
            {
                object obj2;
                if (this.GetNativeItemAt(index, out obj2))
                {
                    return obj2;
                }
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                int num = index - this._cachedIndex;
                if (num < 0)
                {
                    this.UseNewEnumerator();
                    num = index + 1;
                }
                if (this.EnsureCacheCurrent())
                {
                    if (index == this._cachedIndex)
                    {
                        return this._cachedItem;
                    }
                }
                else
                {
                    num = index + 1;
                }
                while ((num > 0) && this._enumerator.MoveNext())
                {
                    num--;
                }
                if (num != 0)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                this.CacheCurrentItem(index, this._enumerator.Current);
                return this._cachedItem;
            }
        }

        internal IList List
        {
            get
            {
                return this._list;
            }
        }

        private class FilteredEnumerator : IEnumerator, IDisposable
        {
            private IEnumerable _enumerable;
            private IEnumerator _enumerator;
            private Predicate<object> _filterCallback;
            private IndexedEnumerable _indexedEnumerable;

            public FilteredEnumerator(IndexedEnumerable indexedEnumerable, IEnumerable enumerable, Predicate<object> filterCallback)
            {
                this._enumerable = enumerable;
                this._enumerator = this._enumerable.GetEnumerator();
                this._filterCallback = filterCallback;
                this._indexedEnumerable = indexedEnumerable;
            }

            public void Dispose()
            {
                IDisposable disposable = this._enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
                this._enumerator = null;
            }

            bool IEnumerator.MoveNext()
            {
                bool flag;
                if (this._indexedEnumerable._enumerable == null)
                {
                    throw new InvalidOperationException(System.Windows.SR.Get("EnumeratorVersionChanged"));
                }
                if (this._filterCallback == null)
                {
                    return this._enumerator.MoveNext();
                }
                while ((flag = this._enumerator.MoveNext()) && !this._filterCallback(this._enumerator.Current))
                {
                }
                return flag;
            }

            void IEnumerator.Reset()
            {
                if (this._indexedEnumerable._enumerable == null)
                {
                    throw new InvalidOperationException(System.Windows.SR.Get("EnumeratorVersionChanged"));
                }
                this.Dispose();
                this._enumerator = this._enumerable.GetEnumerator();
            }

            object IEnumerator.Current
            {
                get
                {
                    return this._enumerator.Current;
                }
            }
        }
    }
}

