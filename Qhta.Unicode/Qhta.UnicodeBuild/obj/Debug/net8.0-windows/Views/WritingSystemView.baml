   M S B A M L   `   `   `  ����L  HQhta.UnicodeBuild, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:Qhta.UnicodeBuild.ViewModelsQhta.UnicodeBuild.ViewModels  5      E'clr-namespace:Qhta.UnicodeBuild.HelpersQhta.UnicodeBuild.Helpers  5	      I)clr-namespace:Qhta.UnicodeBuild.ResourcesQhta.UnicodeBuild.Resources  5
      9!clr-namespace:Qhta.Unicode.ModelsQhta.Unicode.Models  5      U QSystem.Runtime, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3aS:clr-namespace:System.Globalization;assembly=System.RuntimeSystem.Globalization 5      /    )Qhta.UnicodeBuild.Views.WritingSystemView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      \ XSyncfusion.Grid.Wpf, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.GridCommon.WPF, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89]	 YSyncfusion.Linq.Base, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89d
 `Syncfusion.PropertyGrid.Wpf, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89g cSyncfusion.SfBusyIndicator.WPF, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89^ ZSyncfusion.SfGrid.WPF, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89_ [Syncfusion.SfInput.Wpf, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89i eSyncfusion.SfTextInputLayout.WPF, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfTreeView.WPF, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89^ ZSyncfusion.Shared.WPF, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89] YSyncfusion.Tools.Wpf, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89>sf!http://schemas.syncfusion.com/wpf   	 
        5      9pthttp://propertytools.org/wpf   	 
        5      Gvm*clr-namespace:Qhta.UnicodeBuild.ViewModels   	 
        5      Ihelpers'clr-namespace:Qhta.UnicodeBuild.Helpers   	 
        5	      M	resources)clr-namespace:Qhta.UnicodeBuild.Resources   	 
        5
      Bmodels!clr-namespace:Qhta.Unicode.Models   	 
        5      Xsys:clr-namespace:System.Globalization;assembly=System.Runtime   	 
        5        � 	Resources  5         EmptyDataTemplate  CategoryControlTemplate �� 
TargetType � ColumnDefinitions � RowDefinitions J� SnapsToDevicePixels  ValidationErrorTemplate �� DataType2   ,Qhta.UnicodeBuild.Helpers.ValidationResultEx �� 
TargetType �� Property �� Value	 �� Triggers(   "Qhta.UnicodeBuild.Helpers.Severity
   Warning �� Value �� Binding��%  &	        &	      &	 W    �� 5      6   �� 5      " ��67   .R�5      � $��0 8 0 0q�6    5   
   �� 5      $
��Auto �6   �� 5       5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��Header��6   $
��True��62   # z 6M   �� 5    
   $��1��6   $��2��6(   $��0��6<   5!   	   5"      �� 5$      "  63   .R�5%      �� $
��Wrap=�6   ��6&   �� *ErrorMessage+��5&   
   �� 5'      " ��6   .�5(      ��  � ��
 �  ��6+   	 5)      �� 5*      # Z
 69    6   �� *
Severity+.a�5+      ��  � ��
 � ���6/   5,      �� 5-       6   �� *
Severity+ 69   Z� .a�5.      ��  � ��
 �  ��6/   5/      51      53   	   54      56      .��57      � $��	10,0,10,0q�6	     58        	RowMargin  TextBoxPadding��%R   &	       &	      u�59      0,3,0,3q�6-   u�5:      0,2,0,0q�62   5;       5<      �� 5=      $
��Auto �6   �� 5>      $��* �6    5@      �� 5A      $
��Auto �6   �� 5B      $
��Auto �6   �� 5C      $
��Auto �6   �� 5D      $
��Auto �6   �� 5E      $
��Auto �6   �� 5F      $
��Auto �6   �� 5G      $
��Auto �6   �� 5H      $
��Auto �6   �� 5I      $
��Auto �6   �� 5J      $��* �6   .+�5L      �� $��0��6   $��0��6   #��[ 5M      /   )Qhta.UnicodeBuild.Resources.WritingSystem   Name#��Z 5N         NameTooltip � ToolTip# Z 5O      �� 5P      $��0��6   $��1��6   #��[ 5Q      #��[ 5R      ��5S      �� *Name+�� 5U      $��1��6   $��0��6   #��[ 5V         Type#��Z 5W         TypeTooltip# Z 5X      �� 5Y      $��1��6   $��1��6   #��[ 5Z       �� SelectedItem 5[      �� *Name+��5\      ��   
ViewModels �� Source# [  �� Path$! Instance.WritingSystemTypes���5^      �� 5_   
   =   7Qhta.UnicodeBuild.ViewModels.WritingSystemTypeViewModel"  6   .R�5`      �� ��6   ��  & "WritingSystemTypeResourceConverter �� 	Converter# [  �� CurrentUICulture �� ConverterCulture# Z  5c      �� # [ # Z  �� ConverterParameter$ Tooltip��5g      5i      �� 5k      $��2��6   $��0��6   #��[ 5l         Kind#��Z 5m         KindTooltip# Z 5n      �� 5o      $��2��6   $��1��6   #��[ 5p       5q      �� *Kind+��5r      �� # [ $! Instance.WritingSystemKinds���5t      �� 5u   
   =   7Qhta.UnicodeBuild.ViewModels.WritingSystemKindViewModel"  6   .R�5v      �� ��6   ��  & "WritingSystemKindResourceConverter# [ # Z  5y      �� # [ # Z $ Tooltip��5}      5      �� 5�      $��3��6   $��0��6   #��[ 5�         Parent#��Z 5�         ParentTooltip# Z 5�      �� 5�      $��3��6   $��1��6   #��[ 5�       5�      �� *Parent+��5�      �� # [ $  Instance.SelectableScripts�5�      �� 5�      $��4��6   $��0��6   #��[ 5�         	KeyPhrase#��Z 5�         KeyPhraseTooltip# Z 5�      �� 5�      $��4��6   $��1��6   #��[ 5�      #��[ 5�      ��5�      �� *	KeyPhrase+�� 5�      $��5��6   $��0��6   #��[ 5�         Category#��Z 5�          
CtgTooltip# Z  5�      �� 5�      $��5��6   $��1��6   #��[ 5�      ! �� 
IsEditable$
! True��5�      ��5�      �� # [ $ Instance.Categories���5�      �� 5�   
   .R�5�      �� ��6   ��  5�      ��   CategoryResourceConverter# [ # Z $ Tooltip��5�      5�      �� 5�      $��6��6   $��0��6   #��[ 5�      
"   Iso#��Z" 5�      #   
IsoTooltip# Z# 5�      �� 5�      $��6��6   $��1��6   #��[ 5�      #��[ 5�      ��5�      �� *Iso+�� 5�      $��7��6   $��0��6   #��[ 5�      $   Abbreviation#��Z$ 5�      %   AbbrTooltip# Z% 5�      �� 5�      $��7��6   $��1��6   #��[ 5�      #��[ 5�      ��5�      �� *Abbr+�� 5�      $��8��6   $��0��6   #��[ 5�      &   	Extension#��Z& 5�      '   
ExtTooltip# Z' 5�      �� 5�      $��8��6   $��1��6   #��[ 5�      #��[ 5�      ��5�      �� *Ext+�� 5�      $��9��6   $��0��6   #��[ 5�      (   Description#��Z( 5�      �� 5�      $��9��6   $��1��6   #��[ 5�      #��[ 5�      ) �� TextWrapping$
) Wrap=�5�      ��5�      �� *Description+5�      5�      