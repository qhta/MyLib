   M S B A M L   `   `   `  ����I  EQhta.WPF.Utils, Version=3.5.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:Qhta.WPF.UtilsQhta.WPF.Utils  5      ;"clr-namespace:Qhta.WPF.Utils.ViewsQhta.WPF.Utils.Views  5      E'clr-namespace:Qhta.WPF.Utils.ViewModelsQhta.WPF.Utils.ViewModels  5      2    ,Qhta.WPF.Utils.Views.GenericColumnFilterView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .localclr-namespace:Qhta.WPF.Utils     5      4views"clr-namespace:Qhta.WPF.Utils.Views     5      6vm'clr-namespace:Qhta.WPF.Utils.ViewModels     5      $	��200��5        � 	Resources  5         MenuItemPanelTemplate �� 
TargetType �� Property �� Value��%  &	        'm� Q     7   '�� �     7   �� 5      .R�5      �� $��	-20,0,0,0q�6   
�������5      5      �� 5      " m�6   .�5      ��  � ��8   5      5      �� 5      " ��6   .�5      ��  � ��8   5      5      5      .��5      � -   5         �Name$% GenericColumnFilterViewMainGrid��5   	   $
��Auto��5   	   !  System.Windows.SystemColors   ControlBrushKey#���  5   	    � RowDefinitions 5!      �� 5"      $
��Auto �6   �� 5#      $
��Auto �6   .+�5%      � -   5        $$ GenericColumnFilterViewTopGrid��5%       � ColumnDefinitions 5&      �� 5'   
   $��* �6   �� 5(   
   $
��Auto �6   .+�5*      �� -   5         ��Name$ ColumnSelectionBox��5*      $��5q�5+      	 �� 
IsEditable$	 false��5,      
 �� DisplayMemberPath$
 
ColumnName��5/       �� SelectedValuePath$ Column��50       �� SelectedValue 5-      �� *Column.Column+ �� Mode$ OneWay=���5.      �� *	Columns+51      o� 52      -   5         o�Name$ 
MenuButton��52      $��5��53      
������ $��5q�55      .@�56   
   m� ��57      �� 58      $��..\Resources\Menu.png��6   .@�5:      m� "   Qhta.WPF.Utils.CommonStrings   AddAndFunction#��Z 6   3   -Qhta.WPF.Utils.ViewModels.FilterEditOperation   AddAnd m� CommandParameter# Z 5<       m� Command 5;      �� *!EditedInstance.AddFilterCommand+5<      m� 5=         AddOrFunction#��Z 6      AddOr# Z 5?       5>      �� *!EditedInstance.AddFilterCommand+5?      m� 5@         Remove#��Z 6    5A      �� *$"EditedInstance.RemoveFilterCommand+5B      5C   	   5D      �� 5E      -   5         ��Name$- 'GenericColumnFilterViewContentPresenter��5E      $��1��5F      $��5q�5G      $
��Auto��5H      ��5I      �� *SpecificFilter+  5J       �� DataType3   -Qhta.WPF.Utils.ViewModels.TextFilterViewModel)   #Qhta.WPF.Utils.Views.TextFilterView3   -Qhta.WPF.Utils.ViewModels.BoolFilterViewModel)   #Qhta.WPF.Utils.Views.BoolFilterView3   -Qhta.WPF.Utils.ViewModels.EnumFilterViewModel)	   #Qhta.WPF.Utils.Views.EnumFilterView2
   ,Qhta.WPF.Utils.ViewModels.NumFilterViewModel(   "Qhta.WPF.Utils.Views.NumFilterView2   ,Qhta.WPF.Utils.ViewModels.ObjFilterViewModel(   "Qhta.WPF.Utils.Views.ObjFilterView7   1Qhta.WPF.Utils.ViewModels.CompoundFilterViewModel-   'Qhta.WPF.Utils.Views.CompoundFilterView��%�  (��       5K      *, +)(�� 2     5N      *, +)(�� d     5Q      *, +)(�� �     5T      *,
 +)(�� �     5W      *, +)(�� �     5Z      *, +)�� 5K   
   "  6   .R�5L        5M      �� 5N   
   "  6   .R�5O        5P      �� 5Q   
   "  6   .R�5R      	  5S      �� 5T   
   " 
 6   .R�5U        5V      �� 5W   
   "  6   .R�5X        5Y      �� 5Z   
   "  6   .R�5[        5\      5]   	   5^      5_      5`      