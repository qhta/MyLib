using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.Helpers
{
  public partial class AppResourceDictionary : ResourceDictionary
  {
    public AppResourceDictionary()
    {
      InitializeComponent();
    }

    private T? FindParent<T>(DependencyObject child) where T : DependencyObject
    {
      DependencyObject? parentObject = VisualTreeHelper.GetParent(child);
      if (parentObject == null) return null;

      if (parentObject is T parent)
        return parent;
      return FindParent<T>(parentObject);
    }

    private void WrapButton_CheckedChanged(object sender, RoutedEventArgs e)
    {
      if (sender is ToggleButton button)
        if (button.DataContext is ILongTextViewModel viewModel)
        {
          var dataGrid = FindParent<SfDataGrid>(button);
          if (dataGrid != null)
          {
            var rowIndex = dataGrid.ResolveToRowIndex(viewModel);
            //Debug.WriteLine($"Resolved row index: {rowIndex}");
            dataGrid.InvalidateRowHeight(rowIndex);
            //Debug.WriteLine($"dataGrid.InvalidateRowHeight({rowIndex}) invoked");
            dataGrid.UpdateLayout();
            dataGrid.View.Refresh();
          }
        }
    }


    private bool CheckValidationErrors(TextBox textBox)
    {
      var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
      if (bindingExpression == null) return true;
      Validation.ClearInvalid(bindingExpression);
      var validationResult = new RangeValidationRule().Validate(textBox.Text, CultureInfo.CurrentCulture);
      if (!validationResult.IsValid)
      {
        Validation.MarkInvalid(bindingExpression, new ValidationError(new RangeValidationRule(), bindingExpression)
        {
          ErrorContent = validationResult.ErrorContent
        });
        return false;
      }
      return true;
    }

    private void TextBox_OnPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (sender is TextBox textBox)
      {
        // Check if the TAB or ENTER key is pressed
        if (e.Key == System.Windows.Input.Key.Tab || e.Key == System.Windows.Input.Key.Enter)
        {
          // Validate the TextBox content
          if (!CheckValidationErrors(textBox))
          {
            e.Handled = true;
            // Optionally, set focus back to the TextBox to keep the user in the field
            textBox.Focus();
          }
        }
      }
    }
  }
}