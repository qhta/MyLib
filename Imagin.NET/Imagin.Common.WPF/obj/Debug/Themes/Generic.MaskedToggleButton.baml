   M S B A M L   `   `   `  ����L  HImagin.Common.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:Imagin.CommonImagin.Common  5      C&clr-namespace:Imagin.Common.ConvertersImagin.Common.Converters  5      7 clr-namespace:Imagin.Common.LinqImagin.Common.Linq  5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5	      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;    5Imagin.Common.Converters.BooleanToVisibilityConverter    BooleanToVisibilityConverter8   2Imagin.Common.Converters.NullToVisibilityConverter  NullToVisibilityConverter&    Imagin.Common.MaskedToggleButton  �� 
TargetType �� Property �� Value �� 
TargetType ��Name �� Padding#   Imagin.Common.RippleDecorator   SourceColor   IsRippleEnabled   IsRippleEnabled	 � ColumnDefinitions   Imagin.Common.MaskedImage
  Name   SourceColor   SourceHeight   SourceHeight   SourceWidth   SourceWidth ��Name   ContentMargin �� 	Converter � Cursor   Source   DropDownButtonToolTip � ToolTip   DropDownButtonVisibility �� Triggers ]� 
Conditions �� Property �� Value �� 
TargetName �� RelativeSource �� AncestorType P� Property  P� Value�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      Bmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      %localclr-namespace:Imagin.Common  5      -Common http://imagin.tech/imagin/common   5      5
Converters&clr-namespace:Imagin.Common.Converters  5      )Linq clr-namespace:Imagin.Common.Linq  5      1System&clr-namespace:System;assembly=mscorlib  5	      %  &	        &	      '  &     7     5
      6.     5      6+   �� 5      "   66   .�5   
   ��  � ��
 ���� 6'   �� 5   
    �@�Cursor$
 Hand��6#   �� 5   
    � ��$ Center=�6.   �� 5   
    � ��$ Center=�65   �� 5   
    � �� 5      �� 5      "  6"   .R�5      �� $ PART_Border��5      #��z��5      #��z��5      #��z��5      # z��5      ._�5        #��z 5      # z 5      .��5      � 	 5   "   �� 5    &   $
��Auto �67   �� 5!   &   $
��Auto �67   �� 5"   &   $
��Auto �67   .+�5%   "     $
 
PART_Image��5&   %   $��0q�5'   %   $��0q�5(   %   # z 5)   %   # z 5*   %   # z 5+   %   #��z��5,   %   �� 5-   "   $ PART_Content��5/   %   $��1��5.   %   #��z��50   %   #��z 51   %   #��z��52   %   y�53   %   �� *	Content+8     55   "   $
 PART_DropDownButton��57   %   $��2��56   %   $
 Hand��58   %   $P Jpack://application:,,,/Imagin.Common.WPF;component/Images/TriangleDown.png��59   %   # z 5:   %   # z 5;   %   #��z��5<   %   #y�z 5=   %   5>      5?      5@       5A      ]� 5B       5C      �� 5D   "    �@d�	IsChecked$
 TrueY�6A   ./�5F      �� $ PART_Border��5G   !    5I   !   �� *CheckedToolTip+ �� "   �@�ToolTip5J      ]� 5K       5L      �� 5M   "    �@d�	IsChecked$
 TrueY�6A   �� 5N   "    6F   Z�  �@ CheckedToolTip./�5P      �� $ PART_Border��5Q   !    5S   !   �� *	ToolTip+ �� "   �@�ToolTip5T      ]� 5U       5V      �� 5W   "    �@d�	IsChecked$
 TrueY�6A   ./�5Y      �� $ PART_Content��5Z   !    5\   !   �� *CheckedContent+ �� "   � ��5]      ]� 5^       5_      �� 5`   "    �@d�	IsChecked$
 TrueY�6A   �� 5a   "    6F   Z�  �@ CheckedContent./�5c      �� $ PART_Content��5d   !    5f   !   �� *	Content+ �� "   � ��5g      ]� 5h       5i      �� 5j   "    �@d�	IsChecked$
 TrueY�6A   ./�5l      �� $ 
PART_Image��5m   !    5o   !   �� *CheckedSource+ �� "   �@ Source5p      ]� 5q       5r      �� 5s   "    �@d�	IsChecked$
 TrueY�6A   �� 5t   "    6E   Z�  �@ CheckedSource./�5v      �� $ 
PART_Image��5w   !    5y   !   �� *Source+ �� "   �@ Source5z      P� 5{       �@d�	IsChecked$  FalseY�67   . �5|      �� $ PART_Border��5}   !    5   !   �� *	ToolTip+ �� "   �@�ToolTip�� 5�      $ PART_Content��5�   !    5�   !   �� *	Content+ �� "   � ���� 5�      $ 
PART_Image��5�   !    5�   !   �� *Source+ �� "   �@ Source5�      5�      5�      5�      5�      