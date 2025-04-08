using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MediaTekDocuments.manager;
using System;
using System.Windows.Forms;
using System.Configuration;

namespace MediaTekDocuments.controller
{
    /// <summary>
    /// Contrôleur lié à FrmMediatek.
    /// </summary>
    public class FrmMediatekController
    {
        /// <summary>
        /// Objet d'accès aux données.
        /// </summary>
        private readonly Access access;

        /// <summary>
        /// Instance de l'API REST configurée pour communiquer avec le serveur.
        /// </summary>
        private readonly ApiRest api = ApiRest.GetInstance(
            ConfigurationManager.AppSettings["ApiUrl"],
            $"{ConfigurationManager.AppSettings["ApiLogin"]}:{ConfigurationManager.AppSettings["ApiPassword"]}"
        );

        /// <summary>
        /// Constructeur : récupère l'instance unique d'accès aux données.
        /// </summary>
        public FrmMediatekController()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// Récupère la liste de tous les genres.
        /// </summary>
        /// <returns>Liste d'objets Genre encapsulés en Categorie</returns>
        public List<Categorie> GetAllGenres()
        {
            return access.GetAllGenres();
        }

        /// <summary>
        /// Récupère la liste de tous les livres.
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return access.GetAllLivres();
        }

        /// <summary>
        /// Récupère la liste de tous les DVD.
        /// </summary>
        /// <returns>Liste d'objets Dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return access.GetAllDvd();
        }

        /// <summary>
        /// Récupère la liste de toutes les revues.
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return access.GetAllRevues();
        }

        /// <summary>
        /// Récupère la liste de tous les rayons.
        /// </summary>
        /// <returns>Liste d'objets Rayon encapsulés en Categorie</returns>
        public List<Categorie> GetAllRayons()
        {
            return access.GetAllRayons();
        }

        /// <summary>
        /// Récupère la liste de tous les publics.
        /// </summary>
        /// <returns>Liste d'objets Public encapsulés en Categorie</returns>
        public List<Categorie> GetAllPublics()
        {
            return access.GetAllPublics();
        }

        /// <summary>
        /// Récupère les exemplaires d'une revue.
        /// </summary>
        /// <param name="idDocuement">Identifiant de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocuement)
        {
            return access.GetExemplairesRevue(idDocuement);
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la base de données.
        /// </summary>
        /// <param name="exemplaire">Objet Exemplaire à créer</param>
        /// <returns>True si la création a réussi, sinon false</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return access.CreerExemplaire(exemplaire);
        }

        /// <summary>
        /// Ajoute un livre dans la base de données en insérant dans les tables document, livres_dvd et livre.
        /// </summary>
        /// <param name="livre">Objet Livre à ajouter</param>
        /// <returns>True si toutes les insertions réussissent, sinon false</returns>
        public bool AjouterLivre(Livre livre)
        {
            // Données pour le document
            var docData = new Dictionary<string, string>()
            {
                { "id", livre.Id },
                { "titre", livre.Titre },
                { "image", livre.Image },
                { "idGenre", livre.IdGenre },
                { "idPublic", livre.IdPublic },
                { "idRayon", livre.IdRayon }
            };
            string docParams = $"champs={Uri.EscapeDataString(JsonConvert.SerializeObject(docData))}";

            // Données pour livres_dvd
            var livreDvdData = new Dictionary<string, string>()
            {
                { "id", livre.Id }
            };
            string livreDvdParams = $"champs={Uri.EscapeDataString(JsonConvert.SerializeObject(livreDvdData))}";

            // Données pour le livre
            var livreData = new Dictionary<string, string>()
            {
                { "id", livre.Id },
                { "isbn", livre.Isbn },
                { "auteur", livre.Auteur },
                { "collection", livre.Collection }
            };
            string livreParams = $"champs={Uri.EscapeDataString(JsonConvert.SerializeObject(livreData))}";

            JObject responseDoc = api.RecupDistant("POST", "document", docParams);
            JObject responseLivreDvd = api.RecupDistant("POST", "livres_dvd", livreDvdParams);
            JObject responseLivre = api.RecupDistant("POST", "livre", livreParams);

            return responseDoc?["code"]?.ToString() == "200" &&
                   responseLivreDvd?["code"]?.ToString() == "200" &&
                   responseLivre?["code"]?.ToString() == "200";
        }

        /// <summary>
        /// Vérifie si un document peut être supprimé (aucun exemplaire associé).
        /// </summary>
        /// <param name="idDocument">Identifiant du document</param>
        /// <returns>True si le document peut être supprimé, sinon false</returns>
        public bool PeutSupprimerDocument(string idDocument)
        {
            return access.PeutSupprimerDocument(idDocument);
        }

        /// <summary>
        /// Supprime un livre en supprimant les enregistrements dans les tables livre, livres_dvd et document.
        /// </summary>
        /// <param name="idDocument">Identifiant du livre à supprimer</param>
        /// <returns>True si toutes les suppressions réussissent, sinon false</returns>
        public bool SupprimerLivre(string idDocument)
        {
            string jsonData = JsonConvert.SerializeObject(new { id = idDocument });
            string param = $"champs={Uri.EscapeDataString(jsonData)}";

            JObject responseLivre = api.RecupDistant("DELETE", "livre", param);
            if (responseLivre == null || responseLivre["code"]?.ToString() != "200")
            {
                MessageBox.Show("❌ Suppression dans la table 'livre' échouée.\nRéponse invalide ou nulle.");
                return false;
            }

            JObject responseLivreDvd = api.RecupDistant("DELETE", "livres_dvd", param);
            if (responseLivreDvd == null || responseLivreDvd["code"]?.ToString() != "200")
            {
                MessageBox.Show("❌ Suppression dans la table 'livres_dvd' échouée.\nRéponse invalide ou nulle.");
                return false;
            }

            JObject responseDocument = api.RecupDistant("DELETE", "document", param);
            if (responseDocument == null || responseDocument["code"]?.ToString() != "200")
            {
                MessageBox.Show("❌ Suppression dans la table 'document' échouée.\nRéponse invalide ou nulle.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Modifie les informations d'un livre dans la base de données en mettant à jour les tables document, livres_dvd et livre.
        /// </summary>
        /// <param name="livre">Objet Livre modifié</param>
        /// <returns>True si la modification réussit pour toutes les tables, sinon false</returns>
        public bool ModifierLivre(Livre livre)
        {
            // Préparer les données pour la table "document"
            var docData = new Dictionary<string, string>()
            {
                { "id", livre.Id },
                { "titre", livre.Titre },
                { "image", livre.Image },
                { "idGenre", livre.IdGenre },
                { "idPublic", livre.IdPublic },
                { "idRayon", livre.IdRayon }
            };
            string docParams = $"champs={Uri.EscapeDataString(JsonConvert.SerializeObject(docData))}";

            // Préparer les données pour la table "livres_dvd"
            var livreDvdData = new Dictionary<string, string>()
            {
                { "id", livre.Id }
            };
            string livreDvdParams = $"champs={Uri.EscapeDataString(JsonConvert.SerializeObject(livreDvdData))}";

            // Préparer les données pour la table "livre"
            var livreData = new Dictionary<string, string>()
            {
                { "id", livre.Id },
                { "isbn", livre.Isbn },
                { "auteur", livre.Auteur },
                { "collection", livre.Collection }
            };
            string livreParams = $"champs={Uri.EscapeDataString(JsonConvert.SerializeObject(livreData))}";

            JObject responseDoc = api.RecupDistant("PUT", "document/" + livre.Id, docParams);
            JObject responseLivreDvd = api.RecupDistant("PUT", "livres_dvd/" + livre.Id, livreDvdParams);
            JObject responseLivre = api.RecupDistant("PUT", "livre/" + livre.Id, livreParams);

            return responseDoc?["code"]?.ToString() == "200" &&
                   responseLivreDvd?["code"]?.ToString() == "200" &&
                   responseLivre?["code"]?.ToString() == "200";
        }

        /// <summary>
        /// Ajoute un DVD dans la base de données en insérant dans les tables document, livres_dvd et dvd.
        /// </summary>
        /// <param name="dvd">Objet Dvd à ajouter</param>
        /// <returns>True si toutes les insertions réussissent, sinon false</returns>
        public bool AjouterDVD(Dvd dvd)
        {
            // Données pour la table "document"
            var docData = new Dictionary<string, string>()
            {
                { "id", dvd.Id },
                { "titre", dvd.Titre },
                { "image", dvd.Image },
                { "idGenre", dvd.IdGenre },
                { "idPublic", dvd.IdPublic },
                { "idRayon", dvd.IdRayon }
            };
            string docParams = $"champs={Uri.EscapeDataString(JsonConvert.SerializeObject(docData))}";

            // Données pour la table "livres_dvd"
            var dvdLivreData = new Dictionary<string, string>()
            {
                { "id", dvd.Id }
            };
            string dvdLivreParams = $"champs={Uri.EscapeDataString(JsonConvert.SerializeObject(dvdLivreData))}";

            // Données spécifiques pour la table "dvd"
            var dvdData = new Dictionary<string, string>()
            {
                { "id", dvd.Id },
                { "realisateur", dvd.Realisateur },
                { "synopsis", dvd.Synopsis },
                { "duree", dvd.Duree.ToString() }
            };
            string dvdParams = $"champs={Uri.EscapeDataString(JsonConvert.SerializeObject(dvdData))}";

            JObject responseDoc = api.RecupDistant("POST", "document", docParams);
            JObject responseDvdLivre = api.RecupDistant("POST", "livres_dvd", dvdLivreParams);
            JObject responseDvd = api.RecupDistant("POST", "dvd", dvdParams);

            return responseDoc?["code"]?.ToString() == "200" &&
                   responseDvdLivre?["code"]?.ToString() == "200" &&
                   responseDvd?["code"]?.ToString() == "200";
        }

        /// <summary>
        /// Modifie les informations d'un DVD dans la base de données en mettant à jour les tables document, livres_dvd et dvd.
        /// </summary>
        /// <param name="dvd">Objet Dvd modifié</param>
        /// <returns>True si la modification réussit pour toutes les tables, sinon false</returns>
        public bool ModifierDVD(Dvd dvd)
        {
            // Préparer les données pour la table "document"
            var docData = new Dictionary<string, string>()
            {
                { "id", dvd.Id },
                { "titre", dvd.Titre },
                { "image", dvd.Image },
                { "idGenre", dvd.IdGenre },
                { "idPublic", dvd.IdPublic },
                { "idRayon", dvd.IdRayon }
            };
            string docParams = $"champs={Uri.EscapeDataString(JsonConvert.SerializeObject(docData))}";

            // Pour "livres_dvd"
            var dvdLivreData = new Dictionary<string, string>()
            {
                { "id", dvd.Id }
            };
            string dvdLivreParams = $"champs={Uri.EscapeDataString(JsonConvert.SerializeObject(dvdLivreData))}";

            // Pour "dvd" (les champs spécifiques)
            var dvdData = new Dictionary<string, string>()
            {
                { "id", dvd.Id },
                { "realisateur", dvd.Realisateur },
                { "synopsis", dvd.Synopsis },
                { "duree", dvd.Duree.ToString() }
            };
            string dvdParams = $"champs={Uri.EscapeDataString(JsonConvert.SerializeObject(dvdData))}";

            // IMPORTANT : ajoute l'ID dans l'URL !
            JObject responseDoc = api.RecupDistant("PUT", "document/" + dvd.Id, docParams);
            JObject responseDvdLivre = api.RecupDistant("PUT", "livres_dvd/" + dvd.Id, dvdLivreParams);
            JObject responseDvd = api.RecupDistant("PUT", "dvd/" + dvd.Id, dvdParams);

            return responseDoc?["code"]?.ToString() == "200" &&
                   responseDvdLivre?["code"]?.ToString() == "200" &&
                   responseDvd?["code"]?.ToString() == "200";
        }

        /// <summary>
        /// Supprime un DVD en supprimant les enregistrements dans les tables dvd, livres_dvd et document.
        /// </summary>
        /// <param name="idDVD">Identifiant du DVD à supprimer</param>
        /// <returns>True si toutes les suppressions réussissent, sinon false</returns>
        public bool SupprimerDVD(string idDVD)
        {
            string jsonData = JsonConvert.SerializeObject(new { id = idDVD });
            string param = $"champs={Uri.EscapeDataString(jsonData)}";

            JObject responseDvd = api.RecupDistant("DELETE", "dvd", param);
            if (responseDvd == null || responseDvd["code"]?.ToString() != "200")
            {
                MessageBox.Show("❌ Suppression dans la table 'dvd' échouée.\nRéponse invalide ou nulle.");
                return false;
            }

            JObject responseDvdLivre = api.RecupDistant("DELETE", "livres_dvd", param);
            if (responseDvdLivre == null || responseDvdLivre["code"]?.ToString() != "200")
            {
                MessageBox.Show("❌ Suppression dans la table 'livres_dvd' échouée.\nRéponse invalide ou nulle.");
                return false;
            }

            JObject responseDoc = api.RecupDistant("DELETE", "document", param);
            if (responseDoc == null || responseDoc["code"]?.ToString() != "200")
            {
                MessageBox.Show("❌ Suppression dans la table 'document' échouée.\nRéponse invalide ou nulle.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Ajoute une revue dans la base de données en insérant dans les tables document et revue.
        /// </summary>
        /// <param name="revue">Objet Revue à ajouter</param>
        /// <returns>True si toutes les insertions réussissent, sinon false</returns>
        public bool AjouterRevue(Revue revue)
        {
            // Préparer les données pour la table "document"
            var docData = new Dictionary<string, string>()
            {
                { "id", revue.Id },
                { "titre", revue.Titre },
                { "image", revue.Image },
                { "idGenre", revue.IdGenre },
                { "idPublic", revue.IdPublic },
                { "idRayon", revue.IdRayon }
            };
            string docParams = $"champs={Uri.EscapeDataString(JsonConvert.SerializeObject(docData))}";

            // Préparer les données spécifiques pour la table "revue"
            var revueData = new Dictionary<string, string>()
            {
                { "id", revue.Id },
                { "periodicite", revue.Periodicite },
                { "delaiMiseADispo", revue.DelaiMiseADispo.ToString() }
                // Si aucune date de parution n'est disponible, elle peut être omise ou ajoutée selon le besoin.
            };
            string revueParams = $"champs={Uri.EscapeDataString(JsonConvert.SerializeObject(revueData))}";

            // Envoi de la requête POST sur "document" et sur "revue"
            JObject responseDoc = api.RecupDistant("POST", "document", docParams);
            JObject responseRevue = api.RecupDistant("POST", "revue", revueParams);

            return responseDoc?["code"]?.ToString() == "200" &&
                   responseRevue?["code"]?.ToString() == "200";
        }

        /// <summary>
        /// Modifie les informations d'une revue dans la base de données en mettant à jour les tables document et revue.
        /// </summary>
        /// <param name="revue">Objet Revue modifié</param>
        /// <returns>True si la modification réussit pour toutes les tables, sinon false</returns>
        public bool ModifierRevue(Revue revue)
        {
            // Préparer les données pour la table "document"
            var docData = new Dictionary<string, string>()
            {
                { "id", revue.Id },
                { "titre", revue.Titre },
                { "image", revue.Image },
                { "idGenre", revue.IdGenre },
                { "idPublic", revue.IdPublic },
                { "idRayon", revue.IdRayon }
            };
            string docParams = $"champs={Uri.EscapeDataString(JsonConvert.SerializeObject(docData))}";

            // Préparer les données pour la table "revue"
            var revueData = new Dictionary<string, string>()
            {
                { "id", revue.Id },
                { "periodicite", revue.Periodicite },
                { "delaiMiseADispo", revue.DelaiMiseADispo.ToString() }
            };
            string revueParams = $"champs={Uri.EscapeDataString(JsonConvert.SerializeObject(revueData))}";

            // Envoi de la requête PUT avec l'ID dans l'URL pour chaque table
            JObject responseDoc = api.RecupDistant("PUT", "document/" + revue.Id, docParams);
            JObject responseRevue = api.RecupDistant("PUT", "revue/" + revue.Id, revueParams);

            return responseDoc?["code"]?.ToString() == "200" &&
                   responseRevue?["code"]?.ToString() == "200";
        }

        /// <summary>
        /// Supprime une revue en supprimant les enregistrements dans les tables revue et document.
        /// </summary>
        /// <param name="idRevue">Identifiant de la revue à supprimer</param>
        /// <returns>True si toutes les suppressions réussissent, sinon false</returns>
        public bool SupprimerRevue(string idRevue)
        {
            string jsonData = JsonConvert.SerializeObject(new { id = idRevue });
            string param = $"champs={Uri.EscapeDataString(jsonData)}";

            JObject responseRevue = api.RecupDistant("DELETE", "revue", param);
            if (responseRevue == null || responseRevue["code"]?.ToString() != "200")
            {
                MessageBox.Show("❌ Suppression dans la table 'revue' échouée.\nRéponse invalide ou nulle.");
                return false;
            }

            JObject responseDoc = api.RecupDistant("DELETE", "document", param);
            if (responseDoc == null || responseDoc["code"]?.ToString() != "200")
            {
                MessageBox.Show("❌ Suppression dans la table 'document' échouée.\nRéponse invalide ou nulle.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Récupère la liste des commandes associées à un livre.
        /// </summary>
        /// <param name="idLivre">Identifiant du livre</param>
        /// <returns>Liste d'objets CommandeDocument liés au livre</returns>
        public List<CommandeDocument> GetCommandesByLivre(string idLivre)
        {
            return access.GetCommandesByLivre(idLivre);
        }

        /// <summary>
        /// Ajoute une commande pour un document.
        /// </summary>
        /// <param name="commande">Objet CommandeDocument à ajouter</param>
        /// <returns>True si la commande a été ajoutée, sinon false</returns>
        public bool AjouterCommande(CommandeDocument commande)
        {
            return access.AjouterCommande(commande);
        }

        /// <summary>
        /// Modifie l'état de suivi d'une commande.
        /// </summary>
        /// <param name="commande">Commande à modifier</param>
        /// <param name="nouvelIdSuivi">Nouvel identifiant de suivi</param>
        /// <returns>True si la modification est réussie, sinon false</returns>
        public bool ModifierSuiviCommande(CommandeDocument commande, int nouvelIdSuivi)
        {
            return access.ModifierSuiviCommande(commande, nouvelIdSuivi);
        }

        /// <summary>
        /// Supprime une commande donnée.
        /// </summary>
        /// <param name="idCommande">Identifiant de la commande à supprimer</param>
        /// <returns>True si la suppression est réussie, sinon false</returns>
        public bool SupprimerCommande(string idCommande)
        {
            return access.SupprimerCommande(idCommande);
        }

        /// <summary>
        /// Récupère la liste des commandes associées à un DVD.
        /// </summary>
        /// <param name="idDvd">Identifiant du DVD</param>
        /// <returns>Liste d'objets CommandeDocument liés au DVD</returns>
        public List<CommandeDocument> GetCommandesByDvd(string idDvd)
        {
            return access.GetCommandesByDvd(idDvd);
        }

        /// <summary>
        /// Récupère la liste des abonnements associés à une revue.
        /// </summary>
        /// <param name="idRevue">Identifiant de la revue</param>
        /// <returns>Liste d'objets Abonnement liés à la revue</returns>
        public List<Abonnement> GetAbonnementsByRevue(string idRevue)
        {
            return access.GetAbonnementsByRevue(idRevue);
        }

        /// <summary>
        /// Ajoute un abonnement pour une revue.
        /// </summary>
        /// <param name="abonnement">Objet Abonnement à ajouter</param>
        /// <returns>True si l'ajout est réussi, sinon false</returns>
        public bool AjouterAbonnement(Abonnement abonnement)
        {
            return access.AjouterAbonnement(abonnement);
        }

        /// <summary>
        /// Supprime un abonnement donné.
        /// </summary>
        /// <param name="idAbonnement">Identifiant de l'abonnement à supprimer</param>
        /// <returns>True si la suppression est réussie, sinon false</returns>
        public bool SupprimerAbonnement(string idAbonnement)
        {
            return access.SupprimerAbonnement(idAbonnement);
        }

        /// <summary>
        /// Récupère les exemplaires d'une revue à partir de son identifiant.
        /// </summary>
        /// <param name="idRevue">Identifiant de la revue</param>
        /// <returns>Liste d'objets Exemplaire associés à la revue</returns>
        public List<Exemplaire> GetExemplairesByRevue(string idRevue)
        {
            return access.GetExemplairesByRevue(idRevue);
        }

        /// <summary>
        /// Récupère la liste des abonnements arrivant à échéance dans les 30 prochains jours.
        /// </summary>
        /// <returns>Liste d'objets Abonnement triés par date de fin</returns>
        public List<Abonnement> GetAbonnementsEcheantDans30Jours()
        {
            return access.GetAbonnementsEcheantDans30Jours();
        }

        /// <summary>
        /// Récupère les exemplaires d'un livre à partir de son identifiant.
        /// </summary>
        /// <param name="idDocument">Identifiant du livre</param>
        /// <returns>Liste d'objets Exemplaire associés au livre</returns>
        public List<Exemplaire> GetExemplairesByLivre(string idDocument)
        {
            return access.GetExemplairesByLivre(idDocument);
        }

        /// <summary>
        /// Demande à l'API de changer l'état d'un exemplaire.
        /// </summary>
        /// <param name="idDocument">ID du document</param>
        /// <param name="numero">Numéro de l'exemplaire</param>
        /// <param name="idEtat">Nouveau ID d'état</param>
        /// <returns>True si la modification est réussie, sinon false</returns>
        public bool ModifierEtatExemplaire(string idDocument, int numero, string idEtat)
        {
            return access.ModifierEtatExemplaire(idDocument, numero, idEtat);
        }

        /// <summary>
        /// Récupère la liste de tous les états disponibles.
        /// </summary>
        /// <returns>Liste d'objets Etat</returns>
        public List<Etat> GetAllEtats()
        {
            return access.GetAllEtats();
        }

        /// <summary>
        /// Supprime un exemplaire identifié par le document et le numéro d'exemplaire.
        /// </summary>
        /// <param name="idDocument">Identifiant du document</param>
        /// <param name="numero">Numéro de l'exemplaire à supprimer</param>
        /// <returns>True si la suppression est réussie, sinon false</returns>
        public bool SupprimerExemplaire(string idDocument, int numero)
        {
            return access.SupprimerExemplaire(idDocument, numero);
        }

        /// <summary>
        /// Récupère les exemplaires d'un DVD à partir de son identifiant.
        /// </summary>
        /// <param name="idDocument">Identifiant du DVD</param>
        /// <returns>Liste d'objets Exemplaire associés au DVD</returns>
        public List<Exemplaire> GetExemplairesByDvd(string idDocument)
        {
            return access.GetExemplairesByDvd(idDocument);
        }

        /// <summary>
        /// Authentifie un utilisateur à partir de son login et mot de passe.
        /// </summary>
        /// <param name="login">Login de l'utilisateur</param>
        /// <param name="password">Mot de passe de l'utilisateur</param>
        /// <returns>Objet Utilisateur si trouvé, sinon null</returns>
        public Utilisateur AuthentifierUtilisateur(string login, string password)
        {
            return access.AuthentifierUtilisateur(login, password);
        }
    }
}
