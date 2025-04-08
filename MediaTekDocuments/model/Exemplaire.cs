using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Représente un exemplaire d'une revue.
    /// </summary>
    public class Exemplaire
    {
        /// <summary>
        /// Obtient ou définit le numéro de l'exemplaire.
        /// </summary>
        public int Numero { get; set; }

        /// <summary>
        /// Obtient ou définit le chemin ou l'URL de la photo associée à l'exemplaire.
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// Obtient ou définit la date d'achat de l'exemplaire.
        /// </summary>
        public DateTime DateAchat { get; set; }

        /// <summary>
        /// Obtient ou définit l'identifiant de l'état de l'exemplaire.
        /// </summary>
        public string IdEtat { get; set; }

        /// <summary>
        /// Obtient ou définit l'identifiant du document auquel appartient l'exemplaire.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Exemplaire"/>.
        /// </summary>
        /// <param name="numero">Numéro de l'exemplaire.</param>
        /// <param name="dateAchat">Date d'achat de l'exemplaire.</param>
        /// <param name="photo">Chemin ou URL de la photo.</param>
        /// <param name="idEtat">Identifiant de l'état de l'exemplaire.</param>
        /// <param name="idDocument">Identifiant du document associé.</param>
        public Exemplaire(int numero, DateTime dateAchat, string photo, string idEtat, string idDocument)
        {
            this.Numero = numero;
            this.DateAchat = dateAchat;
            this.Photo = photo;
            this.IdEtat = idEtat;
            this.Id = idDocument;
        }
    }
}
