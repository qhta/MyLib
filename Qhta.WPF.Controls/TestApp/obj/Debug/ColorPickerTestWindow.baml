   M S B A M L   `   `   `  ����X  TQhta.WPF.Controls, Version=1.0.2.0, Culture=neutral, PublicKeyToken=2195313f51a3d9f4P:clr-namespace:Qhta.WPF.Controls;assembly=Qhta.WPF.ControlsQhta.WPF.Controls  5   	   B >TestApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:TestAppTestApp 5   	   #   TestApp.ColorPickerTestWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Imy:clr-namespace:Qhta.WPF.Controls;assembly=Qhta.WPF.Controls     5   	   'localclr-namespace:TestApp     5   	   -   5          �Name$  
ThisWindow��5	   	    � Title$ ColorPickerTestWindow��5
   	   $	��300��6'   $	��400��64   .��5      � .+�5      ��  �� Orientation$ 
Horizontal=�5   	   $��	20,50,0,0q�5   	   $��20��5   	   $
��Left=�5   	   $	��Top=�5   	   .+�5      �  � ColumnDefinitions 5   
   �� 5      $��40 �6   �� 5      $��40 �6   �� 5      $��40 �6   �� 5      $��40 �6   �� 5      $��90 �6   .+�5   
   �� -   5         ��Name$ AlphaTextBox��5      $��0��5      $��Stretch=�5      $��Stretch=�5      $��0,0,10,0q�5      ��5       ��  �� ElementName$ 
ThisWindow�� �� Path$ EditedColor.A��� 5"   
   -   5        $ 
RedTextBox��5"      $��1��5#      $��Stretch=�5$      $��Stretch=�5%      $��0,0,10,0q�5&      ��5'      �� $ 
ThisWindow��$ EditedColor.R��� 5)   
   -   5        $ GreenTextBox��5)      $��2��5*      $��Stretch=�5+      $��Stretch=�5,      $��0,0,10,0q�5-      ��5.      �� $ 
ThisWindow��$ EditedColor.G��� 50   
   -   5        $ BlueTextBox��50      $��3��51      $��Stretch=�52      $��Stretch=�53      $��0,0,10,0q�54      ��55      �� $ 
ThisWindow��$ EditedColor.B�+   %Qhta.WPF.Controls.ColorPickerDropDown  57   
   $��4��58      $��Stretch=�59      $��Stretch=�5:         ProbeVisibility$ Visible=�5<         SelectedColor 5;      �� $ 
ThisWindow��$ EditedColor�5<      5>   	   �� 5?      -   5        	 ��Name$	 OK��5?      $��OK��5@      $��20,0,0,0q�5A      $
��Left=�5B      $��75��5C      5D      5F      � 5G      $��
80,100,0,0q�5H   	   $
��Left=�5I   	   $	��120��6$   $	��Top=�5J   	   $	��120��6!   .+�5L      �� $��Probe��6   $
��Auto��6   $
��Auto��6,   $��Center=�5M      $��Center=�6/   �� 5N      -   5        
 ��Name$
 ColorRectangle��5N      
���   �$��1��5P      ��5Q      �� $ 
ThisWindow��$ EditedBrush�5S      5T      5U      