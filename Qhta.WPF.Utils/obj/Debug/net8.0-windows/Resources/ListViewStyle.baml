   M S B A M L   `   `   `  ����R  NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35L5clr-namespace:System.Diagnostics;assembly=WindowsBaseSystem.Diagnostics  5      a ]PresentationFramework.Aero, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35gJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.AeroMicrosoft.Windows.Themes 5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   ListViewControlTemplate1  �� 
TargetType,   &Microsoft.Windows.Themes.ListBoxChrome   Name    BorderBrush    BorderThickness    
Background J� IsMouseOver    RenderMouseOver J� IsKeyboardFocusWithin    RenderFocused	 J� SnapsToDevicePixels
 �� Triggers P� Property P� Value �� 
TargetName!  System.Windows.SystemColors   ControlBrushKey �� Value �� Property ]� 
Conditions �� Property �� Value  ItemsPanelTemplate1 K� IsItemsHost�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      :x,http://schemas.microsoft.com/winfx/2006/xaml      5      >diag5clr-namespace:System.Diagnostics;assembly=WindowsBase  5      TthemeJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero  5      %z  &	        &	 .    �� 5      "  ��65   .R�5         $ Bd��6   # z��6&   # z��5   	   # z��6=   # z 5   	   # z 69   $
	 True��5	   	   ._�5
      �� #��z��6   ��69   C� .��5   
   �� #	 z	 6   5   	   5      
 5      P� 5       � {�$ False��6%   . �5   
   �� $ Bd��6'   # �  67    �@  
Background5   	   ]� 5       5   
   �� 5       �@��
IsGrouping$
 True��6,   �� 5      " �@%�IsVirtualizingWhenGrouping$ False��6N   ./�5   
   ��  � ��$ False��6:   5   	   5      �� 5      .R�5      $� $
 True��6   5      5      