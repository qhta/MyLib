   M S B A M L   `   `   `  ����L  HImagin.Colour.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:Imagin.Colour.ControlsImagin.Colour.Controls  5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089L HImagin.Common.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null0    *Imagin.Colour.Controls.ComponentSliderBase  �� 
TargetType �� Property �� Value �� 
TargetType � 	Resources   Key �� Color �� EndPoint �� 
StartPoint � Color	 � Offset
 J� SnapsToDevicePixels �� Angle T� X T� Y �� Left
 �� Top �� Stretch F�Name $  VerticalSliderThumbShadeGradient K� ZIndex �� Triggers P� Property P� Value �� 
TargetName � ColumnDefinitions ' #VerticalSliderTrackNormalBackground # VerticalSliderTrackNormalBorder �� CornerRadius J� RenderTransformOrigin �� ScaleX �� ScaleY �� AngleX �� AngleY ��Name ]�Name  ]� Thumb! n�Name  Style.Slider.Thumb.Vertical   Imagin.Common.CheckerBoard*   $Imagin.Colour.Controls.ColorViewBase"   CheckerBackground#   CheckerBackground$   CheckerForeground%   CheckerForeground& (� Stretch' ��Name( ��Name) �� IsSnapToTickEnabled  Style.Slider* � Maximum+ �� Mode, �� RelativeSource- � Minimum. �� TickFrequency/ � Value0 �� UpdateSourceTrigger�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      .local$clr-namespace:Imagin.Colour.Controls  5      -Common http://imagin.tech/imagin/common  5      %�  '         7  7  7  7  7  �� 5      "    67   .�5   
   ��  � �� 5      �� 5	      "   6"    5
      ���� 5      '#VerticalSliderTrackNormalBackground  $ 	#FFF0EDE0��6V   �� 5      #VerticalSliderTrackNormalBorder  $	 1,0&�6V   $	 0,0&�6e   .��5      � $ 	#FFAEB1AF��6+   $		 0.1Q�6=   � 5      $ White��6+   $	 .9Q�69   5      �� 5      $ VerticalSliderThumbShadeGradient  $	 0,0&�6W   $	 1,0&�6f   .��5      � $ 	#40FFFFFF��6+   $	 0Q�6=   � 5      $ 	#00FFFFFF��6+   $	 .3Q�6=   � 5      $ 	#00000000��6+   $	 .7Q�6=   � 5      $ 	#40000000��6+   $	 1Q�6=   5      �� 5      Style.Slider.Thumb.Vertical  "  n�6D   .�5      ��  �@�OverridesDefaultStyle . 6F   �� 5       � ��
 �   �6;   �� 5       � ��$ 11��67   �� 5       � ��$ 30��66   �� 5       � �� 5   "   �� 5   &   " n�66   .R�5   *   �� 
 . z�5    .   V� 5!   2   ~�5#   :   X� �� 5$   >   $	 -90Q�6N   T� 5%   >   $ 11Q�6Q   $	 5.5Q�6X   5&   ;   5(   3   .+�5*   .   �� $
��8.75��5+   1   $��8.792��5,   1   $ -4.25��5-   1   $ -10.209��5.   1   
������ 5/   1   F� 50   .   $
��8.75��51   1   $��8.792��52   1   
���   �$
 Fill=�54   1   
���   �,����`�� X4��8T笪�@��c��"��� X4��	$ -4.25��57   1   $ -7.209��58   1   F� 59   .   $ OuterBorder��5;   1   $��Stretch=�5:   1   $��Stretch=�5<   1   $	��9.5��5=   1   $��9.625��5>   1   8��  $��1��5A   1   $ -1��5B   1   ����0���  @͕�9  `<W0D <�����? `� `� o��YUU@$L�-k�?���� �@Zb  `� `� +( &?zd�bW #@u�G��B0?�1��#@ `� `� Q�� �p�"	@#@׈<� ���  `� `� 	$ -4.625��5D   1   $ -7.209��5E   1   ��5@   1   Z� 5E   1   5F   +    5G   *   P� 5H   .    �@J�IsMouseOver . 6M   P� 5I   .    � ��
 � ���6L   P� 5J   .    �@n�
IsDragging . 6L   P� 5K   .    � {� .  . �5L   2   �� $ OuterBorder��6K    � �� �	#FFD6D4C66d   5M   /   5O   '   5Q      5R      �� 5T      Style.Slider  "  ��65   .�5U      ��  �@��IsPressAndHoldEnabled .  6M   �� 5V       � ��
 ���� 6;   �� 5W       � �� �	#FFA1A1926;   �� 5X       �@��Orientation$ Vertical=�6<   �� 5Y       � ��$ 12��66   �� 5Z       � �� 5[   "   �� 5\   &   " ��66   .R�5]   *   �� #��z��5^   -   #��z��5_   -   #��z��5`   -   
 . ._�5b   .   �  5c   2   �� 5d   6   $
��Auto �6G   �� 5e   6   #��z��6G   $
��Auto �6m   �� 5f   6   $
��Auto �6G   .+�5i   2   �� $��1��5j   5   $��1��5k   5   8�� 8�� $��1q�5n   5   $ 1��5o   5   $��Right=�5p   5   $��0,0,10,0q�5q   5   $��3��5r   5   $ 0.5,0.5&�5s   5   z�5t   6   V� 5u   :   .~�5v   >   �� $ 1Q�6M   $ 1Q�6X   �� 5w   >   $ 0Q�6L   $ 0Q�6W   �� 5x   >   $ 0Q�6N   T� 5y   >   $ 0Q�6Q   $ 0Q�6W   5z   ;   ._�5|   6   �� $��-1,-6q�6=   .+�5}   :   �� $ PART_SelectionRange��6X   $y�Hidden=�6D   $	��4.0��6u   #��� ��6�   #��� ��6�   $	��1.0��6  5~   7   5   3   ]� 5�   2   $ 
PART_Track��68   $��1��6L     5�   6   n� 5�   :   $! Thumb��6@   8�� 6O   5�   3   5�   /   5�   +   5�   '   5�      5�      5�      .R�5�      �  5�      �� 5�      $
��Auto �6/   �� 5�      $
��Auto �6/   .+�5�        $��0��5�      #��z��5�      #��z��5�      ## z" 5�      #% z$ 5�      }�. 
 . $��Center=�5�      $��32��5�      ._�5�      (� $
& Fill=�6&   .��5�   "   �� $' 
PART_Image��5�   %   $
��Left=�5�   %   
 . $
��Fill=�5�   %   $	��Top=�5�   %   5�      5�      �� 5�      $( PART_Slider��5�      $��1��5�      $��Center=�5�      ) . 8�� $��Stretch=�5�      $��16��5�      * 5�      �� *	Maximum+$+ OneWay=�, �� *TemplatedParent+- 5�      �� *	Minimum+$+ OneWay=�, �� *TemplatedParent+. 5�      �� *Component.Increment+$+ OneWay=�, �� *TemplatedParent+/ 5�      �� *Value+$+ TwoWay=�, �� *TemplatedParent+$0 PropertyChanged=�5�      5�      5�      5�      5�      5�      