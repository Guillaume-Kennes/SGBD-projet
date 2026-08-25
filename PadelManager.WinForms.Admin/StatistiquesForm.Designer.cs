namespace PadelManager.WinForms.Admin
{
    partial class StatistiquesForm
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
            grdStatistiques = new DataGridView();
            colNomSite = new DataGridViewTextBoxColumn();
            colNombreMatchsPublics = new DataGridViewTextBoxColumn();
            colNombreMatchsPrives = new DataGridViewTextBoxColumn();
            colTauxOccupation = new DataGridViewTextBoxColumn();
            colMembresActifs = new DataGridViewTextBoxColumn();
            lblMessage = new Label();
            ((System.ComponentModel.ISupportInitialize)grdStatistiques).BeginInit();
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
            // grdStatistiques
            //
            grdStatistiques.AllowUserToAddRows = false;
            grdStatistiques.AllowUserToDeleteRows = false;
            grdStatistiques.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grdStatistiques.AutoGenerateColumns = false;
            grdStatistiques.Columns.AddRange(new DataGridViewColumn[] { colNomSite, colNombreMatchsPublics, colNombreMatchsPrives, colTauxOccupation, colMembresActifs });
            grdStatistiques.Location = new Point(20, 130);
            grdStatistiques.Name = "grdStatistiques";
            grdStatistiques.ReadOnly = true;
            grdStatistiques.RowHeadersWidth = 25;
            grdStatistiques.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdStatistiques.Size = new Size(650, 240);
            grdStatistiques.TabIndex = 4;
            //
            // colNomSite
            //
            colNomSite.DataPropertyName = "NomSite";
            colNomSite.HeaderText = "Site";
            colNomSite.Name = "colNomSite";
            colNomSite.ReadOnly = true;
            //
            // colNombreMatchsPublics
            //
            colNombreMatchsPublics.DataPropertyName = "NombreMatchsPublics";
            colNombreMatchsPublics.HeaderText = "Matchs publics";
            colNombreMatchsPublics.Name = "colNombreMatchsPublics";
            colNombreMatchsPublics.ReadOnly = true;
            //
            // colNombreMatchsPrives
            //
            colNombreMatchsPrives.DataPropertyName = "NombreMatchsPrives";
            colNombreMatchsPrives.HeaderText = "Matchs privés";
            colNombreMatchsPrives.Name = "colNombreMatchsPrives";
            colNombreMatchsPrives.ReadOnly = true;
            //
            // colTauxOccupation
            //
            colTauxOccupation.DataPropertyName = "TauxOccupation";
            colTauxOccupation.HeaderText = "Taux d'occupation";
            colTauxOccupation.Name = "colTauxOccupation";
            colTauxOccupation.ReadOnly = true;
            //
            // colMembresActifs
            //
            colMembresActifs.DataPropertyName = "MembresActifs";
            colMembresActifs.HeaderText = "Membres actifs";
            colMembresActifs.Name = "colMembresActifs";
            colMembresActifs.ReadOnly = true;
            //
            // lblMessage
            //
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(20, 385);
            lblMessage.MaximumSize = new Size(650, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 20);
            lblMessage.TabIndex = 5;
            //
            // StatistiquesForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(690, 420);
            Controls.Add(lblMessage);
            Controls.Add(grdStatistiques);
            Controls.Add(btnRafraichir);
            Controls.Add(chkTousLesSites);
            Controls.Add(cboSite);
            Controls.Add(lblSite);
            Name = "StatistiquesForm";
            Text = "Statistiques";
            Load += StatistiquesForm_Load;
            ((System.ComponentModel.ISupportInitialize)grdStatistiques).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblSite;
        private ComboBox cboSite;
        private CheckBox chkTousLesSites;
        private Button btnRafraichir;
        private DataGridView grdStatistiques;
        private DataGridViewTextBoxColumn colNomSite;
        private DataGridViewTextBoxColumn colNombreMatchsPublics;
        private DataGridViewTextBoxColumn colNombreMatchsPrives;
        private DataGridViewTextBoxColumn colTauxOccupation;
        private DataGridViewTextBoxColumn colMembresActifs;
        private Label lblMessage;
    }
}
