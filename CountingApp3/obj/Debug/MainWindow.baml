   M S B A M L   `   `   `  ����G  CCountingApp3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:CountingApp3CountingApp3  5   	       CountingApp3.MainWindow   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:CountingApp3     5   	     � SizeToContent$  WidthAndHeight=�5	   	   $	��500��5
   	    � Topmost .  � Left$ 0��5   	   
 � Top$ 0��5   	    � ShowInTaskbar .   � Icon$2 ,pack://application:,,,/Resources/favicon.ico��5   	    J� 	AllowDrop .  � WindowStyle$ 
ToolWindow=�5   	    � DataContext 5      "   CountingApp3.MainDataContext  5   
   	 J� CommandBindings	 5      )  #System.Windows.Input.CommandBinding  5   
   -   5        
   Command$
 ApplicationCommands.Open��5      6<     5   
   -   5        $"
 local:CustomCommands.SaveAll��5      6@    � 	Resources 5      #   CountingApp3.InverseConverter   InverseConverter  RadioButtonListBoxStyle �� 
TargetType �� Property �� Value-  'System.Windows.Input.KeyboardNavigation J� IsHitTestVisible d� 	IsChecked �� Path �� RelativeSource �� AncestorType �� Mode �� Triggers �� Binding �� Value��%  &	        &	        5   
   6!   �� 5   
   " ��60   .�5      ��  � ��
 ���� 6,   �� 5       �@ DirectionalNavigation$ Cycle=�6I   �� 5       � ��$ Stretch=�6;   �� 5       � ��$ Hidden=�6K   �� 5        � �� 5!      �� 5"      .R�5#      d� $
��Left=�5$   '   
������ 
������  .   5#   '   �� $ 
IsSelected� �� " ��$ TwoWay=���5)      �� 5*   "   " d�6(    5+   &   �� 5,   *    66   �� *	IsVisible+	 False6T   .a�5-   .   ��  �@J�Opacity$	 0.2Q�6H   5.   +   50   #   ��52      ]� 53   "   .+�54   &   A� $��10��55   .   $��10��56   .   
���   �$��1��58   .   ��54   .   �� *Brush+��59   *   �� 5:   .   " A�64    5;   2   �� 5<   6    6B   �� *Color+ 6\   Z� .a�5=   :   ��  � y�$ 	Collapsed=�6W   5>   7   5@   /   5B   '   �� 5C   &   �� 5D   *   ��6;   �� *DisplayCategory+$ OneTime=�5E   '   5F   #   5H      5I      5K      5L      5M      .��5N      �  � RowDefinitions 5O   
   �� 5P      $
��Auto �6   �� 5Q      $��* �6   .+�5S   
   o� $��0��6   .@�5T      m� ��5U      �� 5V       �� Orientation$ 
Horizontal=�6!   .+�5W      �� $<��6pack://application:,,,/Resources/HamburgerMenu_16x.png��6    �� 5X      $
��Menu��6$   5Y      .@�5[      m� $
��Open��6    m� Command$ ApplicationCommands.Open��6)    m� Icon 5\      �� 5]      $9��3pack://application:,,,/Resources/FolderOpen_16x.png��6    5_      m� 5`      $��Save All��6   $" local:CustomCommands.SaveAll��6-    5a      �� 5b      $6��0pack://application:,,,/Resources/SaveAll_16x.png��6    5d      m� 5e      -   5        $��Overlay All��5e       5f      �� 5g      $:��4pack://application:,,,/Resources/ImageChisel_16x.png��6    5i      m� 5j      $��Export All Into...��6    5k      �� 5l      $;��5pack://application:,,,/Resources/ResultToText_16x.png��6    .@�5n      m� -   5        $
��File��5n       5o      �� 5p      $;��5pack://application:,,,/Resources/ResultToText_16x.png��6$   5r      m� 5s      -   5        $��Database��5s       5t      �� 5u      $>��8pack://application:,,,/Resources/WriteToDatabase_16x.png��6$   5w      5x      m� 5y      -   5        $��Database Connection��5y       5z      �� 5{      $<��6pack://application:,,,/Resources/AddDataSource_16x.png��6    5}      �� 5~      m� 5      $��Show Image Menus��6    m� IsCheckable .  m� StaysOpenOnClick .  m� 	IsChecked 5�      �� *ShowImageMenus+ 5�      �� 5�      $8��2pack://application:,,,/Resources/CloakHide_16x.png��6    5�      m� 5�       .  5�      �� 5�      $<��6pack://application:,,,/Resources/BreakPointXHR_16x.png��6    ��5�      �� 5�      $ 
Horizontal=�6%   .+�5�      �� $��
Dot Size: ��6(   $��Center=�6:   �� 5�      -   5        $��-��5�   r   ��6%   ��  �� *Self+$ ActualHeight�6~   �� 5�      -   5        $��+��5�   r   ��6%   ��  �� *Self+$ ActualHeight�6~   5�      5�      �� 5�      m� 5�      -	   5        $��	Close All��5�       5�      �� 5�      $A��;pack://application:,,,/Resources/CloseDocumentGroup_16x.png��6    5�      m� 5�      -
   5        $
��Exit��5�       5�      �� 5�      $4��.pack://application:,,,/Resources/Close_16x.png��6    5�      5�      5�      �� 5�   
   $��1��6   $��Disabled=�6$   $
��Auto=�6M   .��5�      �� -   5        #��[ 5�       �� SelectedValue 5�      �� *
ActiveTool+��5�      �� *CollectionTools+��5�      �� 5�      " ��6   .�5�      ��  � {� 66   �� *	IsVisible+�� 5�       �@��ShowOnDisabled . 6N   �� 5�       � ��$ 0q�64   �� 5�       �@�ToolTip 64   �� *
Category+�� 5�       �@�ContextMenu 5�      �� 5�   "   .@�5�   &   m�  .  5�   /   �� *	IsVisible+  �� 	Converter#  [  ��5�   *   �� 5�   .   ��68   �� *
Category+! �� StringFormat$! Hide {0}��5�   '   5�   #   5�       5�      �� 5�       6*   �� *Color+ 6D   Z� .a�5�   "   ��  �@�ContextMenu 6@   Z� 5�      5�      5�      5�      5�      5�      