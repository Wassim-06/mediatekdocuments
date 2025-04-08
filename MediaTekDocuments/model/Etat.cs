namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier <c>Etat</c> représentant l'état d'usure d'un document.
    /// </summary>
    public class Etat
    {
        /// <summary>
        /// Obtient ou définit l'identifiant de l'état.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Obtient ou définit le libellé de l'état.
        /// </summary>
        public string Libelle { get; set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Etat"/>.
        /// </summary>
        /// <param name="id">Identifiant de l'état.</param>
        /// <param name="libelle">Libellé de l'état.</param>
        public Etat(string id, string libelle)
        {
            this.Id = id;
            this.Libelle = libelle;
        }
    }
}
