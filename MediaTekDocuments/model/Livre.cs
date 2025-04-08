namespace MediaTekDocuments.model
{
    /// <summary>
    /// Représente un livre avec ses propriétés spécifiques. Hérite de <see cref="LivreDvd"/>.
    /// </summary>
    public class Livre : LivreDvd
    {
        /// <summary>
        /// Obtient le numéro ISBN du livre.
        /// </summary>
        public string Isbn { get; }

        /// <summary>
        /// Obtient le nom de l'auteur du livre.
        /// </summary>
        public string Auteur { get; }

        /// <summary>
        /// Obtient la collection à laquelle appartient le livre.
        /// </summary>
        public string Collection { get; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Livre"/>.
        /// </summary>
        /// <param name="id">Identifiant du livre.</param>
        /// <param name="titre">Titre du livre.</param>
        /// <param name="image">Chemin ou URL de l'image du livre.</param>
        /// <param name="isbn">Numéro ISBN.</param>
        /// <param name="auteur">Nom de l'auteur.</param>
        /// <param name="collection">Nom de la collection.</param>
        /// <param name="idGenre">Identifiant du genre.</param>
        /// <param name="genre">Libellé du genre.</param>
        /// <param name="idPublic">Identifiant du public cible.</param>
        /// <param name="lePublic">Libellé du public cible.</param>
        /// <param name="idRayon">Identifiant du rayon.</param>
        /// <param name="rayon">Libellé du rayon.</param>
        public Livre(string id, string titre, string image, string isbn, string auteur, string collection,
            string idGenre, string genre, string idPublic, string lePublic, string idRayon, string rayon)
            : base(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon)
        {
            this.Isbn = isbn;
            this.Auteur = auteur;
            this.Collection = collection;
        }
    }
}
