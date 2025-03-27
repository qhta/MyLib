using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
      LoadData();
    }

    private void LoadData()
    {
      using (var context = new MyDbContext())
      {
        var ucdBlocks = context.UcdBlocks.Include(ub => ub.WritingSystem).ToList();
        var writingSystems = context.WritingSystems.ToList();

        UcdBlocksDataGrid.ItemsSource = ucdBlocks;
        ((DataGridComboBoxColumn)UcdBlocksDataGrid.Columns[2]).ItemsSource = writingSystems;
      }
    }

  }
}
