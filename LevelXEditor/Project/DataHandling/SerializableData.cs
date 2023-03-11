using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

[Serializable]
public class SerializableData
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

    public string Serialize()
    {
        // Serialise the object to XML
        var serializer = new XmlSerializer(typeof(SerializableData));
        using var stream = new StringWriter();
        serializer.Serialize(stream, this);
        return stream.ToString();
    }

    public static SerializableData Deserialize(string str)
    {
        // Deserialise the XML to an object
        var serializer = new XmlSerializer(typeof(SerializableData));
        using var stream = new StringReader(str);
        return (SerializableData)serializer.Deserialize(stream);
    }
}