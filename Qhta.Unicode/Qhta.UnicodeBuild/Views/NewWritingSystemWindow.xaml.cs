using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Shapes;

using Qhta.MVVM;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.ViewModels;

namespace Qhta.UnicodeBuild.Views;

public partial class NewWritingSystemWindow : Window
{
  public NewWritingSystemWindow()
  {
    InitializeComponent();
    DataContextChanged += NewWritingSystemWindow_DataContextChanged;

    AddWritingSystemCommand = new RelayCommand<WritingSystemViewModel>(AddWritingSystem, CanAddWritingSystem);
    AddWritingSystemCommand.CanExecuteChanged += (s, e) =>
    {
      //Debug.WriteLine($"AddWritingSystemCommand.CanExecuteChanged");
      CommandManager.InvalidateRequerySuggested();
    };

    CancelCommand = new RelayCommand(CancelCommand_Executed);
  }

  private void NewWritingSystemWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
  {
    if (e.NewValue is WritingSystemViewModel vm)
      vm.PropertyChanged += Vm_PropertyChanged;
  }

  private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    var addCommand = AddWritingSystemCommand;
    var ok = addCommand.CanExecute(DataContext as WritingSystemViewModel);
    addCommand.NotifyCanExecuteChanged();
  }

  public RelayCommand<WritingSystemViewModel> AddWritingSystemCommand { get; }

  private bool CanAddWritingSystem(WritingSystemViewModel? newItem)
  {
    if (newItem == null) return false;

    var ok = !String.IsNullOrEmpty(newItem.Name) && newItem.Name.Trim().Length > 0
                                                 && newItem.Type != null;
    if (ok)
    {
      if (ok)
        ok = Validate(newItem);
    }
    return ok;
  }

  private void AddWritingSystem(WritingSystemViewModel? newItem)
  {
    if (newItem == null) return;
    var ws = newItem.Model;
    ws.Id = _ViewModels.Instance.GetNewWritingSystemId();
    if (!Validate(newItem))
    {
      Debug.WriteLine("Invalid Writing System");
      return;
    }
    _ViewModels.Instance.DBContext.WritingSystems.Add(ws);
    _ViewModels.Instance.DBContext.SaveChanges();
    _ViewModels.Instance.AllWritingSystems.Add(newItem);
    if (newItem.ParentId==null)
    {
      _ViewModels.Instance.TopWritingSystems.Add(newItem);
    }
    else
    {
      var parent = _ViewModels.Instance.AllWritingSystems.FirstOrDefault(item => item.Id == newItem.ParentId);
      if (parent != null)
      {
        parent.Children?.Add(newItem);
        parent.NotifyPropertyChanged(nameof(WritingSystemViewModel.Children));
      }
    }
    DialogResult = true;
    Close();
  }

  private bool Validate(WritingSystemViewModel newItem)
  {
    var ok = !newItem.HasErrors;
    if (ok)
    {
      foreach (var item in _ViewModels.Instance.AllWritingSystems)
      {
        if (item != newItem && item.Name == newItem.Name)
        {
          newItem.AddError(nameof(newItem.Name), "Name already exists", Severity.Error);
          ok = false;
          break;
        }
      }
    }
    return ok;
  }

  public ICommand CancelCommand { get; }

  private void CancelCommand_Executed()
  {
    DialogResult = false;
    Close();
  }
}