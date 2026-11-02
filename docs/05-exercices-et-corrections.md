# 5. Exercices pratiques et corrections

Exercices **progressifs**, à réaliser **dans Postman** par l'apprenant, dans l'ordre du programme. Pour chacun : objectif, étapes, requête, résultat attendu et **correction détaillée** (à déplier : cliquer sur « Correction »).

**Avant de commencer :**
- l'API tourne sur `http://localhost:5080` ;
- l'environnement **Formation API REST - Local** est sélectionné ;
- l'apprenant range ses requêtes dans un dossier **Mes requêtes** de la collection (clic droit sur la collection > *Add folder*) ;
- les résultats ci-dessous supposent des **données réinitialisées** : *00 - Démarrage > Réinitialiser les données de démo* (ou redémarrage de l'API).

| # | Exercice | Notions | Séquence | Durée |
|---|---|---|---|---|
| 1 | Première requête | Postman, lecture d'une réponse | 3 | 5 min |
| 2 | Un produit précis | GET, paramètre de route, 404 | 4 | 3 min |
| 3 | Filtrer, trier, paginer | Query parameters | 4 | 6 min |
| 4 | Les headers | Headers de requête et de réponse | 4 | 4 min |
| 5 | S'authentifier | Bearer Token, variables, 401 | 5 | 6 min |
| 6 | Créer un produit | POST, body JSON, 201, 400, 409, 415 | 6 | 6 min |
| 7 | PUT ou PATCH ? | PUT, PATCH | 6 | 4 min |
| 8 | Supprimer et droits | DELETE, 204, 403, 404 | 6 | 3 min |
| 9 | Écrire des tests | Scripts, `pm.test`, Runner | 7 | 8 min |
| 10 | *Bonus* : diagnostiquer des requêtes cassées | Toutes | extension | 10 min |
| R | **Exercice récapitulatif** | Toutes | 8 | 10 min |
| Q | **Quiz** (10 questions) | Toutes | 8 | 5 min |

---

## Exercice 1 — Première requête

**🎯 Objectif :** envoyer une requête avec Postman et savoir lire toutes les informations d'une réponse.

**Étapes :**
1. Ouvrir *00 - Démarrage > Ping - l'API répond-elle ?* et cliquer sur **Send**.
2. Relever : le code de statut, le temps de réponse, la taille, la valeur du header de réponse `Content-Type`, le nombre de tests réussis.
3. Créer dans *Mes requêtes* une requête **Ma première liste** : GET `{{baseUrl}}/api/produits`. L'envoyer et l'enregistrer.
4. Combien de produits contient la réponse ?

**Requêtes :**
```http
GET {{baseUrl}}/api/ping
GET {{baseUrl}}/api/produits
```

**Résultat attendu :** `200 OK` pour les deux ; 10 produits.

<details>
<summary><b>Correction</b></summary>

1. *Ping* répond **`200 OK`** avec le body :
   ```json
   { "message": "pong", "serveur": "API Produits (ASP.NET Core .NET 10)", "heureServeur": "..." }
   ```
2. Les informations se lisent en haut à droite de la zone de réponse :
   - code : **200 OK** (en vert) ;
   - temps : quelques millisecondes (la toute première requête après le démarrage de l'API peut prendre 100 à 300 ms, le temps que .NET « chauffe ») ;
   - taille : environ 300 octets (*B*) ;
   - onglet **Headers** de la réponse : `Content-Type: application/json; charset=utf-8` ;
   - onglet **Test Results** : **4/4** (3 tests de la requête + 1 test de collection sur le temps de réponse).
3. Clic droit sur *Mes requêtes* > **Add request**, nom *Ma première liste*, méthode **GET**, URL `{{baseUrl}}/api/produits`, **Send**, **Ctrl+S**.
4. **10 produits** (identifiants 1 à 10). Le header de réponse `X-Total-Count` vaut aussi `10`.

**Points à faire verbaliser :** « le code dit si ça a marché, le body contient les données, les headers donnent des informations sur la réponse ».
</details>

---

## Exercice 2 — Un produit précis

**🎯 Objectif :** utiliser un paramètre de route et reconnaître une erreur 404.

**Étapes :**
1. Afficher le produit **5**.
2. Trouver le prix du produit **8**.
3. Demander le produit **42**.
4. Demander le produit **`cinq`**.

**Requête :** `GET {{baseUrl}}/api/produits/{id}`

**Résultat attendu :** 200 pour 5 et 8 ; 404 pour 42 et `cinq`, avec des messages différents.

<details>
<summary><b>Correction</b></summary>

Utiliser *02 > Obtenir un produit par son id (paramètre de route)* et changer la *Path Variable* `id` dans l'onglet **Params**, ou écrire l'URL directement.

| Requête | Code | Réponse |
|---|---|---|
| `GET {{baseUrl}}/api/produits/5` | 200 | `"nom": "Enceinte portable"`, `"prix": 49.90`, `"stock": 20` |
| `GET {{baseUrl}}/api/produits/8` | 200 | `"nom": "Carnet de notes A5"`, **`"prix": 6.50`** |
| `GET {{baseUrl}}/api/produits/42` | 404 | `"title": "Produit introuvable"`, `"detail": "Aucun produit ne porte l'identifiant 42..."` |
| `GET {{baseUrl}}/api/produits/cinq` | 404 | `"title": "Ressource introuvable"` |

**Pourquoi deux 404 différents ?**
- `42` : la route `/api/produits/{id}` existe, le contrôleur a cherché le produit 42 et ne l'a pas trouvé → message **« Produit introuvable »** écrit par notre code.
- `cinq` : la route attend un **entier** (`{id:int}`) ; aucune route ne correspond, la requête n'atteint même pas le contrôleur → message générique **« Ressource introuvable »**.
</details>

---

## Exercice 3 — Filtrer, trier, paginer

**🎯 Objectif :** combiner plusieurs query parameters.

**Questions (une requête par question) :**
1. Quels sont les produits de la catégorie **Bureau** ?
2. Quels produits **Bureau** coûtent **50 € maximum**, du **moins cher au plus cher** ?
3. Quels produits sont **en rupture de stock** ?
4. Quel est **le produit le plus cher** du catalogue ? (La réponse ne doit contenir **qu'un seul** produit.)
5. Quels produits contiennent le texte **« sans fil »** ?

**Requête de base :** `GET {{baseUrl}}/api/produits?...`

**Résultat attendu :** voir la correction ; pour chaque question, noter aussi la valeur du header `X-Total-Count`.

<details>
<summary><b>Correction</b></summary>

| # | URL | Résultat | `X-Total-Count` |
|---|---|---|---|
| 1 | `{{baseUrl}}/api/produits?categorie=Bureau` | Lampe de bureau LED, Chaise ergonomique, Carnet de notes A5 | 3 |
| 2 | `{{baseUrl}}/api/produits?categorie=Bureau&prixMax=50&tri=prix&ordre=asc` | Carnet de notes A5 (6.50), Lampe de bureau LED (34.90) | 2 |
| 3 | `{{baseUrl}}/api/produits?enStock=false` | Casque audio Bluetooth, Câble USB-C 2 m | 2 |
| 4 | `{{baseUrl}}/api/produits?tri=prix&ordre=desc&taille=1` | Écran 27 pouces (249.00) | 10 |
| 5 | `{{baseUrl}}/api/produits?recherche=sans fil` | Souris sans fil | 1 |

**Explications :**
- Q2 : `ordre=asc` est facultatif (c'est la valeur par défaut). La Chaise ergonomique (199.00) est exclue par `prixMax=50`.
- Q4 : on trie par prix décroissant et on ne garde qu'**un** résultat par page (`taille=1`). `X-Total-Count` vaut 10 : c'est le total **avant** pagination.
- Q5 : dans l'onglet **Params**, saisir la valeur `sans fil` avec l'espace ; Postman l'encode en `sans%20fil` à l'envoi.

**Méthode conseillée :** remplir le tableau **Query Params** plutôt que taper l'URL : moins de fautes de `?` et de `&`.

**Erreurs typiques observées :** `?categorie=Bureau?prixMax=50` (deux `?` → liste vide), `?categori=Bureau` (paramètre ignoré → 10 produits), `prixMax=50,00` (→ 400, il faut un point).
</details>

---

## Exercice 4 — Les headers

**🎯 Objectif :** observer les headers envoyés et reçus, et en ajouter un.

**Étapes :**
1. Envoyer *00 > Voir les headers envoyés par Postman*. Quelle est la valeur du header `User-Agent` reçu par le serveur ?
2. Ajouter un header `X-Apprenant` avec votre prénom. Renvoyer et vérifier que le serveur l'a reçu.
3. Envoyer *02 > Filtrer par catégorie (query param)*. Dans les headers de **réponse**, quelles sont les valeurs de `Content-Type` et de `X-Total-Count` ?
4. Question : quelle est la différence entre `Content-Type` et `Accept` ?

**Résultat attendu :** `User-Agent` commence par `PostmanRuntime/` ; `X-Apprenant` apparaît dans `headersRecus` ; `X-Total-Count: 2`.

<details>
<summary><b>Correction</b></summary>

1. Dans le body de la réponse, `headersRecus["User-Agent"]` vaut **`PostmanRuntime/7.x.x`** (le numéro dépend de la version de Postman). C'est un header ajouté **automatiquement** par Postman : il est visible en cliquant sur **« hidden »** dans l'onglet *Headers* de la requête.
2. Onglet **Headers** de la requête : nouvelle ligne, *Key* = `X-Apprenant`, *Value* = `Camille` (par exemple). **Send** : la réponse contient `"X-Apprenant": "Camille"`.
3. Onglet **Headers** de la réponse : `Content-Type: application/json; charset=utf-8` et **`X-Total-Count: 2`** (Casque audio Bluetooth et Enceinte portable).
4. **`Content-Type`** décrit le format du **body envoyé** (requête) ou **renvoyé** (réponse). **`Accept`** est envoyé par le client pour dire **quel format il souhaite recevoir**.
</details>

---

## Exercice 5 — S'authentifier

**🎯 Objectif :** obtenir un token, l'enregistrer dans une variable, l'utiliser, et reconnaître les erreurs 401.

**Étapes :**
1. Créer une requête **Token alice** : POST `{{baseUrl}}/api/auth/token`, authentification *No Auth*, body JSON avec les identifiants d'alice (`alice` / `Alice123!`).
2. Ajouter un script *Post-response* qui enregistre le token reçu dans la variable d'environnement `tokenUtilisateur`.
3. Créer une requête **Qui suis-je (alice)** : GET `{{baseUrl}}/api/auth/moi`, authentification **Bearer Token** = `{{tokenUtilisateur}}`.
4. Provoquer trois erreurs et lire le message de l'API :
   - a) mot de passe `alice123!` (minuscule) ;
   - b) dans *Qui suis-je (alice)* : authentification *No Auth*, puis header manuel `Authorization` = `{{tokenUtilisateur}}` (sans le mot Bearer) ;
   - c) authentification *Bearer Token* = `{{tokenAlice}}` (variable qui n'existe pas).

**Résultat attendu :** étape 3 : `200 OK`, `"role": "Utilisateur"` ; étape 4 : trois **401** avec trois messages différents.

<details>
<summary><b>Correction</b></summary>

**Étape 1** — onglet *Body* > *raw* > *JSON* :
```json
{
  "nomUtilisateur": "alice",
  "motDePasse": "Alice123!"
}
```
Onglet *Authorization* : **No Auth**. Réponse : `200 OK`, `"role": "Utilisateur"`, `"expiresIn": 3600`.

**Étape 2** — onglet *Scripts > Post-response* :
```javascript
const reponse = pm.response.json();

pm.test("Token reçu", function () {
    pm.response.to.have.status(200);
    pm.expect(reponse.accessToken).to.be.a("string");
});

pm.environment.set("tokenUtilisateur", reponse.accessToken);
```
Vérification : icône **œil** → `tokenUtilisateur` contient `eyJ...`.

**Étape 3** — onglet *Authorization* : *Auth Type* = **Bearer Token**, *Token* = `{{tokenUtilisateur}}`. Réponse :
```json
{
  "nomUtilisateur": "alice",
  "nomComplet": "Alice Martin",
  "role": "Utilisateur",
  ...
}
```

**Étape 4 :**

| Cas | Code | `detail` (extrait) | Explication |
|---|---|---|---|
| a) `alice123!` | 401 | « Le nom d'utilisateur ou le mot de passe est incorrect (le mot de passe est sensible aux majuscules). » | Le mot de passe est sensible à la casse |
| b) sans « Bearer » | 401 | « Le header Authorization doit commencer par le mot 'Bearer' suivi d'un espace puis du token... » | Format attendu : `Bearer <token>` |
| c) `{{tokenAlice}}` | 401 | « Le token reçu contient '{{' : la variable Postman n'a pas été remplacée... » | Variable inexistante : Postman envoie le texte `{{tokenAlice}}` tel quel (il le surligne en rouge) |

