namespace PadelManager.WinForms
{
    partial class MatchDetailForm
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
            lblInfo = new Label();
            grdJoueurs = new DataGridView();
            colMatricule = new DataGridViewTextBoxColumn();
            colStatut = new DataGridViewTextBoxColumn();
            lblMessage = new Label();
            ((System.ComponentModel.ISupportInitialize)grdJoueurs).BeginInit();
            SuspendLayout();
            //
            // lblInfo
            //
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(20, 20);
            lblInfo.MaximumSize = new Size(500, 0);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(0, 20);
            lblInfo.TabIndex = 0;
            //
            // grdJoueurs
            //
            grdJoueurs.AllowUserToAddRows = false;
            grdJoueurs.AllowUserToDeleteRows = false;
            grdJoueurs.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grdJoueurs.AutoGenerateColumns = false;
            grdJoueurs.Columns.AddRange(new DataGridViewColumn[] { colMatricule, colStatut });
            grdJoueurs.Location = new Point(20, 175);
            grdJoueurs.MultiSelect = false;
            grdJoueurs.Name = "grdJoueurs";
            grdJoueurs.ReadOnly = true;
            grdJoueurs.RowHeadersWidth = 25;
            grdJoueurs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdJoueurs.Size = new Size(500, 180);
            grdJoueurs.TabIndex = 1;
            //
            // colMatricule
            //
            colMatricule.DataPropertyName = "MembreMatricule";
            colMatricule.HeaderText = "Matricule";
            colMatricule.Name = "colMatricule";
            colMatricule.ReadOnly = true;
            //
            // colStatut
            //
            colStatut.DataPropertyName = "Statut";
            colStatut.HeaderText = "Statut de paiement";
            colStatut.Name = "colStatut";
            colStatut.ReadOnly = true;
            //
            // lblMessage
            //
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(20, 365);
            lblMessage.MaximumSize = new Size(500, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 20);
            lblMessage.TabIndex = 2;
            //
            // MatchDetailForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(540, 400);
            Controls.Add(lblMessage);
            Controls.Add(grdJoueurs);
            Controls.Add(lblInfo);
            Name = "MatchDetailForm";
            Text = "Détail du match";
            Load += MatchDetailForm_Load;
            ((System.ComponentModel.ISupportInitialize)grdJoueurs).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblInfo;
        private DataGridView grdJoueurs;
        private DataGridViewTextBoxColumn colMatricule;
        private DataGridViewTextBoxColumn colStatut;
        private Label lblMessage;
    }
}
