using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MediaTekDocuments.view
{
    public partial class FrmAjoutDVD : Form
    {
        // Constructeur qui prend en paramètres les dictionnaires pour les genres, publics et rayons.
        public FrmAjoutDVD(Dictionary<string, string> lesGenres, Dictionary<string, string> lesPublics, Dictionary<string, string> lesRayons)
        {
            InitializeComponent();

            // Préparer des dictionnaires pour l'affichage : "Libellé (ID)"
            Dictionary<string, string> genresAffichage = new Dictionary<string, string>();
            foreach (var genre in lesGenres)
            {
                genresAffichage[genre.Key] = $"{genre.Value} ({genre.Key})";
            }
            Dictionary<string, string> publicsAffichage = new Dictionary<string, string>();
            foreach (var pub in lesPublics)
            {
                publicsAffichage[pub.Key] = $"{pub.Value} ({pub.Key})";
            }
            Dictionary<string, string> rayonsAffichage = new Dictionary<string, string>();
            foreach (var rayon in lesRayons)
            {
                rayonsAffichage[rayon.Key] = $"{rayon.Value} ({rayon.Key})";
            }

            // Remplissage des ComboBox avec ces dictionnaires
            cbxGenre.DataSource = new BindingSource(genresAffichage, null);
            cbxGenre.DisplayMember = "Value";
            cbxGenre.ValueMember = "Key";

            cbxPublic.DataSource = new BindingSource(publicsAffichage, null);
            cbxPublic.DisplayMember = "Value";
            cbxPublic.ValueMember = "Key";

            cbxRayon.DataSource = new BindingSource(rayonsAffichage, null);
            cbxRayon.DisplayMember = "Value";
            cbxRayon.ValueMember = "Key";
        }

        // Propriétés pour accéder aux champs saisis dans le formulaire
        public string Id
        {
            get { return txbId.Text; }
            set { txbId.Text = value; }
        }

        public string Titre
        {
            get { return txbTitre.Text; }
            set { txbTitre.Text = value; }
        }

        public string Image
        {
            get { return txbImage.Text; }
            set { txbImage.Text = value; }
        }

        public string Realisateur
        {
            get { return txbRealisateur.Text; }
            set { txbRealisateur.Text = value; }
        }

        public string Synopsis
        {
            get { return txbSynopsis.Text; }
            set { txbSynopsis.Text = value; }
        }

        public string Duree
        {
            get { return txbDuree.Text; }
            set { txbDuree.Text = value; }
        }

        public string IdGenre
        {
            get { return ((KeyValuePair<string, string>)cbxGenre.SelectedItem).Key; }
            set { cbxGenre.SelectedValue = value; }
        }

        public string IdPublic
        {
            get { return ((KeyValuePair<string, string>)cbxPublic.SelectedItem).Key; }
            set { cbxPublic.SelectedValue = value; }
        }

        public string IdRayon
        {
            get { return ((KeyValuePair<string, string>)cbxRayon.SelectedItem).Key; }
            set { cbxRayon.SelectedValue = value; }
        }

        // Bouton Valider : vérifie que tous les champs sont remplis avant de fermer le formulaire en DialogResult.OK
        private void btnValider_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbId.Text) ||
                string.IsNullOrWhiteSpace(txbTitre.Text) ||
                string.IsNullOrWhiteSpace(txbRealisateur.Text) ||
                string.IsNullOrWhiteSpace(txbSynopsis.Text) ||
                string.IsNullOrWhiteSpace(txbDuree.Text) ||
                string.IsNullOrWhiteSpace(txbImage.Text) ||
                cbxGenre.SelectedIndex < 0 ||
                cbxPublic.SelectedIndex < 0 ||
                cbxRayon.SelectedIndex < 0)
            {
                MessageBox.Show("🚨 Tous les champs doivent être remplis !", "Champs manquants", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // Bouton Annuler : ferme le formulaire sans enregistrer
        private void btnAnnuler_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
