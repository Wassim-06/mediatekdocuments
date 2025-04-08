using System;
using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Linq;
using System.Diagnostics;

namespace MediaTekDocuments.dal
{
    /// <summary>
    /// Classe d'accès aux données
    /// </summary>
    public class Access
    {
        /// <summary>
        /// instance unique de la classe
        /// </summary>
        private static Access instance = null;
        /// <summary>
        /// instance de ApiRest pour envoyer des demandes vers l'api et recevoir la réponse
        /// </summary>
        private readonly ApiRest api = null;
        /// <summary>
        /// méthode HTTP pour select
        /// </summary>
        private const string GET = "GET";
        /// <summary>
        /// méthode HTTP pour insert
        /// </summary>
        private const string POST = "POST";
        /// <summary>
        /// méthode HTTP pour update
        /// </summary>

        static Access()
        {
            string logFilePath = "logs.txt"; // Fichier de logs
            Trace.Listeners.Add(new TextWriterTraceListener(logFilePath));
            Trace.AutoFlush = true; // Écrit direct dans le fichier sans attendre
        }

        /// <summary>
        /// Méthode privée pour créer un singleton et initialiser l'accès à l'API
        /// </summary>
        private Access()
        {
            try
            {
                string login = ConfigurationManager.AppSettings["ApiLogin"];
                string password = ConfigurationManager.AppSettings["ApiPassword"];
                string apiUrl = ConfigurationManager.AppSettings["ApiUrl"];

                api = ApiRest.GetInstance(apiUrl, $"{login}:{password}");
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Création et retour de l'instance unique de la classe
        /// </summary>
        /// <returns>instance unique de la classe Access</returns>
        public static Access GetInstance()
        {
            if (instance == null)
            {
                instance = new Access();
            }
            return instance;
        }

        /// <summary>
        /// Retourne tous les genres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Genre sous forme de Categorie</returns>
        public List<Categorie> GetAllGenres()
        {
            IEnumerable<Genre> lesGenres = TraitementRecup<Genre>(GET, "genre", null);
            return new List<Categorie>(lesGenres);
        }

        /// <summary>
        /// Retourne tous les rayons à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Rayon sous forme de Categorie</returns>
        public List<Categorie> GetAllRayons()
        {
            IEnumerable<Rayon> lesRayons = TraitementRecup<Rayon>(GET, "rayon", null);
            return new List<Categorie>(lesRayons);
        }

        /// <summary>
        /// Retourne toutes les catégories de public à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Public sous forme de Categorie</returns>
        public List<Categorie> GetAllPublics()
        {
            IEnumerable<Public> lesPublics = TraitementRecup<Public>(GET, "public", null);
            return new List<Categorie>(lesPublics);
        }

        /// <summary>
        /// Retourne toutes les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            List<Livre> lesLivres = TraitementRecup<Livre>(GET, "livre", null);
            return lesLivres;
        }

        /// <summary>
        /// Retourne tous les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            List<Dvd> lesDvd = TraitementRecup<Dvd>(GET, "dvd", null);
            return lesDvd;
        }

        /// <summary>
        /// Retourne toutes les revues à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            List<Revue> lesRevues = TraitementRecup<Revue>(GET, "revue", null);
            return lesRevues;
        }

