using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;
using MyLib.Resources;

namespace MyLib.LinkedList
{
  [Serializable, ComVisible(false), DebuggerTypeProxy(typeof(System_CollectionDebugView<>)), DebuggerDisplay("Count = {Count}"), __DynamicallyInvokable]
  public class LinkedList<T> : ICollection<T>, IEnumerable<T>, IEnumerable, ICollection, IReadOnlyCollection<T>, ISerializable, IDeserializationCallback where T: class
  {
    // Fields
    private object _syncRoot;
    internal int count;
    private const string CountName = "Count";
    internal LinkedListNode<T> head;
    internal SerializationInfo siInfo;
    private const string ValuesName = "Data";
    internal int version;
    private const string VersionName = "Version";

    // Methods
    [__DynamicallyInvokable]
    public LinkedList()
    {
    }

    [__DynamicallyInvokable]
    public LinkedList(IEnumerable<T> collection)
    {
      if (collection == null)
      {
        throw new ArgumentNullException("collection");
      }
      foreach (T local in collection)
      {
        this.AddLast(local);
      }
    }

    protected LinkedList(SerializationInfo info, StreamingContext context)
    {
      this.siInfo = info;
    }

    [__DynamicallyInvokable]
    public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value)
    {
      this.ValidateNode(node);
      LinkedListNode<T> newNode = new LinkedListNode<T>(node.list, value);
      this.InternalInsertNodeBefore(node.next, newNode);
      return newNode;
    }

    [__DynamicallyInvokable]
    public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
    {
      this.ValidateNode(node);
      this.ValidateNewNode(newNode);
      this.InternalInsertNodeBefore(node.next, newNode);
      newNode.list = (LinkedList<T>)this;
    }

    [__DynamicallyInvokable]
    public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
    {
      this.ValidateNode(node);
      this.ValidateNewNode(newNode);
      this.InternalInsertNodeBefore(node, newNode);
      newNode.list = (LinkedList<T>)this;
      if (node == this.head)
      {
        this.head = newNode;
      }
    }

