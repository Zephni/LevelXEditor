using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using LevelXEditor.Project.ActionTabs;
using LevelXEditor.Project.Views;

namespace LevelXEditor
{
    public class ApplicationMenuHandler
    {
        public void CallAssociatedMethod(MenuItem menuItem, RoutedEventArgs e)
        {
            // Get the method name
            string methodName = (string)menuItem.Tag;

            // Get the method
            MethodInfo? method = GetType().GetMethod(methodName) ?? null;

            if(method == null)
            {
                MessageBox.Show("Failed to find method: " + methodName, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Call the method
            method?.Invoke(this, new object[] { menuItem.ToolTip ?? new object(), e });
        }

        // File -> New
        public void File_New(object parameter, RoutedEventArgs e){
            LevelEditor levelEditor = new(){ Tag = "New Level.lvl (" + MainWindow.instance.actionTabsModel.Tabs.Count + ")" };
            MainWindow.instance.actionTabsModel.AddTab(levelEditor);

            // Once level editor is loaded and ready to scroll, scroll to center
            SubRoutines.WaitUntil(() => levelEditor.levelScrollViewer != null && levelEditor.levelScrollViewer.IsLoaded, () => {
                levelEditor.levelScrollViewer.ScrollToCenter();
            });
        }

        // File -> Open
        public void File_Open(object parameter, RoutedEventArgs e = null){
            // Create level editor
            LevelEditor levelEditor = new();

            // Check if menu item tag set
            bool filePassed = parameter != null && parameter is string && (string)parameter != "";

            // Load level
            bool success = levelEditor.LevelDataHandler.Load(filePassed && parameter != null ? (string)parameter : null, levelEditor);

            // Bail if not successful or window canceled
            if(success == false) {
                return;
            }

            // Add tab
            MainWindow.instance.actionTabsModel.AddTab(levelEditor);
        }

        // File -> Save
        public void File_Save(object parameter, RoutedEventArgs e){
            // Check is enabled
            if(Utilities.GetApplicationMenuItem("_File", "_Save").IsEnabled == false)
            {
                MessageBox.Show("No level editor is open.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Get the correct LevelDataHandler object
            ActionTabItem? actionTabItem = (ActionTabItem)MainWindow.instance.actionTabsModel.tabControl.SelectedItem;
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

        // File -> Quit
        public void File_Quit(object parameter, RoutedEventArgs e){
            Application.Current.Shutdown();
        }
    }
}