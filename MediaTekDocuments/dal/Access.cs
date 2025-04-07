using System;
using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Linq;


namespace MediaTekDocuments.dal
{
    /// <summary>
    /// Classe d'accès aux données
    /// </summary>
    public class Access
    {
        /// <summary>
        /// adresse de l'API
        /// </summary>
        private static readonly string uriApi = "http://localhost/rest_mediatekdocuments/";
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
        /// 
        private const string PUT = "PUT";

        /// <summary>
        /// Méthode privée pour créer un singleton
        /// initialise l'accès à l'API
        /// </summary>
        private Access()
        {
            String authenticationString;
            try
            {
                authenticationString = "admin:adminpwd";
                api = ApiRest.GetInstance(uriApi, authenticationString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Création et retour de l'instance unique de la classe
        /// </summary>
        /// <returns>instance unique de la classe</returns>
        public static Access GetInstance()
        {
            if(instance == null)
            {
                instance = new Access();
            }
            return instance;
        }

        /// <summary>
        /// Retourne tous les genres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            IEnumerable<Genre> lesGenres = TraitementRecup<Genre>(GET, "genre", null);
            return new List<Categorie>(lesGenres);
        }

        /// <summary>
        /// Retourne tous les rayons à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            IEnumerable<Rayon> lesRayons = TraitementRecup<Rayon>(GET, "rayon", null);
            return new List<Categorie>(lesRayons);
        }

        /// <summary>
        /// Retourne toutes les catégories de public à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
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
        /// Retourne toutes les dvd à partir de la BDD
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
        /// Retourne les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocument">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            String jsonIdDocument = convertToJson("id", idDocument);
            List<Exemplaire> lesExemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + jsonIdDocument, null);
            return lesExemplaires;
        }

