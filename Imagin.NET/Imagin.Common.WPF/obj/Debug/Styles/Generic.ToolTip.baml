   M S B A M L   `   `   `  ����L  HImagin.Common.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:Imagin.CommonImagin.Common  5      C&clr-namespace:Imagin.Common.ConvertersImagin.Common.Converters  5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0895    /Imagin.Common.Converters.CenterToolTipConverter   CenterToolTipConverter  �� 
TargetType �� Property  Brush.ToolTip.Background �� Value  Brush.ToolTip.Border  Brush.ToolTip.Foreground `� 	Converter �� Path �� RelativeSource �� 
TargetType � RowDefinitions �� CornerRadius	 �� Padding
 J� Opacity J� Effect3  -System.Windows.Media.Effects.DropShadowEffect  DropShadowColor   Color   Opacity   
BlurRadius   ShadowDepth   Imagin.Common.MaskedImage   Source   SourceColor�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      Bmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      %localclr-namespace:Imagin.Common  5      5
Converters&clr-namespace:Imagin.Common.Converters  5      %`  &	        '_�      7      5      6(   �� 5	      "  _�6   .�5
   
   ��  � ��# �  6'   �� 5   
    � ��# �  6(   �� 5   
    �@��CornerRadius$ 2��60   �� 5   
    � ��# �  6'   �� 5   
    � ��$
 Left=�60   �� 5   
    � ��$	 425��6%   �� 5   
    �@J�Opacity$
 0.95Q�6$   �� 5   
    � ��$
 15,7q�6$   �� 5   
    �@_�	Placement$ Bottom=�6&   �� 5   
    �@_�HorizontalOffset 5      `� 5      8   .1�5      �� $! PlacementTarget.ActualWidth�6E    6   �� *Self+6E   �� 5      $ ActualWidth�6E    6   �� *Self+6E   5      5      �� 5   
    � �� 5      �� 5      " _�6"   .R�5      �  5      �� 5       $
��Auto �6,   �� 5!      $��* �6,   .+�5#      �� $��1��5$      # z 5%      #��z��5&      #��z��5'      #	 z��5(      #��z��5)      #
 z
 5*       5+        5,   "   # �  63   $
 0.25Q�6]   $ 15Q�6l   $ 3Q�6|   ._�5.      �� #��z��5/   !   #��z��50   !   #��z��51   !   $��CharacterEllipsis=�52   !   $
��Wrap=�53   !   54      55        56      $��Center=�57      $��0,0,0,-5q�58      $��0q�59      $L Fpack://application:,,,/Imagin.Common.WPF;component/Images/Triangle.png��5:      # z��5;      5<      5=      5?      5@      5A      