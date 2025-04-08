using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocumentsTest.model
{
    [TestClass]
    public class UtilisateurTest
    {
        [TestMethod]
        public void TestUtilisateurCreation()
        {
            // Arrange
            string id = "U001";
            string nom = "Dupont";
            string prenom = "Jean";
            string service = "Commande";

            // Act
            Utilisateur utilisateur = new Utilisateur(id, nom, prenom, service);

            // Assert
            Assert.AreEqual(id, utilisateur.Id);
            Assert.AreEqual(nom, utilisateur.Nom);
            Assert.AreEqual(prenom, utilisateur.Prenom);
            Assert.AreEqual(service, utilisateur.Service);
        }
    }
}
