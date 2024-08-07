﻿#pragma checksum "..\..\ColorSliderTestWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "D2D5F55797688DC2472A93F0608F9C10DECB41EC"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Qhta.WPF.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using TestApp;


namespace TestApp {
    
    
    /// <summary>
    /// ColorSliderTestWindow
    /// </summary>
    public partial class ColorSliderTestWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\ColorSliderTestWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal TestApp.ColorSliderTestWindow ThisWindow;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\ColorSliderTestWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox HorizontalColorTextBox;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\ColorSliderTestWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle ColorRectangle;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\ColorSliderTestWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox VerticalColorTextBox;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\ColorSliderTestWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OK;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\ColorSliderTestWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Qhta.WPF.Controls.ColorSlider VerticalSlider;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\ColorSliderTestWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Qhta.WPF.Controls.ColorSlider HorizontalSlider;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/TestApp;component/colorslidertestwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ColorSliderTestWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.ThisWindow = ((TestApp.ColorSliderTestWindow)(target));
            return;
            case 2:
            this.HorizontalColorTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.ColorRectangle = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 4:
            this.VerticalColorTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.OK = ((System.Windows.Controls.Button)(target));
            
            #line 41 "..\..\ColorSliderTestWindow.xaml"
            this.OK.Click += new System.Windows.RoutedEventHandler(this.OK_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.VerticalSlider = ((Qhta.WPF.Controls.ColorSlider)(target));
            
            #line 48 "..\..\ColorSliderTestWindow.xaml"
            this.VerticalSlider.ValueChanged += new Qhta.WPF.Controls.ValueChangedEventHandler<System.Windows.Media.Color>(this.VerticalSlider_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.HorizontalSlider = ((Qhta.WPF.Controls.ColorSlider)(target));
            
            #line 70 "..\..\ColorSliderTestWindow.xaml"
            this.HorizontalSlider.ValueChanged += new Qhta.WPF.Controls.ValueChangedEventHandler<System.Windows.Media.Color>(this.HorizontalSlider_ValueChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

