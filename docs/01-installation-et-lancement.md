# 1. Installation et lancement de l'API en local

Ce document explique comment installer les outils, ouvrir la solution dans Visual Studio, lancer l'API, la déboguer et configurer Postman. Comptez **20 minutes** la première fois, la veille de la formation.

---

## 1.1 Prérequis

| Outil | Version | Rôle | Où le trouver |
|---|---|---|---|
| **SDK .NET 10** | 10.0.100 ou plus récent | Compiler et exécuter l'API | https://dotnet.microsoft.com/download/dotnet/10.0 (installé automatiquement avec Visual Studio 2026) |
| **Visual Studio 2026** | toute édition (Community gratuite) | Ouvrir la solution `.sln`, lancer et déboguer l'API | https://visualstudio.microsoft.com/ — charge de travail **« Développement web et ASP.NET »** |
| **Postman (application de bureau)** | version récente (v11 ou plus) | Envoyer les requêtes | https://www.postman.com/downloads/ |
| **Compte Postman** | gratuit | Postman demande une connexion au démarrage | à créer sur https://www.postman.com/ |
| Connexion Internet | — | **Uniquement** pour le premier build (téléchargement de 2 paquets NuGet) et la connexion à Postman | — |

> **Pourquoi l'application de bureau Postman et pas la version web ?** La version web de Postman (navigateur) ne peut pas appeler `localhost` sans installer en plus le « Postman Desktop Agent ». L'application de bureau fonctionne directement.

> **Sans Visual Studio ?** L'API se lance aussi avec la ligne de commande `dotnet` (voir 1.4), avec Visual Studio Code (extension *C# Dev Kit*) ou JetBrains Rider.

Vérifier que le SDK est installé (invite de commandes ou PowerShell) :

```bash
dotnet --list-sdks
```

Une ligne commençant par `10.0.` doit apparaître.

---

## 1.2 Arborescence du kit

```
formation-postman-api-rest/
├── FormationPostman.sln                 ← solution à ouvrir dans Visual Studio
├── README.md                            ← présentation du kit et démarrage rapide
├── docs/                                ← supports pédagogiques
│   ├── 01-installation-et-lancement.md      (ce document)
│   ├── 02-programme-de-la-formation.md      programme minuté (1 h 30, variante 1 h)
│   ├── 03-support-de-cours.md               le cours complet (théorie + pratique)
│   ├── 04-schemas.md                        les schémas commentés
│   ├── 05-exercices-et-corrections.md       exercices progressifs + corrections
│   ├── 06-fiche-recapitulative.md           aide-mémoire à imprimer
│   ├── 07-checklist-avant-formation.md      vérifications J-1 et jour J
│   ├── 08-comprendre-le-code-de-l-api.md    visite guidée du code C#
│   └── schemas/                             9 schémas SVG
├── postman/
│   ├── Formation-API-REST.postman_collection.json          ← collection à importer
│   └── Formation-API-REST-Local.postman_environment.json   ← environnement à importer
└── src/
    └── ProduitsApi/                     ← le projet ASP.NET Core .NET 10
        ├── ProduitsApi.csproj               projet (net10.0, 2 paquets NuGet)
        ├── Program.cs                       démarrage : services + pipeline HTTP
        ├── appsettings.json                 configuration (paramètres du JWT)
        ├── appsettings.Development.json
        ├── ProduitsApi.http                 requêtes de test pour Visual Studio (bonus)
        ├── Properties/
        │   └── launchSettings.json          port http://localhost:5080
        ├── Controllers/
        │   ├── AccueilController.cs         GET /  et  GET /api/ping
        │   ├── AuthController.cs            POST /api/auth/token, GET /api/auth/moi
        │   ├── ProduitsController.cs        CRUD /api/produits
        │   └── DemoController.cs            endpoints pédagogiques (headers, erreur 500, réinitialisation)
        ├── Models/
        │   ├── Produit.cs                   le produit stocké et renvoyé
        │   ├── DonneesProduit.cs            body de POST et PUT (+ règles de validation)
        │   ├── ModificationPartielleProduit.cs   body de PATCH
        │   ├── FiltreProduits.cs            query parameters de GET /api/produits
        │   └── Authentification.cs          body et réponse de /api/auth/token
        ├── Services/
        │   └── ProduitStore.cs              « base de données » en mémoire + 10 produits de démo
        ├── Securite/
        │   ├── ParametresJwt.cs             options du token (clé, durée...)
        │   ├── ServiceToken.cs              fabrication des JWT
        │   ├── UtilisateursDemo.cs          comptes admin et alice
        │   └── EvenementsJwt.cs             messages 401 / 403 explicites
        └── Erreurs/
            ├── GestionnaireErreursGlobal.cs exceptions → 500 en JSON
            └── ProblemDetailsEnFrancais.cs  traduction des erreurs génériques
```

