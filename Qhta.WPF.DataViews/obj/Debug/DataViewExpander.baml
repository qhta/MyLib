   M S B A M L   `   `   `  ����M  IQhta.WPF.DataViews, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:Qhta.WPF.DataViewsQhta.WPF.DataViews  5      )    #Qhta.WPF.DataViews.DataViewExpander   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      2local clr-namespace:Qhta.WPF.DataViews     5        � 	Resources  5	         SimpleExpanderButtonTemp �� 
TargetType ��Name �� Padding � ColumnDefinitions F�Name J� RenderTransformOrigin �� Angle ��Name	 �� Triggers
 P� Property P� Value �� 
TargetName �� Property �� Value  SimpleExpanderTemp d�Name � OverridesDefaultStyle d� 	IsChecked �� Path �� RelativeSource��%"  &	        &	 6    7   �� 5
      " d�5      .R�5      �� $ ExpanderButtonBorder��6   #��z��5      #��z��5      #��z��5      # z��5      ._�5   
   �  5      �� 5      $
��Auto �6   �� 5      $��* �6   .+�5      �� 
������ $��2��5      A� 5      $��Circle��6   $��0��5      
�������$��16��5      $��16��5      $��Center=�5      $��Center=�5      F� 5!      $
 Sign��6   $��0��5"      +����    @KL р�� @KL �@KL     �@KL ��� 	���	#FF666666$��10��5%      $��10��5&      $��2��5'      $��Center=�5(      $��Center=�5)      $ 0.5,0.5&�5*      z�5,      �� 5-      $ 0Q�6    5/      �� 50      $ HeaderContent��6   $��1��51      $��4,0,0,0q�52      $��Content��53      54      55   	   	 56      P� 58   
   
 �@d�	IsChecked$
 TrueY�6'   . �59      �� $
 Sign��6#    � �� ��    @KL р�� @KL 	65   5:      P� 5=   
   
 �@J�IsMouseOver . . �5>      �� $ Circle��69    � �� �	#FF3C7FB1�� 5?      $
 Sign��62    � �� �#2225@      P� 5A   
   
 � �� . . �5B      �� $ Circle��67    � �� �	#FF526C7B�� 5C      $ Circle��6:    � ��$	 1.5��6.   6:   �� 5D      $
 Sign��67    � �� �	#FF0033665E      5G      �� 5J      " 9�61   .R�5K      ]� .+�5L   
   d� $ ExpanderButton��6   $	��Top=�5M      8��  #��z��5O       . $��1.5,0q�5S       5P      �� $ 
IsExpanded� �� *TemplatedParent+5T      �� 5U   
   $ ExpanderContent��6   $y�	Collapsed=�5V      $��Bottom=�5W      5X   	   	 5Y      P� 5Z   
   
 �@9�
IsExpanded . . �5[      �� $ ExpanderContent��6    � y�$ Visible=�5\   %   5]      5_      5`      .��5a      � .+�5b      9� -   5        #��[ 5b   K   5c      5d      