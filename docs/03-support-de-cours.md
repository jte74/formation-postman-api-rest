# 3. Support de cours — API REST et Postman

> Support à projeter ou à partager avec l'apprenant. Chaque chapitre suit la même structure, pour que le formateur puisse l'animer sans préparation supplémentaire.

## Comment utiliser ce support

| Repère | Signification |
|---|---|
| 📖 **Théorie** | L'explication simple, à dire avec ses mots |
| 💡 **Analogie** | Une image de la vie courante pour ancrer la notion |
| 🧩 **Avec notre API** | L'exemple concret sur l'API de démonstration |
| 🔧 **Dans Postman** | La manipulation exacte, pas à pas |
| ✅ **Résultat attendu** | Ce que l'apprenant doit obtenir |
| ⚠️ **Erreurs fréquentes** | Les erreurs typiques et leur signification |
| 🎤 **Note pour le formateur** | Conseil d'animation |
| 📌 **À retenir** | Le résumé en 3 lignes |

**Prérequis techniques :** l'API tourne sur `http://localhost:5080`, Postman est ouvert, la collection *Formation API REST - Produits* et l'environnement *Formation API REST - Local* sont importés, et **l'environnement est sélectionné** (en haut à droite). Voir [01-installation-et-lancement.md](01-installation-et-lancement.md).

### Correspondance avec les notions du programme

