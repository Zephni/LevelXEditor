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
        public ActionTabViewModal actionTabsModel;
        private static List<Action<object, KeyEventArgs>> keyDownEvents = new();

        public MainWindow()
        {
            InitializeComponent();

            // Initiate SubRoutines
            SubRoutines.Initiate();

            // Create action tabs view model and bind to xaml
            actionTabsModel = new(actionTabs);
            actionTabs.ItemsSource = actionTabsModel.Tabs;

            // Populate the view model tabs
            actionTabsModel.AddTab(new Dashboard());

            // Preview key down event
            PreviewKeyDown += OnPreviewKeyDown;

            // Register global key down events
            RegisterMainWindowKeyDownEvents();
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

        private void MenuItem_File_Button(object sender, RoutedEventArgs e)
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