    [__DynamicallyInvokable]
    public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)
    {
      this.ValidateNode(node);
      LinkedListNode<T> newNode = new LinkedListNode<T>(node.list, value);
      this.InternalInsertNodeBefore(node, newNode);
      if (node == this.head)
      {
        this.head = newNode;
      }
      return newNode;
    }

    [__DynamicallyInvokable]
    public void AddFirst(LinkedListNode<T> node)
    {
      this.ValidateNewNode(node);
      if (this.head == null)
      {
        this.InternalInsertNodeToEmptyList(node);
      }
      else
      {
        this.InternalInsertNodeBefore(this.head, node);
        this.head = node;
      }
      node.list = (LinkedList<T>)this;
    }

    [__DynamicallyInvokable]
    public LinkedListNode<T> AddFirst(T value)
    {
      LinkedListNode<T> newNode = new LinkedListNode<T>((LinkedList<T>)this, value);
      if (this.head == null)
      {
        this.InternalInsertNodeToEmptyList(newNode);
        return newNode;
      }
      this.InternalInsertNodeBefore(this.head, newNode);
      this.head = newNode;
      return newNode;
    }

    [__DynamicallyInvokable]
    public void AddLast(LinkedListNode<T> node)
    {
      this.ValidateNewNode(node);
      if (this.head == null)
      {
        this.InternalInsertNodeToEmptyList(node);
      }
      else
      {
        this.InternalInsertNodeBefore(this.head, node);
      }
      node.list = (LinkedList<T>)this;
    }

    [__DynamicallyInvokable]
    public LinkedListNode<T> AddLast(T value)
    {
      LinkedListNode<T> newNode = new LinkedListNode<T>((LinkedList<T>)this, value);
      if (this.head == null)
      {
        this.InternalInsertNodeToEmptyList(newNode);
        return newNode;
      }
      this.InternalInsertNodeBefore(this.head, newNode);
      return newNode;
    }

    [__DynamicallyInvokable]
    public void Clear()
    {
      LinkedListNode<T> head = this.head;
      while (head != null)
      {
        LinkedListNode<T> node2 = head;
        head = head.Next;
        node2.Invalidate();
      }
      this.head = null;
      this.count = 0;
      this.version++;
    }

    [__DynamicallyInvokable]
    public bool Contains(T value)
    {
      return (this.Find(value) != null);
    }

    [__DynamicallyInvokable]
    public void CopyTo(T[] array, int index)
    {
      if (array == null)
      {
        throw new ArgumentNullException("array");
      }
      if ((index < 0) || (index > array.Length))
      {
        object[] args = new object[] { index };
        throw new ArgumentOutOfRangeException("index", SR.GetString("IndexOutOfRange", args));
      }
      if ((array.Length - index) < this.Count)
      {
        throw new ArgumentException(SR.GetString("Arg_InsufficientSpace"));
      }
      LinkedListNode<T> head = this.head;
      if (head != null)
      {
        do
        {
          array[index++] = head.item;
          head = head.next;
        }
        while (head != this.head);
      }
    }

    [__DynamicallyInvokable]
    public LinkedListNode<T> Find(T value)
    {
      LinkedListNode<T> head = this.head;
      EqualityComparer<T> comparer = EqualityComparer<T>.Default;
      if (head != null)
      {
        if (value != null)
        {
          do
          {
            if (comparer.Equals(head.item, value))
            {
              return head;
            }
            head = head.next;
          }
          while (head != this.head);
        }
        else
        {
          do
          {
            if (head.item == null)
            {
              return head;
            }
            head = head.next;
          }
          while (head != this.head);
        }
      }
      return null;
    }

    [__DynamicallyInvokable]
    public LinkedListNode<T> FindLast(T value)
    {
      if (this.head != null)
      {
        LinkedListNode<T> prev = this.head.prev;
        LinkedListNode<T> node2 = prev;
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        if (node2 != null)
        {
          if (value != null)
          {
            do
            {
              if (comparer.Equals(node2.item, value))
              {
                return node2;
              }
              node2 = node2.prev;
            }
            while (node2 != prev);
          }
          else
          {
            do
            {
              if (node2.item == null)
              {
                return node2;
              }
              node2 = node2.prev;
            }
            while (node2 != prev);
          }
        }
      }
      return null;
    }

    [__DynamicallyInvokable]
    public Enumerator<T> GetEnumerator()
    {
      return new Enumerator<T>((LinkedList<T>)this);
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
      {
        throw new ArgumentNullException("info");
      }
      info.AddValue("Version", this.version);
      info.AddValue("Count", this.count);
      if (this.count != 0)
      {
        T[] array = new T[this.Count];
        this.CopyTo(array, 0);
        info.AddValue("Data", array, typeof(T[]));
      }
    }

    private void InternalInsertNodeBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
    {
      newNode.next = node;
      newNode.prev = node.prev;
      node.prev.next = newNode;
      node.prev = newNode;
      this.version++;
      this.count++;
    }

    private void InternalInsertNodeToEmptyList(LinkedListNode<T> newNode)
    {
      newNode.next = newNode;
      newNode.prev = newNode;
      this.head = newNode;
      this.version++;
      this.count++;
    }

    internal void InternalRemoveNode(LinkedListNode<T> node)
    {
      if (node.next == node)
      {
        this.head = null;
      }
      else
      {
        node.next.prev = node.prev;
        node.prev.next = node.next;
        if (this.head == node)
        {
          this.head = node.next;
        }
      }
      node.Invalidate();
      this.count--;
      this.version++;
    }

    public virtual void OnDeserialization(object sender)
    {
      if (this.siInfo != null)
      {
        int num = this.siInfo.GetInt32("Version");
        if (this.siInfo.GetInt32("Count") != 0)
        {
          T[] localArray = (T[])this.siInfo.GetValue("Data", typeof(T[]));
          if (localArray == null)
          {
            throw new SerializationException(SR.GetString("Serialization_MissingValues"));
          }
          for (int i = 0; i < localArray.Length; i++)
          {
            this.AddLast(localArray[i]);
          }
        }
        else
        {
          this.head = null;
        }
        this.version = num;
        this.siInfo = null;
      }
    }

    [__DynamicallyInvokable]
    public bool Remove(T value)
    {
      LinkedListNode<T> node = this.Find(value);
      if (node != null)
      {
        this.InternalRemoveNode(node);
        return true;
      }
      return false;
    }

    [__DynamicallyInvokable]
    public void Remove(LinkedListNode<T> node)
    {
      this.ValidateNode(node);
      this.InternalRemoveNode(node);
    }

    [__DynamicallyInvokable]
    public void RemoveFirst()
    {
      if (this.head == null)
      {
        throw new InvalidOperationException(SR.GetString("LinkedListEmpty"));
      }
      this.InternalRemoveNode(this.head);
    }

    [__DynamicallyInvokable]
    public void RemoveLast()
    {
      if (this.head == null)
      {
        throw new InvalidOperationException(SR.GetString("LinkedListEmpty"));
      }
      this.InternalRemoveNode(this.head.prev);
    }

    [__DynamicallyInvokable]
    void ICollection<T>.Add(T value)
    {
      this.AddLast(value);
    }

    [__DynamicallyInvokable]
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    [__DynamicallyInvokable]
    void ICollection.CopyTo(Array array, int index)
    {
      if (array == null)
      {
        throw new ArgumentNullException("array");
      }
      if (array.Rank != 1)
      {
        throw new ArgumentException(SR.GetString("Arg_MultiRank"));
      }
      if (array.GetLowerBound(0) != 0)
      {
        throw new ArgumentException(SR.GetString("Arg_NonZeroLowerBound"));
      }
      if (index < 0)
      {
        object[] args = new object[] { index };
        throw new ArgumentOutOfRangeException("index", SR.GetString("IndexOutOfRange", args));
      }
      if ((array.Length - index) < this.Count)
      {
        throw new ArgumentException(SR.GetString("Arg_InsufficientSpace"));
      }
      T[] localArray = array as T[];
      if (localArray != null)
      {
        this.CopyTo(localArray, index);
      }
      else
      {
        Type elementType = array.GetType().GetElementType();
        Type c = typeof(T);
        if (!elementType.IsAssignableFrom(c) && !c.IsAssignableFrom(elementType))
        {
          throw new ArgumentException(SR.GetString("Invalid_Array_Type"));
        }
        object[] objArray = array as object[];
        if (objArray == null)
        {
          throw new ArgumentException(SR.GetString("Invalid_Array_Type"));
        }
        LinkedListNode<T> head = this.head;
        try
        {
          if (head != null)
          {
            do
            {
              objArray[index++] = head.item;
              head = head.next;
            }
            while (head != this.head);
          }
        }
        catch (ArrayTypeMismatchException)
        {
          throw new ArgumentException(SR.GetString("Invalid_Array_Type"));
        }
      }
    }

    [__DynamicallyInvokable]
    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    internal void ValidateNewNode(LinkedListNode<T> node)
    {
      if (node == null)
      {
        throw new ArgumentNullException("node");
      }
      if (node.list != null)
      {
        throw new InvalidOperationException(SR.GetString("LinkedListNodeIsAttached"));
      }
    }

    internal void ValidateNode(LinkedListNode<T> node)
    {
      if (node == null)
      {
        throw new ArgumentNullException("node");
      }
      if (node.list != this)
      {
        throw new InvalidOperationException(SR.GetString("ExternalLinkedListNode"));
      }
    }

    // Properties
    [__DynamicallyInvokable]
    public int Count
    {
      [__DynamicallyInvokable]
      get
      {
        return this.count;
      }
    }

    [__DynamicallyInvokable]
    public LinkedListNode<T> First
    {
      [__DynamicallyInvokable]
      get
      {
        return this.head;
      }
    }

    [__DynamicallyInvokable]
    public LinkedListNode<T> Last
    {
      [__DynamicallyInvokable]
      get
      {
        if (this.head != null)
        {
          return this.head.prev;
        }
        return null;
      }
    }

    [__DynamicallyInvokable]
    bool ICollection<T>.IsReadOnly
    {
      [__DynamicallyInvokable]
      get
      {
        return false;
      }
    }

    [__DynamicallyInvokable]
    bool ICollection.IsSynchronized
    {
      [__DynamicallyInvokable]
      get
      {
        return false;
      }
    }

    [__DynamicallyInvokable]
    object ICollection.SyncRoot
    {
      [__DynamicallyInvokable]
      get
      {
        if (this._syncRoot == null)
        {
          Interlocked.CompareExchange<object>(ref this._syncRoot, new object(), null);
        }
        return this._syncRoot;
      }
    }

    // Nested Types
    [Serializable, StructLayout(LayoutKind.Sequential), __DynamicallyInvokable]
    public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator, ISerializable, IDeserializationCallback
    {
      private LinkedList<T> list;
      private LinkedListNode<T> node;
      private int version;
      private T current;
      private int index;
      private SerializationInfo siInfo;
      private const string LinkedListName = "LinkedList";
      private const string CurrentValueName = "Current";
      private const string VersionName = "Version";
      private const string IndexName = "Index";
      internal Enumerator(LinkedList<T> list)
      {
        this.list = list;
        this.version = list.version;
        this.node = list.head;
        this.current = default(T);
        this.index = 0;
        this.siInfo = null;
      }

      internal Enumerator(SerializationInfo info, StreamingContext context)
      {
        this.siInfo = info;
        this.list = null;
        this.version = 0;
        this.node = null;
        this.current = default(T);
        this.index = 0;
      }

      [__DynamicallyInvokable]
      public T Current
      {
        [__DynamicallyInvokable]
        get
        {
          return this.current;
        }
      }
      [__DynamicallyInvokable]
      object IEnumerator.Current
      {
        [__DynamicallyInvokable]
        get
        {
          if ((this.index == 0) || (this.index == (this.list.Count + 1)))
          {
            ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
          }
          return this.current;
        }
      }
      [__DynamicallyInvokable]
      public bool MoveNext()
      {
        if (this.version != this.list.version)
        {
          throw new InvalidOperationException(SR.GetString("InvalidOperation_EnumFailedVersion"));
        }
        if (this.node == null)
        {
          this.index = this.list.Count + 1;
          return false;
        }
        this.index++;
        this.current = this.node.item;
        this.node = this.node.next;
        if (this.node == this.list.head)
        {
          this.node = null;
        }
        return true;
      }

      [__DynamicallyInvokable]
      void IEnumerator.Reset()
      {
        if (this.version != this.list.version)
        {
          throw new InvalidOperationException(SR.GetString("InvalidOperation_EnumFailedVersion"));
        }
        this.current = default(T);
        this.node = this.list.head;
        this.index = 0;
      }

      [__DynamicallyInvokable]
      public void Dispose()
      {
      }

      void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
      {
        if (info == null)
        {
          throw new ArgumentNullException("info");
        }
        info.AddValue("LinkedList", this.list);
        info.AddValue("Version", this.version);
        info.AddValue("Current", this.current);
        info.AddValue("Index", this.index);
      }

      void IDeserializationCallback.OnDeserialization(object sender)
      {
        if (this.list == null)
        {
          if (this.siInfo == null)
          {
            throw new SerializationException(SR.GetString("Serialization_InvalidOnDeser"));
          }
          this.list = (LinkedList<T>)this.siInfo.GetValue("LinkedList", typeof(LinkedList<T>));
          this.version = this.siInfo.GetInt32("Version");
          this.current = (T)this.siInfo.GetValue("Current", typeof(T));
          this.index = this.siInfo.GetInt32("Index");
          if (this.list.siInfo != null)
          {
            this.list.OnDeserialization(sender);
          }
          if (this.index == (this.list.Count + 1))
          {
            this.node = null;
          }
          else
          {
            this.node = this.list.First;
            if ((this.node != null) && (this.index != 0))
            {
              for (int i = 0; i < this.index; i++)
              {
                this.node = this.node.next;
              }
              if (this.node == this.list.First)
              {
                this.node = null;
              }
            }
          }
          this.siInfo = null;
        }
      }
    }
  }


}
