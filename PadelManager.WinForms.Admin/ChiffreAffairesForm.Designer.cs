namespace PadelManager.WinForms.Admin
{
    partial class ChiffreAffairesForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            lblSite = new Label();
            cboSite = new ComboBox();
            chkTousLesSites = new CheckBox();
            btnRafraichir = new Button();
            grdChiffreAffaires = new DataGridView();
            colNomSite = new DataGridViewTextBoxColumn();
            colMontant = new DataGridViewTextBoxColumn();
            lblTotal = new Label();
            lblMessage = new Label();
            ((System.ComponentModel.ISupportInitialize)grdChiffreAffaires).BeginInit();
            SuspendLayout();
            //
            // lblSite
            //
            lblSite.AutoSize = true;
            lblSite.Location = new Point(20, 23);
            lblSite.Name = "lblSite";
            lblSite.Size = new Size(33, 20);
            lblSite.TabIndex = 0;
            lblSite.Text = "Site";
            //
            // cboSite
            //
            cboSite.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSite.Location = new Point(140, 20);
            cboSite.Name = "cboSite";
            cboSite.Size = new Size(200, 28);
            cboSite.TabIndex = 1;
            cboSite.SelectedIndexChanged += cboSite_SelectedIndexChanged;
            //
            // chkTousLesSites
            //
            // Sur sa propre ligne plutôt qu'à droite de cboSite : à cette largeur de fenêtre
            // (440px), "Tous les sites (global)" placé à x=360 sortait du ClientSize et restait
            // invisible sans agrandir la fenêtre à la main.
            chkTousLesSites.AutoSize = true;
            chkTousLesSites.Location = new Point(20, 55);
            chkTousLesSites.Name = "chkTousLesSites";
            chkTousLesSites.Size = new Size(160, 24);
            chkTousLesSites.TabIndex = 2;
            chkTousLesSites.Text = "Tous les sites (global)";
            chkTousLesSites.UseVisualStyleBackColor = true;
            chkTousLesSites.CheckedChanged += chkTousLesSites_CheckedChanged;
            //
            // btnRafraichir
            //
            btnRafraichir.Location = new Point(20, 90);
            btnRafraichir.Name = "btnRafraichir";
            btnRafraichir.Size = new Size(150, 29);
            btnRafraichir.TabIndex = 3;
            btnRafraichir.Text = "Rafraîchir";
            btnRafraichir.UseVisualStyleBackColor = true;
            btnRafraichir.Click += btnRafraichir_Click;
            //
            // grdChiffreAffaires
            //
            grdChiffreAffaires.AllowUserToAddRows = false;
            grdChiffreAffaires.AllowUserToDeleteRows = false;
            grdChiffreAffaires.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grdChiffreAffaires.AutoGenerateColumns = false;
            grdChiffreAffaires.Columns.AddRange(new DataGridViewColumn[] { colNomSite, colMontant });
            grdChiffreAffaires.Location = new Point(20, 130);
            grdChiffreAffaires.Name = "grdChiffreAffaires";
            grdChiffreAffaires.ReadOnly = true;
            grdChiffreAffaires.RowHeadersWidth = 25;
            grdChiffreAffaires.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdChiffreAffaires.Size = new Size(400, 240);
            grdChiffreAffaires.TabIndex = 4;
            //
            // colNomSite
            //
            colNomSite.DataPropertyName = "NomSite";
            colNomSite.HeaderText = "Site";
            colNomSite.Name = "colNomSite";
            colNomSite.ReadOnly = true;
            //
            // colMontant
            //
            colMontant.DataPropertyName = "Montant";
            colMontant.DefaultCellStyle.Format = "0.00€";
            colMontant.HeaderText = "Chiffre d'affaires";
            colMontant.Name = "colMontant";
            colMontant.ReadOnly = true;
            //
            // lblTotal
            //
            lblTotal.AutoSize = true;
            lblTotal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTotal.Location = new Point(20, 380);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(0, 23);
            lblTotal.TabIndex = 5;
            //
            // lblMessage
            //
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(20, 415);
            lblMessage.MaximumSize = new Size(400, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 20);
            lblMessage.TabIndex = 6;
            //
            // ChiffreAffairesForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(440, 450);
            Controls.Add(lblMessage);
            Controls.Add(lblTotal);
            Controls.Add(grdChiffreAffaires);
            Controls.Add(btnRafraichir);
            Controls.Add(chkTousLesSites);
            Controls.Add(cboSite);
            Controls.Add(lblSite);
            Name = "ChiffreAffairesForm";
            Text = "Chiffre d'affaires";
            Load += ChiffreAffairesForm_Load;
            ((System.ComponentModel.ISupportInitialize)grdChiffreAffaires).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblSite;
        private ComboBox cboSite;
        private CheckBox chkTousLesSites;
        private Button btnRafraichir;
        private DataGridView grdChiffreAffaires;
        private DataGridViewTextBoxColumn colNomSite;
        private DataGridViewTextBoxColumn colMontant;
        private Label lblTotal;
        private Label lblMessage;
    }
}
