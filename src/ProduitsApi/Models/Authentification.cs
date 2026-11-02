using System.ComponentModel.DataAnnotations;

namespace ProduitsApi.Models;

/// <summary>
/// Body JSON attendu par POST /api/auth/token.
/// Exemple : { "nomUtilisateur": "admin", "motDePasse": "Admin123!" }
/// </summary>
public class DemandeToken
{
    [Required(ErrorMessage = "Le champ 'nomUtilisateur' est obligatoire.")]
    public string? NomUtilisateur { get; set; }

    [Required(ErrorMessage = "Le champ 'motDePasse' est obligatoire.")]
    public string? MotDePasse { get; set; }
}

/// <summary>
/// Réponse de POST /api/auth/token.
/// Les noms accessToken / tokenType / expiresIn suivent les conventions d'OAuth 2.0.
/// </summary>
/// <param name="AccessToken">Le jeton (JWT) à renvoyer dans le header "Authorization: Bearer &lt;token&gt;".</param>
/// <param name="TokenType">Toujours "Bearer".</param>
/// <param name="ExpiresIn">Durée de validité en secondes.</param>
/// <param name="Expiration">Date et heure d'expiration.</param>
/// <param name="NomUtilisateur">Utilisateur authentifié.</param>
/// <param name="Role">Rôle de l'utilisateur (Admin ou Utilisateur).</param>
public record ReponseToken(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    DateTimeOffset Expiration,
    string NomUtilisateur,
    string Role);
