using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocumentsTest.model
{
    [TestClass]
    public class CommandeDocumentTest
    {
        [TestMethod]
        public void TestCommandeDocumentCreation()
        {
            // Arrange
            string id = "CMD002";
            DateTime dateCommande = new DateTime(2025, 4, 5);
            double montant = 200.75;
            int nbExemplaire = 3;
            int idSuivi = 1;
            string libelleSuivi = "En cours";
            string idLivreDvd = "LD123";

            // Act
            CommandeDocument commandeDocument = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idSuivi, libelleSuivi, idLivreDvd);

            // Assert
            Assert.AreEqual(id, commandeDocument.Id);
            Assert.AreEqual(dateCommande, commandeDocument.DateCommande);
            Assert.AreEqual(montant, commandeDocument.Montant);
            Assert.AreEqual(nbExemplaire, commandeDocument.NbExemplaire);
            Assert.AreEqual(idSuivi, commandeDocument.IdSuivi);
            Assert.AreEqual(libelleSuivi, commandeDocument.LibelleSuivi);
            Assert.AreEqual(idLivreDvd, commandeDocument.IdLivreDvd);
        }
    }
}
