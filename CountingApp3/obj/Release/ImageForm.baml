   M S B A M L   `   `   `  ���� G  CCountingApp3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:CountingApp3CountingApp3      CountingApp3.ImageForm   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ,localclr-namespace:CountingApp3       � WindowStyle$  SingleBorderWindow=� � ShowActivated .   � Icon$2 ,pack://application:,,,/Resources/favicon.ico�� J� 	AllowDrop .  +� FocusedElement ��  �� ElementName$ ImageScrollViewer�� � Title �� *	ImageName+ � 	Resources #   CountingApp3.InverseConverter   InverseConverter+   %CountingApp3.EllipsePositionConverter  PositionConverter,   &CountingApp3.BoolToVisibilityConverter  VisibilityConverter-   'CountingApp3.PointsToThicknessConverter  ThicknessConverter)   #CountingApp3.PointsToWidthConverter  WidthConverter*   $CountingApp3.PointsToHeightConverter  HeightConverter3   -CountingApp3.ScaleFactorToPercentageConverter  ZoomConverter*   $CountingApp3.ActualMarkSizeConverter  ActualMarkSizeConverter  DropDownArrow �� 
TargetType	 �� Property
 �� Value��%S  &	        &	      &	 
     &	      &	      &	      &	      &	 #     &	 (    '�� p     '�� �     '�� �                     F� $��Center=�$��3q�
���   �"����        ���- ��- р�[     	�� " ��.��� 	 �@��Orientation$
 
Horizontal=��� " ��.��� 	 � ��$
 0q��� " ��.�-    J� CommandBindings )	  #System.Windows.Input.CommandBinding	  -    	  Command$ ApplicationCommands.Open��	  -   $ ApplicationCommands.Undo��	  -   $ ApplicationCommands.Redo��	  -   $  ApplicationCommands.Delete��	  -   $ ApplicationCommands.Save��	  -   $  ApplicationCommands.SaveAs��	  -   $" local:CustomCommands.SaveAll��	  -	   $ ApplicationCommands.Copy��	  -
   $! local:CustomCommands.Escape��.���  � RowDefinitions �� $
��Auto ��� $
��Auto ��� $��* �.+�`� $��0��y��� *Parent.ShowImageMenus+ �� 	Converter# [ .�c� .@��� $��ApplicationCommands.Open�� � ToolTip$) #Open an image file in a new window.��.���� $9��3pack://application:,,,/Resources/FolderOpen_16x.png��o� 
������ .@�m� $ Save, Save As, or Overlay������ .+��� $3��-pack://application:,,,/Resources/Save_16x.png���� ����  �� Source# [ .@�m� $
��Save�� m� Command$ ApplicationCommands.Save��$
 Save�� m� Icon �� $3��-pack://application:,,,/Resources/Save_16x.png��m� $��Save As��$  ApplicationCommands.SaveAs��$ 
Save As...�� �� $5��/pack://application:,,,/Resources/SaveAs_16x.png��m� -   $��Overlay��$= 7Save the count marks and lines embedded into the image.�� �� $:��4pack://application:,,,/Resources/ImageChisel_16x.png���� .+��� $��40��$��0q�$��0q�$" Manually set the zoom amount������ *ScaleFactor+# [  �� UpdateSourceTrigger$ PropertyChanged=� �� FallbackValue$	 100���� $��%��o� 
������ .@�m� $ Zoom options������ .+��� ���� # [ �� $y�Hidden=�.@�m� -   $��Fit to window��$. (Scale the image to fit inside the window�� �� $;��5pack://application:,,,/Resources/FitSelection_16x.png��m� -   $��Scale all images��$0 *Scale all images to match this zoom amount�� �� $9��3pack://application:,,,/Resources/ImageStack_16x.png��o� 
������ .@�m� $$ Change brightness and contrast������ .+��� $9��3pack://application:,,,/Resources/Brightness_16x.png���� ���� # [ .@�m�  �� $9��3pack://application:,,,/Resources/Brightness_16x.png������ -    ��Name$ brightnessValue�� � Minimum$ -1Q� � Maximum$ 1Q� �� TickFrequency$	 0.1Q�$	��100��m�  �� $7��1pack://application:,,,/Resources/Contrast_16x.png������ -   $ contrastValue��$ -1Q�$ 1Q�$	 0.1Q�$	��100��m� -   $��Reset�� m� StaysOpenOnClick .  �� $;��5pack://application:,,,/Resources/RestoreImage_16x.png��c� .@��� $��ApplicationCommands.Undo��$ Undo Ctrl+Z�� ^� ShowOnDisabled . .���� $3��-pack://application:,,,/Resources/Undo_16x.png������ " �� �� Triggers P�  P� Property � {� P� Value .  . ��� 	 �@J�Opacity$	
 0.2Q��� $��ApplicationCommands.Redo��$ Redo Ctrl+Y�� . .���� $3��-pack://application:,,,/Resources/Redo_16x.png������ " �� P�  � {� .  . ��� 	 �@J�Opacity$	
 0.2Q��� $��ApplicationCommands.Copy��$- 'Copy the selection as a new file Ctrl+C�� . .���� $>��8pack://application:,,,/Resources/CopyToClipboard_16x.png������ " �� P�  � {� .  . ��� 	 �@J�Opacity$	
 0.2Q��� $ ��ApplicationCommands.Delete��$+ %Delete the selected items Ctrl+Delete�� . .���� $?��9pack://application:,,,/Resources/DeleteBreakpoint_16x.png������ " �� P�  � {� .  . ��� 	 �@J�Opacity$	
 0.2Q�c� ���� *CollectionInputs+���� .R���  �� Orientation$ 
