namespace PadelManager.WinForms
{
    partial class ReservationsForm
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
            grdReservations = new DataGridView();
            colNomSite = new DataGridViewTextBoxColumn();
            colTerrain = new DataGridViewTextBoxColumn();
            colDateHeure = new DataGridViewTextBoxColumn();
            colVisibilite = new DataGridViewTextBoxColumn();
            colStatut = new DataGridViewTextBoxColumn();
            colEstOrganisateur = new DataGridViewCheckBoxColumn();
            btnRafraichir = new Button();
            btnVoirDetail = new Button();
            lblMessage = new Label();
            ((System.ComponentModel.ISupportInitialize)grdReservations).BeginInit();
            SuspendLayout();
            //
            // grdReservations
            //
            grdReservations.AllowUserToAddRows = false;
            grdReservations.AllowUserToDeleteRows = false;
            grdReservations.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grdReservations.AutoGenerateColumns = false;
            grdReservations.Columns.AddRange(new DataGridViewColumn[] { colNomSite, colTerrain, colDateHeure, colVisibilite, colStatut, colEstOrganisateur });
            grdReservations.Location = new Point(20, 20);
            grdReservations.MultiSelect = false;
            grdReservations.Name = "grdReservations";
            grdReservations.ReadOnly = true;
            grdReservations.RowHeadersWidth = 25;
            grdReservations.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdReservations.Size = new Size(700, 300);
            grdReservations.TabIndex = 0;
            //
            // colNomSite
            //
            colNomSite.DataPropertyName = "NomSite";
            colNomSite.HeaderText = "Site";
            colNomSite.Name = "colNomSite";
            colNomSite.ReadOnly = true;
            //
            // colTerrain
            //
            colTerrain.DataPropertyName = "NumeroTerrain";
            colTerrain.HeaderText = "Terrain";
            colTerrain.Name = "colTerrain";
            colTerrain.ReadOnly = true;
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
            // colEstOrganisateur
            //
            colEstOrganisateur.DataPropertyName = "EstOrganisateur";
            colEstOrganisateur.HeaderText = "Organisateur";
            colEstOrganisateur.Name = "colEstOrganisateur";
            colEstOrganisateur.ReadOnly = true;
            //
            // btnRafraichir
            //
            btnRafraichir.Location = new Point(20, 335);
            btnRafraichir.Name = "btnRafraichir";
            btnRafraichir.Size = new Size(150, 29);
            btnRafraichir.TabIndex = 1;
            btnRafraichir.Text = "Rafraîchir";
            btnRafraichir.UseVisualStyleBackColor = true;
            btnRafraichir.Click += btnRafraichir_Click;
            //
            // btnVoirDetail
            //
            btnVoirDetail.Location = new Point(180, 335);
            btnVoirDetail.Name = "btnVoirDetail";
            btnVoirDetail.Size = new Size(150, 29);
            btnVoirDetail.TabIndex = 2;
            btnVoirDetail.Text = "Voir le détail";
            btnVoirDetail.UseVisualStyleBackColor = true;
            btnVoirDetail.Click += btnVoirDetail_Click;
            //
            // lblMessage
            //
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(20, 375);
            lblMessage.MaximumSize = new Size(700, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 20);
            lblMessage.TabIndex = 3;
            //
            // ReservationsForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(740, 420);
            Controls.Add(lblMessage);
            Controls.Add(btnVoirDetail);
            Controls.Add(btnRafraichir);
            Controls.Add(grdReservations);
            Name = "ReservationsForm";
            Text = "Mes réservations";
            Load += ReservationsForm_Load;
            ((System.ComponentModel.ISupportInitialize)grdReservations).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView grdReservations;
        private DataGridViewTextBoxColumn colNomSite;
        private DataGridViewTextBoxColumn colTerrain;
        private DataGridViewTextBoxColumn colDateHeure;
        private DataGridViewTextBoxColumn colVisibilite;
        private DataGridViewTextBoxColumn colStatut;
        private DataGridViewCheckBoxColumn colEstOrganisateur;
        private Button btnRafraichir;
        private Button btnVoirDetail;
        private Label lblMessage;
    }
}
