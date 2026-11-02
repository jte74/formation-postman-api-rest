# 8. Comprendre le code de l'API (annexe pour le formateur)

Cette annexe fait le tour du code C#, fichier par fichier. Elle sert à répondre aux questions de l'apprenant (« mais comment l'API sait-elle que... ? ») et à montrer, si le temps le permet, **ce qui se passe côté serveur** quand Postman envoie une requête.

Le projet est volontairement simple : **ASP.NET Core .NET 10**, **contrôleurs** (le style le plus explicite pour débuter), **stockage en mémoire**, **2 paquets NuGet officiels**.

---

## 8.1 `Program.cs` — le point d'entrée

Il fait deux choses :

**1. Enregistrer les services** (`builder.Services...`) :

| Ligne (résumé) | Rôle |
|---|---|
| `AddSingleton<ProduitStore>()` | Une seule « base de données » en mémoire, partagée par toutes les requêtes |
| `AddControllers(...)` | Active les contrôleurs ; messages de lecture des paramètres en français ; noms de champs JSON dans les erreurs |
| `.AddJsonOptions(...)` | JSON indenté, accents et apostrophes non échappés |
| `.ConfigureApiBehaviorOptions(...)` | Personnalise la réponse **400** automatique (titre et explication en français) |
| `AddProblemDetails(...)` | Toutes les erreurs au format standard **ProblemDetails**, traduites (`Erreurs/ProblemDetailsEnFrancais.cs`) |
| `AddExceptionHandler<GestionnaireErreursGlobal>()` | Toute exception non prévue → **500** propre (`Erreurs/GestionnaireErreursGlobal.cs`) |
| `AddOpenApi()` | Publie la description de l'API sur `/openapi/v1.json` |
| `AddAuthentication(...).AddJwtBearer(...)` | Lit et **vérifie** le header `Authorization: Bearer <token>` : émetteur, audience, expiration, signature |
| `options.Events = EvenementsJwt.Creer()` | Réponses **401 / 403** explicites (`Securite/EvenementsJwt.cs`) |

**2. Construire le pipeline HTTP** (`app.Use...`), traversé par **chaque requête**, dans l'ordre :

```
requête ──► Journal (console) ──► Gestion des exceptions (500) ──► Pages de statut (404/405 en JSON)
        ──► Authentification (qui ?) ──► Autorisation (a le droit ?) ──► Contrôleur ──► réponse
```

> L'ordre compte : l'authentification doit précéder l'autorisation ; le journal est placé en premier pour voir **toutes** les requêtes, y compris celles refusées.

Il n'y a volontairement **pas** de `UseHttpsRedirection()` : on reste en HTTP simple en local. **En production, HTTPS est obligatoire.**

---

## 8.2 Les contrôleurs (`Controllers/`)

Un contrôleur est une classe dont les méthodes publiques (« actions ») répondent à des requêtes. Les **attributs** relient une URL et une méthode HTTP à une action :

```csharp
[ApiController]                    // comportements d'API : body JSON lu automatiquement, 400 automatique
[Route("api/produits")]            // préfixe de toutes les routes du contrôleur
public class ProduitsController(ProduitStore store) : ControllerBase
{
    [HttpGet("{id:int}")]          // GET /api/produits/3   ({id:int} = paramètre de route entier)
    public ActionResult<Produit> ObtenirParId(int id) { ... }

    [HttpPost]                     // POST /api/produits
    [Authorize]                    // token obligatoire, sinon 401
    public ActionResult<Produit> Creer(DonneesProduit donnees) { ... }   // body JSON -> objet C#

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]   // token + rôle Admin, sinon 401 / 403
    public IActionResult Supprimer(int id) { ... }
}
```

