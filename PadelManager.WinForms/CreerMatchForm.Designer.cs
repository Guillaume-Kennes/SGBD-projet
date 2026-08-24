namespace PadelManager.WinForms
{
    partial class CreerMatchForm
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
            lblDate = new Label();
            dtpDate = new DateTimePicker();
            lblFenetre = new Label();
            btnRechercher = new Button();
            grdCreneaux = new DataGridView();
            colTerrain = new DataGridViewTextBoxColumn();
            colHeureDebut = new DataGridViewTextBoxColumn();
            colHeureFin = new DataGridViewTextBoxColumn();
            lblJoueurs = new Label();
            txtJoueur1 = new TextBox();
            txtJoueur2 = new TextBox();
            txtJoueur3 = new TextBox();
            btnCreer = new Button();
            lblMessage = new Label();
            ((System.ComponentModel.ISupportInitialize)grdCreneaux).BeginInit();
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
            // lblDate
            //
            lblDate.AutoSize = true;
            lblDate.Location = new Point(20, 63);
            lblDate.Name = "lblDate";
            lblDate.Size = new Size(41, 20);
            lblDate.TabIndex = 2;
            lblDate.Text = "Date";
            //
            // dtpDate
            //
            dtpDate.Format = DateTimePickerFormat.Short;
            dtpDate.Location = new Point(140, 60);
            dtpDate.Name = "dtpDate";
            dtpDate.Size = new Size(150, 27);
            dtpDate.TabIndex = 3;
            dtpDate.ValueChanged += dtpDate_ValueChanged;
            //
            // lblFenetre
            //
            lblFenetre.AutoSize = true;
            lblFenetre.ForeColor = SystemColors.GrayText;
            lblFenetre.Location = new Point(140, 88);
            lblFenetre.Name = "lblFenetre";
            lblFenetre.Size = new Size(0, 20);
            lblFenetre.TabIndex = 12;
            //
            // btnRechercher
            //
            btnRechercher.Location = new Point(140, 118);
            btnRechercher.Name = "btnRechercher";
            btnRechercher.Size = new Size(150, 29);
            btnRechercher.TabIndex = 4;
            btnRechercher.Text = "Rechercher";
            btnRechercher.UseVisualStyleBackColor = true;
            btnRechercher.Click += btnRechercher_Click;
            //
            // grdCreneaux
            //
            grdCreneaux.AllowUserToAddRows = false;
            grdCreneaux.AllowUserToDeleteRows = false;
            grdCreneaux.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grdCreneaux.AutoGenerateColumns = false;
            grdCreneaux.Columns.AddRange(new DataGridViewColumn[] { colTerrain, colHeureDebut, colHeureFin });
            grdCreneaux.Location = new Point(20, 165);
            grdCreneaux.MultiSelect = false;
            grdCreneaux.Name = "grdCreneaux";
            grdCreneaux.ReadOnly = true;
            grdCreneaux.RowHeadersWidth = 25;
            grdCreneaux.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdCreneaux.Size = new Size(500, 220);
            grdCreneaux.TabIndex = 5;
            //
            // colTerrain
            //
            colTerrain.DataPropertyName = "NumeroTerrain";
            colTerrain.HeaderText = "Terrain";
            colTerrain.Name = "colTerrain";
            colTerrain.ReadOnly = true;
            //
            // colHeureDebut
            //
            colHeureDebut.DataPropertyName = "HeureDebut";
            colHeureDebut.HeaderText = "Heure début";
            colHeureDebut.Name = "colHeureDebut";
            colHeureDebut.ReadOnly = true;
            //
            // colHeureFin
            //
            colHeureFin.DataPropertyName = "HeureFin";
            colHeureFin.HeaderText = "Heure fin";
            colHeureFin.Name = "colHeureFin";
            colHeureFin.ReadOnly = true;
            //
            // lblJoueurs
            //
            lblJoueurs.AutoSize = true;
            lblJoueurs.Location = new Point(20, 400);
            lblJoueurs.Name = "lblJoueurs";
            lblJoueurs.Size = new Size(360, 20);
            lblJoueurs.TabIndex = 6;
            lblJoueurs.Text = "Joueurs à ajouter (matricule, jusqu'à 3, facultatif)";
            //
            // txtJoueur1
            //
            txtJoueur1.Location = new Point(20, 425);
            txtJoueur1.Name = "txtJoueur1";
            txtJoueur1.Size = new Size(120, 27);
            txtJoueur1.TabIndex = 7;
            //
            // txtJoueur2
            //
            txtJoueur2.Location = new Point(150, 425);
            txtJoueur2.Name = "txtJoueur2";
            txtJoueur2.Size = new Size(120, 27);
            txtJoueur2.TabIndex = 8;
            //
            // txtJoueur3
            //
            txtJoueur3.Location = new Point(280, 425);
            txtJoueur3.Name = "txtJoueur3";
            txtJoueur3.Size = new Size(120, 27);
            txtJoueur3.TabIndex = 9;
            //
            // btnCreer
            //
            btnCreer.Location = new Point(20, 465);
            btnCreer.Name = "btnCreer";
            btnCreer.Size = new Size(220, 34);
            btnCreer.TabIndex = 10;
            btnCreer.Text = "Créer et payer 15€";
            btnCreer.UseVisualStyleBackColor = true;
            btnCreer.Click += btnCreer_Click;
            //
            // lblMessage
            //
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(20, 515);
            lblMessage.MaximumSize = new Size(500, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 20);
            lblMessage.TabIndex = 11;
            //
            // CreerMatchForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(540, 560);
            Controls.Add(lblMessage);
            Controls.Add(btnCreer);
            Controls.Add(txtJoueur3);
            Controls.Add(txtJoueur2);
            Controls.Add(txtJoueur1);
            Controls.Add(lblJoueurs);
            Controls.Add(grdCreneaux);
            Controls.Add(btnRechercher);
            Controls.Add(lblFenetre);
            Controls.Add(dtpDate);
            Controls.Add(lblDate);
            Controls.Add(cboSite);
            Controls.Add(lblSite);
            Name = "CreerMatchForm";
            Text = "Créer un match privé";
            Load += CreerMatchForm_Load;
            ((System.ComponentModel.ISupportInitialize)grdCreneaux).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblSite;
        private ComboBox cboSite;
        private Label lblDate;
        private DateTimePicker dtpDate;
        private Label lblFenetre;
        private Button btnRechercher;
        private DataGridView grdCreneaux;
        private DataGridViewTextBoxColumn colTerrain;
        private DataGridViewTextBoxColumn colHeureDebut;
        private DataGridViewTextBoxColumn colHeureFin;
        private Label lblJoueurs;
        private TextBox txtJoueur1;
        private TextBox txtJoueur2;
        private TextBox txtJoueur3;
        private Button btnCreer;
        private Label lblMessage;
    }
}
