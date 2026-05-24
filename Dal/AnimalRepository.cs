using Npgsql;
using RefugeAnimaux.Metier;

namespace RefugeAnimaux.Dal;

public class AnimalRepository
{
    public void Ajouter(Animal animal)
    {
        using var cnx = Database.GetConnection();
        const string sql = @"
            INSERT INTO animal(identifiant, nom, type, sexe, particularites, date_deces, description,
                               date_sterilisation, sterilise, date_naissance)
            VALUES (@id, @nom, @type, @sexe, @part, @deces, @desc, @sterDate, @sterilise, @naissance);";
        using var cmd = new NpgsqlCommand(sql, cnx);
        cmd.Parameters.AddWithValue("id", animal.Identifiant);
        cmd.Parameters.AddWithValue("nom", animal.Nom);
        cmd.Parameters.AddWithValue("type", animal.Type);
        cmd.Parameters.AddWithValue("sexe", animal.Sexe);
        cmd.Parameters.AddWithValue("part", (object?)animal.Particularites ?? DBNull.Value);
        cmd.Parameters.AddWithValue("deces", (object?)animal.DateDeces ?? DBNull.Value);
        cmd.Parameters.AddWithValue("desc", (object?)animal.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("sterDate", (object?)animal.DateSterilisation ?? DBNull.Value);
        cmd.Parameters.AddWithValue("sterilise", animal.Sterilise);
        cmd.Parameters.AddWithValue("naissance", animal.DateNaissance);
        cmd.ExecuteNonQuery();
    }

    public Animal? Consulter(string identifiant)
    {
        using var cnx = Database.GetConnection();
        const string sql = "SELECT * FROM animal WHERE identifiant = @id";
        using var cmd = new NpgsqlCommand(sql, cnx);
        cmd.Parameters.AddWithValue("id", identifiant);
        using var r = cmd.ExecuteReader();
        return r.Read() ? LireAnimal(r) : null;
    }

    public List<Animal> ListerTous()
    {
        using var cnx = Database.GetConnection();
        const string sql = "SELECT * FROM animal ORDER BY nom";
        using var cmd = new NpgsqlCommand(sql, cnx);
        using var r = cmd.ExecuteReader();
        var animaux = new List<Animal>();
        while (r.Read()) animaux.Add(LireAnimal(r));
        return animaux;
    }

    public void Supprimer(string identifiant)
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("DELETE FROM animal WHERE identifiant = @id", cnx);
        cmd.Parameters.AddWithValue("id", identifiant);
        cmd.ExecuteNonQuery();
    }

