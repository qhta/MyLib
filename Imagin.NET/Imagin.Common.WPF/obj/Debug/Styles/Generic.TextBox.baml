   M S B A M L   `   `   `  ����L  HImagin.Common.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:Imagin.CommonImagin.Common  5      C&clr-namespace:Imagin.Common.ConvertersImagin.Common.Converters  5      7 clr-namespace:Imagin.Common.LinqImagin.Common.Linq  5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35O KWPFLocalizeExtension, Version=3.3.1.0, Culture=neutral, PublicKeyToken=nullR NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   Style.TextBox.Placeholder  �� 
TargetType �� BasedOn �� Property  Brush.TextBox.Placeholder �� Value  Style.TextBoxBase  Brush.TextBox.Background  Brush.TextBox.Border  Brush.TextBox.Caret � DataContext �� RelativeSource.   (System.Windows.Input.ApplicationCommands
    Cut m� Command m� InputGestureText	    Copy
    Paste  
Brush.Text  Brush.TextBox.Selection �� Triggers P� Property P� Value " Brush.TextBox.MouseOver.Border  	 Brush.TextBox.Focused.Border $
  Brush.TextBox.Disabled.Selection  Style.TextBox.Large  Style.TextBox.Blank  TextBoxBorder �� 
TargetType ��Name �� Padding ��Name J� SnapsToDevicePixels   Imagin.Common.TextBox � 	Resources;   5Imagin.Common.Converters.BooleanToVisibilityConverter  Key    Imagin.Common.MaskedButton   Source � ToolTip2  ,WPFLocalizeExtension.Extensions.LocExtension   BooleanToVisibilityConverter �� 	Converter(   "Imagin.Common.ScrollViewerExtended �Name � ColumnDefinitions � RowDefinitions ��Name ��Name �� CanContentScroll �� CanHorizontallyScroll �� CanVerticallyScroll �� Mode  ��Name4  .System.Windows.Automation.AutomationProperties!   AutomationId" � Cursor# �� ScrollableHeight$ � Maximum% � Minimum& �� ViewportHeight' �� ViewportSize*( �� #ComputedVerticalScrollBarVisibility) � Value* �� ScrollableWidth+ �� Orientation, �� ViewportWidth,- �� %ComputedHorizontalScrollBarVisibility6   0Imagin.Common.Converters.InverseBooleanConverter&    Imagin.Common.MaskedToggleButton.   Source/ d� 	IsChecked  InverseBooleanConverter!	   Imagin.Common.EditableLabel
   Imagin.Common.UriBox   Imagin.Common.VersionBox   Imagin.Common.UpDown0   SourceHeight1   SourceWidth   Imagin.Common.ByteUpDown!   Imagin.Common.DecimalUpDown    Imagin.Common.DoubleUpDown   Imagin.Common.FloatUpDown   Imagin.Common.Int16UpDown   Imagin.Common.Int32UpDown   Imagin.Common.Int64UpDown!   Imagin.Common.UDoubleUpDown    Imagin.Common.UInt16UpDown    Imagin.Common.UInt32UpDown    Imagin.Common.UInt64UpDown"   Imagin.Common.DateTimeUpDown  Brush.Popup.Background  Brush.Popup.BorderBrush"   Imagin.Common.TimeSpanUpDown   Imagin.Common.TimeUpDown#   Imagin.Common.AlphaNumericBox   Imagin.Common.FileBox2   SourceColor6  0WPFLocalizeExtension.Extensions.LocTextExtension3   Suffix   Imagin.Common.MaskedImage4   Source5   SourceColor   Imagin.Common.HexBox    Imagin.Common.RegexBox!   Imagin.Common.TokenView."   (Imagin.Common.Linq.TextBoxBaseExtensions3#   -Imagin.Common.Linq.FrameworkElementExtensions�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    0l(http://wpflocalizeextension.codeplex.com  5      Bmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      :x,http://schemas.microsoft.com/winfx/2006/xaml     5      %localclr-namespace:Imagin.Common  5      5
Converters&clr-namespace:Imagin.Common.Converters  5      )Linq clr-namespace:Imagin.Common.Linq  5      -Common http://imagin.tech/imagin/common   5	      %�$  &	        7��&	 O     &	 �    7  &	 u    7  '�� �    7  '  �    7  7  7   7  '	  �    7 '
  �    7 '  �    7 '      7 '  �    7 '      7 '  #    7 '  E    7 '  g    7 '  �    7 '  �    7 '  �    7 '  �    7 '      7 '  3    7 '  U    7 '  �    7 '  �    7 '  �    7 '  �    7 '  �    7 '   �    7 '�� 
     7  '!  �!    7���� 5
      "  ��5   	   8   .�5   
   ��  � ��# �  6'   5      �� 5      "  ��6&   .�5   
   ��  � ��# �  6'   �� 5   
    � ��# �  6(   �� 5   
    � ��$ 1q�6,   �� 5   
    �@��
CaretBrush# �  6'   �� 5   
    �@�ContextMenu 5      �� 5       6   �� *PlacementTarget+ �� *Self+.@�5      m� # Z 5      $	��Cut��5      $ Ctrl + X��5      m� 5      # Z	 5       $
��Copy��5!      $ Ctrl + C��5"      m� 5#      # Z
 5$      $��Paste��5%      $ Ctrl + V��5&      5'      5)      �� 5*   
    � ��$1 +pack://application:,,,/Fonts/#Swis721 Cn BT)�6'   �� 5+   
    � ��$ 13(�6%   �� 5,   
    � ��# �  6'   �� 5-   
    � ��$ Stretch=�67   �� 5.   
    � ��$ 3q�6$   �� 5/   
    �@��SelectionBrush# �  6+   �� 50   
    �@��SelectionOpacity$	 0.4Q�6-   �� 51   
    � ��$ Hidden=�6G   �� 52   
    � ��$ Center=�65    53   
   P� 54       �@J�IsMouseOver . . �55      ��  � ��# �  60   56      P� 57       �@J�	IsFocused . . �58      ��  � ��# � 	 60   59      P� 5:       �@��
IsReadOnly . . �5;      ��  �@��SelectionBrush# � 
 63   �� 5<       �@J�Opacity$	 0.8Q�6,   5=      P� 5>       � {� .  . �5?      ��  �@��SelectionBrush# � 
 63   �� 5@       �@J�Opacity$	 0.8Q�6,   5A      5C      �� 5E      "  ��6(   8   .�5F   
   ��  �@��AcceptsReturn . 6*   �� 5G   
    � ��$ 48��6&   �� 5H   
    � ��$	 Top=�65   5I      �� 5K      "  ��6(   8   .�5L   
   ��  �@�OverridesDefaultStyle . 62   �� 5M   
    � ��
 ���� 6'   �� 5N   
    � ��$ 0q�6,   �� 5O   
    � ��# �  6(   �� 5P   
    � ��# �  6'   �� 5Q   
    � ��$ Center=�65   �� 5R   
    � �� 5S      �� 5T      " ��6"   .R�5U      �� $ border��6�   #��z��6   # z��6Q   #��z��6u   #��z��6�   #��z��6�   ._�5V      �� $ PART_ContentHost��6F   $��Stretch=�6'   $��Center=�6`   # z 6�   #��z��6�   5W      5X      5Z      5[      �� 5_      "  ��6%   8   6C   �� 5a      "   6+   8   .�5b   
   ��  �@ CharacterMaskForeground# �  64   �� 5c   
    �@ ClearButtonTemplate 5d      �� 5e       5f      ��  5g       BooleanToVisibilityConverter 6B   5h      .R�5i        |�.  $J Dpack://application:,,,/Imagin.Common.WPF;component/Images/XRound.png��5m      $��5,0,0,0q�5n       5j      �� *Data+��5k      �� *ClearCommand+ 5o        *Imagin.Common.WPF:Main:Clear+y�5p      �� *	IsEnabled+8   �� *Self+5q      5s      �� 5t   
    �@ EnterButtonTemplate 5u      �� 5v      .R�5w        |�.  $��5,0,0,0q�5{      $M Gpack://application:,,,/Imagin.Common.WPF;component/Images/NextArrow.png��5|       5x      �� *Data+��5y      �� *EnterCommand+ 5}        *Imagin.Common.WPF:Main:Enter+5~      5�      �� 5�   
    �@ InnerPadding$ 0q�6)   �� 5�   
    � ��$ 5q�6$   �� 5�   
    �@ PlaceholderStyle8  6-   �� 5�   
    �@ ScrollViewerTemplate 5�      �� 5�      "  6"   .R�5�      � $
 Grid��5�      #��z��5�       5�      �� 5�      $��* �6/   �� 5�      $
