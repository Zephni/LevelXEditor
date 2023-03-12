using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using Microsoft.Win32;
using LevelXEditor.Project.Views;
using System.Windows;

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
            Dashboard.Refresh();
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

                // Update the levelData object
                levelData = _levelData;
                levelData.OnLoad(path, levelEditor);

                // Update recent files
                MainWindow.AppDataHandler.ModifyData(data => {
                    data.AddRecentFile(path);
                });
                
                // Refresh any nessesary UI elements
                Dashboard.Refresh();
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
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Show the dialog and get the result
            bool? result = saveFileDialog.ShowDialog();

            // If the user clicked OK, save the file, otherwise return null
            return (result == true) ? saveFileDialog.FileName : null;
        }

        public string? OpenLoadDialog()
        {
            // Create a OpenFileDialog instance
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set some properties for the dialog
            openFileDialog.Filter = "LVLX files (*.lvlx)|*.lvlx|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Show the dialog and get the result
            bool? result = openFileDialog.ShowDialog();

            // If the user clicked OK, save the file, otherwise return null
            return (result == true) ? openFileDialog.FileName : null;
        }
    }
}