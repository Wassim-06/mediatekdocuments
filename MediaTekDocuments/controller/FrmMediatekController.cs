using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MediaTekDocuments.manager;
using System;
using System.Windows.Forms;

namespace MediaTekDocuments.controller
{
    /// <summary>
    /// Contrôleur lié à FrmMediatek
    /// </summary>
    class FrmMediatekController
    {

        /// <summary>
        /// Objet d'accès aux données
        /// </summary>
        private readonly Access access;
        private readonly ApiRest api = ApiRest.GetInstance("http://localhost/rest_mediatekdocuments/", "admin:adminpwd");

        /// <summary>
        /// Récupération de l'instance unique d'accès aux données
        /// </summary>
        public FrmMediatekController()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// getter sur la liste des genres
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return access.GetAllGenres();
        }

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return access.GetAllLivres();
        }

        /// <summary>
        /// getter sur la liste des Dvd
        /// </summary>
        /// <returns>Liste d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return access.GetAllDvd();
        }

        /// <summary>
        /// getter sur la liste des revues
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return access.GetAllRevues();
        }

        /// <summary>
        /// getter sur les rayons
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return access.GetAllRayons();
        }

        /// <summary>
        /// getter sur les publics
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return access.GetAllPublics();
        }


        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocuement">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocuement)
        {
            return access.GetExemplairesRevue(idDocuement);
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return access.CreerExemplaire(exemplaire);
        }
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

        public bool PeutSupprimerDocument(string idDocument)
        {
            return access.PeutSupprimerDocument(idDocument);
        }

        public bool SupprimerLivre(string idDocument)
        {
            string jsonData = JsonConvert.SerializeObject(new { id = idDocument });
            string param = $"champs={Uri.EscapeDataString(jsonData)}";

            JObject responseLivre = api.RecupDistant("DELETE", "livre", param);
            if (responseLivre?["code"]?.ToString() != "200")
            {
                MessageBox.Show("❌ Suppression dans la table 'livre' échouée.\n" + responseLivre.ToString());
                return false;
            }

            JObject responseLivreDvd = api.RecupDistant("DELETE", "livres_dvd", param);
            if (responseLivreDvd?["code"]?.ToString() != "200")
            {
                MessageBox.Show("❌ Suppression dans la table 'livres_dvd' échouée.\n" + responseLivreDvd.ToString());
                return false;
            }

            JObject responseDocument = api.RecupDistant("DELETE", "document", param);
            if (responseDocument?["code"]?.ToString() != "200")
            {
                MessageBox.Show("❌ Suppression dans la table 'document' échouée.\n" + responseDocument.ToString());
                return false;
            }

            return true;
        }

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

            // Appel à l'API en utilisant PUT
            JObject responseDoc = api.RecupDistant("PUT", "document/" + livre.Id, docParams);
            JObject responseLivreDvd = api.RecupDistant("PUT", "livres_dvd/" + livre.Id, livreDvdParams);
            JObject responseLivre = api.RecupDistant("PUT", "livre/" + livre.Id, livreParams);

            return responseDoc?["code"]?.ToString() == "200" &&
                   responseLivreDvd?["code"]?.ToString() == "200" &&
                   responseLivre?["code"]?.ToString() == "200";
        }


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

        public bool SupprimerDVD(string idDVD)
        {
            string jsonData = JsonConvert.SerializeObject(new { id = idDVD });
            string param = $"champs={Uri.EscapeDataString(jsonData)}";

            // En suivant la documentation, on peut aussi transmettre l'ID dans l'URL
            JObject responseDvd = api.RecupDistant("DELETE", "dvd", param);
            JObject responseDvdLivre = api.RecupDistant("DELETE", "livres_dvd", param);
            JObject responseDoc = api.RecupDistant("DELETE", "document", param);

            if (responseDvd?["code"]?.ToString() != "200")
            {
                MessageBox.Show("❌ Suppression dans la table 'dvd' échouée.\n" + responseDvd.ToString());
                return false;
            }
            if (responseDvdLivre?["code"]?.ToString() != "200")
            {
                MessageBox.Show("❌ Suppression dans la table 'livres_dvd' échouée.\n" + responseDvdLivre.ToString());
                return false;
            }
            if (responseDoc?["code"]?.ToString() != "200")
            {
                MessageBox.Show("❌ Suppression dans la table 'document' échouée.\n" + responseDoc.ToString());
                return false;
            }
            return true;
        }

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
                // Si tu n'as pas de date de parution, omets-la ou ajoute-la si nécessaire.
            };
            string revueParams = $"champs={Uri.EscapeDataString(JsonConvert.SerializeObject(revueData))}";


            // Envoi de la requête POST sur "document" et sur "revue"
            JObject responseDoc = api.RecupDistant("POST", "document", docParams);
            JObject responseRevue = api.RecupDistant("POST", "revue", revueParams);

            return responseDoc?["code"]?.ToString() == "200" &&
                   responseRevue?["code"]?.ToString() == "200";
        }

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


        public bool SupprimerRevue(string idRevue)
        {
            string jsonData = JsonConvert.SerializeObject(new { id = idRevue });
            string param = $"champs={Uri.EscapeDataString(jsonData)}";

            JObject responseRevue = api.RecupDistant("DELETE", "revue", param);
            JObject responseDoc = api.RecupDistant("DELETE", "document", param);

            if (responseRevue?["code"]?.ToString() != "200")
            {
                MessageBox.Show("❌ Suppression dans la table 'revue' échouée.\n" + responseRevue.ToString());
                return false;
            }
            if (responseDoc?["code"]?.ToString() != "200")
            {
                MessageBox.Show("❌ Suppression dans la table 'document' échouée.\n" + responseDoc.ToString());
                return false;
            }
            return true;
        }


        public List<CommandeDocument> GetCommandesByLivre(string idLivre)
        {
            return access.GetCommandesByLivre(idLivre);
        }

        public bool AjouterCommande(CommandeDocument commande)
        {
            return access.AjouterCommande(commande);
        }

        public bool ModifierSuiviCommande(CommandeDocument commande, int nouvelIdSuivi)
        {
            return access.ModifierSuiviCommande(commande, nouvelIdSuivi);
        }

        public bool SupprimerCommande(string idCommande)
        {
            return access.SupprimerCommande(idCommande);
        }

        public List<CommandeDocument> GetCommandesByDvd(string idDvd)
        {
            return access.GetCommandesByDvd(idDvd);
        }


    }
}
