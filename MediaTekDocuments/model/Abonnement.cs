using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    public class Abonnement
    {
        public string Id { get; set; }
        public DateTime DateCommande { get; set; }
        public DateTime DateFinAbonnement { get; set; }
        public double Montant { get; set; }
        public string IdRevue { get; set; }

        public Abonnement(string id, DateTime dateCommande, DateTime dateFinAbonnement, double montant, string idRevue)
        {
            Id = id;
            DateCommande = dateCommande;
            DateFinAbonnement = dateFinAbonnement;
            Montant = montant;
            IdRevue = idRevue;
        }
    }
}
