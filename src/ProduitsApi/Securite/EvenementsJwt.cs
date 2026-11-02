using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ProduitsApi.Securite;

/// <summary>
/// Personnalise les réponses 401 et 403 renvoyées par l'authentification JWT.
///
/// Par défaut, ASP.NET Core renvoie un 401 ou un 403 avec un body VIDE, ce qui est déroutant
/// pour un débutant. Ici, on renvoie un message JSON explicite (format "ProblemDetails")
/// qui explique la cause probable de l'erreur.
///
/// Rappel :
///   401 Unauthorized = "Je ne sais pas qui tu es"      (pas de token, token invalide ou expiré)
///   403 Forbidden    = "Je sais qui tu es, mais c'est interdit pour toi" (rôle insuffisant)
/// </summary>
public static class EvenementsJwt
{
    public static JwtBearerEvents Creer() => new()
    {
        // Appelé quand un endpoint protégé par [Authorize] est appelé sans authentification valide.
        OnChallenge = async contexte =>
        {
            // On prend la main sur la réponse (sinon ASP.NET Core écrirait sa réponse par défaut).
            contexte.HandleResponse();

            var http = contexte.HttpContext;
            var header = http.Request.Headers.Authorization.ToString();
            string detail;

            if (string.IsNullOrWhiteSpace(header))
            {
                detail = "Aucun token fourni. Cet endpoint est protégé : ajoute le header " +
                         "'Authorization: Bearer <token>' (dans Postman : onglet Authorization > Bearer Token). " +
                         "Obtiens d'abord un token avec POST /api/auth/token.";
                http.Response.Headers.WWWAuthenticate = "Bearer";
            }
            else if (header.Trim().Equals("Bearer", StringComparison.OrdinalIgnoreCase))
            {
                detail = "Le header Authorization contient 'Bearer' mais pas de token. " +
                         "La variable {{token}} est probablement vide : exécute d'abord 'Obtenir un token'.";
                http.Response.Headers.WWWAuthenticate = "Bearer";
            }
            else if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                detail = "Le header Authorization doit commencer par le mot 'Bearer' suivi d'un espace puis du token " +
                         "(exemple : 'Authorization: Bearer eyJhbGciOi...').";
                http.Response.Headers.WWWAuthenticate = "Bearer error=\"invalid_request\"";
            }
            else if (header.Contains("{{"))
            {
                detail = "Le token reçu contient '{{' : la variable Postman n'a pas été remplacée. " +
                         "Vérifie que l'environnement 'Formation API REST - Local' est sélectionné (en haut à droite) " +
                         "et que tu as bien exécuté la requête 'Obtenir un token'.";
                http.Response.Headers.WWWAuthenticate = "Bearer error=\"invalid_token\"";
            }
            else if (contexte.AuthenticateFailure is SecurityTokenExpiredException expire)
            {
                detail = $"Le token a expiré le {expire.Expires.ToLocalTime():dd/MM/yyyy à HH:mm:ss}. " +
                         "Demande un nouveau token avec POST /api/auth/token.";
                http.Response.Headers.WWWAuthenticate = "Bearer error=\"invalid_token\", error_description=\"The token expired\"";
            }
            else
            {
                detail = "Le token est invalide : il est incomplet, mal copié, modifié, ou signé avec une autre clé. " +
                         "Demande un nouveau token avec POST /api/auth/token.";
                http.Response.Headers.WWWAuthenticate = "Bearer error=\"invalid_token\"";
            }

            await EcrireProblemeAsync(http, StatusCodes.Status401Unauthorized, "Non authentifié", detail);
        },

        // Appelé quand l'utilisateur est authentifié mais n'a pas le rôle demandé (ex. : [Authorize(Roles = "Admin")]).
        OnForbidden = async contexte =>
        {
            var role = contexte.HttpContext.User.FindFirst("role")?.Value ?? "inconnu";
            var detail = $"Tu es bien authentifié(e) (rôle : {role}), mais ce rôle ne permet pas cette action. " +
                         $"Par exemple, seul le rôle '{UtilisateursDemo.RoleAdmin}' peut supprimer un produit.";

            await EcrireProblemeAsync(contexte.HttpContext, StatusCodes.Status403Forbidden, "Accès refusé", detail);
        }
    };

    private static async Task EcrireProblemeAsync(HttpContext http, int statut, string titre, string detail)
    {
        http.Response.StatusCode = statut;
        var service = http.RequestServices.GetRequiredService<IProblemDetailsService>();
        await service.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = http,
            ProblemDetails = new ProblemDetails
            {
                Status = statut,
                Title = titre,
                Detail = detail,
                Instance = http.Request.Path
            }
        });
    }
}
