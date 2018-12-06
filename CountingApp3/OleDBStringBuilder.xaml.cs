using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.ComponentModel;

namespace CountingApp3
{
    /// <summary>
    /// Interaction logic for OleDBStringBuilder.xaml
    /// </summary>
    public partial class OleDBStringBuilder : Window
    {
        public OleDBStringBuilder(string ConnectionString)
        {
            InitializeComponent();
            ((OleDBBuilderContext)DataContext).OleDBParser(ConnectionString);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            OleDBBuilderContext context = DataContext as OleDBBuilderContext;
            if (context != null)
            {
                context.Password = ((PasswordBox)sender).Password;
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            ((OleDBBuilderContext)DataContext).Save = true;
            ReturnTrueDialogResult();
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            ReturnTrueDialogResult();
        }

        /// <summary>
        /// For Save and Apply buttons- test the connection and return true. 
        /// Make sure that ButtonSave_Click sets the context property Save = true.
        /// </summary>
        private void ReturnTrueDialogResult()
        {
            OleDBBuilderContext context = DataContext as OleDBBuilderContext;
            using (OleDbConnection connection = new OleDbConnection(context.OleDBString))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    MessageBoxResult result = MessageBox.Show($"Testing the connection string '{context.OleDBString}' created the following error: {ex.Message}{Environment.NewLine}{Environment.NewLine}Continue using this connection string?", "Warning", MessageBoxButton.OKCancel);
                    if (result != MessageBoxResult.OK)
                        return;
                }
            }
            DialogResult = true;
        }
    }

    public enum TypeOptions { SQLServer, Oracle, PostgreSQL, Excel, OleDBString }

    public class OleDBBuilderContext : INotifyPropertyChanged
    {

        public string OleDBString
        {
            get { return _oledbString; }
            set
            {
                _oledbString = value;
                NotifyPropertyChanged();
            }
        }

        public TypeOptions Type
        {
            get { return _type; }
            set
            {
                _type = value;
                NotifyPropertyChanged();
                UpdateOleDBString();
            }
        }

        public string Server
        {
            get { return _server; }
            set
            {
                _server = value;
                NotifyPropertyChanged();
                UpdateOleDBString();
            }
        }

        public int? Port
        {
            get { return _port; }
            set
            {
                _port = value;
                NotifyPropertyChanged();
                UpdateOleDBString();
            }
        }

        public string Database
        {
            get { return _database; }
            set
            {
                _database = value;
                NotifyPropertyChanged();
                UpdateOleDBString();
            }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyPropertyChanged();
                UpdateOleDBString();
            }
        }

        public bool Save = false;

        /// <summary>
        /// Using a string rather than a SecureString because have to use a string anyways in the OleDB connection.
        /// </summary>
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                UpdateOleDBString();
            }
        }

        public OleDBBuilderContext()
        {

        }

        private void UpdateOleDBString()
        {
            /********* OleDB String Formats **********
             * SQL Server: Provider=SQLNCLI11;Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;
             * SQL Server: Provider=SQLNCLI11;Server=myServerAddress;Database=myDataBase;Trusted_Connection=yes;
             * Oracle: Provider=msdaora;Data Source=MyOracleDB;User Id=myUsername;Password=myPassword;
             * Oracle: Provider=msdaora;Data Source=MyOracleDB;Persist Security Info=False;Integrated Security=Yes;
             * PostgreSQL: Provider=PostgreSQL OLE DB Provider;Data Source=myServerAddress;location=myDataBase;User ID=myUsername;password=myPassword;
             * Excel: Excel File=C:\myExcelFile.xlsx;
            ****************************************/
            switch (Type)
            {
                case TypeOptions.SQLServer:
                    if (!string.IsNullOrWhiteSpace(UserName))
                        OleDBString = $"Provider=SQLNCLI11;Server={Server};Database={Database};Uid={UserName};Pwd={Password};";
                    else
                        OleDBString = $"Provider=SQLNCLI11;Server={Server};Database={Database};Trusted_Connection=yes;";
                    break;
                case TypeOptions.Oracle:
                    if (!string.IsNullOrWhiteSpace(UserName))
                        OleDBString = $"Provider=msdaora;Data Source={Server};User Id={UserName};Password={Password};";
                    else
                        OleDBString = $"Provider=msdaora;Data Source={Server};Persist Security Info=False;Integrated Security=Yes;";
                    break;
                case TypeOptions.PostgreSQL:
                    OleDBString = $"Provider=PostgreSQL OLE DB Provider;Data Source={Server};location={Database};User ID={UserName};password={Password};";
                    break;
                case TypeOptions.Excel:
                    OleDBString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={Server};Extended Properties=\"Excel 12.0;HDR=YES;\"";
                    break;
            }
        }

        public string OleDBStringWithoutPassword()
        {
            string result = Regex.Replace(OleDBString, @"(?<=(Pwd\s*=)|(Password\s*=)).*?(?=;|$)", "", RegexOptions.IgnoreCase);
            return result;
        }

        public void OleDBParser(string ConnectionString)
        {
            Match mType = Regex.Match(ConnectionString, @"((?<=Provider\s*=).*?(?=;))|(^Excel)", RegexOptions.IgnoreCase);
            Match mServer = Regex.Match(ConnectionString, @"(?<=(Server\s*=)|(Data Source\s*=)|(File\s*=)).*?(?=;)", RegexOptions.IgnoreCase);
            Match mDatabase = Regex.Match(ConnectionString, @"(?<=(Database\s*=)|(location\s*=)).*?(?=;)", RegexOptions.IgnoreCase);
            Match mUserName = Regex.Match(ConnectionString, @"(?<=(Uid\s*=)|(User Id\s*=)).*?(?=;)", RegexOptions.IgnoreCase);
            if (mType.Success)
            {
                switch (mType.Value.ToUpper())
                {
                    case "SQLNCLI11": Type = TypeOptions.SQLServer; break;
                    case "MSDAORA": Type = TypeOptions.Oracle; break;
                    case "POSTGRESQL": Type = TypeOptions.PostgreSQL; break;
                    case "EXCEL": Type = TypeOptions.Excel; break;
                    default: Type = TypeOptions.OleDBString; break;
                }
            }
            if (mServer.Success)
                Server = mServer.Value;
            else
                Server = string.Empty;
            if (mDatabase.Success)
                Database = mDatabase.Value;
            else
                Database = string.Empty;
            if (mUserName.Success)
                UserName = mUserName.Value;
            else
                UserName = string.Empty;
        }

        private string _oledbString = string.Empty;
        private TypeOptions _type = TypeOptions.SQLServer;
        private string _server = string.Empty;
        private int? _port = null;
        private string _database = string.Empty;
        private string _userName = string.Empty;
        private string _password = string.Empty;
        private string _fileName = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

    public class EnumBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter);
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool)value) ? parameter : Binding.DoNothing;
        }
    }


}
