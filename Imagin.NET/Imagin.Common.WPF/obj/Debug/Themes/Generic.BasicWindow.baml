   M S B A M L   `   `   `  ����L  HImagin.Common.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:Imagin.CommonImagin.Common  5      C&clr-namespace:Imagin.Common.ConvertersImagin.Common.Converters  5      7 clr-namespace:Imagin.Common.DataImagin.Common.Data  5      7 clr-namespace:Imagin.Common.LinqImagin.Common.Linq  5	      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35O KWPFLocalizeExtension, Version=3.3.1.0, Culture=neutral, PublicKeyToken=nullR NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0898    2Imagin.Common.Converters.NullToVisibilityConverter   NullToVisibilityConverter<   6Imagin.Common.Converters.WindowTypeVisibilityConverter ! WindowTypeVisibilityConverter  DataTemplate.Window.Icon  �� 	Converter �� Mode   Imagin.Common.MaskedImage   Source " DataTemplate.Window.ResizeGrip �� Stretch  DataTemplate.Window.Title  
Brush.Text  ControlTemplate.BasicWindow �� 
TargetType   Imagin.Common.BasicWindow ��Name%  System.Windows.SystemParameters$   WindowNonClientFrameThickness �� Source � 	Resources%   Imagin.Common.Data.BindingProxy  Key	   Data
 �� RelativeSource �Name � RowDefinitions   HeaderBackground   HeaderBorderBrush   HeaderBorderThickness   HeaderVisibility � ColumnDefinitions   IconTemplate �� Path ��Name   TitleTemplate  BindingProxy'  !System.Windows.Shell.WindowChrome   IsHitTestVisibleInChrome   ButtonsPanel   Buttons �� Orientation ��Name   ButtonStyle � ToolTip2  ,WPFLocalizeExtension.Extensions.LocExtension �� 	Converter   ContentBackground   ContentBorderBrush    ContentBorderThickness!   ContentMargin" �� Padding# ��Name$   Footer%   FooterTemplate&   ResizeGripTemplate'   OverlayTemplate(   OverlayVisibility) �� Triggers* P� Property+ P� Value, �� 
TargetName- �� Property. �� Value/ ]� 
Conditions0 �� Property1 �� Value 	 Style.BasicWindow2 �� 
TargetType)   #Imagin.Common.Linq.WindowExtensions 
 Style.Window.Button&	   System.Windows.Media.TextOptions3   CaptionHeight4   CornerRadius5   GlassFrameThickness6   ResizeBorderThickness7 �� BasedOn8 �� Triggers 
   Imagin.Common.WindowFooter�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    0l(http://wpflocalizeextension.codeplex.com  5      :x,http://schemas.microsoft.com/winfx/2006/xaml     5      %localclr-namespace:Imagin.Common  5      -Common http://imagin.tech/imagin/common   5      5
Converters&clr-namespace:Imagin.Common.Converters  5      )Data clr-namespace:Imagin.Common.Data  5      )Linq clr-namespace:Imagin.Common.Linq  5	      %�  &	        &	      &	 &     7   &	 �     &	 �    &	 �    7  7  7  7  &		 '    7  7  7  '  t    7 	 7  7  '
  8       5
      6+     5      6/   �� 5      .R�5   
   � $��10,0,0,0q�5      $��Center=�5      y�5      �� 8    $ OneWay=�.+�5         6    �� $ OneWay=�5      5      �� 5      .R�5   
   F� �����@B ��� ���- ��� �@KL ��� ���j ��� �@T� ��� ��ا ��� Ѐ� @T� р� �ا Ѐ�[ @T� р�[ �ا Ѐ�� @T� р�� �ا �@KL ��[ ���j ��[ �@T� ��[ ��ا ��[ Ѐ�[ @KL р�[ ��j Ѐ�� @KL р�� ��j �@T� �� ��ا �� Ѐ�� @B р�� ��- $��12��5      $��Right=�5      $��0,0,4,4q�5      $��1��5      $
 None=�5      $��Bottom=�5      $��12��5       ��5      �� *Data.Foreground+$ OneWay=�5       5!      �� 5#      .R�5$   
   �� $��13(�5%      #���  5&      $
��Left=�5'      $��	10,-1,0,0q�5(      $��CharacterEllipsis=�5*      $��Center=�5+      ��5)      �� *
Data.Title+$ OneWay=�5+      5,      �� 5.      "  6:   .R�5/   
   �� $ PART_Border��50      #��z��51      #��z��52      #��z��53      ��54      �� # Z  55      ��  56      BindingProxy 	 69   �� $ OneWay=�
 �� *TemplatedParent+57      ._�58      � $ 
LayoutRoot��6    59      �� 5:      $
��Auto �6$   �� 5;      $��* �6$   �� 5<      $
��Auto �6$   .+�5?      �� #��z 5@      #��z 5A      #��z 5B      $��32��5C      #y�z 5D      ._�5E      �  5F      �� 5G      $
��Auto �6/   �� 5H      $
��Auto �6/   �� 5I      $��* �6/   �� 5J      $
��Auto �6/   .+�5M      �� #��z 5O      ��5N      �� $" (Linq:WindowExtensions.Icon)�
 �� *TemplatedParent+5O      �� 5P      $ PART_TitleGrid��5R      $��1��5Q      #��z 5T      $��30��5U      $��50��5V      ��5S      �� 8   5V      �� 5X      $��2��5Y       . #��z 5[      #��z 5\      $��Right=�5]      $��Center=�5^      5_      �� 5a      $��3��5b      $ 
Horizontal=�5c      $	��Top=�5d       . .+�5f      �� $ PART_MinimizeButton��5g   !   $I��Cpack://application:,,,/Imagin.Common.WPF;component/Images/Minus.png��5i   !   #��z 5j   !   ��5h   !   �� *MinimizeCommand+
 �� *TemplatedParent+ 5k   !     *
Minimize+� 5m      y�6#   �� *Type+8  .+�5n   "   �� $ PART_RestoreButton��5o   %   $K��Epack://application:,,,/Imagin.Common.WPF;component/Images/Restore.png��5q   %   #��z 5r   %   ��5p   %   �� *RestoreCommand+
 �� *TemplatedParent+ 5s   %     *	Restore+�� 5t   "   $ PART_MaximizeButton��5u   %   $L��Fpack://application:,,,/Imagin.Common.WPF;component/Images/Maximize.png��5w   %   #��z 5x   %   ��5v   %   �� *MaximizeCommand+
 �� *TemplatedParent+ 5y   %     *
Maximize+5z      �� 5|      $ PART_CloseButton��5}   !   $E��?pack://application:,,,/Imagin.Common.WPF;component/Images/X.png��5   !   #��z 5�   !   ��5~   !   �� *BeginCloseCommand+
 �� *TemplatedParent+ 5�   !     *Close+5�      5�      5�      �� 5�      $��1��6#   .u�5�      �� $ PART_ContentBorder��5�      #��z 5�      #��z 5�      #��z  5�      #��z! 5�      #" z��5�      ._�5�      �� $# PART_Content��5�      #��z��5�      #��z��5�      5�      5�      �� 5�      $# PART_Footer��5�      $��2��5�      #��z$ 5�      #��z% 5�      �� 5�      $ PART_ResizeGrip��5�      $��3��5�      #��z& 5�      $y�	Collapsed=�5�      ��5�      �� $ OneWay=�8  5�      �� 5�      $ PART_Overlay��5�      $��3��5�      #��z' 5�      #y�z( 5�      ��5�      �� $ OneWay=�8  5�      5�      5�      ) 5�   
   P� 5�      * �@�WindowState$+ 	Maximized=�6-   . �5�      �� $, PART_MaximizeButton��6   - � y�$. 	Collapsed=�6P   �� 5�      $, PART_RestoreButton��6   - � y�$. Visible=�6O   5�      P� 5�      * �@�WindowState$+ Normal=�6-   . �5�      �� $, PART_MaximizeButton��6   - � y�$. Visible=�6P   �� 5�      $, PART_RestoreButton��6   - � y�$. 	Collapsed=�6O   5�      ]� 5�      / 5�      �� 5�      0 �@�
ResizeMode$1 CanResizeWithGrip=�66   �� 5�      0 �@�WindowState$1 Normal=�67   ./�5�      �� $, PART_ResizeGrip��6   - � y�$. Visible=�6L   5�      5�      �� 5�      "2  6&   .�5�   
   ��  - �@ OverridesDefaultBehavior. . 6K   �� 5�   
   - �@�AllowsTransparency. . 6/   �� 5�   
   - �@ ButtonsPanel. 5�      �� 5�      .R�5�      �� $ 
Horizontal=�6!   5�      5�      �� 5�   
   - �@ ButtonStyle#. � 
 6(   �� 5�   
   - � ��#. �  6'   �� 5�   
   - �@ IconTemplate8.   6)   �� 5�   
   - �@ ResizeGripTemplate8.  6/   �� 5�   
   - �@�ShowInTaskbar. . 6*   �� 5�   
   - �@	 TextFormattingMode$. Display=�6;   �� 5�   
   - �@ TitleTemplate8.  6*   �� 5�   
   - �@�UseLayoutRounding. . 6.   �� 5�   
   - �@�WindowStyle$
. None=�6(   �� 5�   
   - �@ WindowChrome. 5�        5�      $3 35Q�5�      $4 0��5�      $5 0,0,0,1q�5�      $6 5q�5�      5�      5�      �� 5�      "2  6/   87   8 5�   
   P� 5�      * �@ Type$+ Window=�6&   . �5�      �� - �@�
ResizeMode$. CanResizeWithGrip=�6/   �� 5�      - � ��8.  6-   5�      P� 5�      * �@ Type$
+ Tool=�6&   . �5�      �� - � ��8.  6-   5�      P� 5�      * �@ Type$+ Blank=�6&   . �5�      �� - � ��. 5�      �� 5�      "  6*   .R�5�      �� #��z��5�   !   #��z��5�   !   #��z��5�   !   }�.  . #" z��5�   !   ._�5�   "   �� .u�5�   &   �� 5�   #   5�      5�      5�      �� 5�      - �@ WindowChrome. 5�        5�      $3 0Q�5�      $4 0��5�      $5 0q�5�      $6 0q�5�      5�      5�      5�      �� 5     "2 
 60   .�5  
   �� - � ��. 5     �� 5     " 
 6"   .R�5     �� #��z��5     #��z��5	     #��z��5
     #" z��5     ._�5     �� #��z��6+   5     5     5     5     5     