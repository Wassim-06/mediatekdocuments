namespace MediaTekDocuments.model
{
    /// <summary>
    /// Représente un genre. Hérite de <see cref="Categorie"/>.
    /// </summary>
    public class Genre : Categorie
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Genre"/>.
        /// </summary>
        /// <param name="id">Identifiant du genre.</param>
        /// <param name="libelle">Libellé du genre.</param>
        public Genre(string id, string libelle) : base(id, libelle)
        {
        }
    }
}
