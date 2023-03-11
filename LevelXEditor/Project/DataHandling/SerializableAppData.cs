using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

[Serializable]
public class SerializableAppData : SerializableData<SerializableAppData>
{
    public string[] recentFiles { get; set; } = new string[0];

    public void AddRecentFile(string path)
    {        
        // If the file is already in the list then remove it
        if (recentFiles.Contains(path))
        {
            recentFiles = recentFiles.Where(x => x != path).ToArray();
        }

        // Prepend the file to the list
        recentFiles = new string[] { path }.Concat(recentFiles).ToArray();

        // If the list is longer than 10 then remove the last item/s
        if (recentFiles.Length > 10)
        {
            recentFiles = recentFiles.Take(10).ToArray();
        }
    }
}