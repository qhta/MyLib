   M S B A M L   `   `   `  ����X  TQhta.WPF.Controls, Version=1.0.2.0, Culture=neutral, PublicKeyToken=2195313f51a3d9f45clr-namespace:Qhta.WPF.ControlsQhta.WPF.Controls  5      $    Qhta.WPF.Controls.ColorPadEdit   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      1localclr-namespace:Qhta.WPF.Controls     5      -   5          >�Name$  ThisControl��5      .��5
      �  � ColumnDefinitions 5      �� 5   
   $
��Auto �6   �� 5   
   $��* �6   .+�5      �� $��0��6   
���   �$��1q�5      $
��Left=�5      $	��Top=�6&   $
��Auto��5      $
��Auto��6   ._�5   
       Qhta.WPF.Controls.ColorPad  -   5          Name$ ColorPad��5         
Resolution$	 256��5         	HueChange$
 None=�6%   $	��128��5      $	��128��6    5   	   �� 5      $��1��6   $��10,0,0,0q�6#   .+�5   
   '   !Qhta.WPF.Controls.ColorMemberEdit  -   5          Name$ HSlider��5   !      Member$	 Hue=�5   #      	LabelText$	 Hue��5   #      
LabelWidth$ 70Q�5   #   	   SliderVisibility$	 	Collapsed=�5   #   
   	BaseColor
 5   #   ��  �� ElementName$ ThisControl�� �� Path$ SelectedColor� �� Mode$ OneWay=�   SelectedColor 5   #   �� $ ThisControl��$ SelectedColor�$ OneWay=�  5    
   -   5        $ SSlider��5    !   $ 
Saturation=�5!   #   $ 
Saturation��5"   #   $ 70Q�5#   #   $	 	Collapsed=�5$   #   
 5%   #   �� $ ThisControl��$ SelectedColor�$ OneWay=� 5&   #   �� $ ThisControl��$ SelectedColor�$ OneWay=�  5(   
   -   5        $ VSlider��5(   !   $ 
Brightness=�5)   #   $ 
Brightness��5*   #   $ 70Q�5+   #   $	 	Collapsed=�5,   #   
 5-   #   �� $ ThisControl��$ SelectedColor�$ OneWay=� 5.   #   �� $ ThisControl��$ SelectedColor�$ OneWay=�50   	   51      52      