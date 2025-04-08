using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocumentsTest.model
{
    [TestClass]
    public class DocumentTest
    {
        [TestMethod]
        public void TestDocumentCreation()
        {
            // Arrange
            string id = "DOC001";
            string titre = "Le Meilleur Document";
            string image = "image.jpg";
            string idGenre = "G01";
            string genre = "Science-fiction";
            string idPublic = "P01";
            string lePublic = "Adulte";
            string idRayon = "R01";
            string rayon = "Romans";

            // Act
            Document document = new Document(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon);

            // Assert
            Assert.AreEqual(id, document.Id);
            Assert.AreEqual(titre, document.Titre);
            Assert.AreEqual(image, document.Image);
            Assert.AreEqual(idGenre, document.IdGenre);
            Assert.AreEqual(genre, document.Genre);
            Assert.AreEqual(idPublic, document.IdPublic);
            Assert.AreEqual(lePublic, document.Public);
            Assert.AreEqual(idRayon, document.IdRayon);
            Assert.AreEqual(rayon, document.Rayon);
        }
    }
}
