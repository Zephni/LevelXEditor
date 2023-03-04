using System.Text.Json.Serialization;
using System.Windows;
using LevelXEditor.Project.ActionTabs;
using LevelXEditor.Project.Views;

namespace LevelXEditor
{
    public class LevelData
    {
        // You can use [JsonInclude] or [JsonIgnore] to include or exclude a property from serialization
        // Also use [JsonPropertyName("someName")] to change the name of the property in the JSON file
        // Finally use [JsonConverter(typeof(JsonStringEnumConverter))] to convert enums to strings in the JSON file

        public string editor_localFilePath { get; set; } = "";

        public Point editor_scrollPosition { get; set; } = new(0, 0);

        public void OnSave(string path)
        {
            LevelEditor? levelEditor = MainWindow.instance.CurrentLevelEditor;

            if(levelEditor == null){
                MessageBox.Show("Cannot save level a level editor tab is not focused", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Set tab header
            levelEditor.Tag = System.IO.Path.GetFileName(path);
            MainWindow.instance.actionTabsModel.UpdateTabHeaders();

            // Set local file path
            editor_localFilePath = path;

            // Get scroll position
            editor_scrollPosition = levelEditor.levelScrollViewer.GetScrollOffset();
        }

        public void OnLoad(string path, LevelEditor levelEditor)
        {
            // Set local file path
            editor_localFilePath = path;

            // Set tab header
            levelEditor.Tag = System.IO.Path.GetFileName(path);
            MainWindow.instance.actionTabsModel.UpdateTabHeaders();

            // Once level editor is loaded and ready to scroll, scroll to center
            SubRoutines.WaitUntil(() => levelEditor.levelScrollViewer != null && levelEditor.levelScrollViewer.IsLoaded, () => {
                levelEditor.levelScrollViewer.ScrollToOffset(editor_scrollPosition);
            });
        }
    }
}