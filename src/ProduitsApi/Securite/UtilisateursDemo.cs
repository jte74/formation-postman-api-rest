namespace ProduitsApi.Securite;

/// <summary>Un compte de démonstration.</summary>
public record Utilisateur(string NomUtilisateur, string MotDePasse, string NomComplet, string Role);

/// <summary>
/// Comptes de démonstration, écrits en dur pour la formation.
/// Dans une vraie application, les utilisateurs sont en base de données et les mots de passe
/// sont stockés sous forme de hash (jamais en clair).
/// </summary>
public static class UtilisateursDemo
{
    public const string RoleAdmin = "Admin";
    public const string RoleUtilisateur = "Utilisateur";

    public static readonly IReadOnlyList<Utilisateur> Tous =
    [
        // Peut tout faire, y compris supprimer des produits.
        new("admin", "Admin123!", "Administratrice de la formation", RoleAdmin),

        // Peut lire, créer et modifier, mais PAS supprimer (=> 403 Forbidden sur DELETE).
        new("alice", "Alice123!", "Alice Martin", RoleUtilisateur)
    ];

    /// <summary>Renvoie l'utilisateur si le couple identifiant / mot de passe est correct, sinon null.</summary>
    public static Utilisateur? Verifier(string nomUtilisateur, string motDePasse) =>
        Tous.FirstOrDefault(u =>
            string.Equals(u.NomUtilisateur, nomUtilisateur.Trim(), StringComparison.OrdinalIgnoreCase) &&
            u.MotDePasse == motDePasse); // le mot de passe, lui, est sensible à la casse
}
