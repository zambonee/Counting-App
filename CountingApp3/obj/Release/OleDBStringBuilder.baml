   M S B A M L   `   `   `  ���� G  CCountingApp3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:CountingApp3CountingApp3  %    CountingApp3.OleDBStringBuilder   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ,localclr-namespace:CountingApp3       � Title$  Data Source Connection��$	��300�� � 
ResizeMode$ NoResize=� � WindowStyle$ 
ToolWindow=� � SizeToContent$ Height=� � DataContext &    CountingApp3.OleDBBuilderContext   � 	Resources $   CountingApp3.EnumBoolConverter   EnumBoolConverter �� 
TargetType �� Property �� Value��%�   &	        '��      'G� -     '�� U     '	� �       �� " ��.���  � ��$ 0,0,0,5q��� " G�.���  � ��$ 0,0,0,5q��� " ��.���  � ��$ 50����  � ��$ 0,0,5,0q��� " 	�.���  � ��$ 0,0,5,0q�.���� $��3q�.+��� $
��Wrap=���Create a connection string, including setting your user name and password for this session. Do not save your password to the config file.�� �� Connection Type:�� 	 �� Orientation$	 
Horizontal=�$��0,0,0,5q�.+��� .+�	� 
 d� 	IsChecked
 �� *Type+ �� 	Converter# [     CountingApp3.TypeOptions   	SQLServer �� ConverterParameter# Z .��
SQL Server	� 
 �� *Type+# [     Oracle# Z .��Oracle�� .+�	� 
 �� *Type+# [     
PostgreSQL# Z .��
PostgreSQL	� 
 �� *Type+# [     Excel# Z .��
Excel File	� 
 �� *Type+# [     OleDBString# Z .��Connection String�� ���� " ��.���  � y�$ 	Collapsed=� �� Triggers ��  �� Value# Z  �� Binding �� *Type+.a���  � y�$ Visible=�.+��� Connection String:��  �� TextWrapping$
 Wrap=����� *OleDBString+�� ���� " ��.���  � y�$ Visible=� �� # Z  �� *Type+.a���  � y�$ 	Collapsed=�.+��� .+��� ���� " ��.���  � ��$ Server:�� �� # Z  �� *Type+.a���  � ��$ 
File Name:���� ���� *Server+�� ���� " ��.���  � y�$ 	Collapsed=� �� # Z  �� *Type+.a���  � y�$ Visible=�.+��� Port:�� ���� *Port+�� ���� " ��.���  � y�$ Visible=� �� # Z  �� *Type+.a���  � y�$ 	Collapsed=�.+��� ���� " ��.���  � y�$ Visible=� �� # Z  �� *Type+.a���  � y�$ 	Collapsed=�.+��� 	Database:�� ���� *
Database+�� .+��� 20User Name (leave blank for Integrated Security):�� ���� *
UserName+�� .+��� 	Password:G� -   �� $	 
Horizontal=�$��RightToLeft=�.+��� $��Cancel�� �� IsCancel . �� -   $��Apply���� -   $
��Save�� �� 	IsDefault . 