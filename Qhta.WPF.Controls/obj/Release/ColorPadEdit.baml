   M S B A M L   `   `   `  ���� L  HQhta.WPF.Controls, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null5clr-namespace:Qhta.WPF.ControlsQhta.WPF.Controls  $    Qhta.WPF.Controls.ColorPadEdit   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     1localclr-namespace:Qhta.WPF.Controls     -     >�Name$  ThisControl��.���  � ColumnDefinitions �� $
��Auto ��� $��* �.+��� $��0��
���   �$��1q�$
��Left=�$	��Top=�$
��Auto��$
��Auto��._�    Qhta.WPF.Controls.ColorPad  -     Name$ ColorPad��   
Resolution$	 256��   	HueChange$
 None=�$	��128��$	��128���� $��1��$��10,0,0,0q�.+�'   !Qhta.WPF.Controls.ColorMemberEdit  -     Name$ HSlider��   Member$	 Hue=�   	LabelText$	 Hue��   
LabelWidth$ 70Q�	   SliderVisibility$	 	Collapsed=�
   	BaseColor
 ��  �� ElementName$ ThisControl�� �� Path$ SelectedColor� �� Mode$ OneWay=�   SelectedColor �� $ ThisControl��$ SelectedColor�$ OneWay=�  -   $ SSlider��$ 
Saturation=�$ 
Saturation��$ 70Q�$	 	Collapsed=�
 �� $ ThisControl��$ SelectedColor�$ OneWay=� �� $ ThisControl��$ SelectedColor�$ OneWay=�  -   $ VSlider��$ 
Brightness=�$ 
Brightness��$ 70Q�$	 	Collapsed=�
 �� $ ThisControl��$ SelectedColor�$ OneWay=� �� $ ThisControl��$ SelectedColor�$ OneWay=