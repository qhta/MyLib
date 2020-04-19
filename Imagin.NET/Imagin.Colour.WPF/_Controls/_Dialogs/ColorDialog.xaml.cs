﻿using Imagin.Common.Input;
using System.Windows;
using System.Windows.Media;

namespace Imagin.Colour.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ColorDialog : SolidColorBrushDialog
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string DefaultTitle => "Select Color";

        /// <summary>
        /// 
        /// </summary>
        public ColorDialog() : this(string.Empty, Brushes.Transparent, null) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="initialValue"></param>
        /// <param name="chip"></param>
        public ColorDialog(string title, SolidColorBrush initialValue, Chip<SolidColorBrush> chip = null) : base(title, initialValue, chip)
        {
            InitializeComponent();
            PART_ColorPicker.OldColor = InitialValue.Color;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnRevert(object sender, RoutedEventArgs e)
            => PART_ColorPicker.NewColor = PART_ColorPicker.OldColor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSelectedColorChanged(object sender, EventArgs<Color> e)
        {
            var v = new SolidColorBrush(e.Value);

            Value = v;

            if (Chip != null && Chip.IsSynchronized)
                Chip.Value = v;
        }
    }
}
