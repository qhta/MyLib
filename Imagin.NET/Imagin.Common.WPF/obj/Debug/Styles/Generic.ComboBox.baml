   M S B A M L   `   `   `  ����L  HImagin.Common.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:Imagin.CommonImagin.Common  5      C&clr-namespace:Imagin.Common.ConvertersImagin.Common.Converters  5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;    5Imagin.Common.Converters.BooleanToVisibilityConverter    BooleanToVisibilityConverter  Template.ComboBox  �� 
TargetType � ColumnDefinitions d�Name �� 	ClickMode d� 	IsChecked �� Mode �� RelativeSource ��Name F�Name	 J� RenderTransformOrigin
 �� 	Converter �� ConverterParameter �� AncestorType �� Triggers P� Property P� Value N� EnterActions �� 
TargetName �� TargetProperty	 W� To g� Duration N� ExitActions ��Name J� IsHitTestVisible �� SelectionBoxItem �� SelectionBoxItemTemplate   Imagin.Common.TextBox  Name �� 
IsReadOnly �� 
IsReadOnly �� UpdateSourceTrigger �Name � AllowsTransparency  �� IsDropDownOpen! �Name" J� SnapsToDevicePixels# � ActualWidth$ �� MaxDropDownHeight & "Brush.ComboBox.DropDown.Background " Brush.ComboBox.DropDown.Border% K� IsItemsHost-  'System.Windows.Input.KeyboardNavigation&   DirectionalNavigation' �� 
TargetName( �� Property) �� Value  Style.ComboBox* �� 
TargetType  Brush.ComboBox.Background  Brush.ComboBox.Border  Brush.ComboBox.Foreground+ �� Triggers ' #Brush.ComboBox.MouseOver.Background #	 Brush.ComboBox.MouseOver.Border '
 #Brush.ComboBox.MouseOver.Foreground % !Brush.ComboBox.Checked.Background ! Brush.ComboBox.Checked.Border % !Brush.ComboBox.Checked.Foreground, �� BasedOn   Imagin.Common.ComboBox!   Imagin.Common.FontFamilyBox   Imagin.Common.FontSizeBox$   Imagin.Common.TreeViewComboBox ! Brush.ComboBoxItem.Foreground- �� Padding * &Brush.ComboBoxItem.Selected.Background * &Brush.ComboBoxItem.Selected.Foreground. ]� 
Conditions/ �� Property0 �� Value + 'Brush.ComboBoxItem.MouseOver.Background�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      Bmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      %localclr-namespace:Imagin.Common  5      -Common http://imagin.tech/imagin/common   5      5
Converters&clr-namespace:Imagin.Common.Converters  5      %�  &	        &	      7   &	 �	    '�� �    7  7  '  ;    7��'  ]    7��'      7��'  �    7  '�� �       5      6.   �� 5
      "  ��60   .R�5   
   �  5      �� 5      $��* �6#   �� 5      $��15 �6#   .+�5      d� $ PART_ToggleButton��5      $��2��5      #��z��5      #��z��5      #��z��5      #��z��5      |�.  $ Press=�5       5      �� *IsDropDownOpen+$ TwoWay=� �� *TemplatedParent+��5      �� 5      "  d�6&   .R�5      �  5      �� 5   "   �� 5   "   $��20 �63   .+�5!      �� $ Border��5"   !   $��2��5#   !   #��z��5$   !   #��z��5%   !   #��z��5&   !   F� 5'      $ 
PART_Arrow��5)   !   $��1��5(   !   =����        �    �� � 	= ��[ � z �� � z     � 	=  	= 	#��z��5+   !   $��Center=�5,   !   $	 0.5, 0.5&�5-   !   $��Center=�5.   !   y�5/   !   �� *
IsReadOnly+8
   $ Inverted�� �� " ��z�50   "   �� 51   &   53      54       55      P� 56       �@d�	IsChecked$
 TrueY�6;    57   "   �� 58   &   .��59   *   �� .��5:   .   W� $ 
PART_Arrow��6>   $4 .(Path.RenderTransform).(RotateTransform.Angle)�6a   $ 90X�6�   $ 0:0:0.4D�6�   5;   +   5<   '    5>   "   �� 5?   &   .��5@   *   �� .��5A   .   W� $ 
PART_Arrow��6>   $4 .(Path.RenderTransform).(RotateTransform.Angle)�6a   $ 0X�6�   $ 0:0:0.4D�6�   5B   +   5C   '   5E      5G      5I      �� 5J      $ ContentSite��5K       .  #��z 5M      #��z 5N      #��z��5O      $
��Left=�5P      #��z��5Q      $��Center=�5R      5S        5T      $ PART_TextBox��5U      |�. 
������ $��0q�5X      #��z��5Y      $	��8,0q�5Z      $��0q�5[      $y�	Collapsed=�5\      # z 5^      ��5]      �� *Text+$ TwoWay=� �� *TemplatedParent+$ PropertyChanged=�5^      � 5_      $ Popup��5`       . |�.  #��z  5c      $��Bottom=�5d      $��Slide=�5e      .��5f      � $! DropDown��5g      " . #��z# 5i      #��z$ 5j      .+�5k      �� $ DropDownBorder��5l      #���  5m      #���  5n      $��1q�5o      �� 5p      $��1,2,1,1q�6#   " . .��5q      �� % . $& 	Contained=�68   5r      5s      5t      5u       5v   
   P� 5w       �@��HasItems .  . �5x      �� $' DropDownBorder��6   ( � ��$	) 125��6J   5y      P� 5z       �@��
