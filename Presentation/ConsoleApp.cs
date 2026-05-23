using RefugeAnimaux.Dal;
using RefugeAnimaux.Metier;

namespace RefugeAnimaux.Presentation;

public class ConsoleApp
{
    private readonly AnimalRepository _animalRepo = new();
    private readonly ContactRepository _contactRepo = new();

    public void Demarrer()
    {
        while (true)
        {
            Console.WriteLine("\n=== Refuge Animaux ===");
            Console.WriteLine("1. Ajouter un animal");
            Console.WriteLine("2. Consulter un animal");
            Console.WriteLine("3. Lister les animaux");
            Console.WriteLine("4. Supprimer un animal");
            Console.WriteLine("5. Ajouter une entrée à un animal");
            Console.WriteLine("6. Ajouter un vaccin à un animal");
            Console.WriteLine("7. Ajouter une personne de contact");
            Console.WriteLine("8. Lister les contacts");
            Console.WriteLine("9. Supprimer un contact");
            Console.WriteLine("0. Quitter");
            Console.Write("Choix : ");
            var choix = Console.ReadLine();

            try
            {
                switch (choix)
                {
                    case "1": AjouterAnimal(); break;
                    case "2": ConsulterAnimal(); break;
                    case "3": ListerAnimaux(); break;
                    case "4": SupprimerAnimal(); break;
                    case "5": AjouterEntree(); break;
                    case "6": AjouterVaccin(); break;
                    case "7": AjouterContact(); break;
                    case "8": ListerContacts(); break;
                    case "9": SupprimerContact(); break;
                    case "0": return;
                    default: Console.WriteLine("Choix invalide."); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
            }
        }
    }

    private void AjouterAnimal()
    {
        var animal = new Animal
        {
            Identifiant = LireTexte("Identifiant yymmdd99999 : "),
            Nom = LireTexte("Nom : "),
            Type = LireTexte("Type (chat/chien) : ").ToLower(),
            Sexe = LireTexte("Sexe (M/F) : ").ToUpper(),
            DateNaissance = LireDate("Date de naissance (yyyy-mm-dd) : "),
            Sterilise = LireBool("Stérilisé ? (o/n) : "),
            Description = LireTexteOptionnel("Description : "),
            Particularites = LireTexteOptionnel("Particularités : ")
        };
        if (animal.Sterilise) animal.DateSterilisation = LireDate("Date de stérilisation (yyyy-mm-dd) : ");
        _animalRepo.Ajouter(animal);
        Console.WriteLine("Animal ajouté.");
    }

    private void ConsulterAnimal()
    {
        var id = LireTexte("Identifiant : ");
        var animal = _animalRepo.Consulter(id);
        Console.WriteLine(animal is null ? "Animal introuvable." : animal);
    }

    private void ListerAnimaux()
    {
        foreach (var animal in _animalRepo.ListerTous()) Console.WriteLine(animal);
    }

    private void SupprimerAnimal()
    {
        _animalRepo.Supprimer(LireTexte("Identifiant : "));
        Console.WriteLine("Suppression effectuée si l'animal existait.");
    }

    private void AjouterEntree()
    {
        var animalId = LireTexte("Identifiant animal : ");
        var raison = LireTexte("Raison (abandon/errant/deces_proprietaire/saisie/retour_adoption/retour_famille_accueil) : ");
        var date = LireDate("Date d'entrée (yyyy-mm-dd) : ");
        var contact = LireIntOptionnel("Id contact (laisser vide si aucun) : ");
        _animalRepo.AjouterEntree(animalId, raison, date, contact);
        Console.WriteLine("Entrée ajoutée.");
    }

    private void AjouterVaccin()
    {
        var animalId = LireTexte("Identifiant animal : ");
        var vaccin = LireTexte("Nom du vaccin : ");
        var date = LireDate("Date du vaccin (yyyy-mm-dd) : ");
        var fait = LireBool("Fait ? (o/n) : ");
        _animalRepo.AjouterVaccin(animalId, vaccin, date, fait);
        Console.WriteLine("Vaccin ajouté.");
    }

    private void AjouterContact()
    {
        var contact = new Contact
        {
            Nom = LireTexte("Nom : "),
            Prenom = LireTexte("Prénom : "),
            RegistreNational = LireTexteOptionnel("Registre national (yy.mm.dd-999.99) : "),
            Rue = LireTexteOptionnel("Rue : "),
            Cp = LireTexteOptionnel("Code postal : "),
            Localite = LireTexteOptionnel("Localité : "),
            Gsm = LireTexteOptionnel("GSM : "),
            Telephone = LireTexteOptionnel("Téléphone : "),
            Email = LireTexteOptionnel("Email : ")
        };
        var id = _contactRepo.Ajouter(contact);
        Console.WriteLine($"Contact ajouté avec l'id {id}.");
    }

    private void ListerContacts()
    {
        foreach (var contact in _contactRepo.ListerTous()) Console.WriteLine(contact);
    }

    private void SupprimerContact()
    {
        _contactRepo.Supprimer(LireInt("Id contact : "));
        Console.WriteLine("Suppression effectuée si le contact existait.");
    }

    private static string LireTexte(string message)
    {
        Console.Write(message);
        return Console.ReadLine()?.Trim() ?? "";
    }

    private static string? LireTexteOptionnel(string message)
    {
        var valeur = LireTexte(message);
        return string.IsNullOrWhiteSpace(valeur) ? null : valeur;
    }

    private static DateOnly LireDate(string message)
    {
        while (true)
        {
            if (DateOnly.TryParse(LireTexte(message), out var date)) return date;
            Console.WriteLine("Date invalide.");
        }
    }

    private static bool LireBool(string message)
    {
        var valeur = LireTexte(message).ToLower();
        return valeur is "o" or "oui" or "y" or "yes";
    }

    private static int LireInt(string message)
    {
        while (true)
        {
            if (int.TryParse(LireTexte(message), out var nombre)) return nombre;
            Console.WriteLine("Nombre invalide.");
        }
    }

    private static int? LireIntOptionnel(string message)
    {
        var texte = LireTexte(message);
        return int.TryParse(texte, out var nombre) ? nombre : null;
    }
}
