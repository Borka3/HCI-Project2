   M S B A M L   `   `   `  ����I  ENetworkService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullC&clr-namespace:NetworkService.ViewModelNetworkService.ViewModel  5   	   /clr-namespace:NetworkServiceNetworkService  5   	   ;"clr-namespace:NetworkService.ViewsNetworkService.Views  5	   	   W SNotification.Wpf, Version=8.0.0.0, Culture=neutral, PublicKeyToken=1acac493089262b5_Aclr-namespace:Notification.Wpf.Controls;assembly=Notification.WpfNotification.Wpf.Controls 5
   	       NetworkService.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   =
viewModels&clr-namespace:NetworkService.ViewModel     5   	   .localclr-namespace:NetworkService     5   	   X TFontAwesome5.Net, Version=2.1.11.0, Culture=neutral, PublicKeyToken=9cfaf01297a008f8/fa5%http://schemas.fontawesome.com/icons/  5   	   .views"clr-namespace:NetworkService.Views  5	   	   UnotificationsAclr-namespace:Notification.Wpf.Controls;assembly=Notification.Wpf  5
   	     � Title$  
Measurment��5   	   $	��800��6   $	��810��6)    � 
ResizeMode$ NoResize=�65   ���#D2649A � DataContext 5      2   ,NetworkService.ViewModel.MainWindowViewModel  5   
    J� InputBindings 5      �� 5   
    �� 	Modifiers$ Controlf�6   
 �� Key$ N��5       �� Command 5      �� *GoToNetworkEntitiesView+�� 5   
   $ Controlf�6   $ D��5       5      �� *GoToDisplayView+�� 5   
   $ Controlf�6   $ G��5       5      �� *GoToGraphView+ � 	Resources 5       �� DataType7   1NetworkService.ViewModel.NetworkEntitiesViewModel*   $NetworkService.Views.NetworkEntities/   )NetworkService.ViewModel.DisplayViewModel"   NetworkService.Views.Display-   'NetworkService.ViewModel.GraphViewModel    NetworkService.Views.Graph��%�   (��       5      *, +)(�� 2     5#      *, +)(�� d     5'      *, +)�� 5   
   "  6   .R�5         5!      �� 5#   
   "  6   .R�5$        5%      �� 5'   
   "  6   .R�5(        5)      5*      .��5+      � 	 � RowDefinitions	 5-   
   �� 5.      $
��Auto �6   �� 5/      $��* �6   �� 50      $
��Auto �6   .+�53   
   ]� $��0��6   $��10q�6!   $	��Top=�6-   ���#D2649A.+�54      o� $	��Top=�6   ���#D2649A.@�55      m� 
 m� CommandParameter$
 NetworkEntities��6:    m� Command 6   �� *
NavCommand+��56      �� 57       � ToolTip$ Go to Network View��6$   ���#F6FAB9$
��Bold"�6V   $��Network Entities��6h   $	��230��6�   $��/Fonts/#Rubik)�6�    �� TextAlignment$ Center=�6�   ���#D2649A6�   59      m� 5:      $
 NetworkDisplay��6:    6   �� *
NavCommand+��5;      �� 5<      $ Go to Display View��6$   ���#F6FAB9$
��Bold"�6V   $��Network Display��6h   $	��230��6   $ Center=�6�   $��/Fonts/#Rubik)�6�   ���#D2649A6�   5>      m� 5?      $
 MeasurementGraph��6:    6   �� *
NavCommand+��5@      �� 5A      $ Go to Graph View��6$   ���#F6FAB9$
��Bold"�6T   $��Measurement Graph��6f   $	��230��6   $ Center=�6�   $��/Fonts/#Rubik)�6�   ���#D2649A6�   5C      5D      5E      � 5G   
   $��1��6   .+�5H      �� ��6   �� *CurrentViewModel+5I      �� 5K   
   $��2��6   $��10q�6"   $��Bottom=�6.   .+�5L      �� $��Enter help in terminal��6   ���#CAE6B2$��/Fonts/#Rubik)�6J   $
��Bold"�6e   $��0,0,0,5q�6w   �� 5N      -   5         ��Name$
 Term��5N      $��14(�6$   $ Terminal��62   ���#F6FAB9$	��150��6Z    �� TextWrapping$
 Wrap=�6g   ���#8E3E63$��2q�6�   ���#8E3E63��6{   �� *	Komanda+ �� Mode$ TwoWay=�.��6�    5O      �� 5P      $  f�6!   $ Enter��5Q       �� CommandParameter 6%   ��  �� ElementName$
 Term�� 5R      �� *
terminal+.��5S   )   5T      5U      0  *Notification.Wpf.Controls.NotificationArea  5W   
   -   5          Name$ WindowNotificationArea��5X       K� ZIndex$
 1000��5Y         MaxItems$ 3��5Z         Position$ TopRight=�5[      5\      5]      