Horizontal=�$��0,0,3,0q�.+��� $��0,0,3,0q�$��Center=����� *Column+ �� StringFormat$
 {0}:���� -   $	��100������ *Value+$ PropertyChanged=��� $��1������ *CollectionCounts+y��� *Parent.ShowImageMenus+# [ ���� .R��� $ 
Horizontal=�$	��3,0q�.+��� ���� *Key+�� $��: ���� ���� *Value.Value+�� -     ��Name$  ImageScrollViewer��$��2��$
��Auto=�$
��Auto=�! � Cursor! �� *Parent.ActiveTool.Cursor+" � FocusVisualStyle" Z� .��� -   # � LayoutTransform# V� .~��� $ �� ScaleX$ �� *ScaleFactor+% �� ScaleY% �� *ScaleFactor+.+��� -   & ��Name$& ImageElement��$
��None=�$
��Left=�$	��Top=�' �� BitmapScalingMode$' NearestNeighbor=����� *ActiveImage+( J� Effect(  
   CountingApp3.ShaderEffects
  ) 
  
Brightness) �� $ brightnessValue��* �� Path$* Value�+ 
  Contrast+ �� $ contrastValue��$* Value�# �� $ �� *
SubScale+% �� *
SubScale+�� ���� *CollectionUIElements+���� .R���  , �� DataType   CountingApp3.LineMark- `� 	Converter. �� RelativeSource/ �� AncestorType0 �� Binding1 �� Value   CountingApp3.CountMark��%�  (��       *, +)7  (�� ,    *, +)7  7  7  7  �� ",  .R�F� ���� *
Geometry+���� " F�.��� 	 � ��
 `� 8-   .1��� $&*  DataContext.Parent.CountDiameter�. �� "/ ��� $* DataContext.ScaleFactor�. �� "/ ��� $ 2���� 	 � ��

 �  �� �� 0 �� *
IsSelected+1 True.a��� 	 � ��

 ������� ",  .R�A� ���� *Button.Brush+���� " A�.��� 	 � ��
 `� 8-   .1��� $&*  DataContext.Parent.CountDiameter�. �� "/ ��� $* DataContext.ScaleFactor�. �� "/ ��� 	 � ��
 `� 8-  .1��� $&*  DataContext.Parent.CountDiameter�. �� "/ ��� $* DataContext.ScaleFactor�. �� "/ ��� 	 � ��
 `� 8-  .1��� $* Location��� $&*  DataContext.Parent.CountDiameter�. �� "/ ��� $* DataContext.ScaleFactor�. �� "/ ��� 	 � ��

 ������� 	 � ��$
 0�� �� 0 �� *
IsSelected+1 True.a��� 	 � ��
 `� 8-  .1��� $&*  DataContext.Parent.CountDiameter�. �� "/ ��� $* DataContext.ScaleFactor�. �� $/ WindowL��� $ 4���� 0 �� *Button.IsVisible+	1 False.a��� 	 � y�$
 	Collapsed=��� $	��Top=�$
��Left=�
���   �2 �� StrokeDashArray$	2 4 4R�y��� *SelectionVisible+# [ ���� " ��.��� 	 � ��
 `� #- [ .1��� $ 5���� $* DataContext.ScaleFactor�. �� "/ ��� 	 � ��
 `� #- [ .1��� $* SelectionStart��� $* SelectionEnd��� 	 � ��
 `� #- [ .1��� $* SelectionStart��� $* SelectionEnd��� 	 � ��
 `� #- [ .1��� $* SelectionStart��� $* SelectionEnd