   M S B A M L   `   `   `  ����L  HQhta.UnicodeBuild, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:Qhta.UnicodeBuild.ResourcesQhta.UnicodeBuild.Resources  5      K*clr-namespace:Qhta.UnicodeBuild.ViewModelsQhta.UnicodeBuild.ViewModels  5      H DQhta.SF.Tools, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullD2clr-namespace:Qhta.SF.Tools;assembly=Qhta.SF.ToolsQhta.SF.Tools 5	      /    )Qhta.UnicodeBuild.Views.UcdCodePointsView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      \ XSyncfusion.Grid.Wpf, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.GridCommon.WPF, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89]	 YSyncfusion.Linq.Base, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89d
 `Syncfusion.PropertyGrid.Wpf, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89g cSyncfusion.SfBusyIndicator.WPF, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89^ ZSyncfusion.SfGrid.WPF, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89_ [Syncfusion.SfInput.Wpf, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89i eSyncfusion.SfTextInputLayout.WPF, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfTreeView.WPF, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89^ ZSyncfusion.Shared.WPF, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89] YSyncfusion.Tools.Wpf, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89>sf!http://schemas.syncfusion.com/wpf   	 
        5      M	resources)clr-namespace:Qhta.UnicodeBuild.Resources   	 
        5      O
viewModels*clr-namespace:Qhta.UnicodeBuild.ViewModels   	 
        5      Rtools2clr-namespace:Qhta.SF.Tools;assembly=Qhta.SF.Tools   	 
        5	        � 	Resources  5      3  -Syncfusion.UI.Xaml.Grid.AdvancedFilterControl �� 
TargetType �� Property �� Value   filterControlStyle/  )Syncfusion.UI.Xaml.Grid.GridFilterControl��%�   '        &	  b     �� 5      "  6   .�5      ��  �@ CanGenerateUniqueItems$ False��61   5      �� 5      "  6'   .�5      ��  �@ 
FilterMode$ AdvancedFilter=�6%   5      5       J� CommandBindings 5      )  #System.Windows.Input.CommandBinding  5      -   5           Command$
 Save��5      6M     5      -   5        $
 Copy��5      6M     5      -   5        $	 Cut��5      6L     5      -   5        $ Paste��5      6N     5      -   5        $ Delete��5      6O     5      -   5        $
 Undo��5      6M     5      -   5        $
 Redo��5      6M    J� InputBindings 5!      �� 5"       �� 	Modifiers$
 Ctrlf�6   
 �� Key$ S��6"   	 �� Command$
	 Save��6*   �� 5#      $
 Ctrlf�6   $ C��6"   $
	 Copy��6*   �� 5$      $
 Ctrlf�6   $ X��6"   $		 Cut��6*   �� 5%      $
 Ctrlf�6   $ V��6"   $	 Paste��6*   �� 5&      $ Delete��6   $	 Delete��6   .��5(      � 
 � RowDefinitions
 5)      �� 5*      $
��Auto �6   �� 5+      $��* �6   �� 5,      $
��Auto �6   .+�5/      o� $��0��6   $��2��6   .@�50      m� )   #Qhta.UnicodeBuild.Resources.Strings   Data#��Z 6   .@�51   
   m�    Save#��Z 6    m� Command$
 Save��6>   52   	   m� 53         Edit#��Z 6   .@�54   
   m� $
��Undo��6   $
 Undo��6!   m� 55   
   $
��Redo��6   $
 Redo��6!   �� 56   
   m� 57   
   
   Cut#��Z 6   $	 Cut��6=   m� 58   
      Copy#��Z 6   $
 Copy��6>   m� 59   
      Paste#��Z 6   $ Paste��6?   5:   	   m� 5;         Tools#��Z 6   .@�5<   
   m� +   %Qhta.UnicodeBuild.Resources.CodePoint   ApplyBlockMapping#��Z 6      ApplyBlockMappingTooltip � ToolTip# Z 5=       5>      ��   
ViewModels �� Source# [  �� Path$' !Instance.ApplyBlockMappingCommand�m� 5@   
   )   "ApplyWritingSystemsMappingFromFile#��Z 6   0   )ApplyWritingSystemsMappingFromFileTooltip# Z 5A       5B      �� # [ $/ )Instance.ApplyWritingSystemMappingCommand�m� 5D   
      
