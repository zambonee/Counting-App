   M S B A M L   `   `   `  ����G  CCountingApp3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:CountingApp3CountingApp3  5   	       CountingApp3.ImageForm   5   
   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:CountingApp3     5   	     � WindowStyle$  SingleBorderWindow=�5   	    � ShowActivated .   � Icon$2 ,pack://application:,,,/Resources/favicon.ico��5   	    J� 	AllowDrop .  +� FocusedElement 5
   	   ��  �� ElementName$ ImageScrollViewer�� � Title 5   	   �� *	ImageName+ � 	Resources 5      #   CountingApp3.InverseConverter   InverseConverter+   %CountingApp3.EllipsePositionConverter  PositionConverter,   &CountingApp3.BoolToVisibilityConverter  VisibilityConverter-   'CountingApp3.PointsToThicknessConverter  ThicknessConverter)   #CountingApp3.PointsToWidthConverter  WidthConverter*   $CountingApp3.PointsToHeightConverter  HeightConverter3   -CountingApp3.ScaleFactorToPercentageConverter  ZoomConverter*   $CountingApp3.ActualMarkSizeConverter  ActualMarkSizeConverter  DropDownArrow �� 
TargetType	 �� Property
 �� Value��%N  &	        &	      &	 &     &	 9     &	 L     &	 _     &	 r     &	 �     &	 �    '�� �     '�� T    '�� �      5   
   6!     5   
   6)     5   
   6*     5   
   6+     5   
   6'     5   
   6(     5   
   61     5   
   6(   F� 5   
   $��Center=�67   $��3q�6R   
���   �"����        ���- ��- р�[     	6j   �� 5   
   " ��6   .�5      �� 	 �@��Orientation$
 
Horizontal=�6,   5      �� 5   
   " ��6   .�5      �� 	 � ��$
 0q�6(   5      �� 5   
   " ��6   .�5      -   5        5       5!       J� CommandBindings 5"      )	  #System.Windows.Input.CommandBinding	  5#   
   -   5         	  Command$ ApplicationCommands.Open��5#      5$      	  5%   
   -   5        $ ApplicationCommands.Undo��5%      5'      	  5(   
   -   5        $ ApplicationCommands.Redo��5(      5*      	  5+   
   -   5        $  ApplicationCommands.Delete��5+      5-      	  5.   
   -   5        $ ApplicationCommands.Save��5.      5/      	  50   
   -   5        $  ApplicationCommands.SaveAs��50      51      	  52   
   -   5        $" local:CustomCommands.SaveAll��52      53      	  54   
   -	   5        $ ApplicationCommands.Copy��54      56      	  57   
   -
   5        $! local:CustomCommands.Escape��57      58      .��5:      �  � RowDefinitions 5;   
   �� 5<      $
��Auto �6   �� 5=      $
��Auto �6   �� 5>      $��* �6   .+�5@   
   `� $��0��6   y�5A      �� *Parent.ShowImageMenus+ �� 	Converter# [ .�5B      c� .@�5C      �� $��ApplicationCommands.Open��6    � ToolTip$) #Open an image file in a new window.��6<   .��5D      �� $9��3pack://application:,,,/Resources/FolderOpen_16x.png��6   5E      o� 5F      
������ .@�5G      m� $ Save, Save As, or Overlay��6   ��5H      �� 5I      .+�5J   "   �� $3��-pack://application:,,,/Resources/Save_16x.png��6(   �� 5K   "   ��61   ��  �� Source# [ 5L      .@�5N      m� $
��Save��6#    m� Command$ ApplicationCommands.Save��5O   #   $
 Save��5P   #    m� Icon 5Q      �� 5R   "   $3��-pack://application:,,,/Resources/Save_16x.png��6(   5T      m� 5U      $��Save As��6#   $  ApplicationCommands.SaveAs��5V   #   $ 