        /// <summary>
        /// Retourne les exemplaires d'une revue à partir de son identifiant
        /// </summary>
        /// <param name="idDocument">Identifiant de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            String jsonIdDocument = convertToJson("id", idDocument);
            List<Exemplaire> lesExemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + jsonIdDocument, null);
            return lesExemplaires;
        }

        /// <summary>
        /// Retourne les exemplaires d'un document en fonction de son identifiant
        /// </summary>
        /// <param name="idDocument">Identifiant du document</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesByIdDocument(string idDocument)
        {
            string jsonIdDocument = convertToJson("id", idDocument);
            List<Exemplaire> exemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + jsonIdDocument, null);
            return exemplaires;
        }

        /// <summary>
        /// Écriture d'un exemplaire en base de données
        /// </summary>
        /// <param name="exemplaire">Exemplaire à insérer</param>
        /// <returns>true si l'insertion a pu se faire, sinon false</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            String jsonExemplaire = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());
            try
            {
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(POST, "exemplaire", "champs=" + jsonExemplaire);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Traite la récupération de données depuis l'API avec conversion du JSON en liste d'objets
        /// </summary>
        /// <typeparam name="T">Type d'objet attendu</typeparam>
        /// <param name="methode">Verbe HTTP (GET, POST, etc.)</param>
        /// <param name="message">Message ou point d'accès de l'API</param>
        /// <param name="parametres">Paramètres à envoyer</param>
        /// <returns>Liste d'objets récupérés ou liste vide</returns>
        private List<T> TraitementRecup<T>(String methode, String message, String parametres)
        {
            List<T> liste = new List<T>();
            try
            {
                JObject retour = api.RecupDistant(methode, message, parametres);
                // Extraction du code retourné
                String code = (String)retour["code"];
                if (code.Equals("200"))
                {
                    // Pour un GET : récupération de la liste d'objets
                    if (methode.Equals(GET))
                    {
                        String resultString = JsonConvert.SerializeObject(retour["result"]);
                        // Construction de la liste d'objets à partir du retour de l'API
                        liste = JsonConvert.DeserializeObject<List<T>>(resultString, new CustomBooleanJsonConverter());
                    }
                }
                else
                {
                    Trace.TraceError("code erreur = " + code + " message = " + (String)retour["message"]);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Erreur lors de l'accès à l'API : " + e.Message);
                Environment.Exit(0);
            }
            return liste;
        }

        /// <summary>
        /// Convertit en JSON un couple nom/valeur
        /// </summary>
        /// <param name="nom">Nom de la clé</param>
        /// <param name="valeur">Valeur associée</param>
        /// <returns>Chaîne JSON représentant le couple</returns>
        private String convertToJson(Object nom, Object valeur)
        {
            Dictionary<Object, Object> dictionary = new Dictionary<Object, Object>();
            dictionary.Add(nom, valeur);
            return JsonConvert.SerializeObject(dictionary);
        }

        /// <summary>
        /// Convertisseur personnalisé pour gérer le format de date
        /// </summary>
        private sealed class CustomDateTimeConverter : IsoDateTimeConverter
        {
            public CustomDateTimeConverter()
            {
                base.DateTimeFormat = "yyyy-MM-dd";
            }
        }

        /// <summary>
        /// Convertisseur personnalisé pour gérer la conversion des booléens depuis JSON
        /// </summary>
        private sealed class CustomBooleanJsonConverter : JsonConverter<bool>
        {
            public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return Convert.ToBoolean(reader.ValueType == typeof(string) ? Convert.ToByte(reader.Value) : reader.Value);
            }

            public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }

        /// <summary>
        /// Vérifie si un document peut être supprimé (aucun exemplaire associé)
        /// </summary>
        /// <param name="idDocument">Identifiant du document</param>
        /// <returns>true si aucun exemplaire n'est associé, sinon false</returns>
        public bool PeutSupprimerDocument(string idDocument)
        {
            string message = "exemplaire/" + convertToJson("id", idDocument);
            List<Exemplaire> exemplaires = TraitementRecup<Exemplaire>(GET, message, null);
            return exemplaires.Count == 0;
        }

        /// <summary>
        /// Récupère les commandes associées à un livre via son identifiant
        /// </summary>
        /// <param name="idLivre">Identifiant du livre</param>
        /// <returns>Liste des commandes liées au livre</returns>
        public List<CommandeDocument> GetCommandesByLivre(string idLivre)
        {
            // 1. Filtrer les CommandeDocument par idLivreDvd
            string jsonIdLivre = convertToJson("idLivreDvd", idLivre);
            string url = "commandedocument";
            string paramCommandes = $"champs={Uri.EscapeDataString(jsonIdLivre)}";
            List<CommandeDocument> commandes = TraitementRecup<CommandeDocument>("GET", url, paramCommandes);

            // 2. Récupérer toutes les commandes (avec date et montant)
            List<Commande> toutesLesCommandes = TraitementRecup<Commande>("GET", "commande", null);

            // Dictionnaire de correspondance IdSuivi -> Libellé
            Dictionary<int, string> libellesSuivi = new Dictionary<int, string>
            {
                { 1, "En cours" },
                { 2, "Relancée" },
                { 3, "Livrée" },
                { 4, "Réglée" }
            };

            // 3. Associer les informations à chaque commande
            foreach (CommandeDocument cmdDoc in commandes)
            {
                // Ajouter Date et Montant
                Commande cmdInfo = toutesLesCommandes.Find(c => c.Id == cmdDoc.Id);
                if (cmdInfo != null)
                {
                    cmdDoc.DateCommande = cmdInfo.DateCommande;
                    cmdDoc.Montant = cmdInfo.Montant;
                }

                // Ajouter le libellé de suivi
                if (libellesSuivi.ContainsKey(cmdDoc.IdSuivi))
                {
                    cmdDoc.LibelleSuivi = libellesSuivi[cmdDoc.IdSuivi];
                }
                else
                {
                    cmdDoc.LibelleSuivi = "Inconnu";
                }
            }

            return commandes;
        }

        /// <summary>
        /// Ajoute une nouvelle commande de document
        /// </summary>
        /// <param name="commande">Commande à ajouter</param>
        /// <returns>True si l'ajout est réussi, sinon false</returns>
        public bool AjouterCommande(CommandeDocument commande)
        {
            // 1. Insertion dans la table 'commande' sans envoyer l'ID
            string messageCommande = "commande";
            string jsonCommande = JsonConvert.SerializeObject(new
            {
                dateCommande = commande.DateCommande.ToString("yyyy-MM-dd"),
                montant = commande.Montant
            });
            string paramCommande = $"champs={Uri.EscapeDataString(jsonCommande)}";
            JObject responseCommande = api.RecupDistant("POST", messageCommande, paramCommande);
            if (responseCommande?["code"]?.ToString() != "200")
            {
                return false;
            }

            // 2. Récupérer l'ID de la dernière commande (non retourné dans le POST)
            string newId = GetLastCommandeId();
            if (string.IsNullOrEmpty(newId))
            {
                return false;
            }

            // Mise à jour de l'ID de l'objet commande
            commande.Id = newId;

            // 3. Insertion dans la table 'commandedocument'
            string messageCommandeDocument = "commandedocument";
            string jsonCommandeDocument = JsonConvert.SerializeObject(new
            {
                id = commande.Id,
                nbExemplaire = commande.NbExemplaire,
                idLivreDvd = commande.IdLivreDvd,
                idSuivi = commande.IdSuivi
            });
            string paramCommandeDocument = $"champs={Uri.EscapeDataString(jsonCommandeDocument)}";
            JObject responseCommandeDocument = api.RecupDistant("POST", messageCommandeDocument, paramCommandeDocument);

            return responseCommandeDocument?["code"]?.ToString() == "200";
        }

        /// <summary>
        /// Récupère l'identifiant de la dernière commande enregistrée
        /// </summary>
        /// <returns>Identifiant de la dernière commande si trouvé, sinon null</returns>
        public string GetLastCommandeId()
        {
            JObject response = api.RecupDistant("GET", "commande", null);

            if (response?["code"]?.ToString() == "200")
            {
                JArray resultArray = (JArray)response["result"];
                if (resultArray != null && resultArray.Count > 0)
                {
                    // Dernière commande (en supposant que l'API retourne dans l'ordre d'insertion)
                    JObject lastCommande = (JObject)resultArray[resultArray.Count - 1];
                    return lastCommande["id"]?.ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// Modifie l'état de suivi d'une commande
        /// </summary>
        /// <param name="commande">Commande dont le suivi doit être modifié</param>
        /// <param name="nouvelIdSuivi">Nouvel identifiant de suivi</param>
        /// <returns>True si la modification est réussie, sinon false</returns>
        public bool ModifierSuiviCommande(CommandeDocument commande, int nouvelIdSuivi)
        {
            Trace.TraceError("Ancien suivi : " + commande.IdSuivi);
            Trace.TraceError("Nouveau suivi : " + nouvelIdSuivi);

            // Règle 1 : une commande livrée ou réglée ne peut pas revenir à un état antérieur
            if ((commande.IdSuivi == 3 || commande.IdSuivi == 4) && nouvelIdSuivi < commande.IdSuivi)
            {
                return false;
            }

            // Règle 2 : une commande ne peut pas passer à "réglée" sans être livrée
            if (nouvelIdSuivi == 4 && commande.IdSuivi != 3)
            {
                return false;
            }

            // Préparation du corps de la requête
            string jsonUpdate = JsonConvert.SerializeObject(new
            {
                idSuivi = nouvelIdSuivi
            });

            string param = $"champs={Uri.EscapeDataString(jsonUpdate)}";

            // L’ID doit être dans l’URL
            string url = $"commandedocument/{commande.Id}";

            JObject response = api.RecupDistant("PUT", url, param);
            Trace.TraceError("Requête PUT envoyée à l'API : " + url);
            Trace.TraceError("Body : " + param);
            Trace.TraceError("Code retour de l'API : " + response?["code"]);

            return response?["code"]?.ToString() == "200";
        }

        /// <summary>
        /// Supprime une commande de document
        /// </summary>
        /// <param name="idCommande">Identifiant de la commande à supprimer</param>
        /// <returns>True si la suppression est réussie, sinon false</returns>
        public bool SupprimerCommande(string idCommande)
        {
            string json = JsonConvert.SerializeObject(new { id = idCommande });
            string param = $"champs={Uri.EscapeDataString(json)}";
            JObject response = api.RecupDistant("DELETE", "commandedocument", param);

            Trace.TraceError("Suppression commande id : " + idCommande);
            return response?["code"]?.ToString() == "200";
        }

        /// <summary>
        /// Récupère les commandes associées à un DVD via son identifiant
        /// </summary>
        /// <param name="idDvd">Identifiant du DVD</param>
        /// <returns>Liste des commandes liées au DVD</returns>
        public List<CommandeDocument> GetCommandesByDvd(string idDvd)
        {
            string jsonIdDvd = convertToJson("idLivreDvd", idDvd);
            string url = "commandedocument";
            string paramCommandes = $"champs={Uri.EscapeDataString(jsonIdDvd)}";
            List<CommandeDocument> commandes = TraitementRecup<CommandeDocument>("GET", url, paramCommandes);

            List<Commande> toutesLesCommandes = TraitementRecup<Commande>("GET", "commande", null);

            Dictionary<int, string> libellesSuivi = new Dictionary<int, string>
            {
                { 1, "En cours" },
                { 2, "Relancée" },
                { 3, "Livrée" },
                { 4, "Réglée" }
            };

            foreach (CommandeDocument cmdDoc in commandes)
            {
                Commande cmdInfo = toutesLesCommandes.Find(c => c.Id == cmdDoc.Id);
                if (cmdInfo != null)
                {
                    cmdDoc.DateCommande = cmdInfo.DateCommande;
                    cmdDoc.Montant = cmdInfo.Montant;
                }

                cmdDoc.LibelleSuivi = libellesSuivi.ContainsKey(cmdDoc.IdSuivi)
                    ? libellesSuivi[cmdDoc.IdSuivi]
                    : "Inconnu";
            }

            return commandes;
        }

        /// <summary>
        /// Récupère les abonnements associés à une revue via son identifiant
        /// </summary>
        /// <param name="idRevue">Identifiant de la revue</param>
        /// <returns>Liste des abonnements liés à la revue</returns>
        public List<Abonnement> GetAbonnementsByRevue(string idRevue)
        {
            string jsonIdRevue = convertToJson("idRevue", idRevue);
            string url = "abonnement";
            string param = $"champs={Uri.EscapeDataString(jsonIdRevue)}";
            List<Abonnement> abonnements = TraitementRecup<Abonnement>("GET", url, param);

            List<Commande> commandes = TraitementRecup<Commande>("GET", "commande", null);

            foreach (Abonnement ab in abonnements)
            {
                Commande cmd = commandes.Find(c => c.Id == ab.Id);
                if (cmd != null)
                {
                    ab.DateCommande = cmd.DateCommande;
                    ab.Montant = cmd.Montant;
                }
            }

            return abonnements;
        }

        /// <summary>
        /// Ajoute un abonnement pour une revue
        /// </summary>
        /// <param name="abonnement">Abonnement à ajouter</param>
        /// <returns>True si l'ajout est réussi, sinon false</returns>
        public bool AjouterAbonnement(Abonnement abonnement)
        {
            // 1. Insertion dans la table 'commande'
            string jsonCommande = JsonConvert.SerializeObject(new
            {
                dateCommande = abonnement.DateCommande.ToString("yyyy-MM-dd"),
                montant = abonnement.Montant
            });

            string paramCommande = $"champs={Uri.EscapeDataString(jsonCommande)}";
            JObject responseCommande = api.RecupDistant("POST", "commande", paramCommande);

            if (responseCommande?["code"]?.ToString() != "200")
                return false;

            // 2. Récupérer l’ID de la commande insérée
            string id = GetLastCommandeId();
            if (string.IsNullOrEmpty(id)) return false;

            // 3. Insertion dans la table 'abonnement'
            string jsonAbonnement = JsonConvert.SerializeObject(new
            {
                id = id,
                dateFinAbonnement = abonnement.DateFinAbonnement.ToString("yyyy-MM-dd"),
                idRevue = abonnement.IdRevue
            });

            string paramAbonnement = $"champs={Uri.EscapeDataString(jsonAbonnement)}";
            JObject responseAbonnement = api.RecupDistant("POST", "abonnement", paramAbonnement);

            return responseAbonnement?["code"]?.ToString() == "200";
        }

        /// <summary>
        /// Supprime un abonnement en fonction de son identifiant
        /// </summary>
        /// <param name="idAbonnement">Identifiant de l'abonnement à supprimer</param>
        /// <returns>True si la suppression est réussie, sinon false</returns>
        public bool SupprimerAbonnement(string idAbonnement)
        {
            string json = JsonConvert.SerializeObject(new { id = idAbonnement });
            string param = $"champs={Uri.EscapeDataString(json)}";
            JObject response = api.RecupDistant("DELETE", "abonnement", param);
            return response?["code"]?.ToString() == "200";
        }

        /// <summary>
        /// Récupère les exemplaires d'une revue via son identifiant
        /// </summary>
        /// <param name="idRevue">Identifiant de la revue</param>
        /// <returns>Liste des exemplaires associés à la revue</returns>
        public List<Exemplaire> GetExemplairesByRevue(string idRevue)
        {
            string jsonIdRevue = convertToJson("id", idRevue);
            string param = $"champs={Uri.EscapeDataString(jsonIdRevue)}";
            return TraitementRecup<Exemplaire>("GET", "exemplaire", param);
        }

        /// <summary>
        /// Récupère tous les abonnements enregistrés
        /// </summary>
        /// <returns>Liste de tous les abonnements</returns>
        public List<Abonnement> GetAllAbonnements()
        {
            // Récupération de tous les abonnements sans filtre particulier
            List<Abonnement> abonnements = TraitementRecup<Abonnement>("GET", "abonnement", null);

            // Récupération de toutes les commandes pour associer la dateCommande et le montant
            List<Commande> commandes = TraitementRecup<Commande>("GET", "commande", null);

            // Complétion de chaque abonnement avec les informations de commande
            foreach (Abonnement ab in abonnements)
            {
                Commande cmd = commandes.Find(c => c.Id == ab.Id);
                if (cmd != null)
                {
                    ab.DateCommande = cmd.DateCommande;
                    ab.Montant = cmd.Montant;
                }
            }

            return abonnements;
        }

        /// <summary>
        /// Récupère les abonnements qui arriveront à échéance dans 30 jours
        /// </summary>
        /// <returns>Liste des abonnements à échéance dans 30 jours triés par date de fin</returns>
        public List<Abonnement> GetAbonnementsEcheantDans30Jours()
        {
            // Récupérer tous les abonnements
            List<Abonnement> allAbonnements = GetAllAbonnements();

            // Filtrer ceux dont la date de fin est <= DateTime.Now + 30 jours
            DateTime limite = DateTime.Now.Date.AddDays(30);
            List<Abonnement> abonnementsEcheant = allAbonnements
                .Where(ab => ab.DateFinAbonnement <= limite)
                .ToList();

            // Tri par date de fin d’abonnement (chronologique)
            abonnementsEcheant.Sort((x, y) => x.DateFinAbonnement.CompareTo(y.DateFinAbonnement));

            return abonnementsEcheant;
        }

        /// <summary>
        /// Récupère les exemplaires d'un document (Livre ou DVD) en se basant sur son identifiant
        /// </summary>
        /// <param name="idDocument">Identifiant du document</param>
        /// <returns>Liste des exemplaires, triés par date d'achat décroissante</returns>
        private List<Exemplaire> GetExemplairesByDocument(string idDocument)
        {
            List<Exemplaire> tousLesExemplaires = TraitementRecup<Exemplaire>("GET", "exemplaire", null);

            return tousLesExemplaires
                .Where(ex => ex.Id == idDocument)
                .OrderByDescending(ex => ex.DateAchat)
                .ToList();
        }

        /// <summary>
        /// Récupère les exemplaires d'un livre via son identifiant
        /// </summary>
        /// <param name="idDocument">Identifiant du livre</param>
        /// <returns>Liste des exemplaires du livre</returns>
        public List<Exemplaire> GetExemplairesByLivre(string idDocument)
        {
            return GetExemplairesByDocument(idDocument);
        }

        /// <summary>
        /// Récupère les exemplaires d'un DVD via son identifiant
        /// </summary>
        /// <param name="idDocument">Identifiant du DVD</param>
        /// <returns>Liste des exemplaires du DVD</returns>
        public List<Exemplaire> GetExemplairesByDvd(string idDocument)
        {
            return GetExemplairesByDocument(idDocument);
        }

        /// <summary>
        /// Modifie l'état d'un exemplaire
        /// </summary>
        /// <param name="idDocument">Identifiant du document</param>
        /// <param name="numero">Numéro d'exemplaire</param>
        /// <param name="idEtat">Nouvel état à attribuer</param>
        /// <returns>True si la modification est réussie, sinon false</returns>
        public bool ModifierEtatExemplaire(string idDocument, int numero, string idEtat)
        {
            Dictionary<string, string> parametres = new Dictionary<string, string>
            {
                { "id", idDocument },
                { "numero", numero.ToString() },
                { "idEtat", idEtat }
            };

            string json = JsonConvert.SerializeObject(parametres);
            JObject retour = api.RecupDistant("PUT", $"exemplaire/{idDocument}", $"champs={Uri.EscapeDataString(json)}");

            return retour != null && retour["code"].ToString() == "200";
        }

        /// <summary>
        /// Récupère tous les états depuis l'API
        /// </summary>
        /// <returns>Liste d'objets Etat</returns>
        public List<Etat> GetAllEtats()
        {
            return TraitementRecup<Etat>("GET", "etat", null);
        }

        /// <summary>
        /// Supprime un exemplaire donné
        /// </summary>
        /// <param name="idDocument">Identifiant du document</param>
        /// <param name="numero">Numéro d'exemplaire à supprimer</param>
        /// <returns>True si la suppression est réussie, sinon false</returns>
        public bool SupprimerExemplaire(string idDocument, int numero)
        {
            Dictionary<string, string> parametres = new Dictionary<string, string>
            {
                { "id", idDocument },
                { "numero", numero.ToString() }
            };

            string json = JsonConvert.SerializeObject(parametres);
            JObject retour = api.RecupDistant("DELETE", "exemplaire", $"champs={Uri.EscapeDataString(json)}");

            return retour != null && retour["code"].ToString() == "200";
        }

        /// <summary>
        /// Authentifie un utilisateur via l'API
        /// </summary>
        /// <param name="login">Nom d'utilisateur</param>
        /// <param name="password">Mot de passe de l'utilisateur</param>
        /// <returns>Objet Utilisateur authentifié ou null en cas d'échec</returns>
        public Utilisateur AuthentifierUtilisateur(string login, string password)
        {
            Dictionary<string, string> parametres = new Dictionary<string, string>
            {
                { "login", login },
                { "password", password }
            };

            string json = JsonConvert.SerializeObject(parametres);
            JObject retour = api.RecupDistant("POST", "authentification", $"champs={Uri.EscapeDataString(json)}");

            if (retour != null && retour["code"].ToString() == "200")
            {
                JObject userData = (JObject)retour["result"];
                return new Utilisateur(
                    userData["id"].ToString(),
                    userData["nom"].ToString(),
                    userData["prenom"].ToString(),
                    userData["service"].ToString()
                );
            }
            else
            {
                return null;
            }
        }
    }
}
