   M S B A M L   `   `   `  ����L  HQhta.UnicodeBuild, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:Qhta.UnicodeBuild.HelpersQhta.UnicodeBuild.Helpers  5      I)clr-namespace:Qhta.UnicodeBuild.ResourcesQhta.UnicodeBuild.Resources  5      K*clr-namespace:Qhta.UnicodeBuild.ViewModelsQhta.UnicodeBuild.ViewModels  5	      G(clr-namespace:Qhta.UnicodeBuild.ControlsQhta.UnicodeBuild.Controls  5
      /    )Qhta.UnicodeBuild.Views.UcdCodePointsView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;helpers'clr-namespace:Qhta.UnicodeBuild.Helpers     5      \ XSyncfusion.Grid.Wpf, Version=30.1.39.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.GridCommon.WPF, Version=30.1.39.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89] YSyncfusion.Linq.Base, Version=30.1.39.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89d	 `Syncfusion.PropertyGrid.Wpf, Version=30.1.39.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89g
 cSyncfusion.SfBusyIndicator.WPF, Version=30.1.39.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89^ ZSyncfusion.SfGrid.WPF, Version=30.1.39.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89_ [Syncfusion.SfInput.Wpf, Version=30.1.39.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89i eSyncfusion.SfTextInputLayout.WPF, Version=30.1.39.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfTreeView.WPF, Version=30.1.39.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89^ ZSyncfusion.Shared.WPF, Version=30.1.39.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89] YSyncfusion.Tools.Wpf, Version=30.1.39.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89>sf!http://schemas.syncfusion.com/wpf    	 
       5      M	resources)clr-namespace:Qhta.UnicodeBuild.Resources    	 
       5      O
viewModels*clr-namespace:Qhta.UnicodeBuild.ViewModels    	 
       5	      Kcontrols(clr-namespace:Qhta.UnicodeBuild.Controls    	 
       5
        � 	Resources  5      3  -Syncfusion.UI.Xaml.Grid.AdvancedFilterControl �� 
TargetType �� Property �� Value   filterControlStyle/  )Syncfusion.UI.Xaml.Grid.GridFilterControl��%�   '        &	  b     �� 5      "  6   .�5      ��  �@ CanGenerateUniqueItems$ False��61   5      �� 5      "  6'   .�5      ��  �@ 
FilterMode$ AdvancedFilter=�6%   5      5       J� CommandBindings 5      )  #System.Windows.Input.CommandBinding  5      -   5           Command$
 Save��5      6M     5      -   5        $
 Copy��5      6M    J� InputBindings 5      �� 5       �� 	Modifiers$
 Ctrlf�6   
 �� Key$ S��6"   	 �� Command$
	 Save��6*   �� 5      $
 Ctrlf�6   $ C��6"   $
	 Copy��6*   .��5!      � 
 � RowDefinitions
 5"      �� 5#      $
��Auto �6   �� 5$      $��* �6   �� 5%      $
��Auto �6   .+�5(      o� $��0��6   $��2��6   .@�5)      m� )   #Qhta.UnicodeBuild.Resources.Strings   Data#��Z 6   .@�5*   
   m�    Save#��Z 6    m� Command$
 Save��6>   5+   	   m� 5,         Edit#��Z 6   .@�5-   
   m� $
��Undo��6   $
 Undo��6!   m� 5.   
   $
��Redo��6   $
 Redo��6!   �� 5/   
   m� 50   
   
   Cut#��Z 6   $	 Cut��6=   m� 51   
      Copy#��Z 6   $
 Copy��6>   m� 52   
      Paste#��Z 6   $ Paste��6?   53   	   m� 54         Tools#��Z 6   .@�55   
   m� +   %Qhta.UnicodeBuild.Resources.CodePoint   ApplyBlockMapping#��Z 6      ApplyBlockMappingTooltip � ToolTip# Z 56       57      ��   
ViewModels �� Source# [  �� Path$' !Instance.ApplyBlockMappingCommand�m� 59   
   )   "ApplyWritingSystemsMappingFromFile#��Z 6   0   )ApplyWritingSystemsMappingFromFileTooltip# Z 5:       5;      �� # [ $/ )Instance.ApplyWritingSystemMappingCommand�m� 5=   
      
