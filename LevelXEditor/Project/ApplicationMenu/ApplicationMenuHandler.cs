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
        public void File_Open(object parameter, RoutedEventArgs e){
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

        // File -> Recent Files -> Clear Recent Files
        public void File_ClearRecentFiles(object parameter, RoutedEventArgs e){
            // Clear recent files
            MainWindow.AppDataHandler.ModifyData(data => {
                data.recentFiles = new string[0];
            });

            Dashboard.Refresh();
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
                string localFilePath = levelEditor.LevelDataHandler.levelData.editor_localFilePath;
                levelEditor.LevelDataHandler.Save(localFilePath != "" ? localFilePath : null);
            }
            else
            {
                MessageBox.Show("No level editor is open.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // File -> Save As
        public void File_SaveAs(object parameter, RoutedEventArgs e){
            // Check is enabled
            if(Utilities.GetApplicationMenuItem("_File", "_Save as").IsEnabled == false)
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
    
        // Go -> Dashboard
        public void Go_Dashboard(object parameter, RoutedEventArgs e){
            // Check if dashboard is already open
            if(MainWindow.instance.actionTabsModel.IsTabOpen("Dashboard"))
            {
                // Switch to dashboard
                MainWindow.instance.actionTabsModel.SwitchToTab("Dashboard");
                return;
            }
            else
            {
                // Add tab
                MainWindow.instance.actionTabsModel.AddTab(new Dashboard());
            }
        }
    
        // Go -> Local AppData Directory
        public void Go_AppData(object parameter, RoutedEventArgs e){
            // AppData path
            string appDataPath = MainWindow.AppDataHandler.GetDirectoryPath();

            // Open AppData directory in windows explorer
            System.Diagnostics.Process.Start("explorer.exe", appDataPath);
        }
    }
}