using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocumentsTest.model
{
    [TestClass]
    public class RevueTest
    {
        [TestMethod]
        public void TestRevueCreation()
        {
            // Arrange
            string id = "R001";
            string titre = "Magazine Exemple";
            string image = "image_revue.jpg";
            string idGenre = "G001";
            string genre = "Culture";
            string idPublic = "P001";
            string lePublic = "Adulte";
            string idRayon = "RY001";
            string rayon = "Magazines";
            string periodicite = "Mensuel";
            int delaiMiseADispo = 5;

            // Act
            Revue revue = new Revue(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon, periodicite, delaiMiseADispo);

            // Assert
            Assert.AreEqual(id, revue.Id);
            Assert.AreEqual(titre, revue.Titre);
            Assert.AreEqual(image, revue.Image);
            Assert.AreEqual(idGenre, revue.IdGenre);
            Assert.AreEqual(genre, revue.Genre);
            Assert.AreEqual(idPublic, revue.IdPublic);
            Assert.AreEqual(lePublic, revue.Public);
            Assert.AreEqual(idRayon, revue.IdRayon);
            Assert.AreEqual(rayon, revue.Rayon);
            Assert.AreEqual(periodicite, revue.Periodicite);
            Assert.AreEqual(delaiMiseADispo, revue.DelaiMiseADispo);
        }
    }
}
