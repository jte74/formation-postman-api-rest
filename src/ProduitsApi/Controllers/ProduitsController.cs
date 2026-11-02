using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProduitsApi.Models;
using ProduitsApi.Securite;
using ProduitsApi.Services;

namespace ProduitsApi.Controllers;

/// <summary>
/// La ressource "produits" : le cœur de l'API REST.
///
///   Méthode  URL                    Action                          Protection        Succès
///   -------  ---------------------  ------------------------------  ----------------  -----------------
///   GET      /api/produits          lister (filtres en query)       public            200 OK
///   GET      /api/produits/{id}     lire un produit                 public            200 OK
///   POST     /api/produits          créer                           token             201 Created
///   PUT      /api/produits/{id}     remplacer entièrement           token             200 OK
///   PATCH    /api/produits/{id}     modifier partiellement          token             200 OK
///   DELETE   /api/produits/{id}     supprimer                       token + Admin     204 No Content
///
/// [ApiController] active des comportements utiles :
///   - le body JSON est lu automatiquement dans le paramètre (DonneesProduit...) ;
///   - si les règles de validation ([Required], [Range]...) ne sont pas respectées,
///     l'API répond automatiquement 400 Bad Request avec la liste des erreurs, sans exécuter la méthode.
/// </summary>
[ApiController]
[Route("api/produits")]
public class ProduitsController(ProduitStore store) : ControllerBase
{
    // ------------------------------------------------------------------ GET (liste)

