   M S B A M L   `   `   `  ����L  HQhta.UnicodeBuild, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:Qhta.UnicodeBuildQhta.UnicodeBuild  5   	   A%clr-namespace:Qhta.UnicodeBuild.ViewsQhta.UnicodeBuild.Views  5   	   I)clr-namespace:Qhta.UnicodeBuild.ResourcesQhta.UnicodeBuild.Resources  5	   	   "    Qhta.UnicodeBuild.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:Qhta.UnicodeBuild     5   	   7views%clr-namespace:Qhta.UnicodeBuild.Views     5   	   \ XSyncfusion.Grid.Wpf, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.GridCommon.WPF, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89] YSyncfusion.Linq.Base, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89d	 `Syncfusion.PropertyGrid.Wpf, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89g
 cSyncfusion.SfBusyIndicator.WPF, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89^ ZSyncfusion.SfGrid.WPF, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89_ [Syncfusion.SfInput.Wpf, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89i eSyncfusion.SfTextInputLayout.WPF, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfTreeView.WPF, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89^ ZSyncfusion.Shared.WPF, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89] YSyncfusion.Tools.Wpf, Version=30.1.40.0, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89F
syncfusion!http://schemas.syncfusion.com/wpf    	 
       5   	   M	resources)clr-namespace:Qhta.UnicodeBuild.Resources    	 
       5	   	   -   5          � Title$  
MainWindow��5   	   $	��450��6   $	��850��6)    J� InputBindings 5      �� 5       �� 	Modifiers$
 Ctrlf�6   
 �� Key$ S��6"    �� Command$
 Save��6*   �� 5      $
 Ctrlf�6   $ C��6"   $
 Copy��6*   �� 5      $
 Ctrlf�6   $ X��6"   $	 Cut��6*   �� 5      $
 Ctrlf�6   $ V��6"   $ Paste��6*   �� 5      $ Delete��6   $ Delete��6   .��5      � .+�5      �� -   5        $��MainTabControl��5      .@�5      �� -   5        ��5   
   �� 5      -   5        )   #Qhta.UnicodeBuild.Resources.Strings   
CodePoints#��Z 5         CodePointsTooltip � ToolTip# Z 5      5      .��5    
   /   )Qhta.UnicodeBuild.Views.UcdCodePointsView  5!   	   �� 5"      -   5        ��5#   
   �� 5$      -   5           	UcdBlocks#��Z 5$      	   UcdBlocksTooltip# Z	 5%      5&      .��5(   
   +   %Qhta.UnicodeBuild.Views.UcdBlocksView  5)   	   �� 5*      -   5        ��5+   
   �� 5,      -   5        
   WritingSystems#��Z
 5,         WritingSystemsTooltip# Z 5-      5.      .��50   
   0   *Qhta.UnicodeBuild.Views.WritingSystemsView  51   	   52      53      54      