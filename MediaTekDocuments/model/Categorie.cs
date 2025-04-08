namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier <c>Categorie</c> (réunit les informations des classes Public, Genre et Rayon).
    /// </summary>
    public class Categorie
    {
        /// <summary>
        /// Obtient l'identifiant de la catégorie.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Obtient le libellé de la catégorie.
        /// </summary>
        public string Libelle { get; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Categorie"/>.
        /// </summary>
        /// <param name="id">Identifiant de la catégorie.</param>
        /// <param name="libelle">Libellé de la catégorie.</param>
        public Categorie(string id, string libelle)
        {
            this.Id = id;
            this.Libelle = libelle;
        }

        /// <summary>
        /// Retourne le libellé pour l'affichage, par exemple dans les listes déroulantes.
        /// </summary>
        /// <returns>Le libellé de la catégorie.</returns>
        public override string ToString()
        {
            return this.Libelle;
        }
    }
}
