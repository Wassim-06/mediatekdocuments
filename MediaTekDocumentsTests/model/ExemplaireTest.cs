using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocumentsTest.model
{
    [TestClass]
    public class ExemplaireTest
    {
        [TestMethod]
        public void TestExemplaireCreation()
        {
            // Arrange
            int numero = 1;
            DateTime dateAchat = new DateTime(2025, 4, 4);
            string photo = "photo.jpg";
            string idEtat = "E01";
            string idDocument = "D001";

            // Act
            Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);

            // Assert
            Assert.AreEqual(numero, exemplaire.Numero);
            Assert.AreEqual(dateAchat, exemplaire.DateAchat);
            Assert.AreEqual(photo, exemplaire.Photo);
            Assert.AreEqual(idEtat, exemplaire.IdEtat);
            Assert.AreEqual(idDocument, exemplaire.Id);
        }
    }
}
