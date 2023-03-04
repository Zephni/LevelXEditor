using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using Microsoft.Win32;
using LevelXEditor.Project.Views;

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

            // Convert data dictionary to JSON
            string json = JsonSerializer.Serialize(levelData);

            // Write JSON to file
            System.IO.File.WriteAllText(path, json);
        }

        public bool Load(string? path, LevelEditor levelEditor)
        {
            // If path is null then open a save dialog
            if (path == null)
            {
                path = OpenLoadDialog();
            }

            // If path is still null then return
            if (path == null)
            {
                return false;
            }

            // Read JSON from file
            string json = System.IO.File.ReadAllText(path);

            // Convert JSON back to LevelData object
            var _levelData = JsonSerializer.Deserialize<LevelData>(json);

            // If the deserialization was successful then update the levelData object
            if (_levelData != null)
            {
                levelData = _levelData;
                levelData.OnLoad(path, levelEditor);
                return true;
            }
            else
            {
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