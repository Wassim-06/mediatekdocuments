using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocumentsTest.model
{
    [TestClass]
    public class DvdTest
    {
        [TestMethod]
        public void TestDvdCreation()
        {
            // Arrange
            string id = "DVD001";
            string titre = "Film Incroyable";
            string image = "film.jpg";
            int duree = 120;
            string realisateur = "Christopher Nolan";
            string synopsis = "Un film époustouflant.";
            string idGenre = "G02";
            string genre = "Action";
            string idPublic = "P02";
            string lePublic = "Tout public";
            string idRayon = "R02";
            string rayon = "Cinéma";

            // Act
            Dvd dvd = new Dvd(id, titre, image, duree, realisateur, synopsis, idGenre, genre, idPublic, lePublic, idRayon, rayon);

            // Assert
            Assert.AreEqual(id, dvd.Id);
            Assert.AreEqual(titre, dvd.Titre);
            Assert.AreEqual(image, dvd.Image);
            Assert.AreEqual(duree, dvd.Duree);
            Assert.AreEqual(realisateur, dvd.Realisateur);
            Assert.AreEqual(synopsis, dvd.Synopsis);
            Assert.AreEqual(idGenre, dvd.IdGenre);
            Assert.AreEqual(genre, dvd.Genre);
            Assert.AreEqual(idPublic, dvd.IdPublic);
            Assert.AreEqual(lePublic, dvd.Public);
            Assert.AreEqual(idRayon, dvd.IdRayon);
            Assert.AreEqual(rayon, dvd.Rayon);
        }
    }
}
