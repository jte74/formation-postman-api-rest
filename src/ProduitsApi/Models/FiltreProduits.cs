using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ProduitsApi.Models;

/// <summary>
/// Paramètres de requête (Query Parameters) acceptés par GET /api/produits.
/// Ils se placent dans l'URL après le "?" et sont séparés par des "&amp;" :
///
///   /api/produits?categorie=Informatique&amp;prixMax=100&amp;tri=prix&amp;ordre=desc
///
/// Tous les paramètres sont facultatifs. Les noms ne sont pas sensibles à la casse.
/// </summary>
public class FiltreProduits : IValidatableObject
{
    /// <summary>Ne garder que les produits de cette catégorie (ex. : Informatique, Audio, Bureau).</summary>
    [FromQuery(Name = "categorie")]
    public string? Categorie { get; set; }

    /// <summary>Texte recherché dans le nom ou la description (sans tenir compte des majuscules ni des accents).</summary>
    [FromQuery(Name = "recherche")]
    public string? Recherche { get; set; }

    /// <summary>Prix minimum (inclus).</summary>
    [Range(0, 100000.0, ErrorMessage = "prixMin doit être compris entre 0 et 100000.")]
    [FromQuery(Name = "prixMin")]
    public decimal? PrixMin { get; set; }

    /// <summary>Prix maximum (inclus).</summary>
    [Range(0, 100000.0, ErrorMessage = "prixMax doit être compris entre 0 et 100000.")]
    [FromQuery(Name = "prixMax")]
    public decimal? PrixMax { get; set; }

    /// <summary>true : seulement les produits disponibles ; false : seulement les produits en rupture.</summary>
    [FromQuery(Name = "enStock")]
    public bool? EnStock { get; set; }

    /// <summary>Champ de tri : "id" (par défaut), "nom", "prix" ou "stock".</summary>
    [RegularExpression("^(?i)(id|nom|prix|stock)$", ErrorMessage = "tri doit valoir id, nom, prix ou stock.")]
    [FromQuery(Name = "tri")]
    public string? Tri { get; set; }

    /// <summary>Ordre du tri : "asc" (croissant, par défaut) ou "desc" (décroissant).</summary>
    [RegularExpression("^(?i)(asc|desc)$", ErrorMessage = "ordre doit valoir asc ou desc.")]
    [FromQuery(Name = "ordre")]
    public string? Ordre { get; set; }

    /// <summary>Numéro de page (pagination), à partir de 1.</summary>
    [Range(1, 1000, ErrorMessage = "page doit être compris entre 1 et 1000.")]
    [FromQuery(Name = "page")]
    public int Page { get; set; } = 1;

    /// <summary>Nombre de produits par page, de 1 à 50.</summary>
    [Range(1, 50, ErrorMessage = "taille doit être compris entre 1 et 50.")]
    [FromQuery(Name = "taille")]
    public int Taille { get; set; } = 20;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PrixMin is not null && PrixMax is not null && PrixMin > PrixMax)
        {
            yield return new ValidationResult(
                "prixMin ne peut pas être supérieur à prixMax.",
                ["prixMin"]);
        }
    }
}
