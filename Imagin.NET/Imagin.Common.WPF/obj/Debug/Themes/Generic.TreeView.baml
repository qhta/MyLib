   M S B A M L   `   `   `  ����L  HImagin.Common.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:Imagin.CommonImagin.Common  5      C&clr-namespace:Imagin.Common.ConvertersImagin.Common.Converters  5      7 clr-namespace:Imagin.Common.LinqImagin.Common.Linq  5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;    5Imagin.Common.Converters.BooleanToVisibilityConverter    BooleanToVisibilityConverter(   "Imagin.Common.TreeViewColumnHeader  �� 
TargetType �� Property �� Value �� 
TargetType � ColumnDefinitions F�Name J� RenderTransformOrigin �� Stretch n�Name	 � Cursor
   GripperStyle �� Triggers P� Property P� Value �� 
TargetName �� Angle  Style.TreeView.ScrollViewer � RowDefinitions2   ,Imagin.Common.TreeViewColumnHeadersPresenter J� SnapsToDevicePixels   Columns �� RelativeSource � ContextMenu!   Imagin.Common.ResizableGrid   ItemTemplate   ColumnWidth �� ContentStringFormat �� AncestorType   SortDirection ��Name-  'System.Windows.Input.KeyboardNavigation   DirectionalNavigation �� ScrollableHeight � Maximum �� ViewportHeight �� ViewportSize  �� VerticalOffset! � Value*" �� #ComputedVerticalScrollBarVisibility# �� Orientation$ �� ScrollableWidth% �� ViewportWidth& �� HorizontalOffset,' �� %ComputedHorizontalScrollBarVisibility   Imagin.Common.TreeView/   )Imagin.Common.Linq.ItemsControlExtensions( ��Name) K� IsItemsHost�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      Bmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      %localclr-namespace:Imagin.Common  5      -Common http://imagin.tech/imagin/common   5      5
Converters&clr-namespace:Imagin.Common.Converters  5      )Linq clr-namespace:Imagin.Common.Linq  5      %�  &	        '       &	 �    '  {    7     5	      6.   �� 5      "   6   .�5   
   ��  � �� 5      �� 5      "  6"   .R�5      �  5      �� 5      $��* �6/   �� 5      $
��Auto �6/   .+�5      �� $��2��5      #��z��5      #��z��5      #��z��5      �� 5      $	��6,3q�5      #��z��5      #��z��5      F� 5      $ 	SortArrow��5      $��1��5      "����        �@B     � � @B 	#��z��5!      $��6��5"      $��0,0,8,0q�5#      $ 0.5,0.4&�5$      $
 Fill=�5%      $��Center=�5&      $y�	Collapsed=�5'      $��8��5(      n� 5)      $ PART_RightHeaderGripper��5+      $��2��5*      $	 SizeWE��5,      $��	-8,0,-9,0q�5-      $��Right=�5.      #��z
 5/      50       51      P� 52       �@ SortDirection$ 	AscendingX�6;   . �53      �� $ 	SortArrow��6%    � y�$ Visible=�6R   �� 54      $ 	SortArrow��6%    � z� 55   "   �� 56   &   $	 180Q�66   58      59      P� 5:       �@ SortDirection$ 
DescendingX�6;   . �5;      �� $ 	SortArrow��6%    � y�$ Visible=�6R   5<      5>      5@      5A      �� 5C      "  ��60   .�5D   
   ��  � �� . 6-   �� 5E   
    � ��$
 Auto=�68   �� 5F   
    � �� 5G      �� 5H      " ��6"   .R�5I      �� #��z��5J      #��z��5K      #��z��5L      ._�5M      � #��z��6    5N      �� 5O   "   $��* �63   �� 5P   "   $
��Auto �63    5R      �� 5S   "   $
��Auto �60   �� 5T   "   $��* �60   �� 5U   "   $
��Auto �60   .+�5X        $��0��5Y   !   $��2��5Z   !   # z 5^   !    5[   !   �� *TemplatedParent.Columns+ �� *TemplatedParent+ 5\   !   �� *)'TemplatedParent.ColumnHeaderContextMenu+ �� *TemplatedParent+��5]   !   �� *$"TemplatedParent.ColumnHeaderHeight+ �� *TemplatedParent+y�5_   !   �� *(&TemplatedParent.ColumnHeaderVisibility+ �� *TemplatedParent+ 5`   "   �� 5a   &   .R�5b   *      5c   -   �� *Width+��5d   -   ��  5e   -   �� **(TemplatedParent.ColumnHeaderStringFormat+ �� " ����5f   -   �� *&$TemplatedParent.ColumnHeaderTemplate+ �� " ����5g   -   �� *.,TemplatedParent.ColumnHeaderTemplateSelector+ �� " ����5h   -   �� *HorizontalContentAlignment+ 5i   -   �� *SortDirection+��5j   -   �� *VerticalContentAlignment+5k   '   5m      �� 5o      $! PART_ScrollContentPresenter��5r   !   $��2��5p   !   $��1��5q   !   $ Local=�5s   !   #��z��5t   !   �� 5v      $��PART_VerticalScrollBar��5y   !   $��1��5w   !   $��1��5x   !   # z 5z   !   # z 5{   !   #! z  5|   !   #y�z" 5}   !   �� 5~      $��PART_HorizontalScrollBar��5�   !   $��2��5   !   $# 
Horizontal=�5�   !   # z$ 5�   !   # z% 5�   !   #! z& 5�   !   #y�z' 5�   !   5�      5�      5�      5�      5�      �� 5�      "   6,   .�5�   
   �� # �@ SelectNoneOnEmptySpaceClick . 6T   �� 5�   
    �@ CanDragSelect . 6F   �� 5�   
    � ��$ Stretch=�60   �� 5�   
    � ��$ Stretch=�6.   �� 5�   
    �@%�IsVirtualizing . 6B   �� 5�   
    �@%�VirtualizationMode$ 	Recycling=�6F   �� 5�   
    � ��$
 Auto=�6G   �� 5�   
    � ��$
 Auto=�6E   �� 5�   
    �@J�SnapsToDevicePixels . 60   �� 5�   
    � �� 5�      �� 5�      "  6"   .R�5�      �� $( PART_ScrollViewer��5�      #��z��5�      #��z��5�      #��z��5�      ��. |�.  #��z��5�      8��  .��5�      � .+�5�   "   �� $��Stretch=�61   5�      5�      5�      5�      �� 5�   
    � �� 5�      �� 5�      .R�5�      $� ) . 6-   5�      5�      5�      5�      