// =====================================================================================
//  API Produits - formation "Postman et API REST" (ASP.NET Core .NET 8 - version de secours)
//
//  Point d'entrée de l'application. Ce fichier fait deux choses :
//    1. il ENREGISTRE les services (builder.Services...) : contrôleurs, authentification, erreurs...
//    2. il CONSTRUIT le pipeline HTTP (app.Use...) : la suite d'étapes traversées par chaque requête.
//
//  Adresse locale : http://localhost:5080  (définie dans Properties/launchSettings.json)
// =====================================================================================

using System.Diagnostics;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.IdentityModel.Tokens;
using ProduitsApi.Erreurs;
using ProduitsApi.Securite;
using ProduitsApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Logs lisibles dans la console : une ligne par message, précédée de l'heure.
builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
    options.TimestampFormat = "HH:mm:ss ";
});

// -------------------------------------------------------------------------------------
// 1. SERVICES
// -------------------------------------------------------------------------------------

// Stockage des produits en mémoire : une seule instance partagée (Singleton).
builder.Services.AddSingleton<ProduitStore>();

// Contrôleurs + options JSON.
builder.Services.AddControllers(options =>
    {
        // Messages d'erreur de lecture des paramètres, en français.
        var messages = options.ModelBindingMessageProvider;
        messages.SetAttemptedValueIsInvalidAccessor((valeur, champ) => $"La valeur '{valeur}' n'est pas valide pour '{CamelCase(champ)}'.");
        messages.SetValueMustBeANumberAccessor(champ => $"Le champ '{CamelCase(champ)}' doit être un nombre.");
        messages.SetValueIsInvalidAccessor(valeur => $"La valeur '{valeur}' n'est pas valide.");
        messages.SetMissingRequestBodyRequiredValueAccessor(() => "Le body de la requête est obligatoire (format JSON).");
        messages.SetNonPropertyAttemptedValueIsInvalidAccessor(valeur => $"La valeur '{valeur}' n'est pas valide.");

        // Les noms de champs dans les erreurs suivent les noms JSON (ex. "prix" au lieu de "Prix").
        options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());

        // Les paramètres non nullables ne sont pas implicitement obligatoires : les règles sont écrites
        // explicitement avec [Required] sur les modèles, avec des messages en français.
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    })
    .AddJsonOptions(options => ConfigurerJson(options.JsonSerializerOptions))
    .ConfigureApiBehaviorOptions(options =>
    {
        // Réponse 400 automatique quand la validation échoue : titre et explication en français.
        options.InvalidModelStateResponseFactory = contexte =>
        {
            var fabrique = contexte.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
            var probleme = fabrique.CreateValidationProblemDetails(
                contexte.HttpContext,
                contexte.ModelState,
                statusCode: StatusCodes.Status400BadRequest,
                title: "La requête contient des erreurs",
                detail: "Corrigez les éléments listés dans 'errors' puis renvoyez la requête.");

            return new BadRequestObjectResult(probleme) { ContentTypes = { "application/problem+json" } };
        };
    });

// Mêmes options JSON pour les réponses écrites hors contrôleurs (erreurs 401, 404, 500...).
builder.Services.ConfigureHttpJsonOptions(options => ConfigurerJson(options.SerializerOptions));

// Format d'erreur standard "ProblemDetails" (RFC 9457) + traduction en français.
builder.Services.AddProblemDetails(options => options.CustomizeProblemDetails = ProblemDetailsEnFrancais.Personnaliser);
builder.Services.AddExceptionHandler<GestionnaireErreursGlobal>();

// ---- Authentification par Bearer Token (JWT) ----------------------------------------
var sectionJwt = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<ParametresJwt>(sectionJwt);
var parametresJwt = sectionJwt.Get<ParametresJwt>() ?? new ParametresJwt();

if (parametresJwt.CleSecrete.Length < 32)
{
    throw new InvalidOperationException("La clé 'Jwt:CleSecrete' de appsettings.json doit contenir au moins 32 caractères.");
}

