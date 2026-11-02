using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProduitsApi.Models;
using ProduitsApi.Securite;

namespace ProduitsApi.Controllers;

/// <summary>
/// Authentification : obtenir un token et vérifier qui l'on est.
///
///   POST /api/auth/token   (public)    => échange identifiant + mot de passe contre un token
///   GET  /api/auth/moi     (protégé)   => renvoie les informations contenues dans le token
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController(ServiceToken serviceToken) : ControllerBase
{
    /// <summary>
    /// Obtenir un token.
    /// Body : { "nomUtilisateur": "admin", "motDePasse": "Admin123!" }
    /// 200 : token créé ; 400 : champ manquant ; 401 : identifiants incorrects.
    /// </summary>
    [HttpPost("token")]
    [AllowAnonymous]
    [ProducesResponseType<ReponseToken>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public ActionResult<ReponseToken> ObtenirToken(DemandeToken demande)
    {
        var utilisateur = UtilisateursDemo.Verifier(demande.NomUtilisateur!, demande.MotDePasse!);
        if (utilisateur is null)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Identifiants incorrects",
                detail: "Le nom d'utilisateur ou le mot de passe est incorrect (le mot de passe est sensible aux majuscules).");
        }

        return Ok(serviceToken.CreerToken(utilisateur));
    }

    /// <summary>
    /// "Qui suis-je ?" : lit les informations (claims) contenues dans le token envoyé.
    /// 200 : token valide ; 401 : pas de token, ou token invalide/expiré.
    /// </summary>
    [HttpGet("moi")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public IActionResult QuiSuisJe()
    {
        var expirationUnix = long.Parse(User.FindFirst("exp")!.Value);

        return Ok(new
        {
            nomUtilisateur = User.FindFirst("sub")?.Value,
            nomComplet = User.Identity?.Name,
            role = User.FindFirst("role")?.Value,
            expiration = DateTimeOffset.FromUnixTimeSeconds(expirationUnix).ToLocalTime(),
            claims = User.Claims.Select(c => new { type = c.Type, valeur = c.Value })
        });
    }
}