Remettre ensuite *Bearer Token* = `{{tokenUtilisateur}}` sur la requête.
</details>

---

## Exercice 6 — Créer un produit

**🎯 Objectif :** envoyer un body JSON avec POST et interpréter 201, 409, 400 et 415.

**Prérequis :** token admin obtenu (*01 > Obtenir un token (admin)*), données réinitialisées.

**Étapes :**
1. Créer une requête **Créer mon produit** : POST `{{baseUrl}}/api/produits` (authentification héritée), qui crée le produit suivant :
   - nom : Support pour ordinateur portable
   - description : Aluminium, hauteur réglable
   - prix : 39,90 €
   - catégorie : Bureau
   - stock : 10
2. Noter le code, l'identifiant attribué et le header `Location`.
3. Renvoyer exactement la même requête.
4. Dans une copie de la requête, supprimer la ligne `categorie` et changer le nom en « Repose-pieds ». Envoyer.
5. Dans une autre copie, passer le body de **JSON** à **Text**. Envoyer.
6. Ouvrir l'URL du header `Location` avec une requête GET.

**Résultat attendu :** 201 (id 11), puis 409, 400, 415, et enfin 200.

<details>
<summary><b>Correction</b></summary>

**Étape 1** — *Body* > **raw** > **JSON** (attention : le prix s'écrit avec un **point**) :
```json
{
  "nom": "Support pour ordinateur portable",
  "description": "Aluminium, hauteur réglable",
  "prix": 39.90,
  "categorie": "Bureau",
  "stock": 10
}
```

**Étape 2** — `201 Created`, `"id": 11`, header `Location: http://localhost:5080/api/produits/11`. Le body contient le produit complet avec `dateCreation` renseignée et `dateModification: null`.

**Étape 3** — `409 Conflict` : « Un produit nommé 'Support pour ordinateur portable' existe déjà (identifiant 11). Choisissez un autre nom. » POST n'est **pas idempotent** : sans cette règle d'unicité, on aurait créé un doublon.

**Étape 4** — `400 Bad Request` :
```json
"errors": { "categorie": [ "Le champ 'categorie' est obligatoire." ] }
```

**Étape 5** — `415 Unsupported Media Type` : « Le format du body ('text/plain') n'est pas supporté. Dans Postman : onglet Body > raw > JSON... » Le JSON est pourtant identique : c'est le header **`Content-Type`** qui a changé.

**Étape 6** — `GET {{baseUrl}}/api/produits/11` → `200 OK`, le produit créé.
</details>

---

## Exercice 7 — PUT ou PATCH ?

**🎯 Objectif :** constater concrètement la différence entre PUT et PATCH.

**Prérequis :** le produit 11 de l'exercice 6 existe.

**Étapes :**
1. Le stock du produit 11 passe à **5**. Faire la modification **sans renvoyer les autres champs**.
2. Vérifier que la description est toujours « Aluminium, hauteur réglable ».
3. Envoyer un **PUT** sur le produit 11 avec : nom inchangé, prix **44.90**, catégorie Bureau, stock 5, **sans description**.
4. Que vaut maintenant la description ? Pourquoi ?
5. Envoyer un PUT contenant **seulement** `{"prix": 49.90}`. Que se passe-t-il ?

**Résultat attendu :** PATCH conserve la description ; PUT l'efface ; PUT incomplet → 400.

<details>
<summary><b>Correction</b></summary>

**Étape 1** — `PATCH {{baseUrl}}/api/produits/11`, body :
```json
{ "stock": 5 }
```
→ `200 OK`, `"stock": 5`.

**Étape 2** — Dans la même réponse : `"description": "Aluminium, hauteur réglable"`, `"prix": 39.90` : **inchangés**.

**Étape 3** — `PUT {{baseUrl}}/api/produits/11`, body :
```json
{
  "nom": "Support pour ordinateur portable",
  "prix": 44.90,
  "categorie": "Bureau",
  "stock": 5
}
```
→ `200 OK`.

**Étape 4** — `"description": null`. **PUT remplace la représentation complète** : un champ absent du body est effacé.

**Étape 5** — `400 Bad Request` : `errors` signale `nom`, `categorie` et `stock` manquants. Pour ne changer que le prix, il faut **PATCH**.
</details>

---

## Exercice 8 — Supprimer et droits

**🎯 Objectif :** supprimer une ressource, distinguer 401, 403 et 404, comprendre 204.

**Étapes :**
1. Supprimer le produit 11 **avec le token d'alice**.
2. Supprimer le produit 11 avec le token **admin**.
3. Relire le produit 11.
4. Le supprimer à nouveau.
5. Question : DELETE est-il idempotent ?

**Résultat attendu :** 403, 204, 404, 404.

<details>
<summary><b>Correction</b></summary>

| Étape | Requête | Authorization | Code | Explication |
|---|---|---|---|---|
| 1 | `DELETE {{baseUrl}}/api/produits/11` | Bearer `{{tokenUtilisateur}}` | **403** | alice est authentifiée, mais son rôle (Utilisateur) ne permet pas de supprimer |
| 2 | `DELETE {{baseUrl}}/api/produits/11` | héritée (`{{token}}` admin) | **204** | Supprimé ; **pas de body** (onglet Body vide) |
| 3 | `GET {{baseUrl}}/api/produits/11` | — | **404** | Le produit n'existe plus |
| 4 | `DELETE {{baseUrl}}/api/produits/11` | héritée | **404** | Rien à supprimer |

**Question 5** — Oui : après 1 ou 10 suppressions, l'**état du serveur** est le même (le produit n'existe pas). Le **code** de réponse change (204 puis 404), mais l'idempotence porte sur l'état, pas sur le code.

