using System;

namespace MediaTekDocuments.utils
{
    public static class Utils
    {
        public static bool ParutionDansAbonnement(DateTime dateCommande, DateTime dateFinAbonnement, DateTime dateParution)
        {
            return dateParution >= dateCommande && dateParution <= dateFinAbonnement;
        }
    }
}
