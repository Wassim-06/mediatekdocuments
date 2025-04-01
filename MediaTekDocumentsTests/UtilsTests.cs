using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MediaTekDocuments.dal;
using MediaTekDocuments.utils;

namespace MediaTekDocumentsTests
{
    [TestClass]
    public class UtilsTests
    {
        [TestMethod]
        public void ParutionDansAbonnement_DateDansPlage_RetourneVrai()
        {
            DateTime dateCommande = new DateTime(2024, 1, 1);
            DateTime dateFin = new DateTime(2024, 12, 31);
            DateTime dateParution = new DateTime(2024, 6, 15);

            bool resultat = Utils.ParutionDansAbonnement(dateCommande, dateFin, dateParution);

            Assert.IsTrue(resultat);
        }

        [TestMethod]
        public void ParutionDansAbonnement_DateAvantCommande_RetourneFaux()
        {
            DateTime dateCommande = new DateTime(2024, 1, 1);
            DateTime dateFin = new DateTime(2024, 12, 31);
            DateTime dateParution = new DateTime(2023, 12, 31);

            bool resultat = Utils.ParutionDansAbonnement(dateCommande, dateFin, dateParution);

            Assert.IsFalse(resultat);
        }

        [TestMethod]
        public void ParutionDansAbonnement_DateApresFin_RetourneFaux()
        {
            DateTime dateCommande = new DateTime(2024, 1, 1);
            DateTime dateFin = new DateTime(2024, 12, 31);
            DateTime dateParution = new DateTime(2025, 1, 1);

            bool resultat = Utils.ParutionDansAbonnement(dateCommande, dateFin, dateParution);

            Assert.IsFalse(resultat);
        }
    }
}
