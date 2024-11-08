using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterGrade.Models
{
    public class FileNameDetails
    {
        public FileNameDetails(bool fileDetailsExtracted = false)
        {
            FileDetailsExtracted = fileDetailsExtracted;
        }

        public FileNameDetails(string imageYear, string inscriptionId, string place, string lineNumber, string posNumber, string character, bool fileDetailsExtracted = false)
        {
            FileDetailsExtracted = fileDetailsExtracted;
            ImageYear = imageYear;
            InscriptionId = inscriptionId;
            Place = place;
            LineNumber = lineNumber;
            PosNumber = posNumber;
            Character = character;
        }
        public bool FileDetailsExtracted { get; set; }

        public string ImageYear { get; set; }
        public string InscriptionId { get; set; }
        public string Place { get; set; }
        public string LineNumber { get; set; }
        public string PosNumber { get; set; }
        public string Character { get; set; }
    }

    public class ImagePixels
    {
        public ImagePixels(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; set; }
        public int Height { get; set; }
    }
}