| Fichier | Endpoints | À montrer |
|---|---|---|
| `AccueilController.cs` | `GET /`, `GET /api/ping` | Un endpoint minimal qui renvoie un objet → JSON |
| `AuthController.cs` | `POST /api/auth/token`, `GET /api/auth/moi` | `[AllowAnonymous]`, vérification des identifiants, lecture des claims (`User.FindFirst("role")`) |
| `ProduitsController.cs` | CRUD `/api/produits` | Le choix du code : `Ok()` 200, `CreatedAtAction()` 201 + `Location`, `NoContent()` 204, `Problem(statusCode: 404/409)` |
| `DemoController.cs` | `/api/demo/headers`, `/api/demo/erreur`, `/api/demo/reinitialiser` | Endpoints pédagogiques (n'existeraient pas dans une vraie API) |

**Comment le code HTTP est choisi** (extrait de `ProduitsController.Creer`) :

```csharp
var doublon = store.TrouverParNom(donnees.Nom!);
if (doublon is not null)
{
    return NomDejaUtilise(donnees.Nom!, doublon.Id);   // 409 Conflict
}

var produit = store.Ajouter(donnees);
return CreatedAtAction(nameof(ObtenirParId), new { id = produit.Id }, produit);   // 201 + Location
```

---

## 8.3 Les modèles (`Models/`)

| Fichier | Rôle |
|---|---|
| `Produit.cs` | Ce qui est stocké et **renvoyé** : `id`, `nom`, `description`, `prix`, `categorie`, `stock`, `dateCreation`, `dateModification` |
| `DonneesProduit.cs` | Ce qui est **reçu** par POST et PUT, avec les **règles de validation** |
| `ModificationPartielleProduit.cs` | Ce qui est reçu par PATCH : tous les champs facultatifs ; au moins un champ exigé |
| `FiltreProduits.cs` | Les **query parameters** de `GET /api/produits`, avec leurs règles |
| `Authentification.cs` | `DemandeToken` (body de `/api/auth/token`) et `ReponseToken` |

**La validation déclarative** : les attributs décrivent les règles, `[ApiController]` renvoie automatiquement un **400** si elles ne sont pas respectées, **avant même** d'exécuter l'action :

```csharp
[Required(ErrorMessage = "Le champ 'prix' est obligatoire.")]
[Range(0.01, 100000.0, ErrorMessage = "Le prix doit être compris entre 0.01 et 100000.")]
public decimal? Prix { get; set; }
```

Le type est `decimal?` (nullable) pour que l'**absence** du champ soit détectée par `[Required]` : avec un simple `decimal`, un prix oublié vaudrait silencieusement 0.

**Séparer le modèle reçu du modèle stocké** (`DonneesProduit` ≠ `Produit`) est une bonne pratique : le client ne peut pas imposer l'`id` ni les dates.

---

## 8.4 Le stockage (`Services/ProduitStore.cs`)

Une simple `List<Produit>` protégée par un verrou (`lock`), car plusieurs requêtes peuvent arriver en même temps. Elle contient **10 produits de démonstration**, remis à chaque démarrage et par `POST /api/demo/reinitialiser`.

À souligner :
- `Remplacer` (PUT) affecte **tous** les champs : `produit.Description = donnees.Description?.Trim();` → `null` si absent.
- `ModifierPartiellement` (PATCH) n'affecte **que** les champs non nuls : `if (modification.Prix is not null) produit.Prix = ...`.
- Le store renvoie toujours des **copies** : un contrôleur ne peut pas modifier la liste par accident.

> Pour passer à une vraie base de données, on remplacerait cette classe par Entity Framework Core (SQLite, SQL Server, PostgreSQL) sans toucher aux contrôleurs ni à la collection Postman : c'est l'intérêt d'une API qui cache son fonctionnement interne.

---

## 8.5 La sécurité (`Securite/`)

| Fichier | Rôle |
|---|---|
| `UtilisateursDemo.cs` | Les 2 comptes (`admin` / Admin, `alice` / Utilisateur), écrits en dur pour la démonstration |
| `ParametresJwt.cs` | Options lues dans `appsettings.json` (section `Jwt`) : émetteur, audience, clé secrète, durée |
| `ServiceToken.cs` | **Fabrique** le JWT : claims (`sub`, `name`, `role`, `jti`), dates, signature HMAC-SHA256 |
| `EvenementsJwt.cs` | Remplace les 401 / 403 « vides » d'ASP.NET Core par des messages qui expliquent la cause probable (pas de token, « Bearer » oublié, variable Postman non remplacée, token expiré, token invalide, rôle insuffisant) |

**Le cycle complet :**
1. `AuthController.ObtenirToken` vérifie le couple identifiant / mot de passe, puis appelle `ServiceToken.CreerToken`.
2. Le client renvoie le token dans `Authorization: Bearer ...`.
3. Le middleware JwtBearer (configuré dans `Program.cs`) vérifie **signature, émetteur, audience et expiration** avec la même clé secrète.
4. Si le token est valide, `User` (le `ClaimsPrincipal`) est rempli ; `[Authorize(Roles = "Admin")]` compare le claim `role`.

`appsettings.json` :

```json
"Jwt": {
  "Emetteur": "ProduitsApi",
  "Audience": "FormationPostman",
  "CleSecrete": "Cle-secrete-de-DEMONSTRATION-pour-la-formation-Postman-2026 (a ne jamais utiliser en production)",
  "DureeMinutes": 60
}
```

> **Limites assumées de la démonstration** (à mentionner si l'apprenant est curieux) : mots de passe en clair dans le code (en vrai : base de données + hachage), clé secrète dans un fichier versionné (en vrai : coffre-fort de secrets), pas de *refresh token*, pas de révocation. Pour une vraie application, on utilise ASP.NET Core Identity ou un fournisseur d'identité (Microsoft Entra ID, Auth0, Keycloak...).

---

## 8.6 Les erreurs (`Erreurs/`)

| Fichier | Rôle |
|---|---|
| `GestionnaireErreursGlobal.cs` | Attrape toute exception non gérée : écrit le détail dans la console (logs), renvoie un **500** ProblemDetails. En environnement *Development* seulement, le message de l'exception est ajouté au body |
| `ProblemDetailsEnFrancais.cs` | Traduit les titres génériques (« Not Found » → « Ressource introuvable ») et ajoute un conseil pour 400, 404, 405 et 415 |

Toutes les erreurs ont donc le même format :

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Ressource introuvable",
  "status": 404,
  "detail": "Aucune ressource ne correspond à l'URL '/api/produit'. Vérifiez l'orthographe de l'URL...",
  "instance": "/api/produit",
  "traceId": "00-d332f1033dfa8fc864c8dc0ead180755-e94fbaa2f6b6b7a5-00"
}
```

Le `traceId` permet de retrouver la requête dans les logs du serveur : c'est l'information à transmettre à l'équipe de l'API en cas de 500.

---

## 8.7 Idées de démonstrations côté code (extensions)

| Démonstration | Comment | Ce que l'apprenant voit |
|---|---|---|
| **Point d'arrêt** | Point d'arrêt dans `ProduitsController.Creer`, F5, envoi depuis Postman | Le body JSON transformé en objet C# ; Postman qui attend |
| **Modifier une règle** | Dans `DonneesProduit.cs`, remplacer la règle du stock par `[Range(0, 100, ErrorMessage = "Le stock doit être compris entre 0 et 100.")]`, relancer, créer un produit avec `"stock": 500` | Une nouvelle erreur 400 apparaît sans écrire de `if` |
| **Expiration du token** | `"DureeMinutes": 1` dans `appsettings.json`, relancer, attendre une minute | 401 « Le token a expiré le ... » |
| **Rendre un endpoint public** | Retirer `[Authorize]` de `Creer`, relancer | POST sans token → 201 (puis remettre l'attribut !) |
| **Description OpenAPI** | Ouvrir http://localhost:5080/openapi/v1.json, puis Postman > *Import* > coller l'URL | Postman génère une collection à partir de la documentation |
