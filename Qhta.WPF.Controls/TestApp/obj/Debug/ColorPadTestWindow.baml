   M S B A M L   `   `   `  ����X  TQhta.WPF.Controls, Version=1.0.2.0, Culture=neutral, PublicKeyToken=2195313f51a3d9f4P:clr-namespace:Qhta.WPF.Controls;assembly=Qhta.WPF.ControlsQhta.WPF.Controls  5   	   B >TestApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:TestAppTestApp 5   	       TestApp.ColorPadTestWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Imy:clr-namespace:Qhta.WPF.Controls;assembly=Qhta.WPF.Controls     5   	   'localclr-namespace:TestApp     5   	   -   5          �Name$  
ThisWindow��5	   	    � Title$ ColorSliderTestWindow��5
   	   $	��320��6'   $	��400��64   .��5      � .+�5      +   %Qhta.WPF.Controls.ColorPickerDropDown  -   5          Name$ TopLeftColorPicker��5      $��90��5      $��20��6   $��	20,20,0,0q�5      $
��Left=�5      $	��Top=�5         SelectedColor$
 Lime��5        5      -   5        $ TopRightColorPicker��5      $��90��5      $��20��6   $��
260,20,0,0q�5      $
��Left=�5      $	��Top=�5      $
 Cyan��5        5      -   5        $ BottomLeftColorPicker��5      $��90��5      $��20��6   $��
20,220,0,0q�5      $
��Left=�5      $	��Top=�5      $	 Red��5        5!      -   5        $ BottomRightColorPicker��5!      $��90��5"      $��20��6   $��260,220,0,0q�5#      $
��Left=�5$      $	��Top=�5%      $
 Blue��5&          Qhta.WPF.Controls.ColorPad  5(      -   5          Name$ ColorPad��5(      $��
125,70,0,0q�5)      $
��Left=�5*      $	��120��6"   $	��Top=�5+      $	��120��6      Color00 5,      ��  �� ElementName$ BottomLeftColorPicker�� �� Path$ SelectedColor�   Color01 5-      �� $ TopLeftColorPicker��$ SelectedColor�	   Color10	 5.      �� $ BottomRightColorPicker��$ SelectedColor�
   Color11
 5/      �� $ TopRightColorPicker��$ SelectedColor��� 51      -   5         ��Name$ ResultValue��51      $
��Left=�52      $��20��6"   $��140,255,0,0q�6.   $	��Top=�54      $��85��6   ��53      �� $ ColorPad��$ SelectedColor�54      55      56      