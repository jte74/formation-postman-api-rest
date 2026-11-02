namespace ProduitsApi.Securite;

/// <summary>
/// Paramètres des jetons JWT, lus dans la section "Jwt" de appsettings.json.
/// </summary>
public class ParametresJwt
{
    /// <summary>Qui a émis le jeton (claim "iss").</summary>
    public string Emetteur { get; set; } = "ProduitsApi";

    /// <summary>À qui le jeton est destiné (claim "aud").</summary>
    public string Audience { get; set; } = "FormationPostman";

    /// <summary>
    /// Clé secrète servant à SIGNER les jetons (algorithme HS256, au moins 32 caractères).
    /// Seul le serveur la connaît : c'est elle qui empêche de fabriquer ou de modifier un jeton.
    /// En production, elle ne doit jamais être écrite dans le code ni dans Git.
    /// </summary>
    public string CleSecrete { get; set; } = string.Empty;

    /// <summary>Durée de validité d'un jeton, en minutes.</summary>
    public int DureeMinutes { get; set; } = 60;
}
