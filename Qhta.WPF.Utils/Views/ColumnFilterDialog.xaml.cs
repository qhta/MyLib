﻿namespace Qhta.WPF.Utils.Views;

/// <summary>
/// Main dialog window for specific filter views.
/// </summary>
public partial class ColumnFilterDialog : Window
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public ColumnFilterDialog()
  {
    InitializeComponent();
    Activated += Window_Activated;
  }

  private void Window_Activated(object? sender, EventArgs e)
  {
    MinHeight = ActualHeight;
  }

  private void OkButton_Click(object sender, RoutedEventArgs e)
  {
    DialogResult = true;
    Close();
  }

  private void CancelButton_Click(object sender, RoutedEventArgs e)
  {
    DialogResult = false;
    Close();
  }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}