using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace UserAppData
{
    public class AppDataHandler
    {
        // A property that holds an object of type SerializableData
        private SerializableData data = new();
        public SerializableData Data { get { LoadData(); return data; } }

        // A property that holds the application name
        public string AppName { get; set; }

        // A property that holds the file name
        public string FileName { get; set; } = "Data.xml";

        // A constructor that sets AppName automatically based on the namespace
        public AppDataHandler()
        {
            // Get the current namespace using reflection from the assembly
            AppName = Application.Current.MainWindow.GetType().Assembly.GetName().Name ?? "DefaultAppName";
        }

        public void ModifyData(Action<SerializableData> serializableDataAction)
        {
            // Load the data
            LoadData();

            // Modify the data
            serializableDataAction(data);

            // Save the data
            SaveData();
        }

        public string GetDirectoryPath()
        {
            // Get the file path for UserAppData in Local folder
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                AppName
            );
        }

        public string GetFullFilePath()
        {
            // Get the file path for UserAppData in Local folder
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                AppName,
                FileName
            );
        }

        // A method that serializes Data and saves it to a file
        public bool SaveData()
        {
            // The same code as before with AppName instead of "YourAppName"
            try
            {
                // Get the file path for UserAppData in Local folder
                string filePath = GetFullFilePath();

                // If one of the directories doesn't exist, create it
                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }

                // Write to the file with FileStreamWriter
                using (var stream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    // Serialize Data as XML
                    string content = data.Serialize();

                    // Rewrite entire file with new content
                    stream.SetLength(0);
                    stream.Write(Encoding.UTF8.GetBytes(content));
                }

                // Return true if successful
                return true;
            }
            catch (IOException ex)
            {
                // Handle IOException such as file in use or disk full
                MessageBox.Show("An IO error occurred while saving data: " + ex.Message);
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle UnauthorizedAccessException such as access denied or read-only file
                MessageBox.Show("An access error occurred while saving data: " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                // Handle any other exception that may occur
                MessageBox.Show("An unexpected error occurred while saving data: " + ex.Message);
                return false;
            }
        }

        // A method that loads Data from a file and deserializes it
        public bool LoadData()
        {
            // The same code as before with AppName instead of "YourAppName"
            try
            {
                // Get the file path for UserAppData in Local folder
                string filePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    AppName,
                    FileName
                );

                // If the file doesn't exist, just set the Data to a new SerializableData object
                if (!File.Exists(filePath))
                {
                    data = new SerializableData();
                    return true;
                }

                // Create a stream to read from the file
                using (FileStream stream = File.OpenRead(filePath))
                {
                    // Get all text content from stream
                    string content = new StreamReader(stream).ReadToEnd();

                    // Get the json data
                    data = SerializableData.Deserialize(content);
                }

                // Return true if successful 
                return true; 
            } 
            catch (IOException ex) 
            { 
                // Handle IOException such as file in use or disk error 
                MessageBox.Show("An IO error occurred while loading data: " + ex.Message); 
                return false; 
            } 
            catch (UnauthorizedAccessException ex)  
            {  
                // Handle UnauthorizedAccessException such as access denied or read-only file  
                MessageBox.Show("An access error occurred while loading data: " + ex.Message);  
                return false;  
            }  
            catch (Exception ex)   
            {   
                // Handle any other exception that may occur   
                MessageBox.Show("An unexpected error occurred while loading data: " + ex.Message);   
                return false;   
            }  
        }
    }
}