**À faire remarquer :** si l'on avait envoyé l'étape 1 **sans aucun token**, la réponse aurait été **401** (et non 403).
</details>

---

## Exercice 9 — Écrire des tests

**🎯 Objectif :** automatiser la vérification des réponses et exécuter un dossier avec le Collection Runner.

**Prérequis :** token admin valide, données réinitialisées.

**Étapes :**
1. Sur **Créer mon produit** (exercice 6), ajouter dans *Scripts > Post-response* des tests qui vérifient :
   - le code **201** ;
   - la présence du header **Location** ;
   - que le **nom** renvoyé est celui envoyé ;
   - que le **prix** est un **nombre** ;
   - puis enregistrer l'`id` dans une variable d'environnement **`monProduitId`**.
2. Créer une requête **Supprimer mon produit** : DELETE `{{baseUrl}}/api/produits/{{monProduitId}}`, avec un test sur le code **204**.
3. Vérifier que *Mes requêtes* contient, dans cet ordre, *Créer mon produit* puis *Supprimer mon produit* (glisser-déposer pour réordonner).
4. Lancer le **Runner** sur le dossier *Mes requêtes* (clic droit sur le dossier > *Run folder*), en ne cochant que ces deux requêtes.
5. Lancer le Runner une deuxième fois. Pourquoi est-ce toujours vert ?

