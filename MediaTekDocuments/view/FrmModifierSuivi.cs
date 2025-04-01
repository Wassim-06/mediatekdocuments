using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace MediaTekDocuments.view
{
    public partial class FrmModifierSuivi : Form
    {
        // 🔁 Dictionnaire Id -> Libellé
        private readonly Dictionary<int, string> suivis = new Dictionary<int, string>
        {
            { 1, "En cours" },
            { 2, "Relancée" },
            { 3, "Livrée" },
            { 4, "Réglée" }
        };

        public int NouvelIdSuivi { get; private set; }

        public FrmModifierSuivi(model.CommandeDocument commande)
        {
            InitializeComponent();

            // Charger les libellés dans la ComboBox
            foreach (var item in suivis)
            {
                cmbSuivis.Items.Add(new ComboBoxItem(item.Value, item.Key));
            }

            // Sélectionner le suivi actuel
            cmbSuivis.SelectedItem = cmbSuivis.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Value == commande.IdSuivi);
        }

        private void btnValider_Click(object sender, EventArgs e)
        {
            if (cmbSuivis.SelectedItem is ComboBoxItem item)
            {
                NouvelIdSuivi = item.Value;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un état de suivi.");
            }
        }

        // Classe pour stocker un item clé/valeur dans la ComboBox
        private class ComboBoxItem
        {
            public string Text { get; }
            public int Value { get; }

            public ComboBoxItem(string text, int value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString() => Text;
        }
    }
}
