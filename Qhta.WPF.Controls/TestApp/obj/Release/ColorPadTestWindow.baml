   M S B A M L   `   `   `  ���� L  HQhta.WPF.Controls, Version=1.0.1.0, Culture=neutral, PublicKeyToken=nullP:clr-namespace:Qhta.WPF.Controls;assembly=Qhta.WPF.ControlsQhta.WPF.Controls  B >TestApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:TestAppTestApp     TestApp.ColorPadTestWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     Imy:clr-namespace:Qhta.WPF.Controls;assembly=Qhta.WPF.Controls     'localclr-namespace:TestApp     -     �Name$  
ThisWindow�� � Title$ ColorSliderTestWindow��$	��320��$	��400��.��� .+�+   %Qhta.WPF.Controls.ColorPickerDropDown  -     Name$ TopLeftColorPicker��$��90��$��20��$��	20,20,0,0q�$
��Left=�$	��Top=�   SelectedColor$
 Lime��  -   $ TopRightColorPicker��$��90��$��20��$��
260,20,0,0q�$
��Left=�$	��Top=�$
 Cyan��  -   $ BottomLeftColorPicker��$��90��$��20��$��
20,220,0,0q�$
��Left=�$	��Top=�$	 Red��  -   $ BottomRightColorPicker��$��90��$��20��$��260,220,0,0q�$
��Left=�$	��Top=�$
 Blue��    Qhta.WPF.Controls.ColorPad  -     Name$ ColorPad��$��
125,70,0,0q�$
��Left=�$	��120��$	��Top=�$	��120��   Color00 ��  �� ElementName$ BottomLeftColorPicker�� �� Path$ SelectedColor�   Color01 �� $ TopLeftColorPicker��$ SelectedColor�	   Color10	 �� $ BottomRightColorPicker��$ SelectedColor�
   Color11
 �� $ TopRightColorPicker��$ SelectedColor��� -    ��Name$ ResultValue��$
��Left=�$��20��$��140,255,0,0q�$	��Top=�$��85������ $ ColorPad��$ SelectedColor