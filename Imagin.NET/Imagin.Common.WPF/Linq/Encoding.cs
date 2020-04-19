﻿using Imagin.Common.Text;

namespace Imagin.Common.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class EncodingExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static System.Text.Encoding GetEncoding(this Encoding encoding)
        {
            switch (encoding)
            {
                case Encoding.ASCII:
                    return System.Text.Encoding.ASCII;
                case Encoding.Unicode:
                    return System.Text.Encoding.Unicode;
                case Encoding.UTF32:
                    return System.Text.Encoding.UTF32;
                case Encoding.UTF7:
                    return System.Text.Encoding.UTF7;
                case Encoding.UTF8:
                    return System.Text.Encoding.UTF8;
            }
            return System.Text.Encoding.Default;
        }
    }
}
