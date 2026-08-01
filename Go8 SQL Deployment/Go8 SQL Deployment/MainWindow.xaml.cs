using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace Go8_SQL_Deployment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            cmbServer.ItemsSource = AppConfig.SQLServers;
        }

        DirectoryClass? directoryClass = null;

        private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            string? rootFolder = "C:\\Go8\\Blasters\\SqlScripts";

            OpenFolderDialog openFolderDialog = new OpenFolderDialog() { InitialDirectory = rootFolder };
            if (openFolderDialog.ShowDialog(this) == true)
            {
                directoryClass = new(openFolderDialog.FolderName, "*.sql");

                tvSQLFiles.ItemsSource = directoryClass.Items;
                cmbFilter.ItemsSource = CommonClass.DateOnlysSorted;
            }
        }

        private void cmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (directoryClass != null)
            {
                CommonClass.Filter = (FilterDate)cmbFilter.SelectedItem;

                tvSQLFiles.ItemsSource = null;
                tvSQLFiles.ItemsSource = directoryClass.Items;
            }
        }

        private async void btnRun_Click(object sender, RoutedEventArgs e)
        {
            if (cmbServer.SelectedItem == null)
            {
                MessageBox.Show(this, "Select Server...", "Server", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            stkMain.IsEnabled = false;

            try
            {
                DirectoryClass? directoryClass = ((FileClass)tvSQLFiles.SelectedItem).ParentDirectory;
                CommonClass? commonClass = (CommonClass)tvSQLFiles.SelectedItem;

                if (directoryClass != null && commonClass != null)
                {
                    await Task.Run(async () => { await RunSQLs(directoryClass, directoryClass.Children.IndexOf(commonClass)); });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            stkMain.IsEnabled = true;
        }

        private async Task<bool> RunSQLs(DirectoryClass? directoryClass, int startIndex)
        {
            if (directoryClass == null) return false;

            ServerConnection? serverConnection = null;
            Server? server = null;

            FileClass? fileClass = null;

            try
            {
                await Dispatcher.Invoke(async ()=>
                {
                    SQLServer? sQLServer = (SQLServer)cmbServer.SelectedItem;

                    serverConnection = new(sQLServer.Server, sQLServer.UserId, sQLServer.Password);                    

                    for (int i = startIndex; i < directoryClass.Children.Count; i++)
                    {
                        if (directoryClass.Children[i] is not null and FileClass)
                        {
                            fileClass = (FileClass)directoryClass.Children[i];
                            fileClass.Selected = true;

                            fileClass.RunStatus = RunStatus.Running;
                            await Task.Delay(100);

                            server ??= new(serverConnection);

                            fileClass.Read();
                            await Task.Delay(100);

                            fileClass.RowsAffected = server.ConnectionContext.ExecuteNonQuery(fileClass.FileContent, ExecutionTypes.Default);
                            
                            fileClass.RunStatus = RunStatus.Success;
                            fileClass.Selected = true;
                            await Task.Delay(100);

                            if (chkRunNext.IsChecked == false) break; 
                        }
                    }
                });
            }
            catch
            {
                Dispatcher.Invoke(() => { if (fileClass != null) fileClass.RunStatus = RunStatus.Failed; });
                await Task.Delay(1);

                throw;
            }

            return true;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            FileEditor fileEditor = new() { Owner = this };
            ((FileClass)tvSQLFiles.SelectedItem).Read();
            fileEditor.txtEditor.Text = ((FileClass)tvSQLFiles.SelectedItem).FileContent;

            if (fileEditor.ShowDialog() == true)
            {
                ((FileClass)tvSQLFiles.SelectedItem).FileContent = fileEditor.txtEditor.Text;
            }
        }

        private void tvSQLFiles_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            stkOperation.IsEnabled = tvSQLFiles.SelectedItem is FileClass;
        }
    }
}