using System;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using MediaTekDocuments.utils;


namespace MediaTekDocuments.view

{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmMediatek : Form
    {
        #region Commun
        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private readonly BindingSource bdgCommandesLivre = new BindingSource();
        private readonly BindingSource bdgCommandesDvd = new BindingSource();
        private readonly BindingSource bdgCommandesRevue = new BindingSource();
        private readonly BindingSource bdgExemplairesLivre = new BindingSource();
        private readonly BindingSource bdgExemplairesDvd = new BindingSource();
        private readonly Utilisateur utilisateurConnecte;

        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        internal FrmMediatek(FrmMediatekController controller, Utilisateur utilisateurConnecte)
        {
            InitializeComponent();
            this.controller = controller;
            this.utilisateurConnecte = utilisateurConnecte; // 💥 On utilise celui qu'on reçoit aussi
            lesLivres = controller.GetAllLivres(); // 🔥
            lesDvd = controller.GetAllDvd();       // 🔥
            lesRevues = controller.GetAllRevues(); // 🔥
            RemplirComboBoxEtat();
            RemplirComboBoxEtatDvd();

            AdapterInterfaceSelonService();
            if (utilisateurConnecte.Service == "Commande")
            {
                List<Abonnement> abonnementsEcheant = controller.GetAbonnementsEcheantDans30Jours();

                if (abonnementsEcheant.Count > 0)
                {
                    List<string> lignes = new List<string>();
                    foreach (Abonnement ab in abonnementsEcheant)
                    {
                        Revue revue = lesRevues.Find(r => r.Id == ab.IdRevue);
                        string titre = (revue != null) ? revue.Titre : "Titre inconnu";
                        lignes.Add($"- {titre} : fin le {ab.DateFinAbonnement:dd/MM/yyyy}");
                    }

                    string message = "Abonnements se terminant dans moins de 30 jours :\n\n" +
                                     string.Join("\n", lignes);

                    MessageBox.Show(message,
                        "Alerte - Échéances Abonnements",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }

        }

        /// <summary>
        /// Adapte l'interface selon le service de l'utilisateur connecté
        /// </summary>
        private void AdapterInterfaceSelonService()
        {
            switch (utilisateurConnecte.Service)
            {
                case "Prêt":
                    // Peut uniquement voir l'onglet Livres
                    tabOngletsApplication.TabPages.Remove(tabCommandesLivres);
                    tabOngletsApplication.TabPages.Remove(tabCommandesDvd);
                    tabOngletsApplication.TabPages.Remove(tabCmdRevues);
                    break;


                case "Commande":
                    tabOngletsApplication.TabPages.Remove(tabLivres);
                    tabOngletsApplication.TabPages.Remove(tabDvd);
                    tabOngletsApplication.TabPages.Remove(tabRevues);
                    tabOngletsApplication.TabPages.Remove(tabReceptionRevue);
                    break;


                case "Culture":
                    // Pas le droit d'accéder -> ferme l'appli (normalement ça sera déjà fait à l'authentification)
                    MessageBox.Show("❌ Vous n'avez pas accès à l'application.", "Accès refusé", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    break;

                default:
                    // Si un autre service inconnu => on ferme aussi
                    MessageBox.Show("❌ Service non reconnu. Accès interdit.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    break;
            }
        }

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }
        #endregion

        #region Onglet Livres
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private List<Livre> lesLivres;

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            txbLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                    AfficherExemplairesLivre(livre.Id);
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }
        #endregion

        #region Gestion Livres - Ajout / Modification / Suppression

        private void btnAjouterLivre_Click(object sender, EventArgs e)
        {
            // Dictionnaires pour remplir les combos
            Dictionary<string, string> genres = controller.GetAllGenres()
                .ToDictionary(genre => genre.Id, genre => genre.Libelle);
            Dictionary<string, string> publics = controller.GetAllPublics()
                .ToDictionary(pub => pub.Id, pub => pub.Libelle);
            Dictionary<string, string> rayons = controller.GetAllRayons()
                .ToDictionary(ray => ray.Id, ray => ray.Libelle);

            // Ouvre la modale
            FrmAjoutLivre frmAjout = new FrmAjoutLivre(genres, publics, rayons);

            if (frmAjout.ShowDialog() == DialogResult.OK)
            {
                // Récupère les valeurs saisies dans la modale
                Livre nouveauLivre = new Livre(
                    frmAjout.Id,
                    frmAjout.Titre,
                    frmAjout.Image,           // ✅ image
                    frmAjout.Isbn,            // ✅ isbn
                    frmAjout.Auteur,          // ✅ auteur
                    frmAjout.Collection,      // ✅ collection
                    frmAjout.IdGenre,
                    genres[frmAjout.IdGenre],
                    frmAjout.IdPublic,
                    publics[frmAjout.IdPublic],
                    frmAjout.IdRayon,
                    rayons[frmAjout.IdRayon]
                );

                // Appel à l’API via le contrôleur
                if (controller.AjouterLivre(nouveauLivre))
                {
                    MessageBox.Show("Livre ajouté avec succès !");
                    lesLivres = controller.GetAllLivres();
                    RemplirLivresListeComplete();
                }
                else
                {
                    MessageBox.Show("Erreur lors de l’ajout du livre.");
                }
            }
        }





        private void btnModifierLivre_Click(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                // Récupérer le livre sélectionné
                Livre livreSelectionne = (Livre)bdgLivresListe.List[bdgLivresListe.Position];

                // Ouvrir la modale de modification (ici, on réutilise FrmAjoutLivre)
                // Vous pouvez adapter FrmAjoutLivre pour qu'il puisse fonctionner en mode édition.
                FrmAjoutLivre frmModifier = new FrmAjoutLivre(
                    controller.GetAllGenres().ToDictionary(g => g.Id, g => g.Libelle),
                    controller.GetAllPublics().ToDictionary(p => p.Id, p => p.Libelle),
                    controller.GetAllRayons().ToDictionary(r => r.Id, r => r.Libelle)
                );

                // Pré-remplir la modale avec les valeurs du livre sélectionné
                frmModifier.Id = livreSelectionne.Id;
                frmModifier.Titre = livreSelectionne.Titre;
                frmModifier.Image = livreSelectionne.Image;
                frmModifier.Isbn = livreSelectionne.Isbn;
                frmModifier.Auteur = livreSelectionne.Auteur;
                frmModifier.Collection = livreSelectionne.Collection;
                frmModifier.IdGenre = livreSelectionne.IdGenre;
                frmModifier.IdPublic = livreSelectionne.IdPublic;
                frmModifier.IdRayon = livreSelectionne.IdRayon;

                if (frmModifier.ShowDialog() == DialogResult.OK)
                {
                    // Création de l'objet livre avec les valeurs modifiées
                    Livre livreModifie = new Livre(
                        frmModifier.Id,
                        frmModifier.Titre,
                        frmModifier.Image,
                        frmModifier.Isbn,
                        frmModifier.Auteur,
                        frmModifier.Collection,
                        frmModifier.IdGenre,
                        // Vous pouvez récupérer le libellé associé si nécessaire, par exemple :
                        controller.GetAllGenres().Find(g => g.Id == frmModifier.IdGenre)?.Libelle,
                        frmModifier.IdPublic,
                        controller.GetAllPublics().Find(p => p.Id == frmModifier.IdPublic)?.Libelle,
                        frmModifier.IdRayon,
                        controller.GetAllRayons().Find(r => r.Id == frmModifier.IdRayon)?.Libelle
                    );

                    // Appel à la méthode de modification
                    if (controller.ModifierLivre(livreModifie))
                    {
                        MessageBox.Show("Livre modifié avec succès !");
                        lesLivres = controller.GetAllLivres();
                        RemplirLivresListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification du livre.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un livre à modifier.");
            }
        }


        private void btnSupprimerLivre_Click(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                Livre livreSelectionne = (Livre)bdgLivresListe.List[bdgLivresListe.Position];

                DialogResult confirmation = MessageBox.Show(
                    $"Voulez-vous vraiment supprimer le livre : {livreSelectionne.Titre} ?",
                    "Confirmation de suppression",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (confirmation == DialogResult.Yes)
                {
                    if (controller.PeutSupprimerDocument(livreSelectionne.Id))
                    {
                        if (controller.SupprimerLivre(livreSelectionne.Id))
                        {
                            MessageBox.Show("Livre supprimé avec succès !");
                            lesLivres = controller.GetAllLivres();
                            RemplirLivresListeComplete();
                        }
                        else
                        {
                            MessageBox.Show("Erreur lors de la suppression du livre.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Impossible de supprimer ce livre car des exemplaires y sont associés.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un livre à supprimer.");
            }
        }


        #endregion

        #region Exemplaire
        private void AfficherExemplairesLivre(string idLivre)
        {
            List<Exemplaire> exemplaires = controller.GetExemplairesByLivre(idLivre);

            // Tri par date d'achat décroissant
            exemplaires = exemplaires.OrderByDescending(e => e.DateAchat).ToList();

            bdgExemplairesLivre.DataSource = null;
            dgvExemplairesLivre.DataSource = null;
            bdgExemplairesLivre.DataSource = exemplaires;
            dgvExemplairesLivre.DataSource = bdgExemplairesLivre;

            // 🔥 Nettoyage des colonnes de dgvExemplairesLivre
            if (dgvExemplairesLivre.Columns.Count > 0)
            {
                // Cacher toutes les colonnes d'abord
                foreach (DataGridViewColumn column in dgvExemplairesLivre.Columns)
                {
                    column.Visible = false;
                }

                // Montrer uniquement numéro, dateAchat, idEtat
                dgvExemplairesLivre.Columns["Numero"].Visible = true;
                dgvExemplairesLivre.Columns["DateAchat"].Visible = true;
                dgvExemplairesLivre.Columns["IdEtat"].Visible = true;

                // (Optionnel) Renommer l'entête si tu veux du plus joli
                dgvExemplairesLivre.Columns["Numero"].HeaderText = "Numéro exemplaire";
                dgvExemplairesLivre.Columns["DateAchat"].HeaderText = "Date d'achat";
                dgvExemplairesLivre.Columns["IdEtat"].HeaderText = "État";

                // Ajuster la taille automatique
                dgvExemplairesLivre.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            // Sélectionner le premier exemplaire si la liste n'est pas vide
            if (bdgExemplairesLivre.Count > 0)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesLivre.List[bdgExemplairesLivre.Position];
                cbxEtatExemplaire.SelectedValue = exemplaire.IdEtat;
            }
            else
            {
                cbxEtatExemplaire.SelectedIndex = -1; // Aucune sélection si pas d'exemplaire
            }


        }

        private void btnModifierEtatExemplaire_Click(object sender, EventArgs e)
        {
            if (dgvExemplairesLivre.CurrentCell != null && cbxEtatExemplaire.SelectedItem != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesLivre.List[bdgExemplairesLivre.Position];
                Etat nouvelEtat = (Etat)cbxEtatExemplaire.SelectedItem;

                if (controller.ModifierEtatExemplaire(exemplaire.Id, exemplaire.Numero, nouvelEtat.Id))
                {
                    MessageBox.Show("✅ État modifié avec succès !");
                    // Actualiser la liste après modification
                    AfficherExemplairesLivre(exemplaire.Id);
                }
                else
                {
                    MessageBox.Show("❌ Échec de la modification de l'état.");
                }
            }
        }


        /// <summary>
        /// Charge les états dans la ComboBox
        /// </summary>
        private void RemplirComboBoxEtat()
        {
            List<Etat> etats = controller.GetAllEtats(); // méthode dans ton controller qui récupère tous les états

            cbxEtatExemplaire.DataSource = etats;
            cbxEtatExemplaire.DisplayMember = "Libelle"; // Ce que l'utilisateur voit (ex: "Neuf", "Usagé", etc.)
            cbxEtatExemplaire.ValueMember = "Id";        // Ce qu'on utilise en interne (ex: "00001", "00002", etc.)

            cbxEtatExemplaire.SelectedIndex = -1; // Rien de sélectionné par défaut
        }

        private void btnSupprimerExemplaire_Click(object sender, EventArgs e)
        {
            if (dgvExemplairesLivre.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesLivre.List[bdgExemplairesLivre.Position];

                DialogResult confirm = MessageBox.Show(
                    "Es-tu sûr de vouloir supprimer cet exemplaire ?",
                    "Confirmation suppression",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (confirm == DialogResult.Yes)
                {
                    if (controller.SupprimerExemplaire(exemplaire.Id, exemplaire.Numero))
                    {
                        MessageBox.Show("✅ Exemplaire supprimé avec succès !");
                        AfficherExemplairesLivre(exemplaire.Id); // Refresh la liste
                    }
                    else
                    {
                        MessageBox.Show("❌ Échec de la suppression de l'exemplaire.");
                    }
                }
            }
        }

        private void AfficherExemplairesDvd(string idDvd)
        {
            List<Exemplaire> exemplaires = controller.GetExemplairesByDvd(idDvd);

            exemplaires = exemplaires.OrderByDescending(e => e.DateAchat).ToList();

            bdgExemplairesDvd.DataSource = null;
            dgvExemplairesDvd.DataSource = null;
            bdgExemplairesDvd.DataSource = exemplaires;
            dgvExemplairesDvd.DataSource = bdgExemplairesDvd;

            if (dgvExemplairesDvd.Columns.Count > 0)
            {
                foreach (DataGridViewColumn column in dgvExemplairesDvd.Columns)
                {
                    column.Visible = false;
                }

                dgvExemplairesDvd.Columns["Numero"].Visible = true;
                dgvExemplairesDvd.Columns["DateAchat"].Visible = true;
                dgvExemplairesDvd.Columns["IdEtat"].Visible = true;

                dgvExemplairesDvd.Columns["Numero"].HeaderText = "Numéro exemplaire";
                dgvExemplairesDvd.Columns["DateAchat"].HeaderText = "Date d'achat";
                dgvExemplairesDvd.Columns["IdEtat"].HeaderText = "État";

                dgvExemplairesDvd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }

            if (bdgExemplairesDvd.Count > 0)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesDvd.List[bdgExemplairesDvd.Position];
                cbxEtatExemplaireDvd.SelectedValue = exemplaire.IdEtat;
            }
            else
            {
                cbxEtatExemplaireDvd.SelectedIndex = -1;
            }
        }

        private void btnModifierEtatExemplaireDvd_Click(object sender, EventArgs e)
        {
            if (dgvExemplairesDvd.CurrentCell != null && cbxEtatExemplaireDvd.SelectedItem != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesDvd.List[bdgExemplairesDvd.Position];
                Etat nouvelEtat = (Etat)cbxEtatExemplaireDvd.SelectedItem;

                if (exemplaire.IdEtat != nouvelEtat.Id)
                {
                    if (controller.ModifierEtatExemplaire(exemplaire.Id, exemplaire.Numero, nouvelEtat.Id))
                    {
                        MessageBox.Show("✅ État modifié avec succès !");
                        AfficherExemplairesDvd(exemplaire.Id);
                    }
                    else
                    {
                        MessageBox.Show("❌ Échec de la modification de l'état.");
                    }
                }
            }
        }


        private void btnSupprimerExemplaireDvd_Click(object sender, EventArgs e)
        {
            if (dgvExemplairesDvd.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesDvd.List[bdgExemplairesDvd.Position];

                if (MessageBox.Show("❗ Es-tu sûr de vouloir supprimer cet exemplaire ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (controller.SupprimerExemplaire(exemplaire.Id, exemplaire.Numero))
                    {
                        MessageBox.Show("✅ Exemplaire supprimé !");
                        AfficherExemplairesDvd(exemplaire.Id);
                    }
                    else
                    {
                        MessageBox.Show("❌ Échec de la suppression !");
                    }
                }
            }
        }

        /// <summary>
        /// Charge les états dans la ComboBox pour les DVD
        /// </summary>
        private void RemplirComboBoxEtatDvd()
        {
            List<Etat> etats = controller.GetAllEtats(); // On récupère la même liste d'états

            cbxEtatExemplaireDvd.DataSource = etats;
            cbxEtatExemplaireDvd.DisplayMember = "Libelle"; // Ce que tu affiches (Neuf, Usagé, etc.)
            cbxEtatExemplaireDvd.ValueMember = "Id";         // La vraie valeur (00001, 00002...)

            cbxEtatExemplaireDvd.SelectedIndex = -1; // Aucune sélection par défaut
        }



        #endregion

        #region Onglet Dvd
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private List<Dvd> lesDvd;

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="Dvds">liste de dvd</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                    AfficherExemplairesDvd(dvd.Id);
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }
        #endregion

        #region Gestion DVD - Ajout / Modification / Suppression

        private void btnAjouterDVD_Click(object sender, EventArgs e)
        {
            // Récupérer les dictionnaires pour remplir les ComboBox
            Dictionary<string, string> genres = controller.GetAllGenres().ToDictionary(g => g.Id, g => g.Libelle);
            Dictionary<string, string> publics = controller.GetAllPublics().ToDictionary(p => p.Id, p => p.Libelle);
            Dictionary<string, string> rayons = controller.GetAllRayons().ToDictionary(r => r.Id, r => r.Libelle);

            // Ouvrir la modale spécifique pour les DVD
            FrmAjoutDVD frmAjout = new FrmAjoutDVD(genres, publics, rayons);

            if (frmAjout.ShowDialog() == DialogResult.OK)
            {
                // Créer l'objet DVD à partir des valeurs de la modale
                Dvd nouveauDvd = new Dvd(
                    frmAjout.Id,
                    frmAjout.Titre,
                    frmAjout.Image,
                    int.Parse(frmAjout.Duree),      // Durée (int) en 4ème position
                    frmAjout.Realisateur,           // Réalisateur (string) en 5ème position
                    frmAjout.Synopsis,              // Synopsis (string) en 6ème position
                    frmAjout.IdGenre,
                    genres[frmAjout.IdGenre],
                    frmAjout.IdPublic,
                    publics[frmAjout.IdPublic],
                    frmAjout.IdRayon,
                    rayons[frmAjout.IdRayon]
                );


                // Appeler l'API via le contrôleur
                if (controller.AjouterDVD(nouveauDvd))
                {
                    MessageBox.Show("DVD ajouté avec succès !");
                    lesDvd = controller.GetAllDvd();
                    RemplirDvdListeComplete();
                }
                else
                {
                    MessageBox.Show("Erreur lors de l’ajout du DVD.");
                }
            }
        }


        private void btnModifierDVD_Click(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                // Récupérer le DVD sélectionné
                Dvd dvdSelectionne = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];

                // Ouvrir la modale en mode édition (FrmAjoutDVD)
                FrmAjoutDVD frmModifier = new FrmAjoutDVD(
                    controller.GetAllGenres().ToDictionary(g => g.Id, g => g.Libelle),
                    controller.GetAllPublics().ToDictionary(p => p.Id, p => p.Libelle),
                    controller.GetAllRayons().ToDictionary(r => r.Id, r => r.Libelle)
                );

                // Pré-remplir le formulaire avec les données du DVD sélectionné
                frmModifier.Id = dvdSelectionne.Id;
                frmModifier.Titre = dvdSelectionne.Titre;
                frmModifier.Image = dvdSelectionne.Image;
                frmModifier.Realisateur = dvdSelectionne.Realisateur;
                frmModifier.Synopsis = dvdSelectionne.Synopsis;
                frmModifier.Duree = dvdSelectionne.Duree.ToString();
                frmModifier.IdGenre = dvdSelectionne.IdGenre;
                frmModifier.IdPublic = dvdSelectionne.IdPublic;
                frmModifier.IdRayon = dvdSelectionne.IdRayon;

                if (frmModifier.ShowDialog() == DialogResult.OK)
                {
                    // Création de l'objet DVD avec les nouvelles valeurs
                    Dvd dvdModifie = new Dvd(
                        frmModifier.Id,
                        frmModifier.Titre,
                        frmModifier.Image,
                        int.Parse(frmModifier.Duree),  // Durée en 4ème position
                        frmModifier.Realisateur,         // Réalisateur en 5ème position
                        frmModifier.Synopsis,            // Synopsis en 6ème position
                        frmModifier.IdGenre,
                        controller.GetAllGenres().Find(g => g.Id == frmModifier.IdGenre)?.Libelle,
                        frmModifier.IdPublic,
                        controller.GetAllPublics().Find(p => p.Id == frmModifier.IdPublic)?.Libelle,
                        frmModifier.IdRayon,
                        controller.GetAllRayons().Find(r => r.Id == frmModifier.IdRayon)?.Libelle
                    );


                    // Appeler la méthode de modification du contrôleur
                    if (controller.ModifierDVD(dvdModifie))
                    {
                        MessageBox.Show("DVD modifié avec succès !");
                        lesDvd = controller.GetAllDvd();
                        RemplirDvdListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification du DVD.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un DVD à modifier.");
            }
        }


        private void btnSupprimerDVD_Click(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                Dvd dvdSelectionne = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];

                DialogResult confirmation = MessageBox.Show(
                    $"Voulez-vous vraiment supprimer le DVD : {dvdSelectionne.Titre} ?",
                    "Confirmation de suppression",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (confirmation == DialogResult.Yes)
                {
                    // Vérifier si le DVD peut être supprimé (aucun exemplaire/commande attaché)
                    if (controller.PeutSupprimerDocument(dvdSelectionne.Id))
                    {
                        if (controller.SupprimerDVD(dvdSelectionne.Id))
                        {
                            MessageBox.Show("DVD supprimé avec succès !");
                            lesDvd = controller.GetAllDvd();
                            RemplirDvdListeComplete();
                        }
                        else
                        {
                            MessageBox.Show("Erreur lors de la suppression du DVD.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Impossible de supprimer ce DVD car des exemplaires ou commandes y sont associés.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un DVD à supprimer.");
            }
        }


        #endregion

        #region Onglet Revues
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private List<Revue> lesRevues;

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="revues"></param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }
        #endregion

        #region Gestion Revues - Ajout / Modification / Suppression
        private void btnAjouterRevues_Click(object sender, EventArgs e)
        {
            // Récupérer les dictionnaires pour remplir les ComboBox
            Dictionary<string, string> genres = controller.GetAllGenres().ToDictionary(g => g.Id, g => g.Libelle);
            Dictionary<string, string> publics = controller.GetAllPublics().ToDictionary(p => p.Id, p => p.Libelle);
            Dictionary<string, string> rayons = controller.GetAllRayons().ToDictionary(r => r.Id, r => r.Libelle);

            // Ouvrir le formulaire d'ajout de revue en lui passant les dictionnaires
            FrmAjoutRevue frmAjout = new FrmAjoutRevue(genres, publics, rayons);

            if (frmAjout.ShowDialog() == DialogResult.OK)
            {
                // Convertir le délai de mise à dispo en entier
                if (!int.TryParse(frmAjout.DelaiMiseADispo, out int delai))
                {
                    MessageBox.Show("Veuillez entrer un délai de mise à disposition valide.");
                    return;
                }

                // Créer l'objet Revue à partir des valeurs saisies
                Revue nouvelleRevue = new Revue(
                    frmAjout.Id,
                    frmAjout.Titre,
                    frmAjout.Image,
                    frmAjout.IdGenre,
                    genres[frmAjout.IdGenre],
                    frmAjout.IdPublic,
                    publics[frmAjout.IdPublic],
                    frmAjout.IdRayon,
                    rayons[frmAjout.IdRayon],
                    frmAjout.Periodicite,
                    delai
                );

                // Appel à l'API via le contrôleur
                if (controller.AjouterRevue(nouvelleRevue))
                {
                    MessageBox.Show("Revue ajoutée avec succès !");
                    lesRevues = controller.GetAllRevues();
                    RemplirRevuesListeComplete();
                }
                else
                {
                    MessageBox.Show("Erreur lors de l'ajout de la revue.");
                }
            }
        }


        private void btnModifierRevues_Click(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                // Récupérer la revue sélectionnée
                Revue revueSelectionnee = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];

                // Récupérer les dictionnaires pour remplir les ComboBox
                Dictionary<string, string> genres = controller.GetAllGenres().ToDictionary(g => g.Id, g => g.Libelle);
                Dictionary<string, string> publics = controller.GetAllPublics().ToDictionary(p => p.Id, p => p.Libelle);
                Dictionary<string, string> rayons = controller.GetAllRayons().ToDictionary(r => r.Id, r => r.Libelle);

                // Ouvrir le formulaire de modification en pré-remplissant les champs
                FrmAjoutRevue frmModifier = new FrmAjoutRevue(genres, publics, rayons);
                frmModifier.Id = revueSelectionnee.Id;
                frmModifier.Titre = revueSelectionnee.Titre;
                frmModifier.Image = revueSelectionnee.Image;
                frmModifier.Periodicite = revueSelectionnee.Periodicite;
                frmModifier.DelaiMiseADispo = revueSelectionnee.DelaiMiseADispo.ToString();
                frmModifier.IdGenre = revueSelectionnee.IdGenre;
                frmModifier.IdPublic = revueSelectionnee.IdPublic;
                frmModifier.IdRayon = revueSelectionnee.IdRayon;

                if (frmModifier.ShowDialog() == DialogResult.OK)
                {
                    // Convertir le délai en entier
                    if (!int.TryParse(frmModifier.DelaiMiseADispo, out int delai))
                    {
                        MessageBox.Show("Veuillez entrer un délai de mise à disposition valide.");
                        return;
                    }

                    // Créer un objet Revue avec les nouvelles valeurs
                    Revue revueModifiee = new Revue(
                        frmModifier.Id,
                        frmModifier.Titre,
                        frmModifier.Image,
                        frmModifier.IdGenre,
                        genres[frmModifier.IdGenre],
                        frmModifier.IdPublic,
                        publics[frmModifier.IdPublic],
                        frmModifier.IdRayon,
                        rayons[frmModifier.IdRayon],
                        frmModifier.Periodicite,
                        delai
                    );

                    // Appeler la méthode de modification
                    if (controller.ModifierRevue(revueModifiee))
                    {
                        MessageBox.Show("Revue modifiée avec succès !");
                        lesRevues = controller.GetAllRevues();
                        RemplirRevuesListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification de la revue.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une revue à modifier.");
            }
        }


        private void btnSupprimerRevues_Click(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                Revue revueSelectionnee = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];

                DialogResult confirmation = MessageBox.Show(
                    $"Voulez-vous vraiment supprimer la revue : {revueSelectionnee.Titre} ?",
                    "Confirmation de suppression",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (confirmation == DialogResult.Yes)
                {
                    // Vérifier si la revue peut être supprimée
                    if (controller.PeutSupprimerDocument(revueSelectionnee.Id))
                    {
                        if (controller.SupprimerRevue(revueSelectionnee.Id))
                        {
                            MessageBox.Show("Revue supprimée avec succès !");
                            lesRevues = controller.GetAllRevues();
                            RemplirRevuesListeComplete();
                        }
                        else
                        {
                            MessageBox.Show("Erreur lors de la suppression de la revue.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Impossible de supprimer cette revue car elle a des exemplaires ou commandes associés.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une revue à supprimer.");
            }
        }

        #endregion

        #region Onglet Paarutions
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        const string ETATNEUF = "00001";

        /// <summary>
        /// Ouverture de l'onglet : récupère le revues et vide tous les champs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            txbReceptionRevueNumero.Text = "";
        }

        /// <summary>
        /// Remplit le dategrid des exemplaires avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires">liste d'exemplaires</param>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires != null)
            {
                bdgExemplairesListe.DataSource = exemplaires;
                dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
                dgvReceptionExemplairesListe.Columns["id"].Visible = false;
                dgvReceptionExemplairesListe.Columns["photo"].Visible = false; // Cacher photo
                dgvReceptionExemplairesListe.Columns["idEtat"].Visible = true;  // Montrer idEtat
                dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
                dgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
                dgvReceptionExemplairesListe.Columns["idEtat"].DisplayIndex = 2; // Nouvelle colonne
                dgvReceptionExemplairesListe.Columns["idEtat"].HeaderText = "État"; // Changer le nom
            }
            else
            {
                bdgExemplairesListe.DataSource = null;
            }
        }


        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
        }

        /// <summary>
        /// Récupère et affiche les exemplaires d'une revue
        /// </summary>
        private void AfficheReceptionExemplairesRevue()
        {
            string idDocuement = txbReceptionRevueNumero.Text;
            lesExemplaires = controller.GetExemplairesRevue(idDocuement);
            RemplirReceptionExemplairesListe(lesExemplaires);
            AccesReceptionExemplaireGroupBox(true);
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            grpReceptionExemplaire.Enabled = acces;
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire à insérer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = txbReceptionRevueNumero.Text;
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);
                    if (controller.CreerExemplaire(exemplaire))
                    {
                        AfficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Photo":
                    sortedList = lesExemplaires.OrderBy(o => o.Photo).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// affichage de l'image de l'exemplaire suite à la sélection d'un exemplaire dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                string image = exemplaire.Photo;
                try
                {
                    pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
            }
        }

        private void btnReceptionChangerEtat_Click(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];

                // Demander le nouvel état avec une InputBox
                string nouvelIdEtat = Microsoft.VisualBasic.Interaction.InputBox(
                    "Entrez le nouvel ID d'état (00001 pour Neuf, 00002 pour Usagé, 00003 pour détérioré, 00004 pour inutilisable)",
                    "Changer l'état",
                    exemplaire.IdEtat
                );

                if (!string.IsNullOrEmpty(nouvelIdEtat))
                {
                    if (controller.ModifierEtatExemplaire(exemplaire.Id, exemplaire.Numero, nouvelIdEtat))
                    {
                        MessageBox.Show("✅ État modifié avec succès !");
                        AfficheReceptionExemplairesRevue(); // Rafraîchir la liste
                    }
                    else
                    {
                        MessageBox.Show("❌ Échec de la modification.");
                    }
                }
            }
        }

        #endregion

        #region Onglet CommandesLivres
        private void btnRechercherLivre_Click(object sender, EventArgs e)
        {
            string idLivre = txbNumLivreCommande.Text.Trim();
            if (string.IsNullOrWhiteSpace(idLivre))
            {
                MessageBox.Show("Veuillez saisir un numéro de livre.");
                return;
            }

            Livre livre = lesLivres.Find(l => l.Id == idLivre);

            if (livre != null)
            {
                lblTitreLivreCommande.Text = $"Titre : {livre.Titre}";
                // Appeler la méthode pour afficher les commandes associées (on la fera après)
                AfficherCommandesLivre(idLivre);
            }
            else
            {
                lblTitreLivreCommande.Text = "📕 Livre introuvable.";
                dgvCommandesLivre.DataSource = null;
            }
        }

        private void AfficherCommandesLivre(string idLivre)
        {
            List<CommandeDocument> toutesLesCommandes = controller.GetCommandesByLivre(idLivre);

            // 🔎 Nouveau : filtrer uniquement celles liées au livre (sécurité max)
            List<CommandeDocument> commandesFiltrees = toutesLesCommandes
                .Where(cmd => cmd.IdLivreDvd == idLivre)
                .ToList();

            commandesFiltrees.Sort((x, y) => y.DateCommande.CompareTo(x.DateCommande));
            bdgCommandesLivre.DataSource = null;
            dgvCommandesLivre.DataSource = null;
            bdgCommandesLivre.DataSource = commandesFiltrees;
            dgvCommandesLivre.DataSource = bdgCommandesLivre;
        }



        private void btnAjouterCommande_Click(object sender, EventArgs e)
        {
            string idLivre = txbNumLivreCommande.Text.Trim();

            if (string.IsNullOrEmpty(idLivre))
            {
                MessageBox.Show("Veuillez saisir un numéro de livre.");
                return;
            }

            // Vérifie si le livre existe
            Livre livre = lesLivres.Find(l => l.Id == idLivre);
            if (livre == null)
            {
                MessageBox.Show("Livre introuvable.");
                return;
            }

            // Vérification des champs
            if (!decimal.TryParse(txbMontantCommande.Text, out decimal montant) || montant <= 0)
            {
                MessageBox.Show("Montant invalide.");
                return;
            }

            if (!int.TryParse(txbNbExemplairesCommande.Text, out int nbExemplaires) || nbExemplaires <= 0)
            {
                MessageBox.Show("Nombre d'exemplaires invalide.");
                return;
            }

            DateTime dateCommande = dtpDateCommande.Value;
            double montantDouble = (double)montant;

            // Création de la commande :
            // - Passez une chaîne vide pour l'ID afin que la base l'auto-incrémente.
            // - Passez idLivre dans la propriété IdLivreDvd pour enregistrer l'ID du livre.
            CommandeDocument commande = new CommandeDocument(
                "",               // ID vide pour auto-incrémentation
                dateCommande,
                montantDouble,
                nbExemplaires,
                1,
                "en cours",
                idLivre         // Cet argument sera affecté à la propriété IdLivreDvd
            );

            if (controller.AjouterCommande(commande))
            {
                MessageBox.Show("Commande ajoutée !");
                // Recharger la liste
                bdgCommandesLivre.DataSource = controller.GetCommandesByLivre(idLivre);
            }
            else
            {
                MessageBox.Show("Erreur lors de l’ajout de la commande.");
            }
        }


        private void btnModifierSuivi_Click(object sender, EventArgs e)
        {
            if (dgvCommandesLivre.SelectedRows.Count > 0)
            {
                CommandeDocument cmd = (CommandeDocument)bdgCommandesLivre[dgvCommandesLivre.SelectedRows[0].Index];

                // Ouvrir une boîte de dialogue pour modifier le suivi
                FrmModifierSuivi frm = new FrmModifierSuivi(cmd);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    // Récupérer le nouveau suivi choisi
                    int nouvelIdSuivi = frm.NouvelIdSuivi;

                    if (controller.ModifierSuiviCommande(cmd, nouvelIdSuivi))
                    {
                        MessageBox.Show("Étape de suivi modifiée !");
                        AfficherCommandesLivre(cmd.IdLivreDvd); // Refresh
                    }
                    else
                    {
                        MessageBox.Show("Modification non autorisée selon les règles de gestion.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une commande.");
            }
        }

        private void btnSupprimerCommande_Click(object sender, EventArgs e)
        {
            if (dgvCommandesLivre.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner une commande à supprimer.");
                return;
            }

            CommandeDocument cmd = (CommandeDocument)bdgCommandesLivre[dgvCommandesLivre.SelectedRows[0].Index];

            if (cmd.LibelleSuivi == "Livrée" || cmd.LibelleSuivi == "Réglée")
            {
                MessageBox.Show("Impossible de supprimer une commande livrée ou réglée.");
                return;
            }

            DialogResult confirm = MessageBox.Show("Confirmer la suppression de la commande ?", "Confirmation", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                if (controller.SupprimerCommande(cmd.Id))
                {
                    MessageBox.Show("Commande supprimée.");
                    AfficherCommandesLivre(cmd.IdLivreDvd); // Refresh
                }
                else
                {
                    MessageBox.Show("Erreur lors de la suppression.");
                }
            }
        }

        #endregion

        #region Onglet CommandesDVD
        private void btnRechercherDvd_Click(object sender, EventArgs e)
        {
            string idDvd = txbNumDvdCommande.Text.Trim();
            if (string.IsNullOrWhiteSpace(idDvd))
            {
                MessageBox.Show("Veuillez saisir un numéro de DVD.");
                return;
            }

            Dvd dvd = lesDvd.Find(d => d.Id == idDvd);

            if (dvd != null)
            {
                lblTitreDvdCommande.Text = $"Titre : {dvd.Titre}";
                AfficherCommandesDvd(idDvd);
            }
            else
            {
                lblTitreDvdCommande.Text = "📀 DVD introuvable.";
                dgvCommandesDvd.DataSource = null;
            }
        }

        private void AfficherCommandesDvd(string idDvd)
        {
            List<CommandeDocument> toutesLesCommandes = controller.GetCommandesByDvd(idDvd);

            List<CommandeDocument> commandesFiltrees = toutesLesCommandes
                .Where(cmd => cmd.IdLivreDvd == idDvd)
                .ToList();

            commandesFiltrees.Sort((x, y) => y.DateCommande.CompareTo(x.DateCommande));
            bdgCommandesDvd.DataSource = null;
            dgvCommandesDvd.DataSource = null;
            bdgCommandesDvd.DataSource = commandesFiltrees;
            dgvCommandesDvd.DataSource = bdgCommandesDvd;
        }

        private void btnAjouterCommandeDvd_Click(object sender, EventArgs e)
        {
            string idDvd = txbNumDvdCommande.Text.Trim();

            if (string.IsNullOrEmpty(idDvd))
            {
                MessageBox.Show("Veuillez saisir un numéro de DVD.");
                return;
            }

            // Vérifie si le DVD existe
            Dvd dvd = lesDvd.Find(d => d.Id == idDvd);
            if (dvd == null)
            {
                MessageBox.Show("DVD introuvable.");
                return;
            }

            // Vérification des champs
            if (!decimal.TryParse(txbMontantCommandeDvd.Text, out decimal montant) || montant <= 0)
            {
                MessageBox.Show("Montant invalide.");
                return;
            }

            if (!int.TryParse(txbNbExemplairesCommandeDvd.Text, out int nbExemplaires) || nbExemplaires <= 0)
            {
                MessageBox.Show("Nombre d'exemplaires invalide.");
                return;
            }

            DateTime dateCommande = dtpDateCommandeDvd.Value;
            double montantDouble = (double)montant;

            // Création de la commande
            CommandeDocument commande = new CommandeDocument(
                "",               // ID vide → auto-incrément
                dateCommande,
                montantDouble,
                nbExemplaires,
                1,                // Suivi "en cours"
                "En cours",
                idDvd
            );

            if (controller.AjouterCommande(commande))
            {
                MessageBox.Show("Commande DVD ajoutée !");
                bdgCommandesDvd.DataSource = controller.GetCommandesByDvd(idDvd);
            }
            else
            {
                MessageBox.Show("Erreur lors de l’ajout de la commande DVD.");
            }
        }
        private void btnModifierSuiviCommandeDvd_Click(object sender, EventArgs e)
        {
            if (dgvCommandesDvd.SelectedRows.Count > 0)
            {
                CommandeDocument cmd = (CommandeDocument)bdgCommandesDvd[dgvCommandesDvd.SelectedRows[0].Index];

                // Ouvrir la fenêtre de modification de suivi
                FrmModifierSuivi frm = new FrmModifierSuivi(cmd);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    int nouvelIdSuivi = frm.NouvelIdSuivi;

                    if (controller.ModifierSuiviCommande(cmd, nouvelIdSuivi))
                    {
                        MessageBox.Show("Étape de suivi modifiée !");
                        AfficherCommandesDvd(cmd.IdLivreDvd); // Recharge les commandes liées au DVD
                    }
                    else
                    {
                        MessageBox.Show("Modification non autorisée selon les règles de gestion.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une commande.");
            }
        }


        private void btnSupprimerCommandeDvd_Click(object sender, EventArgs e)
        {
            if (dgvCommandesDvd.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner une commande à supprimer.");
                return;
            }

            CommandeDocument cmd = (CommandeDocument)bdgCommandesDvd[dgvCommandesDvd.SelectedRows[0].Index];

            if (cmd.LibelleSuivi == "Livrée" || cmd.LibelleSuivi == "Réglée")
            {
                MessageBox.Show("Impossible de supprimer une commande livrée ou réglée.");
                return;
            }

            DialogResult confirm = MessageBox.Show("Confirmer la suppression de la commande ?", "Confirmation", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                if (controller.SupprimerCommande(cmd.Id))
                {
                    MessageBox.Show("Commande supprimée.");
                    AfficherCommandesDvd(cmd.IdLivreDvd); // Recharge les commandes du DVD
                }
                else
                {
                    MessageBox.Show("Erreur lors de la suppression.");
                }
            }
        }

        #endregion

        #region CommandesRevues
        private void btnRechercherRevueCommande_Click(object sender, EventArgs e)
        {
            string idRevue = txbNumRevueCommande.Text.Trim();

            if (string.IsNullOrWhiteSpace(idRevue))
            {
                MessageBox.Show("Veuillez saisir un numéro de revue.");
                return;
            }

            Revue revue = lesRevues.Find(r => r.Id == idRevue);

            if (revue != null)
            {
                lblTitreRevueCommande.Text = $"Titre : {revue.Titre}";
                AfficherAbonnementsRevue(idRevue);
            }
            else
            {
                lblTitreRevueCommande.Text = "📚 Revue introuvable.";
                dgvCommandesRevue.DataSource = null;
            }

        }

        private void AfficherAbonnementsRevue(string idRevue)
        {
            List<Abonnement> abonnements = controller.GetAbonnementsByRevue(idRevue);

            List<Abonnement> abonnementsFiltres = abonnements
                .Where(a => a.IdRevue == idRevue)
                .OrderByDescending(a => a.DateCommande)
                .ToList();

            bdgCommandesRevue.DataSource = null;
            dgvCommandesRevue.DataSource = null;

            bdgCommandesRevue.DataSource = abonnementsFiltres;
            dgvCommandesRevue.DataSource = bdgCommandesRevue;

        }

        private void btnAjouterAbonnement_Click(object sender, EventArgs e)
        {
            string idRevue = txbNumRevueCommande.Text.Trim();

            if (string.IsNullOrEmpty(idRevue))
            {
                MessageBox.Show("Veuillez saisir un numéro de revue.");
                return;
            }

            Revue revue = lesRevues.Find(r => r.Id == idRevue);
            if (revue == null)
            {
                MessageBox.Show("Revue introuvable.");
                return;
            }

            if (!decimal.TryParse(txbMontantCommandeRevue.Text, out decimal montant) || montant <= 0)
            {
                MessageBox.Show("Montant invalide.");
                return;
            }

            DateTime dateCommande = dtpDateCommandeRevue.Value;
            DateTime dateFin = dtpDateFinAbonnement.Value;
            Console.WriteLine(dateCommande);
            Console.WriteLine(dateFin);
            if (dateFin <= dateCommande)
            {
                MessageBox.Show("La date de fin d'abonnement doit être postérieure à la date de commande.");
                return;
            }

            Abonnement abonnement = new Abonnement(
                "", // id vide
                dateCommande,
                dateFin,
                (double)montant,
                idRevue
            );


            if (controller.AjouterAbonnement(abonnement))
            {
                MessageBox.Show("Abonnement ajouté !");
                AfficherAbonnementsRevue(idRevue); // Recharge la liste
            }
            else
            {
                MessageBox.Show("Erreur lors de l'ajout de l'abonnement.");
            }

        }

        private void btnSupprimerAbonnement_Click(object sender, EventArgs e)
        {
            if (dgvCommandesRevue.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner un abonnement à supprimer.");
                return;
            }

            Abonnement abonnement = (Abonnement)bdgCommandesRevue[dgvCommandesRevue.SelectedRows[0].Index];

            // ✅ On demande d'abord la confirmation à l'utilisateur
            DialogResult confirm = MessageBox.Show("Confirmer la suppression de l'abonnement ?", "Confirmation", MessageBoxButtons.YesNo);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            // ✅ Ensuite on récupère les exemplaires (et on peut annuler la suppression si besoin)
            List<Exemplaire> exemplaires = controller.GetExemplairesByRevue(abonnement.IdRevue);

            foreach (var ex in exemplaires)
            {
                if (Utils.ParutionDansAbonnement(abonnement.DateCommande, abonnement.DateFinAbonnement, ex.DateAchat))
                {
                    MessageBox.Show("❌ Impossible de supprimer l’abonnement : des exemplaires sont liés à cette période.");
                    return;
                }
            }

            if (controller.SupprimerAbonnement(abonnement.Id))
            {
                MessageBox.Show("✅ Abonnement supprimé !");
                AfficherAbonnementsRevue(abonnement.IdRevue);
            }
            else
            {
                MessageBox.Show("❌ Erreur lors de la suppression.");
            }
        }


        #endregion

    }
}
