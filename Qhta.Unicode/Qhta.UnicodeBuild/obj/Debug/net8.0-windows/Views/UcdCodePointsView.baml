   M S B A M L   `   `   `  ����L  HQhta.UnicodeBuild, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:Qhta.UnicodeBuild.HelpersQhta.UnicodeBuild.Helpers  5      I)clr-namespace:Qhta.UnicodeBuild.ResourcesQhta.UnicodeBuild.Resources  5      K*clr-namespace:Qhta.UnicodeBuild.ViewModelsQhta.UnicodeBuild.ViewModels  5	      G(clr-namespace:Qhta.UnicodeBuild.ControlsQhta.UnicodeBuild.Controls  5
      /    )Qhta.UnicodeBuild.Views.UcdCodePointsView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;helpers'clr-namespace:Qhta.UnicodeBuild.Helpers     5      \ XSyncfusion.Grid.Wpf, Version=30.1.38.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.GridCommon.WPF, Version=30.1.38.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89] YSyncfusion.Linq.Base, Version=30.1.38.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89d	 `Syncfusion.PropertyGrid.Wpf, Version=30.1.38.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89g
 cSyncfusion.SfBusyIndicator.WPF, Version=30.1.38.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89^ ZSyncfusion.SfGrid.WPF, Version=30.1.38.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89_ [Syncfusion.SfInput.Wpf, Version=30.1.38.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89i eSyncfusion.SfTextInputLayout.WPF, Version=30.1.38.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfTreeView.WPF, Version=30.1.38.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89^ ZSyncfusion.Shared.WPF, Version=30.1.38.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89] YSyncfusion.Tools.Wpf, Version=30.1.38.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89>sf!http://schemas.syncfusion.com/wpf    	 
       5      M	resources)clr-namespace:Qhta.UnicodeBuild.Resources    	 
       5      O
viewModels*clr-namespace:Qhta.UnicodeBuild.ViewModels    	 
       5	      Kcontrols(clr-namespace:Qhta.UnicodeBuild.Controls    	 
       5
        � 	Resources  5      3  -Syncfusion.UI.Xaml.Grid.AdvancedFilterControl �� 
TargetType �� Property �� Value   filterControlStyle/  )Syncfusion.UI.Xaml.Grid.GridFilterControl   WriteSystemColumnContextMenu)   #Qhta.UnicodeBuild.Resources.Strings   
FillColumn   FillColumnTooltip � ToolTip m� Command  
ViewModels �� Source	 �� Path
 m� CommandParameter �� RelativeSource �� AncestorType��%�  '        &	  b     &	 �     7  �� 5      "  6   .�5      ��  �@ CanGenerateUniqueItems$ False��61   5      �� 5      "  6'   .�5      ��  �@ 
FilterMode$ AdvancedFilter=�6%   5      �� 5      .@�5      m� #��Z 6   # Z 5       5      �� 8   $ 	 Instance.FillColumnCommand�
 5      �� *PlacementTarget+ ��  ContextMenu5      5      .��5       �  � RowDefinitions 5!      �� 5"      $
