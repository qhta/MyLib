   M S B A M L   `   `   `  ����I  EQhta.WPF.Utils, Version=3.5.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:Qhta.WPF.UtilsQhta.WPF.Utils  5      ;"clr-namespace:Qhta.WPF.Utils.ViewsQhta.WPF.Utils.Views  5      E'clr-namespace:Qhta.WPF.Utils.ViewModelsQhta.WPF.Utils.ViewModels  5      '    !Qhta.WPF.Utils.Views.FilterDialog   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      .localclr-namespace:Qhta.WPF.Utils     5      4views"clr-namespace:Qhta.WPF.Utils.Views     5      6vm'clr-namespace:Qhta.WPF.Utils.ViewModels     5      Qshell?http://schemas.microsoft.com/winfx/2006/xaml/presentation/shell     5	      -   5           Qhta.WPF.Utils.ToolWindow   Name$  
MainWindow��5
      $	��300��5       � SizeToContent$ WidthAndHeight=�5         ToolWindowStyle#��[  5       � ShowInTaskbar$ False��5      .��5      � -   5         �Name$ MainGrid��5   	   $
��Auto��5   	   $
��Auto��5   	   !  System.Windows.SystemColors   ControlBrushKey#���  5   	    � RowDefinitions 5      �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5      ,   &Qhta.WPF.Utils.Views.GenericFilterView  -   5          Name$ 
FilterView��5      $��0��5      $��2��5       � DataContext 5      �� *EditedInstance+5      �� 5!      $��1��6    �� Orientation$ 
Horizontal=�5"      $
��Left=�5#      $
��0,10q�5$      .+�5%      �� -   5        	 ��Name$	 ApplyButton��5%      "   Qhta.WPF.Utils.CommonStrings
   Apply#��Z
 5&      $��50��5'      $
��10,5q�5(      ��5)      �� *ApplyFilterCommand+�� 5*      -   5        $	 OkButton��5*      	   OK#��Z 5+      $��50��5,      $
��10,5q�5-       �� 	IsDefault$
 True��5.      5/      �� 50      -   5        $	 CancelButton��50         Cancel#��Z 51      $��50��52      $
��10,5q�53       �� IsCancel$
 True��54      55      �� 56      -   5        $	 ClearButton��56         Clear#��Z 57      $��50��58      $
��10,5q�59      5:      5;      5<      5=      