   M S B A M L   `   `   `  ����P  LQhta.WPF.PropertyGrid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:Qhta.WPF.PropertyGridQhta.WPF.PropertyGrid  5      I EQhta.WPF.Utils, Version=1.1.0.0, Culture=neutral, PublicKeyToken=nullG4clr-namespace:Qhta.WPF.Utils;assembly=Qhta.WPF.UtilsQhta.WPF.Utils 5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 /  +DataTemplate.Property.ReadonlyValueTemplate  J� SnapsToDevicePixels �� Mode - )DataTemplate.Property.StringValueTemplate*   $Qhta.WPF.Utils.StringsWidthConverter  StringsWidthConverter + 'DataTemplate.Property.EnumValueTemplate �� Text �� 	Converter �� ConverterParameter , (DataTemplate.Property.Int32ValueTemplate - )DataTemplate.Property.Uint32ValueTemplate , (DataTemplate.Property.Int16ValueTemplate - )DataTemplate.Property.Uint16ValueTemplate + 'DataTemplate.Property.ByteValueTemplate .	 *DataTemplate.Property.BooleanValueTemplate d� 	IsChecked 
 PropertyGrid.NameCellStyle �� 
TargetType*  $System.Windows.Controls.DataGridCell �� Property �� Value	 �� Triggers
 P� Property P� Value  PropertyGrid.ValueCellStyle(   "Qhta.WPF.PropertyGrid.PropertyGrid&   System.Windows.Controls.DataGrid �� BasedOn  
Brush.Text)  #System.Windows.Controls.DataGridRow ]� 
Conditions �� Property �� Value �� Binding �� Path �� Value�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      -local#clr-namespace:Qhta.WPF.PropertyGrid  5      >utils4clr-namespace:Qhta.WPF.Utils;assembly=Qhta.WPF.Utils  5      %k  &	        &	 �     &	 %    &	 8    7  &	 �    &	 ~    &	     &	 �    &	     &		 �    &	
     &	     '  	    7 �� 5      .R�5	      �� 
���   �$	��0.5q�6!     . ._�5
      �� $��2,0,2,0q�5   	   $��Center=�5   	   ��5   	   �� *Value+$ OneWay=�5   	   5      5      �� 5      .R�5      �� $��Center=�5      ��5      �� *Value+5      5         5      6    �� 5      .R�5      �� $
��Left=�5      $��Center=�5       ��5      �� *Enums+ 5      �� *Value+��5      �� *Enums+8   $ 30��5       5!      �� 5#      .R�5$      �� $��80��5&      $
��Left=�5'      $��Center=�5(      ��5%      �� *Value+5(      5)      �� 5+      .R�5,      �� $��80��5.      $
��Left=�5/      $��Center=�50      ��5-      �� *Value+50      51      �� 53      .R�54      �� $��50��56      $
��Left=�57      $��Center=�58      ��55      �� *Value+58      59      �� 5;      .R�5<      �� $��50��5>      $
��Left=�5?      $��Center=�5@      ��5=      �� *Value+5@      5A      �� 5C      .R�5D      �� $��30��5F      $
��Left=�5G      $��Center=�5H      ��5E      �� *Value+5H      5I      �� 5K      .R�5L      �� $
��Left=�5N      $��Center=�5O       5M      �� *Value+5O      5P      �� 5R      "  6-   .�5S      ��  � ��
 ���� 6#   	 5T      P� 5U      
 �@J�IsKeyboardFocusWithin . . �5V   
   ��  � ��
 ���� 6(   �� 5W   
    � ��$ 0q�6,   �� 5X   
    � ��$
 Bold"�6'   �� 5Y   
    � ��
 �   �6'   5Z   	   P� 5[      
 �@J�IsKeyboardFocusWithin .  . �5\   
   ��  � ��
 ���� 6(   �� 5]   
    � ��$ 0q�6,   �� 5^   
    � ��$ Normal"�6'   �� 5_   
    � ��
 �   �6'   5`   	   5b      �� 5d      "  6.   .�5e      ��  � ��
 ���� 6#   	 5f      P� 5g      
 �@J�IsKeyboardFocusWithin . . �5h   
   ��  � ��
 ���� 6(   �� 5i   
    � ��$ 0q�6,   �� 5j   
    � ��$ Normal$�6&   �� 5k   
    � ��
 �   �6'   5l   	   P� 5m      
 �@J�IsKeyboardFocusWithin .  . �5n   
   ��  � ��
 ���� 6(   �� 5o   
    � ��$ 0q�6,   �� 5p   
    � ��$ Normal$�6&   �� 5q   
    � ��
 �   �6'   5r   	   5t      �� 5w      "  6.   8   .�5x      ��  �@ AutoGenerateColumns .  6,   �� 5y       � ��
 ���� 6#   �� 5z       � ��
 ���� 6$   �� 5{       � ��$ 0q�6(   �� 5|       �@ HeadersVisibility$
 None=�6*   �� 5}       �@ CanUserAddRows .  6'   �� 5~       �@ CanUserDeleteRows .  6*   �� 5       �@ CanUserReorderColumns .  6.   �� 5�       �@ CanUserResizeColumns .  6-   �� 5�       �@ CanUserResizeRows .  6*   �� 5�      " �@ EnableColumnVirtualization .  63   �� 5�       �@ EnableRowVirtualization .  60   �� 5�       � ��# �  6#   �� 5�        �@ HorizontalGridLinesBrush
 ���� 61   �� 5�       �@ VerticalGridLinesBrush
 ���� 6/   �� 5�       �@ SelectionMode$ Extended=�6&   �� 5�       � �� 5�      �� 5�   
   "  6   .�5�      ��  � ��$ 0,3,0,3q�6%   	 5�      ]� 5�       5�      �� 5�       �@ 
IsSelected .  62   �� 5�       �@J�IsMouseOver . 63   ./�5�      ��  � ��
 ���� 6-   5�      ]� 5�       5�      �� 5�       �@ 
IsSelected . 62   �� 5�       �@J�IsMouseOver . 63   ./�5�      ��  � ��
 ���� 6-   5�      ]� 5�       5�      �� 5�       �@ 
IsSelected . 62   �� 5�       �@��IsSelectionActive .  6B   �� 5�       �@J�IsMouseOver .  63   ./�5�      ��  � ��
 ���� 6-   5�      ]� 5�       5�      �� 5�       �@ 
IsSelected . 62   �� 5�       �@��IsSelectionActive . 6B   �� 5�       �@J�IsMouseOver .  63   ./�5�      ��  � ��
 ���� 6-   5�      ]� 5�       5�      �� 5�       �@ 
IsSelected .  62   �� 5�       �@��AlternationIndex$ 0��6E   �� 5�       �@J�IsMouseOver .  63   ./�5�      ��  � ��
 ���� 6-   5�      ]� 5�       5�      �� 5�       �@ 
IsSelected .  62   �� 5�       �@��AlternationIndex$ 1��6E   �� 5�       �@J�IsMouseOver .  63   ./�5�      ��  � ��
 ���� 6-   5�      5�      5�      �� 5�       �@�ForceCursor . 6$   	 5�      �� 5�       6   �� $ 	IsLoading� True67   .a�5�   
   ��  �@�Cursor$
 Wait��6#   5�   	   �� 5�       6   �� $ 	IsLoading�	 False67   .a�5�   
   ��  �@�Cursor$ Arrow��6#   5�   	   5�      5�      