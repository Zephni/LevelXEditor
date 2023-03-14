using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using Microsoft.Win32;
using LevelXEditor.Project.Views;
using System.Windows;
using LevelXEditor.Project.ActionTabs;

namespace LevelXEditor
{
    public partial class LevelDataHandler
    {
        public LevelData levelData = new();

        public void Save(string? path = null)
        {
            // If path is null then open a save dialog
            if (path == null)
            {
                path = OpenSaveDialog();
            }

            // If path is still null then return
            if (path == null)
            {
                return;
            }

            // Update file path
            levelData.OnSave(path);

            // Convert data dictionary to XML
            string xml = levelData.Serialize();

            // Write XML to file
            System.IO.File.WriteAllText(path, xml);

            // Update recent files
            MainWindow.AppDataHandler.ModifyData(data => {
                data.AddRecentFile(path);
            });

            // Refresh any nessesary UI elements
            MainWindow.instance.RefreshUI();
        }

        public bool Load(string? path, LevelEditor levelEditor)
        {
            // If path is null then open a save dialog
            if (path == null)
            {
                path = OpenLoadDialog();
            }

            // If path is still null then return
            if (path == null) return false;

            // If the file doesn't exist then show message return
            if (!System.IO.File.Exists(path)) {
                MessageBox.Show("The file does not exist: "+path, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Read XML from file
            string xml = System.IO.File.ReadAllText(path);

            // If JSON is empty then message and return
            if (xml == null || xml == "") return false;

            try
            {
                // Convert JSON back to LevelData object
                var _levelData = LevelData.DeserializeFrom(xml);

                // If there is any level open with the same local file path then show message and return
                ActionTabItem? levelTabThatMayExist = MainWindow.instance.actionTabsModel.GetTabWhere(tab => tab.UserControl is LevelEditor levelEditor && levelEditor.LevelDataHandler.levelData.editor_localFilePath == path);
                if (levelTabThatMayExist != null) {
                    MessageBox.Show("This level file is already open, switching to tab for:\n"+path, "Level Already Open", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow.instance.actionTabsModel.SwitchToTab(levelTabThatMayExist);
                    return false;
                }

                // Update the levelData object
                levelData = _levelData;
                levelData.OnLoad(path, levelEditor);

                // Update recent files
                MainWindow.AppDataHandler.ModifyData(data => {
                    data.AddRecentFile(path);
                });
                
                // Refresh any nessesary UI elements
                MainWindow.instance.RefreshUI();
            
                return true;
            }
            catch(Exception e)
            {
                MessageBox.Show("Failed to load level: "+e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public string? OpenSaveDialog()
        {
            // Create a SaveFileDialog instance
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // Set some properties for the dialog
            saveFileDialog.Filter = "LVLX files (*.lvlx)|*.lvlx|All files (*.*)|*.*";

            // Show the dialog and get the result
            bool? result = saveFileDialog.ShowDialog();

            // If the user clicked OK, save the file, otherwise return null
            if(result == true) {
                MainWindow.AppDataHandler.ModifyData(data => data.lastUsedDirectory = saveFileDialog.FileName);
                return saveFileDialog.FileName;
            } else {
                return null;
            }
        }

        public string? OpenLoadDialog()
        {
            // Create a OpenFileDialog instance
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set some properties for the dialog
            openFileDialog.Filter = "LVLX files (*.lvlx)|*.lvlx|All files (*.*)|*.*";

            // Show the dialog and get the result
            bool? result = openFileDialog.ShowDialog();

            // If the user clicked OK, save the file, otherwise return null
            if(result == true) {
                MainWindow.AppDataHandler.ModifyData(data => data.lastUsedDirectory = openFileDialog.FileName);
                return openFileDialog.FileName;
            } else {
                return null;
            }
        }
    }
}