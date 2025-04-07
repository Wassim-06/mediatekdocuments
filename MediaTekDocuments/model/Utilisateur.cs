using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Utilisateur
    /// </summary>
    public class Utilisateur
    {
        public string Id { get; }
        public string Nom { get; }
        public string Prenom { get; }
        public string Service { get; }

        public Utilisateur(string id, string nom, string prenom, string service)
        {
            this.Id = id;
            this.Nom = nom;
            this.Prenom = prenom;
            this.Service = service;
        }
    }
}

