   M S B A M L   `   `   `  ����G  CCountingApp3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:CountingApp3CountingApp3  5   	   %    CountingApp3.OleDBStringBuilder   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:CountingApp3     5   	     � Title$  Data Source Connection��5   	   $	��300��6(    � 
ResizeMode$ NoResize=�5	   	    � WindowStyle$ 
ToolWindow=�5
   	    � SizeToContent$ Height=�5   	    � DataContext 5      &    CountingApp3.OleDBBuilderContext  5   
    � 	Resources 5      $   CountingApp3.EnumBoolConverter   EnumBoolConverter �� 
TargetType �� Property �� Value��%�  &	        '��      'G� `     '�� �     '	�       5   
   6"   �� 5   
   " ��6   .�5      ��  � ��$ 0,0,0,5q�6'   5      �� 5   
   " G�6   .�5      ��  � ��$ 0,0,0,5q�6'   5      �� 5   
   " ��6   .�5      ��  � ��$ 50��6)   �� 5       � ��$ 0,0,5,0q�6'   5      �� 5   
   " 	�6   .�5      ��  � ��$ 0,0,5,0q�6'   5      5      .��5      �� $��3q�6   .+�5    
   �� $
��Wrap=�6   ��Create a connection string, including setting your user name and password for this session. Do not save your password to the config file.5#      �� 5$   
   �� 5%   
   Connection Type:6&   �� 5&   
   	 �� Orientation$	 
Horizontal=�6   $��0,0,0,5q�6.   .+�5'      �� .+�5(      	� 
 d� 	IsChecked
 6   �� *Type+ �� 	Converter# [     CountingApp3.TypeOptions   	SQLServer �� ConverterParameter# Z .��6�   
SQL Server6�   	� 5)      
 6   �� *Type+# [     Oracle# Z .��6�   Oracle6�   5*      �� 5+      .+�5,      	� 
 6   �� *Type+# [     
PostgreSQL# Z .��6�   
PostgreSQL6�   	� 5-      
 6   �� *Type+# [     Excel# Z .��6�   
Excel File6�   5.      	� 5/      
 6   �� *Type+# [     OleDBString# Z .��6�   Connection String6�   50      �� 51   
   ��52      �� 53      " ��6   .�54      ��  � y�$ 	Collapsed=�63    �� Triggers 55      �� 56       �� Value# Z 6?    �� Binding 6&   �� *Type+.a�57      ��  � y�$ Visible=�6;   58      5:      .+�5<      �� Connection String:6,   �� 5=       �� TextWrapping$
 Wrap=�63   ��6   �� *OleDBString+63   5>      �� 5?   
   ��5@      �� 5A      " ��6   .�5B      ��  � y�$ Visible=�63    5C      �� 5D      # Z 6?    6&   �� *Type+.a�5E      ��  � y�$ 	Collapsed=�6;   5F      5H      .+�5J      �� .+�5K      �� ��5L      �� 5M      " ��6    .�5N      ��  � ��$ Server:��65    5O      �� 5P   "   # Z 6G    6.   �� *Type+.a�5Q   &   ��  � ��$ 
File Name:��6=   5R   #   5T      5V      �� 5W      ��6   �� *Server+5X      �� 5Y      ��5Z      �� 5[      " ��6   .�5\      ��  � y�$ 	Collapsed=�67    5]      �� 5^      # Z 6C    6*   �� *Type+.a�5_   "   ��  � y�$ Visible=�6?   5`      5b      .+�5d      �� Port:6#   �� 5e      ��6   �� *Port+5f      �� 5g      ��5h      �� 5i      " ��6   .�5j      ��  � y�$ Visible=�67    5k      �� 5l      # Z 6C    6*   �� *Type+.a�5m   "   ��  � y�$ 	Collapsed=�6?   5n      5p      .+�5r      �� ��5s      �� 5t      " ��6    .�5u      ��  � y�$ Visible=�6;    5v      �� 5w   "   # Z 6G    6.   �� *Type+.a�5x   &   ��  � y�$ 	Collapsed=�6C   5y   #   5{      .+�5}      �� 	Database:6+   �� 5~      ��6   �� *
Database+5      �� 5�      .+�5�      �� 20User Name (leave blank for Integrated Security):6R   �� 5�      ��6   �� *
UserName+5�      �� 5�      .+�5�      �� 	Password:6+   G� 5�      -   5        5�   "   5�      5�      5�      �� 5�   
   $	 
Horizontal=�6   $��RightToLeft=�6.   .+�5�      �� $��Cancel��6    �� IsCancel . 6&   �� 5�      -   5        $��Apply��5�      6%   �� 5�      -   5        $
��Save��5�       �� 	IsDefault . 65   5�      5�      5�      