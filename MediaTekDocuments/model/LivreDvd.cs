namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe abstraite représentant les informations communes aux livres et aux DVD.
    /// Hérite de <see cref="Document"/>.
    /// </summary>
    public abstract class LivreDvd : Document
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="LivreDvd"/>.
        /// </summary>
        /// <param name="id">Identifiant du document.</param>
        /// <param name="titre">Titre du document.</param>
        /// <param name="image">Chemin ou URL de l'image du document.</param>
        /// <param name="idGenre">Identifiant du genre.</param>
        /// <param name="genre">Libellé du genre.</param>
        /// <param name="idPublic">Identifiant du public cible.</param>
        /// <param name="lePublic">Libellé du public cible.</param>
        /// <param name="idRayon">Identifiant du rayon.</param>
        /// <param name="rayon">Libellé du rayon.</param>
        protected LivreDvd(string id, string titre, string image, string idGenre, string genre,
            string idPublic, string lePublic, string idRayon, string rayon)
            : base(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon)
        {
        }
    }
}
