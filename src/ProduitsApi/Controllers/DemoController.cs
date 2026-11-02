using Microsoft.AspNetCore.Mvc;
using ProduitsApi.Services;

namespace ProduitsApi.Controllers;

/// <summary>
/// Endpoints "pédagogiques", publics, qui n'existeraient pas dans une vraie API :
///   GET  /api/demo/headers        => renvoie les headers reçus par le serveur (pour comprendre les headers)
///   GET  /api/demo/erreur         => provoque volontairement une erreur 500
///   POST /api/demo/reinitialiser  => remet les 10 produits de démonstration
/// </summary>
[ApiController]
[Route("api/demo")]
public class DemoController(ProduitStore store) : ControllerBase
{
    /// <summary>
    /// Renvoie la méthode, l'URL et tous les headers tels que le serveur les a reçus.
    /// Astuce : ajoute un header personnalisé dans Postman (ex. "X-Formation: bonjour") et observe-le ici.
    /// </summary>
    [HttpGet("headers")]
    public IActionResult Headers()
    {
        var headers = Request.Headers
            .OrderBy(h => h.Key)
            .ToDictionary(
                h => h.Key,
                h => h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase)
                    ? Tronquer(h.Value.ToString())
                    : h.Value.ToString());

        return Ok(new
        {
            methode = Request.Method,
            url = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}",
            nombreDeHeaders = headers.Count,
            headersRecus = headers
        });
    }

    /// <summary>Lève volontairement une exception pour montrer une réponse 500 Internal Server Error.</summary>
    [HttpGet("erreur")]
    public IActionResult Erreur() =>
        throw new InvalidOperationException("Erreur volontaire pour illustrer le code 500 pendant la formation.");

    /// <summary>Remet les données de démonstration (utile avant un nouvel exercice ou un passage du Collection Runner).</summary>
    [HttpPost("reinitialiser")]
    public IActionResult Reinitialiser()
    {
        var nombre = store.Reinitialiser();
        return Ok(new
        {
            message = "Données de démonstration réinitialisées.",
            nombreProduits = nombre
        });
    }

    // Le token complet n'a pas besoin d'être réaffiché : on n'en montre que le début.
    private static string Tronquer(string valeur) =>
        valeur.Length <= 30 ? valeur : valeur[..30] + "... (tronqué)";
}
