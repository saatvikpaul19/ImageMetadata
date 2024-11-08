using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CharacterGrade.Models.XMLSerialized
{
    // using System.Xml.Serialization;
    // XmlSerializer serializer = new XmlSerializer(typeof(Metadatas));
    // using (StringReader reader = new StringReader(xml))
    // {
    //    var test = (Metadatas)serializer.Deserialize(reader);
    // }

    [XmlRoot(ElementName = "metadata")]
    public class Metadata
    {

        [XmlElement(ElementName = "key")]
        public string Key { get; set; }

        [XmlElement(ElementName = "descriptor")]
        public string Descriptor { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "required")]
        public string Required { get; set; }

        [XmlElement(ElementName = "values")]
        public Values Values { get; set; }
    }

    [XmlRoot(ElementName = "values")]
    public class Values
    {
        [XmlAttribute(AttributeName = "allowNewValues")]
        public string AllowNewValues { get; set; }

        [XmlElement(ElementName = "value")]
        public List<string> Value { get; set; }
    }

    [XmlRoot(ElementName = "metadatas")]
    public class Metadatas
    {

        [XmlElement(ElementName = "metadata")]
        public List<Metadata> Metadata { get; set; }
    }
}
