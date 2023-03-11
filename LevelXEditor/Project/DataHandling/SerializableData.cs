using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

[Serializable]
public class SerializableData<T>
{
    // Define a generic method for serialization
    public string Serialize()
    {
        // Serialize the object to XML using XmlSerializer
        var serializer = new XmlSerializer(typeof(T));
        using var stream = new StringWriter();
        serializer.Serialize(stream, this);
        return stream.ToString();
    }

    // Define a generic method for deserialization
    public static T DeserializeFrom(string xml)
    {
        // Deserialize the XML to an object using XmlSerializer
        var serializer = new XmlSerializer(typeof(T));
        using var stream = new StringReader(xml);
        return (T)serializer.Deserialize(stream);
    }
}