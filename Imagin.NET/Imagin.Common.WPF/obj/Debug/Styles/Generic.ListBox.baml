   M S B A M L   `   `   `  ����L  HImagin.Common.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:Imagin.CommonImagin.Common  5      C&clr-namespace:Imagin.Common.ConvertersImagin.Common.Converters  5      7 clr-namespace:Imagin.Common.LinqImagin.Common.Linq  5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0894    .Imagin.Common.Converters.ObjectToTypeConverter   ObjectToTypeConverter  FadeInAnimation  �� TargetProperty W� From	 W� To g� Duration g� AccelerationRatio g� DecelerationRatio �� 
TargetType �� Property �� Value  
Brush.Text	 �� 
TargetType
 �� Padding J� SnapsToDevicePixels �� Triggers :� RoutedEvent*   $Imagin.Common.Linq.ControlExtensions P� Property P� Value  Brush.DropTarget.Border ]� 
Conditions �� Property �� Value 0 ,Brush.ListBoxItem.Selected.Active.Background 0 ,Brush.ListBoxItem.Selected.Active.Foreground ) %Brush.ListBoxItem.Selected.Background ) %Brush.ListBoxItem.Selected.Foreground * &Brush.ListBoxItem.MouseOver.Background *	 &Brush.ListBoxItem.MouseOver.Foreground �� BasedOn 
 Style.ListView.ScrollViewer � ColumnDefinitions � RowDefinitions  Brush.GridView.Background �� Columns �� RelativeSource! �� ColumnHeaderContainerStyle �� ColumnHeaderTemplate# �� ColumnHeaderTemplateSelector �� AllowsColumnReorder �� ColumnHeaderContextMenu �� ColumnHeaderToolTip �� CanContentScroll-  'System.Windows.Input.KeyboardNavigation   DirectionalNavigation  �� Orientation! �� ScrollableWidth" � Maximum# �� ViewportWidth$ �� ViewportSize% �� HorizontalOffset& � Value,' �� %ComputedHorizontalScrollBarVisibility( �� ScrollableHeight) �� ViewportHeight* �� VerticalOffset*+ �� #ComputedVerticalScrollBarVisibility  Brush.ListBox.Background  Brush.ListBox.Border*   $Imagin.Common.Linq.ListBoxExtensions/   )Imagin.Common.Linq.ItemsControlExtensions, ��Name- �� Triggers. ^� 
Conditions/ �� Binding0 �� Mode1 �� 	Converter2 �� 
TargetName   Imagin.Common.ListView�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      Bmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      %localclr-namespace:Imagin.Common  5      -Common http://imagin.tech/imagin/common   5      5
Converters&clr-namespace:Imagin.Common.Converters  5      )Linq clr-namespace:Imagin.Common.Linq  5      %�  &	        &	      '�� �     7  '�� g    7��&	
 �    '��     7   7   7 
 '�� �    7��'      7��   5	      6'   �� 5      .��5   
   W� $  Opacity�6   $ 0X�6>   $	 1.0X�6G   $ 0:0:0.5D�6P   $	 0.4Q�6c   $	 0.4Q�6{   5      �� 5      " ��6)   .�5   
   ��  � ��
 ���� 6'   �� 5   
    � ��$ 1q�6,   �� 5   
    � ��# �  6'   �� 5   
    �@J�Opacity$ 0Q�6$   �� 5   
    � ��$ 5q�6$   �� 5   
    � �� 5      �� 5      "	 ��6"   .R�5      �� $��PART_Border��5      #��z��5      #��z��5      #��z��5      #
 z��5       . ._�5      �� 5       5!      5#       5$   
   :� 5%      $ ListBoxItem.Loaded��6   .[�5&      �� 8��  6"   5'      P� 5(       �@ IsDraggingOver . . �5)      ��  � ��# �  60   5*      ]� 5+       5,      �� 5-       �@��
IsSelected . 66   �� 5.       �@��IsSelectionActive . 6F   ./�50      ��  � ��# �  6/   �� 51       � ��# �  6/   52      ]� 53       54      �� 55       �@��
IsSelected . 66   �� 56       �@��IsSelectionActive .  6F   ./�58      ��  � ��# �  6/   �� 59       � ��# �  6/   5:      ]� 5;       5<      �� 5=       �@��
IsSelected .  66   �� 5>       �@J�IsMouseOver . 67   ./�5@      ��  � ��# �  6/   �� 5A       � ��# � 	 6/   5B      5D      �� 5F      " ��6*   8   6M   �� 5H      " ��60   .�5I   
   ��  � �� . 6-   �� 5J   
    � ��$
 Auto=�68   �� 5K   
    � �� 5L      �� 5M      "	 ��6"   .R�5N      �� #��z��5O      #��z��5P      #��z��5Q      ._�5R      �  5S      �� 5T   "   $��* �63   �� 5U   "   $
��Auto �63    5W      �� 5X   "   $
��Auto �60   �� 5Y   "   $��* �60   �� 5Z   "   $
��Auto �60   .+�5]      �� $��2��5^   !   #���  5_   !   �� 5`      $��2��5a   !   $��Stretch=�5b   !   # z 5j   !    5c   !   �� *TemplatedParent.View.Columns+ �� *TemplatedParent+ 5d   !   �� *1/TemplatedParent.View.ColumnHeaderContainerStyle+ �� *TemplatedParent+ 5e   !   �� *+)TemplatedParent.View.ColumnHeaderTemplate+ �� *TemplatedParent+ 5f   !   �� *31TemplatedParent.View.ColumnHeaderTemplateSelector+ �� *TemplatedParent+ 5g   !   �� **(TemplatedParent.View.AllowsColumnReorder+ �� *TemplatedParent+ 5h   !   �� *.,TemplatedParent.View.ColumnHeaderContextMenu+ �� *TemplatedParent+ 5i   !   �� **(TemplatedParent.View.ColumnHeaderToolTip+ �� *TemplatedParent+5j   !   �� 5l      $!��PART_ScrollContentPresenter��5n   !   $��1��5m   !   # z��5o   !   $ Local=�5p   !   �� 5r      $��PART_HorizontalScrollBar��5s   !   $  
Horizontal=�5t   !   $��2��5u   !   #" z! 5v   !   #$ z# 5w   !   #& z% 5x   !   #y�z' 5y   !   �� 5z      $��PART_VerticalScrollBar��5{   !   $��1��5|   !   $��1��5}   !   #" z( 5~   !   #$ z) 5   !   #& z* 5�   !   #y�z+ 5�   !   5�      5�      5�      5�      5�      �� 5�      " ��6%   .�5�   
   ��  � ��# �  6'   �� 5�   
    � ��# �  6(   �� 5�   
    � ��$ 1q�6,   �� 5�   
    � ��$ Disabled=�6G   �� 5�   
   % �@ IsDirectionalSelectionEnabled . 6Q   �� 5�   
    �@%�IsVirtualizing . 6=   �� 5�   
   " �@%�IsVirtualizingWhenGrouping . 6I   �� 5�   
    � ��$ 5q�6$   �� 5�   
    �@%�
ScrollUnit$ Pixel=�69   �� 5�   
   # �@ SelectNoneOnEmptySpaceClick . 6T   �� 5�   
    �@ SelectionWrap . 6A   �� 5�   
    � ��$ Disabled=�6E   �� 5�   
    �@%�VirtualizationMode$ 	Recycling=�6A   �� 5�   
    � �� 5�      �� 5�      "	 ��6"   .R�5�      �� $, PART_ScrollViewer��5�      #��z��5�      #��z��5�      #��z��5�      .��5�      � .+�5�      �� #��z��6-   5�      5�      - 5�      ^� 5�      . 5�      �� 5�   "   " ��6�   / 6,   �� $0 OneWay=�81    �� *Self+6�   �� 5�   "   " ��6�   / 6,   �� *View+$0 OneWay=�81   �� *Self+6�   .0�5�      �� $2 PART_ScrollViewer��6%   8   � ��6U   5�      P� 5�       �@��
IsGrouping . . �5�      ��  � �� .  6N   5�      5�      5�      5�      �� 5�      " ��6&   8   6E   �� 5�      "  6,   8   6Q   5�      