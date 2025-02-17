﻿using System.Linq;
using Qhta.DispatchedObjects;

namespace Qhta.MVVM
{
  public abstract class DataSetViewModel<RowViewModelType>: DataSetViewModel, ITableSource
    where RowViewModelType: DataRowViewModel
  {

    public DispatchedCollection<RowViewModelType> Items => _Items;

    DispatchedCollection<RowViewModelType> _Items = new DispatchedCollection<RowViewModelType>();

    public bool CanGetData => Items.Count>0;

    public int ColumnsCount => VisibleColumns.Count;

    public int RowsCount => Items.Count;

    public DataColumnViewModel[] GetColumns()
    {
      return VisibleColumns.ToArray();
    }

    public DataRowViewModel[] GetRows()
    {
      return Items.ToArray();
    }
  }
}
