using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocumentsTest.model
{
    [TestClass]
    public class CategorieTest
    {
        [TestMethod]
        public void TestCategorieCreation()
        {
            // Arrange
            string id = "GEN001";
            string libelle = "Science Fiction";

            // Act
            Categorie categorie = new Categorie(id, libelle);

            // Assert
            Assert.AreEqual(id, categorie.Id);
            Assert.AreEqual(libelle, categorie.Libelle);
        }

        [TestMethod]
        public void TestCategorieToString()
        {
            // Arrange
            string libelle = "Fantastique";
            Categorie categorie = new Categorie("GEN002", libelle);

            // Act
            string result = categorie.ToString();

            // Assert
            Assert.AreEqual(libelle, result);
        }
    }
}
