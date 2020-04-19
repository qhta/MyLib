﻿using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Imagin.Common.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class DispatcherExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Dispatcher"></param>
        /// <param name="Action"></param>
        /// <param name="Priority"></param>
        /// <returns></returns>
        public static async Task BeginInvoke(this Dispatcher Dispatcher, Action Action, DispatcherPriority Priority = DispatcherPriority.Background)
        {
            await Dispatcher.BeginInvoke(Priority, Action);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Dispatcher"></param>
        /// <param name="Action"></param>
        /// <param name="Priority"></param>
        /// <returns></returns>
        public static object Invoke(this Dispatcher Dispatcher, Action Action, DispatcherPriority Priority = DispatcherPriority.Background)
        {
            return Dispatcher.Invoke(Priority, Action);
        }
    }
    
}
