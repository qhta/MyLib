   M S B A M L   `   `   `  ����L  HImagin.Common.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:Imagin.CommonImagin.Common  5      H DImagin.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullD2clr-namespace:Imagin.Common;assembly=Imagin.CommonImagin.Common 5      C&clr-namespace:Imagin.Common.ConvertersImagin.Common.Converters  5	      7 clr-namespace:Imagin.Common.DataImagin.Common.Data  5
      7 clr-namespace:Imagin.Common.LinqImagin.Common.Linq  5      ;"clr-namespace:Imagin.Common.MarkupImagin.Common.Markup  5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5      M ISystem, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089/$clr-namespace:System;assembly=SystemSystem 5      I2clr-namespace:System.Collections;assembly=mscorlibSystem.Collections 5      M3clr-namespace:System.ComponentModel;assembly=SystemSystem.ComponentModel 5      7(clr-namespace:System.Net;assembly=System
System.Net 5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D1clr-namespace:System.Windows;assembly=WindowsBaseSystem.Windows 5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U<clr-namespace:System.Windows.Media;assembly=PresentationCoreSystem.Windows.Media 5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35O KWPFLocalizeExtension, Version=3.3.1.0, Culture=neutral, PublicKeyToken=nullR NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source5    /Imagin.Common.Converters.NullToBooleanConverter   NullToBooleanConverter4   .Imagin.Common.Converters.ObjectToTypeConverter  ObjectToTypeConverter9   3Imagin.Common.Converters.StringToCamelCaseConverter  StringToCamelCaseConverter$   Imagin.Common.PropertyGridBase �� 
TargetType&   System.Windows.Controls.DataGrid �� BasedOn �� Property �� Value  Brush.Border  
Brush.Text - )Brush.PropertyGrid.Description.Background ) %Brush.PropertyGrid.Description.Border �� Padding � RowDefinitions&    Imagin.Common.LocalizedTextBlock   TextTrimming	   Text
 �� 	Converter �� Mode � ToolTip �� RelativeSource   TextWrapping)  #System.Windows.Controls.DataGridRow �� Triggers ]� 
Conditions �� Property �� Value �� AncestorType   Imagin.Common.MaskedImage   Source ! Brush.GridSplitter.Background �� 
TargetType � 	Resources*  $System.Windows.Controls.DataGridCell ��Name � 	Resources%	   Imagin.Common.Data.BindingProxy  Key 	  Data;
   5Imagin.Common.Converters.BooleanToVisibilityConverter6   0Imagin.Common.Converters.InverseBooleanConverter4   .Imagin.Common.Converters.LocalizationConverter8   2Imagin.Common.Converters.NullToVisibilityConverterH   BImagin.Common.Converters.StartsWithToVisibilityMultiValueConverterF   @Imagin.Common.Converters.ValueEqualsParameterMultiValueConverter,   &Imagin.Common.PropertyTemplateSelector   	Resources2   ,Imagin.Common.Converters.ColorToHexConverter<   6Imagin.Common.Converters.DisplayNameAttributeConverter:   4Imagin.Common.Converters.FileSizeMultiValueConverter4   .Imagin.Common.Converters.RelativeTimeConverter<   6Imagin.Common.Converters.SolidColorBrushToHexConverter d� 	IsChecked �� UpdateSourceTrigger 	 InverseBooleanConverter   Imagin.Common.ByteUpDown �� 
IsReadOnly   Imagin.Common.UpDown   	CanUpDownx   rImagin.Common.UpDown`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   Maximum    Minimum   yImagin.Common.NumericUpDown`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]!   StringFormat"   Value  System.Collections.IList# ��Name"   Imagin.Common.DateTimeUpDown|   vImagin.Common.UpDown`1[[System.DateTime, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]$   Maximum%   Minimum&   Value' �� Triggers( ^� 
Conditions) �� Binding* �� 
TargetName 
 RelativeTimeConverter!   Imagin.Common.DecimalUpDown{   uImagin.Common.UpDown`1[[System.Decimal, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]+   Maximum,   Minimum�   |Imagin.Common.NumericUpDown`1[[System.Decimal, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]-   StringFormat.   Value     Imagin.Common.DoubleUpDownz!   tImagin.Common.UpDown`1[[System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]/ !  Maximum0 !  Minimum�"   {Imagin.Common.NumericUpDown`1[[System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]1 "  StringFormat2 !  Value#  System.Enum3 %� IsVirtualizing4 %� VirtualizationMode)$   #Imagin.Common.Data.EnumerateBinding5 �� SelectedValue ! DisplayNameAttributeConverter6 �� Value7 �� Binding!%   Imagin.Common.FlagCheckView8 %  Flags9 %  
ItemsPanel: %  ItemTemplate&   Imagin.Common.Int16UpDowny'   sImagin.Common.UpDown`1[[System.Int16, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]; '  Maximum< '  Minimum�(   zImagin.Common.NumericUpDown`1[[System.Int16, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]= (  StringFormat> '  Value)   Imagin.Common.Int32UpDowny*   sImagin.Common.UpDown`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]? *  Maximum@ *  Minimum�+   zImagin.Common.NumericUpDown`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]A +  StringFormatB *  Value,   Imagin.Common.Int64UpDowny-   sImagin.Common.UpDown`1[[System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]C -  MaximumD -  Minimum�.   zImagin.Common.NumericUpDown`1[[System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]E .  StringFormatF -  Value  FileSizeMultiValueConverterG `� 	ConverterH �� Path  BindingProxyI �� Source/   Imagin.Common.RegexBoxJ /  Pattern"0  System.Net.NetworkCredential1   Imagin.Common.TextBoxK 1  Placeholder22  ,WPFLocalizeExtension.Extensions.LocExtension L 1  IsCharacterMaskingEnabledM 1  ShowToggleButtonN �� ContentStringFormatO � ColumnDefinitionsP   Name&3    Imagin.Common.MaskedToggleButtonQ 3  SourceR 3  CheckedToolTip4   Imagin.Common.FloatUpDownz5   tImagin.Common.UpDown`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]S 5  MaximumT 5  Minimum�6   {Imagin.Common.NumericUpDown`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]U 6  StringFormatV 5  ValueW 1  IsClearEnabled7   Imagin.Common.FileBoxX 7  
BrowseModeY 7  	CanBrowseZ �� AcceptsReturn[ �� LineStackingStrategy\ �� 
LineHeight] �� TextWrapping8   Imagin.Common.TokenView^ 8  TokensSource_ 8  
TokenStyle 9   Imagin.Common.UInt16UpDownz:   tImagin.Common.UpDown`1[[System.UInt16, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]` :  Maximuma :  Minimum�;   {Imagin.Common.NumericUpDown`1[[System.UInt16, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]b ;  StringFormatc :  Value <   Imagin.Common.UInt32UpDownz=   tImagin.Common.UpDown`1[[System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]d =  Maximume =  Minimum�>   {Imagin.Common.NumericUpDown`1[[System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]f >  StringFormatg =  Value ?   Imagin.Common.UInt64UpDownz@   tImagin.Common.UpDown`1[[System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]h @  Maximumi @  Minimum�A   {Imagin.Common.NumericUpDown`1[[System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]j A  StringFormatk @  ValueB   Imagin.Common.UriBoxwC   qImagin.Common.ParseBox`1[[System.Uri, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]l C  ValueD  System.VersionE   Imagin.Common.VersionBox}F   wImagin.Common.ParseBox`1[[System.Version, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]m F  Value(G  "System.Windows.Controls.GroupStylen G  ContainerStyleo 9� 
IsExpanded   BooleanToVisibilityConverter  
Brush.Linkp �� StringFormatH   Imagin.Common.Lineq H  Orientationr G  Panel>I  8System.Windows.Controls.Primitives.DataGridRowsPresenters � DataContext  Style.Border.Bottom(J   "Imagin.Common.Linq.PanelExtensionst J  Spacingu J  TrimLast K   Imagin.Common.MaskedButtonv K  Source(L   "Imagin.Common.MaskedDropDownButtonw 3  DropDown-M  'System.Windows.Data.CompositeCollectionx �� 
Collection-N   'Imagin.Common.Markup.EnumerateExtension)O   #Imagin.Common.PropertyGridGroupName + 'ValueEqualsParameterMultiValueConvertery `� Mode(P   "Imagin.Common.PropertyGridSortName-Q  'System.ComponentModel.ListSortDirectionz �� ConverterParameter  NullToVisibilityConverter  DataTemplate.Property.Value{ ��Name| K� ZIndex  Style.DataGrid.ScrollViewer} J� SnapsToDevicePixels~ ��Name �� ResizeDirection� �� Triggers R   Imagin.Common.PropertyGrid,S   &Imagin.Common.ResourceDictionaryEditor�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    0l(http://wpflocalizeextension.codeplex.com  5      Bmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      :x,http://schemas.microsoft.com/winfx/2006/xaml     5      %localclr-namespace:Imagin.Common  5      -Common http://imagin.tech/imagin/common   5      FCommon.Portable2clr-namespace:Imagin.Common;assembly=Imagin.Common  5      5
Converters&clr-namespace:Imagin.Common.Converters  5	      )Data clr-namespace:Imagin.Common.Data  5
      )Linq clr-namespace:Imagin.Common.Linq  5      -Markup"clr-namespace:Imagin.Common.Markup  5      1System&clr-namespace:System;assembly=mscorlib  5      1System.2$clr-namespace:System;assembly=System  5      ISystem.Collections2clr-namespace:System.Collections;assembly=mscorlib  5      MSystem.ComponentModel3clr-namespace:System.ComponentModel;assembly=System  5      7
System.Net(clr-namespace:System.Net;assembly=System  5      DSystem.Windows1clr-namespace:System.Windows;assembly=WindowsBase  5      USystem.Windows.Media<clr-namespace:System.Windows.Media;assembly=PresentationCore  5        5      �� 5   
   $U Opack://application:,,,/Imagin.Common.WPF;component/Styles/Generic.DataGrid.xaml?�6   �� 5   
   $Q Kpack://application:,,,/Imagin.Common.WPF;component/Styles/Generic.Menu.xaml?�6   %�_  &	        &	      &	 &     '  9     7 7  7 7 	 7 	 7 
 7 	 7 	 7 	 7  7 	 7  7  7  7  7 	 7 	 7 	 7  7  7 	 7  7  7  7 	 7 	 7 	 7 	 7 	 7 	 7 	 7  7 	 7 	 7 	 7  7  7 	 7  7  7  7   7  7m�7  7  7  7  7m�7  7  7  7  7  7  7  7  7  7  7  7  7   7   7  'R  ^    7 'S  8^    7    5      6(     5      6'     5      6,   �� 5      "  64   8   .�5   
   ��  �@ AutoGenerateColumns .  60   �� 5   
    � ��
 ���� 6'   �� 5    
    � ��# �  6(   �� 5!   
    � ��$ 1q�6,   �� 5"   
    �@ CanUserAddRows .  6+   �� 5#   
    �@ CanUserDeleteRows .  6.   �� 5$   
    �@ CanUserReorderColumns .  62   �� 5%   
    �@ CanUserResizeColumns . 61   �� 5&   
    �@ CanUserResizeRows .  6.   �� 5'   
   " �@ EnableColumnVirtualization . 67   �� 5(   
    �@ EnableRowVirtualization . 64   �� 5)   
    � ��# �  6'   �� 5*   
    �@ DescriptionTemplate 5+      �� 5,      .R�5-      �� #���  5.      #���  5/      $��0,1,0,0q�50      $��36��51      #���  52      $ 10q�53      ._�54      �  55      �� 56   "   $
��Auto �60   �� 57   "   $
��Auto �60   .+�59        $
��Bold"�5:   !   $��0,0,0,7q�5;   !   $ CharacterEllipsis=�5=   !   	 5<   !   �� *DisplayName+8
  $ OneWay=� 5>   !   �� *LocalizedText+$ OneWay=� �� *Self+  5?      $��1��5@   !   $ CharacterEllipsis=�5B   !   $
 Wrap=�5C   !   	 5A   !   �� *Description+$ OneWay=� 5D   !   �� *LocalizedText+$ OneWay=� �� *Self+5E      5F      5G      5I      �� 5J   
    � �� 5K      �� 5L      "  6    5W      ]� 5X       5Y      �� 5Z   "    �@ 
IsSelected .  6B   �� 5[   "    �@J�IsMouseOver . 6C   ./�5]      ��  � ��
 ���� 6;   5^      ]� 5_       5`      �� 5a   "    �@ 
IsSelected . 6B   �� 5b   "    �@J�IsMouseOver . 6C   ./�5d      ��  � ��
 ���� 6;   5e      ]� 5f       5g      �� 5h   "    �@ 
IsSelected . 6B   �� 5i   "    �@��IsSelectionActive .  6R   �� 5j   "    �@J�IsMouseOver .  6C   ./�5l      ��  � ��
 ���� 6;   5m      ]� 5n       5o      �� 5p   "    �@ 
IsSelected . 6B   �� 5q   "    �@��IsSelectionActive . 6R   �� 5r   "    �@J�IsMouseOver .  6C   ./�5t      ��  � ��
 ���� 6;   5u      ]� 5v       5w      �� 5x   "    �@ 
IsSelected .  6B   �� 5y   "    �@��AlternationIndex$ 0��6U   �� 5z   "    �@J�IsMouseOver .  6C   ./�5|      ��  � ��
 ���� 6;   5}      ]� 5~       5      �� 5�   "    �@ 
IsSelected .  6B   �� 5�   "    �@��AlternationIndex$ 1��6U   �� 5�   "    �@J�IsMouseOver .  6C   ./�5�      ��  � ��
 ���� 6;   5�      5�      5�      �� 5�   
    � ��$ 0q�6#   �� 5�   
    �@ NestedPropertyTemplate 5�      �� 5�      .R�5�      �� ��5�      �� *NestCommand+ �� "  ��5�      �� *Value+$ OneWay=�.��5�        $H Bpack://application:,,,/Imagin.Common.WPF;component/Images/Dots.png��5�      5�      5�      5�      �� 5�   
    � ��$ 0q�6$   �� 5�   
    � ��$
 Auto=�6G   �� 5�   
    � ��$
 Auto=�6E   �� 5�   
    �@ SelectionMode$ Single=�6*   �� 5�   
    �@ SplitterStyle 5�      �� 5�      " ��6   .�5�      ��  � ��# �  63   �� 5�       � ��$ 3��6/   �� 5�       � ��$ Stretch=�6<   �� 5�       �@��ResizeBehavior$ PreviousAndNext=�67   �� 5�       � ��$	 Top=�6:   �� 5�       � ��$
 Auto��6.   5�      5�      �� 5�   
    � �� 5�      �� 5�      "  6"    5�      ���� 5�      '        "  6    8  .�5�      ��  � ��$
 10,1q�68   5�      5�      .R�5�      �� $ PART_Border��5�      #��z��5�      #��z��5�      #��z��5�       5�      ��	  5�      BindingProxy  6F   ��  �� "  
  5�       BooleanToVisibilityConverter 6F     5�      InverseBooleanConverter 6A     5�      LocalizationConverter 6?      5�      NullToBooleanConverter 6@     5�      NullToVisibilityConverter 6C     5�      -)StartsWithToVisibilityMultiValueConverter 6S     5�      +'ValueEqualsParameterMultiValueConverter 6Q   �� 5�      DataTemplate.Property.Name .R�5�   "     $ CharacterEllipsis=�5�   !   $��Center=�5�   !   	 5�   !   �� *DisplayName+$ OneWay=� 5�   !   �� *LocalizedText+$ OneWay=� �� *Self+5�   !   5�      �� 5�      DataTemplate.Property.Value .R�5�   "   �� ��61   �� $ OneWay=���5�   &     5�   *    5�   .   �� 5�   2     5�   6    BooleanToVisibilityConverter 6U     5�   6   !DisplayNameAttributeConverter 6_     5�   6   InverseBooleanConverter 6Y     5�   6   FileSizeMultiValueConverter 6]     5�   6   RelativeTimeConverter 6W     5�   6   !SolidColorBrushToHexConverter 6_   �� 5�   6   '��       .R�5�   :   �� $
��Left=�5�   =   $	��0,5q�5�   =    5�   =   �� *Value+$ TwoWay=�$ PropertyChanged=�{�5�   =   �� *
IsReadOnly+8
  $ OneWay=�5�   =   5�   7   �� 5�   6   '��       .R�5�   :     $��Stretch=�5�   =    5�   =   �� *
IsReadOnly+$ OneWay=� 5�   =   �� *
IsReadOnly+8
  $ OneWay=� 5�   =   �� *	Maximum+  5�   =   �� *	Minimum+! 5�   =   �� *StringFormat+" 5�   =   �� *Value+$ TwoWay=�$ PropertyChanged=�5�   7   �� 5�   6   '��       5  7   �� 5  6   '        .R�5  :   � 5  7   �� 5  6   '��       .R�5  :   �� $# PART_Content��5  =   ��5	  =   �� $ OneWay=���5
  >   �� 5  B   .R�5  F     $��Stretch=�5  I   $ 5  I   �� *	Maximum+% 5  I   �� *	Minimum+& 5  I   �� *Value+$ TwoWay=�$ PropertyChanged=�5  C   5  ;   ' 5  :   ^� 5  >   ( 5  B   �� 5  F   ) 6P   �� *
IsReadOnly+ True6o   �� 5  F   ) 6P   �� *Format+ Default6k   .0�5  B   �� $* PART_Content��6I    � �� 5  F   �� 5  J   .R�5  N   �� $	��0,5q�5  Q   ��5  Q   �� *Value+$ OneWay=� 5   Q   �� *Text+$ OneWay=� �� *Self+5!  K   5#  C   5$  ?   ^� 5%  >   ( 5&  B   �� 5'  F   ) 6P   �� *
IsReadOnly+ True6o   �� 5(  F   ) 6P   �� *Format+ Relative6k   .0�5*  B   �� $* PART_Content��6I    � �� 5+  F   �� 5,  J   .R�5-  N   �� $	��0,5q�5.  Q   $��CharacterEllipsis=�50  Q   ��5/  Q   �� *Value+8
  $ OneWay=� 51  Q   �� *Text+$ OneWay=� �� *Self+52  K   54  C   55  ?   57  7   �� 58  6   '��       .R�59  :     $��Stretch=�5:  =    5;  =   �� *
IsReadOnly+$ OneWay=� 5<  =   �� *
IsReadOnly+8
  $ OneWay=�+ 5=  =   �� *	Maximum+, 5>  =   �� *	Minimum+- 5?  =   �� *StringFormat+. 5@  =   �� *Value+$ TwoWay=�$ PropertyChanged=�5A  7   �� 5B  6   'X�       .R�5C  :      $��Stretch=�5D  =    5E  =   �� *
IsReadOnly+$ OneWay=� 5F  =   �� *
IsReadOnly+8
  $ OneWay=�/ 5G  =   �� *	Maximum+0 5H  =   �� *	Minimum+1 5I  =   �� *StringFormat+2 5J  =   �� *Value+$ TwoWay=�$ PropertyChanged=�5K  7   �� 5L  6   '#        .R�5M  :   �� $# PART_Content��6I   .��5N  >   �� 3 . $4 	Recycling=�5S  A   {�5O  A   �� *
IsReadOnly+8
  $ OneWay=���5P  A   $  *Value+5 5Q  A   �� *Value+$ TwoWay=�$ PropertyChanged=���5T  B   �� 5U  F   .R�5V  J     $ CharacterEllipsis=�5X  M   	 5W  M   �� 8
 	 $ OneWay=�5X  M   5Y  G   5[  ?   5\  ;   ' 5]  :   �� 5^  >   $6 Flags��6e   7 6J   �� *Format+.a�5_  B   �� $* PART_Content��6I    � �� 5`  F   %  5a  J   $��0,10,0,0q�5d  M   8 5b  M   �� *Value+$ TwoWay=�$ PropertyChanged=�{�5c  M   �� *
IsReadOnly+8
 
 $ OneWay=�9 5e  N   �� 5f  R   .R�5g  V   �� 5h  S   : 5j  N   �� 5k  R   .R�5l  V   �� $��0,0,0,10q�5n  Y    5m  Y   �� *	IsChecked+$ TwoWay=�$ PropertyChanged=���5o  Z     5p  ^   $ CharacterEllipsis=�5r  a   	 5q  a   �� *Value+8
  $ OneWay=�5r  a    5t  Z     5u  ^   	 5v  a   �� *Value+8
  $ OneWay=�5x  W   5y  S   5{  K   5}  C   5~  ?   ^� 5  >   ( 5�  B   �� 5�  F   ) 6P   �� *
IsReadOnly+ True6o   �� 5�  F   ) 6P   �� *Format+ Default6k   .0�5�  B   �� $* PART_Content��6I    � �� 5�  F     5�  J   $	��0,5q�5�  M   $ CharacterEllipsis=�5�  M   	 5�  M   �� *Value+8
  $ OneWay=� 5�  N     5�  R   	 6k   �� *Value+8
  $ OneWay=�5�  K   5�  C   5�  ?   5�  7   �� 5�  6   '��       .R�5�  :   &  $��Stretch=�5�  =    5�  =   �� *
IsReadOnly+$ OneWay=� 5�  =   �� *
IsReadOnly+8
  $ OneWay=�; 5�  =   �� *	Maximum+< 5�  =   �� *	Minimum+= 5�  =   �� *StringFormat+> 5�  =   �� *Value+$ TwoWay=�$ PropertyChanged=�5�  7   �� 5�  6   '��       .R�5�  :   )  $��Stretch=�5�  =    5�  =   �� *
IsReadOnly+$ OneWay=� 5�  =   �� *
IsReadOnly+8
  $ OneWay=�? 5�  =   �� *	Maximum+@ 5�  =   �� *	Minimum+A 5�  =   �� *StringFormat+B 5�  =   �� *Value+$ TwoWay=�$ PropertyChanged=�5�  7   �� 5�  6   '��       .R�5�  :   �� $# PART_Content��6I   .��5�  >   ,  $��Stretch=�5�  A    5�  A   �� *
IsReadOnly+$ OneWay=� 5�  A   �� *
IsReadOnly+8
  $ OneWay=�C 5�  A   �� *	Maximum+D 5�  A   �� *	Minimum+E 5�  A   �� *StringFormat+F 5�  A   �� *Value+$ TwoWay=�$ PropertyChanged=�5�  ;   ' 5�  :   �� 5�  >   $6 FileSize��6e   7 6J   �� *Format+.a�5�  B   �� $* PART_Content��6I    � �� 5�  F   �� 5�  J   $
��0,10q�6T   ��5�  N   `� 5�  R   8G  .1�5�  V   �� $H Value�6^   $ OneWay=�6k   �� 5�  V   $H Data.FileSizeFormat�6^   8I  6y   5�  S   5�  K   5�  C   5�  ?   5�  7   �� 5�  6   '��       5�  7   �� 5�  6   '��       .R�5�  :   /  $��Stretch=�5�  =   $]J W^([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$��5�  =    5�  =   �� *
IsReadOnly+$ OneWay=���5�  =   �� *Value+$ TwoWay=�$ PropertyChanged=�5�  7   �� 5�  6   '0        .R�5�  :   �� {�6E   �� *
IsReadOnly+8
  $ OneWay=�.+�5�  >   1  $��0,0,0,5q�5�  A   K 5�  A   2  *
Username+��5�  A   �� *Value.UserName+$ TwoWay=�$ PropertyChanged=�1  5�  >   L . M . K 5�  A   2  *
Password+��5�  A   �� *Value.Password+$ TwoWay=�$ PropertyChanged=�5�  ;   5�  7   �� 5�  6   'V�       .R�5�  :   �� ��5�  =   �� $ OneWay=�N 5�  =   �� *!Data.NestedPropertyStringFormat+8I  ��5�  =   �� *Data.NestedPropertyTemplate+8I  ��5�  =   �� *%#Data.NestedPropertyTemplateSelector+8I  5�  7   �� 5�  6   '8�       .R�5�  :   � O 5�  >   �� 5�  B   $��* �6S   �� 5�  B   $��* �6S   .+�5�  >      $��0,0,5,0q�5�  A    5�  A   �� *
IsReadOnly+8
  $ OneWay=� 5�  A   �� *
IsReadOnly+$ OneWay=�0 5�  A   �� *	Minimum.X+1 5�  A   �� *StringFormat+ 5�  A   2  *X+2 5�  A   �� *	Variant.X+$ TwoWay=�$ PropertyChanged=�   5�  >   $��1��5�  A   $��1��5�  A    5�  A   �� *
IsReadOnly+8
  $ OneWay=� 5�  A   �� *
IsReadOnly+$ OneWay=�0 5�  A   �� *	Minimum.Y+1 5�  A   �� *StringFormat+ 5�  A   2  *Y+2 5�  A   �� *	Variant.Y+$ TwoWay=�$ PropertyChanged=�5�  ;   5�  7   �� 5   6   '
�       5  7   �� 5  6   '��       .R�5	  :   � O 5
  >   �� 5  B   $��* �6S   �� 5  B   $
��Auto �6S   �� 5  B   $��* �6S   .+�5  >      $P 
PART_Width��5  E   $0 0Q�5  E    5  E   �� *
IsReadOnly+8
  $ OneWay=� 5  E   �� *
IsReadOnly+$ OneWay=�1 5  E   �� *StringFormat+ 5  E   2  *Width+2 5  E   �� *Variant.Width+$ TwoWay=�$ PropertyChanged=�3  5  >   $��1��5  E   $	��7,0q�5  E   $RQ Lpack://application:,,,/Imagin.Common.WPF;component/Images/LinkHorizontal.png��5  E   R 5  E   2  *Imagin.Common:Main:Unbind+ 5  E   2  *Imagin.Common:Main:Bind+   5  >   $P PART_Height��5   E   $��2��5  E   $0 0Q�5#  E    5!  E   �� *
IsReadOnly+8
  $ OneWay=� 5"  E   �� *
IsReadOnly+$ OneWay=�1 5$  E   �� *StringFormat+ 5%  E   2  *Height+2 5&  E   �� *Variant.Height+$ TwoWay=�$ PropertyChanged=�5'  ;   5(  7   �� 5)  6   '��       5?  7   �� 5@  6   '��       .R�5A  :   4  $��Stretch=�5B  A    5C  A   �� *
IsReadOnly+$ OneWay=� 5D  A   �� *
IsReadOnly+8
  $ OneWay=�S 5E  A   �� *	Maximum+T 5F  A   �� *	Minimum+U 5G  A   �� *StringFormat+V 5H  A   �� *Value+$ TwoWay=�$ PropertyChanged=�5I  7   �� 5J  6   '��       .R�5K  :   �� $# PART_Content��5L  A   ��5M  A   �� $ OneWay=���5N  >   �� 5O  B   .R�5P  F   1  $��Stretch=�5Q  M   W .  5S  M   �� *
IsReadOnly+$ OneWay=���5T  M   �� *Value+$ TwoWay=�$ PropertyChanged=�5U  C   5W  ;   ' 5X  :   �� 5Y  >   $6 FilePath��6�   7 6J   �� *Format+$ PropertyChanged=�.a�5Z  B   �� $* PART_Content��6I    � �� 5[  F   �� 5\  J   .R�5]  N   7  $X OpenFile=�5^  U   $��Stretch=�5`  U   W . Y 5_  U   �� *
IsReadOnly+8
  $ OneWay=� 5b  U   �� *
IsReadOnly+$ OneWay=���5c  U   �� *Value+$ TwoWay=�$ PropertyChanged=�5d  K   5f  C   5g  ?   �� 5h  >   $6 
FolderPath��6�   7 6J   �� *Format+$ PropertyChanged=�.a�5i  B   �� $* PART_Content��6I    � �� 5j  F   �� 5k  J   .R�5l  N   7  $X 
OpenFolder=�5m  U   $��Stretch=�5o  U   W . Y 5n  U   �� *
IsReadOnly+8
  $ OneWay=� 5q  U   �� *
IsReadOnly+$ OneWay=���5r  U   �� *Value+$ TwoWay=�$ PropertyChanged=�5s  K   5u  C   5v  ?   �� 5w  >   $6 	Multiline��6�   7 6J   �� *Format+$ PropertyChanged=�.a�5x  B   �� $* PART_Content��6I    � �� 5y  F   �� 5z  J   .R�5{  N   1  Z . $
��Auto��5}  U   $��Stretch=�5~  U   $	��5,7q�5�  U   $[ BlockLineHeight=�5�  U   $\ 15��5�  U   $	��Top=�5�  U   $
] Wrap=�5�  U    5  U   �� *
IsReadOnly+$ OneWay=���5�  U   �� *Value+$ TwoWay=�$ PropertyChanged=�5�  U   5�  K   5�  C   5�  ?   �� 5�  >   $6 Password��6�   7 6J   �� *Format+$ PropertyChanged=�.a�5�  B   �� $* PART_Content��6I    � �� 5�  F   �� 5�  J   .R�5�  N   1  L . W . M . $��Stretch=�5�  U    5�  U   �� *
IsReadOnly+$ OneWay=���5�  U   �� *Value+$ TwoWay=�$ PropertyChanged=�5�  U   5�  K   5�  C   5�  ?   �� 5�  >   $6 Tokens��6�   7 6J   �� *Format+$ PropertyChanged=�.a�5�  B   �� $* PART_Content��6I    � �� 5�  F   �� 5�  J   .R�5�  N   8  $��Stretch=�5�  U    5�  U   �� *
IsReadOnly+$ OneWay=�^ 5�  U   �� *Value+$ TwoWay=�$ PropertyChanged=�_ 5�  U   �� *Data.TokenStyle+8I  .^ 6�   5�  O   5�  K   5�  C   5�  ?   5�  7   �� 5�  6   'I�       .R�5�  :   9  $��Stretch=�5�  A    5�  A   �� *
IsReadOnly+$ OneWay=� 5�  A   �� *
IsReadOnly+8
   $ OneWay=�` 5�  A   �� *	Maximum+a 5�  A   �� *	Minimum+b 5�  A   �� *StringFormat+c 5�  A   �� *Value+$ TwoWay=�$ PropertyChanged=�5�  7   �� 5�  6   'G�       .R�5�  :   <  $��Stretch=�5�  A    5�  A   �� *
IsReadOnly+$ OneWay=� 5�  A   �� *
IsReadOnly+8
 ! $ OneWay=�d 5�  A   �� *	Maximum+e 5�  A   �� *	Minimum+f 5�  A   �� *StringFormat+g 5�  A   �� *Value+$ TwoWay=�$ PropertyChanged=�5�  7   �� 5�  6   'E�       .R�5�  :   �� $# PART_Content��6I   .��5�  >   ?  $��Stretch=�5�  E    5�  E   �� *
IsReadOnly+$ OneWay=� 5�  E   �� *
IsReadOnly+8
 " $ OneWay=�h 5�  E   �� *	Maximum+i 5�  E   �� *	Minimum+j 5�  E   �� *StringFormat+k 5�  E   �� *Value+$ TwoWay=�$ PropertyChanged=�5�  ;   ' 5�  :   �� 5�  >   $6 FileSize��6f   7 6J   �� *Format+.a�5�  B   �� $* PART_Content��6I    � �� 5�  F   �� 5�  J   $
��0,10q�6T   ��5�  N   `� 5�  R   8G # .1�5�  V   �� $H Value�6^   $ OneWay=�6k   �� 5�  V   $H Data.FileSizeFormat�6^   8I $ 6y   5�  S   5�  K   5�  C   5�  ?   5�  7   �� 5�  6   '@�       .R�5�  :   B  $��Stretch=�5�  A    5�  A   �� *
IsReadOnly+$ OneWay=�l 5�  A   �� *Value+$ TwoWay=�$ PropertyChanged=�5�  7   �� 5�  6   'D        .R�5�  :   E  $��Stretch=�5�  A   {�5�  A   �� *
IsReadOnly+8
 % $ OneWay=�m 5�  A   �� *Value+$ TwoWay=�$ PropertyChanged=�5�  7   5�  3   5�  +   5�  #   5�     G  5�     Style.Group n 5�  "   �� 5�  &   " ��6,   .�5�  *   ��  � �� 5�  .   �� 5�  2   " ��6B   .R�5�  6   9� $��Stretch=�5�  9   o 5�  9   �� *Data.GroupVisibility+8
 & $ OneWay=�8I ' ��5�  :   � 5�  >   O 5�  B   �� 5�  F   $
��Auto �6W   �� 5�  F   $��* �6W   .+�5�  B   �� $��15(�6L   �� 5�  >   #���  5�  A   �5�  B   �� *Name+$ OneWay=�5�  A    �� 5�  >   �6B   �� *	ItemCount+$ OneWay=�$p ({0})��5�  C   H  5   B   $��1��5  =   $��5,0,0,0q�5  =   $q 
Horizontal=�5  =   5  ?   .��5  :   �� 5  7   5  3   5
  +   5  '   r 5  "   �� 5  &   .R�5  *   I  5  '   5     5     ._�5     � s 6   �� *Data+8I (  5     �� 5  "   $��30 �60   �� 5  "   $��* �60   �� 5  "   $
��Auto �60   �� 5  "   $
��Auto �60   .+�5     �� #���  5  !   y�5  !   �� *HeaderVisibility+$ OneWay=�._�5   "   � $��0,0,5,0q�5"  %   $t 0,0,10,0q�5#  %   u . {�5!  %   �� *Source+8
 ) O 5%  &   �� 5&  *   $
��Auto �6;   �� 5'  *   $
��Auto �6;   �� 5(  *   $
��Auto �6;   �� 5)  *   $
��Auto �6;   �� 5*  *   $
��Auto �6;   �� 5+  *   $��* �6;   �� 5,  *   $
��Auto �6;   .+�50  &   �� $��Center=�52  )   ��51  )   �� *HeaderButtons+��53  *   �� 54  .   .R�55  2   �� ��6C   �� $ OneWay=�56  /   58  '   K  5<  &   $��1��5=  )   $Rv Lpack://application:,,,/Imagin.Common.WPF;component/Images/ArrowLeftRound.png��5?  )   $��Center=�5A  )   ��5>  )   �� *RewindNestCommand+ 5@  )   2  *Imagin.Common.WPF:Main:Back+y�5B  )   �� *	IsEnabled+8
 * $ OneWay=� �� *Self+$ PropertyChanged=�L  5C  &   $��2��5D  )   $LQ Fpack://application:,,,/Imagin.Common.WPF;component/Images/Category.png��5E  )   $��Center=�5G  )    5F  )   2  *Imagin.Common.WPF:Main:Group+w 5H  *   �� 5I  .   s 6:   �� *PlacementTarget.DataContext+ �� *Self+��5J  2   M  5K  6   �� 5L  :   x 6N   N  *local:PropertyGridGroupName+5M  7    5O  2   ���� 5P  6   'm�       " m�6<   8 + .�5Q  :   ��  �@m�Command 6T   �� *Data.GroupCommand+8I , �� 5R  :    �@m�CommandParameter 6]   �� $ OneWay=��� 5S  :    �@m�StaysOpenOnClick . 6]    5T  :   �� 5U  >   "6 O 6�   7 6J   �� 8
 - .a�5V  B   ��  �@m�	IsChecked 5W  F   `� 5X  J   8G . $y OneWay=�6�   .1�5Y  N   �� $H Data.GroupName�6V   8I / 6l   �� 5Z  N   $H .�6V   5[  K   5]  C   5^  ?   5`  7   5a  3   5b  /   5d  '   L  5e  &   $��3��5f  )   $HQ Bpack://application:,,,/Imagin.Common.WPF;component/Images/Sort.png��5g  )   $��Center=�5i  )    5h  )   2  *Imagin.Common.WPF:Main:Sort+w 5j  *   �� 5k  .   s 6:   �� *PlacementTarget.DataContext+ �� *Self+��5l  2   M  5m  6   �� 5n  :   x 6N   N  *local:PropertyGridSortName+�� 5o  :   �� 5p  :   x 6N   N  *)'System.ComponentModel:ListSortDirection+5q  7    5s  2   ���� 5t  6   'm�       " m�6<   8 0 .�5u  :   ��  �@m�Command 6T   �� *Data.SortCommand+8I 1 �� 5v  :    �@m�CommandParameter 6]   �� $ OneWay=��� 5w  :    �@m�StaysOpenOnClick . 6]    5x  :   �� 5y  >   "6 P 6�   7 6J   �� 8
 2 .a�5z  B   ��  �@m�	IsChecked 5{  F   `� 5|  J   8G 3 $y OneWay=�6�   .1�5}  N   �� $H Data.SortName�6V   8I 4 6k   �� 5~  N   $H .�6V   5  K   5�  C   5�  ?   �� 5�  >   "6 Q 6�   7 6J   �� 8
 5 .a�5�  B   ��  �@m�	IsChecked 5�  F   `� 5�  J   8G 6 $y OneWay=�6�   .1�5�  N   �� $H Data.SortDirection�6V   8I 7 6p   �� 5�  N   $H .�6V   5�  K   5�  C   5�  ?   5�  7   5�  3   5�  /   5�  '     5�  &   $��4��5�  )   $J Dpack://application:,,,/Imagin.Common.WPF;component/Images/Search.png��5�  )   1  5�  &   $��5��5�  )   
������ $��0q�5�  )   W . $��0q�5�  )   K 5�  )   2  *Imagin.Common.WPF:Main:Search+��5�  )   �� *SearchQuery+$ TwoWay=�$ PropertyChanged=� 5�  )   2  *Imagin.Common.WPF:Main:Search+5�  #   5�     � 5�     $��1��5�  !   .+�5�  "   �� $��Center=�5�  %   $��Center=�5�  %   ��5�  %   �� *LoaderTemplate+y�5�  %   �� *	IsLoading+8
 8 � 5�  "   y�5�  %   �� *	IsLoading+8
 9 $z Inverted�� 5�  &   �� 5�  *   $
