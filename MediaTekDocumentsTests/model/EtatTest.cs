using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocumentsTest.model
{
    [TestClass]
    public class EtatTest
    {
        [TestMethod]
        public void TestEtatCreation()
        {
            // Arrange
            string id = "E01";
            string libelle = "Neuf";

            // Act
            Etat etat = new Etat(id, libelle);

            // Assert
            Assert.AreEqual(id, etat.Id);
            Assert.AreEqual(libelle, etat.Libelle);
        }
    }
}
