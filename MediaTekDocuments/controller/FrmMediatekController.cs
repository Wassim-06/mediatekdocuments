using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MediaTekDocuments.manager;
using System.Windows.Forms;
using System.Linq;
using System;

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




    }
}
