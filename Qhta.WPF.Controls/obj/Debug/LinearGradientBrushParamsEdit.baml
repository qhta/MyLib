   M S B A M L   `   `   `  ����X  TQhta.WPF.Controls, Version=1.0.2.0, Culture=neutral, PublicKeyToken=2195313f51a3d9f45clr-namespace:Qhta.WPF.ControlsQhta.WPF.Controls  5      U QQhta.WPF.Utils, Version=1.1.0.0, Culture=neutral, PublicKeyToken=cec4d5cf70dd23c1G4clr-namespace:Qhta.WPF.Utils;assembly=Qhta.WPF.UtilsQhta.WPF.Utils 5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5      5    /Qhta.WPF.Controls.LinearGradientBrushParamsEdit   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      1localclr-namespace:Qhta.WPF.Controls     5      Futils4clr-namespace:Qhta.WPF.Utils;assembly=Qhta.WPF.Utils     5      6sys&clr-namespace:System;assembly=mscorlib     5      -   5          >�Name$  ThisControl��5	      .��5      �  � ColumnDefinitions 5      �� 5      $��* �6   �� 5      $	��160 �6   .+�5      �� -   5         ��Name$ GradientCanvas��5      $��20q�5   	   $	��Top=�5   	   $��Stretch=�5   	   ��5   	   ��  �� RelativeSource ��  �� Mode$
 Self=� �� Path$ ActualWidth�.+�5      �� 
���   �$��1��5      ��5      ��  �� $ FindAncestor=� �� AncestorType" ��$ ActualWidth���5      ��  �� $ FindAncestor=�" ��$ ActualHeight���5      ��  �� ElementName$ ThisControl��$ EditedBrush� �� Mode$ OneWay=�5      $   Qhta.WPF.Controls.GradientLine  5      -   5        	  Name$	 GradientLine��5      
���   �$��1��5      }�.  
   ClipToMargin
 .    
ClipMargin$ 20q�5#      ��5      ��  �� $ FindAncestor=�" ��$ ActualWidth���5      ��  �� $ FindAncestor=�" ��$ ActualHeight�   GradientStops 5       �� $ ThisControl��$ EditedBrush.GradientStops�5#      5%      �� 5&      $��1��6    �� Orientation$ Vertical=�6!   $��0,10,0,0q�68   .+�5'      �� $ 
Horizontal=�6   .+�5(   
   �� $��Angle��6   &    Qhta.WPF.Controls.NumericEditBox  5)   
   -   5          Name$ AngleNumBox��5*      &    Qhta.WPF.Controls.NumericEditBox   ValueChanged$ AngleNumBox_ValueChanged��50         Minimum$ -5~�5+         Maximum$	 365~�5,         PageIncrement$ 5~�5-      $��50��5.      $��20��5/      50      �� 52   
   $��3q�6   [°]6$   53   	   �� 54      $ 
Horizontal=�6   $��3q�6,   .+�55   
   �� $��Fix start point��56   	   $��Right=�58   	    � ToolTip$z tIf this box is checked then the start points is locked to one of four brush edge rectangle corners during angle edit��59   	    d� 	IsChecked 57   	   �� $ ThisControl��$ 	IsP1Fixed�59   	   5;   	   �� 5<      $ 
Horizontal=�6   $��3q�6,   .+�5=   
   �� $��Fix end point��5>   	   $��Right=�5@   	   $c ]If this box is checked then the end point is locked to brush rectangle edge during angle edit��5A   	    5?   	   �� $ ThisControl��$ 	IsP2Fixed�5A   	   5C   	   �� 5D      $ 
Horizontal=�6   $��3q�6,   .+�5E   
   �� $��Preserve angle��5F      $��Right=�5H      $L FIf this box is checked then angle is preserved during start point edit��5I       5G      �� $ ThisControl��$ IsAnglePreserved�5I      5K   	   �� 5L      -   5         ��Name$ 
PointEdits��5L      $ Vertical=�6'   .+�5M   
   �� $��Start Point��6   �� 5N   
   $ 
Horizontal=�6   .+�5O      �� .��6   X6     5P      -   5        $ StartXNumBox��5Q      $ StartXNumBox_ValueChanged��5V      $
 -100~�5R      $	 200~�5S      $��40��5T      $��20��5U      5V      �� 5X      $��3q�6   [%]6&   �� 5Y      .��6   Y6     5Z      -   5        $ StartYNumBox��5[      $ StartYNumBox_ValueChanged��5`      $
 -100~�5\      $	 200~�5]      $��40��5^      $��20��5_      5`      �� 5b      $��3q�6   [%]6&   5c      �� 5d   
   .��6   	End Point6   �� 5e   
   $ 
Horizontal=�6   .+�5f      �� .��6   X6     5g      -   5        $ 
EndXNumBox��5h      $ EndXNumBox_ValueChanged��5m      $
 -100~�5i      $	 200~�5j      $��40��5k      $��20��5l      5m      �� 5o      $��3q�6   [%]6&   �� 5p      .��6   Y6     5q      -	   5        $ 
EndYNumBox��5r      $ EndYNumBox_ValueChanged��5w      $
 -100~�5s      $	 200~�5t      $��40��5u      $��20��5v      5w      �� 5y      $��3q�6   [%]6&   5z      5{   	   5|      5}      5~      