   M S B A M L   `   `   `  ����a  ]PresentationFramework.Aero, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35gJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.AeroMicrosoft.Windows.Themes  5       Qhta.WPF.UtilsG4clr-namespace:Qhta.WPF.Utils;assembly=Qhta.WPF.UtilsQhta.WPF.Utils 5      N JQhta.WPF.Converters, Version=1.3.2.0, Culture=neutral, PublicKeyToken=nullV>clr-namespace:Qhta.WPF.Converters;assembly=Qhta.WPF.ConvertersQhta.WPF.Converters 5      +   %Qhta.WPF.Utils.CollectionViewBehaviorW SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089    BooleanToVisibilityConverter6  0Qhta.WPF.Converters.NotNullToVisibilityConverter   NotNullToVisibilityConverter  �� 
TargetType �� TypeInTargetAssembly&   System.Windows.Controls.DataGrid �� 
ResourceId �� Property �� Value �� 
TargetType!  System.Windows.SystemColors   ControlBrushKey J� SnapsToDevicePixels J� Opacity	 �� Stretch
 � Points �� Triggers P� Property P� Value �� 
TargetName   ControlDarkBrushKey   ControlTextBrushKey �� Padding � RowDefinitions � Orientation �� Path �� RelativeSource �� Mode �� AncestorType �� 	Converter � Maximum � Value ��Name � ColumnDefinitions   SelectAllCommand!   HeadersVisibilityConverter7  1System.Windows.Controls.DataGridHeadersVisibility
   All  �� ConverterParameterG  ASystem.Windows.Controls.Primitives.DataGridColumnHeadersPresenter!   Column" �� CanContentScroll# �� Orientation$ �� ScrollableHeight% �� ViewportHeight& �� ViewportSize*' �� #ComputedVerticalScrollBarVisibility( �� Mode) �� ScrollableWidth* �� ViewportWidth,+ �� %ComputedHorizontalScrollBarVisibility, �� Triggers- ]� 
Conditions. �� Property/ �� Value=  7System.Windows.Controls.Primitives.DataGridColumnHeader0 J� IsHitTestVisible  ThumbStyleWE  ThumbStyleNS)  #System.Windows.Controls.DataGridRow1   WindowBrushKey?  9System.Windows.Controls.Primitives.SelectiveScrollingGrid?	  9System.Windows.Controls.Primitives.DataGridCellsPresenter2   
ItemsPanelA
  ;System.Windows.Controls.Primitives.DataGridDetailsPresenter3   DetailsVisibility$4   SelectiveScrollingOrientation#5   RowDetailsScrollingConverter;  5System.Windows.Controls.SelectiveScrollingOrientation6   Vertical:  4System.Windows.Controls.Primitives.DataGridRowHeader
7   Row8 �� Color  ì*  $System.Windows.Controls.DataGridCell9 ^� 
Conditions: �� Binding;   HighlightTextBrushKey<   HighlightBrushKey  FocusBorderBrushKey % !InternalSystemColorsExtendedStart&=   InactiveSelectionHighlightBrush>   GrayTextBrushKey  è? �� SelectionBoxItem@ �� SelectionBoxItemTemplate#A �� SelectionBoxItemStringFormatB �� ContentStringFormat3   -Microsoft.Windows.Themes.DataGridHeaderBorderC   SortDirectionD   SortDirectionE J� IsMouseOverF   	IsHoveredG   	IsPressedH   CanUserSortI   IsClickableJ   SeparatorVisibilityK   SeparatorVisibilityL   SeparatorBrushM   SeparatorBrushN � ToolTipO ��NameP �� BindingQ �� ValueR   IsRowSelectedS   
IsSelectedT   OrientationU   SeparatorVisibilityV   SeparatorBrushW �� Orientation   5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     5      TthemeJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero  5      <utl4clr-namespace:Qhta.WPF.Utils;assembly=Qhta.WPF.Utils  5      Fcvt>clr-namespace:Qhta.WPF.Converters;assembly=Qhta.WPF.Converters  5      %\.  &	        &	      (�� &     5
   
   "  $" DataGridSelectAllButtonStyle��)'  @    7   '  j    &	 P    &	 |    (�� �    5�   
   *31DataGridColumnHeader.ColumnFloatingHeaderStyleKey+)(�� X    5�   
   *86DataGridColumnHeader.ColumnHeaderDropSeparatorStyleKey+)'  N    '	       (�� �    5A     *DataGrid.FocusBorderBrushKey+)&	 �    '  �    7  7  (�� �    5�  
   *20DataGridComboBoxColumn.TextBlockComboBoxStyleKey+)'  !    7   7  7  '  c)    7   7  7  �� 5      6!     5      6%   �� 5
      "  ��5   
   .�5      ��  � �� 5      �� 5   
   " ��6   .R�5      � .+�5      �� $��Border��6   #���  5      $
 True��5      � 5      $��Arrow��6   $��Right=�5      $��Bottom=�5      $��8,8,3,3q�5      $
 0.15Q�5      ���	#FF000000$	 Uniform=�5      
 �   ��� ��� ��� ��� 5      5       5      P� 5       �@J�IsMouseOver$
 True��5      . �5      �� $ Border��6   # �  5        � ��5!      5"      P� 5#       � ��$
 True��5$      . �5%      �� $ Border��6   # �  5&       � ��5'      5(      P� 5)       � {�$ False��5*      . �5+      �� $ Arrow��6    � y�$ 	Collapsed=�5-      5.      50      52      53      �� 54      "   55   
   .�56      ��  � ��# �  57      �� 58       � ��# �  59      �� 5:       � �� �	#FF688CAF5;      �� 5<       � ��$ 1q�5=      �� 5>        �@ RowDetailsVisibilityMode$ VisibleWhenSelected=�5?      �� 5@       � ��$
 True��5A      �� 5B       �@��PanningMode$
 Both=�5C      �� 5D       �@��IsFlicksEnabled$ False��5E      �� 5F       � �� 5G      �� 5H   
   "  6   .R�5I      �� #��z��6   #��z��5J      #��z��5K      $
 True��5L      # z��5M      ._�5N      �  5O      �� 5P      $
��Auto �6    �� 5Q      $��* �6    .+�5S      � $��8��6   $ 
Horizontal=�5T      y�5U      �� $2 ,(utl:CollectionViewBehavior.ShowProgressBar)� �� $ FindAncestor=� DataGrid8    5X      �� $5 /(utl:CollectionViewBehavior.ProgressBarMaximum)� �� $ FindAncestor=� DataGrid 5Z      �� $3 -(utl:CollectionViewBehavior.ProgressBarValue)� �� $ FindAncestor=� DataGrid�� 5]      $ DG_ScrollViewer��6   $��1��5^      $|�False��5_      ��5`      �� 5a      " ��6$   .R�5b      �  5c      �� 5d      $
��Auto �6(   �� 5e      $��* �6(   �� 5f      $
��Auto �6(    5h      �� 5i      $
��Auto �6+   �� 5j      $��* �6+   �� 5k      $
��Auto �6+   .+�5m      �� #��Z 6   $|�False��5n      ��5o      ��  �� "  $  CellsPanelHorizontalOffset���5p      C� *�� "  $" DataGridSelectAllButtonStyle��+y�5q      ��  �� "  $ HeadersVisibility�# Z #  Z   5r      $!��PART_ColumnHeadersPresenter��67   $��1��5s   7   y�5t   7   ��  �� "  $ HeadersVisibility�# Z #  Z! �� 5u      $!��PART_ScrollContentPresenter��6/   $��1��5v   /   $��2��5w   /   #" z��5x   /   �� 5y      $��PART_VerticalScrollBar��6"   $��1��5z   "   $��2��5{   "   $# Vertical=�5|   "   # z$ 5}   "   #& z% 5~   "   #y�z' 5   "    5�   "   �� $ VerticalOffset� �� *TemplatedParent+$( OneWay=�� 5�      $��2��6   $��1��5�       5�      �� 5�      ��6-   ��  �� "  $. (NonFrozenColumnsViewportHorizontalOffset��� 5�      $��* �6-   .+�5�      �� $��PART_HorizontalScrollBar��6$   $��1��5�   $   $# 
Horizontal=�5�   $   # z) 5�   $   #& z* 5�   $   #y�z+ 5�   $    5�   $   �� $ HorizontalOffset� �� *TemplatedParent+$( OneWay=�5�      5�      5�      .��5�      �� # z 6!   5�      5�      5�      5�      5�      , 5�      ]� 5�      - 5�   
   �� 5�      . �@��
IsGrouping$
/ True��5�      �� 5�      ". �@%�IsVirtualizingWhenGrouping$/ False��5�      ./�5�   
   ��  � ��$ False��5�      5�   	   5�      �� 5�      "   5�   
   .�5�      ��  � �� 5�      �� 5�   
   "  6   .R�5�      � .+�5�        $��PART_FillerColumnHeader��6#   $0 False��5�   #   �� 5�      5�      5�      5�      5�      �� 5�      "  n�5�   
   .�5�      ��  � ��$ 8��5�      �� 5�       � �� �	#00FFFFFF5�      �� 5�       �@�Cursor$ SizeWE��5�      �� 5�       � �� 5�      �� 5�   
   " n�6   .R�5�      �� # z��6   #��z��5�      5�      5�      5�      �� 5�      "  n�5�   
   .�5�      ��  � ��$ 8��5�      �� 5�       � �� �	#00FFFFFF5�      �� 5�       �@�Cursor$ SizeNS��5�      �� 5�       � �� 5�      �� 5�   
   " n�6   .R�5�      �� # z��6   #��z��5�      5�      5�      5�      �� 5�      "  ��5�   
   .�5�      ��  �@J�Opacity$	 0.6Q�5�      �� 5�       � �� �	#FF688CAF5�      �� 5�       � ��$ 1q�5�      �� 5�       � �� 5�      �� 5�   
   " ��6   .R�5�      �� #��z��6   #��z��5�      # z��5�      #��z��5�      ._�5�      � .+�5�      �� $��PART_VisualBrushCanvas��6   #��z��5�      5�      5�      5�      5�      5�      �� 5�      "  ��5�   
   .�5�      ��  � �� �	#FF0000805�      �� 5�       � �� 5�      �� 5�   
   " ��6   .R�5�      �� #��z��6   #��z��5�      #��z��5�      $
 True��5�      5�      5�      5�      �� 5�      "   5�   
   .�5�      ��  � ��# � 1 5      �� 5      �@J�SnapsToDevicePixels$
 True��5     �� 5      �@=�ErrorTemplate 5     Z� �� 5      �@ ValidationErrorTemplate 5     �� 5  
   .R�5     �� $��2,0,0,0q�6   $��Center=�5	     ���	#FFFF0000$��!��5     5     5     �� 5      � �� 5     �� 5  
   "  6   .R�5     �� $��
DGR_Border��6   #��z��5     #��z��5     #��z��5     $
 True��5     ._�5        5     �� 5     $
��Auto �6#   �� 5     $��* �6#    5     �� 5     $��* �6    �� 5     $
��Auto �6    .+�5      	  $��1��6'   #��z2 5!  '   # z 5"  '   
  5#     $��1��6)   $��1��5$  )   #y�z3 5%  )   4 5&  )   ��  �� "  $ AreRowDetailsFrozen�# Z5 #  Z6   5'     $4 Vertical=�6"   $��2��5(  "   y�5)  "   ��  �� "  $ HeadersVisibility�# Z #  Z7 5*     5+     5,     5.     , 5/     P� 50      �@ 	IsNewItem$
 True��51     . �52  
   ��  � �� 53     ��  �� "  $ NewItemMargin�54  	   56     �� 57     "  	 58  
   .�59     ��  � �� 5:     �� 5;  
   " 	 6   .R�5<     �� 5=     5?     5@     �� 5A     $8 	#FF000000��5B     u�5C     1.5q�6   �� 5D     "   5E  
   .�5F     ��  � �� �	#00FFFFFF5G     �� 5H      � �� �	#00FFFFFF5I     �� 5J      � ��$ 1q�5K     �� 5L      � �� 5M     �� 5N  
   "  6   .R�5O     �� $��Bd��6   #��z��5P     #��z��5Q     #��z��5R     $
 True��5S     ._�5T     �� # z 6   5U      5V     ^� 5W     9 5X     �� 5Z     : 6   �� $% (SystemParameters.HighContrast)�/ true5[     �� 5\     : 6   �� $ IsKeyboardFocused� �� *Self+/ true5]     .0�5_     �� $ Bd��6   # � ; 5`      � ��5a     �� 5b     $ Bd��6   8    � ��5d     5e     5g     5i     , 5j     P� 5k      �@ 
IsSelected$
 True��5l     . �5m  
   ��  � ��# � < 5n     �� 5o  
    � ��# � ; 5p     �� 5q  
    � ��# � < 5r     5s  	   P� 5t      �@J�IsKeyboardFocusWithin$
 True��5u     . �5v  
   ��  � ��# �  5w     5x  	   ]� 5�     - 5�  
   �� 5�     . �@ 
IsSelected$
/ True��5�     �� 5�     . �@��IsSelectionActive$/ False��5�     ./�5�  
   ��  � ��# �  5�     �� 5�  
    � ��# �  5�     �� 5�  
    � ��# � = 5�     5�  	   P� 5�      � {�$ False��5�     . �5�  
   ��  � ��# � > 5�     5�  	   ^� 5�     9 5�  
   �� 5�     : 6   �� $% (SystemParameters.HighContrast)�/ true5�     .0�5�  
   ��  �@�FocusVisualStyle8  5�     5�  	   5�     �� 5�     "  ��5�  
   .�5�     ��  � �� 5�     �� 5�  
   " ��6   .R�5�     �� $��1,1,1,1q�6   #��z? 5�     #��z@ 5�     #��z��5�     #B zA 5�     #��z��5�     #��z��5�     # z 5�     $0 False��5�     5�     5�     5�     �� 5�     "   5�  
   .�5�     ��  � ��$ Center=�5�     �� 5�      � �� 5�     �� 5�  
   "  6   .R�5�     � .+�5�       #D zC 6)   #F zE 5�  )   #G z��5�  )   #I zH 5�  )   #��z��5�  )   #��z��5�  )   #��z��5�  )   # z��5�  )   #K zJ 5�  )   #M zL 5�  )   ._�5�     �  5�     �� 5�     $��* �6%   �� 5�     $
��Auto �6%   �� 5�     $
��Auto �6%   .+�5�     �� # z 6#   #��z��5�  #   #��z��5�  #   $
��True��5�  #   N 5�  #   �� $7 1Column.(utl:CollectionViewBehavior.HeaderTooltip)� �� $ FindAncestor=� DataGridColumnHeader5�  #   �� 5�     $O FilterButton��6   $��2��5�     $��Right=�5�     $��Center=�5�     
������ 
������ y�5�     �� $: 4Column.(utl:CollectionViewBehavior.ShowFilterButton)� �� $ FindAncestor=� DataGridColumnHeader8   ��5�     �� 5�     "  ��6   .�5�     -   5        5�     .��5�     �� ��5�     �� 5�     "  ��6   , 5�     �� 5�     P 6(   �� $; 5Column.(utl:CollectionViewBehavior.FilterButtonShape)� �� $ FindAncestor=� DataGridColumnHeader	Q Empty5�  (   .a�5�     ��  � ��$O Ipack://application:,,,/Qhta.WPF.Utils;component/Resources/EmptyFilter.png��5�  %   5�     �� 5�     P 6(   �� $; 5Column.(utl:CollectionViewBehavior.FilterButtonShape)� �� $ FindAncestor=� DataGridColumnHeader
Q Filled5�  (   .a�5�     ��  � ��$J Dpack://application:,,,/Qhta.WPF.Utils;component/Resources/Filter.png��5�  %   5�     �� 5�     P 6(   �� $; 5Column.(utl:CollectionViewBehavior.FilterButtonShape)� �� $ FindAncestor=� DataGridColumnHeaderQ 5�  (   Z� .a�5      ��  � ��$O Ipack://application:,,,/Qhta.WPF.Utils;component/Resources/EmptyFilter.png��5  %   5     5     5     5     5	     5
     n� 5     $��PART_LeftHeaderGripper��6   $
��Left=�5     8�� 5     n� 5     $��PART_RightHeaderGripper��6   $��Right=�5     8�� 5     5     5     5     5     �� 5     "   5  
   .�5     ��  � �� 5     �� 5  
   "  6   .R�5     � .+�5       #S zR 6)   #F zE 5  )   #G z��5  )   #��z��5  )   #��z��5   )   #��z��5!  )   # z��5"  )   $T 
Horizontal=�5#  )   #K zU 5$  )   #M zV 5%  )   ._�5&     �� $W 
Horizontal=�6   .+�5'     �� # z 6#   $��Center=�5(  #   $
��True��5)  #   �� 5*     $ False��6   y�5+     ��  �� "  $ (Validation.HasError)�8   ��5,     ��  �� "  $ ValidationErrorTemplate�5-     5.     n� 5/     $��PART_TopHeaderGripper��6   $	��Top=�50     8�� 51     n� 52     $��PART_BottomHeaderGripper��6   $��Bottom=�53     8�� 54     55     56     58     59     5:     