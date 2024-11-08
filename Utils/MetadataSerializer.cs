using CharacterGrade.Models.XMLSerialized;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CharacterGrade.Utils
{
    public class MetadataSerializer
    {
        private static readonly object XmlLock = new object();

        public Metadatas ReadMetadataFields(string metadataFieldsPath)
        {
            Metadatas deserializedXML;
            XmlSerializer serializer = new(typeof(Metadatas));
            //using (StringReader reader = new(metadataFieldsPath))
            //{
            //    test = (Metadatas)serializer.Deserialize(reader);
            //}

            using (FileStream stream = File.OpenRead(metadataFieldsPath))
            {
                deserializedXML = (Metadatas)serializer.Deserialize(stream);
                // Now you can access the deserialized data in the 'deserializedXML' object
            }
            return deserializedXML;
        }

        // Serialize the model to XML
        //SerializeToXml(metadata, filePath);

        // Serialize the object to XML and save it to a file
        public void SerializeToXml<T>(T obj, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (TextWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, obj);
            }
        }

        public void UpdateDropdownXMLElement(string filePath, string metadataKey, string dropdownValue)
        {
            // Load the existing XML document
            lock (XmlLock)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);


                // Create a new element (e.g., <NewItem>)
                XmlElement newItem = doc.CreateElement("value");
                newItem.InnerText = dropdownValue;

                // Find the parent element where you want to append the new item (e.g., <Items>)
                XmlNode parentElement = doc.SelectSingleNode("/metadatas/metadata[key='" + metadataKey + "']/values");

                // Append the new item to the parent element
                parentElement?.AppendChild(newItem);

                // Save the modified XML back to the file
                doc.Save(filePath);
            }
        }

        public void RemoveXMLElement(string filePath)
        {
            // Load the existing XML document
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            // Find the specific elements(e.g., < Item > elements) based on some condition
            foreach (XmlElement item in doc.SelectNodes("/metadatas/values"))
            {
                // Example: Remove the element if the 'key' attribute matches a specific value
                if (item.GetAttribute("key") == "SomeKey")
                {
                    item.ParentNode?.RemoveChild(item);
                }
            }

            // Save the modified XML back to the file
            doc.Save(filePath);
        }
    }
}
