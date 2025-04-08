using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model; // 📚 On importe ton projet
using System;

namespace MediaTekDocumentsTest
{
    [TestClass]
    public class LivreTest
    {
        [TestMethod]
        public void Livre_Construction_AssigneValeursCorrectement()
        {
            // Arrange
            Livre livre = new Livre(
                "0001",
                "Le titre",
                "image.png",
                "isbn-123",
                "Auteur X",
                "Collection Y",
                "idGenre1",
                "Genre X",
                "idPublic1",
                "Public Y",
                "idRayon1",
                "Rayon Z"
            );

            // Assert
            Assert.AreEqual("0001", livre.Id);
            Assert.AreEqual("Le titre", livre.Titre);
            Assert.AreEqual("isbn-123", livre.Isbn);
            Assert.AreEqual("Auteur X", livre.Auteur);
            Assert.AreEqual("Collection Y", livre.Collection);
            Assert.AreEqual("idGenre1", livre.IdGenre);
            Assert.AreEqual("Genre X", livre.Genre);
            Assert.AreEqual("idPublic1", livre.IdPublic);
            Assert.AreEqual("Public Y", livre.Public);
            Assert.AreEqual("idRayon1", livre.IdRayon);
            Assert.AreEqual("Rayon Z", livre.Rayon);
        }
    }
}
