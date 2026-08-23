namespace PadelManager.WinForms.Admin
{
    partial class HoraireSiteForm
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
            lblAnnee = new Label();
            numAnnee = new NumericUpDown();
            btnCharger = new Button();
            grpJours = new GroupBox();
            chkDim = new CheckBox();
            chkSam = new CheckBox();
            chkVen = new CheckBox();
            chkJeu = new CheckBox();
            chkMer = new CheckBox();
            chkMar = new CheckBox();
            chkLun = new CheckBox();
            lblHeureDebut = new Label();
            dtpHeureDebut = new DateTimePicker();
            lblHeureFin = new Label();
            dtpHeureFin = new DateTimePicker();
            btnEnregistrer = new Button();
            lblMessage = new Label();
            ((System.ComponentModel.ISupportInitialize)numAnnee).BeginInit();
            grpJours.SuspendLayout();
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
            // lblAnnee
            //
            lblAnnee.AutoSize = true;
            lblAnnee.Location = new Point(20, 63);
            lblAnnee.Name = "lblAnnee";
            lblAnnee.Size = new Size(48, 20);
            lblAnnee.TabIndex = 2;
            lblAnnee.Text = "Année";
            //
            // numAnnee
            //
            numAnnee.Location = new Point(140, 60);
            numAnnee.Maximum = new decimal(new int[] { 2100, 0, 0, 0 });
            numAnnee.Minimum = new decimal(new int[] { 2000, 0, 0, 0 });
            numAnnee.Name = "numAnnee";
            numAnnee.Size = new Size(100, 27);
            numAnnee.TabIndex = 3;
            numAnnee.Value = new decimal(new int[] { 2026, 0, 0, 0 });
            //
            // btnCharger
            //
            btnCharger.Location = new Point(140, 98);
            btnCharger.Name = "btnCharger";
            btnCharger.Size = new Size(110, 29);
            btnCharger.TabIndex = 4;
            btnCharger.Text = "Charger";
            btnCharger.UseVisualStyleBackColor = true;
            btnCharger.Click += btnCharger_Click;
            //
            // grpJours
            //
            grpJours.Controls.Add(chkDim);
            grpJours.Controls.Add(chkSam);
            grpJours.Controls.Add(chkVen);
            grpJours.Controls.Add(chkJeu);
            grpJours.Controls.Add(chkMer);
            grpJours.Controls.Add(chkMar);
            grpJours.Controls.Add(chkLun);
            grpJours.Location = new Point(20, 145);
            grpJours.Name = "grpJours";
            grpJours.Size = new Size(480, 70);
            grpJours.TabIndex = 5;
            grpJours.TabStop = false;
            grpJours.Text = "Jours d'ouverture";
            //
            // chkLun
            //
            chkLun.AutoSize = true;
            chkLun.Location = new Point(15, 30);
            chkLun.Name = "chkLun";
            chkLun.Size = new Size(58, 24);
            chkLun.TabIndex = 0;
            chkLun.Text = "LUN";
            chkLun.UseVisualStyleBackColor = true;
            //
            // chkMar
            //
            chkMar.AutoSize = true;
            chkMar.Location = new Point(80, 30);
            chkMar.Name = "chkMar";
            chkMar.Size = new Size(60, 24);
            chkMar.TabIndex = 1;
            chkMar.Text = "MAR";
            chkMar.UseVisualStyleBackColor = true;
            //
            // chkMer
            //
            chkMer.AutoSize = true;
            chkMer.Location = new Point(147, 30);
            chkMer.Name = "chkMer";
            chkMer.Size = new Size(58, 24);
            chkMer.TabIndex = 2;
            chkMer.Text = "MER";
            chkMer.UseVisualStyleBackColor = true;
            //
            // chkJeu
            //
            chkJeu.AutoSize = true;
            chkJeu.Location = new Point(212, 30);
            chkJeu.Name = "chkJeu";
            chkJeu.Size = new Size(53, 24);
            chkJeu.TabIndex = 3;
            chkJeu.Text = "JEU";
            chkJeu.UseVisualStyleBackColor = true;
            //
            // chkVen
            //
            chkVen.AutoSize = true;
            chkVen.Location = new Point(272, 30);
            chkVen.Name = "chkVen";
            chkVen.Size = new Size(56, 24);
            chkVen.TabIndex = 4;
            chkVen.Text = "VEN";
            chkVen.UseVisualStyleBackColor = true;
            //
            // chkSam
            //
            chkSam.AutoSize = true;
            chkSam.Location = new Point(335, 30);
            chkSam.Name = "chkSam";
            chkSam.Size = new Size(63, 24);
            chkSam.TabIndex = 5;
            chkSam.Text = "SAM";
            chkSam.UseVisualStyleBackColor = true;
            //
            // chkDim
            //
            chkDim.AutoSize = true;
            chkDim.Location = new Point(405, 30);
            chkDim.Name = "chkDim";
            chkDim.Size = new Size(58, 24);
            chkDim.TabIndex = 6;
            chkDim.Text = "DIM";
            chkDim.UseVisualStyleBackColor = true;
            //
            // lblHeureDebut
            //
            lblHeureDebut.AutoSize = true;
            lblHeureDebut.Location = new Point(20, 235);
            lblHeureDebut.Name = "lblHeureDebut";
            lblHeureDebut.Size = new Size(101, 20);
            lblHeureDebut.TabIndex = 6;
            lblHeureDebut.Text = "Heure de début";
            //
            // dtpHeureDebut
            //
            dtpHeureDebut.Format = DateTimePickerFormat.Time;
            dtpHeureDebut.Location = new Point(140, 232);
            dtpHeureDebut.Name = "dtpHeureDebut";
            dtpHeureDebut.ShowUpDown = true;
            dtpHeureDebut.Size = new Size(100, 27);
            dtpHeureDebut.TabIndex = 7;
            //
            // lblHeureFin
            //
            lblHeureFin.AutoSize = true;
            lblHeureFin.Location = new Point(270, 235);
            lblHeureFin.Name = "lblHeureFin";
            lblHeureFin.Size = new Size(80, 20);
            lblHeureFin.TabIndex = 8;
            lblHeureFin.Text = "Heure de fin";
            //
            // dtpHeureFin
            //
            dtpHeureFin.Format = DateTimePickerFormat.Time;
            dtpHeureFin.Location = new Point(400, 232);
            dtpHeureFin.Name = "dtpHeureFin";
            dtpHeureFin.ShowUpDown = true;
            dtpHeureFin.Size = new Size(100, 27);
            dtpHeureFin.TabIndex = 9;
            //
            // btnEnregistrer
            //
            btnEnregistrer.Location = new Point(140, 280);
            btnEnregistrer.Name = "btnEnregistrer";
            btnEnregistrer.Size = new Size(140, 29);
            btnEnregistrer.TabIndex = 10;
            btnEnregistrer.Text = "Enregistrer";
            btnEnregistrer.UseVisualStyleBackColor = true;
            btnEnregistrer.Click += btnEnregistrer_Click;
            //
            // lblMessage
            //
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(20, 330);
            lblMessage.MaximumSize = new Size(480, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 20);
            lblMessage.TabIndex = 11;
            //
            // HoraireSiteForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(524, 391);
            Controls.Add(lblMessage);
            Controls.Add(btnEnregistrer);
            Controls.Add(dtpHeureFin);
            Controls.Add(lblHeureFin);
            Controls.Add(dtpHeureDebut);
            Controls.Add(lblHeureDebut);
            Controls.Add(grpJours);
            Controls.Add(btnCharger);
            Controls.Add(numAnnee);
            Controls.Add(lblAnnee);
            Controls.Add(cboSite);
            Controls.Add(lblSite);
            Name = "HoraireSiteForm";
            Text = "Paramétrage annuel du site";
            Load += HoraireSiteForm_Load;
            ((System.ComponentModel.ISupportInitialize)numAnnee).EndInit();
            grpJours.ResumeLayout(false);
            grpJours.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblSite;
        private ComboBox cboSite;
        private Label lblAnnee;
        private NumericUpDown numAnnee;
        private Button btnCharger;
        private GroupBox grpJours;
        private CheckBox chkDim;
        private CheckBox chkSam;
        private CheckBox chkVen;
        private CheckBox chkJeu;
        private CheckBox chkMer;
        private CheckBox chkMar;
        private CheckBox chkLun;
        private Label lblHeureDebut;
        private DateTimePicker dtpHeureDebut;
        private Label lblHeureFin;
        private DateTimePicker dtpHeureFin;
        private Button btnEnregistrer;
        private Label lblMessage;
    }
}
