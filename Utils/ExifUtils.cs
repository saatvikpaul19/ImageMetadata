using CharacterGrade.Models;
using CharacterGrade.Models.Enums;
using CharacterGrade.Models.XMLSerialized;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.Mime.MediaTypeNames;

namespace CharacterGrade.Utils
{
    internal class ExifUtils
    {
        private string exeResourcePath;
        private string configResourcePath;
        private ProcessStartInfo startInfo;

        private static readonly Logger logger = LogManager.GetLogger("MetadataLogger");

        internal ExifUtils() {
            var assembly = Assembly.GetExecutingAssembly();
            var exePath = assembly.Location;
            var resourcesPath = Path.Combine(Path.GetDirectoryName(exePath), "Resources");
            exeResourcePath = Path.Combine(resourcesPath, "exiftool.exe");
            configResourcePath = Path.Combine(resourcesPath, "exif_config", ".exif_config_namespace");
            
        }

        public string WriteMetadata(string imagePath, List<MetadataModel> metadatas)
        {
            StringBuilder sb = new();
            foreach (var item in metadatas)
            {
                string metadataValue = string.Empty;
                switch (item.Type)
                {
                    case MetadataType.dropdown:
                        metadataValue = item.SelectedValue;
                        break;
                    case MetadataType.date:
                        string date_format = "M/d/yyyy h:mm:ss tt";
                        if (DateTime.TryParseExact(item.Value, date_format, null, System.Globalization.DateTimeStyles.None, out DateTime date_time))
                        {
                            DateTime date_only = date_time.Date;
                            metadataValue = date_only.ToString("yyyy-MM-dd");// ToShortDateString();
                        }
                        break;
                    case MetadataType.integer:
                    case MetadataType.number:
                    case MetadataType.text:
                    default:
                        metadataValue = item.Value;
                        break;
                }
                sb.Append("-" + item.Key + "=" + "\"" + metadataValue + "\" ");
            }

            logger.Info("Metadatas writing into: " + imagePath);
            logger.Info("Metadatas: " + sb);

            var command = $"-config {configResourcePath} -charset filename=UTF8 -P -overwrite_original {sb.ToString()} \"{imagePath}\"";

            startInfo = new ProcessStartInfo
            {
                FileName = exeResourcePath,
                Arguments = command,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                //StandardInputEncoding = Encoding.Unicode,
                //StandardOutputEncoding = Encoding.UTF8
            };
            //Process.Start(exeResourcePath, command);

            Process process = Process.Start(startInfo);
            string output = process.StandardOutput.ReadToEnd(); // assign output to a variable
            process.Close();
            logger.Info("Exiftool complete: " + output);

            return output;
        }

        public string ReadMetadata(string imagePath)
        {
            var command = $"-G1 -a -s -charset filename=UTF8 -XMP:all \"{imagePath}\"";

            startInfo = new ProcessStartInfo
            {
                FileName = exeResourcePath,
                Arguments = command,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                //StandardInputEncoding = Encoding.Unicode,
                //StandardOutputEncoding = Encoding.UTF8
            };
            //Process.Start(exeResourcePath, command);

            Process process = Process.Start(startInfo);
            string output = process.StandardOutput.ReadToEnd(); // assign output to a variable
            process.Close();

            return output;
        }

        public Dictionary<string, string> ExtractKeyValuePairs(string text, string toReplace)
        {
            var result = new Dictionary<string, string>();

            string[] lines = text.Split('\n');
            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    // Remove any prefixes (e.g., "[XMP-ns_1]")
                    key = key.Replace(toReplace, "").Trim();
                    result[key] = value;
                }
            }

            return result;
        }
    }
}