| Notion | Chapitre |
|---|---|
| Qu'est-ce qu'une API ? | [1](#chapitre-1--quest-ce-quune-api-) |
| Qu'est-ce qu'une API REST ? | [2](#chapitre-2--quest-ce-quune-api-rest-) |
| Le protocole HTTP | [3](#chapitre-3--le-protocole-http) |
| Le cycle requête / réponse | [4](#chapitre-4--le-cycle-requête--réponse) |
| Prise en main de Postman | [5](#chapitre-5--prise-en-main-de-postman) |
| GET, POST, PUT, PATCH, DELETE | [6](#chapitre-6--les-méthodes-http) (vue d'ensemble), [12](#chapitre-12--le-body-le-format-json-et-post) et [13](#chapitre-13--put-patch-et-delete-en-pratique) (pratique) |
| Les headers HTTP | [8](#chapitre-8--les-headers-http) |
| Paramètres de route et query parameters | [7](#chapitre-7--paramètres-de-route-et-query-parameters) |
| Le body et le format JSON | [12](#chapitre-12--le-body-le-format-json-et-post) |
| Les codes de réponse HTTP | [9](#chapitre-9--les-codes-de-réponse-http) |
| L'authentification Bearer Token | [10](#chapitre-10--lauthentification-par-bearer-token) |
| Variables et environnements Postman | [11](#chapitre-11--variables-et-environnements-postman) |
| Tests automatisés dans Postman | [14](#chapitre-14--les-tests-automatisés-dans-postman) |

Les chapitres suivent l'**ordre pédagogique** du programme ([02-programme-de-la-formation.md](02-programme-de-la-formation.md)) : comprendre → lire → s'authentifier → écrire → automatiser.

---

# Partie A — Les bases

## Chapitre 1 — Qu'est-ce qu'une API ?

### 📖 Théorie

**API** signifie *Application Programming Interface*, en français **interface de programmation d'application**.

Une API est une **porte d'entrée** qu'un logiciel ouvre pour que **d'autres logiciels** puissent lui demander des informations ou des actions, **sans connaître son fonctionnement interne**.

Une API définit un **contrat** :
- quelles **demandes** on peut faire (lister des produits, créer une commande...) ;
- sous quelle **forme** les envoyer (adresse, format des données) ;
- ce que l'on **reçoit en retour** (les données, ou une erreur expliquée).

On utilise des API tous les jours sans le savoir :

| Situation | Ce qui se passe |
|---|---|
| L'application météo affiche la température | Elle interroge l'API d'un service météo |
| Payer en ligne | Le site marchand appelle l'API de la banque ou du prestataire de paiement |
| « Se connecter avec Google » | Le site utilise l'API d'authentification de Google |
| Une application de livraison affiche une carte | Elle appelle une API de cartographie |
| Suivre un colis | Le site du marchand appelle l'API du transporteur |

**Pourquoi c'est utile ?**
- **Séparer** : l'application mobile ne touche jamais directement la base de données.
- **Réutiliser** : une seule API sert l'application mobile, le site web et les partenaires.
- **Sécuriser** : l'API contrôle qui a le droit de faire quoi.
- **Mélanger les technologies** : une application iPhone écrite en Swift parle sans problème à une API écrite en C#.

### 💡 Analogie : le restaurant

![Fonctionnement général d'une API REST](schemas/01-api-rest-vue-generale.svg)

- **Le client** (vous, Postman, une application) lit **le menu** (la documentation de l'API) et passe commande.
- **Le serveur en salle** (l'API) prend la commande, la transmet en cuisine et rapporte le plat.
- **La cuisine** (le code métier et les données) prépare le plat. Le client n'y entre jamais.
- Commander un plat qui n'est pas au menu ? « Désolé, nous n'avons pas ça » : c'est l'équivalent d'une erreur **404**.

### 🧩 Avec notre API

Notre API gère le **catalogue de produits** d'un magasin. Elle permet de **lister, consulter, créer, modifier et supprimer** des produits. Des clients très différents pourraient l'utiliser : un site marchand, une application de caisse, un outil d'inventaire... et **Postman**, pour la tester.

### 🔧 Manipulation (dans le navigateur, avant Postman)

1. Ouvrir un navigateur.
2. Saisir l'adresse `http://localhost:5080/api/produits` et valider.

### ✅ Résultat attendu

Le navigateur affiche une liste de **10 produits** au format JSON :

```json
[
  {
    "id": 1,
    "nom": "Clavier mécanique",
    "description": "Clavier AZERTY rétroéclairé, touches mécaniques",
    "prix": 89.90,
    "categorie": "Informatique",
    "stock": 15,
    "dateCreation": "2026-01-15T09:00:00+01:00",
    "dateModification": null
  },
  ...
]
```

Ce n'est pas une page web mise en forme : ce sont des **données brutes**, faites pour être lues par un programme.

### ⚠️ Erreurs fréquentes

| Erreur | Explication |
|---|---|
| Confondre API et base de données | L'API est l'**intermédiaire** ; les données sont derrière elle. |
| Penser qu'une API a un écran | Une API s'adresse à des **programmes**. Elle renvoie des données (JSON), pas des pages. |
| « La page ne s'affiche pas » / *ERR_CONNECTION_REFUSED* | L'API n'est pas lancée, ou le port est faux. |

### 📌 À retenir

> Une API est un **contrat** qui permet à un logiciel de demander des données ou des actions à un autre logiciel, sans connaître son fonctionnement interne.

---

## Chapitre 2 — Qu'est-ce qu'une API REST ?

### 📖 Théorie

**REST** (*REpresentational State Transfer*) est un **style d'architecture** décrit en 2000 par Roy Fielding. Ce n'est ni un logiciel, ni une norme stricte : c'est un ensemble de **principes** pour concevoir des API simples et prévisibles.

Les principes, version simple :

1. **Tout est ressource**, identifiée par une **URL** qui est un **nom** (pas un verbe) : `/api/produits`, `/api/produits/3`.
2. On agit sur les ressources avec les **méthodes HTTP standard** : GET (lire), POST (créer), PUT (remplacer), PATCH (modifier), DELETE (supprimer). **Le verbe est dans la méthode, pas dans l'URL.**
3. Le serveur échange une **représentation** de la ressource, le plus souvent en **JSON**.
4. **Sans état** (*stateless*) : chaque requête contient tout ce qu'il faut pour être comprise (y compris le token). Le serveur ne se souvient pas de la requête précédente.
5. **Client / serveur** : le client (affichage) et le serveur (données) évoluent indépendamment.
6. Le résultat est exprimé avec les **codes HTTP standard** (200, 201, 404...).

| Action | ❌ Pas REST | ✅ REST |
|---|---|---|
| Lister les produits | `GET /api/getAllProducts` | `GET /api/produits` |
| Lire le produit 3 | `GET /api/getProduct?id=3` | `GET /api/produits/3` |
| Créer un produit | `POST /api/createProduct` | `POST /api/produits` |
| Modifier le produit 3 | `POST /api/updateProduct` | `PUT` ou `PATCH /api/produits/3` |
| Supprimer le produit 3 | `GET /api/deleteProduct?id=3` | `DELETE /api/produits/3` |

**Bonnes pratiques de nommage :** noms au pluriel, en minuscules, organisés en hiérarchie (`/api/clients/12/commandes`), sans verbe.

> Il existe d'autres styles d'API : **SOAP** (XML, plus ancien, encore présent dans les banques et administrations), **GraphQL** (le client choisit précisément les champs), **gRPC** (très performant, entre serveurs). **REST reste le plus répandu**, et c'est ce que l'on teste le plus souvent avec Postman.

### 💡 Analogie : la bibliothèque

Chaque livre possède une **cote unique** (l'URL). Les actions possibles sont **les mêmes pour tous les livres** : consulter, emprunter, rendre, retirer du catalogue (les méthodes). Inutile d'apprendre un vocabulaire différent pour chaque livre.

### 🧩 Avec notre API

Une seule ressource, **les produits**, et six opérations :

| Méthode | URL | Action |
|---|---|---|
| GET | `/api/produits` | lister (avec filtres facultatifs) |
| GET | `/api/produits/{id}` | lire un produit |
| POST | `/api/produits` | créer |
| PUT | `/api/produits/{id}` | remplacer entièrement |
| PATCH | `/api/produits/{id}` | modifier partiellement |
| DELETE | `/api/produits/{id}` | supprimer |

### 🔧 Dans Postman (observation, sans envoyer)

1. Dans la barre latérale **Collections**, dérouler *Formation API REST - Produits*.
2. Dérouler les dossiers **02** à **06**.
3. Observer les **étiquettes colorées** devant chaque requête : GET (vert), POST (jaune/orange), PUT (bleu), PATCH (violet), DELETE (rouge).

### ✅ Résultat attendu

L'apprenant remarque que **la même URL** `{{baseUrl}}/api/produits/{{produitId}}` apparaît avec **PUT, PATCH et DELETE** : c'est la **méthode** qui change l'action, pas l'URL.

### ⚠️ Erreurs fréquentes

| Erreur | Conséquence |
|---|---|
| Mettre l'action dans l'URL (`/api/supprimerProduit`) | Ce n'est pas REST ; dans notre API, la route n'existe pas → **404** |
| Faute dans l'URL : `/api/produit` (sans « s ») | **404** : aucune route ne correspond |
| Croire que REST impose JSON | JSON est le format le plus courant, pas une obligation |

### 📌 À retenir

> REST = des **ressources** (URL avec des noms) + des **méthodes HTTP** (les verbes) + des **codes de réponse** standard. Chaque requête est indépendante.

---

## Chapitre 3 — Le protocole HTTP

### 📖 Théorie

**HTTP** (*HyperText Transfer Protocol*) est la **langue commune** du web : un ensemble de règles qui permet à un client et à un serveur de se comprendre. Inventé pour afficher des pages web, il est utilisé par la quasi-totalité des API.

Ses caractéristiques :
- **Texte lisible** : une requête HTTP est du texte que l'on peut lire.
- **Requête / réponse** : c'est **toujours le client qui parle en premier** ; le serveur répond une fois.
- **Sans état** : chaque échange est indépendant.
- **HTTPS** = HTTP + chiffrement (le cadenas du navigateur). En local, nous utilisons HTTP pour simplifier. **En production, une API doit toujours être en HTTPS**, sinon les mots de passe et les tokens circulent en clair.
- Un serveur **écoute** sur un **port** : notre API écoute sur le port **5080** (par défaut : 80 pour HTTP, 443 pour HTTPS).

**Anatomie d'une URL :**

![Anatomie d'une URL](schemas/07-anatomie-url.svg)

`localhost` signifie « **cette machine-ci** » (adresse `127.0.0.1`) : Postman et l'API tournent sur le même ordinateur.

> HTTP/1.1, HTTP/2 et HTTP/3 sont des versions successives, de plus en plus rapides. Le sens des requêtes et des réponses reste identique : pour nous, aucune différence.

### 💡 Analogie : le courrier postal

- L'**adresse** sur l'enveloppe = l'**URL**.
- Les mentions « Recommandé », « Fragile », l'expéditeur = les **headers**.
- La lettre dans l'enveloppe = le **body**.
- Le tampon de la poste au retour (« Distribué », « Adresse inconnue ») = le **code de réponse**.

### 🧩 Avec notre API

Le serveur web **Kestrel**, intégré à .NET, écoute sur `http://localhost:5080` et transmet chaque requête au code C# :

![Dialogue entre Postman, l'API et le serveur](schemas/02-dialogue-postman-api-serveur.svg)

### 🔧 Démonstration (outils de développement du navigateur)

1. Dans le navigateur, appuyer sur **F12** et ouvrir l'onglet **Réseau** (*Network*).
2. Ouvrir `http://localhost:5080/api/ping`.
3. Cliquer sur la ligne `ping` qui apparaît dans la liste.

### ✅ Résultat attendu

| Information | Valeur |
|---|---|
| Request URL | `http://localhost:5080/api/ping` |
| Request Method | `GET` |
| Status Code | `200 OK` |
| Response header `Content-Type` | `application/json; charset=utf-8` |
| Response header `Server` | `Kestrel` |

Le navigateur a envoyé une **requête HTTP GET**, exactement comme Postman le fera.

### ⚠️ Erreurs fréquentes

| Erreur | Conséquence |
|---|---|
| Oublier le port : `http://localhost/api/ping` | Connexion refusée (personne n'écoute sur le port 80) |
| `https://localhost:5080` au lieu de `http://` | Erreur SSL : l'API locale écoute en HTTP sur ce port |
| Utiliser `localhost` depuis un autre ordinateur | `localhost` désigne toujours **sa propre** machine |
| Espace ou caractère parasite dans l'URL | 404 ou URL invalide |

### 📌 À retenir

> HTTP est la langue des API : une **requête** du client, **une réponse** du serveur. URL = protocole + hôte + port + chemin (+ paramètres).

---

## Chapitre 4 — Le cycle requête / réponse

### 📖 Théorie

![Cycle d'une requête HTTP et de sa réponse](schemas/03-cycle-requete-reponse.svg)

1. Le **client** prépare la requête : **méthode**, **URL**, **headers**, **body** (facultatif).
2. Il l'**envoie** au serveur.
3. Le **serveur** la reçoit, **vérifie** (authentification, validation des données) et **traite** (lit ou modifie les données).
4. Il renvoie **une réponse** : **code de statut**, **headers**, **body**.
5. Le client **lit** la réponse. L'échange est terminé.

**Une requête se compose de :**

| Élément | Rôle | Exemple |
|---|---|---|
| Méthode | L'action souhaitée | `POST` |
| URL | La ressource visée | `http://localhost:5080/api/produits` |
| Headers | Des informations sur la requête | `Content-Type: application/json`, `Authorization: Bearer eyJ...` |
| Body | Les données envoyées (facultatif) | `{ "nom": "Tapis de souris XXL", ... }` |

**Une réponse se compose de :**

| Élément | Rôle | Exemple |
|---|---|---|
| Code de statut | Le résultat, en 3 chiffres | `201 Created` |
| Headers | Des informations sur la réponse | `Location: http://localhost:5080/api/produits/11` |
| Body | Les données ou l'explication de l'erreur | `{ "id": 11, "nom": "Tapis de souris XXL", ... }` |

**Le texte réellement échangé** ressemble à ceci :

```http
POST /api/produits HTTP/1.1
Host: localhost:5080
Content-Type: application/json
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

{"nom":"Tapis de souris XXL","prix":19.90,"categorie":"Informatique","stock":25}
```

```http
HTTP/1.1 201 Created
Content-Type: application/json; charset=utf-8
Location: http://localhost:5080/api/produits/11

{"id":11,"nom":"Tapis de souris XXL","prix":19.90,"categorie":"Informatique","stock":25, ...}
```

**Distinction importante :**
- **Pas de réponse du tout** (*Could not send request*, *ECONNREFUSED*, *timeout*) → problème de **réseau** : l'API est arrêtée, l'adresse ou le port est faux.
- **Une réponse avec un code d'erreur** (400, 404, 500...) → l'API **a bien répondu** : elle explique ce qui ne va pas.

### 💡 Analogie : le comptoir de la boulangerie

Vous demandez une baguette (requête), la boulangère vous la tend avec le prix (réponse), et l'échange est terminé. Si vous revenez cinq minutes plus tard, elle ne se souvient pas de vous : vous devez tout redemander. C'est pour cela qu'avec une API on renvoie le **token à chaque requête**.

### 🧩 Avec notre API

Pour un `POST /api/produits`, le serveur enchaîne : journalisation → vérification du token → choix de la méthode C# → validation du body → enregistrement → réponse `201 Created` (voir les 8 étapes du schéma du chapitre 3).

### 🔧 Manipulation (navigateur + console de l'API)

1. Placer la **console de l'API** (fenêtre noire ouverte par Visual Studio) à côté du navigateur.
2. Ouvrir `http://localhost:5080/api/produits/3`.
3. Ouvrir `http://localhost:5080/api/produits/999`.

### ✅ Résultat attendu

- Navigateur, étape 2 : le JSON du produit 3 (« Écran 27 pouces »).
- Navigateur, étape 3 : un JSON d'erreur `"title": "Produit introuvable"`, `"status": 404`.
- Console de l'API :

  ```
  09:12:03 info: HTTP[0] GET /api/produits/3 -> 200 (2 ms)
  09:12:09 info: HTTP[0] GET /api/produits/999 -> 404 (1 ms)
  ```

Dans les deux cas, **le serveur a répondu** : une fois avec les données, une fois avec une erreur expliquée.

### ⚠️ Erreurs fréquentes

| Erreur | Explication |
|---|---|
| Confondre « pas de réponse » et « réponse d'erreur » | Pas de réponse = réseau ; code 4xx/5xx = l'API a répondu |
| Ne regarder que le body | Toujours lire **d'abord le code de statut** |
| Croire que le serveur « se souvient » | HTTP est sans état : chaque requête doit être complète |

### 📌 À retenir

> Requête = **méthode + URL + headers + body**. Réponse = **code + headers + body**. Une requête, une réponse, puis le serveur oublie.

---

## Chapitre 5 — Prise en main de Postman

### 📖 Théorie

**Postman** est un **client HTTP** avec une interface graphique : il permet de construire, envoyer, enregistrer, organiser et tester des requêtes **sans écrire de programme**. Il est utilisé par les développeurs (tester leur API), les testeurs (QA), les intégrateurs (découvrir l'API d'un partenaire) et les équipes support.

> Alternatives : Insomnia, Bruno, Hoppscotch, l'extension Thunder Client de VS Code, les fichiers `.http` de Visual Studio, ou la commande `curl`. Les notions sont les mêmes partout.

| Terme Postman | Définition |
|---|---|
| **Workspace** | Espace de travail (personnel ou partagé avec une équipe) |
| **Collection** | Ensemble de requêtes enregistrées, comme un projet |
| **Dossier** (*folder*) | Sous-groupe de requêtes dans une collection |
| **Requête** (*request*) | Méthode + URL + headers + body + scripts, enregistrés |
| **Environnement** | Jeu de variables (ex. : adresse locale ou adresse de production) |
| **Variable** | `{{nom}}`, remplacée par sa valeur au moment de l'envoi |
| **Collection Runner** | Exécute toutes les requêtes d'une collection à la suite |

**Repères dans l'interface :**

```
┌────────────────┬──────────────────────────────────────────────────────────────┐
│ Collections    │  [GET ▼] [ {{baseUrl}}/api/ping                  ]  [ Send ]  │ ← méthode, URL, envoi
│ Environments   │  Params  Authorization  Headers  Body  Scripts  Settings     │ ← onglets de la requête
│ History        │                                                              │
│                ├──────────────────────────────────────────────────────────────┤
│ ▾ Formation    │  Body  Cookies  Headers  Test Results     200 OK  12 ms  312 B│ ← la réponse
│   API REST     │  {                                                           │
│   ▸ 00 - ...   │      "message": "pong", ...                                  │
└────────────────┴──────────────────────────────────────────────────────────────┘
  Console (en bas à gauche)            [ Formation API REST - Local ▼ ] (en haut à droite)
```

> Selon la version de Postman, l'onglet **Authorization** peut s'appeler **Auth**, et l'onglet **Scripts** (avec *Pre-request* et *Post-response*) s'appelait autrefois **Pre-request Script** et **Tests**.

### 💡 Analogie : la télécommande universelle

Postman est une **télécommande universelle pour API** : on programme les boutons une fois (les requêtes enregistrées), puis on appuie dessus autant de fois que nécessaire, sur n'importe quel « appareil » (environnement local, recette, production).

### 🧩 Avec notre API

La collection fournie contient **37 requêtes** rangées en 8 dossiers qui suivent ce cours : `00 - Démarrage`, `01 - Authentification`, `02 - Lire des produits (GET)`, `03 - Créer (POST)`, `04 - Remplacer (PUT)`, `05 - Modifier partiellement (PATCH)`, `06 - Supprimer (DELETE)`, `07 - Erreurs et cas particuliers`.

### 🔧 Dans Postman

**A. Sélectionner l'environnement** (si ce n'est pas déjà fait)
1. En haut à droite, ouvrir la liste **No environment**.
2. Choisir **Formation API REST - Local**.

**B. Voir les variables**
1. Cliquer sur l'icône **œil** (*Environment quick look*) à côté de la liste des environnements.
2. Observer `baseUrl = http://localhost:5080` ; `token` et `produitId` sont vides pour l'instant.

**C. Envoyer la première requête**
1. Dans la collection, ouvrir **00 - Démarrage > Ping - l'API répond-elle ?**
2. Observer la méthode **GET** et l'URL `{{baseUrl}}/api/ping`. **Survoler** `{{baseUrl}}` : Postman affiche sa valeur.
3. Cliquer sur **Send** (raccourci **Ctrl+Entrée**).

**D. Lire la réponse**
1. En haut à droite de la zone de réponse : le **code** (200 OK), le **temps** (en ms), la **taille**.
2. Onglet **Body** : vues **Pretty** (formatée), **Raw** (texte brut), **Preview**.
3. Onglet **Headers** de la réponse : `Content-Type`, `Date`, `Server`...
4. Onglet **Test Results** : les tests automatiques de la requête.

**E. Créer sa propre requête**
1. Clic droit sur la collection (ou menu **⋯**) > **Add folder**, le nommer **Mes requêtes**.
2. Clic droit sur **Mes requêtes** > **Add request**, la nommer **Ma première liste**.
3. Laisser la méthode **GET**, saisir l'URL `{{baseUrl}}/api/produits`.
4. Cliquer sur **Send**, puis enregistrer avec **Ctrl+S**.

**F. Ouvrir la Console Postman**
1. Cliquer sur **Console** en bas à gauche (ou **Ctrl+Alt+C**).
2. Renvoyer *Ping* : la console affiche la requête envoyée, ses headers et la réponse brute.

### ✅ Résultat attendu

- *Ping* : `200 OK`, body `{"message": "pong", ...}`, **Test Results : 4/4** (3 tests propres à la requête + 1 test commun à la collection sur le temps de réponse).
- *Ma première liste* : `200 OK`, un tableau de 10 produits.
- La console affiche la ligne `GET http://localhost:5080/api/ping` (écrite par le script de collection) et le détail de l'échange.

### ⚠️ Erreurs fréquentes

| Symptôme | Cause | Solution |
|---|---|---|
| `Error: getaddrinfo ENOTFOUND {{baseUrl}}` ou `{{baseUrl}}` surligné en rouge | Aucun environnement sélectionné | Sélectionner *Formation API REST - Local* |
| `Error: connect ECONNREFUSED 127.0.0.1:5080` | L'API n'est pas lancée | La lancer (F5 dans Visual Studio) |
| Un point orange sur l'onglet de la requête | Modifications non enregistrées | **Ctrl+S** |
| La requête d'origine de la collection a été modifiée par erreur | — | Clic droit > **Duplicate** avant d'expérimenter |
| Postman dans le navigateur ne joint pas `localhost` | La version web ne peut pas appeler `localhost` seule | Utiliser l'application de bureau |

### 🎤 Note pour le formateur

Agrandissez l'affichage pour le partage d'écran (**Ctrl + +**). Faites remarquer dès maintenant la console de l'API : chaque *Send* y ajoute une ligne. C'est la preuve visible du dialogue client / serveur.

### 📌 À retenir

> Dans Postman : **méthode + URL + Send**, puis lire **le code, le body et les headers**. Toujours vérifier l'**environnement sélectionné**.

---

# Partie B — Lire des données

## Chapitre 6 — Les méthodes HTTP

### 📖 Théorie

La **méthode** (ou verbe) indique **l'intention** de la requête. Sur une même URL, des méthodes différentes produisent des actions différentes.

![Comparaison des méthodes HTTP](schemas/04-methodes-http.svg)

| Méthode | Action | CRUD | Body envoyé | Succès typique |
|---|---|---|---|---|
| **GET** | Lire | **R**ead | non | 200 OK |
| **POST** | Créer | **C**reate | oui, complet | 201 Created |
| **PUT** | Remplacer entièrement | **U**pdate | oui, complet | 200 OK ou 204 |
| **PATCH** | Modifier partiellement | **U**pdate | oui, partiel | 200 OK |
| **DELETE** | Supprimer | **D**elete | non | 204 No Content |

**Deux propriétés utiles :**
- **Sûre** : ne modifie rien sur le serveur. Seule **GET** (ainsi que HEAD et OPTIONS) est sûre : on peut l'envoyer sans risque, autant de fois que l'on veut.
- **Idempotente** : envoyer la requête **1 fois ou 10 fois** laisse le serveur **dans le même état**.
  - `DELETE /api/produits/11` : la 1re fois → 204, la 2e → 404. Mais l'état final est identique : le produit n'existe plus. **Idempotente.**
  - `PUT` avec le même body : le produit a toujours les mêmes valeurs. **Idempotente.**
  - `POST` : chaque envoi tente de **créer un nouveau** produit. **Non idempotente.** (Dans notre API, le 2e envoi est refusé avec un 409 car le nom doit être unique.)

> Autres méthodes, plus rares : **HEAD** (comme GET mais sans body), **OPTIONS** (quelles méthodes sont autorisées ? utilisée automatiquement par les navigateurs).

### 💡 Analogie : le fichier de fiches cartonnées

- **GET** : lire une fiche.
- **POST** : ajouter une nouvelle fiche ; le secrétariat lui attribue un numéro.
- **PUT** : remplacer la fiche entière par une fiche réécrite.
- **PATCH** : corriger une seule ligne au stylo.
- **DELETE** : retirer la fiche du fichier.

### 🧩 Avec notre API

| Requête | Effet | Code |
|---|---|---|
| `GET /api/produits` | liste | 200 |
| `GET /api/produits/3` | produit 3 | 200 |
| `POST /api/produits` | crée un produit | 201 |
| `PUT /api/produits/11` | remplace le produit 11 | 200 |
| `PATCH /api/produits/11` | modifie certains champs du produit 11 | 200 |
| `DELETE /api/produits/11` | supprime le produit 11 | 204 |

### 🔧 Dans Postman

1. Ouvrir **02 - Lire des produits (GET) > Lister tous les produits** et cliquer sur **Send**.
2. Ouvrir la requête personnelle **Mes requêtes > Ma première liste**.
3. Dans la liste déroulante de la méthode (à gauche de l'URL), choisir **DELETE**, sans rien changer d'autre. Cliquer sur **Send**.
4. Remettre la méthode sur **GET** (sans enregistrer la modification).

### ✅ Résultat attendu

- Étape 1 : `200 OK`, 10 produits, header de réponse `X-Total-Count: 10`.
- Étape 3 : **`405 Method Not Allowed`**. Le header de réponse `Allow: GET, POST` liste les méthodes acceptées sur `/api/produits`. Le body l'explique :

  ```json
  {
    "title": "Méthode HTTP non autorisée",
    "status": 405,
    "detail": "La méthode DELETE n'est pas acceptée sur l'URL '/api/produits'. Vérifiez la méthode choisie dans Postman et l'URL (l'identifiant est-il oublié ?). Le header de réponse 'Allow' liste les méthodes acceptées."
  }
  ```

  Même URL, autre méthode, autre résultat : **la méthode compte autant que l'URL.**

### ⚠️ Erreurs fréquentes

| Erreur | Conséquence |
|---|---|
| Body rempli, mais méthode restée sur **GET** | L'API ignore le body et renvoie la liste (200) : rien n'est créé ! |
| PUT au lieu de PATCH pour changer un seul champ | **400** : les autres champs obligatoires manquent |
| Bonne méthode mais URL sans identifiant (`PUT /api/produits`) | **405** Method Not Allowed |
| Être surpris par le body vide d'un DELETE réussi | Normal : **204 No Content** |

### 📌 À retenir

> GET lit, POST crée, PUT remplace, PATCH modifie, DELETE supprime. **La méthode exprime l'action, l'URL désigne la ressource.**

---

## Chapitre 7 — Paramètres de route et query parameters

### 📖 Théorie

![Anatomie d'une URL](schemas/07-anatomie-url.svg)

| | Paramètre de route | Query parameter |
|---|---|---|
| Où ? | Dans le **chemin** : `/api/produits/3` | Après le **`?`** : `/api/produits?categorie=Audio` |
| Rôle | Identifier **une** ressource précise | **Filtrer, trier, paginer**, préciser une option |
| Obligatoire ? | Oui, pour cette route | Non, en général facultatif |
| Plusieurs ? | Selon la route (`/api/clients/12/commandes/5`) | Oui, séparés par **`&`**, dans n'importe quel ordre |
| Si rien ne correspond | **404** (cette ressource n'existe pas) | **200** avec une liste vide `[]` |
| Côté C# (notre API) | `[HttpGet("{id:int}")]` | `[FromQuery] FiltreProduits filtre` |

Les caractères spéciaux d'une valeur (espace, `&`, accents...) sont **encodés** dans l'URL : un espace devient `%20`. Postman le fait automatiquement.

### 💡 Analogie : la livraison

- **Paramètre de route** = l'adresse exacte : « 12 rue des Lilas » (où livrer, précisément).
- **Query parameters** = les instructions : « sonner deux fois, laisser chez le voisin si absent » (options facultatives).

### 🧩 Avec notre API

Query parameters acceptés par `GET /api/produits` :

| Paramètre | Exemple | Effet |
|---|---|---|
| `categorie` | `Audio` | Catégorie : Informatique, Audio, Bureau (insensible à la casse) |
| `recherche` | `souris` | Texte dans le nom ou la description, sans tenir compte des majuscules ni des accents |
| `prixMin`, `prixMax` | `10`, `100` | Fourchette de prix, bornes incluses (séparateur décimal : **point**) |
| `enStock` | `true` / `false` | Disponibles / en rupture |
| `tri` | `id`, `nom`, `prix`, `stock` | Champ de tri (défaut : `id`) |
| `ordre` | `asc` / `desc` | Croissant / décroissant (défaut : `asc`) |
| `page`, `taille` | `2`, `3` | Pagination : numéro de page (≥ 1) et nombre par page (1 à 50, défaut 20) |

Exemples (données de démonstration) :

| Requête | Résultat |
|---|---|
| `GET /api/produits/3` | Écran 27 pouces |
| `GET /api/produits?categorie=Audio` | Casque audio Bluetooth, Enceinte portable |
| `GET /api/produits?categorie=Informatique&enStock=true&prixMax=100&tri=prix&ordre=desc` | Clavier mécanique (89.90), Webcam HD (59.90), Souris sans fil (24.99) — `X-Total-Count: 3` |
| `GET /api/produits?page=2&taille=3` | produits 4, 5 et 6 — `X-Total-Count: 10` |
| `GET /api/produits?recherche=ecran` | Écran 27 pouces (la recherche ignore l'accent) |

### 🔧 Dans Postman

**A. Paramètre de route (Path Variable)**
1. Ouvrir **02 > Obtenir un produit par son id (paramètre de route)**.
2. L'URL est `{{baseUrl}}/api/produits/:id`. La syntaxe **`:id`** crée une *Path Variable*.
3. Onglet **Params**, section **Path Variables** : `id = 3`. Cliquer sur **Send**.
4. Remplacer `3` par `7` et renvoyer.

**B. Query parameters**
1. Ouvrir **02 > Filtres combinés et tri** et regarder l'onglet **Params**, section **Query Params** : une ligne par paramètre (clé, valeur, description).
2. Cliquer sur **Send**.
3. **Cocher** la ligne `recherche = souris` : l'URL se complète automatiquement avec `&recherche=souris`. Renvoyer.
4. **Décocher** `recherche`, puis **décocher** `enStock` : les paramètres disparaissent de l'URL. Renvoyer.

**C. Écrire les paramètres à la main**
1. Dans *Ma première liste*, compléter l'URL : `{{baseUrl}}/api/produits?categorie=Bureau&tri=prix`.
2. Constater que le tableau **Query Params** se remplit tout seul. Cliquer sur **Send**.

### ✅ Résultat attendu

- A.3 : `200 OK`, produit **3** « Écran 27 pouces ». A.4 : produit **7** « Chaise ergonomique ».
- B.2 : 3 produits : Clavier mécanique, Webcam HD, Souris sans fil (prix décroissants).
- B.3 : 1 seul produit : **Souris sans fil**.
- B.4 : sans le filtre de stock, le **Câble USB-C 2 m** (12.90, en rupture) apparaît : Clavier mécanique, Webcam HD, Souris sans fil, Câble USB-C 2 m — `X-Total-Count: 4`.
- C.2 : Carnet de notes A5 (6.50), Lampe de bureau LED (34.90), Chaise ergonomique (199.00).

### ⚠️ Erreurs fréquentes

| Requête | Résultat | Explication |
|---|---|---|
| `/api/produits/999` | **404** « Produit introuvable » | Aucun produit avec cet identifiant |
| `/api/produits/abc` | **404** « Ressource introuvable » | La route attend un **entier** : aucune route ne correspond |
| `?categorie=audio` | 200 (fonctionne) | Notre API ignore la casse des valeurs. Ce n'est pas le cas de toutes les API |
| `?categori=Audio` (faute) | **200 avec les 10 produits** | Un paramètre inconnu est **ignoré silencieusement** : piège classique ! |
| `?categorie=Audio?tri=prix` (deux `?`) | **200 avec `[]`** | Le 2e `?` fait partie de la valeur : la catégorie cherchée est « Audio?tri=prix ». Utiliser `&` |
| `?taille=500` | **400**, `errors.taille` | Valeur hors limites (1 à 50) |
| `?enStock=oui` | **400** « La valeur 'oui' n'est pas valide pour 'enStock' » | Attendu : `true` ou `false` |
| `?prixMax=12,90` | **400** « utilisez un point comme séparateur décimal » | Dans une URL, on écrit `12.90` |
| `?prixMin=50&prixMax=10` | **400** « prixMin ne peut pas être supérieur à prixMax » | Incohérence entre deux paramètres |

### 📌 À retenir

> **Route** = *quelle* ressource (`/produits/3`, sinon 404). **Query** = *comment* filtrer la liste (`?categorie=Audio&tri=prix`, liste vide possible). Le premier paramètre commence par `?`, les suivants par `&`.

---

## Chapitre 8 — Les headers HTTP

### 📖 Théorie

Les **headers** (en-têtes) sont des **métadonnées** au format `Nom: valeur`, qui donnent des informations **sur** la requête ou la réponse, sans faire partie des données elles-mêmes. Le nom d'un header n'est pas sensible à la casse.

**Principaux headers de requête :**

| Header | Rôle | Exemple |
|---|---|---|
| `Host` | Serveur visé | `localhost:5080` |
| `User-Agent` | Logiciel client | `PostmanRuntime/7.x` |
| `Accept` | Format de réponse **souhaité** | `application/json` |
| `Content-Type` | Format du body **envoyé** | `application/json` |
| `Content-Length` | Taille du body (calculée automatiquement) | `112` |
| `Authorization` | Preuve d'identité | `Bearer eyJhbGciOi...` |

**Principaux headers de réponse :**

| Header | Rôle | Exemple dans notre API |
|---|---|---|
| `Content-Type` | Format du body renvoyé | `application/json; charset=utf-8` (ou `application/problem+json` pour une erreur) |
| `Date`, `Server` | Date de la réponse, logiciel serveur | `Kestrel` |
| `Location` | URL de la ressource créée | après un POST : `http://localhost:5080/api/produits/11` |
| `WWW-Authenticate` | Méthode d'authentification attendue | sur un 401 : `Bearer` |
| `Allow` | Méthodes acceptées | sur un 405 : `GET, POST` |
| `X-Total-Count` | **Header personnalisé** : nombre total de résultats | `10` |

> `Content-Type` = « voici le format de **ce que j'envoie** ». `Accept` = « voici le format de **ce que je veux recevoir** ».
> Le préfixe `X-` est une convention (ancienne mais toujours très utilisée) pour les headers non standard.

### 💡 Analogie : les mentions sur un colis

« Fragile », « Recommandé », le nom de l'expéditeur, « Contenu : documents »... Le transporteur lit ces mentions **sans ouvrir le colis**. Les headers jouent ce rôle : le serveur sait comment traiter la requête avant même de lire le body.

### 🧩 Avec notre API

L'endpoint pédagogique `GET /api/demo/headers` renvoie **tous les headers reçus par le serveur**. C'est le moyen idéal de voir ce que Postman envoie réellement.

### 🔧 Dans Postman

**A. Les headers envoyés**
1. Ouvrir **00 - Démarrage > Voir les headers envoyés par Postman**.
2. Onglet **Headers** : deux headers ajoutés à la main, `X-Formation: bonjour` et `Accept: application/json`.
3. Cliquer sur le lien **« hidden »** (ex. « 7 hidden ») : Postman affiche les headers qu'il ajoute **automatiquement** (`Postman-Token`, `Host`, `User-Agent`, `Accept-Encoding`, `Connection`...).
4. Cliquer sur **Send** et lire `headersRecus` dans la réponse.

**B. Ajouter et retirer un header**
1. **Décocher** `X-Formation` et renvoyer : il disparaît de `headersRecus`.
2. Ajouter une ligne `X-Prenom` avec votre prénom comme valeur, renvoyer : il apparaît.

**C. Les headers de réponse**
1. Ouvrir **02 > Lister tous les produits** et envoyer.
2. Dans la zone de réponse, onglet **Headers** : repérer `Content-Type` et `X-Total-Count`.

### ✅ Résultat attendu

Étape A.4, réponse (extrait) :

```json
{
  "methode": "GET",
  "url": "http://localhost:5080/api/demo/headers",
  "nombreDeHeaders": 8,
  "headersRecus": {
    "Accept": "application/json",
    "Accept-Encoding": "gzip, deflate, br",
    "Cache-Control": "no-cache",
    "Connection": "keep-alive",
    "Host": "localhost:5080",
    "Postman-Token": "4f2a6c1e-...",
    "User-Agent": "PostmanRuntime/7.43.0",
    "X-Formation": "bonjour"
  }
}
```

(Le nombre exact de headers et la version de `PostmanRuntime` varient selon la version de Postman.)

Étape C.2 : `Content-Type: application/json; charset=utf-8` et `X-Total-Count: 10`.

### ⚠️ Erreurs fréquentes

| Erreur | Conséquence |
|---|---|
| Body JSON envoyé avec un mauvais `Content-Type` (ex. `text/plain`) | **415** Unsupported Media Type |
| Header `Authorization` mal écrit (« Bearer » oublié, `Bearer:` avec deux-points) | **401** avec un message expliquant le format attendu |
| Token entouré de guillemets : `Bearer "eyJ..."` | **401** token invalide |
| `Authorization` défini à la fois dans l'onglet *Authorization* et dans *Headers* | Confusion sur la valeur envoyée : utiliser **uniquement** l'onglet *Authorization* |
| Chercher les données dans les headers | Les données sont dans le **body** ; les headers sont des informations annexes |

### 📌 À retenir

> Les headers sont les **étiquettes** de la requête et de la réponse. Les trois à connaître : **`Content-Type`**, **`Accept`**, **`Authorization`**.

---

## Chapitre 9 — Les codes de réponse HTTP

### 📖 Théorie

Chaque réponse commence par un **code de statut** à 3 chiffres. **Le premier chiffre donne la famille** et indique **qui doit agir**.

![Les codes de réponse HTTP](schemas/08-codes-http.svg)

| Code | Nom | Signification | Dans notre API | Que faire ? |
|---|---|---|---|---|
| **200** | OK | Succès | GET, PUT, PATCH réussis | Lire le body |
| **201** | Created | Ressource créée | POST réussi (+ header `Location`) | Noter l'`id` |
| **204** | No Content | Succès, rien à renvoyer | DELETE réussi | Rien : c'est normal que le body soit vide |
| **400** | Bad Request | Requête invalide | Champ manquant, valeur hors limites, JSON mal formé | Lire `errors` et corriger |
| **401** | Unauthorized | **Non authentifié** | Pas de token, token invalide ou expiré | Obtenir / renvoyer un token |
| **403** | Forbidden | **Authentifié mais pas autorisé** | alice tente un DELETE | Utiliser un compte qui a le droit |
| **404** | Not Found | Introuvable | id inexistant, URL mal écrite | Vérifier l'URL et l'id |
| **405** | Method Not Allowed | Méthode refusée sur cette URL | `PUT /api/produits` sans id | Vérifier méthode et URL (header `Allow`) |
| **409** | Conflict | Conflit avec l'état actuel | Nom de produit déjà utilisé | Changer le nom |
| **415** | Unsupported Media Type | Format du body non supporté | Body envoyé en *Text* | Body > raw > **JSON** |
| **500** | Internal Server Error | Erreur du serveur | `GET /api/demo/erreur` | Prévenir l'équipe de l'API (avec le `traceId`) |

> Les codes **3xx** (redirections, ex. 301, 302, 307) indiquent que la ressource est ailleurs. Postman suit automatiquement les redirections. Notre API n'en utilise pas.

**Le body des erreurs.** Toutes les erreurs de notre API utilisent le format standard **ProblemDetails** (RFC 9457) :

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Produit introuvable",
  "status": 404,
  "detail": "Aucun produit ne porte l'identifiant 999. Utilisez GET /api/produits pour voir les identifiants existants.",
  "instance": "/api/produits/999",
  "traceId": "00-9b5bcb8850216d8ffe6d6e928bbcb99b-6907e3f0ccd77b0a-00"
}
```

Pour une erreur de validation (400), un champ `errors` détaille chaque problème, champ par champ.

### 💡 Analogie : le guichet

- **2xx** : « Voilà, c'est fait. »
- **4xx** : « Votre formulaire est mal rempli / vous n'avez pas votre badge / ce dossier n'existe pas. » C'est **au client** de corriger.
- **5xx** : « Désolé, notre système est en panne. » Le client n'y peut rien : c'est **au serveur** de corriger.

Pour **401 / 403** : à l'entrée d'un immeuble, **401** = « Je ne vous connais pas, montrez votre badge » ; **403** = « Je vous connais, mais votre badge n'ouvre pas cet étage. »

### 🔧 Dans Postman

1. Envoyer **02 > Produit inexistant → 404**.
2. Envoyer **02 > Query param invalide → 400** et lire le champ `errors`.
3. Envoyer **07 > Erreur serveur → 500**, puis regarder la **console de l'API**.
4. Survoler le code de statut affiché dans la zone de réponse : Postman en affiche la description.

### ✅ Résultat attendu

- Étape 1 : `404 Not Found`, `"title": "Produit introuvable"`.
- Étape 2 : `400 Bad Request`, `"errors": { "taille": ["taille doit être compris entre 1 et 50."] }`.
- Étape 3 : `500 Internal Server Error`, `"title": "Erreur interne du serveur"`. La console de l'API affiche une ligne `fail:` avec le détail de l'exception (*InvalidOperationException : Erreur volontaire...*). En environnement de développement, le body contient aussi un champ `exception` ; en production, ce détail ne doit jamais être exposé.
- Le code s'affiche en **vert** pour 2xx, en **rouge** ou orange pour 4xx/5xx.

### ⚠️ Erreurs fréquentes

| Erreur | Explication |
|---|---|
| Ne pas lire le body d'une erreur | Il explique **précisément** le problème (`title`, `detail`, `errors`) |
| Croire que 200 = tout va bien | Vérifier aussi le **contenu** (ex. une liste vide `[]` peut révéler un filtre mal écrit) |
| Confondre 401 et 403 | 401 = **qui êtes-vous ?** ; 403 = **vous n'avez pas le droit** |
| Modifier sa requête après un 500 | Inutile : le problème est côté serveur. Transmettre le `traceId` à l'équipe |
| 404 = « l'API ne marche pas » | 404 peut signifier une URL fausse **ou** un identifiant inexistant : lire le `detail` |

### 📌 À retenir

> **2xx** succès, **4xx** erreur du client (à corriger dans Postman), **5xx** erreur du serveur. Toujours lire le **body** des erreurs.

---

# Partie C — Sécuriser

## Chapitre 10 — L'authentification par Bearer Token

### 📖 Théorie

Tout le monde peut **consulter** le catalogue, mais seules les personnes autorisées doivent pouvoir le **modifier**. Il faut donc :
- **s'authentifier** : prouver **qui l'on est** ;
- puis être **autorisé** : avoir **le droit** de faire l'action demandée.

Le principe du **Bearer Token** (« jeton du porteur ») :

1. On envoie **une seule fois** son identifiant et son mot de passe à un endpoint dédié (`POST /api/auth/token`).
2. Si c'est correct, le serveur renvoie un **token** : une longue chaîne de caractères, valable pour une durée limitée (60 minutes ici).
3. Ensuite, **à chaque requête protégée**, on présente ce token dans le header :

   ```
   Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOi...
   ```

4. Le serveur vérifie le token. S'il est valide, la requête est acceptée.

« **Bearer** » signifie « **porteur** » : **quiconque possède le token est accepté**. Il faut donc le protéger comme un mot de passe : ne pas le publier, ne pas l'envoyer par e-mail, et toujours utiliser HTTPS en production.

![L'authentification par Bearer Token](schemas/06-authentification-bearer.svg)

**Le JWT (JSON Web Token).** Notre token est un JWT : trois parties en Base64, séparées par des points.
- **En-tête** : l'algorithme de signature (HS256).
- **Contenu** (*payload*, les *claims*) : qui est l'utilisateur (`sub`), son rôle (`role`), la date d'expiration (`exp`)... Ce contenu est **lisible par n'importe qui** (il est encodé, pas chiffré) : **on n'y met jamais de secret**.
- **Signature** : calculée avec une **clé secrète connue du seul serveur**. Si quelqu'un modifie le contenu (par exemple pour se donner le rôle Admin), la signature ne correspond plus et le serveur refuse le token.

Le serveur n'a **pas besoin de mémoriser** les tokens qu'il a distribués : il lui suffit de vérifier la signature et la date d'expiration. C'est cohérent avec le principe « sans état » de REST.

**Pourquoi ne pas envoyer le mot de passe à chaque requête ?** Le mot de passe circulerait sans cesse ; un token, lui, **expire**, peut porter des **droits limités** et peut être **renouvelé** sans redonner son mot de passe.

### 💡 Analogie : le bracelet de festival

- À l'**accueil** (`POST /api/auth/token`), on montre sa pièce d'identité (identifiant + mot de passe) et on reçoit un **bracelet** (le token) valable jusqu'à une certaine heure.
- Ensuite, aux entrées, on montre **seulement le bracelet** (le header `Authorization`) : plus besoin de la carte d'identité.
- Pas de bracelet, ou bracelet expiré : « Passez d'abord à l'accueil » → **401**.
- Bracelet standard devant la zone VIP : « Vous êtes bien festivalier, mais pas VIP » → **403**.
- Le vigile regarde le bracelet, pas le visage : **celui qui le porte entre** (*bearer*). On ne le prête pas !

### 🧩 Avec notre API

| Endpoint | Protection |
|---|---|
| `GET /api/produits`, `GET /api/produits/{id}`, `/api/ping`, `/api/demo/*` | **Public** |
| `GET /api/auth/moi`, `POST`, `PUT`, `PATCH /api/produits...` | **Token** (n'importe quel compte) — attribut `[Authorize]` |
| `DELETE /api/produits/{id}` | **Token + rôle Admin** — attribut `[Authorize(Roles = "Admin")]` |

| Compte | Mot de passe | Rôle |
|---|---|---|
| `admin` | `Admin123!` | Admin (peut supprimer) |
| `alice` | `Alice123!` | Utilisateur (ne peut pas supprimer) |

Contenu décodé d'un token de l'API (partie centrale du JWT) :

```json
{
  "aud": "FormationPostman",
  "iss": "ProduitsApi",
  "exp": 1793662966,
  "iat": 1793659366,
  "nbf": 1793659366,
  "sub": "admin",
  "name": "Administratrice de la formation",
  "role": "Admin",
  "jti": "066816ca-7f7e-42fe-8ca6-c852c941e032"
}
```

`iss` = émetteur, `aud` = destinataire, `iat` / `nbf` / `exp` = dates d'émission, de début et de fin de validité (en secondes depuis le 1er janvier 1970), `sub` = utilisateur, `jti` = identifiant unique du token.

### 🔧 Dans Postman

**A. Appeler un endpoint protégé sans token**
1. Ouvrir **01 - Authentification > Qui suis-je ? (sans token) → 401**. Onglet **Authorization** : *No Auth*.
2. **Send**. Lire le `detail` et, dans les headers de réponse, `WWW-Authenticate: Bearer`.

**B. Obtenir un token**
1. Ouvrir **01 > Obtenir un token (admin)**.
2. Onglet **Body** (raw, JSON) :

   ```json
   {
     "nomUtilisateur": "{{nomUtilisateur}}",
     "motDePasse": "{{motDePasse}}"
   }
   ```

   Les identifiants viennent des variables d'environnement.
3. Onglet **Authorization** : *No Auth* (on n'a pas encore de token !).
4. Onglet **Scripts > Post-response** : le script qui **enregistre le token** :

   ```javascript
   if (pm.response.code === 200) {
       pm.environment.set("token", reponse.accessToken);
   }
   ```

5. **Send**, puis cliquer sur l'icône **œil** : la variable `token` est maintenant remplie.

**C. Utiliser le token**
1. Cliquer sur le **nom de la collection**, onglet **Authorization** : *Auth Type* = **Bearer Token**, *Token* = **`{{token}}`**. Toutes les requêtes réglées sur *Inherit auth from parent* utilisent ce réglage.
2. Ouvrir **01 > Qui suis-je ? (avec token)**. Onglet **Authorization** : *Inherit auth from parent*.
3. **Send**. Ouvrir la **Console Postman** pour voir le header `Authorization: Bearer eyJ...` réellement envoyé.

**D. Tester l'autorisation (403)**
1. Envoyer **01 > Obtenir un token (alice, rôle Utilisateur)** : le token est enregistré dans `tokenUtilisateur`.
2. Créer une requête **DELETE** `{{baseUrl}}/api/produits/1`. Onglet **Authorization** : *Bearer Token*, *Token* = `{{tokenUtilisateur}}`.
3. **Send**. Le produit 1 n'est **pas** supprimé (vérifier avec `GET /api/produits/1`).

**E. Facultatif : décoder le token**
Copier la valeur du token (icône œil) et la coller sur https://jwt.io : le contenu s'affiche en clair.
> ⚠️ Ne jamais coller un token **de production** sur un site web.

### ✅ Résultat attendu

- A : `401 Unauthorized`, `"detail": "Aucun token fourni. Cet endpoint est protégé : ajoutez le header 'Authorization: Bearer <token>' (dans Postman : onglet Authorization > Bearer Token). Obtenez d'abord un token avec POST /api/auth/token."`
- B : `200 OK` :

  ```json
  {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOi...",
    "tokenType": "Bearer",
    "expiresIn": 3600,
    "expiration": "2026-11-03T10:42:46+01:00",
    "nomUtilisateur": "admin",
    "role": "Admin"
  }
  ```

- C : `200 OK`, `"nomUtilisateur": "admin"`, `"role": "Admin"` et la liste des claims.
- D : `403 Forbidden`, `"detail": "Vous êtes bien authentifié(e) (rôle : Utilisateur), mais ce rôle ne permet pas cette action. Par exemple, seul le rôle 'Admin' peut supprimer un produit."`

### ⚠️ Erreurs fréquentes

L'API renvoie un message adapté à chaque cas :

| Situation | Code | Message de l'API (champ `detail`, extrait) |
|---|---|---|
| Aucun token | 401 | « Aucun token fourni. Cet endpoint est protégé... » |
| Mot « Bearer » oublié (header écrit à la main) | 401 | « Le header Authorization doit commencer par le mot 'Bearer' suivi d'un espace... » |
| Environnement non sélectionné : `{{token}}` envoyé tel quel | 401 | « Le token reçu contient '{{' : la variable Postman n'a pas été remplacée... » |
| Variable `token` vide | 401 | « Le header Authorization contient 'Bearer' mais pas de token... » |
| Token expiré (après 60 minutes) | 401 | « Le token a expiré le 03/11/2026 à 10:42:46. Demandez un nouveau token... » |
| Token tronqué, modifié, entouré de guillemets | 401 | « Le token est invalide : il est incomplet, mal copié, modifié... » |
| Mauvais mot de passe | 401 | « Identifiants incorrects » |
| Rôle insuffisant (alice + DELETE) | 403 | « Vous êtes bien authentifié(e) (rôle : Utilisateur), mais ce rôle ne permet pas cette action... » |
| Token mis dans le body ou dans l'URL | 401 | Le token doit être dans le **header** `Authorization` |

### 🎤 Note pour le formateur

**Démonstration de l'expiration (facultative, 3 minutes).** Dans `appsettings.json`, passer `"DureeMinutes": 60` à `1`, redémarrer l'API, obtenir un token, attendre un peu plus d'une minute, puis appeler *Qui suis-je ? (avec token)* : **401** « Le token a expiré le ... ». Remettre `60` ensuite.

**Sécurité, à dire en une phrase :** en production, HTTPS obligatoire, clé secrète hors du code (coffre-fort de secrets), durée de vie courte, et jamais de token dans une capture d'écran partagée.

### 📌 À retenir

> On échange **une fois** ses identifiants contre un **token**, puis on l'envoie à **chaque** requête protégée : `Authorization: Bearer <token>`. **401** = qui êtes-vous ? **403** = vous n'avez pas le droit.

---

## Chapitre 11 — Variables et environnements Postman

### 📖 Théorie

Une **variable** est un nom, écrit **`{{nom}}`**, que Postman **remplace par sa valeur au moment de l'envoi**. On peut l'utiliser partout : URL, paramètres, headers, body, authentification.

Un **environnement** est un **ensemble nommé de variables**. On crée un environnement par cible : *Local*, *Recette*, *Production*. **Changer d'environnement suffit pour viser un autre serveur**, sans modifier une seule requête.

![Variables et environnements Postman](schemas/09-variables-postman.svg)

**Les portées**, de la plus large à la plus précise : **Globales** → **Collection** → **Environnement** → **Data** (fichier de données du Runner) → **Locales**. Si une variable du même nom existe à plusieurs niveaux, **la plus précise l'emporte** (une variable d'environnement masque une variable de collection du même nom).

**Valeur initiale et valeur courante :**
- **Initial value** : la valeur du fichier importé ; elle est partagée si l'environnement est synchronisé avec une équipe.
- **Current value** : la valeur **réellement utilisée** à l'envoi. C'est elle que les scripts modifient ; elle reste sur votre poste. Idéal pour un token.

**Types :** *default* (valeur visible) ou *secret* (valeur masquée à l'écran ; à utiliser pour les mots de passe et tokens dans un vrai projet).

**Variables dynamiques** fournies par Postman : `{{$guid}}` (identifiant unique), `{{$timestamp}}` (horodatage), `{{$randomInt}}` (entier aléatoire), `{{$randomFirstName}}`... Exemple : `"nom": "Produit {{$timestamp}}"` donne un nom différent à chaque envoi.

**Dans les scripts :**

```javascript
pm.environment.set("token", "abc");      // écrire dans l'environnement actif
pm.environment.get("token");             // lire
pm.collectionVariables.set("cle", "v");  // écrire une variable de collection
pm.variables.get("baseUrl");             // lire en tenant compte de toutes les portées
```

### 💡 Analogie : le répertoire du téléphone

On appelle « Maman » sans retaper son numéro. Si elle change de numéro, on modifie **une seule fiche** et tous les appels suivants sont corrects. Les environnements, ce sont **plusieurs répertoires** : un pour le bureau, un pour la maison.

### 🧩 Avec notre API

| Variable | Valeur | Remplie par |
|---|---|---|
| `baseUrl` | `http://localhost:5080` | le fichier d'environnement |
| `nomUtilisateur`, `motDePasse` | `admin`, `Admin123!` | le fichier d'environnement |
| `token` | `eyJ...` | le script de *Obtenir un token (admin)* |
| `tokenUtilisateur` | `eyJ...` | le script de *Obtenir un token (alice...)* |
| `produitId` | `11` | le script de *Créer un produit → 201* |

Chaîne automatique : **token** → requêtes protégées ; **produitId** → PUT, PATCH, DELETE du produit créé.

### 🔧 Dans Postman

**A. Explorer l'environnement**
1. Barre latérale > **Environments** > **Formation API REST - Local** : tableau *Variable / Type / Initial value / Current value*.
2. Dans une requête, **survoler** `{{baseUrl}}` : valeur et portée s'affichent.

**B. Constater l'intérêt de centraliser**
1. Dans l'environnement, changer la **valeur courante** de `baseUrl` en `http://localhost:5081` et enregistrer (**Ctrl+S**).
2. Renvoyer *Ping* : erreur de connexion (*ECONNREFUSED*). **Toutes** les requêtes visent maintenant ce port.
3. Remettre `http://localhost:5080` et enregistrer.

**C. Créer sa propre variable**
1. Dans l'environnement, ajouter la variable `categorieFavorite` avec la valeur `Audio`. Enregistrer.
2. Dans *Ma première liste*, utiliser l'URL `{{baseUrl}}/api/produits?categorie={{categorieFavorite}}`. **Send**.
3. Changer la valeur en `Bureau`, enregistrer, renvoyer.

**D. Utiliser une variable dynamique**
1. Dupliquer **03 > Créer un produit → 201** (clic droit > *Duplicate*).
2. Dans le body de la copie, remplacer le nom par `"nom": "Produit {{$timestamp}}"`.
3. Envoyer la copie trois fois.

### ✅ Résultat attendu

- B.2 : `Error: connect ECONNREFUSED 127.0.0.1:5081`.
- C.2 : les 2 produits Audio ; C.3 : les 3 produits Bureau.
- D.3 : **trois `201 Created`** avec des noms différents (« Produit 1793659366 »...), au lieu d'un `409 Conflict` dès le 2e envoi. Les tests de la copie qui vérifient le nom exact échouent : c'est normal, adaptez-les ou supprimez-les.

### ⚠️ Erreurs fréquentes

| Erreur | Conséquence | Solution |
|---|---|---|
| Aucun environnement sélectionné | `{{baseUrl}}` non remplacé : *ENOTFOUND* | Sélectionner l'environnement |
| Faute de frappe ou de casse : `{{BaseUrl}}` | Variable introuvable, surlignée en rouge | Les noms sont sensibles à la casse |
| Modifier la valeur **initiale** et s'étonner que rien ne change | C'est la valeur **courante** qui est utilisée | Modifier la valeur courante (ou *Reset* pour recopier l'initiale) |
| Même nom en collection et en environnement | L'environnement gagne | Éviter les doublons de noms |
| Mot de passe réel en valeur initiale d'un workspace d'équipe | Il est partagé avec toute l'équipe | Type *secret* + valeur courante uniquement |

### 📌 À retenir

> `{{variable}}` évite de répéter les valeurs ; l'**environnement** regroupe les valeurs d'une cible. Les **scripts** remplissent les variables automatiquement (token, identifiants).

---

# Partie D — Écrire des données

## Chapitre 12 — Le body, le format JSON et POST

### 📖 Théorie

**Le body** (corps, ou *payload*) contient **les données** envoyées avec la requête (ou renvoyées dans la réponse). On l'utilise surtout avec **POST, PUT et PATCH**.

Formats courants, indiqués par le header `Content-Type` :

| Format | `Content-Type` | Usage |
|---|---|---|
| **JSON** | `application/json` | La grande majorité des API REST |
| Formulaire | `application/x-www-form-urlencoded` | Formulaires web classiques |
| Multipart | `multipart/form-data` | Envoi de fichiers |
| XML | `application/xml` | API plus anciennes (SOAP...) |
| Texte | `text/plain` | Rare pour une API |

**Le format JSON** (*JavaScript Object Notation*) :

```json
{
  "nom": "Tapis de souris XXL",
  "description": "Tapis 90 x 40 cm, base antidérapante",
  "prix": 19.90,
  "categorie": "Informatique",
  "stock": 25,
  "enPromotion": false,
  "couleurs": ["noir", "gris"],
  "dimensions": { "longueur": 90, "largeur": 40 },
  "remarque": null
}
```

(Les 4 dernières lignes illustrent les types JSON ; notre API ne les utilise pas et les ignorerait.)

| Règle | ✅ Correct | ❌ Incorrect |
|---|---|---|
| Un objet est entre accolades | `{ ... }` | |
| Clés entre **guillemets doubles** | `"nom": "Tapis"` | `nom: "Tapis"`, `'nom': 'Tapis'` |
| Chaînes entre guillemets doubles **droits** | `"Tapis"` | `“Tapis”` ou `« Tapis »` (copié depuis Word) |
| Nombres **sans guillemets**, avec un **point** | `19.90` | `19,90` |
| Booléens et null en minuscules, sans guillemets | `true`, `false`, `null` | `True`, `"true"` |
| Virgule **entre** les éléments, **pas après le dernier** | `"stock": 25 }` | `"stock": 25, }` |
| Tableaux entre crochets | `["noir", "gris"]` | |
| Pas de commentaires | | `// prix TTC` |

**Règles de validation de notre API** (body de POST et PUT) :

| Champ | Obligatoire | Règle |
|---|---|---|
| `nom` | oui | 2 à 100 caractères, unique dans le catalogue |
| `description` | non | 500 caractères maximum |
| `prix` | oui | nombre entre 0.01 et 100000 |
| `categorie` | oui | 2 à 50 caractères |
| `stock` | oui | entier entre 0 et 10000 |

Un champ inconnu est **ignoré**. Un champ `id` envoyé dans le body est **ignoré** : c'est le serveur qui attribue l'identifiant.

**POST** crée une ressource. Réponse : **`201 Created`**, le produit créé (avec son `id`) dans le body, et le header **`Location`** qui donne l'URL du nouveau produit.

### 💡 Analogie : le formulaire papier

Chaque case du formulaire a une **étiquette** (la clé) et un **contenu** (la valeur). Le `Content-Type` indique **dans quelle langue** le formulaire est rempli. Si l'on remplit un formulaire français en japonais, l'administration le refuse (415) ; s'il manque une case obligatoire, elle le renvoie (400).

### 🧩 Avec notre API

```http
POST http://localhost:5080/api/produits
Content-Type: application/json
Authorization: Bearer eyJ...

{
  "nom": "Tapis de souris XXL",
  "description": "Tapis 90 x 40 cm, base antidérapante",
  "prix": 19.90,
  "categorie": "Informatique",
  "stock": 25
}
```

```http
HTTP/1.1 201 Created
Content-Type: application/json; charset=utf-8
Location: http://localhost:5080/api/produits/11

{
  "id": 11,
  "nom": "Tapis de souris XXL",
  "description": "Tapis 90 x 40 cm, base antidérapante",
  "prix": 19.90,
  "categorie": "Informatique",
  "stock": 25,
  "dateCreation": "2026-11-03T09:20:15.1234567+01:00",
  "dateModification": null
}
```

### 🔧 Dans Postman

**A. Avec la collection**
1. Vérifier que le token est obtenu (*01 > Obtenir un token (admin)*).
2. Envoyer **03 > Créer un produit → 201**. Observer le body, l'onglet **Headers** de la réponse (`Location`), et la variable `produitId` (icône œil).
3. Renvoyer la **même** requête : **409**.
4. Envoyer les requêtes d'erreur du dossier 03 : sans token (401), invalide (400), doublon (409), body au format texte (415), JSON mal formé (400).

**B. Créer une requête POST de zéro**
1. Dans *Mes requêtes* : **Add request**, nom **Créer mon produit**.
2. Méthode **POST**, URL `{{baseUrl}}/api/produits`.
3. Onglet **Authorization** : laisser *Inherit auth from parent*.
4. Onglet **Body** : cocher **raw**, puis choisir **JSON** dans la liste déroulante à droite (elle affiche *Text* par défaut). Postman ajoute alors automatiquement le header `Content-Type: application/json` (visible dans *Headers*, parmi les headers cachés).
5. Saisir :

   ```json
   {
     "nom": "Support pour ordinateur portable",
     "description": "Aluminium, hauteur réglable",
     "prix": 39.90,
     "categorie": "Bureau",
     "stock": 10
   }
   ```

6. **Send**, puis **Ctrl+S**.
7. Dans les headers de la réponse, copier l'URL de `Location`, la coller dans une nouvelle requête GET et l'envoyer.

> Astuce : le lien **Beautify** au-dessus de l'éditeur de body remet en forme un JSON mal indenté.

### ✅ Résultat attendu

- A.2 : `201 Created`, `"id": 11` (si les données venaient d'être réinitialisées), `Location: http://localhost:5080/api/produits/11`.
- A.3 : `409 Conflict`, `"detail": "Un produit nommé 'Tapis de souris XXL' existe déjà (identifiant 11). Choisissez un autre nom."`
- B.6 : `201 Created`, identifiant suivant (12 si A.2 a été fait juste avant).
- B.7 : `200 OK`, le même produit.

### ⚠️ Erreurs fréquentes

| Erreur dans Postman | Code | Message de l'API (extrait) |
|---|---|---|
| Body en **Text** au lieu de **JSON** | **415** | « Le format du body ('text/plain') n'est pas supporté. Dans Postman : onglet Body > raw > JSON... » |
| Virgule après le dernier champ | **400** | `errors.$` : « The JSON object contains a trailing comma... » |
| Clé sans guillemets : `{ nom: "x" }` | **400** | `errors.$` : « 'n' is an invalid start of a property name... » |
| Guillemets typographiques `“nom”` | **400** | `errors.$` : « '0xE2' is an invalid start of a property name... » |
| Virgule décimale : `"prix": 19,90` | **400** | `errors.$` : JSON invalide (le `90` est lu comme une nouvelle clé) |
| Champ obligatoire absent | **400** | `errors.categorie` : « Le champ 'categorie' est obligatoire. » |
| Valeur hors limites (`"prix": -5`) | **400** | `errors.prix` : « Le prix doit être compris entre 0.01 et 100000. » |
| Texte à la place d'un nombre (`"prix": "abc"`) | **400** | `errors.$.prix` : « The JSON value could not be converted... » |
| Body vide | **400** | « Le body de la requête est obligatoire (format JSON). » |
| Nom déjà utilisé | **409** | « Un produit nommé '...' existe déjà... » |
| Pas de token | **401** | « Aucun token fourni... » |
| Méthode restée sur **GET** | **200** | La liste est renvoyée ; le body est ignoré, **rien n'est créé** |

> Les messages techniques du champ `$` (syntaxe JSON) viennent du lecteur JSON de .NET et restent en anglais : le chemin indiqué (`Path: $ | LineNumber: 0 | BytePositionInLine: 24`) aide à trouver l'endroit de l'erreur.
>
> Notre API (comme toute API ASP.NET Core par défaut) accepte aussi un nombre écrit entre guillemets (`"prix": "19.90"`). Beaucoup d'autres API le refusent : prenez l'habitude d'écrire les nombres **sans guillemets**.

### 📌 À retenir

> Le body transporte les données ; en REST, c'est presque toujours du **JSON** avec `Content-Type: application/json`. Un **POST** réussi renvoie **201**, l'objet créé et le header **`Location`**.

---

## Chapitre 13 — PUT, PATCH et DELETE en pratique

### 📖 Théorie

![PUT ou PATCH ?](schemas/05-put-vs-patch.svg)

| | PUT | PATCH | DELETE |
|---|---|---|---|
| Rôle | **Remplacer entièrement** | **Modifier partiellement** | **Supprimer** |
| Body | La représentation **complète** | **Uniquement** les champs à changer | Aucun |
| Champ facultatif absent du body | **Effacé** (devient `null`) | **Inchangé** | — |
| Succès dans notre API | `200 OK` + produit | `200 OK` + produit | `204 No Content` |
| Ressource inexistante | `404` | `404` | `404` |
| Idempotent | oui | pas forcément (oui ici) | oui |

> **PUT peut-il créer ?** Certaines API créent la ressource si elle n'existe pas (PUT « upsert »). La nôtre répond 404 : on crée toujours avec POST.
>
> **Formats PATCH standard.** Notre API reçoit un JSON contenant les champs à modifier, proche du format *JSON Merge Patch* (RFC 7396). Il existe aussi *JSON Patch* (RFC 6902), qui décrit des opérations : `[{ "op": "replace", "path": "/prix", "value": 17.5 }]` avec `Content-Type: application/json-patch+json`. Notre API ne le gère pas (réponse 400). Simplification pédagogique : envoyer `"description": null` dans un PATCH ne vide pas la description (utiliser PUT pour cela).

### 💡 Analogie : la pièce à rénover

- **PUT** : on vide la pièce et on la réaménage entièrement selon le nouveau plan. Ce qui n'est pas sur le plan disparaît.
- **PATCH** : on change seulement les rideaux. Le reste ne bouge pas.
- **DELETE** : on condamne la pièce.

### 🧩 Avec notre API

Partons du produit créé au chapitre 12 (`produitId` = 11) : nom « Tapis de souris XXL », description « Tapis 90 x 40 cm... », prix 19.90, stock 25.

| Requête | Body | Résultat |
|---|---|---|
| `PUT /api/produits/11` | `{"nom":"Tapis de souris XXL Pro","prix":24.90,"categorie":"Informatique","stock":30}` | 200, **description = null** |
| `PATCH /api/produits/11` | `{"prix":17.50}` | 200, seul le prix change |
| `DELETE /api/produits/11` (alice) | — | **403** |
| `DELETE /api/produits/11` (admin) | — | **204**, body vide |
| `GET /api/produits/11` | — | **404** |

### 🔧 Dans Postman

Exécuter dans l'ordre (le produit doit avoir été créé : variable `produitId` remplie) :

1. **04 > Remplacer entièrement le produit créé → 200** : lire la réponse, `description` vaut `null`.
2. **04 > PUT incomplet → 400** : body `{"prix": 29.90}` seul.
3. **04 > PUT sans identifiant dans l'URL → 405** : regarder le header de réponse `Allow`.
4. **05 > Modifier uniquement le prix → 200** : nom et stock inchangés.
5. **05 > Mettre en rupture de stock → 200**, puis dans *Ma première liste* : `{{baseUrl}}/api/produits?enStock=false`.
6. **05 > PATCH sans champ reconnu → 400** : `{"prixx": 10}`.
7. **06 > Supprimer avec le compte alice → 403** (onglet Authorization : `{{tokenUtilisateur}}`).
8. **06 > Supprimer le produit créé → 204** : onglet Body vide.
9. **06 > Vérifier la suppression (GET) → 404**.
10. **06 > Supprimer à nouveau → 404**.

### ✅ Résultat attendu

- 1 : `200 OK`, `"nom": "Tapis de souris XXL Pro"`, `"description": null`, `"prix": 24.90`, `"stock": 30`, `dateModification` renseignée.
- 2 : `400`, `errors` contient `nom`, `categorie`, `stock`.
- 3 : `405`, `Allow: GET, POST`.
- 4 : `200`, `"prix": 17.50`, `"nom": "Tapis de souris XXL Pro"` (inchangé), `"stock": 30` (inchangé).
- 5 : `200`, `"stock": 0` ; la liste `enStock=false` contient le Casque audio Bluetooth, le Câble USB-C 2 m et le Tapis de souris XXL Pro.
- 6 : `400`, « Le body doit contenir au moins un champ à modifier parmi : nom, description, prix, categorie, stock... »
- 7 : `403 Forbidden`. 8 : `204 No Content`. 9 et 10 : `404 Not Found`.

### ⚠️ Erreurs fréquentes

| Erreur | Code | Explication |
|---|---|---|
| PUT avec un seul champ | 400 | PUT exige la représentation complète : utiliser PATCH |
| Oublier l'identifiant dans l'URL | 405 | `/api/produits` n'accepte que GET et POST |
| `{{produitId}}` vide (produit pas encore créé) | 405 | L'URL devient `/api/produits/` : créer le produit d'abord |
| Identifiant inexistant | 404 | Vérifier avec `GET /api/produits` |
| PATCH qui renomme avec le nom d'un autre produit | 409 | Les noms sont uniques |
| Faute dans le nom d'un champ du PATCH (`prixx`) | 400 | Le champ inconnu est ignoré : il ne reste rien à modifier |
| DELETE avec le token d'alice | 403 | Seul le rôle Admin peut supprimer |
| Croire qu'un 204 est une erreur parce que le body est vide | — | 204 = succès sans contenu |

### 📌 À retenir

> **PUT** : j'envoie **tout**, tout est remplacé. **PATCH** : j'envoie **la différence**. **DELETE** : `204`, puis `404` si on recommence.

---

# Partie E — Automatiser

## Chapitre 14 — Les tests automatisés dans Postman

### 📖 Théorie

Vérifier « à l'œil » chaque réponse devient vite fastidieux. Postman permet d'écrire de petits **scripts en JavaScript** qui s'exécutent automatiquement :

| Moment | Onglet | Usage |
|---|---|---|
| **Avant** l'envoi | *Scripts > Pre-request* | Préparer des données, calculer une variable |
| **Après** la réception | *Scripts > Post-response* | **Tester** la réponse, **extraire** des valeurs (token, id) |

Les scripts peuvent être placés sur la **collection**, un **dossier** ou une **requête**. Ordre d'exécution : collection → dossier → requête (pour chacun des deux moments). Notre collection a un script de collection *Pre-request* (qui écrit la requête envoyée dans la console) et un test de collection *Post-response* (temps de réponse inférieur à 2 secondes), en plus des tests propres à chaque requête.

**L'essentiel de l'API de script `pm` :**

```javascript
pm.test("Nom lisible du test", function () {
    // une ou plusieurs vérifications ; si l'une échoue, le test est rouge
});

pm.response.to.have.status(201);                    // code de statut
pm.response.to.have.header("Location");             // présence d'un header
pm.response.to.be.json;                             // le body est du JSON

const reponse = pm.response.json();                 // body JSON -> objet JavaScript
pm.expect(reponse.nom).to.eql("Tapis de souris XXL");      // égalité
pm.expect(reponse.id).to.be.a("number");                   // type
pm.expect(reponse.prix).to.be.below(100);                  // comparaison
pm.expect(reponse).to.have.property("categorie");          // propriété présente
pm.expect(reponse.errors).to.include.all.keys("nom", "prix");
pm.expect(pm.response.responseTime).to.be.below(2000);     // temps de réponse (ms)
pm.expect(pm.response.text()).to.be.empty;                 // body vide (204)

pm.environment.set("produitId", reponse.id);       // enregistrer une valeur
console.log("Produit créé :", reponse.id);          // écrire dans la Console Postman
```

**Le Collection Runner** exécute toutes les requêtes d'une collection (ou d'un dossier), dans l'ordre, et affiche un rapport des tests réussis et échoués. **Newman** est sa version en ligne de commande, utilisée dans les chaînes d'intégration continue (CI).

### 💡 Analogie : le contrôle qualité en sortie d'usine

Chaque produit qui sort de la chaîne (chaque réponse) passe devant une série de capteurs (les tests) : poids correct ? étiquette présente ? Un voyant **vert** ou **rouge** s'allume. Le Collection Runner, c'est faire passer **toute la production du jour** devant les capteurs en un clic.

### 🧩 Avec notre API

Les tests de **03 > Créer un produit → 201** :

```javascript
pm.test("Statut 201 Created", function () {
    pm.response.to.have.status(201);
});

pm.test("Le header Location donne l'URL du nouveau produit", function () {
    pm.response.to.have.header("Location");
});

const produit = pm.response.json();

pm.test("Le produit créé contient les valeurs envoyées", function () {
    pm.expect(produit.id).to.be.a("number");
    pm.expect(produit.nom).to.eql("Tapis de souris XXL");
    pm.expect(produit.prix).to.eql(19.9);
    pm.expect(produit.stock).to.eql(25);
});

// On mémorise l'identifiant pour les requêtes suivantes (PUT, PATCH, DELETE).
if (pm.response.code === 201) {
    pm.environment.set("produitId", produit.id);
}
```

### 🔧 Dans Postman

**A. Lire et exécuter des tests existants**
1. Ouvrir **00 > Réinitialiser les données de démo** et l'envoyer.
2. Ouvrir **03 > Créer un produit → 201**, onglet **Scripts > Post-response** : lire le code ci-dessus.
3. **Send**, puis onglet **Test Results** de la réponse.

**B. Écrire ses premiers tests**
1. Ouvrir *Mes requêtes > Ma première liste* (GET `{{baseUrl}}/api/produits`, sans paramètre).
2. Onglet **Scripts > Post-response**. Dans la liste **Snippets** (à droite ou en bas de l'éditeur selon la version), cliquer sur **Status code: Code is 200**. Le code est inséré.
3. Ajouter à la main :

   ```javascript
   pm.test("La liste contient au moins 5 produits", function () {
       pm.expect(pm.response.json().length).to.be.at.least(5);
   });

   pm.test("Le header X-Total-Count est présent", function () {
       pm.response.to.have.header("X-Total-Count");
   });
   ```

4. **Send** : onglet **Test Results**.
5. **Casser** volontairement un test : remplacer `5` par `500`, renvoyer, lire le message d'échec. Remettre `5`, enregistrer (**Ctrl+S**).

**C. Un script Pre-request (facultatif)**
Dans une copie de *Créer mon produit*, onglet **Scripts > Pre-request** :

```javascript
pm.variables.set("nomUnique", "Produit du " + new Date().toLocaleString("fr-FR"));
```

et dans le body : `"nom": "{{nomUnique}}"`. Chaque envoi crée un produit au nom différent.

**D. Lancer le Collection Runner**
1. Clic droit sur la collection (ou menu **⋯**) > **Run collection**.
2. Laisser toutes les requêtes cochées, dans l'ordre, 1 itération.
3. Cliquer sur **Run Formation API REST - Produits**.
4. Parcourir le rapport : chaque requête, son code, ses tests. Filtrer sur **Failed** s'il y a des échecs.

### ✅ Résultat attendu

- A.3 : **4 tests réussis** (3 tests de la requête + 1 test de collection).
- B.4 : 4 tests réussis (3 ajoutés + celui de la collection).
- B.5 : un test rouge : `AssertionError: expected 10 to be at least 500` (le nombre exact dépend des produits présents).
- D.4 : **37 requêtes exécutées, 110 tests réussis, 0 échec** (la requête *Réinitialiser les données de démo* du dossier 00 remet les données à zéro en début d'exécution, ce qui rend le Runner répétable). Vos requêtes du dossier *Mes requêtes* s'ajoutent si elles sont cochées.

### ⚠️ Erreurs fréquentes

| Erreur | Conséquence | Solution |
|---|---|---|
| Tests écrits dans *Pre-request* | `pm.response` n'existe pas encore : erreur de script | Les mettre dans *Post-response* |
| `pm.response.json()` sur une réponse 204 (vide) | Erreur `JSONError: No data, empty input` | Utiliser `pm.response.text()` |
| Test trop dépendant des données (« exactement 10 produits ») | Il échoue dès qu'un produit est ajouté | Préférer des vérifications robustes (« au moins », « chaque produit a un id ») |
| Runner lancé sans le token | Toutes les requêtes protégées en 401 | Garder la requête *Obtenir un token* au début |
| Ordre des requêtes modifié dans le Runner | `produitId` vide, tests rouges | Respecter l'ordre création → modification → suppression |
| Erreur de syntaxe JavaScript (parenthèse oubliée) | Message « There was an error in evaluating the test script » | Lire la ligne indiquée ; s'aider des snippets |

### 📌 À retenir

> Un test = `pm.test("nom", function () { pm.expect(...)... })` dans **Scripts > Post-response**. Le **Collection Runner** rejoue toute la collection et affiche un rapport vert / rouge.

---

# Conclusion

## Les 10 points à retenir

1. Une **API** est un contrat qui permet à un logiciel de parler à un autre.
2. **REST** : des ressources (URL avec des noms) + des méthodes HTTP (les verbes) + des codes standard.
3. Requête = **méthode + URL + headers + body** ; réponse = **code + headers + body**.
4. **GET** lit, **POST** crée, **PUT** remplace, **PATCH** modifie, **DELETE** supprime.
5. **Paramètre de route** = quelle ressource ; **query parameters** = filtrer, trier, paginer.
6. **Headers** essentiels : `Content-Type`, `Accept`, `Authorization`.
7. **Body** en **JSON** : guillemets doubles, point décimal, pas de virgule finale.
8. **2xx** succès, **4xx** erreur du client, **5xx** erreur du serveur ; lire le body des erreurs.
9. **Bearer Token** : obtenu une fois, envoyé à chaque requête ; **401** ≠ **403**.
10. Postman : **environnements et variables** pour ne rien répéter, **tests** et **Runner** pour tout vérifier en un clic.

## Pour aller plus loin

| Sujet | Piste |
|---|---|
| S'entraîner sur d'autres API | API publiques gratuites : https://jsonplaceholder.typicode.com (fausses données, accepte POST/PUT/DELETE), https://pokeapi.co (lecture seule) |
| Documentation d'API | **OpenAPI / Swagger** : notre API publie sa description sur http://localhost:5080/openapi/v1.json. Dans Postman : **Import** > coller ce lien → Postman génère une collection |
| Générer du code | Dans Postman, icône **`</>`** (*Code snippet*) dans la barre de droite : la requête traduite en cURL, C# (HttpClient), JavaScript, Python... |
| Authentification avancée | **OAuth 2.0** et **OpenID Connect** (connexion avec Google, Microsoft Entra ID...) : Postman sait obtenir ces tokens dans l'onglet Authorization |
| Tests avancés | Runner avec **fichier de données CSV** (une itération par ligne), **Newman** dans une chaîne CI (GitHub Actions, Azure DevOps) |
| Postman en équipe | Workspaces partagés, **mock servers** (simuler une API qui n'existe pas encore), **monitors** (tests planifiés) |
| Ligne de commande | `curl`, l'outil universel : `curl -X POST http://localhost:5080/api/auth/token -H "Content-Type: application/json" -d "{\"nomUtilisateur\":\"admin\",\"motDePasse\":\"Admin123!\"}"` |
