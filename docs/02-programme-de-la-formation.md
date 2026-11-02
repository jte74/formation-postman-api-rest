# 2. Programme de la formation

**Intitulé :** Découvrir les API REST et maîtriser l'essentiel de Postman
**Format :** individuel, en présentiel ou à distance, **essentiellement pratique**
**Durée :** **1 h 30** (programme principal), avec une **variante courte de 60 minutes** et des **extensions facultatives** de 15 à 30 minutes
**Public :** débutant. Aucun prérequis en programmation. Savoir utiliser un ordinateur et un navigateur suffit.
**Matériel :** un poste avec l'API de démonstration lancée (`http://localhost:5080`) et Postman configuré (voir [01-installation-et-lancement.md](01-installation-et-lancement.md)). Idéalement un second écran : support de cours ou console de l'API d'un côté, Postman de l'autre.

---

## 2.1 Objectifs pédagogiques

À la fin de la séance, l'apprenant est capable de :

1. **Expliquer** avec ses mots ce qu'est une API et ce qu'est une API REST.
2. **Décrire** une requête HTTP (méthode, URL, headers, body) et une réponse HTTP (code, headers, body).
3. **Choisir** la bonne méthode HTTP (GET, POST, PUT, PATCH, DELETE) pour une action donnée.
4. **Distinguer** un paramètre de route d'un query parameter.
5. **Rédiger** un body JSON valide.
6. **Interpréter** les principaux codes de réponse (200, 201, 204, 400, 401, 403, 404, 409, 415, 500) et savoir qui doit corriger.
7. **Utiliser** Postman : collections, requêtes, environnements, variables, authentification.
8. **S'authentifier** avec un Bearer Token et expliquer la différence entre 401 et 403.
9. **Écrire** des tests simples dans Postman et exécuter une collection avec le Collection Runner.

---

## 2.2 Vue d'ensemble

| # | Horaire | Durée | Séquence | Chapitres du support | Exercices | Variante 60 min |
|---|---|---|---|---|---|---|
| 1 | 0:00 – 0:05 | 5 min | Accueil, objectifs, tour de table | — | — | 3 min |
| 2 | 0:05 – 0:20 | 15 min | Les bases : API, REST, HTTP, cycle requête/réponse | 1 à 4 | — | 10 min |
| 3 | 0:20 – 0:30 | 10 min | Prise en main de Postman | 5 | Ex. 1 | 7 min |
| 4 | 0:30 – 0:45 | 15 min | Lire des données : GET, paramètres, headers, codes | 6 à 9 | Ex. 2, 3, 4 | 10 min (Ex. 3 seul) |
| 5 | 0:45 – 0:55 | 10 min | Authentification Bearer et variables | 10 et 11 | Ex. 5 | 8 min |
| 6 | 0:55 – 1:10 | 15 min | Écrire : body JSON, POST, PUT, PATCH, DELETE | 12 et 13 | Ex. 6, 7, 8 | 12 min (Ex. 6 et 8) |
| 7 | 1:10 – 1:20 | 10 min | Tests automatisés et Collection Runner | 14 | Ex. 9 | 5 min (démo seule) |
| 8 | 1:20 – 1:30 | 10 min | Exercice récapitulatif, quiz, bilan | Conclusion | Récap | 5 min (quiz seul) |
| | | **90 min** | | | | **60 min** |

**Principe de rythme :** jamais plus de 5 minutes de théorie d'affilée. Chaque notion est **montrée par le formateur**, puis **refaite par l'apprenant** sur son clavier.

**Extensions facultatives (+15 à +30 min)** : exercice 10 (diagnostiquer des requêtes cassées), import de la description OpenAPI dans Postman, génération de code (*Code snippet*), exploration du JWT sur https://jwt.io, point d'arrêt dans Visual Studio pendant l'envoi d'une requête.

---

## 2.3 Déroulé détaillé

### Séquence 1 — Accueil (0:00 – 0:05)

| | |
|---|---|
| **Objectif** | Mettre l'apprenant à l'aise, cadrer la séance, vérifier le matériel. |
| **Le formateur dit** | « Aujourd'hui, vous allez parler à une API comme le font les applications. À la fin, vous saurez créer, lire, modifier et supprimer des données avec Postman, et vérifier automatiquement que tout fonctionne. » |
| **Questions à poser** | « Avez-vous déjà entendu parler d'API ? Dans quel contexte ? Qu'attendez-vous de la séance ? » |
| **Vérifications** | API lancée (console visible), Postman ouvert et connecté, collection et environnement importés, environnement sélectionné. |
| **Support** | Le schéma 01 affiché en fond d'écran. |

