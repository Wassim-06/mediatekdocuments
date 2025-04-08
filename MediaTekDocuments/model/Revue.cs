namespace MediaTekDocuments.model
{
    /// <summary>
    /// Représente une revue avec ses propriétés spécifiques. Hérite de <see cref="Document"/>.
    /// </summary>
    public class Revue : Document
    {
        /// <summary>
        /// Obtient ou définit la périodicité de la revue.
        /// </summary>
        public string Periodicite { get; set; }

        /// <summary>
        /// Obtient ou définit le délai de mise à disposition de la revue.
        /// </summary>
        public int DelaiMiseADispo { get; set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Revue"/>.
        /// </summary>
        /// <param name="id">Identifiant de la revue.</param>
        /// <param name="titre">Titre de la revue.</param>
        /// <param name="image">Chemin ou URL de l'image de la revue.</param>
        /// <param name="idGenre">Identifiant du genre.</param>
        /// <param name="genre">Libellé du genre.</param>
        /// <param name="idPublic">Identifiant du public cible.</param>
        /// <param name="lePublic">Libellé du public cible.</param>
        /// <param name="idRayon">Identifiant du rayon.</param>
        /// <param name="rayon">Libellé du rayon.</param>
        /// <param name="periodicite">Périodicité de la revue.</param>
        /// <param name="delaiMiseADispo">Délai de mise à disposition.</param>
        public Revue(string id, string titre, string image, string idGenre, string genre,
            string idPublic, string lePublic, string idRayon, string rayon,
            string periodicite, int delaiMiseADispo)
            : base(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon)
        {
            Periodicite = periodicite;
            DelaiMiseADispo = delaiMiseADispo;
        }
    }
}
