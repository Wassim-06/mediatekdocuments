using System;

namespace MediaTekDocuments.model
{
    public class CommandeDocument
    {
        public string Id { get; set; }
        public DateTime DateCommande { get; set; }
        public double Montant { get; set; }
        public int NbExemplaire { get; set; }
        public int IdSuivi { get; set; }
        public string LibelleSuivi { get; set; }
        public string IdLivreDvd { get; set; } // <-- Ajouté pour identifier le document (livre ou DVD)

        public CommandeDocument(string id, DateTime dateCommande, double montant, int nbExemplaire, int idSuivi, string libelleSuivi, string idLivreDvd)
        {
            Id = id;
            DateCommande = dateCommande;
            Montant = montant;
            NbExemplaire = nbExemplaire;
            IdSuivi = idSuivi;
            LibelleSuivi = libelleSuivi;
            IdLivreDvd = idLivreDvd;
        }
    }
}
