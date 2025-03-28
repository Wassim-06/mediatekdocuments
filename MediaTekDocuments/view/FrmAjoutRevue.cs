using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaTekDocuments.view
{
    public partial class FrmAjoutRevue : Form
    {
        public FrmAjoutRevue(Dictionary<string, string> lesGenres, Dictionary<string, string> lesPublics, Dictionary<string, string> lesRayons)
        {
            InitializeComponent();

            // Préparer des dictionnaires d'affichage
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

            // Remplir les ComboBox
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


        private void btnValider_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbId.Text) ||
                string.IsNullOrWhiteSpace(txbTitre.Text) ||
                string.IsNullOrWhiteSpace(txbImage.Text) ||
                string.IsNullOrWhiteSpace(txbPeriodicite.Text) ||
                string.IsNullOrWhiteSpace(txbDelaiRetour.Text) ||
                cbxGenre.SelectedIndex < 0 ||
                cbxPublic.SelectedIndex < 0 ||
                cbxRayon.SelectedIndex < 0)
            {
                MessageBox.Show("Tous les champs doivent être remplis.", "Champs manquants", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tu peux aussi ajouter d'autres validations (par exemple, vérifier que le délai est un nombre, etc.)

            this.DialogResult = DialogResult.OK;
            this.Close();
        }


        private void btnAnnuler_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }


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

        public string Periodicite
        {
            get { return txbPeriodicite.Text; }
            set { txbPeriodicite.Text = value; }
        }
        public string DelaiMiseADispo
        {
            get { return txbDelaiRetour.Text; }
            set { txbDelaiRetour.Text = value; }
        }

        public string DateParution
        {
            // Format : "yyyy-MM-dd"
            get { return dtpDateParution.Value.ToString("yyyy-MM-dd"); }
            // Tu peux ne pas définir de setter si la date n'est pas modifiable manuellement
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

    }
}
