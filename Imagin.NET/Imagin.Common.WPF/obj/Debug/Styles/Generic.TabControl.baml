   M S B A M L   `   `   `  ����R  NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   Style.TabItem  �� 
TargetType �� Property  Brush.TabItem.Background �� Value  Brush.TabItem.Border  Brush.TabItem.Foreground �� 
TargetType ��Name �� Padding J� SnapsToDevicePixels ��Name �� RelativeSource	 �� Triggers
 P� Property P� Value % !Brush.TabItem.Selected.Background ! Brush.TabItem.Selected.Border % !Brush.TabItem.Selected.Foreground ]� 
Conditions �� Property �� Value & "Brush.TabItem.MouseOver.Background " Brush.TabItem.MouseOver.Border &	 "Brush.TabItem.MouseOver.Foreground 
 Style.TabControl.Horizontal  Brush.TabControl.Background  Brush.TabControl.Border-   'System.Windows.Input.KeyboardNavigation    TabNavigation � RowDefinitions ��Name ��Name K� IsItemsHost    TabIndex K� ZIndex    DirectionalNavigation �� SelectedContent �� SelectedContentTemplate" �� SelectedContentStringFormat �� ContentStringFormat �� Triggers �� 
TargetName  Style.TabControl.Vertical �� BasedOn � ColumnDefinitions ��Name�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml      5      Bmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      %�  &	        &	
 �    7   &	 �	    7   '�� �    7 
 �� 5      "  ��6"   .�5   
   ��  � ��# �  6'   �� 5   
    � ��# �  6(   �� 5   
    � ��$ 1q�6,   �� 5	   
    � ��# �  6'   �� 5
   
    � ��$
 10,4q�6$   �� 5   
    � ��$ Stretch=�65   �� 5   
    � ��$ Stretch=�67   �� 5   
    � �� 5      �� 5      " ��6"   .R�5      �� $ PART_Border��5      #��z��5      #��z��5      #��z��5      $��0,0,-1,0q�5      # z��5       . ._�5      �� $ PART_Content��5      $��Header��5      #��z��5      #��z��5      ��5      �� *
Foreground+ �� *TemplatedParent+5      5      5!      	 5"   
   P� 5#      
 �@��
IsSelected . . �5$      ��  � ��# �  6/   �� 5%       � ��# �  60   �� 5&       � ��$ 1,1,1,0q�64   �� 5'       �@�Cursor$ Arrow��6+   �� 5(       � ��# �  6/   5)      P� 5*      
 �@��
IsSelected .  . �5+      ��  �@�Cursor$
 Hand��6+   5,      ]� 5-       5.      �� 5/       �@��
IsSelected .  66   �� 50       �@J�IsMouseOver . 67   ./�52      ��  � ��# �  6/   �� 53       � ��# �  60   �� 54       � ��# � 	 6/   55      57      �� 59      "  ��60   .�5:   
   ��  � ��# �  6'   �� 5;   
    � ��# �  6(   �� 5<   
    � ��$ 1q�6,   �� 5=   
    �@��TabStripPlacement$	 Top=�6.   �� 5>   
    � ��8   6/   �� 5?   
    � �� 5@      �� 5A      " ��6"   .R�5B      � }�. $ Local=�5D       .  5F      �� 5G      $ RowDefinition0��6:   $
��Auto �6,   6:   �� 5H      $ RowDefinition1��67   $��* �6,   67   .+�5J      �� $ 
PART_Panel��5K       . $ 1��5M      $ 1��5N      �� 5O      $ PART_Content��5Q      $��1��5P      #��z��5R      #��z��5S      #��z��5T      $ 2��5U      $ Local=�5V      $ 	Contained=�5W      $��0,-1,0,0q�5X      # z��5Y      ._�5Z      �� $ PART_SelectedContentHost��5[   !   #��z 5\   !   #��z 5]   !   # z 5^   !   $��SelectedContent��5_   !   # z 5`   !   5a      5b       5c      P� 5d      
 �@��TabStripPlacement$ Bottom=�6?   . �5e      �� $ 
PART_Panel��6%    � ��$ 1��6Q   �� 5f      $ PART_Content��6%    � ��$ 0��6S   �� 5g      $ RowDefinition0��6%    � ��$ * �6S   �� 5h      $ RowDefinition1��6%    � ��$
 Auto �6S   5i      5k      5m      5n      �� 5p      "  ��6.   .�5q   
   ��  � ��# �  6'   �� 5r   
    � ��# �  6(   �� 5s   
    � ��$ 1q�6,   �� 5t   
    �@��TabStripPlacement$
 Left=�6.   �� 5u   
    � �� 5v      �� 5w      "  ��6   8   .�5x      ��  � ��$
 10,2q�60   5y      5{      �� 5|   
    � �� 5}      �� 5~      " ��6"   .R�5      � }�.  . $ Local=�6J    5�      �� 5�      $ ColumnDefinition0��6/   �� 5�      $ ColumnDefinition1��69   $��0 �6/   69    5�      �� 5�      $ RowDefinition0��6:   $
��Auto �6,   6:   �� 5�      $ RowDefinition1��67   $��* �6,   67   .+�5�      �� $ 
PART_Panel��5�       . $ 1��5�      $ 1��5�      �� 5�      $ PART_Content��5�      $��1��5�      #��z��5�      #��z��5�      #��z��5�      $ 2��5�      $ Local=�5�      $ 	Contained=�5�      # z��5�      ._�5�      �� $ PART_SelectedContentHost��5�   !   #��z 5�   !   #��z 5�   !   # z 5�   !   $��SelectedContent��5�   !   # z 5�   !   5�      5�       5�      P� 5�      
 �@��TabStripPlacement$ Bottom=�6?   . �5�      �� $ 
PART_Panel��6%    � ��$ 1��6Q   �� 5�      $ PART_Content��6%    � ��$ 0��6S   �� 5�      $ RowDefinition0��6%    � ��$ * �6S   �� 5�      $ RowDefinition1��6%    � ��$
 Auto �6S   5�      P� 5�      
 �@��TabStripPlacement$
 Left=�6?   . �5�      �� $ 
PART_Panel��6%    � ��$ 0��6Q   �� 5�      $ 
PART_Panel��6%    � ��$ 0��6T   �� 5�      $ PART_Content��6%    � ��$ 0��6S   �� 5�      $ PART_Content��6%    � ��$ 1��6V   �� 5�      $ ColumnDefinition0��6%    � ��$
 Auto �6U   �� 5�      $ ColumnDefinition1��6%    � ��$ * �6U   �� 5�      $ RowDefinition0��6%    � ��$ * �6S   �� 5�      $ RowDefinition1��6%    � ��$ 0 �6S   5�      P� 5�      
 �@��TabStripPlacement$ Right=�6?   . �5�      �� $ 
PART_Panel��6%    � ��$ 0��6Q   �� 5�      $ 
PART_Panel��6%    � ��$ 1��6T   �� 5�      $ PART_Content��6%    � ��$ 0��6S   �� 5�      $ PART_Content��6%    � ��$ 0��6V   �� 5�      $ ColumnDefinition0��6%    � ��$ * �6U   �� 5�      $ ColumnDefinition1��6%    � ��$
 Auto �6U   �� 5�      $ RowDefinition0��6%    � ��$ * �6S   �� 5�      $ RowDefinition1��6%    � ��$ 0 �6S   5�      5�      5�      5�      �� 5�      "  ��6(   8   .�5�   
   ��  � ��$	 125��6&   �� 5�   
    � ��$ 10q�6$   5�      5�      