FillColumn#��Z 6      FillColumnTooltip# Z 5>       5?      �� # [ $  Instance.FillColumnCommand� m� CommandParameter 5A      ��  �� ElementName$ CodePointDataGrid��5D   	   m� 5E         Help#��Z 6   .@�5F   
   m�    About#��Z 6   $
 Help��6?   5G   	   5H      (  "Syncfusion.UI.Xaml.Grid.SfDataGrid  5L      -   5           Name$  CodePointDataGrid��5L      $��1��5M      ! � DataContext#! [ 5N      (  "Syncfusion.UI.Xaml.Grid.SfGridBase"   AllowDraggingColumns$
" True��5P      #   AllowEditing$
# True��5Q      $   AllowFiltering$
$ True��5R      %   AllowResizingColumns$
% True��5S      &   AllowSorting$
& True��5T      '   AllowTriStateSorting$
' True��5U      (   ColumnSizer$
( Auto=�5V      )   HeaderRowHeight$) 26Q�5W      *   AutoGenerateColumns$* False��5Y      +   GridValidationMode$+ InEdit=�5Z      ,   EditTrigger$, OnTap=�5[      -   EnableDataVirtualization$
- True��5\      .   GridCopyOption$. CopyData,IncludeFormat=�5_      /   SelectionUnit$	/ Any=�5`      0   SelectionMode$0 Extended=�5a      1   ShowRowHeader$
1 True��5b      2   ItemsSource2 5O      �� *Instance.UcdCodePoints+3   Columns3 5r   
   0  *Syncfusion.UI.Xaml.Grid.GridTemplateColumn  ,	  &Syncfusion.UI.Xaml.Grid.GridColumnBase4 	  MappingName$4 CP��5s      	5   CP6 	  
HeaderText#6 Z5 5t      7 	  VerticalAlignment$	7 Top=�5u      8 	  GridValidationMode$8 InEdit=�5v      (
  "Syncfusion.UI.Xaml.Grid.GridColumn9 
  ColumnFilter$9 DisplayText=�5w        CodePointTemplate: 	  CellTemplate#: [ 5x      ; 	  AllowEditing$; False��5y      < 	  HeaderTemplate< 5{      �� 5|      .R�5}      �� -   5        #��Z5 5}      =   	CPTooltip# Z= 5~      5      5�      5�      ,  &Syncfusion.UI.Xaml.Grid.GridTextColumn  5�   
   $4 CharName��5�      >   Name#6 Z> 5�      $	7 Top=�5�      ? 	  Width$	? 100Q�5�      < 5�      �� 5�      .R�5�      �� #��Z> 6   @   NameTooltip# Z@ 5�      5�      5�        5�   
   $4 Glyph��5�      A   Glyph#6 ZA 5�      $	7 Top=�5�      $? 34Q�5�      B 
  AllowFiltering$B False��5�      C 	  AllowSorting$C False��5�      $; False��5�      < 5�      �� 5�      D �� DataType8   2Qhta.UnicodeBuild.ViewModels.UcdCodePointViewModel"D  6   .R�5�      �� #��ZA 6   E   GlyphTooltip# ZE 5�      5�      : 5�      �� 5�      "D  6   .R�5�      � F � ColumnDefinitionsF 5�      �� 5�      $��* �6%   �� 5�      $
��Auto �6%   .+�5�      �� $��0��6   G �� Padding$G 3q�5�      ��5�   ,   �� *Glyph+��5�      �� *	GlyphSize+&   Syncfusion.Windows.Shared.UpDown  5�      -   5        $��1��5�      H   MinValue$H 12Q�5�      I   MaxValue$I 36Q�5�      $��16��5�      J   ValueJ 5�      �� *	GlyphSize+5�      5�      5�      5�        5�   
   $4 Description��5�      K   Description#6 ZK 5�      $	7 Top=�5�      $; False��5�      $	? 300Q�5�      < 5�      �� 5�      .R�5�      �� #��ZK 6   L   DescriptionTooltip# ZL 5�      5�      5�      0  *Syncfusion.UI.Xaml.Grid.GridComboBoxColumn  5�   
   -   5        	CtgColumn��$4 Category��5�      M   Category#6 ZM 5�      N   DisplayMemberPath$
N Name��5�      $	7 Top=�5�      $; False��5�      $	? 150Q�5�      O   ItemsSourceO 5�      �� # [ $$ Instance.UnicodeCategoriesList�< 5�      �� 5�      .R�5�      �� #��ZM 6   P   
CtgTooltip# ZP 5�      5�      : 5�      �� 5�      "D  6   .R�5�      �� $G 3q�5�      ��5�      �� *Category.Name+ 5�      �� *Category.Description+5�      5�      5�        5�   
   -   5        UcdBlockColumn��$4 UcdBlock��5�      Q   UcdBlock#6 ZQ 5�      $
N Name��5�      $	7 Top=�5�      $
; True��5�      $	? 200Q�5�      O 5�      �� # [ $ Instance.SelectableBlocks�  InsertItemConverterR �� 	Converter#R [   BlankUcdBlockS �� ConverterParameter#S [ < 5�      �� 5�      .R�5�      �� #��ZQ 6   T   UcdBlockTooltip# ZT 5�      5�      : 5�      �� 5�      "D  6   .R�5�      �� $G 3q�5�      ��5�      �� *UcdBlock.Name+ 5�      �� *UcdBlock.Description+5�      5�      5�        5�   
   $
4 Area��5�      U   Area#6 ZU 5�      $
N Name��5     $	7 Top=�5     $
; True��5     $	? 200Q�5     O 5      �� # [ $ Instance.SelectableAreas�#R [   BlankWritingSystem#S [ < 5	     �� 5
     .R�5     �� #��ZU 6   V   AreaTooltip# ZV 5     5     : 5     �� 5     "D  6   .R�5     �� $G 3q�5     ��5     �� *	Area.Name+ 5     �� *Area.Description+5     5     5       5  
   $4 Script��5     W   Script#6 ZW 5     $
N Name��5!     $	7 Top=�5"     $
; True��5#     $	? 200Q�5$     O 5     �� # [ $  Instance.SelectableScripts�#R [ #S [ < 5&     �� 5'     .R�5(     �� #��ZW 6   X   ScriptTooltip# ZX 5)     5+     : 5-     �� 5.     "D  6   .R�5/     �� $G 3q�51     ��5/     �� *Script.Name+ 50     �� *Script.Description+51     53     55       57  
   $4 Language��58     Y   Language#6 ZY 59     $
N Name��5>     $	7 Top=�5?     $
; True��5@     $	? 200Q�5A     O 5:     �� # [ $" Instance.SelectableLanguages�#R [ #S [ < 5C     �� 5D     .R�5E     �� #��ZY 6   Z   LanguageTooltip# ZZ 5F     5H     : 5J     �� 5K     "D  6   .R�5L     �� $G 3q�5N     ��5L     �� *Language.Name+ 5M     �� *Language.Description+5N     5P     5R       5T  
   $4 Notation��5U     [   Notation#6 Z[ 5V     $
N Name��5[     $	7 Top=�5\     $
; True��5]     $	? 200Q�5^     O 5W     �� # [ $" Instance.SelectableNotations�#R [ #S [ < 5`     �� 5a     .R�5b     �� #��Z[ 6   \   NotationTooltip# Z\ 5c     5e     : 5g     �� 5h     "D  6   .R�5i     �� $G 3q�5k     ��5i     �� *Notation.Name+ 5j     �� *Notation.Description+5k     5m     5o       5q  
   $4 	SymbolSet��5r     ]   	SymbolSet#6 Z] 5s     $
