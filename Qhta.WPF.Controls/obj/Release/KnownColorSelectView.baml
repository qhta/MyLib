   M S B A M L   `   `   `  ���� L  HQhta.WPF.Controls, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null5clr-namespace:Qhta.WPF.ControlsQhta.WPF.Controls  I EQhta.WPF.Utils, Version=1.1.0.0, Culture=neutral, PublicKeyToken=nullG4clr-namespace:Qhta.WPF.Utils;assembly=Qhta.WPF.UtilsQhta.WPF.Utils N JQhta.WPF.ColorUtils, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null@3clr-namespace:Qhta.WPF;assembly=Qhta.WPF.ColorUtilsQhta.WPF ,    &Qhta.WPF.Controls.KnownColorSelectView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     1localclr-namespace:Qhta.WPF.Controls     Futils4clr-namespace:Qhta.WPF.Utils;assembly=Qhta.WPF.Utils     Bcu3clr-namespace:Qhta.WPF;assembly=Qhta.WPF.ColorUtils     .��� -     �Name$  ThisGrid�� � 	Resources #  Qhta.WPF.Utils.ColorConverter   ColorConverter.  (Qhta.WPF.Utils.BoolToVisibilityConverter  BoolToVisibilityConverter  DeleteImage��%e   &	        &	      &	 
         �� $7��1/Qhta.WPF.Controls;component/Resources/Delete.png��.+��� -    ��Name$ KnownColorsListBox��$��Disabled=� �� SelectionMode$ Single=� J� SnapsToDevicePixels .   �� 
TargetType �� BasedOn �� Property �� Value	 �� Mode
 �� DataType  Qhta.WPF.KnownColor � ToolTip �� Path � ContextMenu �� 	Converter  Qhta.WPF.CustomColor �� Orientation m� Icon��%�  '��       7��(�� O     *, +)7   (�� �    *, +)7  7   �� " ��8   .���  �@��
IsSelected �� *
IsSelected+$	 TwoWay=��� "
  .R�� $
��0,-1q� �� $
 Name� �� .@�m� $3��-Context menu is active for custom colors only��.+�F� $��15��$��15��$	��2,1q�$��Center=�
���   �$��1��+����        ���� ��� �    ��� ����     	�� $��15��$��15��$	��2,1q�$��Center=�
���   ����� $ Color�8   �� "
  .R�� $
��0,-1q� �� $
 Name� �� .@�m� ���� $ 
Horizontal=�.+��� Name�� -   $	��120��$��20��$��10,0,0,0q����� *Name+m� -   $��Delete��8   .+�F� $��15��$��15��$	��2,1q�$��Center=�
���   �$��1��+����        ���� ��� �    ��� ����     	�� $��15��$��15��$	��2,1q�$��Center=�
���   ����� $ Color�8  �� $��16��$��16��$B��</Qhta.WPF.Controls;component/Resources/CustomColorMarker.png��$��Right=�$	��Top=����� .R��  K� IsItemsHost . 