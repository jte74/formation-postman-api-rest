using System.Globalization;
using ProduitsApi.Models;

namespace ProduitsApi.Services;

/// <summary>
/// "Base de données" en mémoire : une simple liste de produits.
///
/// - Enregistrée en Singleton (une seule instance pour toute l'application) dans Program.cs.
/// - Les données sont perdues à chaque redémarrage de l'API : c'est voulu pour la formation,
///   on repart toujours d'un état connu (10 produits de démonstration).
/// - Un verrou (lock) protège la liste car plusieurs requêtes peuvent arriver en même temps.
/// </summary>
public class ProduitStore
{
    private readonly Lock _verrou = new();
    private readonly List<Produit> _produits = [];
    private int _dernierId;

    public ProduitStore()
    {
        Reinitialiser();
    }

    /// <summary>Remet les 10 produits de démonstration (identifiants 1 à 10).</summary>
    public int Reinitialiser()
    {
        lock (_verrou)
        {
            _produits.Clear();
            _produits.AddRange(DonneesDeDemonstration());
            _dernierId = _produits.Max(p => p.Id);
            return _produits.Count;
        }
    }

    /// <summary>Renvoie une copie de la liste (pour ne jamais exposer la liste interne).</summary>
    public List<Produit> ObtenirTous()
    {
        lock (_verrou)
        {
            return _produits.Select(Copier).ToList();
        }
    }

    public Produit? ObtenirParId(int id)
    {
        lock (_verrou)
        {
            var produit = _produits.FirstOrDefault(p => p.Id == id);
            return produit is null ? null : Copier(produit);
        }
    }

    /// <summary>Cherche un produit portant déjà ce nom (sans tenir compte des majuscules), en excluant éventuellement un identifiant.</summary>
    public Produit? TrouverParNom(string nom, int? idAExclure = null)
    {
        lock (_verrou)
        {
            var produit = _produits.FirstOrDefault(p =>
                p.Id != idAExclure && string.Equals(p.Nom.Trim(), nom.Trim(), StringComparison.OrdinalIgnoreCase));
            return produit is null ? null : Copier(produit);
        }
    }

    public Produit Ajouter(DonneesProduit donnees)
    {
        lock (_verrou)
        {
            var produit = new Produit
            {
                Id = ++_dernierId,
                Nom = donnees.Nom!.Trim(),
                Description = donnees.Description?.Trim(),
                Prix = donnees.Prix!.Value,
                Categorie = donnees.Categorie!.Trim(),
                Stock = donnees.Stock!.Value,
                DateCreation = DateTimeOffset.Now
            };
            _produits.Add(produit);
            return Copier(produit);
        }
    }

    /// <summary>PUT : TOUS les champs sont remplacés (un champ facultatif absent devient null).</summary>
    public Produit? Remplacer(int id, DonneesProduit donnees)
    {
        lock (_verrou)
        {
            var produit = _produits.FirstOrDefault(p => p.Id == id);
            if (produit is null)
            {
                return null;
            }

            produit.Nom = donnees.Nom!.Trim();
            produit.Description = donnees.Description?.Trim(); // absent du body => null
            produit.Prix = donnees.Prix!.Value;
            produit.Categorie = donnees.Categorie!.Trim();
            produit.Stock = donnees.Stock!.Value;
            produit.DateModification = DateTimeOffset.Now;
            return Copier(produit);
        }
    }

    /// <summary>PATCH : seuls les champs envoyés (non null) sont modifiés.</summary>
    public Produit? ModifierPartiellement(int id, ModificationPartielleProduit modification)
    {
        lock (_verrou)
        {
            var produit = _produits.FirstOrDefault(p => p.Id == id);
            if (produit is null)
            {
                return null;
            }

            if (modification.Nom is not null) produit.Nom = modification.Nom.Trim();
            if (modification.Description is not null) produit.Description = modification.Description.Trim();
            if (modification.Prix is not null) produit.Prix = modification.Prix.Value;
            if (modification.Categorie is not null) produit.Categorie = modification.Categorie.Trim();
            if (modification.Stock is not null) produit.Stock = modification.Stock.Value;
            produit.DateModification = DateTimeOffset.Now;
            return Copier(produit);
        }
    }

    public bool Supprimer(int id)
    {
        lock (_verrou)
        {
            return _produits.RemoveAll(p => p.Id == id) > 0;
        }
    }

    /// <summary>Comparaison "souple" : ignore les majuscules et les accents ("ecran" trouve "Écran").</summary>
    public static bool ContientSansAccent(string? texte, string recherche) =>
        texte is not null &&
        CultureInfo.InvariantCulture.CompareInfo.IndexOf(
            texte, recherche, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0;

    private static Produit Copier(Produit p) => new()
    {
        Id = p.Id,
        Nom = p.Nom,
        Description = p.Description,
        Prix = p.Prix,
        Categorie = p.Categorie,
        Stock = p.Stock,
        DateCreation = p.DateCreation,
        DateModification = p.DateModification
    };

    private static IEnumerable<Produit> DonneesDeDemonstration()
    {
        var date = new DateTimeOffset(2026, 1, 15, 9, 0, 0, TimeSpan.FromHours(1));
        return
        [
            new() { Id = 1,  Nom = "Clavier mécanique",      Categorie = "Informatique", Prix = 89.90m,  Stock = 15,  Description = "Clavier AZERTY rétroéclairé, touches mécaniques", DateCreation = date },
            new() { Id = 2,  Nom = "Souris sans fil",        Categorie = "Informatique", Prix = 24.99m,  Stock = 42,  Description = "Souris ergonomique, autonomie 12 mois", DateCreation = date },
            new() { Id = 3,  Nom = "Écran 27 pouces",        Categorie = "Informatique", Prix = 249.00m, Stock = 8,   Description = "Écran QHD 2560 x 1440, 75 Hz", DateCreation = date },
            new() { Id = 4,  Nom = "Casque audio Bluetooth", Categorie = "Audio",        Prix = 79.50m,  Stock = 0,   Description = "Réduction de bruit active, 30 h d'autonomie", DateCreation = date },
            new() { Id = 5,  Nom = "Enceinte portable",      Categorie = "Audio",        Prix = 49.90m,  Stock = 20,  Description = "Étanche, Bluetooth 5.3", DateCreation = date },
            new() { Id = 6,  Nom = "Lampe de bureau LED",    Categorie = "Bureau",       Prix = 34.90m,  Stock = 12,  Description = "3 intensités, port USB intégré", DateCreation = date },
            new() { Id = 7,  Nom = "Chaise ergonomique",     Categorie = "Bureau",       Prix = 199.00m, Stock = 3,   Description = "Soutien lombaire réglable", DateCreation = date },
            new() { Id = 8,  Nom = "Carnet de notes A5",     Categorie = "Bureau",       Prix = 6.50m,   Stock = 100, Description = "192 pages quadrillées", DateCreation = date },
            new() { Id = 9,  Nom = "Câble USB-C 2 m",        Categorie = "Informatique", Prix = 12.90m,  Stock = 0,   Description = "Charge rapide 100 W", DateCreation = date },
            new() { Id = 10, Nom = "Webcam HD",              Categorie = "Informatique", Prix = 59.90m,  Stock = 7,   Description = "1080p, micro intégré", DateCreation = date }
        ];
    }
}