��Auto �6/    5�      �� 5�      $��* �6,   �� 5�      $
��Auto �6,   .+�5�      �� $ Corner��5�      $��1��5�      $��1��5�      #��� ��5�      �� 5�      $! PART_ScrollContentPresenter��5�      $��0��5�      $��0��5�      # z��5�       .   .  #��z��5�      #��z��5�      ��5�      �� *	Padding+$ OneWay=� �� *TemplatedParent+�� 5�      $  PART_VerticalScrollBar��5�      $��0��5�      $��1��5�      $! VerticalScrollBar��5�      $" Arrow��5�      #$ z# 5�      $% 0Q�5�      #' z& 5�      #y�z( 5�      ) 5�      �� *VerticalOffset+$ OneWay=� �� *TemplatedParent+�� 5�      $  PART_HorizontalScrollBar��5�      $��1��5�      $��0��5�      $! HorizontalScrollBar��5�      $" Arrow��5�      #$ z* 5�      $% 0Q�5�      $+ 
Horizontal=�5�      #' z, 5�      #y�z- 5�      ) 5�      �� *HorizontalOffset+$ OneWay=� �� *TemplatedParent+5�      5�      5�      5�      �� 5�   
    �@ ToggleButtonTemplate 5�      �� 5�       5�      ��  5�      InverseBooleanConverter 6=   5�      .R�5�        |�.  $��5,0,0,0q�5�      $J. Dpack://application:,,,/Imagin.Common.WPF;component/Images/Toggle.png��5�      $��Center=�5�       5�      �� *Data+/ 5�      �� *IsCharacterMaskingEnabled+8  5�      5�      5�       5�   
   P� 5�       �@J�	IsFocused . . �5�      ��  �@ CharacterMaskForeground# � 	 6<   5�      5�      �� 5�      "  	 61   8   6[   �� 5�      "  
 6*   8   6M   �� 5�      "   6.   8   6U   �� 5�      "   6*   8   .�5�   
   ��  �@ DownButtonTemplate 5�      �� 5�      .R�5�        $
