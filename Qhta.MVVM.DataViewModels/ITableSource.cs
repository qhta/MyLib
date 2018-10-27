namespace Qhta.MVVM
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
