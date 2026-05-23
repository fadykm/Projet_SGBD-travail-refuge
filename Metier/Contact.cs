namespace RefugeAnimaux.Metier;

public class Contact
{
    public int Identifiant { get; set; }
    public string Nom { get; set; } = "";
    public string Prenom { get; set; } = "";
    public string? RegistreNational { get; set; }
    public string? Rue { get; set; }
    public string? Cp { get; set; }
    public string? Localite { get; set; }
    public string? Gsm { get; set; }
    public string? Telephone { get; set; }
    public string? Email { get; set; }

    public override string ToString() => $"{Identifiant} | {Nom} {Prenom} | GSM: {Gsm} | Tél: {Telephone} | Email: {Email}";
}
