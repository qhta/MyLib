   M S B A M L   `   `   `  ����L  HQhta.UnicodeBuild, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:Qhta.UnicodeBuild.ViewsQhta.UnicodeBuild.Views  5      I)clr-namespace:Qhta.UnicodeBuild.ResourcesQhta.UnicodeBuild.Resources  5      /    )Qhta.UnicodeBuild.Views.SelectValueWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      7local%clr-namespace:Qhta.UnicodeBuild.Views     5      ?	resources)clr-namespace:Qhta.UnicodeBuild.Resources     5      $	��500��5	      $	��300��6   .��5
      �   � RowDefinitions  5      �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5      �� $
��10,5q�5      ��5      ��  �� RelativeSource ��  �� AncestorType/   )Qhta.UnicodeBuild.Views.SelectValueWindow"   �� Path$ Prompt�5      �� 5      $��1��5      $
��10,5q�5      ��5      ��  �� "  $ ItemsSource� �� SelectedItem 5      ��  �� "  $ SelectedItem� �� Mode$ TwoWay=���5      �� 5   
   .R�5       �� ��6   �� *Name+ �� FallbackValue$  �� � ToolTip 5!      �� *Description+5#      5%      � 5&      $��2��5'       � Orientation$ 
Horizontal=�5(      $
��Left=�5)      $
��10,5q�5*      .+�5,      �� )   #Qhta.UnicodeBuild.Resources.Strings	   EmptyCellsOnly#��Z	 5/   	   
 d� 	IsChecked
 5-   	   ��  �� "  $ EmptyCellsOnly�5/   	   51      � 52      $��3��53      $ 
Horizontal=�54      $��Center=�55      $
��10,5q�56      .+�58      �� -   5         ��Name$ OkButton��58      	   OK#��Z 6!   $��80��6K   6V   �� 59      -   5        $ CancelButton��59         Cancel#��Z 6%   $��80��6S   $��10,0,0,0q�6{   5:      5;      5<      