    public void AjouterEntree(string animalId, string raison, DateOnly dateEntree, int? contactId)
    {
        using var cnx = Database.GetConnection();
        const string sql = @"INSERT INTO ani_entree(raison, date_entree, ani_identifiant, entree_contact)
                             VALUES (@raison, @date, @animal, @contact)";
        using var cmd = new NpgsqlCommand(sql, cnx);
        cmd.Parameters.AddWithValue("raison", raison);
        cmd.Parameters.AddWithValue("date", dateEntree);
        cmd.Parameters.AddWithValue("animal", animalId);
        cmd.Parameters.AddWithValue("contact", (object?)contactId ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public void AjouterVaccin(string animalId, string nomVaccin, DateOnly dateVaccin, bool fait)
    {
        using var cnx = Database.GetConnection();
        using var tx = cnx.BeginTransaction();
        var vaccinCmd = new NpgsqlCommand(@"
            INSERT INTO vaccin(nom) VALUES (@nom)
            ON CONFLICT (nom) DO UPDATE SET nom = EXCLUDED.nom
            RETURNING id_vaccin;", cnx, tx);
        vaccinCmd.Parameters.AddWithValue("nom", nomVaccin);
        int idVaccin = Convert.ToInt32(vaccinCmd.ExecuteScalar());

        var vaccCmd = new NpgsqlCommand(@"
            INSERT INTO vaccination(vaccination_date, vac_animal, id_vaccin, fait)
            VALUES (@date, @animal, @vaccin, @fait);", cnx, tx);
        vaccCmd.Parameters.AddWithValue("date", dateVaccin);
        vaccCmd.Parameters.AddWithValue("animal", animalId);
        vaccCmd.Parameters.AddWithValue("vaccin", idVaccin);
        vaccCmd.Parameters.AddWithValue("fait", fait);
        vaccCmd.ExecuteNonQuery();
        tx.Commit();
    }
    public void SupprimerAnimal(string id)
    {
        using var cnx = Database.GetConnection();

        string sql = "DELETE FROM animal WHERE identifiant=@id";

        using var cmd = new NpgsqlCommand(sql, cnx);

        cmd.Parameters.AddWithValue("id", id);

        cmd.ExecuteNonQuery();
    }
    public void AjouterFamilleAccueil(string animalId, int contactId, DateOnly dateArrivee)
    {
        using var cnx = Database.GetConnection();

        string sql = @"
            INSERT INTO famille_accueil (fa_ani_identifiant, fa_contact, date_debut)
            VALUES (@animalId, @contactId, @dateArrivee)";

        using var cmd = new NpgsqlCommand(sql, cnx);

        cmd.Parameters.AddWithValue("animalId", animalId);
        cmd.Parameters.AddWithValue("contactId", contactId);
        cmd.Parameters.AddWithValue("dateArrivee", dateArrivee);

        cmd.ExecuteNonQuery();
    }
    public void AjouterAdoption(string animalId, int contactId, DateOnly dateDemande)
    {
        using var cnx = Database.GetConnection();

        string sql = @"
            INSERT INTO adoption (statut, date_demande, ani_identifiant, adop_contact)
            VALUES ('demande', @dateDemande, @animalId, @contactId)";
        using var cmd = new NpgsqlCommand(sql, cnx);

        cmd.Parameters.AddWithValue("dateDemande", dateDemande);
        cmd.Parameters.AddWithValue("animalId", animalId);
        cmd.Parameters.AddWithValue("contactId", contactId);

        cmd.ExecuteNonQuery();
    }
    public void ModifierStatutAdoption(int idAdoption, string statut)
    {
        using var cnx = Database.GetConnection();

        string sql = @"
            UPDATE adoption
            SET statut = @statut
            WHERE id_adoption = @idAdoption";

        using var cmd = new NpgsqlCommand(sql, cnx);

        cmd.Parameters.AddWithValue("statut", statut);
        cmd.Parameters.AddWithValue("idAdoption", idAdoption);

        cmd.ExecuteNonQuery();
    }
    public void AjouterCompatibilite(string animalId, int idCompatibilite, string valeur)
    {
        using var cnx = Database.GetConnection();

        string sql = @"
            INSERT INTO ani_compatibilite
            (ani_identifiant, id_compatibilite, valeur)
            VALUES
            (@animalId, @idCompatibilite, @valeur)";

        using var cmd = new NpgsqlCommand(sql, cnx);

        cmd.Parameters.AddWithValue("animalId", animalId);
        cmd.Parameters.AddWithValue("idCompatibilite", idCompatibilite);
        cmd.Parameters.AddWithValue("valeur", valeur);

        cmd.ExecuteNonQuery();
    }

public void AjouterSortie(string animalId, int contactId, string raison, DateOnly dateSortie)
{
    using var cnx = Database.GetConnection();

    string sql = @"
        INSERT INTO ani_sortie
        (raison, date_sortie, ani_identifiant, sortie_contact)
        VALUES
        (@raison, @dateSortie, @animalId, @contactId)";

    using var cmd = new NpgsqlCommand(sql, cnx);

    cmd.Parameters.AddWithValue("raison", raison);
    cmd.Parameters.AddWithValue("dateSortie", dateSortie);
    cmd.Parameters.AddWithValue("animalId", animalId);
    cmd.Parameters.AddWithValue("contactId", contactId);

    cmd.ExecuteNonQuery();
}
public void ListerFamillesAccueilAnimal(string animalId)
{
    using var cnx = Database.GetConnection();

    string sql = @"
        SELECT fa_contact, date_debut, date_fin
        FROM famille_accueil
        WHERE fa_ani_identifiant = @animalId";

    using var cmd = new NpgsqlCommand(sql, cnx);
    cmd.Parameters.AddWithValue("animalId", animalId);

    using var reader = cmd.ExecuteReader();

    while (reader.Read())
    {
        int contact = reader.GetInt32(0);
        DateOnly dateDebut = reader.GetFieldValue<DateOnly>(1);
        string dateFin = reader.IsDBNull(2) ? "En cours" : reader.GetFieldValue<DateOnly>(2).ToString();

        Console.WriteLine($"Contact : {contact} | Début : {dateDebut} | Fin : {dateFin}");
    }
}
    private static Animal LireAnimal(NpgsqlDataReader r) => new()
    {
        Identifiant = r.GetString(r.GetOrdinal("identifiant")),
        Nom = r.GetString(r.GetOrdinal("nom")),
        Type = r.GetString(r.GetOrdinal("type")),
        Sexe = r.GetString(r.GetOrdinal("sexe")),
        Particularites = r["particularites"] as string,
        Description = r["description"] as string,
        DateDeces = r["date_deces"] == DBNull.Value ? null : DateOnly.FromDateTime((DateTime)r["date_deces"]),
        DateSterilisation = r["date_sterilisation"] == DBNull.Value ? null : DateOnly.FromDateTime((DateTime)r["date_sterilisation"]),
        Sterilise = r.GetBoolean(r.GetOrdinal("sterilise")),
        DateNaissance = DateOnly.FromDateTime((DateTime)r["date_naissance"])
    };
}
