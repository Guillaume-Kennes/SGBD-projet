namespace PadelManager.WinForms.Admin
{
    partial class MembresForm
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
            grdMembres = new DataGridView();
            colMatricule = new DataGridViewTextBoxColumn();
            colTypeMembre = new DataGridViewTextBoxColumn();
            colSiteId = new DataGridViewTextBoxColumn();
            colDetteActive = new DataGridViewCheckBoxColumn();
            colPenaliteActive = new DataGridViewCheckBoxColumn();
            lblMessage = new Label();
            ((System.ComponentModel.ISupportInitialize)grdMembres).BeginInit();
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
            // grdMembres
            //
            grdMembres.AllowUserToAddRows = false;
            grdMembres.AllowUserToDeleteRows = false;
            grdMembres.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            grdMembres.AutoGenerateColumns = false;
            grdMembres.Columns.AddRange(new DataGridViewColumn[] { colMatricule, colTypeMembre, colSiteId, colDetteActive, colPenaliteActive });
            grdMembres.Location = new Point(20, 130);
            grdMembres.Name = "grdMembres";
            grdMembres.ReadOnly = true;
            grdMembres.RowHeadersWidth = 25;
            grdMembres.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdMembres.Size = new Size(660, 380);
            grdMembres.TabIndex = 4;
            //
            // colMatricule
            //
            colMatricule.DataPropertyName = "Matricule";
            colMatricule.HeaderText = "Matricule";
            colMatricule.Name = "colMatricule";
            colMatricule.ReadOnly = true;
            colMatricule.Width = 110;
            //
            // colTypeMembre
            //
            colTypeMembre.DataPropertyName = "TypeMembre";
            colTypeMembre.HeaderText = "Type";
            colTypeMembre.Name = "colTypeMembre";
            colTypeMembre.ReadOnly = true;
            colTypeMembre.Width = 110;
            //
            // colSiteId
            //
            colSiteId.DataPropertyName = "SiteId";
            colSiteId.HeaderText = "Site";
            colSiteId.Name = "colSiteId";
            colSiteId.ReadOnly = true;
            colSiteId.Width = 90;
            //
            // colDetteActive
            //
            colDetteActive.DataPropertyName = "DetteActive";
            colDetteActive.HeaderText = "Dette active";
            colDetteActive.Name = "colDetteActive";
            colDetteActive.ReadOnly = true;
            colDetteActive.Width = 130;
            //
            // colPenaliteActive
            //
            colPenaliteActive.DataPropertyName = "PenaliteActive";
            colPenaliteActive.HeaderText = "Pénalité active";
            colPenaliteActive.Name = "colPenaliteActive";
            colPenaliteActive.ReadOnly = true;
            colPenaliteActive.Width = 160;
            //
            // lblMessage
            //
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(20, 520);
            lblMessage.MaximumSize = new Size(660, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 20);
            lblMessage.TabIndex = 5;
            //
            // MembresForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 560);
            Controls.Add(lblMessage);
            Controls.Add(grdMembres);
            Controls.Add(btnRafraichir);
            Controls.Add(chkTousLesSites);
            Controls.Add(cboSite);
            Controls.Add(lblSite);
            Name = "MembresForm";
            Text = "Membres";
            Load += MembresForm_Load;
            ((System.ComponentModel.ISupportInitialize)grdMembres).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblSite;
        private ComboBox cboSite;
        private CheckBox chkTousLesSites;
        private Button btnRafraichir;
        private DataGridView grdMembres;
        private DataGridViewTextBoxColumn colMatricule;
        private DataGridViewTextBoxColumn colTypeMembre;
        private DataGridViewTextBoxColumn colSiteId;
        private DataGridViewCheckBoxColumn colDetteActive;
        private DataGridViewCheckBoxColumn colPenaliteActive;
        private Label lblMessage;
    }
}
