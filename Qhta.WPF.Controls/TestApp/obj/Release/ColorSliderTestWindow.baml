   M S B A M L   `   `   `  ���� L  HQhta.WPF.Controls, Version=1.0.1.0, Culture=neutral, PublicKeyToken=nullP:clr-namespace:Qhta.WPF.Controls;assembly=Qhta.WPF.ControlsQhta.WPF.Controls  B >TestApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:TestAppTestApp #   TestApp.ColorSliderTestWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     Imy:clr-namespace:Qhta.WPF.Controls;assembly=Qhta.WPF.Controls     'localclr-namespace:TestApp     -     �Name$  
ThisWindow�� � Title$ ColorSliderTestWindow��$	��320��$	��400��.��� .+��� -    ��Name$ HorizontalColorTextBox��$��	80,50,0,0q�$	��100��$��23��$
��Left=�$	��Top=�����  �� ElementName$ HorizontalSlider�� �� Path$ SelectedColor��� -    ��Name$ ColorRectangle��$��
80,100,0,0q�$
��Left=�$	��120��$	��Top=�$	��120��
���   �$��1������ $ 
ThisWindow��$ EditedBrush��� -   $ VerticalColorTextBox��$��
80,240,0,0q�$	��100��$��23��$
��Left=�$	��Top=����� $ VerticalSlider��$ SelectedColor��� -    ��Name$ OK��$��OK��$��
242,50,0,0q�$
��Left=�$	��Top=�$��75��#   Qhta.WPF.Controls.ColorSlider  -     Name$ VerticalSlider��   Orientation$ Vertical=�$��30��$	��256��$
��Left=�$��10q�	   Color0$		 Red��
   Color1$	
 Red��   	HueChange$ Positive=�   SelectedColor$ 	#FF00FF00��   
Resolution$	 360��� $��	50,10,0,0q�$	��256��$	��30 ��$	��Top=�����  j� TileMode$
 Tile=� j� Stretch$ Uniform=� j� 
AlignmentX$
 Left=� j� 
AlignmentY$	 Top=� j� Viewport$ 	0,0,30,30� j� ViewportUnits$ Absolute=� �� ImageSource ��  �� 	UriSource$? 9/Qhta.WPF.Controls;component/Resources/OpacityPattern.png?�.+�  -   $ HorizontalSlider��$ 
Horizontal=�   Position$ 1Q