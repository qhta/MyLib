using System;
using System.Collections.Generic;
using System.Text;

namespace MyLib.MVVM
{
  public interface ITableSource
  {
    bool CanGetData { get; }
    int ColumnsCount { get; }
    DataColumnViewModel[] GetColumns();
    int RowsCount { get; }
    DataRowViewModel[] GetRows();
  }
}
