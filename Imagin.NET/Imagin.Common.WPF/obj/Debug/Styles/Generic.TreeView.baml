   M S B A M L   `   `   `  ����L  HImagin.Common.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:Imagin.CommonImagin.Common  5      C&clr-namespace:Imagin.Common.ConvertersImagin.Common.Converters  5      7 clr-namespace:Imagin.Common.LinqImagin.Common.Linq  5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0894    .Imagin.Common.Converters.IntToBooleanConverter   IntToBooleanConverter4   .Imagin.Common.Converters.ObjectToTypeConverter  ObjectToTypeConverter  �� 
TargetType �� Property  Brush.TreeView.Background �� Value  Brush.TreeView.Border  
Brush.Text+   %Imagin.Common.Linq.TreeViewExtensions �� Triggers P� Property P� Value/   )Imagin.Common.Linq.TreeViewItemExtensions �� Mode �� 
TargetType � ColumnDefinitions	 � RowDefinitions
 d�Name . *Brush.TreeViewItem.ToggleButton.Empty.Fill 0 ,Brush.TreeViewItem.ToggleButton.Empty.Stroke �� 	ClickMode d� 	IsChecked �� RelativeSource J� SnapsToDevicePixels �� Padding%   Imagin.Common.TreeViewRowHeader  Name ��Name �� Triggers �� Value   Imagin.Common.Line �� Binding �� 	Converter �� 
TargetName ]� 
Conditions �� Property �� Value 7 3Brush.TreeViewItem.ToggleButton.Filled.Checked.Fill 9 5Brush.TreeViewItem.ToggleButton.Filled.Checked.Stroke /	 +Brush.TreeViewItem.ToggleButton.Filled.Fill 1
 -Brush.TreeViewItem.ToggleButton.Filled.Stroke �� 
SourceName + 'Brush.TreeViewItem.MouseOver.Background + 'Brush.TreeViewItem.MouseOver.Foreground 4 0Brush.TreeViewItem.MouseOver.ToggleButton.Stroke * &Brush.TreeViewItem.Selected.Background*   $Imagin.Common.Linq.ControlExtensions  Brush.DropTarget.Border * &Brush.TreeViewItem.Selected.Foreground :� RoutedEvent �� TargetProperty W� From	 W� To g� Duration  g� AccelerationRatio! g� DecelerationRatio   Imagin.Common.TreeView" �� BasedOn(   "Imagin.Common.TreeViewColumnHeader % !Brush.DataGridColumnHeader.Border 3 /Brush.DataGridColumnHeader.MouseOver.Background / +Brush.DataGridColumnHeader.MouseOver.Border 3 /Brush.DataGridColumnHeader.MouseOver.Foreground 1 -Brush.DataGridColumnHeader.Pressed.Background - )Brush.DataGridColumnHeader.Pressed.Border 1 -Brush.DataGridColumnHeader.Pressed.Foreground2	   ,Imagin.Common.TreeViewColumnHeadersPresenter ) %Brush.DataGridColumnHeader.Background�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      Bmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      %localclr-namespace:Imagin.Common  5      -Common http://imagin.tech/imagin/common   5      5
Converters&clr-namespace:Imagin.Common.Converters  5      )Linq clr-namespace:Imagin.Common.Linq  5      %  &	        &	      'R� &     'Q� �    7  7  '  �    7R�'  X    '	  k       5	      6'     5
      6'   �� 5      "  R�6&   .�5   
   ��  � ��# �  6'   �� 5   
    � ��# �  6(   �� 5   
    � ��$ 1q�6,   �� 5   
    � ��# �  6'   �� 5   
    � ��$ 5q�6$   �� 5   
    �@ SelectionMode$ Single=�6B   �� 5   
    �@%�IsVirtualizing . 6=   �� 5   
    �@%�VirtualizationMode$ 	Recycling=�6A    5   
   P� 5       � {� .  . �5      ��  �@J�Opacity$	 0.6Q�6,   5      5      �� 5      "  Q�6*   .�5   
   ��  �@�OverridesDefaultStyle . 62   �� 5   
    � ��$ 1q�6,   �� 5   
    �@�Cursor$
 Hand��6#   �� 5    
    � ��# �  61   �� 5!   
    � ��$ Stretch=�67   �� 5"   
    �@Q�
IsExpanded .  6'   �� 5#   
    �@ 
IsSelected 6C   �� *
IsSelected+$ TwoWay=��� 5$   
    �@ ExpandOnClick . 6F   �� 5%   
    �@ CollapseSiblingsOnClick . 6P   �� 5&   
    �@ SelectOnRightClick . 6K   �� 5'   
    �@J�Opacity$ 0Q�6$   �� 5(   
    � ��$
 10,5q�6$   �� 5)   
    � �� 5*      �� 5+      " Q�6"   .R�5,      �  5-      �� 5.      $��15��6/   $��15 �6=   �� 5/      $
��Auto �6/   �� 50      $��* �6/   	 52      �� 53      $
��Auto �6,   �� 54      .+�57      d� $
 PART_Expander��58      #���  59      #���  5:      $��1q�5;      $��12��5<      $��12��5=      |�.  $ Press=�5@       5?      �� *
IsExpanded+ �� *TemplatedParent+��5A      �� 5B   "   "  d�6(   .�5C   &   ��  �@�OverridesDefaultStyle . 6N   �� 5D   &    � ��$ Center=�6Q   �� 5E   &    � �� 5F   *   �� 5G   .   " d�6>   .R�5H   2   A� #��z��5I   5   #��z��5J   5   #��z��5K   5   #��z��5L   5   #��z��5M   5    . 5N   5   5O   /   5Q   '   5R   #   5T      �� 5U      $��PART_Border��5X      $��1��5V      $��2��5W      #��z��5Y      #��z��5Z      #��z��5[      # z��5\       . ._�5^        $ PART_Header��5_   !   #��z��5`   !   5a      �� 5b      $ 	ItemsHost��5f      $��1��5c      $��1��5d      $��2��5e      5f      5g       5h      �� 5i      "  6x    6&   �� $ OneWay=�8   .a�5j      �� $ PART_Expander��6%    � y�$ 	Collapsed=�6V   �� 5k       � {� .  6:   5l      �� 5m      " ��6x    6&   �� $ OneWay=�8  .a�5n      �� $ PART_Expander��6%    � y�$ 	Collapsed=�6V   �� 5o       � {� .  6:   5p      ]� 5q       5r      �� 5s   "    �@��HasItems . 6@   �� 5t   "    �@Q�
IsExpanded . 6B   ./�5v      �� $ 	ItemsHost��6%    � y�$ Visible=�6R   �� 5w      $ PART_Expander��6%   # �  6V    � ���� 5x      $ PART_Expander��6%   # �  6W    � ��5y      ]� 5z       5{      �� 5|   "    �@��HasItems . 6@   �� 5}   "    �@Q�
IsExpanded .  6B   ./�5      �� $ 	ItemsHost��6%    � y�$ 	Collapsed=�6R   �� 5�      $ PART_Expander��6%   # � 	 6V    � ���� 5�      $ PART_Expander��6%   # � 
 6W    � ��5�      ]� 5�       5�      �� 5�   "    �@��HasItems .  6@   �� 5�   "    �@Q�
