namespace MediaTekDocuments.model
{
    /// <summary>
    /// Représente le public concerné par un document. Hérite de <see cref="Categorie"/>.
    /// </summary>
    public class Public : Categorie
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Public"/>.
        /// </summary>
        /// <param name="id">Identifiant du public.</param>
        /// <param name="libelle">Libellé du public.</param>
        public Public(string id, string libelle) : base(id, libelle)
        {
        }
    }
}
