   M S B A M L   `   `   `  ����I  EQhta.WPF.Utils, Version=3.5.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:Qhta.WPF.UtilsQhta.WPF.Utils  5      R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35L5clr-namespace:System.Diagnostics;assembly=WindowsBaseSystem.Diagnostics 5      a ]PresentationFramework.Aero, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35gJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.AeroMicrosoft.Windows.Themes 5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   ListBoxBorderBrush  �� Color  MyListBoxStyle �� 
TargetType �� Property!   System.Windows.SystemColors    WindowBrushKey �� Value    ControlTextBrushKey �� 
TargetType J� SnapsToDevicePixels �� Padding	 �� Triggers
 P� Property P� Value �� 
TargetName    ControlBrushKey ]� 
Conditions �� Property �� Value  ListBoxItemFocusVisualStyle    HighlightTextBrushKey �� StrokeDashArray �� Path �� RelativeSource �� AncestorType    HighlightBrushKey    GrayTextBrushKey �� BasedOn�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     5      &utilsclr-namespace:Qhta.WPF.Utils  5      >diag5clr-namespace:System.Diagnostics;assembly=WindowsBase  5      TthemeJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero  5      %�  &	        &	 !     7   &	 g    '�� (    '�� �    7  �� 5      $  #828790��6/   �� 5      " ��6!   .�5      ��  � ��# �  5   	   �� 5       � ��8   6$   �� 5       � ��$ 1q�6(   �� 5       � ��# �  5   	   �� 5       � ��$
 Auto=�6C   �� 5       � ��$
 Auto=�6A   �� 5       � ��$
 True��66   �� 5       �@��PanningMode$
 Both=�61   �� 5       �@��IsFlicksEnabled$ False��6/   �� 5       � ��$ Center=�61   �� 5        � �� 5!      �� 5"   
   " ��6   .R�5#      �� $��Bd��6   #��z��6   #��z��5$      #��z��5%      $
 True��5&      $ 0q�6*   ._�5'      �� #��z��6   $|�False��6?   .��5(      � .+�5)      �� .u�5*      �� # z 5+      5,      5-      5.      5/      	 50      P� 51      
 � {�$ False��6+   . �52      �� $ Bd��6   # �  53       � ��54      55      ]� 56       57      �� 58       �@��
IsGrouping$
 True��6?   �� 59      " �@%�IsVirtualizingWhenGrouping$ False��5;      ./�5=      ��  � ��$ False��6@   5>      5@      5B      5C      �� 5D      .�5E      ��  � �� 5F      �� 5G   
   .R�5H      �� $��0,1,0,1q�6   $��1��6'   #���  5I      $	 1 2R�5J      $
 True��6%   5K      5M      5N      �� 5T      " ��6'   .�5U      ��  � �� �	#00FFFFFF6#   �� 5V       � �� 5W   	   �� $  HorizontalContentAlignment� �� " ���� 5X       � �� 5Y   	   �� $ VerticalContentAlignment� �� " ���� 5Z       � ��$ 2,0,0,0q�6    �� 5[       � �� 5\      �� 5]   
   " ��6   .R�5^      � $��0,0,0,-1q�6   .+�5_      �� $��Bd��6   #��z��6!   $��0,1,0,0q�5`      # z��5a      $
 True��65   ._�5f      �� #��z��5g      #��z��5h      # z 5i      5j      5v      	 5w      P� 5x      
 �@��
IsSelected$
 True��68   . �5y      �� $ Bd��6   # �  5z       � ��5{      �� 5|      # �  5}       � ��5~      5      P� 5�      
 � {�$ False��6+   . �5�      �� # �  5�       � ��5�      5�      5�      5�      5�      �� 5�      " ��6#   8   5�      5�      