IsEditable . . �5{      �� $' PART_TextBox��6   ( � y�$) Visible=�6I   �� 5|      $' ContentSite��6   ( � y�$) Hidden=�6H   5}      5      �� 5�      "* ��6#   .�5�   
   �� ( � ��#) �  6'   �� 5�   
   ( � ��#) �  6(   �� 5�   
   ( � ��$) 1q�6,   �� 5�   
   ( � ��#) �  6'   �� 5�   
   ( � ��$	) 7,5q�6$   �� 5�   
   ( � ��) . 6:   �� 5�   
   ( � ��$
) Auto=�6G   �� 5�   
   ( � ��$
) Auto=�6E   �� 5�   
   ( �@J�SnapsToDevicePixels) . 60   �� 5�   
   ( � ��$
) Auto��6"   + 5�   
   P� 5�       �@J�IsMouseOver . . �5�      �� ( � ��#) �  6/   �� 5�      ( � ��#) � 	 60   �� 5�      ( � ��#) � 
 6/   5�      P� 5�       �@��
IsGrouping . . �5�      �� ( � ��) .  6B   5�      P� 5�       �@��
IsEditable . . �5�      �� ( � ��) .  6.   5�      P� 5�       �@��IsDropDownOpen . . �5�      �� ( � ��#) �  6/   �� 5�      ( � ��#) �  60   �� 5�      ( � ��#) �  6/   5�      P� 5�       � {� .  . �5�      �� ( �@J�Opacity$	) 0.6Q�6,   5�      5�      �� 5�      "* ��6&   8,   .�5�   
   �� ( �@�OverridesDefaultStyle) . 62   �� 5�   
   ( � ��8)  6%   5�      �� 5�      "*  6,   8,   6Q   �� 5�      "*  61   8,   6[   �� 5�      "*  6/   8,   6W   �� 5�      "*  64   8,   6a   �� 5�      "* ��6*   .�5�   
   �� ( �@�OverridesDefaultStyle) . 62   �� 5�   
   ( � ��
) ���� 6'   �� 5�   
   ( � ��#) �  6'   �� 5�   
   ( � ��$	) 6,5q�6$   �� 5�   
   ( �@J�SnapsToDevicePixels) . 60   �� 5�   
   ( � ��) 5�      �� 5�      "  ��6"   .R�5�      �� $ Border��5�      #��z��5�      #- z��5�      " . ._�5�      �  5�      �� 5�   "   $
��Auto �63   �� 5�   "   $
��Auto �63   �� 5�   "   $��* �63   .+�5�      �� $��Center=�6/   5�      5�       5�      P� 5�       � {� .  . �5�      �� ( �@J�Opacity$	) 0.6Q�68   5�      P� 5�       �@��
IsSelected . . �5�      �� #) �  6;   ( � ���� 5�      #) �  6;   ( � ��5�      ]� 5�      . 5�      �� 5�   "   / �@J�IsMouseOver0 . 6C   �� 5�   "   / �@��
IsSelected0 .  6B   ./�5�      �� #) �  6;   ( � ��5�      5�      5�      5�      5�      