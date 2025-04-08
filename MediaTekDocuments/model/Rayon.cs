namespace MediaTekDocuments.model
{
    /// <summary>
    /// Représente un rayon de classement pour un document. Hérite de <see cref="Categorie"/>.
    /// </summary>
    public class Rayon : Categorie
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Rayon"/>.
        /// </summary>
        /// <param name="id">Identifiant du rayon.</param>
        /// <param name="libelle">Libellé du rayon.</param>
        public Rayon(string id, string libelle) : base(id, libelle)
        {
        }
    }
}