��Down��5�      |�.  $M Gpack://application:,,,/Imagin.Common.WPF;component/Images/ArrowDown.png��5�      $0 9Q�5�      $1 9Q�5�       5�      �� *Data+$ OneWay=���5�      �� *DownCommand+ 5�        *!Imagin.Common.WPF:Main:Decrease+5�      5�      �� 5�   
    �@ UpButtonTemplate 5�      �� 5�      .R�5�        $��Up��5�      |�.  $K Epack://application:,,,/Imagin.Common.WPF;component/Images/ArrowUp.png��5�      $0 9Q�5�      $1 9Q�5�       5�      �� *Data+$ OneWay=���5�      �� *	UpCommand+ 5�        *!Imagin.Common.WPF:Main:Increase+5      5      5  
   P� 5      �@ 	CanUpDown . . �5     ��  � ��$	 5,0q�6,   5     P� 5      �@ 	CanUpDown .  . �5     ��  � ��$ 5q�6,   5	     5     �� 5     "   6.   8   6U   �� 5     "   61   8   6[   �� 5     "   60   8   6Y   �� 5     "   6/   8   6W   �� 5     "   6/   8   6W   �� 5     "   6/   8   6W   �� 5     "   6/   8   6W   �� 5     "   61   8   6[   �� 5     "   60   8   6Y   �� 5!     "   60   8   6Y   �� 5#     "   60   8   6Y   �� 5'     "   62   8   .�5(  
   ��  �@ DropDownStretch$
 None=�6,   �� 5)  
    �@ DropDownStyle 5*     �� 5+     "  ��6   .�5,     ��  � ��# �  63   �� 5-      � ��# �  64   �� 5.      � ��$ 1q�68   5/     51     �� 52  
    �@ StaysOpenOnSelection . 61   53     �� 55     "   62   8   6]   �� 57     "   6.   8   .�58  
   ��  �@ DropDownStyle 59     �� 5:     "  ��6   .�5;     ��  � ��# �  63   �� 5<      � ��# �  64   �� 5=      � ��$ 1q�68   5>     5@     5A     �� 5E     "   63   8   6_   �� 5G     "   6+   8   .�5H  
   ��  �@ BrowseButtonTemplate 5I     �� 5J     .R�5K       |�.  $��5,0,0,0q�5P     $J Dpack://application:,,,/Imagin.Common.WPF;component/Images/Folder.png��5Q      5L     �� *Data+$ OneWay=���5M     �� *BrowseCommand+2 5O     �� *
Foreground+5Q     5R     5T     �� 5U  
    �@ BrowseButtonToolTip 60     *Browse+�� 5V  
    �@ 
BrowseMode$ OpenFile=�6'   �� 5W  
    �@ BrowseTitle 6(     *Browse+$	3 ...���� 5X  
    �@ ShowBrowseButton . 6-   �� 5Y  
   ! �@ ValidityIndicatorTemplate 5Z     �� 5[     .R�5\       $��5,0,0,0q�5_     $R4 Lpack://application:,,,/Imagin.Common.WPF;component/Images/CheckmarkRound.png��5`      5]     �� *Data+$ OneWay=�5 5^     �� *
Foreground+5`     5a     5c     �� 5d  
     �@ ValidityIndicatorToolTip 65     *Valid+5e     �� 5g     "   6*   8   6M   �� 5i     "    6,   8   6Q   �� 5m     "  ��6)   8   .�5n  
   ��  �@�OverridesDefaultStyle . 62   �� 5o  
    � ��$
 Left=�67   �� 5p  
    � ��$ 10q�6$   �� 5q  
    � ��$ Center=�65   �� 5r  
    � ��$
 Auto=�6E   �� 5s  
    � �� 5t     �� 5u     " ��6"   .R�5v     �� $��Bd��5w     #��z��5x     #��z��5y     #��z��5z      . ._�5|     �� $��PART_ContentHost��5}     # z 5~     5     5�     5�     5�     �� 5�     "  ! 6-   8   .�5�  
   ��  �@" EnableCopyCommand .  6I   �� 5�  
    �@" EnableCutCommand .  6H   �� 5�  
    �@" EnablePasteCommand .  6J   �� 5�  
    �@# EnableContextMenu .  6N   �� 5�  
    � ��$	 3,7q�6$   5�     5�     