builder.Services.AddSingleton<ServiceToken>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Conserver les noms de claims courts du JWT ("sub", "role"...) sans les renommer.
        options.MapInboundClaims = false;

        // Règles de vérification appliquées à CHAQUE token reçu.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,                 // le token a-t-il été émis par notre API ?
            ValidIssuer = parametresJwt.Emetteur,
            ValidateAudience = true,               // est-il destiné à notre API ?
            ValidAudience = parametresJwt.Audience,
            ValidateLifetime = true,               // est-il encore valide (non expiré) ?
            ClockSkew = TimeSpan.FromSeconds(5),   // tolérance d'horloge (5 min par défaut, réduite pour la démo)
            ValidateIssuerSigningKey = true,       // la signature est-elle correcte (token non falsifié) ?
            IssuerSigningKey = ServiceToken.CreerCleDeSignature(parametresJwt.CleSecrete),
            NameClaimType = "name",
            RoleClaimType = "role"                 // claim utilisé par [Authorize(Roles = "Admin")]
        };

        // Réponses 401 / 403 explicites, en français (voir Securite/EvenementsJwt.cs).
        options.Events = EvenementsJwt.Creer();
    });

builder.Services.AddAuthorization();

// -------------------------------------------------------------------------------------
// 2. PIPELINE HTTP : chaque requête traverse ces étapes, dans cet ordre.
// -------------------------------------------------------------------------------------
var app = builder.Build();

// a) Journal des requêtes : une ligne par requête dans la console de l'API.
//    Pratique pendant la formation pour montrer que Postman "parle" bien au serveur.
var journal = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("HTTP");
app.Use(async (contexte, suite) =>
{
    var chrono = Stopwatch.StartNew();
    await suite(contexte);
    chrono.Stop();

    var avecToken = contexte.Request.Headers.ContainsKey("Authorization") ? " [avec token]" : string.Empty;
    journal.LogInformation("{Methode} {Url} -> {Statut} ({Duree} ms){Token}",
        contexte.Request.Method,
        $"{contexte.Request.Path}{contexte.Request.QueryString}",
        contexte.Response.StatusCode,
        chrono.ElapsedMilliseconds,
        avecToken);
});

// b) Exceptions non gérées => 500 au format JSON (voir Erreurs/GestionnaireErreursGlobal.cs).
app.UseExceptionHandler();

// c) Réponses d'erreur sans body (route inconnue 404, méthode non autorisée 405...) => body JSON explicatif.
app.UseStatusCodePages();

// Remarque : pas de app.UseHttpsRedirection() pour rester en HTTP simple pendant la formation locale.
// En production, une API doit TOUJOURS être servie en HTTPS (sinon le token circule en clair).

// d) Authentification (qui êtes-vous ? lecture du token) puis autorisation (avez-vous le droit ?).
app.UseAuthentication();
app.UseAuthorization();

// e) Endpoints.
app.MapControllers();   // routes définies par les attributs [Route], [HttpGet]... des contrôleurs

// Message d'aide affiché au démarrage dans la console.
app.Lifetime.ApplicationStarted.Register(() =>
{
    var adresses = string.Join(", ", app.Urls);
    app.Logger.LogInformation(
        "API Produits démarrée sur {Adresses}. Comptes : admin / Admin123! (Admin) ; alice / Alice123! (Utilisateur). " +
        "Arrêt : Ctrl+C.", adresses);
});

app.Run();

// Options JSON communes : indentation (réponses lisibles) et caractères écrits tels quels
// (accents et apostrophes non échappés en \uXXXX). Ce mode "relaxed" convient à une API qui renvoie du JSON ;
// il ne faut pas l'utiliser pour du JSON inséré directement dans une page HTML.
static void ConfigurerJson(System.Text.Json.JsonSerializerOptions options)
{
    options.WriteIndented = true;
    options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
}

// "EnStock" => "enStock" : affiche les noms de champs tels qu'ils s'écrivent dans l'URL ou le JSON.
static string CamelCase(string nom) =>
    string.IsNullOrEmpty(nom) ? nom : char.ToLowerInvariant(nom[0]) + nom[1..];