**Résultat attendu :** tous les tests verts, deux fois de suite.

<details>
<summary><b>Correction</b></summary>

**Étape 1** — *Créer mon produit*, onglet *Scripts > Post-response* :
```javascript
pm.test("Statut 201 Created", function () {
    pm.response.to.have.status(201);
});

pm.test("Header Location présent", function () {
    pm.response.to.have.header("Location");
});

const produit = pm.response.json();

pm.test("Le nom renvoyé est celui envoyé", function () {
    pm.expect(produit.nom).to.eql("Support pour ordinateur portable");
});

pm.test("Le prix est un nombre", function () {
    pm.expect(produit.prix).to.be.a("number");
});

if (pm.response.code === 201) {
    pm.environment.set("monProduitId", produit.id);
}
```

> Variante plus robuste pour le nom : comparer à ce qui a été envoyé plutôt qu'à une valeur écrite en dur :
> `pm.expect(produit.nom).to.eql(JSON.parse(pm.request.body.raw).nom);`

**Étape 2** — *Supprimer mon produit*, onglet *Scripts > Post-response* :
```javascript
pm.test("Statut 204 No Content", function () {
    pm.response.to.have.status(204);
});
```
(Ne pas appeler `pm.response.json()` ici : le body d'un 204 est vide.)

**Étape 4** — Rapport : 2 requêtes, **7 tests réussis** : 4 tests sur la création + 1 sur la suppression, plus le test de collection (temps de réponse) exécuté pour chacune des 2 requêtes.

**Étape 5** — Toujours vert, car le scénario **remet le serveur dans son état initial** : le produit créé est supprimé à la fin. Un bon scénario de test est **rejouable**. Si la suppression avait échoué, le 2e passage aurait renvoyé un **409** à la création.

**Si tout est rouge en 401** : le token a expiré (60 minutes). Renvoyer *01 > Obtenir un token (admin)* puis relancer.
</details>

---

## Exercice 10 (bonus) — Diagnostiquer des requêtes cassées

**🎯 Objectif :** devenir autonome face aux erreurs. Le formateur prépare (ou décrit) chaque requête ; l'apprenant trouve la cause **en lisant le code et le body de la réponse**, puis corrige.

| # | Requête préparée | Code obtenu |
|---|---|---|
| 1 | `GET {{baseUrl}}/api/produit` | ? |
| 2 | `POST {{baseUrl}}/api/produits`, body `{"nom":"Lampe","prix":12.50,"categorie":"Bureau","stock":3}` en **Text** | ? |
| 3 | `PUT {{baseUrl}}/api/produits`, body complet | ? |
| 4 | `POST {{baseUrl}}/api/produits`, body JSON `{"nom":"Lampe","prix":12,50,"categorie":"Bureau","stock":3}` | ? |
| 5 | `GET {{baseUrl}}/api/auth/moi`, header manuel `Authorization: eyJhbGciOi...` | ? |
| 6 | `GET {{baseUrl}}/api/produits?categorie=Audio&enStock=oui` | ? |
| 7 | `DELETE {{baseUrl}}/api/produits/3` avec `{{tokenUtilisateur}}` | ? |
| 8 | N'importe quelle requête, **aucun environnement sélectionné** | ? |

<details>
<summary><b>Correction</b></summary>

| # | Code | Cause | Correction |
|---|---|---|---|
| 1 | **404** « Ressource introuvable » | Faute dans l'URL : il manque le « s » | `/api/produits` |
| 2 | **415** | Body en *Text* → `Content-Type: text/plain` | *Body > raw > JSON* |
| 3 | **405**, header `Allow: GET, POST` | Identifiant oublié dans l'URL | `/api/produits/{id}` |
| 4 | **400**, `errors.$` | JSON invalide : virgule décimale `12,50` | `12.50` |
| 5 | **401** « doit commencer par le mot 'Bearer' » | Mot « Bearer » oublié | Onglet *Authorization* > *Bearer Token* |
| 6 | **400** « La valeur 'oui' n'est pas valide pour 'enStock' » | Booléen attendu | `enStock=true` |
| 7 | **403** | alice n'a pas le rôle Admin | Utiliser le token admin |
| 8 | Pas de code : `Error: getaddrinfo ENOTFOUND {{baseUrl}}` | `{{baseUrl}}` non remplacé | Sélectionner l'environnement |

**Méthode à retenir :** 1) lire le **code** ; 2) lire le **`title`** et le **`detail`** ; 3) pour un 400, lire **`errors`** ; 4) pas de code du tout = problème de connexion ou de variable.
</details>

