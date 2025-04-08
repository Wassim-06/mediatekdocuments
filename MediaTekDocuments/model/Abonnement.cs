using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Représente un abonnement à une revue.
    /// </summary>
    public class Abonnement
    {
        /// <summary>
        /// Obtient ou définit l'identifiant de l'abonnement.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Obtient ou définit la date de la commande associée à l'abonnement.
        /// </summary>
        public DateTime DateCommande { get; set; }

        /// <summary>
        /// Obtient ou définit la date de fin de l'abonnement.
        /// </summary>
        public DateTime DateFinAbonnement { get; set; }

        /// <summary>
        /// Obtient ou définit le montant de la commande.
        /// </summary>
        public double Montant { get; set; }

        /// <summary>
        /// Obtient ou définit l'identifiant de la revue liée à l'abonnement.
        /// </summary>
        public string IdRevue { get; set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Abonnement"/>.
        /// </summary>
        /// <param name="id">Identifiant de l'abonnement.</param>
        /// <param name="dateCommande">Date de la commande.</param>
        /// <param name="dateFinAbonnement">Date de fin de l'abonnement.</param>
        /// <param name="montant">Montant de la commande.</param>
        /// <param name="idRevue">Identifiant de la revue associée.</param>
        public Abonnement(string id, DateTime dateCommande, DateTime dateFinAbonnement, double montant, string idRevue)
        {
            Id = id;
            DateCommande = dateCommande;
            DateFinAbonnement = dateFinAbonnement;
            Montant = montant;
            IdRevue = idRevue;
        }
    }
}
