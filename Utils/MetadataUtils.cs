using CharacterGrade.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CharacterGrade.Utils
{
    internal class MetadataUtils
    {
        public static string getMetadataXMLPath(MetadataCategory category)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var exePath = assembly.Location;
            var resourcesPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(exePath), "Resources");
            string xmlPath = System.IO.Path.Combine(resourcesPath, "XML", getXMLPath(category));
            return xmlPath;
        }

        public static string getXMLPath(MetadataCategory category)
        {
            switch (category)
            {
                case MetadataCategory.Inscription:
                    return "inscription-metadata.xml";
                case MetadataCategory.Character:
                    return "character-metadata.xml";
                case MetadataCategory.Image:
                    return "image-metadata.xml";
                default:
                    return "";
            }
        }

        public static string getReplacementText(MetadataCategory category)
        {
            switch (category)
            {
                case MetadataCategory.Inscription:
                    return "[XMP-Inscription]";
                case MetadataCategory.Character:
                    return "[XMP-Character]";
                case MetadataCategory.Image:
                    return "[XMP-Image]";
                default:
                    return "";
            }
        }
    }
}
