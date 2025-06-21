using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

using Qhta.DeepCopy;
using Qhta.MVVM;
using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.ViewModels;

namespace Qhta.UnicodeBuild.Views;

public partial class EditWritingSystemWindow : Window
{
  public EditWritingSystemWindow()
  {
    InitializeComponent();
    DataContextChanged += EditWritingSystemWindow_DataContextChanged;

    AddWritingSystemCommand = new RelayCommand<WritingSystemViewModel>(SaveData, CanAddWritingSystem);
    AddWritingSystemCommand.CanExecuteChanged += (s, e) =>
    {
      //Debug.WriteLine($"AddWritingSystemCommand.CanExecuteChanged");
      CommandManager.InvalidateRequerySuggested();
    };

    CancelCommand = new RelayCommand(CancelCommand_Executed);
  }

  public static DependencyProperty AddModeProperty = DependencyProperty.Register(
    nameof(AddMode),
    typeof(bool),
    typeof(EditWritingSystemWindow),
    new PropertyMetadata(false));

  public bool AddMode
  {
    get => (bool)GetValue(AddModeProperty);
    set => SetValue(AddModeProperty, value);
  }

  private void EditWritingSystemWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
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
    return true;
    //if (newItem == null) return false;

    //var ok = !String.IsNullOrEmpty(newItem.Name) && newItem.Name.Trim().Length > 0
    //                                             && newItem.Type != null;
    //if (ok)
    //{
    //  if (ok)
    //    ok = Validate(newItem);
    //}
    //return ok;
  }

  private void SaveData(WritingSystemViewModel? newItem)
  {
    if (newItem == null) return;
    var ws = newItem.Model;
    ws.Id = _ViewModels.Instance.GetNewWritingSystemId();
    if (!Validate(newItem))
    {
      Debug.WriteLine("Invalid Writing System");
      return;
    }
    var existingItem = _ViewModels.Instance.AllWritingSystems.FirstOrDefault(item => item.Id == ws.Id);
    if (existingItem != null)
    {
      Debug.WriteLine($"Writing System with Id {ws.Id} already exists in the database.");
      DeepCopier.CopyDeep<WritingSystemViewModel>(existingItem, newItem);
    }
    else
    {

      _ViewModels.Instance.DBContext.WritingSystems.Add(ws);
      _ViewModels.Instance.DBContext.SaveChanges();
      _ViewModels.Instance.AllWritingSystems.Add(newItem);
      if (newItem.ParentId == null)
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
        if (item != newItem && item.Id!=newItem.Id && item.Name == newItem.Name && item.Type == newItem.Type)
        {
          if (newItem.Type != null)
          {
            var errorMsg = Qhta.UnicodeBuild.Resources.WritingSystem.NameAlreadyExistsMessage ??
                           "{0} already exists.";
            errorMsg = string.Format(errorMsg,
              Qhta.UnicodeBuild.Resources.WritingSystemType.ResourceManager.GetString(item.Type.ToString()!) + " " + item.Name);

            newItem.AddError(nameof(newItem.Name),
              errorMsg, Severity.Error);
            ok = false;
          }
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