using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.IO;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace CountingApp3
{
    /// <summary>
    /// Interaction logic for ImageForm.xaml
    /// </summary>
    public partial class ImageForm : Window
    {
        public ImageFormContext context;

        /// <summary>
        /// True when user clicks inside the image area, and the area captures the mouse events. False when the user releases the mouse button, and the mouse events are released.
        /// </summary>
        private bool isDrawing = false;

        /// <summary>
        /// Keep track of dragging with the Move tool in the view-model layer. Cannot offset a ScrollViewer with bindings.
        /// </summary>
        private Point startPoint = new Point(0, 0);

        /// <summary>
        /// Set to true when the Move tool is used to move count marks instead of the viewport.
        /// </summary>
        private bool isMoving = false;

        /// <summary>
        /// List of CountMarks and LineMarks with their old locations (if moved) where isSelected = true.
        /// Add to this with the Select Tool to improve performance of the Delete command and the Move Tool.
        /// </summary>
        private Dictionary<Mark, Point> selection = new Dictionary<Mark, Point>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">FilePath for the image file.</param>
        /// <param name="parent">The MainWindow.</param>
        public ImageForm(string path, MainDataContext parent)
        {   
            // If there is a default window size, change it.
            // Do not set Window.Width and .Height in XAML- it will render after this.
            if (parent.DefaultWindowSize.HasValue)
            {
                Width = parent.DefaultWindowSize.Value.Width;
                Height = parent.DefaultWindowSize.Value.Height;
            }
            // Default window size.
            else
            {
                Width = 565;
                Height = 400;
            }            context = new ImageFormContext(path, parent);
            DataContext = context;
            InitializeComponent();
            // Routed commands to change tools. Do this after InitializeComponent() so it can reference the UI Element.
            context.Parent.AddKeyBindings(ImageScrollViewer, context.Parent.CollectionTools);
        }

        /// <summary>
        /// After the content is rendered, set ScaleFactor to its default.
        /// Cannot do this before the window size is set.
        /// </summary>
        protected override void OnContentRendered(EventArgs e)
        {
            // If there is a default ScaleFactor, start there. Otherwise, set it so that the whole image fits in the window.
            if (context.Parent.DefaultScaleFactor.HasValue)
            {
                context.ScaleFactor = context.Parent.DefaultScaleFactor.Value;
            }
            else
            {
                FitToWindow();
            }
            base.OnContentRendered(e);
        }

        /// <summary>
        /// Start of the selected tool use.
        /// </summary>
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point location = e.GetPosition(sender as UIElement);
            // Capture the other mouse events inside the UI Element to avoid some awkward behaviors when the user drags outside of the frame.
            (sender as UIElement).CaptureMouse();
            // Select and Zoom tool
            if (context.Parent.ActiveTool == MainDataContext.ToolSelect || context.Parent.ActiveTool == MainDataContext.ToolZoom)
            {
                context.SelectionVisible = false;
                context.SelectionStart = location;
                context.SelectionEnd = location;
            }
            // Move tool
            else if (context.Parent.ActiveTool == MainDataContext.ToolMove)
            {
                startPoint = e.GetPosition(ImageScrollViewer);
                // If button down over a selected count, move the selected counts rather than the viewport.                
                isMoving = false;
                if (selection.Count > 0)
                {
                    double minX = location.X - context.Parent.CountDiameter / context.ScaleFactor / 2;
                    double minY = location.Y - context.Parent.CountDiameter / context.ScaleFactor / 2;
                    double maxX = location.X + context.Parent.CountDiameter / context.ScaleFactor / 2;
                    double maxY = location.Y + context.Parent.CountDiameter / context.ScaleFactor / 2;
                    foreach (Mark m in selection.Keys)
                    {
                        CountMark c = m as CountMark;
                        if (c != null)
                        {
                            if (c.Button.IsVisible
                                && minX <= c.Location.X && c.Location.X <= maxX
                                && minY <= c.Location.Y && c.Location.Y <= maxY)
                            {
                                isMoving = true;
                                break;
                            }
                        }
                    }
                }
            }
            // Draw tool
            else if (context.Parent.ActiveTool == MainDataContext.ToolDraw)
            {
                context.AddLine(location);
            }
            // One of the Count tools
            else if (context.Parent.ActiveTool.IsVisible)
            {
                context.AddCount(location, context.Parent.ActiveTool);
            }
            isDrawing = true;
        }

        /// <summary>
        /// Moving the current tool around.
        /// </summary>
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {            
            FrameworkElement grid = sender as FrameworkElement; // Keeping as much of the UI elements anonymous.
            Point location = e.GetPosition(sender as UIElement);
            if (!isDrawing)
                return;
            // Select and Zoom
            else if (context.Parent.ActiveTool == MainDataContext.ToolSelect || context.Parent.ActiveTool == MainDataContext.ToolZoom)
            {
                context.SelectionEnd = location;
                context.SelectionVisible = true;
            }
            // Move
            else if (context.Parent.ActiveTool == MainDataContext.ToolMove)
            {
                location = e.GetPosition(ImageScrollViewer);
                double x = startPoint.X - location.X;
                double y = startPoint.Y - location.Y;
                // Moving counts.
                if (isMoving)
                {
                    // Iterate twice- first to make sure no counts will be out of range, then to change all selected counts.
                    bool flag = false;                    
                    foreach (KeyValuePair<Mark, Point> m in selection)
                    {
                        CountMark c = m.Key as CountMark;
                        if (c != null)
                        {
                            Point p = new Point(c.Location.X - x, c.Location.Y - y);
                            if (p.X <= 0 || p.Y <= 0 || p.X >= grid.ActualWidth || p.Y >= grid.ActualHeight)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (!flag)
                    {
                        foreach (Mark m in selection.Keys)
                        {
                            CountMark c = m as CountMark;
                            if (c != null)
                            {
                                Point p = new Point(c.Location.X - x, c.Location.Y - y);
                                c.Location = p;
                            }
                        }
                        // Change startPoint, otherwise, point will move exponentially with the mouse.
                        startPoint = location;
                    }
                }
                // Scrolling
                else
                {
                    ImageScrollViewer.ScrollToHorizontalOffset(ImageScrollViewer.HorizontalOffset + x);
                    ImageScrollViewer.ScrollToVerticalOffset(ImageScrollViewer.VerticalOffset + y);
                    // Offset ignores negative values, so do not need to handle it.
                    startPoint = location;
                }
            }
            // Draw
            else if (context.Parent.ActiveTool == MainDataContext.ToolDraw)
            {
                // Only draw within the lines!
                Point p = new Point(location.X, location.Y);
                if (p.X < 0)
                {
                    p.X = 0;
                }
                else if (p.X > grid.ActualWidth)
                {
                    p.X = grid.ActualWidth;
                }
                if (p.Y < 0)
                {
                    p.Y = 0;
                }
                else if (p.Y > grid.ActualHeight)
                {
                    p.Y = grid.ActualHeight;
                }
                context.AddPoint(location);
            }
        }

        /// <summary>
        /// End of the current tool use.
        /// </summary>
        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Release the mouse capture set in MouseDown.
            (sender as UIElement).ReleaseMouseCapture();
            if (!isDrawing)
                return;
            isDrawing = false;
            // Select
            if (context.Parent.ActiveTool == MainDataContext.ToolSelect)
            {
                if (context.CollectionUIElements.Count < 1)
                    return;
                double minX = 0;
                double minY = 0;
                double maxX = 0;
                double maxY = 0;
                bool single = false;
                double threshold = context.Parent.CountDiameter / context.ScaleFactor / 2;
                // Add to selection when control key is pressed.
                bool addSingle = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) ? true : false;
                if (context.SelectionVisible)
                {
                    minX = Math.Min(context.SelectionStart.X, context.SelectionEnd.X);
                    minY = Math.Min(context.SelectionStart.Y, context.SelectionEnd.Y);
                    maxX = Math.Max(context.SelectionStart.X, context.SelectionEnd.X);
                    maxY = Math.Max(context.SelectionStart.Y, context.SelectionEnd.Y);
                }
                // When the mouse has not moved, but sometimes the mouse moves without really moving.
                if (!context.SelectionVisible || (maxX - minX < 2 && maxY - minY < 2))
                {
                    // Range of a single click in CountDiameter / 2 around the MouseUp location.
                    minX = context.SelectionStart.X - threshold;
                    minY = context.SelectionStart.Y - threshold;
                    maxX = context.SelectionStart.X + threshold;
                    maxY = context.SelectionStart.Y + threshold;
                    single = true;
                }
                // Do 2 searches- after and then before StartSelectSearch as an index for CollectionUIElements, with the latter including StartSelectSearch.
                // That way, if a user selects a single elements without the selection Rectangle, a second click will select the next element in the range when relevant.
                // If only a single element can be selected, flag = true when one is selected and the rest will be unselected.
                int i = context.StartSelectSearch;
                int newSelection = i;
                bool flag = false;
                if (!addSingle)
                    selection.Clear();
                do
                {
                    i++;
                    if (i >= context.CollectionUIElements.Count)
                        i = 0;
                    CountMark m = context.CollectionUIElements[i] as CountMark;
                    if (m != null)
                    {
                        if (!flag
                            && m.Button.IsVisible
                            && minX <= m.Location.X && m.Location.X <= maxX
                            && minY <= m.Location.Y && m.Location.Y <= maxY)
                        {
                            if (m.IsSelected && single)
                            {
                                m.IsSelected = false;
                                selection.Remove(m);
                            }
                            else
                            {
                                m.IsSelected = true;
                                selection[m] = new Point(m.Location.X, m.Location.Y);
                                newSelection = i;
                            }
                            if (single)
                                flag = true;
                        }
                        else if (!addSingle)
                        {
                            m.IsSelected = false;
                        }
                    }
                    LineMark l = context.CollectionUIElements[i] as LineMark;
                    if (single && l != null)
                    {
                        PathGeometry p1 = l.Geometry.GetWidenedPathGeometry(new Pen(Brushes.Black, threshold));
                        PathGeometry p2 = PathGeometry.CreateFromGeometry(new EllipseGeometry(e.GetPosition(sender as UIElement), threshold, threshold));
                        if (!flag && p1.FillContainsWithDetail(p2) != IntersectionDetail.Empty)
                        {
                            if (l.IsSelected && single)
                            {
                                l.IsSelected = false;
                                selection.Remove(l);
                            }
                            else
                            {
                                l.IsSelected = true;
                                selection[l] = new Point();
                                newSelection = i;
                            }
                            if (single)
                                flag = true;
                        }
                        else if (!addSingle)
                            l.IsSelected = false;
                    }
                }
                while (i != context.StartSelectSearch);
                // Reset StartSelectSearch to the current selection.
                context.StartSelectSearch = newSelection;
                // Keep the selection Rectangle visible- users may want it to linger.
            }
            // Zoom
            else if (context.Parent.ActiveTool == MainDataContext.ToolZoom)
            {
                context.FitToWindow = false;
                bool usedBox = context.SelectionVisible;
                // Hide the Selection Rectangle here, before the workflows separate.
                context.SelectionVisible = false;
                if (usedBox)
                {
                    double minX = Math.Min(context.SelectionStart.X, context.SelectionEnd.X);
                    double minY = Math.Min(context.SelectionStart.Y, context.SelectionEnd.Y);
                    double maxX = Math.Max(context.SelectionStart.X, context.SelectionEnd.X);
                    double maxY = Math.Max(context.SelectionStart.Y, context.SelectionEnd.Y);
                    // Selection box used for zoom.
                    // Sometimes the mouse moves without really moving.
                    if (maxX - minX > 2 && maxY - minY > 2)
                    {
                        double scaleX = ImageScrollViewer.ActualWidth / (maxX - minX);
                        double scaleY = ImageScrollViewer.ActualHeight / (maxY - minY);
                        double scale = Math.Min(scaleX, scaleY);
                        context.ScaleFactor = scale;
                        ImageScrollViewer.ScrollToHorizontalOffset(minX * context.ScaleFactor);
                        ImageScrollViewer.ScrollToVerticalOffset(minY * context.ScaleFactor);
                        return;
                    }
                }
                // If a selection box was used, method has already returned.
                bool zoomOut = false;
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    zoomOut = true;
                Point viewportLocation = e.GetPosition(ImageScrollViewer);
                Point absoluteLocation = e.GetPosition(sender as UIElement); // Keeping the WPF Elements as anonymous as I can.
                ZoomWithMouse(zoomOut, viewportLocation, absoluteLocation);
            }
            // Move tool is being used to re-locate count marks.
            else if (isMoving)
            {
                // Counts have already been moved. Just have to create the commands for Undo/Redo.
                context.MoveCounts(selection);
                isMoving = false;
            }
            // No actions for Draw.
        }

        /// <summary>
        /// Zoom in and out.
        /// </summary>
        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            context.FitToWindow = false;
            Point viewportLocation = e.GetPosition(ImageScrollViewer);
            Point absoluteLocation = e.GetPosition(sender as UIElement);
            bool zoomOut = false;
            if (e.Delta < 0)
                zoomOut = true;
            ZoomWithMouse(zoomOut, viewportLocation, absoluteLocation);
        }

        /// <summary>
        /// Zoom out when the Zoom tool is selected. Unfortunately, this means that the user cannot use the ContextMenu while the Zoom tool is active.
        /// </summary>
        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (context.Parent.ActiveTool == MainDataContext.ToolZoom)
            {
                e.Handled = true;
                Point viewportLocation = e.GetPosition(ImageScrollViewer);
                Point absoluteLocation = e.GetPosition(sender as UIElement);
                ZoomWithMouse(true, viewportLocation, absoluteLocation);
            }
        }

        /// <summary>
        /// Change the ScaleFactor up or down to the next pre-defined step.
        /// </summary>
        /// <param name="zoomOut">True if zooming out (decreasing ScaleFactor)</param>
        /// <param name="viewportLocation">Mouse cursor location relative to the ScrollViewer</param>
        /// <param name="absoluteLocation">Mouse cursor location relative to the Grid</param>
        private void ZoomWithMouse(bool zoomOut, Point viewportLocation, Point absoluteLocation)
        {
            int i = 0;
            if (zoomOut)
            {
                i = ImageFormContext.ZoomLevels.Length - 1;
                while (i > 0 && context.ScaleFactor <= ImageFormContext.ZoomLevels[i])
                {
                    i--;
                }
            }
            else
            {
                while (i < ImageFormContext.ZoomLevels.Length - 1 && context.ScaleFactor >= ImageFormContext.ZoomLevels[i])
                {
                    i++;
                }
            }
            double targetScaleFactor = ImageFormContext.ZoomLevels[i];
            // Find the minimum ScaleFactor that will keep the image at the ScrollViewer boundaries.
            double width = ImageElement.ActualWidth * targetScaleFactor;
            double height = ImageElement.ActualHeight * targetScaleFactor;
            if (width < ImageScrollViewer.ActualWidth && height < ImageScrollViewer.ActualHeight)
            {
                width = ImageScrollViewer.ActualWidth / ImageElement.ActualWidth;
                height = ImageScrollViewer.ActualHeight / ImageElement.ActualHeight;
                targetScaleFactor = width < height ? width : height;
                // Have to force the scroll bars to be hidden, otherwise the vertical scrollbar messes with the horizontal and v.v.
                ImageScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                ImageScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
            // Set the final ScaleFactor.
            context.ScaleFactor = targetScaleFactor;
            // Bring back the scroll bars now that the ScaleFactor has been calculated.
            ImageScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            ImageScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            // Offset ScrollViewer so that the zoom keeps the same click position under the mouse cursor.
            double x = absoluteLocation.X * context.ScaleFactor;
            double y = absoluteLocation.Y * context.ScaleFactor;
            x -= viewportLocation.X;
            y -= viewportLocation.Y;
            ImageScrollViewer.ScrollToHorizontalOffset(x);
            ImageScrollViewer.ScrollToVerticalOffset(y);
        }

        /// <summary>
        /// Keep the latest image full-resolution and change all the rest to low-resolution.
        /// </summary>
        protected override void OnActivated(EventArgs e)
        {
            foreach (Window w in Application.Current.Windows)
            {
                ImageForm form = w as ImageForm;
                if (form == null || form == this)
                {
                    continue;
                }
                form.context.ActiveImage = form.context.ThumbnailImage;
            }
            UpdateImage();
            base.OnActivated(e);
        }

        /// <summary>
        /// Asynchronously load the full-resolution image. Do not do this in the model layer, otherwise the UI will freeze while loading.
        /// </summary>
        private async void UpdateImage()
        {
            context.ActiveImage = await ImageFormContext.LoadFullResImage(context.FilePath);
        }

        /// <summary>
        /// Open an image file in a new window, handled by the DataContext of the main window.
        /// </summary>
        private void CommandOpen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            context.Parent.OpenFile();
        }

        /// <summary>
        /// Drop files into this window like in the MainWindow to open them in the App.
        /// </summary>
        protected override void OnDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files.Length > 0)
                {
                    context.Parent.OpenFile(files);
                }
            }
        }

        /// <summary>
        /// Image-specific undo command.
        /// </summary>
        private void CommandUndo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            context.Undo();
        }

        /// <summary>
        /// Cannot execute the undo command if there are no actions or user has undone all the way to the beginning.
        /// </summary>
        private void CommandUndo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (context.CollectionCommands.Count > 0 && context.CursorCommands > -1)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        /// <summary>
        /// Image-specific redo command.
        /// </summary>
        private void CommandRedo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            context.Redo();
        }

        /// <summary>
        /// Cannot execute the redo command if there are no actions or user has not undone any of them since adding an action.
        /// </summary>
        private void CommandRedo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (context.CollectionCommands.Count > 0 && context.CursorCommands < context.CollectionCommands.Count - 1)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        /// <summary>
        /// Delete selected marks and lines from the image.
        /// </summary>
        private void CommandDelete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            context.RemoveElements(selection.Keys.ToArray());
            selection.Clear();
        }
        
        /// <summary>
        /// Cannot delete if nothing is selected. Use the selection field to keep track of selected elements rather than LINQing through context.CollectionUIElements for performance.
        /// </summary>
        private void CommandDelete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (selection.Count > 0)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        /// <summary>
        /// Save the data from this image only as an XML.
        /// </summary>
        private void CommandSave_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            context.Parent.SaveFile(this);
        }

        /// <summary>
        /// Can change the file name and directory for a single image at a time.
        /// </summary>
        private void CommandSaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string file = Path.GetFileNameWithoutExtension(context.XMLPath);
            string directory = Path.GetDirectoryName(context.XMLPath);
            // Keep track of the image path inside the XML file so that they can be in different directories.
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = file;
            dialog.Filter = "Data File|*.xml|All Files|*.*";
            dialog.DefaultExt = ".xml";
            dialog.AddExtension = true;
            dialog.InitialDirectory = directory;
            dialog.CheckPathExists = true;
            // User cancelled out of this command.
            if (dialog.ShowDialog() != true)
            {
                return;
            }
            context.XMLPath = dialog.FileName;
            // Save this with its new name and path.
            context.Parent.SaveFile(this);
        }

        /// <summary>
        /// Copy the cropped image inside the selection box and save it as a new image file. Because the image may be scaled or loading from a low-resolution copy, use the original image. 
        /// This may take more memory, but it avoids the issues of re-scaling the image, which is difficult because CroppedBitmap is in pixel units, and waiting for the full-resolution image to load.
        /// </summary>
        private void CommandCopy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Title = "Save cropped image as...";
            dialog.Filter = "JPG Image|*.jpg";
            dialog.DefaultExt = "jpg";
            dialog.AddExtension = true;
            dialog.OverwritePrompt = false;
            dialog.FileName = $"{Path.GetFileNameWithoutExtension(context.XMLPath)}_cropped";
            bool? result = dialog.ShowDialog();
            if (result != true)
                return;
            string fileName = dialog.FileName;
            int i = 1;
            while (File.Exists(fileName))
            {
                fileName = Path.Combine(Path.GetDirectoryName(dialog.FileName), $"{Path.GetFileNameWithoutExtension(dialog.FileName)} ({i}).jpg");
                i++;
            }
            // Re-loads the original image file. This takes more memory, but cannot crop from ImageGrid.Source because it may be resized in terms of pixels, and CroppedBitmap uses pixel dimensions, and it is difficult to back-calculate pixels.
            // If I end up changing this back to original = ImageGrid.Source, have to wait for workerLoadImage to finish. eg- while (workerLoadImage.IsBusy)...
            using (Stream streamRead = File.OpenRead(context.FilePath))
            {
                BitmapDecoder decoder = BitmapDecoder.Create(streamRead, BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnDemand);
                BitmapSource original = decoder.Frames[0] as BitmapSource;
                double resolutionX = original.PixelWidth / ImageElement.ActualWidth;
                double resolutionY = original.PixelHeight / ImageElement.ActualHeight;
                int x = (int)(context.SelectionStart.X * resolutionX);
                int y = (int)(context.SelectionStart.Y * resolutionY);
                int w = (int)((context.SelectionEnd.X - context.SelectionStart.X) * resolutionX);
                int h = (int)((context.SelectionEnd.Y - context.SelectionStart.Y) * resolutionY);
                BitmapSource cropped = new CroppedBitmap(original, new Int32Rect(x, y, w, h));
                BitmapFrame newFrame = BitmapFrame.Create(cropped);
                JpegBitmapEncoder encoder = new JpegBitmapEncoder() { QualityLevel = 100 };
                encoder.Frames.Add(newFrame);
                using (Stream streamWrite = File.Create(fileName))
                {
                    encoder.Save(streamWrite);
                }
            }
        }

        /// <summary>
        /// Can copy the image inside the selection rectangle only if the rectangle is visible.
        /// </summary>
        private void CommandCopy_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // Sometimes SelectionVisible is true but the rectangle is really small.
            if (context.SelectionVisible && context.SelectionEnd.X - context.SelectionStart.X > 5 && context.SelectionEnd.Y - context.SelectionStart.Y > 5)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }

        }

        private void MenuOverlay_Click(object sender, RoutedEventArgs e)
        {
            context.Parent.SaveOverlay(this);
        }

        /// <summary>
        /// Switch between no brightness and contrast change, and the last used settings.
        /// </summary>
        private void MenuResetBAndR_Click(object sender, RoutedEventArgs e)
        {
            if (brightnessValue.Value == 0 && contrastValue.Value == 0)
            {
                brightnessValue.Value = brightness;
                contrastValue.Value = contrast;
            }
            else
            {
                brightness = brightnessValue.Value;
                contrast = contrastValue.Value;
                brightnessValue.Value = 0;
                contrastValue.Value = 0;
            }
        }

        // Last used brightness and contrast to switch between with the shader reset button.
        private double brightness = 0;
        private double contrast = 0;

        /// <summary>
        /// Save all images.
        /// </summary>
        private void CommandSaveAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            context.Parent.SaveFile(context.Parent.GetAllWindows());
        }

        /// <summary>
        /// Resize the image (change context.ScaleFactor) so that it fits inside the window.
        /// </summary>
        private void MenuFitToWindow_Click(object sender, RoutedEventArgs e)
        {
            FitToWindow();
        }

        /// <summary>
        /// Resize the image (change context.ScaleFactor) so that it fits inside the window.
        /// Adjust for the thumbnail image being active when the window first opens.
        /// </summary>
        private void FitToWindow()
        {
            double width = ImageScrollViewer.ActualWidth / ImageElement.ActualWidth;
            double height = ImageScrollViewer.ActualHeight / ImageElement.ActualHeight;
            if (context.ActiveImage == context.ThumbnailImage)
            {
                width /= context.SubScale;
                height /= context.SubScale;
            }
            context.ScaleFactor = width <= height ? width : height;
        }

        /// <summary>
        /// Match all ScaleFactors with this window's.
        /// </summary>
        private void MenuFitAll_Click(object sender, RoutedEventArgs e)
        {
            context.Parent.ScaleAllWindows(this);
        }

        /// <summary>
        /// If changes were made without saving, bring up a dialog to save those changes. User can save, decline to save, or cancel out of closing this window.
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (context.HasChanges)
            {
                MessageBoxResult result = MessageBox.Show($"Save changes to {context.ImageName}?", "Save Changes", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    context.Parent.SaveFile(this);
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
            base.OnClosing(e);
        }

        /// <summary>
        /// Hide the selection rectangle when the Escape key is pressed.
        /// </summary>
        private void CommandEscape_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            context.SelectionVisible = false;
        }

        /// <summary>
        /// Resize the image as the window size changes as long as the image is smaller or equal to the window size.
        /// Do this here rather than in a size change override because the ImageScrollViewer size can change without the window changing size.
        /// </summary>
        private void ImageScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (context == null || ImageElement.Source == null || context.ScaleFactor == 0)
            {
                return;
            }
            // Calculate the displayed image size using the ScaleFactor. 
            // When resize is quick, sometimes the image becomes magically larger than e.PreviousSize. But when you subtract a small margin from the dimensions, this problem goes away.
            double width = (ImageElement.ActualWidth * context.ScaleFactor);
            double height = (ImageElement.ActualHeight * context.ScaleFactor);
            if (width - 10 <= e.PreviousSize.Width && height - 10 <= e.PreviousSize.Height)
            {
                width = ImageScrollViewer.ActualWidth / ImageElement.ActualWidth / context.SubScale;
                height = ImageScrollViewer.ActualHeight / ImageElement.ActualHeight / context.SubScale;
                context.ScaleFactor = width <= height ? width : height;
            }
        }

        /// <summary>
        /// Set HasChanges to true when a text input has changed. This only triggers on user input.
        /// </summary>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            context.HasChanges = true;
        }

        /// <summary>
        /// Change the behavior of the Return key.
        /// </summary>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ImageScrollViewer.Focus();
            }
        }
    }

    public class ImageFormContext : INotifyPropertyChanged
    {
        public MainDataContext Parent { get; }

        /// <summary>
        /// The full path for the image. Load a low-resolution copy of the image and XML elements if applicable.
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                // When opening an XML file, look for an image file stored as an XAttribute.
                if (Path.GetExtension(value).ToLower() == ".xml")
                {
                    XMLPath = value;
                    XDocument doc = XDocument.Load(XMLPath);
                    // Get the image path if the user opened an XML file.
                    XAttribute fp = doc.Root.Attribute("FilePath");
                    _filePath = fp.Value;
                }
                else
                {
                    _filePath = value;
                    XMLPath = Path.Combine(Path.GetDirectoryName(value), $"{Path.GetFileNameWithoutExtension(value)}.xml");
                }
                double width;
                double height;
                int pixelWidth;
                int pixelHeight;
                // Get metadata and the original image dimensions.
                using (Stream stream = File.OpenRead(FilePath))
                {
                    BitmapSource source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    width = source.Width;
                    height = source.Height;
                    pixelWidth = source.PixelWidth;
                    pixelHeight = source.PixelHeight;
                    BitmapMetadata meta = (BitmapMetadata)source.Metadata;
                    if (meta != null)
                    {
                        foreach (KeyValuePair<string, MetadataTag> pair in MetadataTagList)
                        {
                            object o = null;
                            foreach (string query in pair.Value.Queries)
                            {
                                o = meta.GetQuery(query);
                                if (o != null)
                                {
                                    if (pair.Value.Format == typeof(DateTime))
                                    {
                                        DateTime parsedDate;
                                        if (DateTime.TryParseExact(o.ToString(), "yyyy:MM:dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out parsedDate))
                                        {
                                            o = parsedDate;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            CollectionImageValues.Add(pair.Key, o);
                        }
                    }
                }
                // Windows attributes
                CollectionImageValues["FileName"] = Path.GetFileNameWithoutExtension(FilePath);
                CollectionImageValues["Computer"] = Environment.MachineName;
                CollectionImageValues["Directory"] = Path.GetDirectoryName(FilePath);
                CollectionImageValues["DateModified"] = File.GetLastWriteTime(FilePath);
                CollectionImageValues["UserName"] = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                CollectionImageValues["CurrentDate"] = DateTime.Now.Date;
                // DateCreated is in the image metadata.
                if (!CollectionImageValues.ContainsKey("DateCreated") || CollectionImageValues["DateCreated"] == null)
                {
                    CollectionImageValues["DateCreated"] = File.GetCreationTime(FilePath);
                }
                // Separate streams for the image and metadata. This simplifies the code and should actually speed up loading rather than using a stream source to create a BitmapFrame and re-drawing it as a smaller image.
                ThumbnailImage = new BitmapImage();
                ThumbnailImage.BeginInit();
                ThumbnailImage.CacheOption = BitmapCacheOption.OnLoad;
                ThumbnailImage.UriSource = new Uri(FilePath);
                // May have to change the thumbnail pixel size.
                ThumbnailImage.DecodePixelWidth = pixelWidth > 1280 ? 1280 : pixelWidth;
                ThumbnailImage.EndInit();
                ThumbnailImage.Freeze();
                // Rescale the low-resolution image so that it is the original unit-less dimensions.
                thumbnailScale = width / ThumbnailImage.Width;
                // Set the ActiveImage as ThumbnailImage because the window is not focused when created and this saves system memory when opening multiple images simultaneously.
                ActiveImage = ThumbnailImage;
                // Load the XML elements.
                if (File.Exists(XMLPath))
                {
                    XDocument doc = XDocument.Load(XMLPath);
                    // Text Inputs as attributes- override values brought in from main. Do not bring in attributes not set in the configuration file- it will just cause confusion.
                    IEnumerable<XAttribute> inputs = doc.Root.Descendants("TextInputs").Attributes();
                    foreach (XAttribute att in inputs)
                    {
                        for (int i = 0; i < CollectionInputs.Count; i++)
                        {
                            ColumnValuePair pair = CollectionInputs[i];
                            if (pair.Column.Equals(att.Name.ToString()))
                            {
                                pair.Value = att.Value;
                                break;
                            }
                        }
                    }
                    // UI Elements
                    IEnumerable<XElement> elements = doc.Root.Descendants("UIElements").Descendants();
                    foreach (XElement el in elements)
                    {
                        Mark m = null;
                        if (el.Name == "CountMark")
                        {
                            string category = el.Attribute("Category").Value;
                            Point location = Point.Parse(el.Attribute("Location").Value);
                            ButtonType button = null;
                            // Add a count button when a count type is introduced that was not defined in config.txt.
                            foreach (ButtonType b in Parent.CollectionTools)
                            {
                                if (b.Category == category)
                                {
                                    button = b;
                                    break;
                                }
                            }
                            if (button == null)
                            {
                                button = new ButtonType(category, Colors.Red, Cursors.None);
                                Parent.CollectionTools.Add(button);
                            }
                            m = new CountMark(button, location);
                        }
                        else if (el.Name == "LineMark")
                        {
                            string v = el.Attribute("Geometry").Value;
                            m = new LineMark(v);
                        }
                        CollectionUIElements.Add(m);
                    }
                }
                // Without this garbage collector, the loaded images hang in memory when multiple images are opened simultaneously.
                GC.Collect();
            }
        }
        private string _filePath = string.Empty;

        public string XMLPath = string.Empty;
        
        /// <summary>
        /// No resolution image to show when the window is not active.
        /// </summary>
        public BitmapImage ThumbnailImage { get; private set; }

        /// <summary>
        /// The source for the Image element. Switch between low and high resolution copies when windows activated changes. Change Subscale based on which resolution is used so that both are displayed as the same screen size.
        /// </summary>
        public BitmapImage ActiveImage
        {
            get { return _activeImage; }
            set
            {
                _activeImage = value;
                if (value == ThumbnailImage)
                {
                    SubScale = thumbnailScale;
                }
                else
                {
                    SubScale = 1;
                }
                NotifyPropertyChanged();
                GC.Collect();
            }
        }
        private BitmapImage _activeImage;

        public double thumbnailScale = 1;

        /// <summary>
        /// Re-scale the image (inside an alread-scaled element) when the low-resolution thumbnail image is displayed.
        /// </summary>
        public double SubScale
        {
            get { return _subScale; }
            set
            {
                _subScale = value;
                NotifyPropertyChanged();
            }
        }
        private double _subScale = 1d;

        /// <summary>
        /// Scale factors to run through when using the zoom tool without the Rectangle.
        /// </summary>
        public static double[] ZoomLevels { get; } = new double[] { .005, .01, .02, .033, .05, .1, .2, .33, .5, .66, .8, 1, 1.5, 2, 3, 4, 5, 7, 10, 15, 20 };
        
        /// <summary>
        /// Asynchronously load the full-resolution image.
        /// </summary>
        public static Task<BitmapImage> LoadFullResImage(string path)
        {
            return Task.Run(() =>
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(path);
                image.EndInit();
                image.Freeze();
                return image;
            });
        }

        /// <summary>
        /// The relevant metadata tags and how to extract them.
        /// </summary>
        public static Dictionary<string, MetadataTag> MetadataTagList = new Dictionary<string, MetadataTag>
        {
            {"Artist", new MetadataTag(typeof(string), "/app1/{ushort=0}/{ushort=315}", "/app13/{ushort=0}/{ulonglong=61857348781060}/iptc/{str=By-line}")},
            {"BodySerialNumber", new MetadataTag(typeof(string), "/app1/{ushort=0}/{ushort=34665}/{ushort=42033}")},
            {"CaptionWriter",new MetadataTag(typeof(string), "/app13/{ushort=0}/{ulonglong=61857348781060}/iptc/{str=Writer/Editor}", "/xmp/photoshop:CaptionWriter")},
            {"Copyright", new MetadataTag(typeof(string), "/app13/{ushort=0}/{ulonglong=61857348781060}/iptc/{str=Copyright Notice}", "/app1/{ushort=0}/{ushort=33432}")},
            {"Creator", new MetadataTag(typeof(string), "/xmp/dc:creator/{ulong=0}")},
            {"DateCreated", new MetadataTag(typeof(DateTime), "/xmp/photoshop:DateCreated")},
            {"DateTime", new MetadataTag(typeof(DateTime), "/app1/{ushort=0}/{ushort=306}")},
            {"DateTimeDigitized", new MetadataTag(typeof(DateTime), "/app1/{ushort=0}/{ushort=34665}/{ushort=36868}", "/xmp/exif:DateTimeDigitized", "/xmp/xmp:CreateDate")},
            {"DateTimeOriginal", new MetadataTag(typeof(DateTime), "/app1/{ushort=0}/{ushort=34665}/{ushort=36867}", "/xmp/exif:DateTimeOriginal")},
            {"Description", new MetadataTag(typeof(string), "/xmp/dc:description/x-default")},
            {"Headline", new MetadataTag(typeof(string), "/xmp/photoshop:Headline", "/app13/{ushort=0}/{ulonglong=61857348781060}/iptc/{str=Headline}")},
            {"ImageDescription", new MetadataTag(typeof(string), "/app13/{ushort=0}/{ulonglong=61857348781060}/iptc/{str=Caption}", "/app1/{ushort=0}/{ushort=270}")},
            {"Keywords", new MetadataTag(typeof(string), "/app13/{ushort=0}/{ulonglong=61857348781060}/iptc/{str=Keywords}")},
            {"LensModel", new MetadataTag(typeof(string), "/app1/{ushort=0}/{ushort=34665}/{ushort=42036}")},
            {"LensSerialNumber", new MetadataTag(typeof(string), "/app1/{ushort=0}/{ushort=34665}/{ushort=42037}")},
            {"Make", new MetadataTag(typeof(string), "/app1/{ushort=0}/{ushort=271}")},
            {"Model", new MetadataTag(typeof(string), "/app1/{ushort=0}/{ushort=272}")},
            {"Rights", new MetadataTag(typeof(string), "/xmp/dc:rights/x-default")},
            {"Subject", new MetadataTag(typeof(string), "/xmp/dc:subject/{ulong=0}")},
            {"TextEntry", new MetadataTag(typeof(string), "/com/TextEntry")},
            {"Title", new MetadataTag(typeof(string), "/xmp/dc:title/x-default", "/app13/{ushort=0}/{ulonglong=61857348781060}/iptc/{str=Object Name}")},
            {"UserComment", new MetadataTag(typeof(string), "/app1/{ushort=0}/{ushort=34665}/{ushort=37510}")}
        };

        /// <summary>
        /// The file name from Path, for displaying.
        /// </summary>
        public string ImageName
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(FilePath); }
        }

        /// <summary>
        /// Re-scales the image and all of the line and count marks.
        /// </summary>
        public double ScaleFactor
        {
            get { return _scaleFactor; }
            set
            {
                _scaleFactor = value;
                NotifyPropertyChanged();
            }
        }
        private double _scaleFactor = 1f;

        /// <summary>
        /// Diameter of the count dots as displayed. This scales inverse to ScaleFactor.
        /// Use a Property rather than a Converter so that the ViewModel layer can use this to select elements.
        /// </summary>
        public double ActualCountDiameter
        {
            get { return Parent.CountDiameter / ScaleFactor; }
        }

        /// <summary>
        /// True to have the image resize with the window.
        /// </summary>
        public bool FitToWindow
        {
            get { return _fitToWindow; }
            set
            {
                _fitToWindow = value;
                NotifyPropertyChanged();
            }
        }
        private bool _fitToWindow = true;

        #region Command patterns for changes to CollectionUIElements

        /// <summary>
        /// True when changes have been made to CollectionUIElements since it was last saved.
        /// </summary>
        public bool HasChanges = false;

        /// <summary>
        /// A collection of commands to add/remove counts and lines
        /// </summary>
        public List<Command> CollectionCommands { get; } = new List<Command>();

        /// <summary>
        /// Keeps track of where in the CollectionCommands the user is.
        /// </summary>
        public int CursorCommands
        {
            get { return _cursorCommands; }
            set
            {
                if (value < -1)
                    _cursorCommands = -1;
                else if (value > CollectionCommands.Count)
                    _cursorCommands = CollectionCommands.Count;
                else
                    _cursorCommands = value;
            }
        }
        private int _cursorCommands = 0;

        /// <summary>
        /// For both Redo() and Undo(), allow CursorCommands to exceed the CollectionCommands range to allow the user to hit undo or redo beyond the range without applying repeat add commands.
        /// </summary>
        public void Undo()
        {
            if (CursorCommands > CollectionCommands.Count - 1)
            {
                CursorCommands = CollectionCommands.Count - 1;
            }
            if (CursorCommands > -1)
            {
                CollectionCommands[CursorCommands].UnExecute();
            }
            CursorCommands--;
        }

        /// <summary>
        /// For both Redo() and Undo(), allow CursorCommands to exceed the CollectionCommands range to allow the user to hit undo or redo beyond the range without applying repeat add commands.
        /// </summary>
        public void Redo()
        {
            CursorCommands++;
            if (CursorCommands < 0)
                CursorCommands = 0;
            if (CursorCommands < CollectionCommands.Count)
            {
                CollectionCommands[CursorCommands].Execute();
            }
        }

        public void AddCount(Point location, ButtonType button)
        {
            AddCountCommand c = new AddCountCommand(this, location, button);
            c.Execute();
        }

        public void AddLine(Point location)
        {
            AddLineCommand c = new AddLineCommand(this, location);
            c.Execute();
        }

        public void RemoveElements(Mark[] items)
        {
            RemoveCommand c = new RemoveCommand(this, items);
            c.Execute();
        }

        public void MoveCounts(Dictionary<Mark, Point> locations)
        {

            Dictionary<CountMark, MovePoints> list = new Dictionary<CountMark, MovePoints>();
            foreach (KeyValuePair<Mark, Point> m in locations)
            {
                CountMark c = m.Key as CountMark;
                if (c != null)
                {
                    list[c] = new MovePoints(m.Value, c.Location);
                }
            }
            if (list.Count > 0)
            {
                MoveCommand command = new MoveCommand(this, list);
            }
        }

        #endregion

        /// <summary>
        /// Add a line segment to an existing PathGeometry.
        /// This method falls outside the Undo/Redo scope.
        /// </summary>
        /// <param name="location">Point for the next line segment.</param>
        public void AddPoint(Point location)
        {
            LineMark l = CollectionUIElements.Last() as LineMark;
            l.AddPoint(location);
        }

        /// <summary>
        /// Track category counts to display in the image window.
        /// </summary>
        public Dictionary<string, NotifyingInt> CollectionCounts { get; } = new Dictionary<string, NotifyingInt>();

        /// <summary>
        /// Collection of variables taken from the MainWindow, which are taken from the config file.
        /// </summary>
        public ObservableCollection<ColumnValuePair> CollectionInputs { get; } = new ObservableCollection<ColumnValuePair>();

        /// <summary>
        /// Collection of default values from the image metadata and windows information.
        /// </summary>
        public Dictionary<string, object> CollectionImageValues { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Count marks and lines to show over the image.
        /// </summary>
        public ObservableCollection<Mark> CollectionUIElements { get; } = new ObservableCollection<Mark>();

        #region Rectangle properties for selection tool

        /// <summary>
        /// True when the Selection Rectangle is visible. Otherwise, false.
        /// Better to maintain a bool than a Visibility. Convert this to a Visibility in the view layer.
        /// </summary>
        public bool SelectionVisible
        {
            get { return _selectionVisible; }
            set
            {
                _selectionVisible = value;
                NotifyPropertyChanged();
            }
        }
        private bool _selectionVisible = false;

        /// <summary>
        /// First point set for the Selection Rectangle. Use this with SelectionEnd to calculate Rectangle.Margin and Rectangle.Width & Height.
        /// </summary>
        public Point SelectionStart
        {
            get { return _selectionStart; }
            set
            {
                _selectionStart = value;
                NotifyPropertyChanged();
            }
        }
        private Point _selectionStart;

        /// <summary>
        /// Last point set for the Selection Rectangle. Use this with SelectionStart to calculate Rectangle.Margin and Rectangle.Width & Height.
        /// </summary>
        public Point SelectionEnd
        {
            get { return _selectionEnd; }
            set
            {
                _selectionEnd = value;
                NotifyPropertyChanged();
            }
        }
        private Point _selectionEnd;

        /// <summary>
        /// When a user clicks a single point with the select tool, only one element close by is selected.
        /// Keep track of the CollectionUIElement index of the current selected element to find the next one near by.
        /// </summary>
        public int StartSelectSearch = 0;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">The full path of the image file.</param>
        /// <param name="parent">The DataContext of the main window.</param>
        public ImageFormContext(string path, MainDataContext parent)
        {
            Parent = parent;
            foreach (KeyValuePair<string, object> kvp in parent.CollectionVariables)
            {
                CollectionInputs.Add(new ColumnValuePair(kvp.Key, kvp.Value));
            }
            foreach (KeyValuePair<string, int> kvp in parent.CollectionCounts)
            {
                CollectionCounts[kvp.Key] = new NotifyingInt();
            }
            CollectionUIElements.CollectionChanged += CollectionUIElements_CollectionChanged;
            FilePath = path;
        }

        /// <summary>
        /// Update CollectionCounts whenever CollectionUIElements changes, and then use CollectionCounts to display count totals in the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollectionUIElements_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (object o in e.NewItems)
                {
                    CountMark c = o as CountMark;
                    if (c != null)
                    {
                        CollectionCounts[c.Button.Category].Increment();
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (object o in e.OldItems)
                {
                    CountMark c = o as CountMark;
                    if (c != null)
                    {
                        CollectionCounts[c.Button.Category].Decrement();
                    }
                }
            }
            NotifyPropertyChanged("CollectionCounts");
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class RelayCommand : ICommand
    {
        private Predicate<object> _canExecute;
        private Action<object> _execute;

        public RelayCommand(Predicate<object> canExecute, Action<object> execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }

    /// <summary>
    /// Use this for the Dictionary.Value type to update the UI.
    /// </summary>
    public class NotifyingInt : INotifyPropertyChanged
    {
        private int _value = 0;
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                NotifyPropertyChanged();
            }
        }

        public NotifyingInt()
        {

        }

        public void Increment()
        {
            Value++;
        }

        public void Decrement()
        {
            Value--;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// This shader only controls brightness and contrast for now.
    /// Eventually I want to mimic the Highlight Shadows tool in Photoshop.
    /// Contrast change is non-linear with the slider change from -1 to 1.
    /// </summary>
    public class ShaderEffects : System.Windows.Media.Effects.ShaderEffect
    {
        // A static PixelShader causes a really bad memory leak.
        // Could not figure out how to set shader.UriSource to resource pack. So, SetStreamSource to an assembly resource stream in this constructor.
        private System.Windows.Media.Effects.PixelShader shader = new System.Windows.Media.Effects.PixelShader();

        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(ShaderEffects), 0);
        public static readonly DependencyProperty BrightnessProperty = DependencyProperty.Register("Brightness", typeof(double), typeof(ShaderEffects), new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0)));
        public static readonly DependencyProperty ContrastProperty = DependencyProperty.Register("Contrast", typeof(double), typeof(ShaderEffects), new UIPropertyMetadata(0.0, PixelShaderConstantCallback(1)));

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public float Brightness
        {
            get { return (float)GetValue(BrightnessProperty); }
            set { SetValue(BrightnessProperty, value); }
        }

        public float Contrast
        {
            get { return (float)GetValue(ContrastProperty); }
            set { SetValue(ContrastProperty, value); }
        }

        public ShaderEffects()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream("CountingApp3.PixelShader.ps");
            shader.SetStreamSource(stream);
            PixelShader = shader;
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(BrightnessProperty);
            UpdateShaderValue(ContrastProperty);
        }

    }

    public class MetadataTag
    {
        public string[] Queries;
        public Type Format;
        public MetadataTag(Type Format, params string[] Queries)
        {
            this.Format = Format;
            this.Queries = Queries;
        }
    }

    #region Commands for Undo/Redo Patterns

    public abstract class Command
    {
        protected ImageFormContext context;
        public Command(ImageFormContext context)
        {
            this.context = context;
            while (context.CollectionCommands.Count - 1 > context.CursorCommands)
                context.CollectionCommands.RemoveAt(context.CollectionCommands.Count - 1);
            context.CollectionCommands.Add(this);
            context.CursorCommands++;
            context.HasChanges = true;
        }
        public abstract void Execute();
        public abstract void UnExecute();
    }

    public class AddCountCommand : Command
    {
        private CountMark mark;

        public AddCountCommand(ImageFormContext context, Point location, ButtonType button) : base(context)
        {
            mark = new CountMark(button, location);
        }

        public override void Execute()
        {
            context.CollectionUIElements.Add(mark);
        }

        public override void UnExecute()
        {
            mark.IsSelected = false;
            context.CollectionUIElements.Remove(mark);
        }

    }

    public class AddLineCommand : Command
    {
        private LineMark mark;

        public AddLineCommand(ImageFormContext context, Point start) : base(context)
        {
            mark = new LineMark(start);
        }

        public override void Execute()
        {
            context.CollectionUIElements.Add(mark);
        }

        public override void UnExecute()
        {
            mark.IsSelected = false;
            context.CollectionUIElements.Remove(mark);
        }
    }

    public class RemoveCommand : Command
    {
        private Mark[] items;

        public RemoveCommand(ImageFormContext context, Mark[] items) : base(context)
        {
            this.items = items;
        }

        public override void Execute()
        {
            foreach (Mark item in items)
            {
                context.CollectionUIElements.Remove(item);
            }
        }

        public override void UnExecute()
        {
            foreach (Mark item in items)
            {
                item.IsSelected = false;
                context.CollectionUIElements.Add(item);
            }
        }
    }

    public class MoveCommand : Command
    {
        private Dictionary<CountMark, MovePoints> list = new Dictionary<CountMark, MovePoints>();

        public MoveCommand(ImageFormContext context, Dictionary<CountMark, MovePoints> list) : base(context)
        {
            this.list = list;
        }

        public override void Execute()
        {
            foreach (KeyValuePair<CountMark, MovePoints> m in list)
            {
                m.Key.Location = m.Value.NewPoint;
            }
        }

        public override void UnExecute()
        {
            foreach (KeyValuePair<CountMark, MovePoints> m in list)
            {
                m.Key.Location = m.Value.OldPoint;
            }
        }
    }

    /// <summary>
    /// A class to hold the old and new locations when CountMarks are moved.
    /// </summary>
    public class MovePoints
    {
        public Point OldPoint = new Point();
        public Point NewPoint = new Point();

        public MovePoints(Point OldPoint, Point NewPoint)
        {
            this.OldPoint = OldPoint;
            this.NewPoint = NewPoint;
        }
    }

    #endregion
}