��Auto �6   �� 5#      $��* �6   �� 5$      $
��Auto �6   .+�5'      o� $��0��6   $��2��6   .@�5(      m�    Edit#��Z 6   .@�5,   
   m� 
   Cut#��Z 6   $	 Cut��6=   m� 5-   
      Copy#��Z 6   $
 Copy��6>   m� 5.   
      Paste#��Z 6   $ Paste��6?   5/   	   m� 50      $��Tools��6   .@�51   
   m� +   %Qhta.UnicodeBuild.Resources.CodePoint   ApplyBlockMapping#��Z 6      ApplyBlockMappingTooltip# Z 52       53      �� # [ $'	 !Instance.ApplyBlockMappingCommand�m� 55   
   )   "ApplyWritingSystemsMappingFromFile#��Z 6   0   )ApplyWritingSystemsMappingFromFileTooltip# Z 56       57      �� # [ $/	 )Instance.ApplyWritingSystemMappingCommand�59   	   m� 5:         Help#��Z 6   .@�5;   
   m�    About#��Z 6   $
 Help��6?   5<   	   5=      (  "Syncfusion.UI.Xaml.Grid.SfDataGrid  5A      -   5          Name$ CodePointDataGrid��5A      $��1��5B       � DataContext# [ 5C      (  "Syncfusion.UI.Xaml.Grid.SfGridBase   AllowDraggingColumns$
 True��5E         AllowEditing$
 True��5F         AllowFiltering$
 True��5G         AllowResizingColumns$
 True��5H         ColumnSizer$
 Auto=�5I         HeaderRowHeight$ 26Q�5J          AutoGenerateColumns$  False��5L      !   GridValidationMode$! InEdit=�5M      "   EditTrigger$" OnDoubleTap=�5N      #   EnableDataVirtualization$
# True��5O      $   GridCopyOption$$ CopyData=�5Q      %   ItemsSource% 5D      �� *Instance.UcdCodePoints+&   Columns& 5\   
   0  *Syncfusion.UI.Xaml.Grid.GridTemplateColumn  ,  &Syncfusion.UI.Xaml.Grid.GridColumnBase'   MappingName$' CP��5]      (   
HeaderText$( CP��5^      )   VerticalAlignment$	) Top=�5_      *   GridValidationMode$* InEdit=�5`      (	  "Syncfusion.UI.Xaml.Grid.GridColumn+ 	  ColumnFilter$+ DisplayText=�5a        CodePointTemplate,   CellTemplate#, [ 5b      -   AllowEditing$- False��5c      .   HeaderTemplate. 5e      �� 5f      .R�5g      �� 	/   CP#��Z/ 6   0   	CPTooltip# Z0 5h      5i      5k      ,
  &Syncfusion.UI.Xaml.Grid.GridTextColumn
  5l   
   $' CharName��5m      $( CharName��5n      $	) Top=�5o      1   Width$	1 100Q�5p      . 5r      �� 5s      .R�5t      �� 2   Name#��Z2 6   3   NameTooltip# Z3 5u      5v      5x        5z   
   $' Glyph��5{      $( Glyph��5|      $	) Top=�5}      $1 34Q�5~      4 	  AllowFiltering$4 False��5      5   AllowSorting$5 False��5�      $- False��5�      . 5�      �� 5�      6 �� DataType8   2Qhta.UnicodeBuild.ViewModels.UcdCodePointViewModel"6  6   .R�5�      �� 7   Glyph#��Z7 6   8   GlyphTooltip# Z8 5�      5�      , 5�      �� 5�      "6  6   .R�5�      � 9 � ColumnDefinitions9 5�      �� 5�      $��* �6%   �� 5�      $
��Auto �6%   .+�5�      �� $��0��6   : �� Padding$: 3q�5�      ��5�   ,   �� *Glyph+��5�      �� *	GlyphSize+&   Syncfusion.Windows.Shared.UpDown  5�      -   5        $��1��5�      ;   MinValue$; 12Q�5�      <   MaxValue$< 36Q�5�      $��16��5�      =   Value= 5�      �� *	GlyphSize+5�      5�      5�      5�      
  5�   
   $' Description��5�      $( Description��5�      $	) Top=�5�      $- False��5�      $	1 300Q�5�      . 5�      �� 5�      .R�5�      �� >   Description#��Z> 6   ?   DescriptionTooltip# Z? 5�      5�      5�      0  *Syncfusion.UI.Xaml.Grid.GridComboBoxColumn  5�   
   -   5        	CtgColumn��$' Category��5�      $( Category��5�      @   DisplayMemberPath$
@ Name��5�      $	) Top=�5�      $- False��5�      $	1 150Q�5�      A   ItemsSourceA 5�      �� # [ $$	 Instance.UnicodeCategoriesList�. 5�      �� 5�      .R�5�      �� B   Category#��ZB 6   C   
CtgTooltip# ZC 5�      D � ContextMenu#D [ 5�      5�      , 5�      �� 5�      "6  6   .R�5�      �� $: 3q�5�      #D [ 5�      ��5�      �� *Category.Name+ 5�      �� *Category.Description+5�      5�      5�        5�   
   -   5        UcdBlockColumn��$' UcdBlock��5�      $( UcdBlock��5�      $
@ Name��5�      $	) Top=�5�      $
- True��5�      $	1 200Q�5�      A 5�      �� # [ $	 Instance.SelectableBlocks�  InsertItemConverterE �� 	Converter#E [   BlankUcdBlockF �� ConverterParameter#F [ . 5�      �� 5�      .R�5�      �� G   UcdBlock#��ZG 6   H   UcdBlockTooltip# ZH 5�      #D [ 5�      5�      , 5�      �� 5�      "6  6   .R�5�      �� $: 3q�5�      #D [ 5�      ��5�      �� *UcdBlock.Name+ 5�      �� *UcdBlock.Description+5�      5�      5�        5�   
   $
' Area��5�      $
( Area��5�      $
@ Name��5�      $	) Top=�5�      $
- True��5�      $	1 200Q�5�      A 5�      �� # [ $	 Instance.SelectableAreas�#E [   BlankWritingSystem#F [ . 5�      �� 5�      .R�5�      �� I   Area#��ZI 6   J   AreaTooltip# ZJ 5�      #D [ 5�      5�      , 5�      �� 5�      "6  6   .R�5�      �� $: 3q�5�      #D [ 5�      ��5�      �� *	Area.Name+ 5�      �� *Area.Description+5�      5�      5       5  
   $' Script��5     $( Script��5     $
@ Name��5
     $	) Top=�5     $
- True��5     $	1 200Q�5     A 5     �� # [ $ 	 Instance.SelectableScripts�#E [ #F [ . 5     �� 5     .R�5     �� K   Script#��ZK 6   L   ScriptTooltip# ZL 5     #D [ 5     5     , 5     �� 5     "6  6   .R�5     �� $: 3q�5     #D [ 5     ��5     �� *Script.Name+ 5     �� *Script.Description+5     5     5       5   
   $' Language��5!     $( Language��5"     $
@ Name��5'     $	) Top=�5(     $
- True��5)     $	1 200Q�5*     A 5#     �� # [ $"	 Instance.SelectableLanguages�#E [ #F [ . 5,     �� 5-     .R�5.     �� M   Language#��ZM 6   N   LanguageTooltip# ZN 5/     #D [ 50     51     , 53     �� 54     "6  6   .R�55     �� $: 3q�57     #D [ 58     ��55     �� *Language.Name+ 56     �� *Language.Description+58     59     5;       5=  
   $' Notation��5>     $( Notation��5?     $
@ Name��5D     $	) Top=�5E     $
- True��5F     $	1 200Q�5G     A 5@     �� # [ $"	 Instance.SelectableNotations�#E [ #F [ . 5I     �� 5J     .R�5K     �� O   Notation#��ZO 6   P   NotationTooltip# ZP 5L     #D [ 5M     5N     , 5P     �� 5Q     "6  6   .R�5R     �� $: 3q�5T     #D [ 5U     ��5R     �� *Notation.Name+ 5S     �� *Notation.Description+5U     5V     5X       5Z  
   $' 	SymbolSet��5[     $( 	SymbolSet��5\     $
@ Name��5a     $	) Top=�5b     $
- True��5c     $	1 200Q�5d     A 5]     �� # [ $#	 Instance.SelectableSymbolSets�#E [ #F [ . 5f     �� 5g     .R�5h     �� Q   	SymbolSet#��ZQ 6   R   SymbolSetTooltip# ZR 5i     #D [ 5j     5k     , 5m     �� 5n     "6  6   .R�5o     �� $: 3q�5q     #D [ 5r     ��5o     �� *SymbolSet.Name+ 5p     �� *SymbolSet.Description+5r     5s     5u       5w  
   $' Subset��5x     $( Subset��5y     $
@ Name��5~     $	) Top=�5     $
- True��5�     $	1 200Q�5�     A 5z     �� # [ $ 	 Instance.SelectableSubsets�#E [ #F [ . 5�     �� 5�     .R�5�     �� S   Subset#��ZS 6   T   SubsetTooltip# ZT 5�     #D [ 5�     5�     , 5�     �� 5�     "6  6   .R�5�     �� $: 3q�5�     #D [ 5�     ��5�     �� *Subset.Name+ 5�     �� *Subset.Description+5�     5�     5�       5�  
   $' Artefact��5�     $( Artefact��5�     $
@ Name��5�     $	) Top=�5�     $
- True��5�     $	1 200Q�5�     A 5�     �� # [ $"	 Instance.SelectableArtefacts�#E [ #F [ . 5�     �� 5�     .R�5�     �� U   Artefact#��ZU 6   V   ArtefactTooltip# ZV 5�     #D [ 5�     5�     , 5�     �� 5�     "6  6   .R�5�     �� $: 3q�5�     #D [ 5�     ��5�     �� *Artefact.Name+ 5�     �� *Artefact.Description+5�     5�     5�     5�     4   .Qhta.UnicodeBuild.Controls.RecordNavigationBar  5�     $��2��6#   W   DataGridW 60   �� X �� ElementName$X CodePointDataGrid��� 5�     -   5        Y �Name$Y 	StatusBar��5�     $��2��6   $
��Auto��6,   
�������y�5�     �� # [ $#	 Instance.UcdCodePoints.IsBusy�  BoolToVisibilityConverter#E [ 9 5�     �� 5�  
   $
��Auto �6   �� 5�  
   $��* �6   �� 5�  
   $
��Auto �6   .+�5�     �� $��0��6   $��Center=�6�   $	��5,0q�6�   ��6"   �� # [ $*	 $Instance.UcdCodePoints.StatusMessage�6�   � 5�     $��1��6   Z � Minimum$Z 0Q�6$   [ � Maximum$	[ 100Q�60   $��20��6�   \ � Value\ 6>   �� # [ $*	 $Instance.UcdCodePoints.ProgressValue�6�   �� 5�     $��2��6   ]   Cancel#��Z] 5�     $��26��6=   $	��5,0q�6I   ��5�     �� # [ $4	 .Instance.BreakApplyWritingSystemMappingCommand�5�     5�     5�     