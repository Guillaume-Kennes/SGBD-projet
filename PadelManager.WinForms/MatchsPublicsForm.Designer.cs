namespace PadelManager.WinForms
{
    partial class MatchsPublicsForm
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
            grdMatchs = new DataGridView();
            colNomSite = new DataGridViewTextBoxColumn();
            colTerrain = new DataGridViewTextBoxColumn();
            colDateHeure = new DataGridViewTextBoxColumn();
            colPlacesRestantes = new DataGridViewTextBoxColumn();
            btnRafraichir = new Button();
            btnRejoindre = new Button();
            lblMessage = new Label();
            ((System.ComponentModel.ISupportInitialize)grdMatchs).BeginInit();
            SuspendLayout();
            //
            // grdMatchs
            //
            grdMatchs.AllowUserToAddRows = false;
            grdMatchs.AllowUserToDeleteRows = false;
            grdMatchs.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grdMatchs.AutoGenerateColumns = false;
            grdMatchs.Columns.AddRange(new DataGridViewColumn[] { colNomSite, colTerrain, colDateHeure, colPlacesRestantes });
            grdMatchs.Location = new Point(20, 20);
            grdMatchs.MultiSelect = false;
            grdMatchs.Name = "grdMatchs";
            grdMatchs.ReadOnly = true;
            grdMatchs.RowHeadersWidth = 25;
            grdMatchs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdMatchs.Size = new Size(600, 300);
            grdMatchs.TabIndex = 0;
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
            // colPlacesRestantes
            //
            colPlacesRestantes.DataPropertyName = "PlacesRestantes";
            colPlacesRestantes.HeaderText = "Places restantes";
            colPlacesRestantes.Name = "colPlacesRestantes";
            colPlacesRestantes.ReadOnly = true;
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
            // btnRejoindre
            //
            btnRejoindre.Location = new Point(180, 335);
            btnRejoindre.Name = "btnRejoindre";
            btnRejoindre.Size = new Size(220, 29);
            btnRejoindre.TabIndex = 2;
            btnRejoindre.Text = "Rejoindre et payer 15€";
            btnRejoindre.UseVisualStyleBackColor = true;
            btnRejoindre.Click += btnRejoindre_Click;
            //
            // lblMessage
            //
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(20, 385);
            lblMessage.MaximumSize = new Size(600, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 20);
            lblMessage.TabIndex = 3;
            //
            // MatchsPublicsForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(640, 430);
            Controls.Add(lblMessage);
            Controls.Add(btnRejoindre);
            Controls.Add(btnRafraichir);
            Controls.Add(grdMatchs);
            Name = "MatchsPublicsForm";
            Text = "Matchs publics";
            Load += MatchsPublicsForm_Load;
            ((System.ComponentModel.ISupportInitialize)grdMatchs).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView grdMatchs;
        private DataGridViewTextBoxColumn colNomSite;
        private DataGridViewTextBoxColumn colTerrain;
        private DataGridViewTextBoxColumn colDateHeure;
        private DataGridViewTextBoxColumn colPlacesRestantes;
        private Button btnRafraichir;
        private Button btnRejoindre;
        private Label lblMessage;
    }
}
