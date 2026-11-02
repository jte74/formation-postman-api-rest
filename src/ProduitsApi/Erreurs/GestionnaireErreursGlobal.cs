using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ProduitsApi.Erreurs;

/// <summary>
/// Filet de sécurité : intercepte toute exception non prévue et renvoie une réponse 500 propre,
/// au format JSON "ProblemDetails", au lieu d'une page d'erreur ou d'une connexion coupée.
///
/// Le code 500 signifie "le problème vient du SERVEUR" : la requête du client peut être parfaitement correcte.
/// Le détail technique est écrit dans les logs (la console de l'API), pas envoyé au client,
/// sauf en environnement de développement pour faciliter l'apprentissage.
/// </summary>
public sealed class GestionnaireErreursGlobal(
    IProblemDetailsService problemDetailsService,
    IHostEnvironment environnement,
    ILogger<GestionnaireErreursGlobal> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Erreur non gérée pendant {Methode} {Chemin}",
            httpContext.Request.Method, httpContext.Request.Path);

        var probleme = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Erreur interne du serveur",
            Detail = "Une erreur inattendue s'est produite côté serveur. " +
                     "La requête n'est pas forcément en cause : consultez la console de l'API pour le détail.",
            Instance = httpContext.Request.Path
        };

        if (environnement.IsDevelopment())
        {
            // Visible uniquement en Development : ne jamais exposer ce détail en production.
            probleme.Extensions["exception"] = $"{exception.GetType().Name} : {exception.Message}";
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = probleme,
            Exception = exception
        });
    }
}
