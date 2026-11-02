# 6. Fiche récapitulative — API REST et Postman

*Aide-mémoire à imprimer (2 pages) ou à garder ouvert pendant les exercices.*

---

## Les méthodes HTTP

| Méthode | Action | Body | Succès | Idempotente | Exemple |
|---|---|---|---|---|---|
| **GET** | Lire | non | **200** OK | oui (et sûre) | `GET /api/produits/3` |
| **POST** | Créer | oui, complet | **201** Created + `Location` | **non** | `POST /api/produits` |
| **PUT** | Remplacer **tout** | oui, **complet** | **200** OK (ou 204) | oui | `PUT /api/produits/3` |
| **PATCH** | Modifier **une partie** | oui, **partiel** | **200** OK | pas forcément | `PATCH /api/produits/3` |
| **DELETE** | Supprimer | non | **204** No Content | oui | `DELETE /api/produits/3` |

> **PUT** : j'envoie tout, un champ absent est effacé. **PATCH** : j'envoie seulement ce qui change.

## Les codes de réponse

| Famille | Sens | Qui corrige ? |
|---|---|---|
| **2xx** | Succès | — |
| **3xx** | Redirection | — (Postman suit automatiquement) |
| **4xx** | Erreur du **client** | **Moi** : je corrige la requête |
| **5xx** | Erreur du **serveur** | L'équipe de l'API (lui donner le `traceId`) |

| Code | Nom | Cause typique | Réflexe |
|---|---|---|---|
| **200** | OK | GET / PUT / PATCH réussi | Lire le body |
| **201** | Created | POST réussi | Noter l'`id`, voir `Location` |
| **204** | No Content | DELETE réussi | Body vide : normal |
| **400** | Bad Request | Champ manquant, valeur invalide, JSON mal formé | Lire `errors` |
| **401** | Unauthorized | Pas de token, token invalide ou **expiré** | Obtenir un nouveau token |
| **403** | Forbidden | Connecté, mais **pas le droit** (rôle) | Utiliser un compte autorisé |
| **404** | Not Found | URL mal écrite, **id inexistant** | Vérifier URL et id |
| **405** | Method Not Allowed | Mauvaise méthode pour l'URL (id oublié ?) | Lire le header `Allow` |
| **409** | Conflict | Doublon (nom déjà utilisé) | Changer la valeur |
| **415** | Unsupported Media Type | Body en *Text* au lieu de *JSON* | Body > raw > **JSON** |
| **500** | Internal Server Error | Bug côté serveur | Prévenir, ne pas « corriger » la requête |

> **401** = « Qui êtes-vous ? » — **403** = « Je sais qui vous êtes, mais non. »

## Anatomie d'une requête et d'une URL

```
POST  http://localhost:5080/api/produits/3?categorie=Audio&tri=prix
 │     │      │         │    │          │  │                └── & sépare les paramètres
 │     │      │         │    │          │  └── ? ouvre les query parameters (filtrer, trier, paginer)
 │     │      │         │    │          └── paramètre de route (quelle ressource)
 │     │      │         │    └── chemin (path)
 │     │      │         └── port
 │     │      └── hôte (localhost = cette machine)
 │     └── protocole (https en production)
 └── méthode
```

| Requête | Réponse |
|---|---|
| Méthode + URL | Code de statut |
| Headers | Headers |
| Body (facultatif) | Body |

## Les headers essentiels

| Header | Sens | Exemple |
|---|---|---|
| `Content-Type` | Format du body **envoyé** | `application/json` |
| `Accept` | Format de réponse **souhaité** | `application/json` |
| `Authorization` | Preuve d'identité | `Bearer eyJhbGciOi...` |
| `Location` (réponse) | URL de la ressource créée | `http://localhost:5080/api/produits/11` |
| `WWW-Authenticate` (réponse) | Authentification attendue (sur 401) | `Bearer` |
| `Allow` (réponse) | Méthodes acceptées (sur 405) | `GET, POST` |
| `X-Total-Count` (réponse, notre API) | Nombre total de résultats | `10` |

## JSON en 6 règles

```json
{
  "nom": "Tapis de souris XXL",
  "prix": 19.90,
  "stock": 25,
  "enPromotion": false,
  "description": null,
  "couleurs": ["noir", "gris"]
}
```

1. Clés et chaînes entre **guillemets doubles droits** `"..."` (pas `'...'`, ni `“...”`).
2. Nombres **sans guillemets**, avec un **point** : `19.90`.
3. `true`, `false`, `null` en minuscules, sans guillemets.
4. Virgule **entre** les éléments, **jamais après le dernier**.
5. Objets `{ }`, tableaux `[ ]`.
6. Pas de commentaires.

## Bearer Token en 4 étapes

1. `POST /api/auth/token` avec `{ "nomUtilisateur": "...", "motDePasse": "..." }` (sans authentification).
2. Récupérer `accessToken` dans la réponse (valable 60 minutes ici).
3. Le stocker dans une variable : `pm.environment.set("token", pm.response.json().accessToken);`
4. L'envoyer à chaque requête protégée : onglet **Authorization** > **Bearer Token** > `{{token}}`, ce qui produit le header `Authorization: Bearer <token>`.

