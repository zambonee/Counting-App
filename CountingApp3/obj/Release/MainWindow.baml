   M S B A M L   `   `   `  ���� G  CCountingApp3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:CountingApp3CountingApp3      CountingApp3.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ,localclr-namespace:CountingApp3       � SizeToContent$  WidthAndHeight=�$	��500�� � Topmost .  � Left$ 0��
 � Top$ 0�� � ShowInTaskbar .   � Icon$2 ,pack://application:,,,/Resources/favicon.ico�� J� 	AllowDrop .  � WindowStyle$ 
ToolWindow=� � DataContext "   CountingApp3.MainDataContext  	 J� CommandBindings	 )  #System.Windows.Input.CommandBinding  -   
   Command$
 ApplicationCommands.Open��  -   $"
 local:CustomCommands.SaveAll�� � 	Resources #   CountingApp3.InverseConverter   InverseConverter  RadioButtonListBoxStyle �� 
TargetType �� Property �� Value-  'System.Windows.Input.KeyboardNavigation J� IsHitTestVisible d� 	IsChecked �� Path �� RelativeSource �� AncestorType �� Mode �� Triggers �� Binding �� Value��%H  &	        &	        �� " ��.���  � ��
 ���� ��  �@ DirectionalNavigation$ Cycle=���  � ��$ Stretch=���  � ��$ Hidden=���  � �� �� .R�d� $
��Left=�
������ 
������  .   �� $ 
IsSelected� �� " ��$ TwoWay=����� " d� ��  �� *	IsVisible+	 False.a���  �@J�Opacity$	 0.2Q���]� .+�A� $��10��$��10��
���   �$��1������ *Brush+���� " A� ��  �� *Color+ Z� .a���  � y�$ 	Collapsed=��� �� ���� *DisplayCategory+$ OneTime=�.���  � RowDefinitions �� $
��Auto ��� $��* �.+�o� $��0��.@�m� ����  �� Orientation$ 
Horizontal=�.+��� $<��6pack://application:,,,/Resources/HamburgerMenu_16x.png���� $
��Menu��.@�m� $
��Open�� m� Command$ ApplicationCommands.Open�� m� Icon �� $9��3pack://application:,,,/Resources/FolderOpen_16x.png��m� $��Save All��$" local:CustomCommands.SaveAll�� �� $6��0pack://application:,,,/Resources/SaveAll_16x.png��m� -   $��Overlay All�� �� $:��4pack://application:,,,/Resources/ImageChisel_16x.png��m� $��Export All Into...�� �� $;��5pack://application:,,,/Resources/ResultToText_16x.png��.@�m� -   $
��File�� �� $;��5pack://application:,,,/Resources/ResultToText_16x.png��m� -   $��Database�� �� $>��8pack://application:,,,/Resources/WriteToDatabase_16x.png��m� -   $��Database Connection�� �� $<��6pack://application:,,,/Resources/AddDataSource_16x.png���� m� $��Show Image Menus�� m� IsCheckable .  m� StaysOpenOnClick .  m� 	IsChecked �� *ShowImageMenus+ �� $8��2pack://application:,,,/Resources/CloakHide_16x.png��m�  .  �� $<��6pack://application:,,,/Resources/BreakPointXHR_16x.png������ $ 
Horizontal=�.+��� $��
Dot Size: ��$��Center=��� -   $��-������  �� *Self+$ ActualHeight��� -   $��+������  �� *Self+$ ActualHeight��� m� -	   $��	Close All�� �� $A��;pack://application:,,,/Resources/CloseDocumentGroup_16x.png��m� -
   $
��Exit�� �� $4��.pack://application:,,,/Resources/Close_16x.png���� $��1��$��Disabled=�$
��Auto=�.���� -   #��[  �� SelectedValue �� *
ActiveTool+���� *CollectionTools+���� " ��.���  � {� �� *	IsVisible+��  �@��ShowOnDisabled . ��  � ��$ 0q���  �@�ToolTip �� *
Category+��  �@�ContextMenu �� .@�m�  .  �� *	IsVisible+  �� 	Converter#  [  ���� ���� *
Category+! �� StringFormat$! Hide {0}�� ��  �� *Color+ Z� .a���  �@�ContextMenu Z� 