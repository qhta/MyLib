using System.Windows;
using System.Windows.Input;

using Qhta.DeepCopy;
using Qhta.MVVM;
using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.ViewModels;

namespace Qhta.UnicodeBuild.Views;

/// <summary>
/// Window for editing a writing system.
/// </summary>
public partial class EditWritingSystemWindow : Window
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EditWritingSystemWindow"/> class.
  /// </summary>
  public EditWritingSystemWindow()
  {
    InitializeComponent();
    //DataContextChanged += EditWritingSystemWindow_DataContextChanged;

    OkCommand = new RelayCommand<WritingSystemViewModel>(SaveData);
    CancelCommand = new RelayCommand(CancelCommand_Executed);
  }

  /// <summary>
  /// AddMode property indicates whether the window is in add mode or edit mode.
  /// </summary>
  public bool AddMode
  {
    get => (bool)GetValue(AddModeProperty);
    set => SetValue(AddModeProperty, value);
  }

  /// <summary>
  /// DependencyProperty for the AddMode property.
  /// </summary>
  public static DependencyProperty AddModeProperty = DependencyProperty.Register(
    nameof(AddMode),
    typeof(bool),
    typeof(EditWritingSystemWindow),
    new PropertyMetadata(false, AddModePropertyChanged));

  private static void AddModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    (d as EditWritingSystemWindow)?.AddModeChanged(e.NewValue);
  }

  private void AddModeChanged(object? newValue)
  {
    if (newValue is bool addMode)
    {
      Title = addMode
        ? Qhta.UnicodeBuild.Resources.WritingSystem.NewWritingSystemTitle
        : Qhta.UnicodeBuild.Resources.WritingSystem.EditWritingSystemTitle;
      OkCommand.NotifyCanExecuteChanged();
    }
  }


  /// <summary>
  /// OK command that is executed when the user clicks the OK button.
  /// </summary>
  public RelayCommand<WritingSystemViewModel> OkCommand { get; }


  private void SaveData(WritingSystemViewModel? newItem)
  {
    if (newItem == null) return;
    //if (!Validate(newItem))
    //  return;

    var ws = newItem.Model;
    var existingItem = _ViewModels.Instance.WritingSystems.FirstOrDefault(item => item.Id == ws.Id);
    if (existingItem != null)
    {
      // Update existing item in the database and the view model collection
      var changeParentChildren = existingItem.ParentId != newItem.ParentId;
      if (changeParentChildren)
      {
        // Remove existing item from its parent's children collection if it has a parent
        if (existingItem.Parent != null)
        {
          existingItem.Parent.Children?.Remove(existingItem);
          existingItem.Parent.NotifyPropertyChanged(nameof(WritingSystemViewModel.Children));
        }
        else
        {
          existingItem.NotifyPropertyChanged(nameof(WritingSystemViewModel.ParentId));
        }
      }
      // Copy properties from newItem to existingItem
      DeepCopier.CopyDeep<WritingSystemViewModel>(existingItem, newItem);
      _ViewModels.Instance.DbContext.SaveChanges();
      if (changeParentChildren)
      {
        // Add the updated item to the new parent's children collection if it has a parent
        if (newItem.Parent != null)
        {
          newItem.Parent.Children?.Add(newItem);
          newItem.Parent.NotifyPropertyChanged(nameof(WritingSystemViewModel.Children));
        }
        else
        {
          existingItem.NotifyPropertyChanged(nameof(WritingSystemViewModel.ParentId));
        }
      }
    }
    else
    {
      // Add a new writing system to the database and the view model collection
      _ViewModels.Instance.DbContext.WritingSystems.Add(ws);
      _ViewModels.Instance.DbContext.SaveChanges();
      _ViewModels.Instance.WritingSystems.Add(newItem);
      if (newItem.ParentId != null)
      {
        var parent = _ViewModels.Instance.WritingSystems.FirstOrDefault(item => item.Id == newItem.ParentId);
        if (parent != null)
        {
          parent.Children?.Add(newItem);
          parent.NotifyPropertyChanged(nameof(WritingSystemViewModel.Children));
        }
      }
      else
      {
        newItem.NotifyPropertyChanged(nameof(WritingSystemViewModel.ParentId));
      }
    }

    DialogResult = true;
    Close();
  }

  //private bool Validate(WritingSystemViewModel newItem)
  //{
  //  var ok = !newItem.HasErrors;
  //  if (ok)
  //  {
  //    foreach (var item in _ViewModels.Instance.WritingSystems)
  //    {
  //      if (item != newItem && item.Id!=newItem.Id && item.Name == newItem.Name && item.Type == newItem.Type)
  //      {
  //        if (newItem.Type != null)
  //        {
  //          var errorMsg = Qhta.UnicodeBuild.Resources.WritingSystem.NameAlreadyExistsMessage ??
  //                         "{0} already exists.";
  //          errorMsg = string.Format(errorMsg,
  //            Qhta.UnicodeBuild.Resources.WritingSystemType.ResourceManager.GetString(item.Type.ToString()!) + " " + item.Name);

  //          newItem.AddError(nameof(newItem.Name),
  //            errorMsg, Severity.Error);
  //          ok = false;
  //        }
  //        break;
  //      }
  //    }
  //  }
  //  return ok;
  //}

  /// <summary>
  /// Cancel command that is executed when the user clicks the Cancel button.
  /// </summary>
  public ICommand CancelCommand { get; }

  private void CancelCommand_Executed()
  {
    DialogResult = false;
    Close();
  }
}