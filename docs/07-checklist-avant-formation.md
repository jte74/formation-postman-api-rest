# 7. Checklist avant la formation

À cocher **la veille** (J-1, avec une connexion Internet), puis **30 minutes avant** le début de la séance.

---

## J-1 — Installation et vérification complète (environ 30 minutes)

### Poste et logiciels

- [ ] **SDK .NET 10** installé : `dotnet --list-sdks` affiche une ligne `10.0.x`.
- [ ] **Visual Studio 2026** (ou Visual Studio 2022 **17.14+**, *Aide > À propos*) installé avec la charge de travail **Développement web et ASP.NET** (ou VS Code + C# Dev Kit, ou Rider).
- [ ] **Postman** (application de bureau) installé et à jour.
- [ ] **Compte Postman** créé ; connexion testée (Postman l'exige au démarrage).
- [ ] Si l'apprenant utilise son propre poste : mêmes vérifications sur son poste (ou prévoir de travailler sur le poste du formateur).

### Projet

- [ ] Kit récupéré : `git clone https://github.com/jte74/formation-postman-api-rest.git` (ou téléchargement ZIP depuis GitHub : *Code > Download ZIP*).
- [ ] `FormationPostman.sln` ouvert dans Visual Studio.
- [ ] **Premier build réussi** (*Générer > Générer la solution*) : il télécharge les 2 paquets NuGet, **une connexion Internet est nécessaire cette fois-ci**. Ensuite, les paquets sont en cache : la formation peut se faire hors ligne (sauf la connexion à Postman).
- [ ] Profil de lancement **`http`** sélectionné ; **F5** lance l'API ; la console affiche `Now listening on: http://localhost:5080`.
- [ ] Le navigateur affiche le JSON de bienvenue sur http://localhost:5080/.
- [ ] Le port **5080** est libre (sinon : modifier `Properties/launchSettings.json` **et** `baseUrl` dans l'environnement Postman).

### Postman

- [ ] Collection importée : *Formation API REST - Produits* (8 dossiers, 37 requêtes).
- [ ] Environnement importé : *Formation API REST - Local*.
- [ ] Environnement **sélectionné** en haut à droite.
- [ ] *00 - Démarrage > Ping* → `200 OK`, 4 tests sur 4 réussis.
- [ ] *01 - Authentification > Obtenir un token (admin)* → `200 OK` ; la variable `token` est remplie (icône œil).
- [ ] **Collection Runner** sur toute la collection → **37 requêtes, 110 tests réussis, 0 échec**.
- [ ] (Facultatif) Vérification en ligne de commande, API lancée : `npx newman run postman/Formation-API-REST.postman_collection.json -e postman/Formation-API-REST-Local.postman_environment.json`.

### Supports

- [ ] [02-programme-de-la-formation.md](02-programme-de-la-formation.md) relu (timing, séquences).
- [ ] [03-support-de-cours.md](03-support-de-cours.md) parcouru ; les 9 schémas s'affichent.
- [ ] [05-exercices-et-corrections.md](05-exercices-et-corrections.md) : exercice récapitulatif fait une fois soi-même.
- [ ] [06-fiche-recapitulative.md](06-fiche-recapitulative.md) imprimée ou envoyée à l'apprenant.
- [ ] (Facultatif) Démonstration du point d'arrêt testée une fois dans Visual Studio (§ 1.3 de l'installation).

---

## Jour J — 30 minutes avant

### Démarrage

- [ ] API lancée (**F5** ou **Ctrl+F5**), console visible.
- [ ] *00 - Démarrage > Réinitialiser les données de démo* envoyée → `nombreProduits: 10`.
- [ ] *Ping* → `200 OK`.
- [ ] Token obtenu (*Obtenir un token (admin)*) — il est valable **60 minutes** : le renouveler à la séquence 5 si besoin.
- [ ] Variables `produitId` et `tokenUtilisateur` : vides ou anciennes, sans importance (les requêtes les remplissent).
- [ ] Dossier **Mes requêtes** de la séance précédente supprimé (ou renommé) si le poste a déjà servi.

### Affichage et confort

- [ ] Postman agrandi pour la lisibilité : **Ctrl + +** (2 ou 3 fois) en partage d'écran ou projection.
- [ ] Police de la console de l'API agrandie (clic droit sur la barre de titre > *Propriétés* > *Police*).
- [ ] Disposition : Postman à gauche, console de l'API (ou support de cours) à droite ; deuxième écran si possible.
- [ ] Notifications (messagerie, e-mails) coupées ; mode « Ne pas déranger » activé.
- [ ] Onglets de navigateur ouverts : http://localhost:5080/, le support de cours, les schémas.
- [ ] Si la formation est à distance : partage de l'écran **entier** testé (Postman et console visibles), et l'apprenant a installé Postman + le kit ou a accès au poste.

---

## Plan B — si quelque chose ne fonctionne pas

| Problème le jour J | Solution de repli |
|---|---|
| Visual Studio ne démarre pas / ne compile pas | `cd src/ProduitsApi` puis `dotnet run` dans un terminal |
| `NETSDK1045` (pas de SDK .NET 10) et impossible d'installer quoi que ce soit | Branche **`net8`** du dépôt : `git checkout net8` (ou *Code > Download ZIP* sur cette branche). Même API, même collection, en .NET 8 |
| Port 5080 occupé | `dotnet run --urls http://localhost:5090`, puis `baseUrl` = `http://localhost:5090` dans l'environnement |
| Pas d'Internet pour la connexion Postman | Postman garde la session ouverte s'il a été connecté la veille ; sinon, utiliser le fichier `src/ProduitsApi/ProduitsApi.http` de Visual Studio (mêmes notions : méthode, URL, headers, body) |
| La collection a été modifiée par erreur | La réimporter depuis `postman/` (choisir *Replace* ou *Import as copy*) |
| Les données sont dans un état incohérent | *Réinitialiser les données de démo*, ou redémarrer l'API |
| Tout est en 401 | Le token a expiré : renvoyer *Obtenir un token (admin)* |
| L'apprenant est en avance | Exercice 10 (diagnostic), import OpenAPI (`http://localhost:5080/openapi/v1.json`), génération de code (`</>`), jwt.io |
| L'apprenant est en retard | Version 60 minutes du programme : passer les exercices 2, 4, 7 et la démonstration Pre-request |

---

## Après la formation

- [ ] Inviter l'apprenant à **exporter** sa collection (clic droit > *Export*) pour garder ses requêtes.
- [ ] Lui envoyer la fiche récapitulative et le lien du dépôt.
- [ ] Noter ce qui a pris plus de temps que prévu pour ajuster le programme.
- [ ] Arrêter l'API (**Maj+F5** dans Visual Studio, ou **Ctrl+C** dans la console).