### Séquence 2 — Les bases (0:05 – 0:20)

| | |
|---|---|
| **Objectif** | Comprendre ce qu'est une API, REST et HTTP avant de manipuler. |
| **Notions** | API (contrat entre deux logiciels), client / serveur, ressource, URL, méthode, JSON, HTTP sans état, requête / réponse. |
| **Démonstrations** | 1. Schéma 01 et l'analogie du restaurant. 2. Ouvrir http://localhost:5080/api/ping **dans le navigateur** : « Le navigateur vient d'envoyer une requête GET. » 3. Schéma 03 : anatomie d'une requête et d'une réponse. 4. Schéma 02 : montrer la console de l'API qui affiche la requête reçue. |
| **Manipulation apprenant** | Ouvrir http://localhost:5080/ et http://localhost:5080/api/produits dans le navigateur et lire le JSON. |
| **Vérification des acquis** | « Dans l'analogie du restaurant, qui est l'API ? » « Que contient une réponse HTTP ? » |
| **Chapitres** | 1, 2, 3, 4 |

> **Transition :** « Le navigateur ne sait envoyer facilement que des GET. Pour créer, modifier ou supprimer, et pour ajouter un token, il nous faut un vrai outil : Postman. »

### Séquence 3 — Prise en main de Postman (0:20 – 0:30)

| | |
|---|---|
| **Objectif** | Se repérer dans Postman et envoyer une première requête. |
| **Notions** | Workspace, collection, dossier, requête, environnement, variable `{{baseUrl}}`, zone de réponse (code, temps, taille, body, headers, résultats des tests). |
| **Démonstrations** | Tour de l'interface ; montrer la collection importée ; sélectionner l'environnement ; survoler `{{baseUrl}}` dans l'URL ; envoyer *Ping* ; montrer Pretty / Raw ; onglet Headers de la réponse ; Console Postman. |
| **Manipulation apprenant** | **Exercice 1** : envoyer *Ping*, relever le code, le temps, la taille et le `Content-Type` ; créer sa propre requête GET `{{baseUrl}}/api/produits` et l'enregistrer dans la collection. |
| **Vérification des acquis** | « Que se passe-t-il si aucun environnement n'est sélectionné ? » (le faire : erreur `ENOTFOUND {{baseUrl}}`). |
| **Chapitre** | 5 |

### Séquence 4 — Lire des données (0:30 – 0:45)

