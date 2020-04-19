﻿using System;
using System.Windows;

namespace Imagin.Common.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Duration ToDuration(this TimeSpan t)
        {
            return new Duration(t);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ToShortTime(this TimeSpan t)
        {
            if (t.TotalSeconds == 0) return string.Empty;
            string Result = string.Empty;
            if (t.Hours > 0)
            {
                Result += string.Format("{0}h ", t.Hours.ToString());
            }
            if (t.Minutes > 0)
            {
                Result += string.Format("{0}m ", t.Minutes.ToString());
            }
            if (t.Seconds > 0)
            {
                Result += string.Format("{0}s", t.Seconds.ToString());
            }
            return Result;
        }
    }
}
