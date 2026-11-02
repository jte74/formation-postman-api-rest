namespace ProduitsApi.Models;

/// <summary>
/// Un produit tel qu'il est stocké par l'API et renvoyé au client (Postman).
/// En JSON, les noms de propriétés sont écrits en camelCase : id, nom, description, prix...
/// </summary>
public class Produit
{
    /// <summary>Identifiant unique, attribué par le serveur (jamais par le client).</summary>
    public int Id { get; set; }

    /// <summary>Nom du produit (unique dans le catalogue).</summary>
    public string Nom { get; set; } = string.Empty;

    /// <summary>Description facultative.</summary>
    public string? Description { get; set; }

    /// <summary>Prix en euros, par exemple 24.99 (le séparateur décimal en JSON est toujours le point).</summary>
    public decimal Prix { get; set; }

    /// <summary>Catégorie : Informatique, Audio, Bureau...</summary>
    public string Categorie { get; set; } = string.Empty;

    /// <summary>Quantité disponible. 0 signifie "en rupture de stock".</summary>
    public int Stock { get; set; }

    /// <summary>Date de création du produit.</summary>
    public DateTimeOffset DateCreation { get; set; }

    /// <summary>Date de la dernière modification (null si le produit n'a jamais été modifié).</summary>
    public DateTimeOffset? DateModification { get; set; }
}
