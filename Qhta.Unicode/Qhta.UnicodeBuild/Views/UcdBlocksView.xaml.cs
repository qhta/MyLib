using System.Windows.Controls;

using Microsoft.EntityFrameworkCore;

using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.Views
{
  /// <summary>
  /// Interaction logic for UcdBlocksView.xaml
  /// </summary>
  public partial class UcdBlocksView : UserControl
  {
    public UcdBlocksView()
    {
      InitializeComponent();
      //LoadData();
    }

    //private void LoadData()
    //{
    //  using (var context = new _DbContext())
    //  {
    //    var ucdBlocks = context.UcdBlocks.Include(ub => ub.WritingSystem).ToList();
    //    var writingSystems = context.WritingSystems.ToList();

    //    UcdBlocksDataGrid.ItemsSource = ucdBlocks;
    //    ((DataGridComboBoxColumn)UcdBlocksDataGrid.Columns[2]).ItemsSource = writingSystems;
    //  }
    //}

  }
}
