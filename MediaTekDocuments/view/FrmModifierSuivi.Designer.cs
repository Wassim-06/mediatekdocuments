
namespace MediaTekDocuments.view
{
    partial class FrmModifierSuivi
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSuivis = new System.Windows.Forms.ComboBox();
            this.btnValider = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nouvel état de suivi :";
            // 
            // cmbSuivis
            // 
            this.cmbSuivis.FormattingEnabled = true;
            this.cmbSuivis.Location = new System.Drawing.Point(29, 78);
            this.cmbSuivis.Name = "cmbSuivis";
            this.cmbSuivis.Size = new System.Drawing.Size(140, 21);
            this.cmbSuivis.TabIndex = 1;
            // 
            // btnValider
            // 
            this.btnValider.Location = new System.Drawing.Point(75, 117);
            this.btnValider.Name = "btnValider";
            this.btnValider.Size = new System.Drawing.Size(48, 24);
            this.btnValider.TabIndex = 2;
            this.btnValider.Text = "Valider";
            this.btnValider.UseVisualStyleBackColor = true;
            this.btnValider.Click += new System.EventHandler(this.btnValider_Click);
            // 
            // FrmModifierSuivi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(208, 232);
            this.Controls.Add(this.btnValider);
            this.Controls.Add(this.cmbSuivis);
            this.Controls.Add(this.label1);
            this.Name = "FrmModifierSuivi";
            this.Text = "FrmModifierSuivi";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSuivis;
        private System.Windows.Forms.Button btnValider;
    }
}