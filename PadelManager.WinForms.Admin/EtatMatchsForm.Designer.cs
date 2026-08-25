namespace PadelManager.WinForms.Admin
{
    partial class EtatMatchsForm
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
            lblTerrains = new Label();
            grdMatchs = new DataGridView();
            colId = new DataGridViewTextBoxColumn();
            colNomSite = new DataGridViewTextBoxColumn();
            colTerrainId = new DataGridViewTextBoxColumn();
            colNumeroTerrain = new DataGridViewTextBoxColumn();
            colDateHeure = new DataGridViewTextBoxColumn();
            colVisibilite = new DataGridViewTextBoxColumn();
            colStatut = new DataGridViewTextBoxColumn();
            lblMessage = new Label();
            ((System.ComponentModel.ISupportInitialize)grdMatchs).BeginInit();
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
            chkTousLesSites.AutoSize = true;
            chkTousLesSites.Location = new Point(360, 23);
            chkTousLesSites.Name = "chkTousLesSites";
            chkTousLesSites.Size = new Size(160, 24);
            chkTousLesSites.TabIndex = 2;
            chkTousLesSites.Text = "Tous les sites (global)";
            chkTousLesSites.UseVisualStyleBackColor = true;
            chkTousLesSites.CheckedChanged += chkTousLesSites_CheckedChanged;
            //
            // btnRafraichir
            //
            btnRafraichir.Location = new Point(20, 60);
            btnRafraichir.Name = "btnRafraichir";
            btnRafraichir.Size = new Size(150, 29);
            btnRafraichir.TabIndex = 3;
            btnRafraichir.Text = "Rafraîchir";
            btnRafraichir.UseVisualStyleBackColor = true;
            btnRafraichir.Click += btnRafraichir_Click;
            //
            // lblTerrains
            //
            // Propre ligne, pleine largeur (même leçon que le label de dette côté Membre) : un
            // récap "tous les sites" peut être long, il ne doit jamais sortir de la zone visible.
            lblTerrains.AutoSize = true;
            lblTerrains.Location = new Point(20, 100);
            lblTerrains.MaximumSize = new Size(900, 0);
            lblTerrains.Name = "lblTerrains";
            lblTerrains.Size = new Size(0, 20);
            lblTerrains.TabIndex = 4;
            //
            // grdMatchs
            //
            grdMatchs.AllowUserToAddRows = false;
            grdMatchs.AllowUserToDeleteRows = false;
            grdMatchs.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            grdMatchs.AutoGenerateColumns = false;
            grdMatchs.Columns.AddRange(new DataGridViewColumn[] { colId, colNomSite, colTerrainId, colNumeroTerrain, colDateHeure, colVisibilite, colStatut });
            grdMatchs.Location = new Point(20, 135);
            grdMatchs.Name = "grdMatchs";
            grdMatchs.ReadOnly = true;
            grdMatchs.RowHeadersWidth = 25;
            grdMatchs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdMatchs.Size = new Size(900, 420);
            grdMatchs.TabIndex = 5;
            //
            // colId
            //
            colId.DataPropertyName = "Id";
            colId.HeaderText = "ID match";
            colId.Name = "colId";
            colId.ReadOnly = true;
            //
            // colNomSite
            //
            colNomSite.DataPropertyName = "NomSite";
            colNomSite.HeaderText = "Site";
            colNomSite.Name = "colNomSite";
            colNomSite.ReadOnly = true;
            //
            // colTerrainId
            //
            colTerrainId.DataPropertyName = "TerrainId";
            colTerrainId.HeaderText = "ID terrain";
            colTerrainId.Name = "colTerrainId";
            colTerrainId.ReadOnly = true;
            //
            // colNumeroTerrain
            //
            colNumeroTerrain.DataPropertyName = "NumeroTerrain";
            colNumeroTerrain.HeaderText = "N° terrain";
            colNumeroTerrain.Name = "colNumeroTerrain";
            colNumeroTerrain.ReadOnly = true;
            //
            // colDateHeure
            //
            colDateHeure.DataPropertyName = "DateHeure";
            colDateHeure.HeaderText = "Date / heure";
            colDateHeure.Name = "colDateHeure";
            colDateHeure.ReadOnly = true;
            //
            // colVisibilite
            //
            colVisibilite.DataPropertyName = "Visibilite";
            colVisibilite.HeaderText = "Visibilité";
            colVisibilite.Name = "colVisibilite";
            colVisibilite.ReadOnly = true;
            //
            // colStatut
            //
            colStatut.DataPropertyName = "Statut";
            colStatut.HeaderText = "Statut";
            colStatut.Name = "colStatut";
            colStatut.ReadOnly = true;
            //
            // lblMessage
            //
            lblMessage.AutoSize = true;
            lblMessage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblMessage.Location = new Point(20, 565);
            lblMessage.MaximumSize = new Size(900, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 20);
            lblMessage.TabIndex = 6;
            //
            // EtatMatchsForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(940, 600);
            Controls.Add(lblMessage);
            Controls.Add(grdMatchs);
            Controls.Add(lblTerrains);
            Controls.Add(btnRafraichir);
            Controls.Add(chkTousLesSites);
            Controls.Add(cboSite);
            Controls.Add(lblSite);
            Name = "EtatMatchsForm";
            Text = "État des matchs et terrains";
            Load += EtatMatchsForm_Load;
            ((System.ComponentModel.ISupportInitialize)grdMatchs).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblSite;
        private ComboBox cboSite;
        private CheckBox chkTousLesSites;
        private Button btnRafraichir;
        private Label lblTerrains;
        private DataGridView grdMatchs;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colNomSite;
        private DataGridViewTextBoxColumn colTerrainId;
        private DataGridViewTextBoxColumn colNumeroTerrain;
        private DataGridViewTextBoxColumn colDateHeure;
        private DataGridViewTextBoxColumn colVisibilite;
        private DataGridViewTextBoxColumn colStatut;
        private Label lblMessage;
    }
}
