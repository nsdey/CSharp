using QA_Master.Models.Core;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace QA_Master.Forms.UserControls
{
    /// <summary>
    /// Interaction logic for ListBoxWithButtons.xaml
    /// </summary>
    public partial class AdvancedListControl : UserControl, INotifyPropertyChanged
    {
        public enum EnumButtonType
        {
            Add,
            AddSimilar,
            Up,
            Down,
            Edit,
            Run,
            Delete,
        }

        private string? title = "Advanced List Control";
        public string? Title {
            get { return title; }
            set { title = value; OnPropertyChanged(); }
        }

        private bool hideAddButton;
        public bool HideAddButton
        {
            get { return hideAddButton; }
            set { hideAddButton = value; btnAdd.Visibility = Visibility.Collapsed; OnPropertyChanged(); }
        }

        private bool hideAddSimilarButton;
        public bool HideAddSimilarButton
        {
            get { return hideAddSimilarButton; }
            set { hideAddSimilarButton = value; btnAddSimilar.Visibility = Visibility.Collapsed; OnPropertyChanged(); }
        }

        private string runButtonText = "Run...";

        public string RunButtonTooltip
        {
            get { return runButtonText; }
            set { runButtonText = value; btnRun.ToolTip = runButtonText; OnPropertyChanged(); }
        }

        private bool hideRunButton;
        public bool HideRunButton
        {
            get { return hideRunButton; }
            set { hideRunButton = value; btnRun.Visibility = Visibility.Collapsed; OnPropertyChanged(); }
        }

        private bool hideEditButton;
        public bool HideEditButton
        {
            get { return hideEditButton; }
            set { hideEditButton = value; btnEdit.Visibility = Visibility.Collapsed; OnPropertyChanged(); }
        }

        private IEnumerable? listCollection;
        public IEnumerable? ListCollection
        {
            get { return listCollection; }
            set
            {
                listCollection = value;
                lstBox.ItemsSource = listCollection;
                btnAdd.IsEnabled = listCollection != null;
                OnPropertyChanged();
            }
        }

        private object? selectedItem;
        public object? SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem != null && value != null && selectedItem?.GetType() != value?.GetType())
                {
                    if (listCollection != null)
                    {
                        IList list = (IList)listCollection;
                
                        int index = 0;
                
                        index = list.IndexOf(selectedItem);
                
                        if (index > 0)
                        {
                            list.RemoveAt(index);
                            list.Insert(index, value);

                            selectedItem = value;
                            OnPropertyChanged();
                        }
                    }
                }
                else
                {
                    selectedItem = value;
                    OnPropertyChanged();
                }

                //selectedItem = value;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class ButtonClickEventArgs : RoutedEventArgs
        {
            public EnumButtonType ButtonType;
            public object? SelectedItem;
            public ButtonClickEventArgs(EnumButtonType buttonType, object? selectedItem)
            {
                ButtonType = buttonType;
                SelectedItem = selectedItem;
            }
        }
        public delegate void _ButtonClick(object sender, ButtonClickEventArgs e);
        public event _ButtonClick? ButtonClick = null;

        public class SelectedItemChangedEventArgs : RoutedEventArgs
        {
            public object? SelectedItem;
            public SelectionChangedEventArgs SelectionChangedEventArgs;

            public SelectedItemChangedEventArgs(object? selectedItem, SelectionChangedEventArgs selectionChangedEventArgs)
            {
                SelectedItem = selectedItem;
                SelectionChangedEventArgs = selectionChangedEventArgs;
            }
        }
        public delegate void _SelectedItemChanged(object sender, SelectedItemChangedEventArgs e);
        public event _SelectedItemChanged? SelectedItemChanged = null;

        public AdvancedListControl()
        {
            DataContext = this;
            
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (listCollection != null) ButtonClick?.Invoke(this, new ButtonClickEventArgs(EnumButtonType.Add, SelectedItem));
        }

        private void btnAddSimilar_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItem != null) ButtonClick?.Invoke(this, new ButtonClickEventArgs(EnumButtonType.AddSimilar, SelectedItem));
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            int index = lstBox.SelectedIndex;

            MethodInfo? methodInfo = listCollection?.GetType().GetMethod("Move");
            methodInfo?.Invoke(listCollection, [index, index - 1]);

            Perform_lstBox_SelectionChanged();
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            int index = lstBox.SelectedIndex;

            MethodInfo? methodInfo = listCollection?.GetType().GetMethod("Move");
            methodInfo?.Invoke(listCollection, [index, index + 1]);

            Perform_lstBox_SelectionChanged();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItem != null) ButtonClick?.Invoke(this, new ButtonClickEventArgs(EnumButtonType.Edit, SelectedItem));
        }
        private void lstBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (selectedItem != null) ButtonClick?.Invoke(this, new ButtonClickEventArgs(EnumButtonType.Edit, SelectedItem));
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItem != null) ButtonClick?.Invoke(this, new ButtonClickEventArgs(EnumButtonType.Run, SelectedItem));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listCollection != null && selectedItem != null)
            {
                if (MessageBox.Show
                    (
                        Window.GetWindow(this),
                        $"Are you sure to delete {selectedItem} ?",
                        "Confirm delete",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question
                    ) == MessageBoxResult.Yes)
                {
                    MethodInfo? methodInfo = listCollection.GetType().GetMethod("Remove");
                    methodInfo?.Invoke(listCollection, [selectedItem]);
                }
            }
        }

        private void lstBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Perform_lstBox_SelectionChanged();

            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem, e));
        }

        private void Perform_lstBox_SelectionChanged()
        {
            lstBox.ScrollIntoView(lstBox.SelectedItem);
            selectedItem = lstBox.SelectedItem;

            btnUp.IsEnabled = (lstBox.SelectedIndex > 0);
            btnDown.IsEnabled = (lstBox.SelectedIndex < lstBox.Items.Count - 1) && (lstBox.SelectedIndex >= 0);

            btnAddSimilar.IsEnabled = (selectedItem != null);
            btnEdit.IsEnabled = (selectedItem != null);
            btnRun.IsEnabled = (selectedItem != null);
            btnDelete.IsEnabled = (selectedItem != null);
        }

        public void AddNewItem(object? item)
        {
            if (item != null)
            {
                MethodInfo? methodInfo = listCollection?.GetType().GetMethod("Add");
                methodInfo?.Invoke(listCollection, [item]);

                SelectedItem = item;
            }
        }

        public void AddNewItems(IEnumerable? items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    AddNewItem(item);
                }
            }
        }
    }
}
