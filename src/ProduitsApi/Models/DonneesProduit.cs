using System.ComponentModel.DataAnnotations;

namespace ProduitsApi.Models;

/// <summary>
/// Corps (body) JSON attendu pour CRÉER (POST) ou REMPLACER ENTIÈREMENT (PUT) un produit.
/// Tous les champs obligatoires doivent être présents : c'est la représentation complète du produit.
///
/// Exemple :
/// {
///   "nom": "Tapis de souris XXL",
///   "description": "Tapis 90 x 40 cm, base antidérapante",
///   "prix": 19.90,
///   "categorie": "Informatique",
///   "stock": 25
/// }
///
/// Remarque : l'identifiant ne fait PAS partie du body. Pour un POST il est attribué par le serveur,
/// pour un PUT il est indiqué dans l'URL (/api/produits/{id}). Un champ "id" envoyé dans le body est ignoré.
/// </summary>
public class DonneesProduit
{
    [Required(ErrorMessage = "Le champ 'nom' est obligatoire.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 100 caractères.")]
    public string? Nom { get; set; }

    [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
    public string? Description { get; set; }

    // Le type est "decimal?" (nullable) pour que l'absence du champ soit détectée par [Required].
    // Avec un simple "decimal", un prix oublié vaudrait 0 sans message clair.
    [Required(ErrorMessage = "Le champ 'prix' est obligatoire.")]
    [Range(0.01, 100000.0, ErrorMessage = "Le prix doit être compris entre 0.01 et 100000.")]
    public decimal? Prix { get; set; }

    [Required(ErrorMessage = "Le champ 'categorie' est obligatoire.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "La catégorie doit contenir entre 2 et 50 caractères.")]
    public string? Categorie { get; set; }

    [Required(ErrorMessage = "Le champ 'stock' est obligatoire.")]
    [Range(0, 10000, ErrorMessage = "Le stock doit être compris entre 0 et 10000.")]
    public int? Stock { get; set; }
}
