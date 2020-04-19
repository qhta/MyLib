﻿using Imagin.Common;
using Imagin.Common.Input;
using System;
using System.ComponentModel;
using System.Windows;

namespace Imagin.Common
{
    /// <summary>
    /// 
    /// </summary>
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    public class ComboBox : System.Windows.Controls.ComboBox, IPropertyChanged
    {
        TextBox PART_TextBox;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<TextSubmittedEventArgs> Entered;

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty CheckedToolTipProperty = DependencyProperty.Register("CheckedToolTip", typeof(string), typeof(ComboBox), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>
        /// 
        /// </summary>
        public string CheckedToolTip
        {
            get
            {
                return (string)GetValue(CheckedToolTipProperty);
            }
            set
            {
                SetValue(CheckedToolTipProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(ComboBox), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>
        /// 
        /// </summary>
        public string Placeholder
        {
            get
            {
                return (string)GetValue(PlaceholderProperty);
            }
            set
            {
                SetValue(PlaceholderProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty UncheckedToolTipProperty = DependencyProperty.Register("UncheckedToolTip", typeof(string), typeof(ComboBox), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>
        /// 
        /// </summary>
        public string UncheckedToolTip
        {
            get
            {
                return (string)GetValue(UncheckedToolTipProperty);
            }
            set
            {
                SetValue(UncheckedToolTipProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ComboBox() : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_TextBox = Template.FindName("PART_TextBox", this) as TextBox;
            if (PART_TextBox != null)
                PART_TextBox.Entered += (s, e) => OnEntered((s as TextBox).Text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Text"></param>
        protected virtual void OnEntered(string Text)
        {
            Entered?.Invoke(this, new TextSubmittedEventArgs(Text));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
