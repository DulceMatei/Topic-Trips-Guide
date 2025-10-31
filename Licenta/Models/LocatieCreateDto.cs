public class LocatieCreateDto
{
    public string Denumire { get; set; }
    public string TipLocatie { get; set; }
    public string Descriere { get; set; }
    public string Geolocatie { get; set; }
    public int TimpEstimativ { get; set; }

    public int? TaraId { get; set; }
    public int? OrasId { get; set; }

    public string? TaraNoua { get; set; }
    public string? OrasNou { get; set; }

    public string? Strada { get; set; }
    public string? NumarStrada { get; set; }

    public string? ImagineUrl { get; set; }

    public int TraseuId { get; set; }
}
