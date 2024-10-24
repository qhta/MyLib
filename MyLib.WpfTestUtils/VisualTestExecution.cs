﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MyLib.WpfTestUtils
{
  public class VisualTestExecution: DispatcherObject
  {
    private bool sequentialRun;

    public VisualTestExecution(Type testClass)
    {
      TestClass = testClass;
      if (testClass.GetCustomAttribute<SequentialRunAttribute>()!=null)
        sequentialRun=true;
      TestMethods = new Dictionary<string, TestMethodInfo>();
      RecognizeMethods(testClass);
    }

    protected void RecognizeMethods(Type classType)
    {
      ConstructorMethod = classType.GetConstructor(new Type[]{});
      if (ConstructorMethod==null)
        throw new ArgumentException("Test type must have a public parameterless constructor");

      MethodInfo[] publicMethods = classType.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      int n = 0;
      TestMethodInfo priorMethod = null;
      foreach (MethodInfo method in publicMethods)
      {
        ParameterInfo[] parameters = method.GetParameters();
        if (method.GetCustomAttribute<ClassInitializeAttribute>() != null)
        {
          if (!method.IsPublic || !method.IsStatic|| parameters.Length != 1 || parameters[0].ParameterType != typeof(TestContext))
            throw new ArgumentException("ClassInitialize method must be a public static method with one parameter of TestContext type");
          if (ClassInitializeMethod!=null)
            throw new ArgumentException("Only one ClassInitialize method can be declared");
          ClassInitializeMethod = method;
        }
        if (method.GetCustomAttribute<ClassCleanupAttribute>() != null)
        {
          if (!method.IsPublic || !method.IsStatic || parameters.Length != 0)
            throw new ArgumentException("ClassCleanup method must be a public static parameterless method");
          if (ClassCleanupMethod != null)
            throw new ArgumentException("Only one ClassCleanup method can be declared");
          ClassCleanupMethod = method;
        }
        if (method.GetCustomAttribute<TestInitializeAttribute>() != null)
        {
          if (!method.IsPublic || method.IsStatic || parameters.Length != 1 || parameters[0].ParameterType != typeof(TestContext))
            throw new ArgumentException("TestInitialize method must be a public non-static with one parameter of TestContext type");
          if (TestInitializeMethod != null)
            throw new ArgumentException("Only one TestInitialize method can be declared");
          TestInitializeMethod = method;
        }
        if (method.GetCustomAttribute<TestCleanupAttribute>() != null)
        {
          if (!method.IsPublic || method.IsStatic || parameters.Length != 0)
            throw new ArgumentException("TestCleanup method must be a public non-static parameterless method");
          if (TestInitializeMethod != null)
            throw new ArgumentException("Only one TestCleanup method can be declared");
          TestCleanupMethod = method;
        }
        if (method.GetCustomAttribute<TestMethodAttribute>() != null)
        {
          if (!method.IsPublic || method.IsStatic || parameters.Length != 0)
            throw new ArgumentException("TestMethod must be a public non-static parameterless method");
          var methodInfo = new TestMethodInfo(++n, method);
          if (sequentialRun)
          {
            if (priorMethod!=null)
            {
              if (methodInfo.RunAfter==null)
                methodInfo.RunAfter = new List<TestMethodInfo>();
              methodInfo.RunAfter.Add(priorMethod);
            }
            priorMethod = methodInfo;
          }
          else
          {
            foreach (var runAfterAttrib in method.GetCustomAttributes<RunAfterAttribute>())
            {
              if (methodInfo.RunAfter==null)
                methodInfo.RunAfter = new List<TestMethodInfo>();
              var afterMethod = publicMethods.FirstOrDefault(item => item.Name==runAfterAttrib.Name);
              var afterMethodInfo = new TestMethodInfo(0, afterMethod);
              methodInfo.RunAfter.Add(afterMethodInfo);
            }
          }
          TestMethods.Add(method.Name, methodInfo);
        }
      }

      PropertyInfo testContextProperty = classType.GetProperty("TestContext", BindingFlags.Public | BindingFlags.DeclaredOnly);
      if (testContextProperty!=null && testContextProperty.CanWrite)
      {
        TestContextSetMethod = testContextProperty.SetMethod;
      }
    }

    public void Run(VisualTestContext testContext)
    {
      TestContext = testContext;
      TestContext.TestExecution = this;
      execThread = new Thread(Execute);
      execThread.SetApartmentState(ApartmentState.STA);
      execThread.Start(new Action(Fine));
    }

    public VisualTestContext TestContext { get; protected set; }
    Thread execThread;
    bool TerminateRequest;

    public object RunContext
    {
      get { return TestContext; }
    }

    public void Terminate()
    {
      TerminateRequest = true;
    }

    private void Fine()
    {
      execThread = null;
    }

    private void Execute(object onTerminate)
    {
      try
      {
        if (BeforeClassInitialize != null)
          BeforeClassInitialize(this, CreateTestExecutionEventArgs<BeforeClassInitializeEventArgs>(TestClass, ClassInitializeMethod, TestContext));
        if (ClassInitializeMethod != null)
          ClassInitializeMethod.Invoke(null, new object[] { TestContext });
        if (AfterClassInitialize != null)
          AfterClassInitialize(this, CreateTestExecutionEventArgs<AfterClassInitializeEventArgs>(TestClass, ClassInitializeMethod, TestContext, false));

        TestInstance = ConstructorMethod.Invoke(new object[] { });

        if (BeforeTestInitialize != null)
          BeforeTestInitialize(this, CreateTestExecutionEventArgs<BeforeTestInitializeEventArgs>(TestInstance, TestInitializeMethod, TestContext));
        if (TestInitializeMethod != null)
          TestInitializeMethod.Invoke(TestInstance, new object[] { TestContext });
        else if (TestContextSetMethod != null)
          TestContextSetMethod.Invoke(TestInstance, new object[] { TestContext });
        if (AfterTestInitialize != null)
          AfterTestInitialize(this, CreateTestExecutionEventArgs<AfterTestInitializeEventArgs>(TestInstance, TestInitializeMethod, TestContext, false));

        int overallTestCount=0;
        foreach (var methodInfo in TestMethods.Values.OrderBy(item=>item.Number))
        {
          if (TerminateRequest)
            return;
          AddTestResult(methodInfo);
          overallTestCount++;
        }

        if (TestContext.InitialDelay != 0)
          Thread.Sleep(TestContext.InitialDelay);
        VisualTestOverallResult overallResult = TestContext.Results.FirstOrDefault(item => item is VisualTestOverallResult) as VisualTestOverallResult;
        if (overallResult != null)
        {
          overallResult.Outcome = TestState.InProgress;
          overallResult.OverallTestsCount = overallTestCount;
        }

        foreach (var methodInfo in TestMethods.Values.OrderBy(item => item.Number))
        {
          if (TerminateRequest)
            return;
          ScheduleTestMethod(methodInfo, new object[0]);
          if (TestContext.InternalDelay != 0)
            Thread.Sleep(TestContext.InternalDelay);
        }

        if (BeforeTestCleanup != null)
          BeforeTestCleanup(this, CreateTestExecutionEventArgs<BeforeTestCleanupEventArgs>(TestInstance, TestCleanupMethod, TestContext));
        if (TestCleanupMethod != null)
          TestCleanupMethod.Invoke(TestInstance, new object[] { });
        if (AfterTestCleanup != null)
          AfterTestCleanup(this, CreateTestExecutionEventArgs<AfterTestCleanupEventArgs>(TestInstance, TestCleanupMethod, TestContext));

        if (BeforeClassCleanup != null)
          BeforeClassCleanup(this, CreateTestExecutionEventArgs<BeforeClassCleanupEventArgs>(TestClass, ClassCleanupMethod, TestContext));
        if (ClassCleanupMethod != null)
          ClassCleanupMethod.Invoke(null, new object[] { });
        if (AfterClassCleanup != null)
          AfterClassCleanup(this, CreateTestExecutionEventArgs<AfterClassCleanupEventArgs>(TestClass, ClassCleanupMethod, TestContext));
      }
      finally
      {
        if (onTerminate != null)
          (onTerminate as Action).Invoke();
      }
    }

    protected virtual void AddTestResult(TestMethodInfo method)
    {
      Application.Current.Dispatcher.BeginInvoke(new Action(()=>  TestContext.Results.Add(new VisualTestResult(method))));
    }

    public void ScheduleTestMethod(TestMethodInfo methodInfo, object[] args)
    {
      if (methodInfo.RunAfter==null)
      {
        //Action<MethodInfo, object[], VisualTestResult> invokeMethod = InvokeTestMethod;
        //Application.Current.Dispatcher.BeginInvoke(invokeMethod, DispatcherPriority.Background, new object[] { methodInfo.Method, args, result });
        InvokeTestMethod(methodInfo, args);
      }
    }

    protected virtual async void InvokeTestMethod(TestMethodInfo methodInfo, object[] args)
    {
      var method = methodInfo.Method;
      VisualTestOverallResult overallResult = TestContext.Results.FirstOrDefault(item => item is VisualTestOverallResult) as VisualTestOverallResult;
      Stopwatch sw = new Stopwatch();
      sw.Start();
      VisualTestResult result = methodInfo.Result;
      try
      {
        if (result!=null)
          result.Outcome=TestState.InProgress;
        await Task.Run(()=>method.Invoke(TestInstance, args));
        methodInfo.State = TestState.Passed;
        result = methodInfo.Result;
        if (result != null)
        {
          result.Outcome = TestState.Passed;
        }
        if (overallResult != null)
        {
          overallResult.PassedTestsCount += 1;
          if (overallResult.PassedTestsCount >= overallResult.OverallTestsCount)
            if (overallResult.Outcome == TestState.InProgress)
              overallResult.Outcome = TestState.Passed;
        }
      }
      catch (TargetInvocationException ex)
      {
        if (ex.InnerException is AssertFailedException)
        {
          methodInfo.State = TestState.Failed;
          if (result != null)
          {
            result.Outcome = TestState.Failed;
            result.Message = ex.InnerException.Message;
          }
          if (overallResult != null)
          {
            overallResult.Outcome = TestState.Failed;
            overallResult.FailedTestsCount += 1;
          }
        }
        else 
        if (ex.InnerException is AssertInconclusiveException)
        {
          methodInfo.State = TestState.Inconclusive;
          if (result != null)
          {
            result.Outcome = TestState.Inconclusive;
            result.Message = ex.InnerException.Message;
          }
          if (overallResult != null)
            if (overallResult.Outcome != TestState.Failed)
              overallResult.Outcome = TestState.Inconclusive;
          overallResult.InconclusiveTestsCount += 1;
        }
        else
        {
          methodInfo.State=TestState.Error;
          if (result != null)
          {
            result.Outcome = TestState.Error;
            result.Message = ex.InnerException.Message;
          }
          if (overallResult != null)
          {
            overallResult.Outcome = TestState.Failed;
            overallResult.FailedTestsCount += 1;
          }
        }
      }
      finally
      {
        sw.Stop();
        long time = sw.ElapsedMilliseconds;
        if (result != null)
          result.ExecTime = time;
        if (overallResult != null)
          overallResult.ExecTime += time;
        CheckPendingMethods(methodInfo);
      }
    }

    protected void CheckPendingMethods(TestMethodInfo completedMethod)
    {
      foreach (var pendingMethod in TestMethods.Values
        .Where(item=>item.RunAfter!=null && item.State==TestState.Pending)
        .OrderBy(item=>item.Number))
      {
        var pendingForMethod = pendingMethod.RunAfter.FirstOrDefault(item => item.Method==completedMethod.Method);
        if (pendingForMethod!=null)
          pendingForMethod.State=completedMethod.State;
        bool start = true;
        foreach (var runAfterMethod in pendingMethod.RunAfter)
          if (runAfterMethod.State!=TestState.Passed)
            start = false;
        if (start)
          InvokeTestMethod(pendingMethod, new object[0]);
        bool cancel = false;
        foreach (var runAfterMethod in pendingMethod.RunAfter)
          if (runAfterMethod.State!=TestState.Passed && runAfterMethod.State!=TestState.Pending)
            cancel = true;
        if (cancel)
          CancelTestMethod(pendingMethod);
      }
    }

    protected virtual void CancelTestMethod(TestMethodInfo methodInfo)
    {
      methodInfo.State=TestState.Cancelled;
      var method = methodInfo.Method;
      VisualTestOverallResult overallResult = TestContext.Results.FirstOrDefault(item => item is VisualTestOverallResult) as VisualTestOverallResult;
      if (overallResult!=null)
        overallResult.CancelledTestsCount+=1;
    }

    public Type TestClass { get; protected set; }

    public object TestInstance { get; protected set; }

    public MethodInfo TestContextSetMethod { get; set; }

    public ConstructorInfo ConstructorMethod { get; set; }

    public MethodInfo ClassInitializeMethod { get; set; }

    public MethodInfo ClassCleanupMethod { get; set; }

    public MethodInfo TestInitializeMethod { get; set; }

    public MethodInfo TestCleanupMethod { get; set; }

    public Dictionary<string, TestMethodInfo> TestMethods { get; set; }


    public event EventHandler<AfterClassCleanupEventArgs> AfterClassCleanup;

    public event EventHandler<AfterClassInitializeEventArgs> AfterClassInitialize;

    public event EventHandler<AfterTestCleanupEventArgs> AfterTestCleanup;

    public event EventHandler<AfterTestInitializeEventArgs> AfterTestInitialize;

    public event EventHandler<BeforeClassCleanupEventArgs> BeforeClassCleanup;

    public event EventHandler<BeforeClassInitializeEventArgs> BeforeClassInitialize;

    public event EventHandler<BeforeTestCleanupEventArgs> BeforeTestCleanup;

    public event EventHandler<BeforeTestInitializeEventArgs> BeforeTestInitialize;

    //public event EventHandler<OnTestStoppingEventArgs> OnTestStopping;


    private ArgsType CreateTestExecutionEventArgs<ArgsType>(object instance, MethodInfo methodInfo, TestContext testContext, bool? failed=null) where ArgsType: TestExecutionEventArgs    
    {
      ConstructorInfo[] constructors = typeof(ArgsType).GetConstructors
        (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance);
      foreach (ConstructorInfo constructor in constructors)
      {
        ParameterInfo[] parameters = constructor.GetParameters();
        if (failed != null)
        {
          if (parameters.Length == 4 && parameters[0].ParameterType == typeof(TestContext)
                                   && parameters[1].ParameterType == typeof(MethodInfo)
                                   && parameters[2].ParameterType == typeof(object)
                                   && parameters[3].ParameterType == typeof(bool))
            return (ArgsType)constructor.Invoke(new object[] { testContext, methodInfo, instance, (bool)failed });
          if (parameters.Length == 0)
          {
            ArgsType result = (ArgsType)constructor.Invoke(new object[] { });
            SetValue<ArgsType>(result, "Instance", instance);
            SetValue<ArgsType>(result, "MethodInfo", methodInfo);
            SetValue<ArgsType>(result, "TestContext", testContext);
            return result;
          }
        }
        else
        {
          if (parameters.Length == 3 && parameters[0].ParameterType == typeof(TestContext)
                                   && parameters[1].ParameterType == typeof(MethodInfo)
                                   && parameters[2].ParameterType == typeof(object))
            return (ArgsType)constructor.Invoke(new object[] { testContext, methodInfo, instance });
          if (parameters.Length == 0)
          {
            ArgsType result = (ArgsType)constructor.Invoke(new object[] { });
            SetValue<ArgsType>(result, "Instance", instance);
            SetValue<ArgsType>(result, "MethodInfo", methodInfo);
            SetValue<ArgsType>(result, "TestContext", testContext);
            return result;
          }
        }
      }
      return null;
    }

    private void SetValue<ArgsType>(object instance, string propertyName, object value)
    {
      PropertyInfo propInfo =
        typeof(ArgsType).GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      if (propInfo != null)
        if (propInfo.CanWrite)
        {
          propInfo.SetValue(instance, value);
          return;
        }
      throw new ArgumentException(String.Format("Type {0} does not have accesible set_{1} method", typeof(ArgsType).Name, propertyName));
    }

  }
}
