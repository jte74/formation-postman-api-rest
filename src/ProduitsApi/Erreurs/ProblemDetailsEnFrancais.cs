using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;

namespace ProduitsApi.Erreurs;

/// <summary>
/// Traduit en français les messages d'erreur génériques produits par ASP.NET Core
/// (route inconnue, méthode non autorisée, mauvais Content-Type...) et ajoute un conseil.
///
/// Toutes les erreurs de l'API utilisent le même format JSON standard "ProblemDetails" (RFC 9457) :
/// {
///   "type":   lien vers la définition du code HTTP,
///   "title":  résumé de l'erreur,
///   "status": code HTTP,
///   "detail": explication,
///   "instance": chemin appelé,
///   "traceId": identifiant pour retrouver la requête dans les logs
/// }
/// </summary>
public static class ProblemDetailsEnFrancais
{
    private static readonly Dictionary<int, string> Titres = new()
    {
        [400] = "Requête invalide",
        [401] = "Non authentifié",
        [403] = "Accès refusé",
        [404] = "Ressource introuvable",
        [405] = "Méthode HTTP non autorisée",
        [406] = "Format de réponse non disponible",
        [409] = "Conflit",
        [415] = "Type de contenu non supporté",
        [500] = "Erreur interne du serveur"
    };

    public static void Personnaliser(ProblemDetailsContext contexte)
    {
        var probleme = contexte.ProblemDetails;
        var statut = probleme.Status ?? contexte.HttpContext.Response.StatusCode;
        var requete = contexte.HttpContext.Request;

        // Titre anglais par défaut ("Not Found", "Method Not Allowed"...) => titre français.
        if ((probleme.Title is null || probleme.Title == ReasonPhrases.GetReasonPhrase(statut))
            && Titres.TryGetValue(statut, out var titre))
        {
            probleme.Title = titre;
        }

        // Pas d'explication => on en ajoute une, orientée "que dois-je vérifier dans Postman ?".
        probleme.Detail ??= statut switch
        {
            404 => $"Aucune ressource ne correspond à l'URL '{requete.Path}'. " +
                   "Vérifie l'orthographe de l'URL (ex. : /api/produits) et l'identifiant demandé.",
            405 => $"La méthode {requete.Method} n'est pas acceptée sur l'URL '{requete.Path}'. " +
                   "Vérifie la méthode choisie dans Postman et l'URL (l'identifiant est-il oublié ?). " +
                   "Le header de réponse 'Allow' liste les méthodes acceptées.",
            415 => $"Le format du body ('{requete.ContentType ?? "aucun Content-Type"}') n'est pas supporté. " +
                   "Dans Postman : onglet Body > raw > JSON, afin d'envoyer le header 'Content-Type: application/json'.",
            _ => null
        };

        probleme.Instance ??= requete.Path;

        // Identifiant de trace (déjà ajouté par ASP.NET Core dans la plupart des cas).
        if (!probleme.Extensions.ContainsKey("traceId"))
        {
            probleme.Extensions["traceId"] =
                contexte.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity.Id
                ?? contexte.HttpContext.TraceIdentifier;
        }
    }
}
