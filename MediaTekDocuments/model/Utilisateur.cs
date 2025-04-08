using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Représente un utilisateur du système.
    /// </summary>
    public class Utilisateur
    {
        /// <summary>
        /// Obtient l'identifiant de l'utilisateur.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Obtient le nom de l'utilisateur.
        /// </summary>
        public string Nom { get; }

        /// <summary>
        /// Obtient le prénom de l'utilisateur.
        /// </summary>
        public string Prenom { get; }

        /// <summary>
        /// Obtient le service auquel appartient l'utilisateur.
        /// </summary>
        public string Service { get; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Utilisateur"/>.
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur.</param>
        /// <param name="nom">Nom de l'utilisateur.</param>
        /// <param name="prenom">Prénom de l'utilisateur.</param>
        /// <param name="service">Service de l'utilisateur.</param>
        public Utilisateur(string id, string nom, string prenom, string service)
        {
            this.Id = id;
            this.Nom = nom;
            this.Prenom = prenom;
            this.Service = service;
        }
    }
}
