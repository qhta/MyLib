   M S B A M L   `   `   `  ����I  EQhta.WPF.Utils, Version=3.5.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:Qhta.WPF.UtilsQhta.WPF.Utils  5      ;"clr-namespace:Qhta.WPF.Utils.ViewsQhta.WPF.Utils.Views  5      E'clr-namespace:Qhta.WPF.Utils.ViewModelsQhta.WPF.Utils.ViewModels  5      )    #Qhta.WPF.Utils.Views.EnumFilterView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .localclr-namespace:Qhta.WPF.Utils     5      4views"clr-namespace:Qhta.WPF.Utils.Views     5      6vm'clr-namespace:Qhta.WPF.Utils.ViewModels     5      $	��300��5      .��5      � $
��Auto��6	   !  System.Windows.SystemColors    ControlBrushKey#���   5   	    � RowDefinitions 5      �� 5      $
��Auto �6   �� 5      $��* �6   $��30��5      .+�5      � $��0��6    � ColumnDefinitions 5      �� 5   
   $��* �6   �� 5   
   $��* �6   .+�5      �� .+�5   
   	�  	� 	GroupName$ SelectFunction��6   "   Qhta.WPF.Utils.CommonStrings   IsEqual#��Z 5       d� 	IsChecked 5      ��  �� Path$ IsEqual�5      	� 5   
   $ SelectFunction��6      NotEqual#��Z 5        5      �� $ NotEqual�5       5!   	   �� 5"      $��1��6   .+�5#   
   	� $ SelectFunction��6      IsEmpty#��Z 5%       5$      �� $ IsEmpty�5%      	� 5&   
   $ SelectFunction��6   	   NotEmpty#��Z	 5(       5'      �� $ NotEmpty�5(      5)   	   5*      �� 5+      -   5        
 ��Name$
 ComboBox��5+      $��1��5,      $��4q�5-      $
��Auto��5.       �� 
IsEditable$ False��5/      $ �� IsSynchronizedWithCurrentItem$
 TrueY�50      ��51      �� *
EnumValues+ �� SelectedItem 52      �� *FilterValue+53      54      