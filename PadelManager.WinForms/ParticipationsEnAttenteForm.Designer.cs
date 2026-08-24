namespace PadelManager.WinForms
{
    partial class ParticipationsEnAttenteForm
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
            grdParticipations = new DataGridView();
            colNomSite = new DataGridViewTextBoxColumn();
            colTerrain = new DataGridViewTextBoxColumn();
            colDateHeure = new DataGridViewTextBoxColumn();
            colOrganisateur = new DataGridViewTextBoxColumn();
            btnRafraichir = new Button();
            btnPayer = new Button();
            lblMontant = new Label();
            lblMessage = new Label();
            ((System.ComponentModel.ISupportInitialize)grdParticipations).BeginInit();
            SuspendLayout();
            //
            // grdParticipations
            //
            grdParticipations.AllowUserToAddRows = false;
            grdParticipations.AllowUserToDeleteRows = false;
            grdParticipations.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grdParticipations.AutoGenerateColumns = false;
            grdParticipations.Columns.AddRange(new DataGridViewColumn[] { colNomSite, colTerrain, colDateHeure, colOrganisateur });
            grdParticipations.Location = new Point(20, 20);
            grdParticipations.MultiSelect = false;
            grdParticipations.Name = "grdParticipations";
            grdParticipations.ReadOnly = true;
            grdParticipations.RowHeadersWidth = 25;
            grdParticipations.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdParticipations.Size = new Size(600, 300);
            grdParticipations.TabIndex = 0;
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
            // colOrganisateur
            //
            colOrganisateur.DataPropertyName = "OrganisateurMatricule";
            colOrganisateur.HeaderText = "Organisé par";
            colOrganisateur.Name = "colOrganisateur";
            colOrganisateur.ReadOnly = true;
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
            // btnPayer
            //
            btnPayer.Location = new Point(180, 335);
            btnPayer.Name = "btnPayer";
            btnPayer.Size = new Size(220, 29);
            btnPayer.TabIndex = 2;
            btnPayer.Text = "Payer ma participation 15€";
            btnPayer.UseVisualStyleBackColor = true;
            btnPayer.Click += btnPayer_Click;
            //
            // lblMontant
            //
            lblMontant.AutoSize = true;
            lblMontant.Location = new Point(410, 341);
            lblMontant.MaximumSize = new Size(400, 0);
            lblMontant.Name = "lblMontant";
            lblMontant.Size = new Size(0, 20);
            lblMontant.TabIndex = 3;
            //
            // lblMessage
            //
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(20, 385);
            lblMessage.MaximumSize = new Size(600, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 20);
            lblMessage.TabIndex = 4;
            //
            // ParticipationsEnAttenteForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(640, 430);
            Controls.Add(lblMessage);
            Controls.Add(lblMontant);
            Controls.Add(btnPayer);
            Controls.Add(btnRafraichir);
            Controls.Add(grdParticipations);
            Name = "ParticipationsEnAttenteForm";
            Text = "Matchs privés à payer";
            Load += ParticipationsEnAttenteForm_Load;
            ((System.ComponentModel.ISupportInitialize)grdParticipations).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView grdParticipations;
        private DataGridViewTextBoxColumn colNomSite;
        private DataGridViewTextBoxColumn colTerrain;
        private DataGridViewTextBoxColumn colDateHeure;
        private DataGridViewTextBoxColumn colOrganisateur;
        private Button btnRafraichir;
        private Button btnPayer;
        private Label lblMontant;
        private Label lblMessage;
    }
}
