using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using LevelXEditor.Project.ActionTabs;
using LevelXEditor.Project.Views;

namespace LevelXEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow instance;
        public ActionTabViewModal actionTabViewModel = new();
        public TabControl tabControl { get { return actionTabs; } }
        private List<Action<object, KeyEventArgs>> keyDownEvents = new();

        public MainWindow()
        {
            InitializeComponent();

            instance = this;

            // Initiate SubRoutines
            SubRoutines.Initiate();

            // Bind the xaml TabControl to view model tabs
            actionTabs.ItemsSource = actionTabViewModel.Tabs;

            // Populate the view model tabs
            AddTab(new Dashboard());

            // Preview key down event
            PreviewKeyDown += OnPreviewKeyDown;

            // Register global key down events
            RegisterMainWindowKeyDownEvents();
        }

        public static void RegisterKeyDownEvent(Action<object, KeyEventArgs> keyDownEvent)
        {
            instance.keyDownEvents.Add(keyDownEvent);
        }

        private void Button_CloseTab(object sender, RoutedEventArgs e)
        {
            // Find which index was clicked
            int index = actionTabs.Items.IndexOf(((Button)sender).DataContext);

            // Close tab
            CloseTab(index);

            // Handled
            e.Handled = true;
        }

        private void CloseTab(int index)
        {
            // If there is only 1 tab then bail
            if (actionTabs.Items.Count <= 1)
            {
                return;
            }

            // Give focus
            actionTabs.Focus();

            // If we are closing the last tab, move to the previous tab
            if (index == actionTabs.Items.Count - 1) actionTabs.SelectedIndex = index - 1;
            
            // If we are closing the first tab, move to the next tab
            else if (index == 0) actionTabs.SelectedIndex = index + 1;
            
            // If we are closing a tab in the middle, move to the previous tab
            else actionTabs.SelectedIndex = index - 1;

            // Remove tab from view model
            actionTabViewModel.Tabs.RemoveAt(index);
        }

        private void AddTab(UserControl userControl, int? index = null)
        {
            // Create new tab
            ActionTabItem actionTabItem = new(userControl);

            // Add tab to view model
            if (index == null) actionTabViewModel.Tabs.Add(actionTabItem);
            else actionTabViewModel.Tabs.Insert((int)index, actionTabItem);

            // Select tab
            actionTabs.SelectedItem = actionTabItem;

            // Hide close button
            if (actionTabViewModel.Tabs.Count <= 1) actionTabItem.CloseButtonVisibility = Visibility.Hidden;
        }


        private void Tab_Clicked(object sender, RoutedEventArgs e)
        {
            // Get tab object
            var tabObject = ((StackPanel)sender).DataContext;

            // If middle click, close the tab
            if (e is MouseButtonEventArgs && ((MouseButtonEventArgs)e).ChangedButton == MouseButton.Middle)
            {
                CloseTab(actionTabs.Items.IndexOf(tabObject));
            }
        }

        private void MenuItem_File_Button(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            // New
            if (menuItem.Name == "MenuItem_File_New")
            {
                LevelEditor levelEditor = new(){ Tag = "New Level.lvl (" + actionTabViewModel.Tabs.Count + ")" };
                AddTab(levelEditor);

                // Once level editor is loaded and ready to scroll, scroll to center
                SubRoutines.WaitUntil(() => levelEditor.levelScrollViewer != null && levelEditor.levelScrollViewer.IsLoaded, () => {
                    levelEditor.levelScrollViewer.ScrollToCenter();
                });
            }
            // Quit
            else if(menuItem.Name == "MenuItem_File_Quit")
            {
                Application.Current.Shutdown();
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Call all key down events
            for (int i = 0; i < keyDownEvents.Count; i++)
            {
                keyDownEvents[i](sender, e);
            }
        }

        private void RegisterMainWindowKeyDownEvents()
        {
            // New level (Ctrl + N)
            RegisterKeyDownEvent((object sender, KeyEventArgs e) => {
                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.N)
                {
                    MenuItem_File_Button(MenuItem_File_New, new RoutedEventArgs());
                }
            });

            // Close tab (Ctrl + W)
            RegisterKeyDownEvent((object sender, KeyEventArgs e) => {
                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.W)
                {
                    // Get stack panel
                    StackPanel stackPanel = actionTabs.GetItemContainer().GetContentPresenter().FindContentByName<StackPanel>("stackPanel");
                    
                    // Get Close Button
                    Button closeButton = stackPanel.FindChildOfType<Button>((button) => button.Name == "button_CloseTab");
                    
                    // Call close button event
                    Button_CloseTab(closeButton, e);
                }
            });
        }
    }
}
