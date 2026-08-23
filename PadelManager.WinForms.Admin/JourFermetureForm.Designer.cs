namespace PadelManager.WinForms.Admin
{
    partial class JourFermetureForm
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
            lblDate = new Label();
            dtpDate = new DateTimePicker();
            btnDeclarer = new Button();
            lblAnnee = new Label();
            numAnnee = new NumericUpDown();
            btnCharger = new Button();
            lstFermetures = new ListBox();
            btnSupprimer = new Button();
            lblMessage = new Label();
            ((System.ComponentModel.ISupportInitialize)numAnnee).BeginInit();
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
            //
            // chkTousLesSites
            //
            chkTousLesSites.AutoSize = true;
            chkTousLesSites.Location = new Point(140, 55);
            chkTousLesSites.Name = "chkTousLesSites";
            chkTousLesSites.Size = new Size(160, 24);
            chkTousLesSites.TabIndex = 2;
            chkTousLesSites.Text = "Tous les sites (global)";
            chkTousLesSites.UseVisualStyleBackColor = true;
            chkTousLesSites.CheckedChanged += chkTousLesSites_CheckedChanged;
            //
            // lblDate
            //
            lblDate.AutoSize = true;
            lblDate.Location = new Point(20, 93);
            lblDate.Name = "lblDate";
            lblDate.Size = new Size(41, 20);
            lblDate.TabIndex = 3;
            lblDate.Text = "Date";
            //
            // dtpDate
            //
            dtpDate.Format = DateTimePickerFormat.Short;
            dtpDate.Location = new Point(140, 90);
            dtpDate.Name = "dtpDate";
            dtpDate.Size = new Size(140, 27);
            dtpDate.TabIndex = 4;
            //
            // btnDeclarer
            //
            btnDeclarer.Location = new Point(300, 89);
            btnDeclarer.Name = "btnDeclarer";
            btnDeclarer.Size = new Size(140, 29);
            btnDeclarer.TabIndex = 5;
            btnDeclarer.Text = "Déclarer";
            btnDeclarer.UseVisualStyleBackColor = true;
            btnDeclarer.Click += btnDeclarer_Click;
            //
            // lblAnnee
            //
            lblAnnee.AutoSize = true;
            lblAnnee.Location = new Point(20, 138);
            lblAnnee.Name = "lblAnnee";
            lblAnnee.Size = new Size(48, 20);
            lblAnnee.TabIndex = 6;
            lblAnnee.Text = "Année";
            //
            // numAnnee
            //
            numAnnee.Location = new Point(140, 135);
            numAnnee.Maximum = new decimal(new int[] { 2100, 0, 0, 0 });
            numAnnee.Minimum = new decimal(new int[] { 2000, 0, 0, 0 });
            numAnnee.Name = "numAnnee";
            numAnnee.Size = new Size(100, 27);
            numAnnee.TabIndex = 7;
            numAnnee.Value = new decimal(new int[] { 2026, 0, 0, 0 });
            //
            // btnCharger
            //
            btnCharger.Location = new Point(300, 133);
            btnCharger.Name = "btnCharger";
            btnCharger.Size = new Size(140, 29);
            btnCharger.TabIndex = 8;
            btnCharger.Text = "Charger la liste";
            btnCharger.UseVisualStyleBackColor = true;
            btnCharger.Click += btnCharger_Click;
            //
            // lstFermetures
            //
            lstFermetures.FormattingEnabled = true;
            lstFermetures.ItemHeight = 20;
            lstFermetures.Location = new Point(20, 180);
            lstFermetures.Name = "lstFermetures";
            lstFermetures.Size = new Size(420, 144);
            lstFermetures.TabIndex = 9;
            //
            // btnSupprimer
            //
            btnSupprimer.Location = new Point(20, 335);
            btnSupprimer.Name = "btnSupprimer";
            btnSupprimer.Size = new Size(180, 29);
            btnSupprimer.TabIndex = 10;
            btnSupprimer.Text = "Annuler la sélection";
            btnSupprimer.UseVisualStyleBackColor = true;
            btnSupprimer.Click += btnSupprimer_Click;
            //
            // lblMessage
            //
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(20, 380);
            lblMessage.MaximumSize = new Size(420, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 20);
            lblMessage.TabIndex = 11;
            //
            // JourFermetureForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(464, 440);
            Controls.Add(lblMessage);
            Controls.Add(btnSupprimer);
            Controls.Add(lstFermetures);
            Controls.Add(btnCharger);
            Controls.Add(numAnnee);
            Controls.Add(lblAnnee);
            Controls.Add(btnDeclarer);
            Controls.Add(dtpDate);
            Controls.Add(lblDate);
            Controls.Add(chkTousLesSites);
            Controls.Add(cboSite);
            Controls.Add(lblSite);
            Name = "JourFermetureForm";
            Text = "Fermetures ponctuelles";
            Load += JourFermetureForm_Load;
            ((System.ComponentModel.ISupportInitialize)numAnnee).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblSite;
        private ComboBox cboSite;
        private CheckBox chkTousLesSites;
        private Label lblDate;
        private DateTimePicker dtpDate;
        private Button btnDeclarer;
        private Label lblAnnee;
        private NumericUpDown numAnnee;
        private Button btnCharger;
        private ListBox lstFermetures;
        private Button btnSupprimer;
        private Label lblMessage;
    }
}
