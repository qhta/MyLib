   M S B A M L   `   `   `  ����L  HImagin.Colour.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:Imagin.Colour.ControlsImagin.Colour.Controls  5	      H DImagin.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullP8clr-namespace:Imagin.Common.Media;assembly=Imagin.CommonImagin.Common.Media 5      M+clr-namespace:Imagin.Colour.Controls.ModelsImagin.Colour.Controls.Models  5      (    "Imagin.Colour.Controls.ColorPicker   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      O KWPFLocalizeExtension, Version=3.3.1.0, Culture=neutral, PublicKeyToken=null0l(http://wpflocalizeextension.codeplex.com  5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      6local$clr-namespace:Imagin.Colour.Controls     5	      L HImagin.Common.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-Common http://imagin.tech/imagin/common  5
      KCommon.Media8clr-namespace:Imagin.Common.Media;assembly=Imagin.Common  5      8Models+clr-namespace:Imagin.Colour.Controls.Models  5      -   5          Imagin.Common.UserControl   Name$  PART_ColorPicker��5       � 	Resources 5      %  Imagin.Common.Data.BindingProxy   BindingProxy   Data �� ElementName;  5Imagin.Common.Converters.BooleanToVisibilityConverter   BooleanToVisibilityConverter2  ,Imagin.Common.Converters.ColorToHexConverter  ColorToHexConverter>  8Imagin.Common.Converters.ColorToSolidColorBrushConverter # ColorToSolidColorBrushConverter  DataTemplate.Models �� DataType.   (Imagin.Colour.Controls.Models.ColorModel � 	Resources  Key �� Mode �� RelativeSource+   %Imagin.Colour.Controls.ColorSpaceView  Name	   
ColorSpace-   'Imagin.Colour.Controls.Models.Component*	   $Imagin.Colour.Controls.ComponentView
 	 Name 	  Color �� AncestorType(
   "Imagin.Colour.Controls.ColorPicker 	  ColorSpaceModel 	  ComponentModel 	  	Converter 	  Value(  "Imagin.Common.Linq.PanelExtensions   Spacing   TrimLast   VerticalContentAlignment � ColumnDefinitions ��Name ��Name   Imagin.Common.DoubleUpDown  Name  Imagin.Common.UpDown   	CanUpDown�  {Imagin.Common.NumericUpDown`1[[System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	Incrementz  tImagin.Common.UpDown`1[[System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   Maximum   Minimum   StringFormat ��Name �� Triggers �� Value  �� Binding! �� 
TargetName" �� Value# �� IsAsync$ �� UpdateSourceTrigger% �� Property& 	�Name' 	� 	GroupName( d� 	IsChecked'  !Imagin.Common.Media.ComponentUnit)   Degrees*   None+   Percent, �� 
TargetType- �� Orientation��%�  &	        &	 3     &	 F     &	 Y     &	 l       5   
    63   �� $ PART_ColorPicker��  5   
   6.     5   
   6%     5   
   61   �� 5   
   "  5       5      ��  5      ComponentProxy  6=   �� $ OneWay=� �� *Self+5      .R�5        $ PART_Components��5      $��0,0,0,15q�5      	 5      �� $ OneWay=���5      �� *
Components+$ OneWay=���5       �� 5!      "  6#   (��       *, +) 5"      ��  5#       BooleanToVisibilityConverter 6B   5$      .R�5%      	  $
 PART_ComponentView��5&       5'      �� *
NewColor+ �� " 
  5(      �� *DataContext+$ OneWay=� �� "   5)      �� $ OneWay=� 5*      �� *	Converter+$ OneWay=� �� " 
  5+      �� *Value+.��5,      � $ 0,0,7,0q�5-   !    . $ Center=�5/   !    50   "   �� 51   &   $ PART_Column0��67   �� 52   &   ��67   �� *ComponentWidth+ �� " 
 �� 53   &   $
��Auto �67   .+�56   "   �� $ 
PART_Label��57   %   ��58   %   �� $ OneWay=�  59   "   $ 
PART_Value��5;   %   $��1��5:   %    5<   %   �� *CanUpDownComponents+ �� " 
  5=   %   �� *	Increment+$ OneWay=� 5>   %   �� *	Maximum+$ OneWay=� 5?   %   �� *	Minimum+$ OneWay=� 5@   %   �� *ComponentStringFormat+ �� " 
 �� 5A   "   $ 	PART_Unit��5B   %   $��2��5C   %   $��Consolas)�5D   %   5E      5F       5G      �� 5H      $ False��6�     6*   �� *	IsAsync+ �� " 
 .a�5I   "   �� $! 
PART_Value��6)   " 6R   �� *Value+# .  $ TwoWay=�$$ PropertyChanged=�% �@ Value5J      �� 5K      $
 True��6�     6*   �� *	IsAsync+ �� " 
 .a�5L   "   �� $! 
PART_Value��6)   " 6R   �� *Value+# . $ TwoWay=�$$ PropertyChanged=�% �@ Value5M      �� 5N      $
 True��6H     6*   �� *	CanSelect+.a�5O   "   �� $! PART_Column0��6)   % � ��$" 50 �6T   �� 5P   "   $! 
PART_Label��6)   % � ��" 5Q   &   �� 5R   *   .R�5S   .   	� $& 
PART_Radio��5T   1   $' 
ColorSpace��5V   1   $��Center=�5W   1   ��5U   1   �� *Label+( 5X   1   �� *
IsSelected+$ TwoWay=�$$ PropertyChanged=�5Y   +   5[   #   5\      �� 5]      $ False��6H     6*   �� *	CanSelect+.a�5^   "   �� $! PART_Column0��6)   % � ��$" 25 �6T   �� 5_   "   $! 
PART_Label��6)   % � ��" 5`   &   �� 5a   *   .R�5b   .   �� $��Center=�5d   1   ��5c   1   �� *Label+5d   1   5e   +   5g   #   5h      �� 5i      # Z) 6C     6*   �� *Unit+.a�5j   "   �� $! 	PART_Unit��6)   % � ��$" °��6S   5k      �� 5l      # Z* 6C     6*   �� *Unit+.a�5m   "   �� $! 	PART_Unit��6)   % � ��$"  ��6S   5n      �� 5o      # Z+ 6C     6*   �� *Unit+.a�5p   "   �� $! 	PART_Unit��6)   % � ��$" %��6S   5q      5s      5u       5v      �� 5w      $ 
Horizontal��6>     6   �� *Orientation+.a�5x      �� $! PART_Components��6   % � ��" 5y      �� 5z      ", ��6$   .�5{   "   �� % � ��$" 0,0,10,0q�6;   5|      5~      �� 5      $! PART_Components��6   % � ��" 5�      �� 5�      .R�5�   "   �� $- 
Horizontal=�6-   5�      5�      5�      �� 5�      $ Vertical��6>     6   �� *Orientation+.a�5�      �� $! PART_Components��6   % � ��" 5�      �� 5�      ", ��6$   .�5�   "   �� % � ��$" 0,0,7,7q�6;   5�      5�      �� 5�      $! PART_Components��6   % � ��" 5�      �� 5�      .R�5�   "   �� 5�      5�      5�      5�      5�      .��5�      � . � DataContext. 5�   	   �� $ PART_ColorPicker�� 5�   
   �� 5�      $
��Auto �6   �� 5�      ��6   �� *
InputWidth+�� 5�      $
��Auto �6   .+�5�   
   *   $Imagin.Colour.Controls.ColorSelector  /   Alpha/ 5�      �� *Alpha+$ TwoWay=�$$ PropertyChanged=�0   CheckerBackground0 5�      �� *CheckerBackground+$ OneWay=�1   CheckerForeground1 5�      �� *CheckerForeground+$ OneWay=�2   Color2 5�      �� *
NewColor+3   	Component3 5�      �� *SelectedComponent+$ OneWay=�4   SelectionLength4 5�      �� *SelectionLength+$ OneWay=�5   SelectionType5 5�      �� *SelectionType+$ OneWay=�6   Transparency6 5�      �� *Transparency+$ OneWay=�� 5�   
   $��1��5�      $	��256��5�      $��10,0,0,0q�5�      y�5�      �� *InputVisibility+$ OneWay=�.+�5�      �� $
��Auto=�6   .��5�      �� #��[ 5�      7 %� IsVirtualizing7 . 8 %� VirtualizationMode$8 	Recycling=�5�      $��Center=�5�      ��5�      �� *ColorModels+$ OneWay=���5�      �� 5�      .R�5�      � 9 � Orientation$9 
Horizontal=�6(   5�      5�      5�      5�      �� 5�   
   $��2��5�      $	��256��5�      $��Center=�5�      $��15,0,0,0q�5�      .+�5�        Imagin.Common.SplitView  $	��125��5�      $��Center=�5�      :   Orientation$: Vertical=�5�      $	��Top=�5�      $	��100��5�      ;   Content1; 5�        5�      $: Vertical=�6'   ; 5�      �� 5�      $��Center=�5�   !   $��0,0,0,5q�5�   !   ��5�   !   2  ,WPFLocalizeExtension.Extensions.LocExtension  *%#Imagin.Colour.Controls.WPF:Main:New+5�   !   <   Content2< 5�         Imagin.Common.CheckerBoard  5�      
���   �$��1q�5�   !   . 5�   !   �� *Data+= �� Source#= [  >   CheckerBackground> 5�   !   �� *CheckerBackground+$ OneWay=�?   CheckerForeground? 5�   !   �� *CheckerForeground+$ OneWay=�@ � ToolTip@ 5�   !   �� *
NewColor+A �� 	Converter#A [ B �� ConverterParameter$B 	WithAlpha��C �� StringFormat$
C #{0}��._�5�   "   �� ��6,   �� *
NewColor+#A [ 5�      5�      < 5�        5�      $: Vertical=�6'   ; 5�        5�      
���   �$��1q�5�   !   $��0,-1,0,0q�5�   !   . 5�   !   �� *Data+#= [  > 5�   !   �� *CheckerBackground+$ OneWay=�? 5�   !   �� *CheckerForeground+$ OneWay=�@ 5�   !   �� *
OldColor+#A [ $B 	WithAlpha��$
C #{0}��._�5�   "   �� -   5        D ��Name$D PART_OldRectangle��5�   ,   5�      < 5�      �� 5�      $��Center=�5�   !   $��0,5,0,0q�5�   !   ��5�   !     *)'Imagin.Colour.Controls.WPF:Main:Current+5�   !   5�      5�        Imagin.Common.HexBox  5�      $��0,10,0,0q�5�      $��Center=�5�      $��90��5�      .��6   ��5�      �� 5�      ",  6   E �� BasedOn#E [B 67   F �� TriggersF 5�      ^� 5�      G ^� 
ConditionsG 5   "   �� 5  &   H �� BindingH 60   �� *Transparency+ �� " 
 I �� ValueI Transparent6�   �� 5  &   H 60   �� *	IsAsync+ �� " 
 I True6�   .0�5  "   �� % � ��" 69   �� *
NewColor+$ TwoWay=�$$ 	LostFocus=�$ PART_ColorPicker��#A [ $B 	WithAlpha��# . 5     ^� 5     G 5  "   �� 5  &   H 60   �� *Transparency+ �� " 
 I Transparent6�   �� 5	  &   H 60   �� *	IsAsync+ �� " 
 	I False6�   .0�5  "   �� % � ��" 69   �� *
NewColor+$ TwoWay=�$$ 	LostFocus=�$ PART_ColorPicker��#A [ $B 	WithAlpha��5     ^� 5     G 5  "   �� 5  &   H 60   �� *Transparency+ �� " 
 
I Opaque6�   �� 5  &   H 60   �� *	IsAsync+ �� " 
 I True6�   .0�5  "   �� % � ��" 69   �� *
NewColor+$ TwoWay=�$$ 	LostFocus=�$ PART_ColorPicker��#A [ # . 5     ^� 5     G 5  "   �� 5  &   H 60   �� *Transparency+ �� " 
 
I Opaque6�   �� 5  &   H 60   �� *	IsAsync+ �� " 
 	I False6�   .0�5  "   �� % � ��" 69   �� *
NewColor+$ TwoWay=�$$ 	LostFocus=�$ PART_ColorPicker��#A [ 5     �� 5       6*   �� *Transparency+ �� " 
  Transparent6�   .a�5  "   �� % �@��	MaxLength$" 8��6>   5     �� 5       6*   �� *Transparency+ �� " 
 
 Opaque6�   .a�5  "   �� % �@��	MaxLength$" 6��6>   5      5"     .��5#  '   5$     5%     5&     5'     