Le projet n'utilise **aucune base de données** : les produits sont gardés en mémoire et **les 10 produits de démonstration reviennent à chaque redémarrage** (ou via `POST /api/demo/reinitialiser`).

Dépendances NuGet (officielles Microsoft, rien d'autre) :

| Paquet | Version | Pourquoi |
|---|---|---|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 10.0.14 | Valider le header `Authorization: Bearer <token>` |
| `Microsoft.AspNetCore.OpenApi` | 10.0.14 | Publier la description de l'API sur `/openapi/v1.json` (bonus : importable dans Postman) |

---

## 1.3 Lancer l'API avec Visual Studio (recommandé)

1. Double-cliquez sur **`FormationPostman.sln`** (ou *Fichier > Ouvrir > Projet/Solution*).
2. Attendez la fin de la restauration des paquets NuGet (barre d'état en bas). La première fois, une connexion Internet est nécessaire.
3. Vérifiez que le profil de lancement sélectionné (liste déroulante à côté du bouton ▶ vert) est **`http`**.
4. Appuyez sur **F5** (démarrer avec débogage) ou **Ctrl+F5** (sans débogage, plus rapide).
5. Une fenêtre de console s'ouvre et affiche notamment :

   ```
   info: Microsoft.Hosting.Lifetime[14] Now listening on: http://localhost:5080
   info: ProduitsApi[0] API Produits démarrée sur http://localhost:5080. Comptes : admin / Admin123! (Admin) ; alice / Alice123! (Utilisateur). Arrêt : Ctrl+C.
   ```

6. Le navigateur s'ouvre sur http://localhost:5080/ et affiche un JSON de bienvenue listant les endpoints. **L'API fonctionne.**

Pendant la formation, chaque requête reçue s'affiche dans cette console, par exemple :

```
10:42:17 info: HTTP[0] GET /api/produits?categorie=Audio -> 200 (3 ms)
10:42:31 info: HTTP[0] POST /api/produits -> 401 (1 ms)
10:42:58 info: HTTP[0] POST /api/produits -> 201 (6 ms) [avec token]
```

C'est très parlant pour l'apprenant : **il voit le serveur recevoir les requêtes envoyées par Postman.** Placez la console à côté de Postman.

### Déboguer pendant la formation (effet garanti)

1. Ouvrez `Controllers/ProduitsController.cs`.
2. Cliquez dans la marge grise à gauche de la ligne `var doublon = store.TrouverParNom(donnees.Nom!);` de la méthode `Creer` : un point rouge (point d'arrêt) apparaît.
3. Lancez l'API avec **F5** (avec débogage).
4. Dans Postman, envoyez *03 - Créer un produit (POST) > Créer un produit → 201*.
5. Visual Studio s'arrête sur la ligne : survolez `donnees` pour voir **le body JSON envoyé par Postman, transformé en objet C#**. Postman, lui, attend la réponse (la requête est « en cours »).
6. Appuyez sur **F5** pour continuer : Postman reçoit la réponse 201.

> Si vous restez arrêté plus de quelques dizaines de secondes, Postman peut afficher une erreur de délai dépassé (*timeout*). Il suffit de renvoyer la requête.

---

## 1.4 Lancer l'API en ligne de commande (alternative)

```bash
cd src/ProduitsApi
dotnet run
```

`dotnet run` compile, puis lance l'API avec le premier profil de `launchSettings.json` (http://localhost:5080). Arrêt : **Ctrl+C**.

Autres commandes utiles :

```bash
dotnet build FormationPostman.sln
```

```bash
dotnet run --urls http://localhost:5090
```

La seconde commande lance l'API sur un autre port si 5080 est déjà occupé (pensez alors à modifier `baseUrl` dans l'environnement Postman).

---

## 1.5 Configurer Postman

### Importer la collection et l'environnement

1. Ouvrez Postman et connectez-vous.
2. Cliquez sur **Import** (en haut de la barre latérale gauche) ou faites **Ctrl+O**.
3. Glissez-déposez **les deux fichiers** du dossier `postman/` :
   - `Formation-API-REST.postman_collection.json`
   - `Formation-API-REST-Local.postman_environment.json`
4. Validez avec **Import**.
5. Résultat :
   - dans **Collections** : *Formation API REST - Produits* avec 8 dossiers (00 à 07) ;
   - dans **Environments** : *Formation API REST - Local*.

### Sélectionner l'environnement (étape indispensable)

En haut à droite de la fenêtre, la liste déroulante affiche **« No environment »**. Choisissez **« Formation API REST - Local »**.

Sans cette sélection, la variable `{{baseUrl}}` n'est pas remplacée et toutes les requêtes échouent (voir *Dépannage*).

### Contenu de l'environnement

| Variable | Valeur initiale | Rôle |
|---|---|---|
| `baseUrl` | `http://localhost:5080` | Adresse de l'API, utilisée dans toutes les URL : `{{baseUrl}}/api/produits` |
| `nomUtilisateur` | `admin` | Identifiant envoyé pour obtenir le token |
| `motDePasse` | `Admin123!` | Mot de passe envoyé pour obtenir le token |
| `token` | *(vide)* | Rempli **automatiquement** par la requête *Obtenir un token (admin)* |
| `tokenUtilisateur` | *(vide)* | Rempli automatiquement par *Obtenir un token (alice...)* |
| `produitId` | *(vide)* | Rempli automatiquement par *Créer un produit → 201* |

### Premier test

1. Ouvrez *00 - Démarrage > Ping - l'API répond-elle ?*
2. Cliquez sur **Send** (ou **Ctrl+Entrée**).
3. Résultat attendu : `200 OK` et le body :

   ```json
   {
     "message": "pong",
     "serveur": "API Produits (ASP.NET Core .NET 10)",
     "heureServeur": "2026-11-03T09:15:42.1234567+01:00"
   }
   ```

4. Onglet **Test Results** de la réponse : 4 tests sur 4 réussis (3 tests de la requête + 1 test de la collection sur le temps de réponse).

### Vérification complète (2 minutes)

Clic droit sur la collection (ou menu **⋯**) > **Run collection**, puis **Run Formation API REST - Produits**. Les 37 requêtes s'exécutent dans l'ordre : **tous les tests doivent être verts** (110 assertions). La requête *Réinitialiser les données de démo* (dossier 00) remet les données à zéro en début d'exécution : le Runner peut être relancé autant de fois que nécessaire.

> La collection a été vérifiée avec Newman (le moteur d'exécution en ligne de commande de Postman) contre l'API réelle : 37 requêtes, 110 assertions, 0 échec, y compris lors de deux exécutions successives.
>
> Commande facultative pour reproduire ce test (nécessite Node.js), depuis la racine du kit, API lancée :
>
> ```bash
> npx newman run postman/Formation-API-REST.postman_collection.json -e postman/Formation-API-REST-Local.postman_environment.json
> ```

---

## 1.6 Comptes de démonstration

| Identifiant | Mot de passe | Rôle | Droits |
|---|---|---|---|
| `admin` | `Admin123!` | Admin | lire, créer, modifier, **supprimer** |
| `alice` | `Alice123!` | Utilisateur | lire, créer, modifier ; **pas de suppression** (403) |

Le mot de passe est sensible aux majuscules, l'identifiant non.

---

## 1.7 Liste des endpoints

| Méthode | URL | Protection | Codes possibles |
|---|---|---|---|
| GET | `/` | public | 200 |
| GET | `/api/ping` | public | 200 |
| POST | `/api/auth/token` | public | 200, 400, 401 |
| GET | `/api/auth/moi` | token | 200, 401 |
| GET | `/api/produits` | public | 200, 400 |
| GET | `/api/produits/{id}` | public | 200, 404 |
| POST | `/api/produits` | token | 201, 400, 401, 409, 415 |
| PUT | `/api/produits/{id}` | token | 200, 400, 401, 404, 409, 415 |
| PATCH | `/api/produits/{id}` | token | 200, 400, 401, 404, 409, 415 |
| DELETE | `/api/produits/{id}` | token + rôle Admin | 204, 401, 403, 404 |
| GET | `/api/demo/headers` | public | 200 |
| GET | `/api/demo/erreur` | public | 500 (volontaire) |
| POST | `/api/demo/reinitialiser` | public | 200 |
| GET | `/openapi/v1.json` | public | 200 (description OpenAPI) |

Query parameters de `GET /api/produits` (tous facultatifs) :

| Paramètre | Exemple | Effet |
|---|---|---|
| `categorie` | `Audio` | Filtre sur la catégorie (Informatique, Audio, Bureau) |
| `recherche` | `souris` | Texte dans le nom ou la description, sans tenir compte des majuscules ni des accents |
| `prixMin` / `prixMax` | `10` / `100` | Fourchette de prix (incluse) |
| `enStock` | `true` / `false` | Disponibles / en rupture |
| `tri` | `id`, `nom`, `prix`, `stock` | Champ de tri (par défaut : `id`) |
| `ordre` | `asc` / `desc` | Ordre du tri (par défaut : `asc`) |
| `page` | `2` | Numéro de page (à partir de 1) |
| `taille` | `3` | Produits par page (1 à 50, par défaut 20) |

Le header de réponse **`X-Total-Count`** donne le nombre total de produits trouvés (avant pagination).

### Les 10 produits de démonstration

| id | Nom | Catégorie | Prix (€) | Stock |
|---|---|---|---|---|
| 1 | Clavier mécanique | Informatique | 89.90 | 15 |
| 2 | Souris sans fil | Informatique | 24.99 | 42 |
| 3 | Écran 27 pouces | Informatique | 249.00 | 8 |
| 4 | Casque audio Bluetooth | Audio | 79.50 | **0** |
| 5 | Enceinte portable | Audio | 49.90 | 20 |
| 6 | Lampe de bureau LED | Bureau | 34.90 | 12 |
| 7 | Chaise ergonomique | Bureau | 199.00 | 3 |
| 8 | Carnet de notes A5 | Bureau | 6.50 | 100 |
| 9 | Câble USB-C 2 m | Informatique | 12.90 | **0** |
| 10 | Webcam HD | Informatique | 59.90 | 7 |

Après une réinitialisation, le premier produit créé reçoit l'**id 11**.

---

## 1.8 Dépannage

| Symptôme | Cause probable | Solution |
|---|---|---|
| Postman : `Error: getaddrinfo ENOTFOUND {{baseUrl}}` ou *Invalid URL* | Aucun environnement sélectionné | Sélectionner *Formation API REST - Local* en haut à droite |
| Postman : `Error: connect ECONNREFUSED 127.0.0.1:5080` | L'API n'est pas lancée (ou s'est arrêtée) | Lancer l'API (F5 ou `dotnet run`) et vérifier la console |
| Console : `Failed to bind to address http://127.0.0.1:5080: address already in use` | Une autre instance de l'API (ou un autre programme) utilise le port | Fermer l'autre instance ; sinon changer le port dans `launchSettings.json` **et** `baseUrl` |
| Visual Studio : `NETSDK1045 : le SDK .NET actuel ne prend pas en charge .NET 10.0` | SDK .NET 10 absent | Installer le SDK .NET 10 / mettre à jour Visual Studio |
| Visual Studio : erreur de restauration NuGet | Pas d'accès Internet au premier build | Se connecter à Internet et recompiler (les paquets sont ensuite en cache) |
| Profil `https` sélectionné : avertissement de certificat | Certificat de développement HTTPS non approuvé | Repasser sur le profil `http` (recommandé pour la formation) |
| Postman : `SSL Error: Self signed certificate` (si `baseUrl` en https) | Certificat local non reconnu | Utiliser `http://localhost:5080`, ou désactiver *Settings > General > SSL certificate verification* |
| Toutes les requêtes protégées répondent 401 | Token absent ou expiré (60 minutes) | Renvoyer *01 - Authentification > Obtenir un token (admin)* |
| `409 Conflict` sur *Créer un produit* | Le produit « Tapis de souris XXL » existe déjà | Envoyer *00 - Démarrage > Réinitialiser les données de démo* ou changer le nom |
| Les données ont « disparu » | L'API a redémarré (données en mémoire) | Normal : on repart des 10 produits de démonstration |

---

## 1.9 Recréer le projet de zéro (facultatif)

Pour montrer à l'apprenant comment naît un tel projet, ou pour le reconstruire ailleurs :

```bash
dotnet new sln --format sln -n FormationPostman
```

```bash
dotnet new webapi --use-controllers -n ProduitsApi -o src/ProduitsApi --framework net10.0
```

```bash
dotnet sln FormationPostman.sln add src/ProduitsApi/ProduitsApi.csproj
```

```bash
dotnet add src/ProduitsApi package Microsoft.AspNetCore.Authentication.JwtBearer --version 10.0.14
```

Supprimez ensuite le contrôleur d'exemple `WeatherForecastController.cs` et la classe `WeatherForecast.cs` générés par le modèle, puis ajoutez les fichiers des dossiers `Controllers`, `Models`, `Services`, `Securite` et `Erreurs` du kit, et remplacez `Program.cs`, `appsettings.json` et `Properties/launchSettings.json`.

> Avec le SDK .NET 10, `dotnet new sln` crée par défaut un fichier `.slnx` (nouveau format). L'option `--format sln` produit le format classique `.sln`, utilisé par ce kit.
