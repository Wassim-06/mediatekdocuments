using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Représente une commande.
    /// </summary>
    public class Commande
    {
        /// <summary>
        /// Obtient ou définit l'identifiant de la commande.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Obtient ou définit la date à laquelle la commande a été passée.
        /// </summary>
        public DateTime DateCommande { get; set; }

        /// <summary>
        /// Obtient ou définit le montant total de la commande.
        /// </summary>
        public double Montant { get; set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Commande"/>.
        /// </summary>
        /// <param name="id">Identifiant de la commande.</param>
        /// <param name="dateCommande">Date de la commande.</param>
        /// <param name="montant">Montant total de la commande.</param>
        public Commande(string id, DateTime dateCommande, double montant)
        {
            Id = id;
            DateCommande = dateCommande;
            Montant = montant;
        }
    }
}