    /// <summary>
    /// Liste des produits, avec filtres, tri et pagination facultatifs (Query Parameters).
    /// Exemple : GET /api/produits?categorie=Audio&amp;enStock=true&amp;tri=prix&amp;ordre=desc
    /// Le header de réponse "X-Total-Count" indique le nombre total de produits trouvés (avant pagination).
    /// </summary>
    [HttpGet]
    [ProducesResponseType<List<Produit>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<List<Produit>> Lister([FromQuery] FiltreProduits filtre)
    {
        // Piège classique : "prixMax=12,90". Dans une URL, le séparateur décimal est le point (12.90) ;
        // la virgule serait lue comme un séparateur de milliers (1290). On préfère renvoyer une erreur claire.
        foreach (var cle in new[] { "prixMin", "prixMax" })
        {
            if (Request.Query[cle].ToString().Contains(','))
            {
                ModelState.AddModelError(cle, $"{cle} : utilisez un point comme séparateur décimal (exemple : 12.90), pas une virgule.");
            }
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        IEnumerable<Produit> produits = store.ObtenirTous();

        if (!string.IsNullOrWhiteSpace(filtre.Categorie))
            produits = produits.Where(p => string.Equals(p.Categorie, filtre.Categorie.Trim(), StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(filtre.Recherche))
            produits = produits.Where(p => ProduitStore.ContientSansAccent(p.Nom, filtre.Recherche.Trim())
                                        || ProduitStore.ContientSansAccent(p.Description, filtre.Recherche.Trim()));

        if (filtre.PrixMin is not null)
            produits = produits.Where(p => p.Prix >= filtre.PrixMin);

        if (filtre.PrixMax is not null)
            produits = produits.Where(p => p.Prix <= filtre.PrixMax);

        if (filtre.EnStock is not null)
            produits = produits.Where(p => (p.Stock > 0) == filtre.EnStock);

        var decroissant = string.Equals(filtre.Ordre, "desc", StringComparison.OrdinalIgnoreCase);
        produits = (filtre.Tri?.ToLowerInvariant()) switch
        {
            "nom" => decroissant ? produits.OrderByDescending(p => p.Nom) : produits.OrderBy(p => p.Nom),
            "prix" => decroissant ? produits.OrderByDescending(p => p.Prix) : produits.OrderBy(p => p.Prix),
            "stock" => decroissant ? produits.OrderByDescending(p => p.Stock) : produits.OrderBy(p => p.Stock),
            _ => decroissant ? produits.OrderByDescending(p => p.Id) : produits.OrderBy(p => p.Id)
        };

        var resultat = produits.ToList();

        // Header de réponse personnalisé : visible dans l'onglet "Headers" de la réponse dans Postman.
        Response.Headers["X-Total-Count"] = resultat.Count.ToString();

        return resultat
            .Skip((filtre.Page - 1) * filtre.Taille)
            .Take(filtre.Taille)
            .ToList();
    }

    // ------------------------------------------------------------------ GET (un seul)

    /// <summary>
    /// Un produit par son identifiant (paramètre de route).
    /// Exemple : GET /api/produits/3
    /// La contrainte ":int" fait que /api/produits/abc ne correspond à aucune route (404).
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<Produit>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<Produit> ObtenirParId(int id)
    {
        var produit = store.ObtenirParId(id);
        if (produit is null)
        {
            return ProduitIntrouvable(id);
        }

        return produit;
    }

    // ------------------------------------------------------------------ POST

    /// <summary>
    /// Créer un produit. Nécessite un token.
    /// Réponse 201 Created + header "Location" contenant l'URL du nouveau produit.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType<Produit>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status415UnsupportedMediaType)]
    public ActionResult<Produit> Creer(DonneesProduit donnees)
    {
        var doublon = store.TrouverParNom(donnees.Nom!);
        if (doublon is not null)
        {
            return NomDejaUtilise(donnees.Nom!, doublon.Id);
        }

        var produit = store.Ajouter(donnees);

        // CreatedAtAction => statut 201 + header Location: http://localhost:5080/api/produits/{id}
        return CreatedAtAction(nameof(ObtenirParId), new { id = produit.Id }, produit);
    }

    // ------------------------------------------------------------------ PUT

    /// <summary>
    /// Remplacer ENTIÈREMENT un produit. Nécessite un token.
    /// Le body doit contenir la représentation complète : un champ facultatif absent (description) est effacé.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType<Produit>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public ActionResult<Produit> Remplacer(int id, DonneesProduit donnees)
    {
        if (store.ObtenirParId(id) is null)
        {
            return ProduitIntrouvable(id);
        }

        var doublon = store.TrouverParNom(donnees.Nom!, idAExclure: id);
        if (doublon is not null)
        {
            return NomDejaUtilise(donnees.Nom!, doublon.Id);
        }

        return store.Remplacer(id, donnees)!;
    }

    // ------------------------------------------------------------------ PATCH

    /// <summary>
    /// Modifier PARTIELLEMENT un produit. Nécessite un token.
    /// Seuls les champs présents dans le body sont modifiés. Exemple : { "prix": 17.50 }
    /// </summary>
    [HttpPatch("{id:int}")]
    [Authorize]
    [ProducesResponseType<Produit>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public ActionResult<Produit> ModifierPartiellement(int id, ModificationPartielleProduit modification)
    {
        if (store.ObtenirParId(id) is null)
        {
            return ProduitIntrouvable(id);
        }

        if (modification.Nom is not null)
        {
            var doublon = store.TrouverParNom(modification.Nom, idAExclure: id);
            if (doublon is not null)
            {
                return NomDejaUtilise(modification.Nom, doublon.Id);
            }
        }

        return store.ModifierPartiellement(id, modification)!;
    }

    // ------------------------------------------------------------------ DELETE

    /// <summary>
    /// Supprimer un produit. Nécessite un token ET le rôle Admin.
    /// Réponse 204 No Content : succès, sans body.
    /// Avec le compte "alice" (rôle Utilisateur) => 403 Forbidden.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = UtilisateursDemo.RoleAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public IActionResult Supprimer(int id)
    {
        if (!store.Supprimer(id))
        {
            return ProduitIntrouvable(id);
        }

        return NoContent();
    }

    // ------------------------------------------------------------------ Réponses d'erreur communes

    private ObjectResult ProduitIntrouvable(int id) => Problem(
        statusCode: StatusCodes.Status404NotFound,
        title: "Produit introuvable",
        detail: $"Aucun produit ne porte l'identifiant {id}. Utilisez GET /api/produits pour voir les identifiants existants.");

    private ObjectResult NomDejaUtilise(string nom, int idExistant) => Problem(
        statusCode: StatusCodes.Status409Conflict,
        title: "Conflit : nom déjà utilisé",
        detail: $"Un produit nommé '{nom.Trim()}' existe déjà (identifiant {idExistant}). Choisissez un autre nom.");
}
