   M S B A M L   `   `   `  ����I  EQhta.WPF.Utils, Version=3.5.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:Qhta.WPF.UtilsQhta.WPF.Utils  5      ;"clr-namespace:Qhta.WPF.Utils.ViewsQhta.WPF.Utils.Views  5      E'clr-namespace:Qhta.WPF.Utils.ViewModelsQhta.WPF.Utils.ViewModels  5      -    'Qhta.WPF.Utils.Views.CompoundFilterView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .localclr-namespace:Qhta.WPF.Utils     5      4views"clr-namespace:Qhta.WPF.Utils.Views     5      6vm'clr-namespace:Qhta.WPF.Utils.ViewModels     5      $	��300��5      .��5      �   � RowDefinitions  5      �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5      �  � ColumnDefinitions 5      �� 5   
   $��* �6   �� 5   
   $
��auto �6   .+�5      �� -   5         ��Name$ OpSelectionBox��5      $��Center=�5       �� DisplayMemberPath$	 Key��5       �� SelectedValuePath$ Value��5      ��5      �� *
Operations+ �� SelectedValue 5      �� *	Operation+o� 5      -   5         o�Name$ 
MenuButton��5      $��1��5      
������ $��5q�5!      .@�5"   
   m� ��5#      �� 5$      $��..\Resources\Menu.png��6   .@�5&      m� "   Qhta.WPF.Utils.CommonStrings   AddNextCondition#��Z 6    m� Command 5'      �� *%#EditedInstance.AddNextFilterCommand+�� 5(      m� 5)      	   AddAndFunction#��Z	 6   3   -Qhta.WPF.Utils.ViewModels.FilterEditOperation
   AddAnd m� CommandParameter# Z
 5+       5*      �� *!EditedInstance.AddFilterCommand+5+      m� 5,         AddOrFunction#��Z 6      AddOr# Z 5.       5-      �� *!EditedInstance.AddFilterCommand+5.      m� 5/         Remove#��Z 6    50      �� *$"EditedInstance.RemoveFilterCommand+52      53   	   54      �� 55      -   5         ��Name$ MainListView��55      $��1��56      $
��Auto��57      $
��Auto��58      ��59      �� *Items+ � 	Resources 5:       �� DataType<   6Qhta.WPF.Utils.ViewModels.GenericColumnFilterViewModel2   ,Qhta.WPF.Utils.Views.GenericColumnFilterView3   -Qhta.WPF.Utils.ViewModels.TextFilterViewModel3   -Qhta.WPF.Utils.ViewModels.BoolFilterViewModel3   -Qhta.WPF.Utils.ViewModels.EnumFilterViewModel2   ,Qhta.WPF.Utils.ViewModels.NumFilterViewModel2	   ,Qhta.WPF.Utils.ViewModels.ObjFilterViewModel7
   1Qhta.WPF.Utils.ViewModels.CompoundFilterViewModel-   'Qhta.WPF.Utils.Views.CompoundFilterView��%C  (��       5;      *, +)(�� 2     5>      *, +)(�� d     5A      *, +)(�� �     5D      *, +)(�� �     5G      *, +)(�� �     5J      *,	 +)(�� ,    5M      *,
 +)�� 5;   
   "  6   .R�5<        5=      �� 5>   
   "  6   .R�5?        5@      �� 5A   
   "  6   .R�5B        5C      �� 5D   
   "  6   .R�5E        5F      �� 5G   
   "  6   .R�5H        5I      �� 5J   
   " 	 6   .R�5K        5L      �� 5M   
   " 
 6   .R�5N      �� 
���  ��$��1q�5O      ._�5P        5Q      5R      5S   	   ��5T      �� 5U   
    �� 
TargetType" ��6   .�5V      ��  �� Property � �� �� Value 5W      �� 5X      .R�5Y      ��  �� Orientation$ 
Horizontal=�6   68   5Z      5\      �� 5]       � ��$ Stretch=�5^      �� 5_       � ��$ Stretch=�5`      �� 5a       � ��$ 0q�5b      �� 5c       � ��
 �����5d      5e      ��5g      �� 5h   
   " ��6   .�5i      ��  � ��$ 0q�5j      �� 5k       � ��!  System.Windows.SystemColors   ControlBrushKey# �  5l      �� 5m       � ��
 �����5n      �� 5o       � ��$ 1q�5p      5q      5s      5t      5u      