��Auto �68   �� 5�  *   $
��Auto �68   �� 5�  *   $��* �68   .+�5�  &   �� $# 	PART_Type��5�  )   $
��10,5q�5�  )   ��5�  )   �� *Source+N 5�  )   �� *TypeStringFormat+��5�  )   �� *TypeTemplate+��5�  )   �� *TypeTemplateSelector+5�  )   � 5�  &   $��1��5�  )   $
��10,5q�5�  )   y�5�  )   �� *Properties.Featured+8
 : $ OneWay=�O 5�  *   �� 5�  .   $
��Auto �6?   �� 5�  .   $��* �6?   .+�5   *     $��0,0,10,0q�5  -   $��Center=�5  -   	 5  -   �� *!Properties.Featured.DisplayName+$ OneWay=�5  -   �� 5  *   $��1��5  -   8��; ��5  -   �� *Properties.Featured+$ OneWay=�5  -   5  '   �� 5  &   ${ PART_ScrollViewer��5  )   $��2��5  )   $| 1��5  )   |�.  8��< {�5  )   �� *IsSourceEnabled+.��5  *   �� #��z��5  -   #} z} 5  -   5  '   5  #   5     �� 5     $~ PART_Splitter��5  !   $��2��5  !   $
 Rows=�5  !   ��5  !   �� *SplitterStyle+�� 5     $��3��5   !   ��5!  !   �� *Properties.ActiveProperty+N 5"  !   �� *DescriptionStringFormat+��5#  !   �� *DescriptionTemplate+��5$  !   �� *DescriptionTemplateSelector+y�5%  !   �� *DescriptionVisibility+5&     5'     � 5(     �� 5)     $6 False��6t   7 6&   �� *Source+8
 = .a�5*     �� $* 	PART_Type��6%    � y�$ 	Collapsed=�6R   5+     �� 5,     $
6 True��6t   7 6&   �� *Source+8
 > .a�5-     �� $* 	PART_Type��6%    6R   �� *TypeVisibility+ �� *TemplatedParent+ � y�5.     50     52     �� 53  
    �@ TypeTemplate 54     �� 55     .R�56     �� $
��Bold"�57     ��58     �� $ OneWay=�8
 ? 59     5;     �� 5<  
     �@%�IsContainerVirtualizable . 6G   �� 5=  
    �@%�IsVirtualizing . 6=   �� 5>  
    �@%�VirtualizationMode$ 	Recycling=�6A   5?     �� 5A     " R 5B  	   8   5C  	   �� 5E     " S 5F  	   8   5G  	   5H     