Save As...��5W   #    5X      �� 5Y   "   $5��/pack://application:,,,/Resources/SaveAs_16x.png��6(   5[      m� 5\      -   5        $��Overlay��5\   #   $= 7Save the count marks and lines embedded into the image.��5^   #    5_      �� 5`   "   $:��4pack://application:,,,/Resources/ImageChisel_16x.png��6(   5b      5c      5d      �� 5e      .+�5f      �� $��40��6   $��0q�5g      $��0q�5h      $" Manually set the zoom amount��5i      ��5j      �� *ScaleFactor+# [  �� UpdateSourceTrigger$ PropertyChanged=� �� FallbackValue$	 100���� 5k      $��%��6    5l      o� 5m      
������ .@�5n      m� $ Zoom options��6   ��5o      �� 5p      .+�5q   "   �� ��61   �� # [ �� 5r   "   $y�Hidden=�6,   5s      .@�5u      m� -   5        $��Fit to window��5u   #   $. (Scale the image to fit inside the window��5v   #    5x      �� 5y   "   $;��5pack://application:,,,/Resources/FitSelection_16x.png��6(   5{      m� 5|      -   5        $��Scale all images��5|   #   $0 *Scale all images to match this zoom amount��5}   #    5      �� 5�   "   $9��3pack://application:,,,/Resources/ImageStack_16x.png��6(   5�      5�      5�      o� 5�      
������ .@�5�      m� $$ Change brightness and contrast��6   ��5�      �� 5�      .+�5�   "   �� $9��3pack://application:,,,/Resources/Brightness_16x.png��6(   �� 5�   "   ��61   �� # [ 5�      .@�5�      m�  5�      �� 5�   "   $9��3pack://application:,,,/Resources/Brightness_16x.png��6(   ��5�      �� 5�   "   -   5         ��Name$ brightnessValue��5�   )    � Minimum$ -1Q�5�   )    � Maximum$ 1Q�5�   )    �� TickFrequency$	 0.1Q�5�   )   $	��100��5�   )   5�      m� 5�       5�      �� 5�   "   $7��1pack://application:,,,/Resources/Contrast_16x.png��6(   ��5�      �� 5�   "   -   5        $ contrastValue��5�   )   $ -1Q�5�   )   $ 1Q�5�   )   $	 0.1Q�5�   )   $	��100��5�   )   5�      m� 5�      -   5        $��Reset��5�   #    m� StaysOpenOnClick .  5�      �� 5�   "   $;��5pack://application:,,,/Resources/RestoreImage_16x.png��6(   5�      5�      5�      5�      c� 5�      .@�5�      �� $��ApplicationCommands.Undo��6   $ Undo Ctrl+Z��5�       ^� ShowOnDisabled . .��5�      �� $3��-pack://application:,,,/Resources/Undo_16x.png��6   ��5�      �� 5�      " ��6     �� Triggers 5�      P� 5�   "    P� Property � {� P� Value .  . �5�   &   �� 	 �@J�Opacity$	
 0.2Q�6@   5�   #   5�      5�      �� 5�      $��ApplicationCommands.Redo��6   $ Redo Ctrl+Y��5�       . .��5�      �� $3��-pack://application:,,,/Resources/Redo_16x.png��6   ��5�      �� 5�      " ��6     5�      P� 5�   "    � {� .  . �5�   &   �� 	 �@J�Opacity$	
 0.2Q�6@   5�   #   5�      5�      �� 5�      $��ApplicationCommands.Copy��6   $- 'Copy the selection as a new file Ctrl+C��5�       . .��5�      �� $>��8pack://application:,,,/Resources/CopyToClipboard_16x.png��6   ��5�      �� 5�      " ��6     5�      P� 5�   "    � {� .  . �5�   &   �� 	 �@J�Opacity$	
 0.2Q�6@   5�   #   5�      5�      �� 5�      $ ��ApplicationCommands.Delete��6   $+ %Delete the selected items Ctrl+Delete��5�       . .��5�      �� $?��9pack://application:,,,/Resources/DeleteBreakpoint_16x.png��6   ��5�      �� 5�      " ��6     5�      P� 5�   "    � {� .  . �5�   &   �� 	 �@J�Opacity$	
 0.2Q�6@   5�   #   5�      5�      5�      c� 5�      ��6   �� *CollectionInputs+��5�      �� 5�      .R�5�      ��  �� Orientation$ 
