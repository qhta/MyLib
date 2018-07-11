using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyLib.ErrorHandlingBehaviorLibrary
{
    public interface IExceptionToFaultConverter
    {
        object ConvertExceptionToFaultDetail(Exception error);
    }
}
