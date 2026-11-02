using System.ComponentModel.DataAnnotations;

namespace ProduitsApi.Models;

/// <summary>
/// Corps (body) JSON attendu pour MODIFIER PARTIELLEMENT (PATCH) un produit.
/// On n'envoie QUE les champs à changer ; les champs absents restent inchangés.
///
/// Exemple : changer uniquement le prix
/// { "prix": 17.50 }
///
/// Exemple : mettre le produit en rupture de stock
/// { "stock": 0 }
///
/// Simplification pédagogique : un champ envoyé avec la valeur null est considéré comme "non envoyé".
/// (Il existe un format standard plus riche, JSON Patch - RFC 6902, présenté dans le support de cours.)
/// </summary>
public class ModificationPartielleProduit : IValidatableObject
{
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 100 caractères.")]
    public string? Nom { get; set; }

    [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
    public string? Description { get; set; }

    [Range(0.01, 100000.0, ErrorMessage = "Le prix doit être compris entre 0.01 et 100000.")]
    public decimal? Prix { get; set; }

    [StringLength(50, MinimumLength = 2, ErrorMessage = "La catégorie doit contenir entre 2 et 50 caractères.")]
    public string? Categorie { get; set; }

    [Range(0, 10000, ErrorMessage = "Le stock doit être compris entre 0 et 10000.")]
    public int? Stock { get; set; }

    /// <summary>Un PATCH sans aucun champ à modifier n'a pas de sens : on renvoie une erreur 400.</summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Nom is null && Description is null && Prix is null && Categorie is null && Stock is null)
        {
            yield return new ValidationResult(
                "Le body doit contenir au moins un champ à modifier parmi : nom, description, prix, categorie, stock " +
                "(attention aux fautes de frappe dans les noms de champs).");
        }
    }
}
