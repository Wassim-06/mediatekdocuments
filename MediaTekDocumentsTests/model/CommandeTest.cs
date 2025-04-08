using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocumentsTest.model
{
    [TestClass]
    public class CommandeTest
    {
        [TestMethod]
        public void TestCommandeCreation()
        {
            // Arrange
            string id = "CMD001";
            DateTime dateCommande = new DateTime(2025, 4, 4);
            double montant = 150.50;

            // Act
            Commande commande = new Commande(id, dateCommande, montant);

            // Assert
            Assert.AreEqual(id, commande.Id);
            Assert.AreEqual(dateCommande, commande.DateCommande);
            Assert.AreEqual(montant, commande.Montant);
        }
    }
}
