using Microsoft.AspNetCore.Mvc;

namespace ProduitsApi.Controllers;

/// <summary>
/// Endpoints de découverte, publics :
///   GET /          => page d'accueil JSON listant les endpoints
///   GET /api/ping  => la toute première requête de la formation : "l'API répond-elle ?"
/// </summary>
[ApiController]
public class AccueilController : ControllerBase
{
    [HttpGet("/")]
    public IActionResult Accueil() => Ok(new
    {
        api = "API Produits - formation Postman et API REST",
        version = "1.0",
        message = "L'API fonctionne. Utilise Postman pour envoyer tes requêtes.",
        documentationOpenApi = "/openapi/v1.json",
        comptesDeDemonstration = new[]
        {
            new { nomUtilisateur = "admin", motDePasse = "Admin123!", role = "Admin" },
            new { nomUtilisateur = "alice", motDePasse = "Alice123!", role = "Utilisateur" }
        },
        endpoints = new[]
        {
            "GET    /api/ping",
            "POST   /api/auth/token",
            "GET    /api/auth/moi                (token)",
            "GET    /api/produits                (filtres : categorie, recherche, prixMin, prixMax, enStock, tri, ordre, page, taille)",
            "GET    /api/produits/{id}",
            "POST   /api/produits                (token)",
            "PUT    /api/produits/{id}           (token)",
            "PATCH  /api/produits/{id}           (token)",
            "DELETE /api/produits/{id}           (token + rôle Admin)",
            "GET    /api/demo/headers",
            "GET    /api/demo/erreur",
            "POST   /api/demo/reinitialiser"
        }
    });

    [HttpGet("/api/ping")]
    public IActionResult Ping() => Ok(new
    {
        message = "pong",
        serveur = "API Produits (ASP.NET Core .NET 10)",
        heureServeur = DateTimeOffset.Now
    });
}
