   M S B A M L   `   `   `  ���� M  IQhta.WPF.DataViews, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:Qhta.WPF.DataViewsQhta.WPF.DataViews  )    #Qhta.WPF.DataViews.DataViewExpander   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     2local clr-namespace:Qhta.WPF.DataViews       � 	Resources     SimpleExpanderButtonTemp �� 
TargetType ��Name �� Padding � ColumnDefinitions F�Name J� RenderTransformOrigin �� Angle ��Name	 �� Triggers
 P� Property P� Value �� 
TargetName �� Property �� Value  SimpleExpanderTemp d�Name � OverridesDefaultStyle d� 	IsChecked �� Path �� RelativeSource��%U  &	        &	     7   �� " d�.R��� $ ExpanderButtonBorder��#��z��#��z��#��z��# z��._��  �� $
��Auto ��� $��* �.+��� 
������ $��2��A� $��Circle��$��0��
�������$��16��$��16��$��Center=�$��Center=�F� $
 Sign��$��0��+����    @KL р�� @KL �@KL     �@KL ��� 	���	#FF666666$��10��$��10��$��2��$��Center=�$��Center=�$ 0.5,0.5&�z��� $ 0Q��� $ HeaderContent��$��1��$��4,0,0,0q�$��Content��	 P� 
 �@d�	IsChecked$
 TrueY�. ��� $
 Sign�� � �� ��    @KL р�� @KL 	P� 
 �@J�IsMouseOver . . ��� $ Circle�� � �� �	#FF3C7FB1�� $
 Sign�� � �� �#222P� 
 � �� . . ��� $ Circle�� � �� �	#FF526C7B�� $ Circle�� � ��$	 1.5���� $
 Sign�� � �� �	#FF003366�� " 9�.R�]� .+�d� $ ExpanderButton��$	��Top=�8��  #��z�� . $��1.5,0q� �� $ 
IsExpanded� �� *TemplatedParent+�� $ ExpanderContent��$y�	Collapsed=�$��Bottom=�	 P� 
 �@9�
IsExpanded . . ��� $ ExpanderContent�� � y�$ Visible=�.��� .+�9� -   #��[ 