---

## Exercice récapitulatif — Le cycle de vie d'un produit

**🎯 Objectif :** vérifier, **en autonomie**, que l'apprenant maîtrise les notions du cours : méthodes, paramètres, body JSON, authentification, variables, codes de réponse et tests.

**Consigne donnée à l'apprenant :**

> Le magasin référence un nouveau produit, le met en avant, baisse son prix, puis l'arrête. Créez un dossier **Récapitulatif** contenant **8 requêtes**, chacune avec **au moins un test**, puis exécutez le dossier avec le **Collection Runner** : tout doit être vert.
>
> 1. Réinitialiser les données de démonstration.
> 2. Obtenir un token admin et l'enregistrer automatiquement.
> 3. Lister les produits **Audio en stock**, triés par **prix décroissant**.
> 4. Créer le produit **« Barre de son »** (Audio, 129.90 €, stock 6, description « Bluetooth, 120 W ») et enregistrer son identifiant dans la variable `recapId`.
> 5. Le prix baisse à **99.90 €** : le modifier **sans renvoyer les autres champs**.
> 6. Relister les produits Audio en stock, par prix décroissant : la barre de son doit apparaître **en premier**.
> 7. Le produit est arrêté : le supprimer.
> 8. Vérifier qu'il n'existe plus.