        public List<Exemplaire> GetExemplairesByIdDocument(string idDocument)
        {
            string jsonIdDocument = convertToJson("id", idDocument);
            List<Exemplaire> exemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + jsonIdDocument, null);
            return exemplaires;
        }


        /// <summary>
        /// ecriture d'un exemplaire en base de données
        /// </summary>
        /// <param name="exemplaire">exemplaire à insérer</param>
        /// <returns>true si l'insertion a pu se faire (retour != null)</returns>
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
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Traitement de la récupération du retour de l'api, avec conversion du json en liste pour les select (GET)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methode">verbe HTTP (GET, POST, PUT, DELETE)</param>
        /// <param name="message">information envoyée dans l'url</param>
        /// <param name="parametres">paramètres à envoyer dans le body, au format "chp1=val1&chp2=val2&..."</param>
        /// <returns>liste d'objets récupérés (ou liste vide)</returns>
        private List<T> TraitementRecup<T> (String methode, String message, String parametres)
        {
            // trans
            List<T> liste = new List<T>();
            try
            {
                JObject retour = api.RecupDistant(methode, message, parametres);
                // extraction du code retourné
                String code = (String)retour["code"];
                if (code.Equals("200"))
                {
                    // dans le cas du GET (select), récupération de la liste d'objets
                    if (methode.Equals(GET))
                    {
                        String resultString = JsonConvert.SerializeObject(retour["result"]);
                        // construction de la liste d'objets à partir du retour de l'api
                        liste = JsonConvert.DeserializeObject<List<T>>(resultString, new CustomBooleanJsonConverter());
                    }
                }
                else
                {
                    Console.WriteLine("code erreur = " + code + " message = " + (String)retour["message"]);
                }
            }catch(Exception e)
            {
                Console.WriteLine("Erreur lors de l'accès à l'API : "+e.Message);
                Environment.Exit(0);
            }
            return liste;
        }

        /// <summary>
        /// Convertit en json un couple nom/valeur
        /// </summary>
        /// <param name="nom"></param>
        /// <param name="valeur"></param>
        /// <returns>couple au format json</returns>
        private String convertToJson(Object nom, Object valeur)
        {
            Dictionary<Object, Object> dictionary = new Dictionary<Object, Object>();
            dictionary.Add(nom, valeur);
            return JsonConvert.SerializeObject(dictionary);
        }

        /// <summary>
        /// Modification du convertisseur Json pour gérer le format de date
        /// </summary>
        private sealed class CustomDateTimeConverter : IsoDateTimeConverter
        {
            public CustomDateTimeConverter()
            {
                base.DateTimeFormat = "yyyy-MM-dd";
            }
        }

        /// <summary>
        /// Modification du convertisseur Json pour prendre en compte les booléens
        /// classe trouvée sur le site :
        /// https://www.thecodebuzz.com/newtonsoft-jsonreaderexception-could-not-convert-string-to-boolean/
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
        /// <param name="idDocument">id du document</param>
        /// <returns>true si aucun exemplaire, false sinon</returns>
        public bool PeutSupprimerDocument(string idDocument)
        {
            string message = "exemplaire/" + convertToJson("id", idDocument);
            List<Exemplaire> exemplaires = TraitementRecup<Exemplaire>(GET, message, null);
            return exemplaires.Count == 0;
        }


        public List<CommandeDocument> GetCommandesByLivre(string idLivre)
        {
            // 1. Filtrer les CommandeDocument par idLivreDvd
            string jsonIdLivre = convertToJson("idLivreDvd", idLivre);
            string url = "commandedocument";
            string paramCommandes = $"champs={Uri.EscapeDataString(jsonIdLivre)}";
            List<CommandeDocument> commandes = TraitementRecup<CommandeDocument>("GET", url, paramCommandes);

            // 2. Récupérer toutes les commandes (avec date et montant)
            List<Commande> toutesLesCommandes = TraitementRecup<Commande>("GET", "commande", null);

            // ✅ Dictionnaire de correspondance IdSuivi -> Libellé
            Dictionary<int, string> libellesSuivi = new Dictionary<int, string>
            {
                { 1, "En cours" },
                { 2, "Relancée" },
                { 3, "Livrée" },
                { 4, "Réglée" }
            };

            // 3. Associer les infos à chaque commande
            foreach (CommandeDocument cmdDoc in commandes)
            {
                // Ajouter Date + Montant
                Commande cmdInfo = toutesLesCommandes.Find(c => c.Id == cmdDoc.Id);
                if (cmdInfo != null)
                {
                    cmdDoc.DateCommande = cmdInfo.DateCommande;
                    cmdDoc.Montant = cmdInfo.Montant;
                }

                // Ajouter le libellé de suivi à partir du dictionnaire
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







        public bool AjouterCommande(CommandeDocument commande)
        {
            ApiRest api = ApiRest.GetInstance("http://localhost/rest_mediatekdocuments/", "admin:adminpwd");

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

            // 2. Récupérer l'ID de la dernière commande (car il n'est pas retourné dans la réponse du POST)
            string newId = GetLastCommandeId();
            if (string.IsNullOrEmpty(newId))
            {
                return false;
            }

            // On met à jour l'ID de l'objet commande
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


        public string GetLastCommandeId()
        {
            ApiRest api = ApiRest.GetInstance("http://localhost/rest_mediatekdocuments/", "admin:adminpwd");
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

        public bool ModifierSuiviCommande(CommandeDocument commande, int nouvelIdSuivi)
        {
            Console.WriteLine("Ancien suivi : " + commande.IdSuivi);
            Console.WriteLine("Nouveau suivi : " + nouvelIdSuivi);

            // ✅ Règle 1 : une commande livrée ou réglée ne peut pas revenir à un état précédent
            if ((commande.IdSuivi == 3 || commande.IdSuivi == 4) && nouvelIdSuivi < commande.IdSuivi)
            {
                return false;
            }

            // ✅ Règle 2 : une commande ne peut pas passer à "réglée" sans être livrée
            if (nouvelIdSuivi == 4 && commande.IdSuivi != 3)
            {
                return false;
            }

            // ✅ Préparer le corps de la requête (le champ à modifier)
            string jsonUpdate = JsonConvert.SerializeObject(new
            {
                idSuivi = nouvelIdSuivi
            });

            string param = $"champs={Uri.EscapeDataString(jsonUpdate)}";

            // ✅ L’ID doit être dans l’URL maintenant
            string url = $"commandedocument/{commande.Id}";

            JObject response = api.RecupDistant("PUT", url, param);
            Console.WriteLine("Requête PUT envoyée à l'API : " + url);
            Console.WriteLine("Body : " + param);
            Console.WriteLine("Code retour de l'API : " + response?["code"]);

            return response?["code"]?.ToString() == "200";
        }

        public bool SupprimerCommande(string idCommande)
        {
            string json = JsonConvert.SerializeObject(new { id = idCommande });
            string param = $"champs={Uri.EscapeDataString(json)}";
            JObject response = api.RecupDistant("DELETE", "commandedocument", param);

            Console.WriteLine("Suppression commande id : " + idCommande);
            return response?["code"]?.ToString() == "200";
        }




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

        public bool AjouterAbonnement(Abonnement abonnement)
        {
            ApiRest api = ApiRest.GetInstance("http://localhost/rest_mediatekdocuments/", "admin:adminpwd");

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

        public bool SupprimerAbonnement(string idAbonnement)
        {
            string json = JsonConvert.SerializeObject(new { id = idAbonnement });
            string param = $"champs={Uri.EscapeDataString(json)}";
            JObject response = api.RecupDistant("DELETE", "abonnement", param);
            return response?["code"]?.ToString() == "200";
        }

        public List<Exemplaire> GetExemplairesByRevue(string idRevue)
        {
            string jsonIdRevue = convertToJson("id", idRevue);
            string param = $"champs={Uri.EscapeDataString(jsonIdRevue)}";
            return TraitementRecup<Exemplaire>("GET", "exemplaire", param);
        }

        public List<Abonnement> GetAllAbonnements()
        {
            // On récupère tous les abonnements (sans filtre particulier)
            List<Abonnement> abonnements = TraitementRecup<Abonnement>("GET", "abonnement", null);

            // On récupère toutes les commandes pour y associer la dateCommande et le montant
            List<Commande> commandes = TraitementRecup<Commande>("GET", "commande", null);

            // On complète chaque abonnement avec les infos de commande
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

        public List<Abonnement> GetAbonnementsEcheantDans30Jours()
        {
            // Récupérer tous les abonnements
            List<Abonnement> allAbonnements = GetAllAbonnements();

            // Filtrer ceux dont la date de fin est <= DateTime.Now + 30 jours
            DateTime limite = DateTime.Now.Date.AddDays(30);
            List<Abonnement> abonnementsEcheant = allAbonnements
                .Where(ab => ab.DateFinAbonnement <= limite)
                .ToList();

            // Les trier par date de fin d’abonnement (chronologique)
            abonnementsEcheant.Sort((x, y) => x.DateFinAbonnement.CompareTo(y.DateFinAbonnement));

            return abonnementsEcheant;
        }

        /// <summary>
        /// Récupère les exemplaires d'un document (livre ou autre) en filtrant par Id
        /// </summary>
        /// <param name="idDocument">ID du document (livre)</param>
        /// <returns>Liste des exemplaires correspondants</returns>
        public List<Exemplaire> GetExemplairesByLivre(string idDocument)
        {
            // Récupère TOUS les exemplaires
            List<Exemplaire> tousLesExemplaires = TraitementRecup<Exemplaire>("GET", "exemplaire", null);

            // Filtre sur l'ID et tri par date d'achat décroissante
            return tousLesExemplaires
                .Where(ex => ex.Id == idDocument)
                .OrderByDescending(ex => ex.DateAchat)
                .ToList();
        }

        /// <summary>
        /// Change l'état d'un exemplaire
        /// </summary>
        /// <param name="idDocument">ID du document</param>
        /// <param name="numero">Numéro d'exemplaire</param>
        /// <param name="idEtat">Nouveau état</param>
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
        public List<Etat> GetAllEtats()
        {
            return TraitementRecup<Etat>("GET", "etat", null);
        }

        /// <summary>
        /// Supprime un exemplaire donné
        /// </summary>
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

        public List<Exemplaire> GetExemplairesByDvd(string idDocument)
        {
            // Récupère TOUS les exemplaires
            List<Exemplaire> tousLesExemplaires = TraitementRecup<Exemplaire>("GET", "exemplaire", null);

            // Filtre sur l'ID du DVD (c'est comme pour Livre)
            return tousLesExemplaires
                .Where(ex => ex.Id == idDocument)
                .OrderByDescending(ex => ex.DateAchat)
                .ToList();
        }

        /// <summary>
        /// Authentifie un utilisateur via l'API
        /// </summary>
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
