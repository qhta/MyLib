   M S B A M L   `   `   `  ����X  TQhta.WPF.Controls, Version=1.0.2.0, Culture=neutral, PublicKeyToken=2195313f51a3d9f4P:clr-namespace:Qhta.WPF.Controls;assembly=Qhta.WPF.ControlsQhta.WPF.Controls  5   	   B >TestApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:TestAppTestApp 5   	   #   TestApp.ColorSliderTestWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Imy:clr-namespace:Qhta.WPF.Controls;assembly=Qhta.WPF.Controls     5   	   'localclr-namespace:TestApp     5   	   -   5          �Name$  
ThisWindow��5	   	    � Title$ ColorSliderTestWindow��5
   	   $	��320��6'   $	��400��64   .��5      � .+�5      �� -   5         ��Name$ HorizontalColorTextBox��5      $��	80,50,0,0q�5      $	��100��5      $��23��5      $
��Left=�5      $	��Top=�5      ��5      ��  �� ElementName$ HorizontalSlider�� �� Path$ SelectedColor��� 5      -   5         ��Name$ ColorRectangle��5      $��
80,100,0,0q�5      $
��Left=�5      $	��120��6+   $	��Top=�5      $	��120��6(   
���   �$��1��5      ��5      �� $ 
ThisWindow��$ EditedBrush��� 5      -   5        $ VerticalColorTextBox��5      $��
80,240,0,0q�5      $	��100��5      $��23��5      $
��Left=�5       $	��Top=�5!      ��5"      �� $ VerticalSlider��$ SelectedColor��� 5$      -   5         ��Name$ OK��5$      $��OK��5%      $��
242,50,0,0q�5&      $
��Left=�5'      $	��Top=�5(      $��75��6%   5)      #   Qhta.WPF.Controls.ColorSlider  5+      -   5          Name$ VerticalSlider��5+         Orientation$ Vertical=�5,      $��30��6   $	��256��6)   $
��Left=�66   $��10q�5-      	   Color0$		 Red��5.      
   Color1$	
 Red��6      	HueChange$ Positive=�6!      SelectedColor$ 	#FF00FF00��5/         
Resolution$	 360��51      � 53      $��	50,10,0,0q�54   	   $	��256��55   	   $	��30 ��6   $	��Top=�6"   ��57      �� 58   
    j� TileMode$
 Tile=�6    j� Stretch$ Uniform=�59       j� 
AlignmentX$
 Left=�5:       j� 
AlignmentY$	 Top=�5;       j� Viewport$ 	0,0,30,30�5<       j� ViewportUnits$ Absolute=�5=       �� ImageSource 5>      �� 5?       �� 	UriSource$? 9/Qhta.WPF.Controls;component/Resources/OpacityPattern.png?�6   5A      .+�5C        -   5        $ HorizontalSlider��5C      $ 
Horizontal=�5D   	      Position$ 1Q�5E   	   5F   	   5H      5I      5J      