**Critères de réussite :** 8 requêtes correctes, les bonnes méthodes, des variables (pas d'URL ni de token écrits en dur), au moins un test par requête, Runner entièrement vert.

<details>
<summary><b>Correction</b></summary>

**1. Réinitialiser** — `POST {{baseUrl}}/api/demo/reinitialiser` (No Auth)
```javascript
pm.test("Données réinitialisées", function () {
    pm.response.to.have.status(200);
    pm.expect(pm.response.json().nombreProduits).to.eql(10);
});
```

**2. Token admin** — `POST {{baseUrl}}/api/auth/token` (No Auth), body JSON :
```json
{ "nomUtilisateur": "{{nomUtilisateur}}", "motDePasse": "{{motDePasse}}" }
```
```javascript
pm.test("Token obtenu", function () {
    pm.response.to.have.status(200);
});
pm.environment.set("token", pm.response.json().accessToken);
```

**3. Lister Audio en stock** — `GET {{baseUrl}}/api/produits?categorie=Audio&enStock=true&tri=prix&ordre=desc`
Résultat : **un seul produit**, l'Enceinte portable (49.90) — le Casque audio Bluetooth est en rupture (stock 0).
```javascript
pm.test("Uniquement des produits Audio en stock", function () {
    pm.response.to.have.status(200);
    pm.response.json().forEach(function (p) {
        pm.expect(p.categorie).to.eql("Audio");
        pm.expect(p.stock).to.be.above(0);
    });
});
```

**4. Créer** — `POST {{baseUrl}}/api/produits` (authentification héritée : `{{token}}`), body JSON :
```json
{
  "nom": "Barre de son",
  "description": "Bluetooth, 120 W",
  "prix": 129.90,
  "categorie": "Audio",
  "stock": 6
}
```
Résultat : `201 Created`, `"id": 11`.
```javascript
pm.test("Produit créé", function () {
    pm.response.to.have.status(201);
    pm.response.to.have.header("Location");
});
pm.environment.set("recapId", pm.response.json().id);
```

**5. Baisser le prix** — `PATCH {{baseUrl}}/api/produits/{{recapId}}`, body JSON :
```json
{ "prix": 99.90 }
```
```javascript
pm.test("Prix modifié, reste inchangé", function () {
    pm.response.to.have.status(200);
    const p = pm.response.json();
    pm.expect(p.prix).to.eql(99.9);
    pm.expect(p.description).to.eql("Bluetooth, 120 W");
});
```
(Un PUT avec seulement le prix aurait renvoyé 400 ; un PUT complet sans description l'aurait effacée.)

**6. Relister** — même requête qu'à l'étape 3. Résultat : Barre de son (99.90), puis Enceinte portable (49.90).
```javascript
pm.test("La barre de son est en tête", function () {
    pm.response.to.have.status(200);
    const liste = pm.response.json();
    pm.expect(liste[0].nom).to.eql("Barre de son");
    pm.expect(liste.length).to.eql(2);
});
```

**7. Supprimer** — `DELETE {{baseUrl}}/api/produits/{{recapId}}` (token admin hérité)
```javascript
pm.test("Produit supprimé", function () {
    pm.response.to.have.status(204);
});
```

**8. Vérifier** — `GET {{baseUrl}}/api/produits/{{recapId}}`
```javascript
pm.test("Le produit n'existe plus", function () {
    pm.response.to.have.status(404);
});
```

**Runner** — clic droit sur *Récapitulatif* > *Run folder* > **Run** : 8 requêtes, tous les tests verts. Relancé une 2e fois : toujours vert (l'étape 1 réinitialise, l'étape 7 supprime).

**Grille d'observation :**

| Critère | Acquis | À revoir |
|---|---|---|
| Choisit la bonne méthode à chaque étape (POST, GET, PATCH, DELETE) | | |
| Utilise des query parameters correctement (`?` puis `&`) | | |
| Écrit un JSON valide (guillemets, point décimal) | | |
| Configure l'authentification (héritage, No Auth sur le token) | | |
| Utilise des variables (`{{baseUrl}}`, `{{token}}`, `{{recapId}}`) | | |
| Lit et interprète les codes (201, 204, 404) | | |
| Écrit des tests et utilise le Runner | | |
</details>

---

## Quiz final (10 questions)

À poser à l'oral à la fin de la séance. Objectif : **7 bonnes réponses ou plus**.

1. En une phrase, qu'est-ce qu'une API ?
2. Je veux changer **uniquement** le prix d'un produit : quelle méthode ?
3. Quelle est la différence entre **401** et **403** ?
4. Dans `/api/produits/7?champ=nom`, qu'est-ce qui est un paramètre de route et qu'est-ce qui est un query parameter ?
5. Que renvoie un **POST** réussi (code, header, body) ?
6. Trouvez les deux erreurs dans ce JSON : `{ 'nom': 'Lampe', "prix": 12,5 }`
7. Quel header indique le format du body envoyé ?
8. Je reçois une **500** : dois-je corriger ma requête ?
9. Pourquoi écrire `{{baseUrl}}` plutôt que `http://localhost:5080` ?
10. Où écrit-on un test dans Postman, et avec quelle fonction ?

<details>
<summary><b>Réponses</b></summary>

1. Un **contrat / une porte d'entrée** qui permet à un logiciel de demander des données ou des actions à un autre, sans connaître son fonctionnement interne.
2. **PATCH** (avec par exemple `{"prix": 17.50}`).
3. **401** : non authentifié (pas de token, token invalide ou expiré) — « qui êtes-vous ? ». **403** : authentifié mais **pas autorisé** (rôle insuffisant).
4. `7` est un **paramètre de route** (il identifie le produit) ; `champ=nom` est un **query parameter** (après le `?`).
5. **201 Created**, le header **`Location`** (URL de la nouvelle ressource) et, dans le body, la ressource créée avec son **identifiant**.
6. Les **guillemets simples** (`'nom'`, `'Lampe'`) : JSON exige des guillemets doubles. La **virgule décimale** (`12,5`) : il faut un point, `12.5`.
7. **`Content-Type`** (par exemple `application/json`).
8. **Non** : 5xx = erreur **du serveur**. On signale le problème à l'équipe de l'API (avec le `traceId`).
9. Pour changer d'adresse (recette, production) **en un seul endroit**, en changeant d'environnement, sans modifier les requêtes.
10. Dans l'onglet **Scripts > Post-response**, avec **`pm.test("nom", function () { ... })`** et `pm.expect(...)` / `pm.response.to.have.status(...)`.
</details>
