   M S B A M L   `   `   `  ����I  EQhta.WPF.Utils, Version=3.5.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:Qhta.WPF.UtilsQhta.WPF.Utils  5   <   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089 #  TreeViewItemExpanderButtonStyle  �� 
TargetType �� Property �� Value �� 
TargetType �� Padding F�Name �� Angle �� CenterX �� CenterY	 �� Triggers
 P� Property P� Value �� 
TargetName ' #TreeViewItem.IsMouseOver.Background �� 
StartPoint �� EndPoint � Color � Offset # TreeViewItem.IsMouseOver.Border �� Color & "TreeViewItem.IsSelected.Background " TreeViewItem.IsSelected.Border % !TreeViewItem.IsFocused.Background ! TreeViewItem.IsFocused.Border  MyTreeViewItem � ColumnDefinitions � RowDefinitions d�Name �� 	ClickMode d� 	IsChecked �� RelativeSource ��Name J� SnapsToDevicePixels ��Name J� Opacity ��Name �� HeaderStringFormat �� ContentStringFormat ��Name  �� Value! �� Binding" �� ElementName# �� Path%    Qhta.WPF.Utils.TreeViewBehavior!  System.Windows.SystemColors$   GrayTextBrushKey�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     5      &utilsclr-namespace:Qhta.WPF.Utils  6<   %�  &	        &	     &	 �    &	 �    &	 ^    &	 �    &	     &	 9    7   7  7  7  7  7  7  �� 5      "  d�62   .�5      ��  � |�$ false��6,   �� 5       � ��$ 16��6/   �� 5       � ��$ 16��60   �� 5       � �� 5      �� 5	   
   " d�6   .R�5
      �� $��16��6   $��16��6   ���	#00FFFFFF$ 5,5,5,5q�6A   ._�5      F� $ 
ExpandPath��6   ���	#00FFFFFF���	#FF989898��5      B� 5      $��M0,0L0,6L6,0zC�6   z�5      �� 5      $	 135Q�6"   $ 3Q�6.   $ 3Q�6:   5      5      	 5      P� 5      
 �@J�IsMouseOver$
 True��6-   . �5      �� $ 
ExpandPath��6    � �� �	#FF1BBBFA6G   �� 5      $ 
ExpandPath��6    � �� �	#00FFFFFF6?   5      P� 5      
 �@d�	IsChecked$
 TrueY�68   . �5      �� $ 
ExpandPath��6    � z� 5      �� 5      $	 180Q�6$   $ 3Q�60   $ 3Q�6<   5      �� 5      $ 
ExpandPath��6    � �� �	#FF5959596?   �� 5       $ 
ExpandPath��6    � �� �	#FF2626266G   5!      5#      5%      5&      �� 5(      $	 0,0&�6D   $	 0,1&�6U   ��5*      � $ 	#FFF1FBFF��6   $ 0Q�6'   � 5+      $ 	#FFD5F1FE��6   $ 1Q�6'   5-      �� 5.      $
 Cyan��6<   �� 5/      $	 0,0&�6C   $	 0,1&�6T   ��51      � $ 	LightCyan��6   $ 0Q�6'   � 52      $ DeepSkyBlue��6   $ 1Q�6)   54      �� 55      $ DarkCyan��6;   �� 56      $	 0,0&�6B   $	 0,1&�6S   ��58      � $ 
DodgerBlue��6   $ 0Q�6(   � 59      $ 
MediumBlue��6   $ 1Q�6(   5;      �� 5<      $
 Navy��6:   �� 5>      "  Q�6!   .�5?      ��  � ��$
 Left=�6,   �� 5@       � �� 5A      �� 5B   
   " Q�6   .R�5C      �� .+�5D      � $��Stretch=�6    5E      �� 5F      $
��Auto �6#   $��19��60   �� 5G      $��* �6#    5I      �� 5J      $
��Auto �6    �� 5K      .+�5N      d� $ Expander��6   8��  $ Press=�6h    5O      �� *
IsExpanded+ �� *TemplatedParent+�� 5P      $ Bd��6   $��1��6#   $��Stretch=�63   #��z��5Q      #��z��6C   #��z��5R      # z��6N   $
 True��5S      ._�5T      � .+�5U      �� $ FocusBackground��6   $y�	Collapsed=�67   8�� 8�� $��1��5W   O   �� 5X      $ SelectionBackground��6   $y�	Collapsed=�6;   $	 0.5Q�6S   8�� 8�� $��1��5Z   P   �� 5[      $ HoverBackground��6   $y�	Collapsed=�67   $	 0.5Q�6N   8�� 8�� $��1��5]   Q   �� 5^      $ PART_Header��6%   #��z��5_      #��z��5`      $��Header��6Q   # z 5a      #��z��5b      #��z��5c      # z 5d      5e      5f      �� 5h      $ 	ItemsHost��6   $��1��62   $��1��6?   5i      5j      	 5k      P� 5l      
 �@Q�
IsExpanded$ False��69   . �5m      �� $ 	ItemsHost��6    � y�$ 	Collapsed=�6D   5n      P� 5o      
 �@��HasItems$ False��67   . �5p      �� $ Expander��6    � y�$ Hidden=�6C   5q      �� 5r      $
  True��6O   ! 6   �� $" Bd��$# IsMouseOver�.a�5s      �� $ HoverBackground��6    � y�$ Visible=�6J   5t      P� 5u      
 �@��
IsSelected$
 True��64   . �5v      �� $ SelectionBackground��6    � y�$ Visible=�6N   5w      P� 5x      
 �@  IsItemSelected$
 True��6G   . �5y      �� $ SelectionBackground��6    � y�$ Visible=�6N   5z      P� 5{      
 �@J�	IsFocused$
 True��6+   . �5|      �� $ FocusBackground��6    � y�$ Visible=�6J   �� 5}       � ��
 �����6-   5~      P� 5      
 � {�$ False��6+   . �5�      �� # � $ 6-    � ��5�      5�      5�      5�      5�      