Horizontal=�6%   $��0,0,3,0q�5�   %   .+�5�      �� $��0,0,3,0q�5�   (   $��Center=�5�   (   ��5�   (   �� *Column+ �� StringFormat$
 {0}:��5�   (   �� 5�      -   5        $	��100��5�   �   ��6&   �� *Value+$ PropertyChanged=�6�   5�      5�      5�      5�      �� 5�   
   $��1��6   ��5�      �� *CollectionCounts+y�5�      �� *Parent.ShowImageMenus+# [ ��5�      �� 5�      .R�5�      �� $ 
Horizontal=�6!   $	��3,0q�6:   .+�5�      �� ��6$   �� *Key+�� 5�      $��: ��6$   �� 5�      ��6$   �� *Value.Value+5      5     5     �� 5  
   -   5          ��Name$  ImageScrollViewer��5     $��2��5     $
��Auto=�5     $
��Auto=�5     ! � Cursor! 5     �� *Parent.ActiveTool.Cursor+" � FocusVisualStyle" 5
     Z� .��5     � -   5        # � LayoutTransform# 5     V� 5     .~�5     �� $ �� ScaleX$ 6)   �� *ScaleFactor+% �� ScaleY% 6H   �� *ScaleFactor+5     .+�5     �� -   5        & ��Name$& ImageElement��5     $
��None=�5     $
��Left=�5     $	��Top=�5     ' �� BitmapScalingMode$' NearestNeighbor=�5     ��5     �� *ActiveImage+( J� Effect( 5      
   CountingApp3.ShaderEffects
  5     ) 
  
Brightness) 6.   �� $ brightnessValue��* �� Path$* Value�+ 
  Contrast+ 5  .   �� $ contrastValue��$* Value�# 5     �� 5      $ 6)   �� *
SubScale+% 6E   �� *
SubScale+5"     �� 5#     ��6   �� *CollectionUIElements+��5$     �� 5%     .R�5&     �� 5'      5)     , �� DataType   CountingApp3.LineMark- `� 	Converter. �� RelativeSource/ �� AncestorType0 �� Binding1 �� Value   CountingApp3.CountMark��%�  (��       5*  '   *, +)7  (��     5A  '   *, +)7  7  7  7  �� 5*     ",  6'   .R�5+     F� ��6#   �� *
Geometry+��5,  "   �� 5-  &   " F�6,   .�5.  *   �� 	 � ��
 5/  .   `� 50  2   8-   .1�51  6   �� $&*  DataContext.Parent.CountDiameter�6>   . 6f   �� "/ ��� 52  6   $* DataContext.ScaleFactor�6>   . 6]   �� "/ ��� 53  6   $ 2��6>   54  3   56  +   �� 57  *   	 � ��

 �  ��6C    58  *   �� 59  .   0 6:   �� *
IsSelected+1 True6Y   .a�5:  2   �� 	 � ��

 �����6K   5;  /   5=  '   5?     5@     �� 5A     ",  6'   .R�5B     A� ��6&   �� *Button.Brush+��5C  "   �� 5D  &   " A�6,   .�5E  *   �� 	 � ��
 5F  .   `� 5G  2   8-   .1�5H  6   �� $&*  DataContext.Parent.CountDiameter�6>   . 6f   �� "/ ��� 5I  6   $* DataContext.ScaleFactor�6>   . 6]   �� "/ �5J  3   5L  +   �� 5M  *   	 � ��
 5N  .   `� 5O  2   8-  .1�5P  6   �� $&*  DataContext.Parent.CountDiameter�6>   . 6f   �� "/ ��� 5Q  6   $* DataContext.ScaleFactor�6>   . 6]   �� "/ �5R  3   5T  +   �� 5U  *   	 � ��
 5V  .   `� 5W  2   8-  .1�5X  6   �� $* Location�6>   �� 5Y  6   $&*  DataContext.Parent.CountDiameter�6>   . 6f   �� "/ ��� 5Z  6   $* DataContext.ScaleFactor�6>   . 6]   �� "/ �5[  3   5]  +   �� 5^  *   	 � ��

 �����5_  5   �� 5`  *   	 � ��$
 0��5a  5    5b  *   �� 5c  .   0 6:   �� *
IsSelected+1 True6Y   .a�5d  2   �� 	 � ��
 5e  6   `� 5f  :   8-  .1�5g  >   �� $&*  DataContext.Parent.CountDiameter�6F   . 6n   �� "/ ��� 5h  >   $* DataContext.ScaleFactor�6F   . 6e   �� $/ WindowL��� 5i  >   $ 4��6F   5j  ;   5l  3   5m  /   �� 5n  .   0 6:   �� *Button.IsVisible+	1 False6_   .a�5o  2   �� 	 � y�$
 	Collapsed=�6O   5p  /   5r  '   5t     5u     5v     5w     �� 5x     $	��Top=�6   $
��Left=�5y  !   
���   �2 �� StrokeDashArray$	2 4 4R�5{  !   y�5|  !   �� *SelectionVisible+# [ ��5}     �� 5~     " ��6    .�5     �� 	 � ��
 5�  "   `� 5�  &   #- [ 63   .1�5�  *   �� $ 5��62   �� 5�  *   $* DataContext.ScaleFactor�62   . 6Q   �� "/ �5�  '   5�     �� 5�     	 � ��
 5�  "   `� 5�  &   #- [ 63   .1�5�  *   �� $* SelectionStart�62   �� 5�  *   $* SelectionEnd�62   5�  '   5�     �� 5�     	 � ��
 5�  "   `� 5�  &   #- [ 63   .1�5�  *   �� $* SelectionStart�62   �� 5�  *   $* SelectionEnd�62   5�  '   5�     �� 5�     	 � ��
 5�  "   `� 5�  &   #- [ 63   .1�5�  *   �� $* SelectionStart�62   �� 5�  *   $* SelectionEnd�62   5�  '   5�     5�     5�     5�     5�     5�     5�     