FillColumn#��Z 6      FillColumnTooltip# Z 5E       5F      �� # [ $  Instance.FillColumnCommand� m� CommandParameter 5H      ��  �� ElementName$ CodePointDataGrid��5K   	   m� 5L         Help#��Z 6   .@�5M   
   m�    About#��Z 6   $
 Help��6?   5N   	   5O      (  "Syncfusion.UI.Xaml.Grid.SfDataGrid  5S      -   5           Name$  CodePointDataGrid��5S      $��1��5T      ! � DataContext#! [ 5U      (  "Syncfusion.UI.Xaml.Grid.SfGridBase"   AutoGenerateColumns$" False��5X      &   Qhta.SF.Tools.SfDataGridBehavior#   AllowRowResizing$
# True��5[      $   ItemsSource$ 5V      �� *Instance.UcdCodePoints+%   Columns% 5k   
   0	  *Syncfusion.UI.Xaml.Grid.GridTemplateColumn	  ,
  &Syncfusion.UI.Xaml.Grid.GridColumnBase& 
  MappingName$& CP��5l      	'   CP( 
  
HeaderText#( Z' 5m      ) 
  VerticalAlignment$	) Top=�5n      * 
  GridValidationMode$* InEdit=�5o      (  "Syncfusion.UI.Xaml.Grid.GridColumn+   ColumnFilter$+ DisplayText=�5p        CodePointTemplate, 
  CellTemplate#, [ 5q      - 
  AllowEditing$- False��5r      . 
  HeaderTemplate. 5t      �� 5u      .R�5v      �� #��Z' 6   /   	CPTooltip# Z/ 5w      5x      5z      ,  &Syncfusion.UI.Xaml.Grid.GridTextColumn  5{   
   $& CharName��5|      0   Name#( Z0 5}      $	) Top=�5~      1 
  Width$	1 100Q�5      . 5�      �� 5�      .R�5�      �� #��Z0 6   2   NameTooltip# Z2 5�      5�      5�      	  5�   
   $& Glyph��5�      3   Glyph#( Z3 5�      $	) Top=�5�      $1 34Q�5�      4   AllowFiltering$4 False��5�      5 
  AllowSorting$5 False��5�      $- False��5�      . 5�      �� 5�      6 �� DataType8   2Qhta.UnicodeBuild.ViewModels.UcdCodePointViewModel"6  6   .R�5�      �� #��Z3 6   7   GlyphTooltip# Z7 5�      5�      , 5�      �� 5�      "6  6   .R�5�      � 8 � ColumnDefinitions8 5�      �� 5�      $��* �6%   �� 5�      $
��Auto �6%   .+�5�      �� $��0��6   9 �� Padding$9 3q�5�      ��5�   ,   �� *Glyph+��5�      �� *	GlyphSize+&   Syncfusion.Windows.Shared.UpDown  5�      -	   5        $��1��5�      :   MinValue$: 12Q�5�      ;   MaxValue$; 36Q�5�      $��16��5�      <   Value< 5�      �� *	GlyphSize+5�      5�      5�      5�        5�   
   $& Description��5�      =   Description#( Z= 5�      $	) Top=�5�      $- False��5�      $	1 200Q�5�      . 5�      �� 5�      .R�5�      �� #��Z= 6   >   DescriptionTooltip# Z> 5�      5�      5�      0  *Syncfusion.UI.Xaml.Grid.GridComboBoxColumn  5�   
   -
   5        	CtgColumn��$& Category��5�      ?   Category#( Z? 5�      @   DisplayMemberPath$
@ Name��5�      $	) Top=�5�      $- False��5�      $	1 150Q�5�      A   ItemsSourceA 5�      �� # [ $$ Instance.UnicodeCategoriesList�. 5�      �� 5�      .R�5�      �� #��Z? 6   B   
CtgTooltip# ZB 5�      5�      , 5�      �� 5�      "6  6   .R�5�      �� $9 3q�5�      ��5�      �� *Category.Name+ 5�      �� *Category.Description+5�      5�      5�        5�   
   -   5        UcdBlockColumn��$& UcdBlock��5�      C   UcdBlock#( ZC 5�      $
@ Name��5�      $	) Top=�5�      $
- True��5�      $	1 150Q�5�      A 5�      �� # [ $ Instance.SelectableBlocks�  InsertItemConverterD �� 	Converter#D [   BlankUcdBlockE �� ConverterParameter#E [ . 5�      �� 5�      .R�5�      �� #��ZC 6   F   UcdBlockTooltip# ZF 5�      5�      , 5�      �� 5�      "6  6   .R�5�      �� $9 3q�5�      ��5�      �� *UcdBlock.Name+ 5�      �� *UcdBlock.Description+5�      5�      5�        5�   
   $
& Area��5�      G   Area#( ZG 5�      $
@ Name��5�      $	) Top=�5�      $
- True��5�      $	1 100Q�5�      A 5�      �� # [ $ Instance.SelectableAreas�#D [   BlankWritingSystem#E [ . 5     �� 5     .R�5     �� #��ZG 6   H   AreaTooltip# ZH 5     5     , 5     �� 5	     "6  6   .R�5
     �� $9 3q�5     ��5
     �� *	Area.Name+ 5     �� *Area.Description+5     5     5       5  
   $& Script��5     I   Script#( ZI 5     $
