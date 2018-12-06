using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Data.OleDb;

namespace CountingApp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainDataContext context;

        /// <summary>
        /// Listen for global hotkeys.
        /// </summary>
        private LowLevelKeyboardListener _listener;

        public MainWindow()
        {
            InitializeComponent();
            context = DataContext as MainDataContext;
            ReadConfig();
            context.AddKeyBindings(this, context.CollectionTools);
            context.ActiveTool = MainDataContext.ToolSelect;
            _listener = new LowLevelKeyboardListener();
            _listener.OnKeyPressed += _listener_OnKeyPressed;
            _listener.OnKeyReleased += _listener_OnKeyReleased;
            _listener.HookKeyboard();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            // Set the MaxWidth and MaxHeight properties after rendering so that Width cannot be changed from the SizeToContent width and height can be changed after it is rendered with a restriction to SizeToContent.
            MaxWidth = ActualWidth;
            MinWidth = ActualWidth;
            MaxHeight = Int32.MaxValue;
            SizeToContent = SizeToContent.Manual;
        }

        /// <summary>
        /// Using a keyword config file rather than XML. Users should be able to intuitively modify the config file.
        /// </summary>
        private void ReadConfig()
        {
            // Store read count categories/colors. Create the count categories in the App and their buttons after reading all of the lines to assign distinct colors to missing colors.
            Dictionary<string, Color?> counts = new Dictionary<string, Color?>();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
            if (File.Exists(path))
            {
                context.CollectionVariables.Clear();
                string[] lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++)
                {
                    // Trim whitespaces to simplify Regex.
                    string line = lines[i].Trim();
                    // Comments are preceded with -- or #
                    if (string.IsNullOrEmpty(line) || line.Substring(0, 2) == "--" || line[0] == '#')
                    {
                        continue;
                    }
                    // Count category is formatted as "Count: [Category], [Color]", where whitespaces are not important.
                    // When a color cannot be found, a distinct random color is assigned to that category.
                    // When the line is empty after Count: ignore it.
                    // Duplicate categories are over-written.
                    Match m = Regex.Match(line, @"(?<=Count:).*", RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        string[] result = m.Value.Split(',');
                        if (result.Length < 1)
                        {
                            continue;
                        }
                        string category = result[0].Trim();
                        Color? color = null;
                        if (result.Length > 1)
                        {
                            try
                            {
                                color = (Color)ColorConverter.ConvertFromString(result[1].Trim());
                            }
                            catch { }
                        }
                        counts[category] = color;
                        continue;
                    }
                    // New! Save count dot size programmatically or manually to the configuration file.
                    // Default diameter is already set in DataContext.
                    m = Regex.Match(line, @"(?<=Count Size:).*", RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        double d = context.CountDiameter;
                        double.TryParse(m.Value.Trim(), out d);
                        context.CountDiameter = d;
                        continue;
                    }
                    // OleDB connection string line is formatted as "Connection String: [...]".
                    // Can create a connection string in the App.
                    // Ignore passwords in the connection string- disincentivize saving passwords to this unsecure text file!
                    m = Regex.Match(line, @"(?<=Database Connection:).*", RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        context.OleDBString = m.Value.Trim();
                        continue;
                    }
                    // Column names for the table saved as a file.
                    // Start with "File Columns:" followed by a comma separated list of column names.
                    // The next line should start with "Values:" and a comma separated list.
                    // This way, the column names and the variable names displayed in the App can be different.
                    m = Regex.Match(line, @"(?<=^File Columns:).*", RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        string[] keys = SplitString(m.Value);
                        string[] values = new string[0];
                        if (i + 1 < lines.Length)
                        {
                            Match matchValue = Regex.Match(lines[i + 1].Trim(), @"(?<=^Values:).*", RegexOptions.IgnoreCase);
                            if (matchValue.Success)
                            {
                                values = SplitString(matchValue.Value);
                                i++;
                            }
                        }
                        for (int j = 0; j < keys.Length; j++)
                        {
                            string c = keys[j].Trim();
                            string v = j < values.Length ? values[j].Trim() : string.Empty;
                            context.CollectionFileColumns[c] = v;
                        }
                        continue;
                    }
                    // Database table name, columns, and values.
                    // Start with "Database Table:" followed by the database table name.
                    // The next line should start with "Columns:" followed by a comma separated list of the database columns.
                    // The 3rd line should start with "Values:" followed by a comma separated list of the values. This way, column and variable names can be different.
                    m = Regex.Match(line, @"(?<=^Database Table:).*", RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        string table = m.Value.Trim();
                        if (!Regex.IsMatch(table, @"^\[.*\]"))
                        {
                            table = $"[{table.Replace(".", "].[")}]";
                        }
                        string[] columns = new string[0];
                        string[] values = new string[0];
                        if (i + 1 < lines.Length)
                        {
                            Match matchColumns = Regex.Match(lines[i + 1].Trim(), @"(?<=^Columns:).*", RegexOptions.IgnoreCase);
                            if (matchColumns.Success)
                            {
                                columns = SplitString(matchColumns.Value, true);
                                i++;
                                if (i + 1 < lines.Length)
                                {
                                    Match matchValues = Regex.Match(lines[i + 1].Trim(), @"(?<=^Values:).*", RegexOptions.IgnoreCase);
                                    if (matchValues.Success)
                                    {
                                        values = SplitString(matchValues.Value);
                                        i++;
                                    }
                                }
                            }
                        }
                        context.DatabaseTableName = table;
                        for (int j = 0; j < columns.Length; j++)
                        {
                            string c = columns[j].Trim();
                            string v = j < values.Length ? values[j].Trim() : string.Empty;
                            context.CollectionDatabaseTable[c] = v;
                        }
                        continue;
                    }
                    // Database table name, columns, and values to get the target table foreign key values.
                    // Start with "Parent Table:" followed by the parent database table name.
                    // The next 3 lines should be for the "Columns:", "Values:", "Return:".
                    m = Regex.Match(line, @"(?<=^Reference Table:).*", RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        string table = m.Value.Trim();
                        if (!Regex.IsMatch(table, @"^\[.*\]"))
                        {
                            table = $"[{table.Replace(".", "].[")}]";
                        }
                        string[] columns = new string[0];
                        string[] values = new string[0];
                        string[] returns = new string[0];
                        if (i + 1 < lines.Length)
                        {
                            Match matchColumns = Regex.Match(lines[i + 1].Trim(), @"(?<=^Columns:).*", RegexOptions.IgnoreCase);
                            if (matchColumns.Success)
                            {
                                columns = SplitString(matchColumns.Value, true);
                                i++;
                                if (i + 1 < lines.Length)
                                {
                                    Match matchValues = Regex.Match(lines[i + 1].Trim(), @"(?<=^Values:).*", RegexOptions.IgnoreCase);
                                    if (matchValues.Success)
                                    {
                                        values = SplitString(matchValues.Value);
                                        i++;
                                        if (i + 1 < lines.Length)
                                        {
                                            Match matchReturn = Regex.Match(lines[i + 1].Trim(), @"(?<=^Return:).*", RegexOptions.IgnoreCase);
                                            if (matchReturn.Success)
                                            {
                                                // Do not add brackets to the select column here because it will mess with value lookup later on. Do it in the query builder.
                                                returns = SplitString(matchReturn.Value);
                                                i++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        Dictionary<string, string> columnValuePair = new Dictionary<string, string>();
                        for (int j = 0; j < columns.Length; j++)
                        {
                            string c = columns[j].Trim();
                            string v = j < values.Length ? values[j].Trim() : string.Empty;
                            columnValuePair[c] = v;
                        }
                        ReferencedTable referenced = new ReferencedTable(table, columnValuePair, returns);
                        context.CollectionReferencedTables.Add(referenced);
                        continue;
                    }
                    // If it gets this far, that line sets a variable.
                    m = Regex.Match(line, @"^(?<name>.*?)\s*:\s*(?<value>.*)", RegexOptions.IgnoreCase);
                    if (m.Groups["name"].Success)
                    {
                        string name = m.Groups["name"].Value;
                        string value = m.Groups["value"].Success ? m.Groups["value"].Value : string.Empty;
                        context.CollectionVariables[name] = value;
                        continue;
                    }
                }
            }
            // Default counts and fields.
            if (counts.Count < 1)
            {
                counts.Add("Count", Colors.Red);
            }
            if (context.CollectionVariables.Count < 1)
            {
                context.CollectionVariables["Notes"] = "";
            }
            // Assign hotkeys, and distinct random colors to missing count colors, then add count Buttons.
            int counter = 1;
            foreach (KeyValuePair<string, Color?> kvp in counts)
            {
                Color? c = kvp.Value;
                while (c == null)
                {
                    byte[] array = new byte[3];
                    Random rand = new Random();
                    rand.NextBytes(array);
                    Color newC = Color.FromRgb(array[0], array[1], array[3]);
                    bool hit = false;
                    // Compare the new random color with existing colors.
                    // The current button has no color, so it will be passed over without any special handling.
                    foreach (KeyValuePair<string, Color?> otherKVP in counts)
                    {
                        Color? otherC = otherKVP.Value;
                        if (!otherC.HasValue)
                            continue;
                        if (Color.AreClose(newC, otherC.Value))
                        {
                            hit = true;
                            break;
                        }
                    }
                    if (!hit)
                    {
                        c = newC;
                    }
                }
                Key[] k = new Key[0];
                if (counter <= 10)
                {
                    k = new Key[2]
                    {
                        Key.D1 - 1 + counter,
                        counter == 10 ? Key.NumPad0 : Key.NumPad1 - 1 + counter
                    };
                }
                Span s =
                    counter < 10 ? MainDataContext.CompileSpan(false, MainDataContext.CompileSpan(true, counter), " - " + kvp.Key)
                    : counter == 10 ? MainDataContext.CompileSpan(false, MainDataContext.CompileSpan(true, 0), " - " + kvp.Key)
                    : MainDataContext.CompileSpan(false, kvp.Key);
                ButtonType newButton = new ButtonType(kvp.Key, c, s, k);
                context.CollectionTools.Add(newButton);
                context.CollectionCounts[kvp.Key] = 0;
                counter++;
            }
            // Default columns to save to file.
            if (context.CollectionFileColumns.Count < 1)
            {
                context.CollectionFileColumns["FileName"] = "FileName";
                context.CollectionFileColumns["CurrentDate"] = "CurrentDate";
                context.CollectionFileColumns["DateModified"] = "DateModified";
                context.CollectionFileColumns["UserName"] = "UserName";
                context.CollectionFileColumns["Notes"] = "Notes";
                foreach (KeyValuePair<string, Color?> kvp in counts)
                {
                    context.CollectionFileColumns[kvp.Key] = kvp.Key;
                }
            }
        }

        /// <summary>
        /// Split a string by commas not nested inside single- or double-quotes, or inside parentheses or brackets.
        /// </summary>
        public static string[] SplitString(string input, bool addBrackets = false)
        {
            // Easier to do this manually rather than with Regex.
            List<int> indexes = new List<int>() { 0 };
            List<char> close = new List<char>();
            for (int i = 0; i < input.Length; i++)
            {
                char last = close.Count == 0 ? ' ' : close.Last();
                switch (input[i])
                {
                    // Split by commas when not encompassed by single- or double-quotes, or preceeded with an open parenthesis or bracket.
                    case ',':
                        if (close.Count == 0)
                            indexes.Add(i + 1);
                        break;
                    // Pass over all other characters after an unclosed single or double quote.
                    case '"':
                        if (last == '"')
                            close.RemoveAt(close.Count - 1);
                        else if (last != '\'' && last != '[')
                            close.Add('"');
                        break;
                    // Escaped single-quotes with 2 single quotes does not need to be handled separately.
                    case '\'':
                        if (last == '\'')
                            close.RemoveAt(close.Count - 1);
                        else if (last != '"' && last != '[')
                            close.Add('\'');
                        break;
                    // Can only nest parentheses.
                    case '(':
                        if (last == ' ' || last == '(')
                            close.Add('(');
                        break;
                    case ')':
                        if (last == '(')
                            close.RemoveAt(close.Count - 1);
                        break;
                    case '[':
                        if (last == ' ' || last == '(')
                            close.Add('[');
                        break;
                    case ']':
                        if (last == '[')
                            close.RemoveAt(close.Count - 1);
                        break;
                }
            }
            indexes.Add(input.Length + 1);
            List<string> result = new List<string>();
            for (int i = 0; i < indexes.Count - 1; i++)
            {
                string s = input.Substring(indexes[i], indexes[i + 1] - indexes[i] - 1).Trim();
                if (addBrackets && !Regex.IsMatch(s, @"^\[.*\]$"))
                {
                    s = $"[{s.Replace(".", "].[")}]";
                }
                result.Add(s);
            }
            return result.ToArray();
        }

        /// <summary>
        /// The last tool selected when a shift key is pressed. This is the active tool when the shift key is released.
        /// </summary>
        private ButtonType lastButton;

        /// <summary>
        /// Listen for key press anywhere in Windows. Activate the Move tool when either shift key is pressed.
        /// </summary>
        private void _listener_OnKeyPressed(object sender, KeyPressedArgs e)
        {
            if (e.KeyPressed == Key.RightShift || e.KeyPressed == Key.LeftShift)
            {
                lastButton = context.ActiveTool;
                context.ActiveTool = MainDataContext.ToolMove;
            }
        }

        /// <summary>
        /// Listen for key press anywhere in Windows. Change the active tool back to whatever it was when either shift key is released.
        /// </summary>
        private void _listener_OnKeyReleased(object sender, KeyPressedArgs e)
        {
            if (e.KeyPressed == Key.RightShift || e.KeyPressed == Key.LeftShift)
            {
                if (lastButton != null)
                {
                    context.ActiveTool = lastButton;
                }
            }
        }

        /// <summary>
        /// Drag and drop files instead of using the Open file menu item.
        /// </summary>
        protected override void OnDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files.Length > 0)
                {
                    context.OpenFile(files);
                }
            }
        }

        /// <summary>
        /// Show an OpenFileDialog to browse and open an image or XML file.
        /// </summary>
        private void CommandOpen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            context.OpenFile();
        }

        /// <summary>
        /// Save all of the open image information into a single text file. Used to have an option to save into one file per open image. Removed because I don't think anybody will use that feature.
        /// </summary>
        private void MenuExportIntoFile_Click(object sender, RoutedEventArgs e)
        {
            context.ExportAsFile(context.GetAllWindows());
        }

        /// <summary>
        /// Save all of the open image information into an OleDB data source.
        /// </summary>
        private void MenuExportIntoDatabase_Click(object sender, RoutedEventArgs e)
        {
            context.SaveToDataSource(context.GetAllWindows());
        }

        /// <summary>
        /// Because passwords should never be stored in the unsecure text config file, this gives the user the chance to enter their username and password, and might as well give them the chance to build the entire OleDB connection string while we're at it.
        /// </summary>
        private void MenuDatabaseConnection_Click(object sender, RoutedEventArgs e)
        {
            context.DBConnection();
        }
        
        /// <summary>
        /// Close all open images. Allow Window.Closing override to handle saving data and handling the close.
        /// </summary>
        private void MenuCloseAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (w is ImageForm)
                {
                    w.Close();
                }
            }
        }
        
        /// <summary>
        /// Save all open images with the counts and lines embedded into the image.
        /// </summary>
        private void MenuOverlayAll_Click(object sender, RoutedEventArgs e)
        {
            context.SaveOverlay(context.GetAllWindows());
        }

        /// <summary>
        /// Make the count marks smaller. Let the context.CountDiameter property handle minimum size.
        /// </summary>
        private void ButtonDotSizeSmaller_Click(object sender, RoutedEventArgs e)
        {
            context.CountDiameter--;
        }

        /// <summary>
        /// Make the count marks smaller. Let the context.CountDiameter property handle maximum size.
        /// </summary>
        private void ButtonDotSizeEmbiggen_Click(object sender, RoutedEventArgs e)
        {
            context.CountDiameter++;
        }

        /// <summary>
        /// Have to handle ListBox right button down to avoid selecting an item when the user right-clicks it for its contextmenu.
        /// </summary>
        private void ListBox_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Handle everything in this.OnClosing() override.
        /// </summary>
        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Close all of the open windows. Other than this. Most of these will be ImageForms and will trigger Save dialogs. 
        /// If any of these ImageForms.Close() are cancelled, then keep this open.
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            foreach (Window w in Application.Current.Windows)
            {
                ImageForm f = w as ImageForm;
                if (f != null)
                {
                    f.Close();
                }
            }
            foreach (Window w in Application.Current.Windows)
            {
                if (w is ImageForm)
                {
                    e.Cancel = true;
                    break;
                }
            }
            base.OnClosing(e);
        }

        /// <summary>
        /// Clean up keyboard listener and save CountDiameter to the config file.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        { 
            _listener.UnHookKeyboard();
            // Save the dot size to the config file.
            // Save to config file.
            string result = $"Count Size: {context.CountDiameter}";
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\config.txt";
            if (File.Exists(path))
            {
                // Cannot write over a single line- have to write the entire file.
                List<string> lines = new List<string>(File.ReadAllLines(path));
                bool hit = false;
                for (int i = 0; i < lines.Count; i++)
                {
                    if (Regex.IsMatch(lines[i], "^Count Size:", RegexOptions.IgnoreCase))
                    {
                        lines[i] = result;
                        hit = true;
                        break;
                    }
                }
                if (!hit)
                {
                    lines.Add(result);
                }
                File.WriteAllLines(path, lines);
            }
            base.OnClosed(e);
        }

        /// <summary>
        /// Save all open images without changing their file names and directories. Have to save an individual image to change its file name and directory.
        /// </summary>
        private void CommandSaveAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            context.SaveFile(context.GetAllWindows());
        }

    }

    /// <summary>
    /// Data Context for the Main Window as well as part
    /// </summary>
    public class MainDataContext : INotifyPropertyChanged
    {
        /// <summary>
        /// Collection of variables taken from the config file.
        /// Photo-specific variables stored in formImage.
        /// </summary>
        public Dictionary<string, object> CollectionVariables { get; } = new Dictionary<string, object>();

        /// <summary>
        /// All tools (Select, Draw, Zoom, Move, all counts).
        /// </summary>
        public List<ButtonType> CollectionTools { get; }

        /// <summary>
        /// Just the count categories for new image forms to copy.
        /// </summary>
        public Dictionary<string, int> CollectionCounts { get; } = new Dictionary<string, int>();

        // The non-counting tool buttons. Set them here instead of in XAML so I can reference them by name, they are drawn like the count buttons, and it should be easier to add/remove tools in later versions.
        public static ButtonType ToolSelect = new ButtonType("Select", null, Cursors.Arrow, Key.S);
        public static ButtonType ToolMove = new ButtonType("Move", null, Cursors.ScrollAll, Key.M);
        public static ButtonType ToolDraw = new ButtonType("Draw", null, Cursors.Pen, Key.D);
        public static ButtonType ToolZoom = new ButtonType("Zoom", null, Cursors.SizeNESW, Key.Z);

        /// <summary>
        /// The selected tool/button from CollectionTools.
        /// </summary>
        public ButtonType ActiveTool
        {
            get { return _activeTool; }
            set
            {
                _activeTool = value;
                NotifyPropertyChanged();
            }
        }
        private ButtonType _activeTool;

        /// <summary>
        /// For the saved files, Keys are the column headers, and Values are the variable names in CollectionVariables here or in the formImage.
        /// </summary>
        public Dictionary<string, string> CollectionFileColumns { get; } = new Dictionary<string, string>();

        #region Database information

        /// <summary>
        /// OLEDB connection string to the data source to save to.
        /// </summary>
        public string OleDBString { get; set; } = string.Empty;

        public string DatabaseTableName { get; set; } = string.Empty;

        public Dictionary<string, string> CollectionDatabaseTable { get; } = new Dictionary<string, string>();

        public List<ReferencedTable> CollectionReferencedTables { get; } = new List<ReferencedTable>();

        #endregion Database information

        /// <summary>
        /// The diameter that is actually set, before it is inverse-scaled.
        /// </summary>
        public double CountDiameter
        {
            get { return _countDiameter; }
            set
            {
                if (value > 1)
                {
                    _countDiameter = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _countDiameter = 10f;

        /// <summary>
        /// Show or hide the ToolBar in the image windows. Toolbars contain menu options and custom inputs.
        /// </summary>
        public bool ShowImageMenus
        {
            get { return _showImageMenus; }
            set
            {
                _showImageMenus = value;
                NotifyPropertyChanged();
            }
        }
        private bool _showImageMenus = true;

        /// <summary>
        /// When user matches the ScaleFactors for all open windows, keep that ScaleFactor for all new windows.
        /// When this is null, set the ScaleFactor for new windows to a size that will fit the window.
        /// </summary>
        public double? DefaultScaleFactor = null;

        /// <summary>
        /// When ScaleFactors of all images are forced to match, so should window sizes.
        /// </summary>
        public Size? DefaultWindowSize = null;

        public MainDataContext()
        {
            ToolSelect.DisplayCategory = CompileSpan(false, CompileSpan(true, "S"), "elect");
            ToolMove.DisplayCategory = CompileSpan(false, CompileSpan(true, "M"), "ove");
            ToolDraw.DisplayCategory = CompileSpan(false, CompileSpan(true, "D"), "raw");
            ToolZoom.DisplayCategory = CompileSpan(false, CompileSpan(true, "Z"), "oom");
            CollectionTools = new List<ButtonType>() { ToolSelect, ToolMove, ToolDraw, ToolZoom };
        }

        /// <summary>
        /// Span does not have an easy constructor, so build a span here for better inline coding.
        /// </summary>
        /// <param name="contents">Inlines, Strings, and UIElements</param>
        public static Span CompileSpan(bool isUnderline, params object[] contents)
        {
            Span span = new Span();
            if (isUnderline)
            {
                span = new Underline();
            }
            foreach (object o in contents)
            {
                if (o is Inline)
                {
                    span.Inlines.Add(o as Inline);
                }
                else if (o is string)
                {
                    span.Inlines.Add(o as string);
                }
                else if (o is UIElement)
                {
                    span.Inlines.Add(o as UIElement);
                }
                else
                {
                    span.Inlines.Add(Convert.ToString(o));
                }
            }
            return span;
        }

        #region Shared Functions

        /// <summary>
        /// Save the OpenFileDialog.FilterIndex so that it stays when the user opens another file.
        /// </summary>
        private int defaultOpenIndex = 0;

        /// <summary>
        /// Show an OpenFileDialog before opening the file or files. Calls OpenFile(string[] paths) after the dialog returns true.
        /// </summary>
        public void OpenFile()
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "Open Image File or Files";
            dialog.Filter = "Image (*.jpg)|*.jpg|Data (*.xml)|*.xml|All files|*.*";
            dialog.FilterIndex = defaultOpenIndex;
            dialog.Multiselect = true;
            if (dialog.ShowDialog() != true)
            {
                return;
            }
            defaultOpenIndex = dialog.FilterIndex;
            OpenFile(dialog.FileNames);
        }

        /// <summary>
        /// Open a new count image form for each path. If the path is already open, that form is focused.
        /// Can open an XML file only if the image file with the same name can be found in that directory. Could include the image path in the XML to decouple the files, but that may become too confusing for the user, so don't.
        /// Send the path string to the new window, and handle file types and loading images there.
        /// Throws an error if something goes wrong- most likely the image file is corrupt or image is not found.
        /// </summary>
        /// <param name="paths">Full paths to open</param>
        public void OpenFile(string[] paths)
        {
            foreach (string p in paths)
            {
                // If the image is already open, focus that window and do not open a new one. Then, continue with the other paths to open.
                bool flag = false;
                foreach (Window win in Application.Current.Windows)
                {
                    ImageForm form = win as ImageForm;
                    if (form != null && form.context != null && form.context.FilePath == p)
                    {
                        flag = true;
                        form.Activate();
                        break;
                    }
                }
                if (flag)
                    continue;
                try
                {
                    ImageForm newForm = new ImageForm(p, this);
                    newForm.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not open file {p}. Error message: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Save the count marks, lines, and input values to an XML file with the same name and directory as the image file.
        /// To change the directory or file name, handle that in the ViewModel- copy the image, then run SaveFile().
        /// </summary>
        public bool SaveFile(params ImageForm[] forms)
        {
            List<string> errors = new List<string>();
            bool result = true;
            foreach (ImageForm form in forms)
            {
                ImageFormContext context = form.context;
                // Save the counts, lines, and text inputs as a separate XML file. 
                // As much as I want to, do not store in the image metadata- it can easily fill up the memory limits.
                XDocument doc = new XDocument();
                XElement root = new XElement("Data");
                doc.Add(root);
                // Allow opening XML files that will find it's own image elsewhere.
                root.Add(new XAttribute("FilePath", context.FilePath));
                // Save user inputs.
                XElement inputs = new XElement("TextInputs");
                root.Add(inputs);
                foreach (ColumnValuePair input in context.CollectionInputs)
                {
                    inputs.Add(new XAttribute(input.Column, input.Value));
                }
                XElement elements = new XElement("UIElements");
                root.Add(elements);
                foreach (Mark m in context.CollectionUIElements)
                {
                    if (m is CountMark)
                    {
                        CountMark c = m as CountMark;
                        elements.Add(new XElement(
                            "CountMark",
                            new XAttribute("Location", c.Location),
                            new XAttribute("Category", c.Button.Category)));
                    }
                    else if (m is LineMark)
                    {
                        LineMark l = m as LineMark;
                        elements.Add(new XElement(
                            "LineMark",
                            new XAttribute("Geometry", l.Geometry)));
                    }
                }
                try
                {
                    // Save the xml document
                    doc.Save(context.XMLPath);
                    // Reset HasChanges so that the user can exit without a save file prompt.
                    context.HasChanges = false;
                    return true;
                }
                catch (Exception ex)
                {
                    errors.Add($"{Path.GetFileNameWithoutExtension(context.XMLPath)}. Error message: {ex.Message}");
                    result = false;
                }
            }
            if (errors.Count > 0)
            {
                MessageBox.Show($"Could not save the following images:{Environment.NewLine}{string.Join(Environment.NewLine + "\t", errors.ToArray())}");
            }
            return result;
        }

        /// <summary>
        /// Save the image with the marks overlaid. When saving more than 1, cannot overwrite any files to avoid accidental overwrite.
        /// </summary>
        public void SaveOverlay(params ImageForm[] forms)
        {
            foreach (ImageForm form in forms)
            {
                ImageFormContext context = form.context;
                string fileName = $"{Path.GetFileNameWithoutExtension(context.XMLPath)}_overlaid";
                if (forms.Length == 1)
                {
                    Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
                    dialog.Title = "Save image overlaid with marks as...";
                    dialog.Filter = "JPG Image|*.jpg";
                    dialog.DefaultExt = "jpg";
                    dialog.AddExtension = true;
                    dialog.OverwritePrompt = false;
                    dialog.FileName = fileName;
                    bool? result = dialog.ShowDialog();
                    if (result != true)
                        return;
                    fileName = dialog.FileName;
                }
                if (forms.Length > 1)
                {
                    int i = 1;
                    string adjustedFileName = fileName;
                    while (File.Exists(adjustedFileName))
                    {
                        adjustedFileName = Path.Combine(Path.GetDirectoryName(fileName), $"{Path.GetFileNameWithoutExtension(fileName)} ({i}).jpg");
                        i++;
                    }
                    fileName = adjustedFileName;
                }
                // Have to keep stream open for BitmapMetadata even when it has been cloned!
                using (Stream streamOpen = File.OpenRead(context.FilePath))
                using (FileStream streamSave = new FileStream(fileName, FileMode.Create))
                {
                    DrawingVisual visual = new DrawingVisual();
                    DrawingContext dc = visual.RenderOpen();
                    BitmapDecoder decoder = BitmapDecoder.Create(streamOpen, BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.OnDemand);
                    BitmapFrame source = decoder.Frames[0];
                    BitmapMetadata metadata = source.Metadata.Clone() as BitmapMetadata;
                    BitmapSource thumb = source.Thumbnail;
                    System.Collections.ObjectModel.ReadOnlyCollection<ColorContext> cc = source.ColorContexts;
                    dc.DrawImage(source, new Rect(0, 0, source.Width, source.Height));
                    // Mark sizes are stretched on RenderTargetBitmap() when it converts from screen. Make them smaller here. 
                    double diameter = CountDiameter / 2;// * original.Width / original.PixelWidth / 2 * 3;
                    foreach (Mark m in context.CollectionUIElements)
                    {
                        if (m is CountMark)
                        {
                            CountMark c = m as CountMark;
                            dc.DrawEllipse(c.Button.Brush, null, c.Location, diameter, diameter);
                        }
                        else if (m is LineMark)
                        {
                            LineMark l = m as LineMark;
                            dc.DrawGeometry(null, new Pen(new SolidColorBrush(Colors.Red), diameter / 2), l.Geometry);
                        }
                    }
                    DrawingImage image = new DrawingImage(visual.Drawing);
                    dc.Close();
                    RenderTargetBitmap rtb = new RenderTargetBitmap(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, PixelFormats.Default);
                    rtb.Render(visual);
                    BitmapFrame frame = BitmapFrame.Create(rtb, thumb, metadata, cc);
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder() { QualityLevel = 75 };
                    encoder.Frames.Add(frame);
                    encoder.Save(streamSave);
                }
            }
        }

        /// <summary>
        /// Save counts, values taken from the config file, and image-specific values to an XML, TXT, or CSV file.
        /// </summary>
        public void ExportAsFile(params ImageForm[] forms)
        {
            if (forms.Length < 1)
            {
                return;
            }
            // Set the file name and type.
            string fileName = Path.GetFileNameWithoutExtension(forms[0].context.XMLPath);
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = fileName;
            dialog.Filter = "Tab-separated|*.txt|Comma-separated|*.csv|XML|*.xml";
            dialog.AddExtension = true;
            if (dialog.ShowDialog() != true)
                return;
            string extension = Path.GetExtension(dialog.FileName).ToLower();
            // Iterate through all of the params forms, and then the CollectionFileColumns within each.
            // Separate XML file type from the others
            if (extension == ".xml")
            {
                XDocument doc = new XDocument();
                XElement root = new XElement("Results");
                doc.Add(root);
                foreach (ImageForm form in forms)
                {
                    XElement image = new XElement("Image");
                    root.Add(image);
                    foreach (KeyValuePair<string, string> pair in CollectionFileColumns)
                    {
                        string value = GetAttributeValue(form.context, pair.Value);
                        XAttribute attribute = new XAttribute(pair.Value, value);
                        image.Add(attribute);
                    }
                }
                doc.Save(dialog.FileName);
            }
            // TXT and CSV file types
            else
            {
                char delimiter = extension == ".csv" ? ',' : '\t';
                List<string> result = new List<string>();
                List<string> columns = new List<string>();
                foreach (KeyValuePair<string, string> pair in CollectionFileColumns)
                {
                    string name = pair.Key;
                    if (name.Contains(delimiter) || name.Contains('"') || name.Contains(Environment.NewLine))
                    {
                        name = $"\"{name.Replace("\"", "\"\"")}\"";
                    }
                    columns.Add(name);
                }
                result.Add(string.Join(delimiter.ToString(), columns));
                foreach (ImageForm form in forms)
                {
                    List<string> row = new List<string>();
                    foreach (KeyValuePair<string, string> pair in CollectionFileColumns)
                    {
                        string value = GetAttributeValue(form.context, pair.Value);
                        // Fields containing line breaks (CRLF), double quotes, and commas should be enclosed in double-quotes.
                        if (value.Contains(delimiter) || value.Contains('"') || value.Contains(Environment.NewLine))
                        {
                            value = $"\"{value.Replace("\"", "\"\"")}\"";
                        }
                        row.Add(value);
                    }
                    result.Add(string.Join(delimiter.ToString(), row));
                }
                File.WriteAllLines(dialog.FileName, result);
            }
        }

        /// <summary>
        /// Save counts, values taken from the config file, and image-specific information to an OleDB data source.
        /// </summary>
        public void SaveToDataSource(params ImageForm[] forms)
        {
            // Get foreign key referenced table values from CollectionReferencedTables.
            // Then, for each open image, get the column values for CollectionDatabaseTable, 
            // which is a dictionary of key = DB column names, and value = dictionary key for CollectionVariable, imageform.CollectionVariable, imageform count button category, or referenced table return column.
            // If a value contains quotes, search the dictionary, etc for the quoted values, assuming that the unquoted parts are SQL functions.
            if (string.IsNullOrWhiteSpace(OleDBString))
            {
                if (!DBConnection())
                    return;
            }
            Dictionary<string, string> errorMessages = new Dictionary<string, string>();
            // Have to insert one at a time. Testing with Excel OLEDB, it cannot bulk insert and cannot insert more than one row in an INSERT ... SELECT ...
            foreach (ImageForm image in forms)
            {
                // Build a single reference table for ease of lookup and better control over duplicate keys.
                // Sources of values: context.CollectionVariables, context.CollectionReferencedTables, imageForm.CollectionVariables, imageForm.CollectionCountButtons
                // These are ordered in ascending priorities when there are duplicate keys: image.CollectionVariables, CollectionVariables, CollectionCountButtons, CollectionReferencedTables.
                List<object> parameters = new List<object>();
                Dictionary<string, object> consolidatedDictionary = new Dictionary<string, object>(image.context.CollectionImageValues);
                foreach (ColumnValuePair pair in image.context.CollectionInputs)
                {
                    consolidatedDictionary[pair.Column] = pair.Value;
                }
                foreach (KeyValuePair<string, NotifyingInt> pair in image.context.CollectionCounts)
                {
                    consolidatedDictionary[pair.Key] = pair.Value.Value;
                }
                foreach (ReferencedTable table in CollectionReferencedTables)
                {
                    // Get referenced column value. But also may have to insert a row first.
                    // Any value used in a referenced table should already be in consolidatedDictionary.
                    // Maintain separate lists for columns, values, and WHERE expressions just in case we have to insert a new row.
                    // Tried OleDBCommand with named parameters- does not work! Instead, use ?'s for parameters.
                    List<string> wheres = new List<string>();
                    List<string> columns2 = new List<string>();
                    List<object> values2 = new List<object>();
                    List<object> parameters2 = new List<object>();
                    List<object> parametersInsert = new List<object>();
                    foreach (KeyValuePair<string, string> pair in table.ColumnValuePairs)
                    {
                        columns2.Add(pair.Key);
                        // Plain old variables
                        if (consolidatedDictionary.ContainsKey(pair.Value))
                        {
                            wheres.Add($"(? IS NULL OR {pair.Key} = ?)");
                            parameters2.Add(consolidatedDictionary[pair.Value]);
                            parameters2.Add(consolidatedDictionary[pair.Value]);
                            parametersInsert.Add(consolidatedDictionary[pair.Value]);
                            values2.Add('?');
                            continue;
                        }
                        // String literals
                        Match matchLiteral = Regex.Match(pair.Value, "(?<=^').*?(?='$)");
                        if (matchLiteral.Success)
                        {
                            wheres.Add($"(? IS NULL OR {pair.Key} = ?)");
                            parameters2.Add(string.IsNullOrWhiteSpace(matchLiteral.Value) ? null : matchLiteral.Value);
                            parameters2.Add(string.IsNullOrWhiteSpace(matchLiteral.Value) ? null : matchLiteral.Value);
                            parametersInsert.Add(string.IsNullOrWhiteSpace(matchLiteral.Value) ? null : matchLiteral.Value);
                            values2.Add('?');
                            continue;
                        }
                        // SQL expression with variables
                        MatchCollection matchExpression = Regex.Matches(pair.Value, "(?<=\").*?(?=\")");
                        if (matchExpression != null && matchExpression.Count > 0)
                        {
                            string value = pair.Value;
                            foreach (string variable in matchExpression)
                            {
                                // Find that value. If it does not exist, assume it was a string accidentally wrapped in double quotes.
                                object o = consolidatedDictionary.ContainsKey(variable) ? consolidatedDictionary[variable] : variable;
                                int count = value.Replace($"\"{variable}\"", "").Length / (variable.Length + 2);
                                for (int i = 0; i < count; i++)
                                {
                                    parameters2.Add(o);
                                    parametersInsert.Add(o);
                                }
                                value = value.Replace($"\"{variable}\"", "?");
                            }
                            wheres.Add($"{pair.Key} = {value}");
                            values2.Add(value);
                            continue;
                        }
                        // SQL expression without variables. Default assumption.
                        wheres.Add($"{pair.Key} = {pair.Value}");
                        values2.Add(pair.Value);
                    }
                    for (int i = 0; i < parameters2.Count; i++)
                    {
                        if (string.IsNullOrEmpty(parameters2[i].ToString()))
                            parameters2[i] = null;
                    }
                    for (int i = 0; i < parametersInsert.Count; i++)
                    {
                        if (string.IsNullOrEmpty(parametersInsert[i].ToString()))
                            parametersInsert[i] = null;
                    }
                    // Get the values from the database.
                    // OLEDB cannot return a scalar- just one object at a time.
                    using (OleDbConnection connection = new OleDbConnection(OleDBString))
                    {
                        OleDbCommand commandCount = new OleDbCommand($"SELECT COUNT(*) FROM {table.TableName} WHERE {string.Join(" AND ", wheres)}", connection);
                        OleDbCommand commandInsert = new OleDbCommand($"INSERT INTO {table.TableName} ({string.Join(",", columns2)}) VALUES ({string.Join(",", values2)});", connection);
                        for (int i = 0; i < parameters2.Count; i++)
                        {
                            commandCount.Parameters.Add(new OleDbParameter($"@var{i}", parameters2[i]));
                        }
                        for (int i = 0; i < parametersInsert.Count; i++)
                        {
                            commandInsert.Parameters.Add(new OleDbParameter($"@var{i}", parametersInsert[i]));
                        }
                        try
                        {
                            connection.Open();
                            int? rowCount = commandCount.ExecuteScalar() as int?;
                            if (!rowCount.HasValue || rowCount.Value < 1)
                            {
                                // Insert a row
                                commandInsert.ExecuteNonQuery();
                            }
                            if (rowCount.HasValue && rowCount.Value > 1)
                            {
                                MessageBox.Show($"Warning: multiple rows found given the config file columns for referenced table {table.TableName}. The database table values are filled in with the referenced table's first row values.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        catch (Exception ex)
                        {
                            errorMessages[Path.GetFileNameWithoutExtension(image.context.XMLPath)] = ex.Message;
                            continue;
                        }
                        finally
                        {
                            commandCount.Parameters.Clear();
                            commandInsert.Parameters.Clear();
                        }
                        foreach (string returnColumn in table.ReturnScalar)
                        {
                            // Bracket return column if not already bracketed. Keep it separate from the returnColumn lookup value.
                            string column = Regex.IsMatch(returnColumn, @"^\[.*\]$") ? returnColumn : $"[{returnColumn.Replace(".", "].[")}]";
                            OleDbCommand commandSelect = new OleDbCommand($"SELECT TOP 1 {column} FROM {table.TableName} WHERE {string.Join(" AND ", wheres)}", connection);
                            for (int i = 0; i < parameters2.Count; i++)
                            {
                                commandSelect.Parameters.Add(new OleDbParameter($"@var{i}", parameters2[i]));
                            }
                            try
                            {
                                object o = commandSelect.ExecuteScalar();
                                consolidatedDictionary[returnColumn] = o;
                            }
                            catch (Exception ex)
                            {
                                errorMessages[Path.GetFileNameWithoutExtension(image.context.XMLPath)] = ex.Message;
                                continue;
                            }
                            finally
                            {
                                commandSelect.Parameters.Clear();
                            }
                        }
                    }
                }
                // Iterate through CollectionDatabaseTable columns to build the insert query.
                List<string> row = new List<string>();
                foreach (KeyValuePair<string, string> pair in CollectionDatabaseTable)
                {
                    // Plain old variables
                    if (consolidatedDictionary.ContainsKey(pair.Value))
                    {
                        row.Add("?");
                        parameters.Add(consolidatedDictionary[pair.Value]);
                        continue;
                    }
                    // String literals
                    Match matchLiteral = Regex.Match(pair.Value, "(?<=^').*?(?='$)");
                    if (matchLiteral.Success)
                    {
                        row.Add("?");
                        parameters.Add(string.IsNullOrWhiteSpace(matchLiteral.Value) ? null : matchLiteral.Value);
                        continue;
                    }
                    // SQL expression with variables
                    MatchCollection matchExpression = Regex.Matches(pair.Value, "(?<=\").*?(?=\")");
                    if (matchExpression != null && matchExpression.Count > 0)
                    {
                        string value = pair.Value;
                        foreach (Match variable in matchExpression)
                        {
                            // Find that value. If it does not exist, assume it was a string accidentally wrapped in double quotes.
                            object o = consolidatedDictionary.ContainsKey(variable.ToString()) ? consolidatedDictionary[variable.ToString()] : variable.ToString();
                            value = value.Replace($"\"{variable.ToString()}\"", "?");
                            parameters.Add(o);
                        }
                        row.Add(value);
                        continue;
                    }
                    // SQL expression without variables. Default assumption.
                    row.Add(pair.Value);
                }
                for (int i = 0; i < parameters.Count; i++)
                {
                    if (string.IsNullOrEmpty(parameters[i].ToString()))
                        parameters[i] = null;
                }
                using (OleDbConnection connection = new OleDbConnection(OleDBString))
                {
                    OleDbCommand command = new OleDbCommand();
                    try
                    {
                        connection.Open();
                        command = new OleDbCommand($"INSERT INTO {DatabaseTableName} ({string.Join(",", CollectionDatabaseTable.Keys)}) VALUES ({string.Join(",", row)});") { Connection = connection };
                        for (int i = 0; i < parameters.Count; i++)
                        {
                            object value = parameters[i];
                            if (value == null || string.IsNullOrEmpty(value.ToString()))
                            {
                                value = DBNull.Value;
                            }
                            command.Parameters.Add(new OleDbParameter($"@var{i}", value));
                        }
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        errorMessages[Path.GetFileNameWithoutExtension(image.context.XMLPath)] = ex.Message;
                    }
                    finally
                    {
                        command.Parameters.Clear();
                    }
                }
            }
            if (errorMessages.Count < 1)
                MessageBox.Show($"All {forms.Length} images were saved to the database.");
            else if (errorMessages.Count == forms.Length)
                MessageBox.Show($"No images were saved to the database. Error message: {errorMessages.First()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MessageBox.Show($"The following images were not saved to the database with the error message: {errorMessages.First()} \r\n\r\n {string.Join("\r\n", errorMessages.Keys)}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Dialog window that creates an OLEDB connection string.
        /// Tested on SQL Server, Excel, and Access
        /// Have to warn user that Excel needs square brackets in the configuration file table and column names. Also, table names have to end in $.
        /// </summary>
        /// <returns>Returns false if user cancels out of the dialog</returns>
        public bool DBConnection()
        {
            OleDBStringBuilder dialog = new OleDBStringBuilder(OleDBString);
            if (dialog.ShowDialog() == true)
            {
                OleDBBuilderContext data = dialog.DataContext as OleDBBuilderContext;
                if (data.Save)
                {
                    // Save to config file.
                    string result = $"Database Connection: {data.OleDBStringWithoutPassword()}";
                    string path = AppDomain.CurrentDomain.BaseDirectory + @"\config.txt";
                    if (!File.Exists(path))
                        File.Create(path);
                    // Cannot write over a single line- have to write the entire file.
                    List<string> lines = new List<string>(File.ReadAllLines(path));
                    bool hit = false;
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (Regex.IsMatch(lines[i], "^Database Connection:", RegexOptions.IgnoreCase))
                        {
                            lines[i] = result;
                            hit = true;
                            break;
                        }
                    }
                    if (!hit)
                        lines.Add(result);
                    File.WriteAllLines(path, lines);
                }
                OleDBString = data.OleDBString;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get a string value from a form's data based on the custom variables or count category names defined in the config file.
        /// </summary>
        private static string GetAttributeValue(ImageFormContext imageContext, string attribute)
        {
            string value = attribute;
            // If the attribute is wrapped in single or double quotes, treat it as a literal stripped of those quotes.
            Match m = Regex.Match(attribute, "(?<=^(\"|'))(.*)(?=\\1$)", RegexOptions.IgnoreCase);
            // In case of duplicate key names, the priority is Count > Input > ImageValue.
            if (imageContext.CollectionCounts.ContainsKey(attribute))
            {
                value = imageContext.CollectionCounts[attribute].Value.ToString();
            }
            else if (imageContext.CollectionInputs.Any(x => x.Column == attribute))
            {
                value = imageContext.CollectionInputs.Where(x => x.Column == attribute).FirstOrDefault().Value.ToString();
            }
            else if (imageContext.CollectionImageValues.ContainsKey(attribute))
            {
                value = imageContext.CollectionImageValues[attribute].ToString();
            }
            else if (m.Success)
            {
                value = m.Value;
            }
            return value;
        }
        
        /// <summary>
        /// Get all ImageForms that are currently open.
        /// </summary>
        public ImageForm[] GetAllWindows()
        {
            List<ImageForm> l = new List<ImageForm>();
            foreach (Window w in Application.Current.Windows)
            {
                ImageForm f = w as ImageForm;
                if (f != null)
                {
                    l.Add(f);
                }
            }
            return l.ToArray();
        }

        /// <summary>
        /// Match all of the forms' ScaleFactor with that in basedon.
        /// </summary>
        public void ScaleAllWindows(ImageForm basedon)
        {
            DefaultScaleFactor = basedon.context.ScaleFactor;
            DefaultWindowSize = basedon.RenderSize;
            foreach (ImageForm form in GetAllWindows())
            {
                if (form != basedon)
                {
                    form.context.ScaleFactor = DefaultScaleFactor.Value;
                    form.Width = DefaultWindowSize.Value.Width;
                    form.Height = DefaultWindowSize.Value.Height;
                }
            }
        }

        /// <summary>
        /// Dynamically add KeyBindings to the window for each tool with a ShortKey.
        /// </summary>
        public void AddKeyBindings(UIElement sender, List<ButtonType> tools)
        {
            foreach (ButtonType tool in tools)
            {
                if (tool.ShortKeys.Length > 0)
                {
                    foreach (Key k in tool.ShortKeys)
                    {
                        KeyBinding kb = new KeyBinding()
                        {
                            Command = new RelayCommand(p => true, p => ActiveTool = tool),
                            Key = k
                        };
                        sender.InputBindings.Add(kb);
                    }
                }
            }
        }

        #endregion Shared Functions

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public static class CustomCommands
    {
        public static readonly RoutedUICommand SaveAll = new RoutedUICommand("SaveAll", "SaveAll", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift) });
        public static readonly RoutedUICommand Escape = new RoutedUICommand("Escape", "Escape", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.Escape) });
    }
    
    public class ButtonType : INotifyPropertyChanged
    {
        public string Category { get; set; }
        public Key[] ShortKeys { get; set; }
        public Color? Color { get; set; }
        public Cursor Cursor { get; }

        /// <summary>
        /// Bind XAML colors to this- not to the Color property.
        /// </summary>
        public SolidColorBrush Brush
        {
            get { return new SolidColorBrush(Color ?? Colors.White); }
        }

        /// <summary>
        /// Sometimes the display is different from the actual Category name. Can use XAML to decorate this.
        /// </summary>
        public Span DisplayCategory
        {
            get
            {
                if (_displayCategory == null)
                {
                    Span s = new Span();
                    s.Inlines.Add(Category);
                    return s;
                }
                else
                {
                    return _displayCategory;
                }
            }
            set
            {
                _displayCategory = value;
            }
        }
        private Span _displayCategory;

        /// <summary>
        /// Allow user to set all counts of a category to Visibility.Collapsed.
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                NotifyPropertyChanged();
            }
        }
        private bool _isVisible = true;

        /// <summary>
        /// Create a new tool button.
        /// </summary>
        /// <param name="Category">The count category or count button. Mostly used for display and maintaining image count sums.</param>
        /// <param name="Color">Color of the count dot and cursor.</param>
        /// <param name="Cursor">Cursor to use over the image count window.</param>
        /// <param name="ShortKey">The KeyBinding Key to press to activate this tool</param>
        public ButtonType(string Category, Color? Color, Cursor Cursor, params Key[] ShortKeys)
        {
            this.Category = Category;
            this.Color = Color;
            this.Cursor = Cursor;
            this.ShortKeys = ShortKeys;
        }

        /// <summary>
        /// Create a new tool button with a cursor created with CreateCursor().
        /// </summary>
        /// <param name="Category">The count category or count button. Mostly used for display and maintaining image count sums.</param>
        /// <param name="Color">Color of the count dot and cursor.</param>
        /// <param name="Cursor">Cursor to use over the image count window.</param>
        /// <param name="ShortKey">The KeyBinding Key to press to activate this tool</param>
        /// <param name="DisplayCategory">Have a different display than the actual Category name. Use XAML to decorate it.</param>
        public ButtonType(string Category, Color? Color, Span DisplayCategory, params Key[] ShortKeys)
        {
            this.Category = Category;
            this.Color = Color;
            this.ShortKeys = ShortKeys;
            this.DisplayCategory = DisplayCategory;
            Cursor = CreateCursor(Color);
        }

        /// <summary>
        /// Create a mouse cursor of any given color.
        /// </summary>
        /// <param name="color">The color of the cursor. If null, the cursor will be white.</param>
        private static Cursor CreateCursor(Color? color)
        {
            if (!color.HasValue)
                color = Colors.White;
            Point p0 = new Point(0, 0);
            Point p1 = new Point(12, 12);
            Point p2 = new Point(5, 12);
            Point p3 = new Point(0, 16);
            StreamGeometry geom = new StreamGeometry();
            using (StreamGeometryContext cont = geom.Open())
            {
                cont.BeginFigure(p0, true, true);
                cont.PolyLineTo(new PointCollection { p1, p2, p3 }, true, true);
            }
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext cont = visual.RenderOpen())
            {
                cont.DrawGeometry(new SolidColorBrush(color.Value), new Pen(Brushes.Black, 1), geom);
            }
            RenderTargetBitmap rtb = new RenderTargetBitmap(16, 16, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(visual);
            using (MemoryStream stream0 = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                encoder.Save(stream0);
                byte[] arr = stream0.ToArray();
                int size = arr.GetLength(0);
                using (MemoryStream stream1 = new MemoryStream())
                {
                    {//ICONDIR Structure
                        stream1.Write(BitConverter.GetBytes((Int16)0), 0, 2);//Reserved must be zero; 2 bytes
                        stream1.Write(BitConverter.GetBytes((Int16)2), 0, 2);//image type 1 = ico 2 = cur; 2 bytes
                        stream1.Write(BitConverter.GetBytes((Int16)1), 0, 2);//number of images; 2 bytes
                    }

                    {//ICONDIRENTRY structure
                        stream1.WriteByte(16); //image width in pixels
                        stream1.WriteByte(16); //image height in pixels

                        stream1.WriteByte(0); //Number of Colors in the color palette. Should be 0 if the image doesn't use a color palette
                        stream1.WriteByte(0); //reserved must be 0

                        stream1.Write(BitConverter.GetBytes((Int16)(0)), 0, 2);//2 bytes. In CUR format: Specifies the horizontal coordinates of the hotspot in number of pixels from the left.
                        stream1.Write(BitConverter.GetBytes((Int16)(0)), 0, 2);//2 bytes. In CUR format: Specifies the vertical coordinates of the hotspot in number of pixels from the top.

                        stream1.Write(BitConverter.GetBytes(size), 0, 4);//Specifies the size of the image's data in bytes
                        stream1.Write(BitConverter.GetBytes((Int32)22), 0, 4);//Specifies the offset of BMP or PNG data from the beginning of the ICO/CUR file
                    }
                    stream1.Write(arr, 0, size);
                    stream1.Seek(0, SeekOrigin.Begin);
                    return new Cursor(stream1);
                }
            }
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

    public class ColumnValuePair
    {
        public string Column { get; set; } = string.Empty;
        public object Value { get; set; } = null;
        public ColumnValuePair(string Column, object Value)
        {
            this.Column = Column;
            this.Value = Value;
        }
    }

    public class ReferencedTable
    {
        public string TableName = string.Empty;
        public Dictionary<string, string> ColumnValuePairs = new Dictionary<string, string>();
        public string[] ReturnScalar = new string[0];

        public ReferencedTable(string TableName, Dictionary<string, string> ColumnValuePairs, string[] ReturnScalar)
        {
            this.TableName = TableName;
            this.ColumnValuePairs = ColumnValuePairs;
            this.ReturnScalar = ReturnScalar;
        }
    }

    public abstract class Mark : INotifyPropertyChanged
    {
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }
        private bool _isSelected = false;

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
    /// Bind to the Ellipses that mark each count by category over the image.
    /// </summary>
    public class CountMark : Mark
    {
        public ButtonType Button { get; set; }
        public Point Location
        {
            get { return _location; }
            set
            {
                _location = value;
                NotifyPropertyChanged();
            }
        }
        private Point _location;

        public CountMark(ButtonType button, Point location)
        {
            Button = button;
            Location = location;
        }
    }

    /// <summary>
    /// Bind Geometry to a Path.Data, and use IsSelected to bind to Path.Stroke to highlight selected Paths.
    /// </summary>
    public class LineMark : Mark
    {
        public PathGeometry Geometry
        {
            get;
        }

        /// <summary>
        /// Create a new PathGeometry.
        /// </summary>
        /// <param name="start">The start point of the PathGeometry's PathFigure.</param>
        public LineMark(Point start)
        {
            Geometry = new PathGeometry();
            PathFigure f = new PathFigure(start, new PathSegment[] { }, false);
            Geometry.Figures.Add(f);
        }

        /// <summary>
        /// Already have a PathGeometry.
        /// </summary>
        /// <param name="Geometry"></param>
        public LineMark(string PathString)
        {
            var geometry = System.Windows.Media.Geometry.Parse(PathString);
            Geometry = new PathGeometry();
            Geometry.AddGeometry(geometry);
        }

        /// <summary>
        /// Add a point to an existing PathGeometry.
        /// </summary>
        /// <param name="location">The Point to add to the PathGeometry's PathFigure.</param>
        public void AddPoint(Point location)
        {
            PathFigure f = Geometry.Figures[0];
            f.Segments.Add(new LineSegment(location, true));
        }
    }
}
