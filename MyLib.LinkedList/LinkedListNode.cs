using System.Runtime.InteropServices;

namespace MyLib.LinkedList
{
  [ComVisible(false), __DynamicallyInvokable]
  public sealed class LinkedListNode<T> where T: class
  {
    // Fields
    internal T item;
    internal LinkedList<T> list;
    internal LinkedListNode<T> next;
    internal LinkedListNode<T> prev;

    // Methods
    [__DynamicallyInvokable]
    public LinkedListNode(T value)
    {
      this.item = value;
    }

    internal LinkedListNode(LinkedList<T> list, T value)
    {
      this.list = list;
      this.item = value;
    }

    internal void Invalidate()
    {
      this.list = null;
      this.next = null;
      this.prev = null;
    }

    // Properties
    [__DynamicallyInvokable]
    public LinkedList<T> List
    {
      [__DynamicallyInvokable]
      get
      {
        return this.list;
      }
    }

    [__DynamicallyInvokable]
    public LinkedListNode<T> Next
    {
      [__DynamicallyInvokable]
      get
      {
        if ((this.next != null) && (this.next != this.list.head))
        {
          return this.next;
        }
        return null;
      }
    }

    [__DynamicallyInvokable]
    public LinkedListNode<T> Previous
    {
      [__DynamicallyInvokable]
      get
      {
        if ((this.prev != null) && (this != this.list.head))
        {
          return this.prev;
        }
        return null;
      }
    }

    [__DynamicallyInvokable]
    public T Value
    {
      [__DynamicallyInvokable]
      get
      {
        return this.item;
      }
      [__DynamicallyInvokable]
      set
      {
        this.item = value;
      }
    }
  }


}
