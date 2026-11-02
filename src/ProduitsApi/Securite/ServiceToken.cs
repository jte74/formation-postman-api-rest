using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ProduitsApi.Models;

namespace ProduitsApi.Securite;

/// <summary>
/// Fabrique les jetons JWT (JSON Web Token).
///
/// Un JWT est composé de 3 parties encodées en Base64Url et séparées par des points :
///   EN-TÊTE . CONTENU (claims) . SIGNATURE
///   eyJhbGciOi...  .  eyJzdWIiOi...  .  SflKxwRJSM...
///
/// - L'en-tête indique l'algorithme de signature (HS256).
/// - Le contenu (payload) porte les "claims" : qui est l'utilisateur, son rôle, la date d'expiration...
///   Il est LISIBLE par tout le monde (encodé, pas chiffré) : on n'y met jamais de secret.
/// - La signature est calculée avec la clé secrète du serveur : si quelqu'un modifie le contenu,
///   la signature ne correspond plus et le serveur refuse le jeton (401).
/// </summary>
public class ServiceToken(IOptions<ParametresJwt> options)
{
    private readonly ParametresJwt _parametres = options.Value;

    public ReponseToken CreerToken(Utilisateur utilisateur)
    {
        var maintenant = DateTimeOffset.UtcNow;
        var expiration = maintenant.AddMinutes(_parametres.DureeMinutes);

        var descripteur = new SecurityTokenDescriptor
        {
            Issuer = _parametres.Emetteur,
            Audience = _parametres.Audience,
            IssuedAt = maintenant.UtcDateTime,
            NotBefore = maintenant.UtcDateTime,
            Expires = expiration.UtcDateTime,
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, utilisateur.NomUtilisateur), // "sub" : identifiant
                new Claim(JwtRegisteredClaimNames.Name, utilisateur.NomComplet),    // "name" : nom affiché
                new Claim("role", utilisateur.Role),                                // "role" : utilisé par [Authorize(Roles = ...)]
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())   // "jti" : identifiant unique du jeton
            ]),
            SigningCredentials = new SigningCredentials(
                CreerCleDeSignature(_parametres.CleSecrete),
                SecurityAlgorithms.HmacSha256)
        };

        var jeton = new JsonWebTokenHandler().CreateToken(descripteur);

        return new ReponseToken(
            AccessToken: jeton,
            TokenType: "Bearer",
            ExpiresIn: _parametres.DureeMinutes * 60,
            Expiration: expiration.ToLocalTime(),
            NomUtilisateur: utilisateur.NomUtilisateur,
            Role: utilisateur.Role);
    }

    /// <summary>Clé utilisée à la fois pour signer (ici) et pour vérifier (dans Program.cs) les jetons.</summary>
    public static SymmetricSecurityKey CreerCleDeSignature(string cleSecrete) =>
        new(Encoding.UTF8.GetBytes(cleSecrete));
}
