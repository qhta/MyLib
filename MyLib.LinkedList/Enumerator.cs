using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using MyLib.Resources;

namespace MyLib.LinkedList
{
  [Serializable, StructLayout(LayoutKind.Sequential), __DynamicallyInvokable]
  public struct Enumerator<T> : IEnumerator<T>, IDisposable, IEnumerator, ISerializable, IDeserializationCallback where T: class
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
