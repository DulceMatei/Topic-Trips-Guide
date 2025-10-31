namespace Licenta.Models
{
    public class PdfTraseuDto
    {
        public string Titlu { get; set; }
        public List<LocatiePdfDto> Locatii { get; set; }
        public double DistantaTotala { get; set; }
        public int DurataTotala { get; set; }
        public string ImageBase64 { get; set; }
    }

    public class LocatiePdfDto
    {
        public string Nume { get; set; }
        public string Geolocatie { get; set; }
        public double? Distanta { get; set; }
        public double? Durata { get; set; }
        public string? Strada { get; set; }
        public string? NumarStrada { get; set; }
        public string? Descriere { get; set; }
        public string? ImagineUrl { get; set; }
        public int? TimpEstimativ { get; set; }
        public string? Oras { get; set; }       
        public string? Tara { get; set; }
    }

}