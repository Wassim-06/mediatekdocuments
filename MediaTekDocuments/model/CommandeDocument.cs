using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Représente une commande liée à un document (livre ou DVD).
    /// </summary>
    public class CommandeDocument
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
        /// Obtient ou définit le nombre d'exemplaires commandés.
        /// </summary>
        public int NbExemplaire { get; set; }

        /// <summary>
        /// Obtient ou définit l'identifiant du suivi de la commande.
        /// </summary>
        public int IdSuivi { get; set; }

        /// <summary>
        /// Obtient ou définit le libellé correspondant au suivi.
        /// </summary>
        public string LibelleSuivi { get; set; }

        /// <summary>
        /// Obtient ou définit l'identifiant du document (livre ou DVD) associé.
        /// </summary>
        public string IdLivreDvd { get; set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="CommandeDocument"/>.
        /// </summary>
        /// <param name="id">Identifiant de la commande.</param>
        /// <param name="dateCommande">Date de la commande.</param>
        /// <param name="montant">Montant total de la commande.</param>
        /// <param name="nbExemplaire">Nombre d'exemplaires commandés.</param>
        /// <param name="idSuivi">Identifiant du suivi de la commande.</param>
        /// <param name="libelleSuivi">Libellé du suivi.</param>
        /// <param name="idLivreDvd">Identifiant du document associé (livre ou DVD).</param>
        public CommandeDocument(string id, DateTime dateCommande, double montant, int nbExemplaire, int idSuivi, string libelleSuivi, string idLivreDvd)
        {
            Id = id;
            DateCommande = dateCommande;
            Montant = montant;
            NbExemplaire = nbExemplaire;
            IdSuivi = idSuivi;
            LibelleSuivi = libelleSuivi;
            IdLivreDvd = idLivreDvd;
        }
    }
}
