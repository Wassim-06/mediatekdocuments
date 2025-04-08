using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MediaTekDocuments.model;

namespace MediaTekDocumentsTest.model
{
    [TestClass]
    public class AbonnementTest
    {
        [TestMethod]
        public void TestAbonnementCreation()
        {
            // Arrange
            string id = "1";
            DateTime dateCommande = new DateTime(2024, 1, 1);
            DateTime dateFinAbonnement = new DateTime(2025, 1, 1);
            double montant = 120.50;
            string idRevue = "REV123";

            // Act
            Abonnement abonnement = new Abonnement(id, dateCommande, dateFinAbonnement, montant, idRevue);

            // Assert
            Assert.AreEqual(id, abonnement.Id);
            Assert.AreEqual(dateCommande, abonnement.DateCommande);
            Assert.AreEqual(dateFinAbonnement, abonnement.DateFinAbonnement);
            Assert.AreEqual(montant, abonnement.Montant);
            Assert.AreEqual(idRevue, abonnement.IdRevue);
        }
    }
}
