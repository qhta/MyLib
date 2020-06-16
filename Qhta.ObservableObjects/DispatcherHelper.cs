using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;

namespace Qhta.ObservableObjects
{
  public class DispatcherHelper
  {

    public DispatcherHelper()
    {
      _initialDispatcher = Dispatcher.FromThread(Thread.CurrentThread);
      if (_initialDispatcher == null)
        Debug.Assert(true);
    }

    public Dispatcher Dispatcher
    {
      get
      {
        if (_initialDispatcher != null)
          return _initialDispatcher;
        return GetCurrentDispatcher();
      }
    }
    private Dispatcher _initialDispatcher;


    public DispatcherHelper(LockTypeEnum lockType)
    {
      _lockType = lockType;
      _lockObj = new object();
    }

    public LockTypeEnum LockType => _lockType;
    private readonly LockTypeEnum _lockType;
    private bool _lockObjWasTaken;

    private readonly object _lockObj;
    private int _lock; // 0=unlocked, 1=locked


    #region SpinWait/PumpWait Methods

    // returns a valid dispatcher if this is a UI thread 
    // (can be more than one UI thread so different dispatchers are possible); 
    // null if not a UI thread
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dispatcher GetCurrentDispatcher()
    {
      var result =  Dispatcher.FromThread(Thread.CurrentThread);
      //Debug.WriteLine($"DispatcherHelper.GetDispatcher={result!=null}");
      return result;
    }

    public void WaitForCondition(Func<bool> condition)
    {
      //Debug.WriteLine("DispatcherHelper.WaitForCondition");

      var dispatcher = GetCurrentDispatcher();

      if (dispatcher == null)
      {
        switch (LockType)
        {
          case LockTypeEnum.SpinWait:
            SpinWait.SpinUntil(condition); // spin baby... 
            break;
          case LockTypeEnum.Lock:
            var isLockTaken = false;
            Monitor.Enter(_lockObj, ref isLockTaken);
            _lockObjWasTaken = isLockTaken;
            break;
        }
        return;
      }

      _lockObjWasTaken = true;
      PumpWait_PumpUntil(dispatcher, condition);
    }

    public void PumpWait_PumpUntil(Dispatcher dispatcher, Func<bool> condition)
    {
      //Debug.WriteLine("PumpWait_PumpUntil");
      var frame = new DispatcherFrame();
      BeginInvokePump(dispatcher, frame, condition);
      Dispatcher.PushFrame(frame);
    }

    private static void BeginInvokePump(Dispatcher dispatcher, DispatcherFrame frame, Func<bool> condition)
    {
      //Debug.WriteLine("BeginInvokePump");
      dispatcher.BeginInvoke
        (
        DispatcherPriority.DataBind,
        (Action)(() =>
          {
            frame.Continue = !condition();

            if (frame.Continue)
              BeginInvokePump(dispatcher, frame, condition);
          })
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DoEvents()
    {
      //Debug.WriteLine("DoEvents");
      var dispatcher = GetCurrentDispatcher();
      if (dispatcher == null)
      {
        return;
      }

      var frame = new DispatcherFrame();
      dispatcher.BeginInvoke(DispatcherPriority.DataBind, new DispatcherOperationCallback(ExitFrame), frame);
      Dispatcher.PushFrame(frame);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static object ExitFrame(object frame)
    {
      //Debug.WriteLine("ExitFrame");

      ((DispatcherFrame)frame).Continue = false;
      return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryLock()
    {
      //Debug.WriteLine("TryLock");
      bool ok=false;
      switch (LockType)
      {
        case LockTypeEnum.SpinWait:
          ok = Interlocked.CompareExchange(ref _lock, 1, 0) == 0;
          break;
        case LockTypeEnum.Lock:
          ok = Monitor.TryEnter(_lockObj);
          break;
      }
      //Debug.WriteLine($"Locked = {ok}");
      return ok;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Lock()
    {
      //Debug.WriteLine("Lock");
      switch (LockType)
      {
        case LockTypeEnum.SpinWait:
          WaitForCondition(() => Interlocked.CompareExchange(ref _lock, 1, 0) == 0);
          break;
        case LockTypeEnum.Lock:
          WaitForCondition(() => Monitor.TryEnter(_lockObj));
          break;
      }
      //Debug.WriteLine("Locked");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unlock()
    {
      //Debug.WriteLine("Unlock");
      switch (LockType)
      {
        case LockTypeEnum.SpinWait:
          _lock = 0;
          break;
        case LockTypeEnum.Lock:
          if (_lockObjWasTaken)
            Monitor.Exit(_lockObj);
          _lockObjWasTaken = false;
          break;
      }
      //Debug.WriteLine("Unlocked");
    }

    #endregion SpinWait/PumpWait Methods  
  }
}