IsExpanded . 6B   ./�5�      �� $ 	ItemsHost��6%    � y�$ Visible=�6R   5�      ]� 5�       5�      �� 5�   "    �@��HasItems .  6@   �� 5�   "    �@Q�
IsExpanded .  6B   ./�5�      �� $ 	ItemsHost��6%    � y�$ 	Collapsed=�6R   5�      ]� 5�       5�      �� 5�   "    �@ 
IsSelected .  6^   �� 5�   "   $ PART_Border��6,    �@J�IsMouseOver . 6\   ./�5�      �� # �  6;    � ���� 5�      # �  6E    � ���� 5�      $ PART_Expander��6%   # �  6V    � ���� 5�      $ PART_Expander��6%   # �  6W    � ��5�      ]� 5�       5�      �� 5�   "    �@ 
IsSelected . 6^   ./�5�      �� $ PART_Expander��6%   # �  6V    � ���� 5�      $ PART_Expander��6%   # �  6W    � ��5�      5�      5�       5�   
   P� 5�       �@ IsDraggingOver . . �5�      ��  � ��# �  60   5�      P� 5�       �@ 
IsSelected . . �5�      ��  � ��# �  6/   �� 5�       � ��# �  69   5�      :� 5�      $ TreeViewItem.Loaded��6   .[�5�      �� .��5�      �� .��5�      W� $ Opacity�6*   $ 0X�6N   $	 1.0X�6W   $ 0:0:0.5D�6`   $	  0.4Q�6s   $	! 0.4Q�6�   5�      5�      5�      5�      �� 5�      "   6,   8"   .�5�   
   ��  �@ ColumnHeaderHeight$ 30Q�6/   5�      �� 5�      "   68   .�5�   
   ��  � ��
 ���� 6'   �� 5�   
    � ��# �  6(   �� 5�   
    � ��$ 1,0,1,1q�6,   �� 5�   
    � ��# �  6'   �� 5�   
    � ��$	 5,7q�6$    5�   
   P� 5�       �@J�IsMouseOver . . �5�      ��  � ��# �  6/   �� 5�       � ��# �  60   �� 5�       � ��# �  6/   5�      P� 5�       � �� . . �5�      ��  � ��# �  6/   �� 5�       � ��# �  60   �� 5�       � ��# �  6/   5�      5�      �� 5�      "  	 6B   .�5�   
   ��  � ��# �  6'   5�      5�      