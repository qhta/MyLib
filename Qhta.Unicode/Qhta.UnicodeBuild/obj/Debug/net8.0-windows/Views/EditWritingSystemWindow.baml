   M S B A M L   `   `   `  ����L  HQhta.UnicodeBuild, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:Qhta.UnicodeBuild.ViewsQhta.UnicodeBuild.Views  5   	   5    /Qhta.UnicodeBuild.Views.EditWritingSystemWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7views%clr-namespace:Qhta.UnicodeBuild.Views     5   	     � Title$  New Writing System��5	   	   $	��450��6$   $	��400��61   .��5
      �  � RowDefinitions 5      �� 5      $��* �6   �� 5      $��50 �6   .+�5      /   )Qhta.UnicodeBuild.Views.WritingSystemView  �� 5      $��1��5       �� Orientation$ 
Horizontal=�6   $��Center=�5      .+�5   	   �� $��Cancel��5   
   $��20,10q�5   
   $
��10,0q�6   ��5   
   ��  �� RelativeSource ��  �� AncestorType
 Window �� Path$ CancelCommand�5      �� 5   	   $��OK��5   
   $��20,10q�5   
   $
��10,0q�6   ��5   
   ��  �� 
 Window$ 	OkCommand���5   
   �� 5      5      5      5       