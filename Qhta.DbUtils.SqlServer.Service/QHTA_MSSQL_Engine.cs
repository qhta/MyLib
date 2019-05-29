using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Qhta.DbUtils.SqlServer.Service
{
  public partial class QHTA_MSSQL_Engine : ServiceBase
  {
    public QHTA_MSSQL_Engine()
    {
      InitializeComponent();
      eventLog1 = new System.Diagnostics.EventLog();
      if (!System.Diagnostics.EventLog.SourceExists("MySource"))
      {
        System.Diagnostics.EventLog.CreateEventSource(
            "MySource", "MyNewLog");
      }
      eventLog1.Source = "MySource";
      eventLog1.Log = "MyNewLog";
    }

    protected override void OnStart(string[] args)
    {
      eventLog1.WriteEntry("In OnStart.");
      // Set up a timer that triggers every minute.
      Timer timer = new Timer();
      timer.Interval = 60000; // 60 seconds
      timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
      timer.Start();
    }

    public void OnTimer(object sender, ElapsedEventArgs args)
    {
      // TODO: Insert monitoring activities here.
      eventLog1.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);
    }
    int eventId = 1;

    protected override void OnStop()
    {
      eventLog1.WriteEntry("In OnStop.");
    }

    protected override void OnContinue()
    {
      eventLog1.WriteEntry("In OnContinue.");
    }
  }
}