## Postman : où trouver quoi ?

| Je veux... | Où ? |
|---|---|
| Choisir l'environnement | Liste déroulante **en haut à droite** |
| Voir les valeurs des variables | Icône **œil** à côté de l'environnement, ou survoler `{{variable}}` |
| Modifier la méthode | Liste déroulante à **gauche de l'URL** |
| Ajouter des query / path parameters | Onglet **Params** |
| Configurer le token | Onglet **Authorization** (de la requête, du dossier ou de la collection) |
| Ajouter un header | Onglet **Headers** (cliquer sur « hidden » pour voir les headers automatiques) |
| Envoyer du JSON | Onglet **Body** > **raw** > **JSON** |
| Écrire des tests | Onglet **Scripts** > **Post-response** (anciennement *Tests*) |
| Préparer avant l'envoi | Onglet **Scripts** > **Pre-request** |
| Voir les échanges bruts et les `console.log` | **Console** (en bas à gauche) |
| Exécuter toute la collection | Clic droit sur la collection > **Run collection** |
| Obtenir le code équivalent (cURL, C#...) | Icône **`</>`** (*Code snippet*) à droite |

| Raccourci (Windows / macOS) | Action |
|---|---|
| **Ctrl+Entrée** / Cmd+Entrée | Envoyer la requête |
| **Ctrl+S** / Cmd+S | Enregistrer la requête |
| **Ctrl+T** / Cmd+T | Nouvel onglet |
| **Ctrl+O** / Cmd+O | Importer |
| **Ctrl+Alt+C** / Cmd+Alt+C | Ouvrir la Console |
| **Ctrl + +** / Cmd + + | Agrandir l'affichage |

## Tests Postman : les indispensables

```javascript
pm.test("Statut 201", function () {
    pm.response.to.have.status(201);
});

pm.test("Header Location présent", function () {
    pm.response.to.have.header("Location");
});

const reponse = pm.response.json();

pm.test("Valeurs attendues", function () {
    pm.expect(reponse.nom).to.eql("Tapis de souris XXL");
    pm.expect(reponse.id).to.be.a("number");
    pm.expect(reponse.prix).to.be.below(100);
    pm.expect(reponse).to.have.property("categorie");
});

pm.test("Réponse rapide", function () {
    pm.expect(pm.response.responseTime).to.be.below(2000);
});

pm.environment.set("produitId", reponse.id);   // réutiliser : {{produitId}}
```

Pour un **204** (body vide) : `pm.expect(pm.response.text()).to.be.empty;` — ne pas appeler `pm.response.json()`.

## L'API de la formation

**Adresse :** `http://localhost:5080` — **Comptes :** `admin` / `Admin123!` (Admin), `alice` / `Alice123!` (Utilisateur, ne peut pas supprimer)

| Méthode | URL | Protection |
|---|---|---|
| GET | `/api/ping` | public |
| POST | `/api/auth/token` | public |
| GET | `/api/auth/moi` | token |
| GET | `/api/produits?categorie=&recherche=&prixMin=&prixMax=&enStock=&tri=&ordre=&page=&taille=` | public |
| GET | `/api/produits/{id}` | public |
| POST | `/api/produits` | token |
| PUT | `/api/produits/{id}` | token |
| PATCH | `/api/produits/{id}` | token |
| DELETE | `/api/produits/{id}` | token + **Admin** |
| GET | `/api/demo/headers` | public |
| GET | `/api/demo/erreur` | public (500 volontaire) |
| POST | `/api/demo/reinitialiser` | public |

**Body d'un produit (POST / PUT) :**

```json
{
  "nom": "Support pour ordinateur portable",
  "description": "Aluminium, hauteur réglable",
  "prix": 39.90,
  "categorie": "Bureau",
  "stock": 10
}
```

`nom` (2 à 100 caractères, unique), `prix` (0.01 à 100000), `categorie` (2 à 50 caractères) et `stock` (0 à 10000) sont **obligatoires** ; `description` (500 caractères maximum) est facultative.

## Diagnostic express

| Je vois... | Je vérifie... |
|---|---|
| `ENOTFOUND {{baseUrl}}` | L'environnement est-il sélectionné ? |
| `ECONNREFUSED` | L'API est-elle lancée ? Le port est-il le bon ? |
| 401 | Ai-je un token ? Est-il expiré ? « Bearer » est-il présent ? |
| 403 | Mon compte a-t-il le bon rôle ? |
| 404 | L'URL est-elle bien écrite ? L'id existe-t-il ? |
| 405 | La méthode est-elle la bonne ? L'id est-il dans l'URL ? |
| 400 | Que dit `errors` ? Mon JSON est-il valide ? |
| 415 | Body en *raw > JSON* ? |
| 200 mais rien n'a changé | La méthode est-elle restée sur GET ? |
| 200 avec `[]` | Mes query parameters sont-ils bien écrits (`?` puis `&`, noms exacts) ? |
