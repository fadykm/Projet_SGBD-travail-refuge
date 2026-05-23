namespace RefugeAnimaux.Metier;

public class Adoption
{
    public int IdAdoption { get; set; }
    public string Statut { get; set; } = "demande";
    public DateOnly DateDemande { get; set; }
    public string AnimalId { get; set; } = "";
    public int ContactId { get; set; }
}
