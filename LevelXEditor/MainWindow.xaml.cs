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
        public ActionTabViewModal actionTabsModel;
        public Menu ApplicationMenu => applicationMenu;
        public LevelEditor? CurrentLevelEditor { get => (LevelEditor)((ActionTabItem)actionTabsModel.tabControl.SelectedItem).UserControl ?? null; }
        private static List<Action<object, KeyEventArgs>> keyDownEvents = new();

        public MainWindow()
        {
            InitializeComponent();

            // Set instance
            instance = this;

            // Initiate SubRoutines
            SubRoutines.Initiate();

            // Set Icon
            Icon = new BitmapImage(new Uri("./Resources/Icons/ApplicationIcon.png", UriKind.Relative));

            // Create action tabs view model and bind to xaml
            actionTabsModel = new(actionTabs);

            // Populate the view model tabs
            actionTabsModel.AddTab(new Dashboard());

            // Preview key down event
            PreviewKeyDown += OnPreviewKeyDown;

            // Register global key down events
            RegisterMainWindowKeyDownEvents();

            // Handle file drops, and attempt to open file
            PreviewDragOver += (sender, e) => e.Handled = true;
            PreviewDrop += (sender, e) => {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (files.Length > 0)
                    {
                        foreach(string file in files)
                        {
                            MenuItem_File_Button(new MenuItem() { Name = "MenuItem_File_Open", Tag = file }, new RoutedEventArgs());
                        }
                    }
                }
            };
        }

        public static void RegisterKeyDownEvent(Action<object, KeyEventArgs> keyDownEvent)
        {
            keyDownEvents.Add(keyDownEvent);
        }

        private void Button_CloseTab(object sender, RoutedEventArgs e)
        {
            // Find which index was clicked
            int index = actionTabsModel.tabControl.Items.IndexOf(((Button)sender).DataContext);

            // Close tab
            actionTabsModel.CloseTab(index);

            // Handled
            if(e != null)
            {
                e.Handled = true;
            }
        }

        private void Tab_Clicked(object sender, RoutedEventArgs e)
        {
            // Get tab object
            var tabObject = ((StackPanel)sender).DataContext;

            // If middle click, close the tab
            if (e is MouseButtonEventArgs && ((MouseButtonEventArgs)e).ChangedButton == MouseButton.Middle)
            {
                actionTabsModel.CloseTab(actionTabsModel.tabControl.Items.IndexOf(tabObject));
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If there are no tabs, return
            if (actionTabsModel.tabControl.Items.Count == 0)
            {
                return;
            }

            // Get the selected tab
            ActionTabItem selectedTab = (ActionTabItem)actionTabsModel.tabControl.SelectedItem;

            // If the selected tab is null, return
            if (selectedTab == null)
            {
                return;
            }

            // If the selected tab is a level editor, enable / disable menu valid buttons
            if (selectedTab.UserControl is LevelEditor levelEditor)
            {
                Utilities.GetApplicationMenuItem("_File", "_Save").IsEnabled = true;
            }
            else
            {
                Utilities.GetApplicationMenuItem("_File", "_Save").IsEnabled = false;
            }
        }

        public void MenuItem_File_Button(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            // New
            if (menuItem.Name == "MenuItem_File_New")
            {
                LevelEditor levelEditor = new(){ Tag = "New Level.lvl (" + actionTabsModel.Tabs.Count + ")" };
                actionTabsModel.AddTab(levelEditor);

                // Once level editor is loaded and ready to scroll, scroll to center
                SubRoutines.WaitUntil(() => levelEditor.levelScrollViewer != null && levelEditor.levelScrollViewer.IsLoaded, () => {
                    levelEditor.levelScrollViewer.ScrollToCenter();
                });
            }
            // Open
            else if(menuItem.Name == "MenuItem_File_Open")
            {
                // Create level editor
                LevelEditor levelEditor = new();

                // Check if menu item tag set
                bool filePassed = menuItem.Tag != null && menuItem.Tag is string && (string)menuItem.Tag != "";

                // Load level
                bool success = levelEditor.LevelDataHandler.Load(filePassed ? (string)menuItem.Tag : null, levelEditor);

                // Check valid
                if(success == false)
                {
                    return;
                }

                // Add tab
                actionTabsModel.AddTab(levelEditor);
            }
            // Save
            else if(menuItem.Name == "MenuItem_File_Save")
            {
                // Check is enabled
                if(Utilities.GetApplicationMenuItem("_File", "_Save").IsEnabled == false)
                {
                    MessageBox.Show("No level editor is open.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Get the correct LevelDataHandler object
                ActionTabItem? actionTabItem = (ActionTabItem)actionTabsModel.tabControl.SelectedItem;
                LevelEditor? levelEditor = (actionTabItem != null && actionTabItem.UserControl != null) ? (LevelEditor)actionTabItem.UserControl : null;

                if(levelEditor != null)
                {
                    levelEditor.LevelDataHandler.Save();
                }
                else
                {
                    MessageBox.Show("No level editor is open.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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

            // Save level
            RegisterKeyDownEvent((object sender, KeyEventArgs e) => {
                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
                {
                    MenuItem_File_Button(MenuItem_File_Save, new RoutedEventArgs());
                }
            });

            // Open level
            RegisterKeyDownEvent((object sender, KeyEventArgs e) => {
                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.O)
                {
                    MenuItem_File_Button(MenuItem_File_Open, new RoutedEventArgs());
                }
            });

            // Close tab (Ctrl + W)
            RegisterKeyDownEvent((object sender, KeyEventArgs e) => {
                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.W)
                {
                    // If a tab item exists, close it
                    if(actionTabsModel.tabControl.Items.Count > 0)
                    {
                        actionTabsModel.CloseTab(actionTabsModel.tabControl.SelectedIndex);
                    }
                }
            });
        }
    }
}
