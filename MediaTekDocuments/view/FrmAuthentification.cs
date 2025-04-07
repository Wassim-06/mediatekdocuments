using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaTekDocuments.model; // 🔥 Ajout pour Utilisateur
using MediaTekDocuments.controller; // 🔥 (quand on aura créé le controller)

namespace MediaTekDocuments.view
{
    public partial class FrmAuthentification : Form
    {
        private FrmMediatekController controller; // 💥 Ajouté
        private Utilisateur utilisateurConnecte = null; // 💥 Ajouté

        public FrmAuthentification(FrmMediatekController controller) // 💥 on passe le controller
        {
            InitializeComponent();
            this.controller = controller; // 💥 on récupère la référence
        }

        private void btnConnexion_Click(object sender, EventArgs e)
        {
            string login = txbLogin.Text.Trim();
            string password = txbPassword.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                lblMessageErreur.Text = "Veuillez remplir tous les champs.";
                return;
            }

            utilisateurConnecte = controller.AuthentifierUtilisateur(login, password); // 💥 attention ici aussi on met controller

            if (utilisateurConnecte != null)
            {
                if (utilisateurConnecte.Service == "Culture")
                {
                    MessageBox.Show("❌ Vous n'avez pas les droits suffisants pour accéder à cette application.", "Accès refusé", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            else
            {
                lblMessageErreur.Text = "Login ou mot de passe incorrect.";
            }
        }
        /// <summary>
        /// Permet d'accéder à l'utilisateur connecté
        /// </summary>
        public Utilisateur UtilisateurConnecte
        {
            get { return utilisateurConnecte; }
        }

    }
}