| | |
|---|---|
| **Objectif** | Maîtriser GET, les paramètres d'URL, les headers et les codes de réponse. |
| **Notions** | Les 5 méthodes (vue d'ensemble, schéma 04), GET, paramètre de route `/api/produits/3`, query parameters `?categorie=Audio&tri=prix`, header de requête / de réponse, `X-Total-Count`, familles de codes, 200 / 400 / 404. |
| **Démonstrations** | Dossier *02 - Lire des produits* : liste, par id (Path Variable `:id`), filtres, pagination, 404, 400. Requête *Voir les headers envoyés par Postman*. Schémas 07 et 08. |
| **Manipulations apprenant** | **Exercice 2** (produit par id, 404), **exercice 3** (filtrer, trier), **exercice 4** (headers). |
| **Vérification des acquis** | « Pour obtenir le produit 7 : route ou query ? » « Pour les produits à moins de 50 € ? » « Une liste vide est-elle une erreur ? » (non : 200 et `[]`). |
| **Chapitres** | 6, 7, 8, 9 |

### Séquence 5 — Authentification et variables (0:45 – 0:55)

| | |
|---|---|
| **Objectif** | Comprendre et utiliser un Bearer Token ; automatiser sa récupération avec une variable. |
| **Notions** | Endpoint public / protégé, authentification vs autorisation, token, JWT, header `Authorization: Bearer ...`, expiration, 401 vs 403, héritage de l'authentification dans Postman, portées des variables, valeur initiale / courante. |
| **Démonstrations** | Schéma 06. Appeler *Qui suis-je ? (sans token)* → 401. Envoyer *Obtenir un token (admin)* ; montrer le script **Post-response** et la variable `token` qui se remplit (icône œil). *Qui suis-je ? (avec token)* → 200. Onglet Authorization de la collection : *Bearer Token* = `{{token}}`. Facultatif : coller le token sur https://jwt.io. Schéma 09. |
| **Manipulation apprenant** | **Exercice 5** : obtenir un token pour alice, appeler `/api/auth/moi`, provoquer les erreurs classiques (mot « Bearer » oublié, mauvais mot de passe). |
| **Vérification des acquis** | « 401 ou 403 : lequel signifie "je ne sais pas qui vous êtes" ? » « Pourquoi ne jamais mettre de secret dans un JWT ? » |
| **Chapitres** | 10, 11 |

### Séquence 6 — Écrire des données (0:55 – 1:10)

| | |
|---|---|
| **Objectif** | Créer, modifier et supprimer une ressource ; savoir lire les erreurs. |
| **Notions** | Body, JSON (syntaxe, types), `Content-Type: application/json`, validation (400), 201 + `Location`, conflit (409), mauvais format (415), PUT vs PATCH (schéma 05), 204, idempotence, 403 pour alice. |
| **Démonstrations** | Dossiers 03 à 06 dans l'ordre : créer (201), créer sans token (401), body invalide (400), doublon (409), body en texte (415), JSON mal formé (400), PUT (description effacée), PUT incomplet (400), PUT sans id (405), PATCH (seul le prix change), DELETE alice (403), DELETE admin (204), GET après suppression (404). |
| **Manipulations apprenant** | **Exercice 6** (POST), **exercice 7** (PUT vs PATCH), **exercice 8** (DELETE et droits). |
| **Vérification des acquis** | « Je veux seulement changer le stock : PUT ou PATCH ? » « Pourquoi un DELETE réussi n'a-t-il pas de body ? » « Que signifie 415 ? » |
| **Chapitres** | 12, 13 |

### Séquence 7 — Tests automatisés (1:10 – 1:20)

| | |
|---|---|
| **Objectif** | Écrire des tests Postman et exécuter toute une collection. |
| **Notions** | Onglet *Scripts* (Pre-request / Post-response), `pm.test`, `pm.expect`, `pm.response`, snippets, tests de collection, Collection Runner, rapport vert / rouge. |
| **Démonstrations** | Lire les tests de *Créer un produit → 201* ; utiliser un snippet *Status code: Code is 200* ; casser volontairement un test (attendre 200 au lieu de 201) pour voir le rouge ; lancer le **Collection Runner** sur toute la collection (37 requêtes, tout vert). |
| **Manipulation apprenant** | **Exercice 9** : ajouter 3 tests à une requête et l'exécuter dans le Runner. |
| **Vérification des acquis** | « Quelle est la différence entre un script Pre-request et Post-response ? » |
| **Chapitre** | 14 |

### Séquence 8 — Récapitulatif et bilan (1:20 – 1:30)

| | |
|---|---|
| **Objectif** | Vérifier l'autonomie de l'apprenant sur un scénario complet. |
| **Activité** | **Exercice récapitulatif** « Le cycle de vie d'un produit » (en autonomie, le formateur observe sans intervenir), puis **quiz de 10 questions** à l'oral. |
| **Bilan** | Ce qui a été appris, ce qui reste flou, suite possible (API publiques à explorer, documentation OpenAPI, authentification OAuth 2.0, tests avancés). Remettre la **fiche récapitulative** ([06-fiche-recapitulative.md](06-fiche-recapitulative.md)). |

---

## 2.4 Conseils d'animation

- **Faire faire plutôt que montrer.** Après chaque démonstration, rendre le clavier : « À vous, sur le produit 5. »
- **Provoquer les erreurs.** Les erreurs volontaires (401, 404, 415...) sont les moments où l'on apprend le plus. L'API renvoie des messages explicites en français : faire lire le champ `detail` à voix haute.
- **Montrer le serveur.** La console de l'API affiche chaque requête reçue (`POST /api/produits -> 201`). Elle matérialise le « dialogue ».
- **Une seule source de vérité.** En cas de doute, réinitialiser les données (*00 - Démarrage > Réinitialiser les données de démo*).
- **Agrandir l'affichage** pour un partage d'écran : Postman *View > Zoom In* (**Ctrl + +**), police de la console à 16 ou plus.
- **Laisser des traces.** L'apprenant enregistre ses requêtes (**Ctrl+S**) dans un dossier personnel *Mes requêtes* de la collection : il repart avec son travail (export : clic droit sur la collection > **Export**).

## 2.5 Évaluation

| Moment | Méthode | Critère de réussite |
|---|---|---|
| Pendant la séance | Questions de vérification à chaque séquence | Réponse juste avec ses mots |
| Exercices 1 à 9 | Observation | Résultat attendu obtenu, l'apprenant sait expliquer pourquoi |
| Exercice récapitulatif | Autonomie | 8 étapes réalisées, tests verts dans le Runner |
| Quiz final | 10 questions | 7 bonnes réponses ou plus |
