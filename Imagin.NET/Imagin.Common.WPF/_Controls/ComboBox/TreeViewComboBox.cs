﻿using Imagin.Common.Input;
using Imagin.Common.Linq;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Imagin.Common
{
    /// <summary>
    /// 
    /// </summary>
    [TemplatePart(Name = "PART_Content", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_TreeView", Type = typeof(TreeView))]
    public class TreeViewComboBox : ComboBox
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<EventArgs<object>> SelectedItemChanged;

        bool SelectionChangeHandled
        {
            get; set;
        }

        bool SelectedItemChangeHandled
        {
            get; set;
        }

        ContentControl PART_Content
        {
            get; set;
        }

        TreeView PART_TreeView
        {
            get; set;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(TreeViewComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>
        /// 
        /// </summary>
        public DataTemplate ContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ContentTemplateProperty);
            }
            set
            {
                SetValue(ContentTemplateProperty, value);
            }
        }

        #endregion

        #region TreeViewComboBox

        /// <summary>
        /// 
        /// </summary>
        public TreeViewComboBox() : base()
        {
            DefaultStyleKey = typeof(TreeViewComboBox);
            SelectionChanged += OnSelectionChanged;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!SelectionChangeHandled && e.AddedItems.Count > 0)
            {
                var i = e.AddedItems[0];

                SelectedItemChangeHandled = true;
                PART_TreeView.Select(i);

                if (PART_Content != null)
                    PART_Content.Content = i;

                SelectedItemChangeHandled = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!SelectedItemChangeHandled && !e.Handled)
            {
                e.Handled = true;

                SelectionChangeHandled = true;
                SelectedItem = e.NewValue;
                SelectionChangeHandled = false;

                PART_Content.Content = e.NewValue;

                //IsDropDownOpen = false;
            }

            SelectedItemChanged?.Invoke(this, new EventArgs<object>(e.NewValue));
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.ApplyTemplate();

            PART_Content = Template.FindName("PART_Content", this) as ContentControl;
            PART_Content.Content = SelectedItem;

            PART_TreeView = Template.FindName("PART_TreeView", this) as TreeView;
            if (PART_TreeView != null)
            {
                PART_TreeView.Resources = Resources;
                PART_TreeView.SelectedItemChanged += OnSelectedItemChanged;
            }
        }

        #endregion
    }
}