N Name��5x     $	7 Top=�5y     $
; True��5z     $	? 200Q�5{     O 5t     �� # [ $# Instance.SelectableSymbolSets�#R [ #S [ < 5}     �� 5~     .R�5     �� #��Z] 6   ^   SymbolSetTooltip# Z^ 5�     5�     : 5�     �� 5�     "D  6   .R�5�     �� $G 3q�5�     ��5�     �� *SymbolSet.Name+ 5�     �� *SymbolSet.Description+5�     5�     5�       5�  
   $4 Subset��5�     _   Subset#6 Z_ 5�     $
N Name��5�     $	7 Top=�5�     $
; True��5�     $	? 200Q�5�     O 5�     �� # [ $  Instance.SelectableSubsets�#R [ #S [ < 5�     �� 5�     .R�5�     �� #��Z_ 6   `   SubsetTooltip# Z` 5�     5�     : 5�     �� 5�     "D  6   .R�5�     �� $G 3q�5�     ��5�     �� *Subset.Name+ 5�     �� *Subset.Description+5�     5�     5�       5�  
   $4 Artefact��5�     a   Artefact#6 Za 5�     $
N Name��5�     $	7 Top=�5�     $
; True��5�     $	? 200Q�5�     O 5�     �� # [ $" Instance.SelectableArtefacts�#R [ #S [ < 5�     �� 5�     .R�5�     �� #��Za 6   b   ArtefactTooltip# Zb 5�     5�     : 5�     �� 5�     "D  6   .R�5�     �� $G 3q�5�     ��5�     �� *Artefact.Name+ 5�     �� *Artefact.Description+5�     5�     5�     5�     4   .Qhta.UnicodeBuild.Controls.RecordNavigationBar  5�     $��2��6#   c   DataGridc 60   �� $ CodePointDataGrid��� 5�     -   5        d �Name$d 	StatusBar��5�     $��2��6   $
��Auto��6,   
�������y�5�     �� # [ $# Instance.UcdCodePoints.IsBusy�  BoolToVisibilityConverter#R [ F 5�     �� 5�  
   $
��Auto �6   �� 5�  
   $��* �6   �� 5�  
   $
��Auto �6   .+�5�     �� $��0��6   $��Center=�6�   $	��5,0q�6�   ��6"   �� # [ $* $Instance.UcdCodePoints.StatusMessage�6�   � 5�     $��1��6   e � Minimum$e 0Q�6$   f � Maximum$	f 100Q�60   $��20��6�   g � Valueg 6>   �� # [ $* $Instance.UcdCodePoints.ProgressValue�6�   �� 5�     $��2��6   h   Cancel#��Zh 5�     $��26��6=   $	��5,0q�6I   ��5�     �� # [ $4 .Instance.BreakApplyWritingSystemMappingCommand�5�     5�     5�     