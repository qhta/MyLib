   M S B A M L   `   `   `  ���� L  HQhta.WPF.Controls, Version=1.0.1.0, Culture=neutral, PublicKeyToken=nullP:clr-namespace:Qhta.WPF.Controls;assembly=Qhta.WPF.ControlsQhta.WPF.Controls  B >TestApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:TestAppTestApp #   TestApp.ColorPickerTestWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     Imy:clr-namespace:Qhta.WPF.Controls;assembly=Qhta.WPF.Controls     'localclr-namespace:TestApp     -     �Name$  
ThisWindow�� � Title$ ColorPickerTestWindow��$	��300��$	��400��.��� .+���  �� Orientation$ 
Horizontal=�$��	20,50,0,0q�$��20��$
��Left=�$	��Top=�.+��  � ColumnDefinitions �� $��40 ��� $��40 ��� $��40 ��� $��40 ��� $��90 �.+��� -    ��Name$ AlphaTextBox��$��0��$��Stretch=�$��Stretch=�$��0,0,10,0q�����  �� ElementName$ 
ThisWindow�� �� Path$ EditedColor.A��� -   $ 
RedTextBox��$��1��$��Stretch=�$��Stretch=�$��0,0,10,0q����� $ 
ThisWindow��$ EditedColor.R��� -   $ GreenTextBox��$��2��$��Stretch=�$��Stretch=�$��0,0,10,0q����� $ 
ThisWindow��$ EditedColor.G��� -   $ BlueTextBox��$��3��$��Stretch=�$��Stretch=�$��0,0,10,0q����� $ 
ThisWindow��$ EditedColor.B�+   %Qhta.WPF.Controls.ColorPickerDropDown  $��4��$��Stretch=�$��Stretch=�   ProbeVisibility$ Visible=�   SelectedColor �� $ 
ThisWindow��$ EditedColor��� -   	 ��Name$	 OK��$��OK��$��20,0,0,0q�$
��Left=�$��75��� $��
80,100,0,0q�$
��Left=�$	��120��$	��Top=�$	��120��.+��� $��Probe��$
��Auto��$
��Auto��$��Center=�$��Center=��� -   
 ��Name$
 ColorRectangle��
���   �$��1������ $ 
ThisWindow��$ EditedBrush