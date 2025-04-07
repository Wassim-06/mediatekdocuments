using MediaTekDocuments.view;
using System;
using System.Windows.Forms;
using MediaTekDocuments.controller;
using MediaTekDocuments.model; // ⚡ Ajouter aussi le model pour Utilisateur

namespace MediaTekDocuments
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FrmMediatekController controller = new FrmMediatekController();
            FrmAuthentification frmAuth = new FrmAuthentification(controller);
            if (frmAuth.ShowDialog() == DialogResult.OK)
            {
                Utilisateur utilisateurConnecte = frmAuth.UtilisateurConnecte; // 💥 récupérer l'utilisateur connecté
                Application.Run(new FrmMediatek(controller, utilisateurConnecte)); // 💥 passer controller + utilisateur
            }
        }
    }
}
