   M S B A M L   `   `   `  ����  Imagin.NET.Demo!    Imagin.NET.Demo.ControlView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      -   5          >�Name$  PART_ControlView��5         Brush.Border#���   5      $��1q�5      .��5      �  � DataContext 5	   	   ��  �� ElementName$ PART_ControlView�� � ColumnDefinitions 5
   
   �� 5      $	��30* �6   �� 5      $
��Auto �6   �� 5      $	��70* �6   .+�5   
   �� -   5         ��Name$ PART_ListBox��5      ��5      �� *
Controls+ �� Mode$ OneWay=� �� SelectedValue 5      �� *SelectedControl+$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=���5      �� 5      .R�5      �� ��5      �� *Name+$ OneWay=�5      5      �� 5   
   $��1��5      #���   5      $
��Auto��5       �� ResizeBehavior$ PreviousAndNext=�5      	 �� ResizeDirection$	 Columns=�5      
 �� ShowsPreview
 . $��2��5!      � 5"   
   $��2��5#       � RowDefinitions 5$      �� 5%      �� 5&      $
��Auto �6    �� 5'      .+�5)      � $��10q�5*       5+      �� 5,      $
��Auto �6$   �� 5-      $��* �6$   .+�5/      �� $��Preview��50        H1#���  51      �� 52      $��1��53      $��0,10,0,0q�55      ��54      �� *SelectedControl.Instance+$ OneWay=�55      56      �� 57      $��1��58      #���   59      $��2��5:      $ PreviousAndNext=�5;      $
	 Rows=�5<      
 . $
��Auto��5>      � 5?      $��2��5@      $��10q�5A       5B      �� 5C      $
��Auto �6$   �� 5D      $��* �6$   .+�5F      �� $��Options��5G      #���  5H      �� 5I      $��1��5J      $��Disabled=�5K      $��0,10,0,0q�5L      $
��Auto=�5M      .��5N      �� ��5O      �� *SelectedControl+$ OneWay=���5P      �� *SelectedControl.Options+$ OneWay=�5Q      5R      5S      5T      5U      5V      