@ Name��5     J   SortMode$J Display=�5     $	) Top=�5     $
- True��5     $	1 100Q�5     A 5     �� # [ $  Instance.SelectableScripts�#D [ #E [ . 5     �� 5      .R�5!     �� #��ZI 6   K   ScriptTooltip# ZK 5"     5$     , 5&     �� 5'     "6  6   .R�5(     �� $9 3q�5*     ��5(     �� *Script.Name+ 5)     �� *Script.Description+5*     5,     5.       50  
   $& Language��51     L   Language#( ZL 52     $
@ Name��57     $	) Top=�58     $
- True��59     $	1 100Q�5:     A 53     �� # [ $" Instance.SelectableLanguages�#D [ #E [ . 5<     �� 5=     .R�5>     �� #��ZL 6   M   LanguageTooltip# ZM 5?     5A     , 5C     �� 5D     "6  6   .R�5E     �� $9 3q�5G     ��5E     �� *Language.Name+ 5F     �� *Language.Description+5G     5I     5K       5M  
   $& Notation��5N     N   Notation#( ZN 5O     $
@ Name��5T     $	) Top=�5U     $
- True��5V     $	1 100Q�5W     A 5P     �� # [ $" Instance.SelectableNotations�#D [ #E [ . 5Y     �� 5Z     .R�5[     �� #��ZN 6   O   NotationTooltip# ZO 5\     5^     , 5`     �� 5a     "6  6   .R�5b     �� $9 3q�5d     ��5b     �� *Notation.Name+ 5c     �� *Notation.Description+5d     5f     5h       5j  
   $& 	SymbolSet��5k     P   	SymbolSet#( ZP 5l     $
@ Name��5q     $	) Top=�5r     $
- True��5s     $	1 100Q�5t     A 5m     �� # [ $# Instance.SelectableSymbolSets�#D [ #E [ . 5v     �� 5w     .R�5x     �� #��ZP 6   Q   SymbolSetTooltip# ZQ 5y     5{     , 5}     �� 5~     "6  6   .R�5     �� $9 3q�5�     ��5     �� *SymbolSet.Name+ 5�     �� *SymbolSet.Description+5�     5�     5�       5�  
   $& Subset��5�     R   Subset#( ZR 5�     $
@ Name��5�     $	) Top=�5�     $
- True��5�     $	1 100Q�5�     A 5�     �� # [ $  Instance.SelectableSubsets�#D [ #E [ . 5�     �� 5�     .R�5�     �� #��ZR 6   S   SubsetTooltip# ZS 5�     5�     , 5�     �� 5�     "6  6   .R�5�     �� $9 3q�5�     ��5�     �� *Subset.Name+ 5�     �� *Subset.Description+5�     5�     5�       5�  
   $& Artefact��5�     T   Artefact#( ZT 5�     $
@ Name��5�     $	) Top=�5�     $
- True��5�     $	1 100Q�5�     A 5�     �� # [ $" Instance.SelectableArtefacts�#D [ #E [ . 5�     �� 5�     .R�5�     �� #��ZT 6   U   ArtefactTooltip# ZU 5�     5�     , 5�     �� 5�     "6  6   .R�5�     �� $9 3q�5�     ��5�     �� *Artefact.Name+ 5�     �� *Artefact.Description+5�     5�     5�     5�     '  !Qhta.SF.Tools.RecordNavigationBar  5�     $��2��6    V   DataGridV 6-   �� $ CodePointDataGrid��� 5�     -   5        W �Name$W 	StatusBar��5�     $��2��6   $
��Auto��6,   
�������y�5�     �� # [ $# Instance.UcdCodePoints.IsBusy�  BoolToVisibilityConverter#D [ 8 5�     �� 5�  
   $
��Auto �6   �� 5�  
   $��* �6   �� 5�  
   $
��Auto �6   .+�5�     �� $��0��6   $��Center=�6�   $	��5,0q�6�   ��6"   �� # [ $* $Instance.UcdCodePoints.StatusMessage�6�   � 5�     $��1��6   X � Minimum$X 0Q�6$   Y � Maximum$	Y 100Q�60   $��20��6�   Z � ValueZ 6>   �� # [ $* $Instance.UcdCodePoints.ProgressValue�6�   �� 5�     $��2��6   [   Cancel#��Z[ 5�     $��26��6=   $	��5,0q�6I   ��5�     �� # [ $4 .Instance.BreakApplyWritingSystemMappingCommand�5�     5�     5�     