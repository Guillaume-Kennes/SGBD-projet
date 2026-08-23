namespace PadelManager.WinForms.Admin
{
    partial class FermetureHebdoGlobaleForm
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
            btnEnregistrer = new Button();
            btnSupprimer = new Button();
            lblMessage = new Label();
            ((System.ComponentModel.ISupportInitialize)numAnnee).BeginInit();
            grpJours.SuspendLayout();
            SuspendLayout();
            //
            // lblAnnee
            //
            lblAnnee.AutoSize = true;
            lblAnnee.Location = new Point(20, 23);
            lblAnnee.Name = "lblAnnee";
            lblAnnee.Size = new Size(48, 20);
            lblAnnee.TabIndex = 0;
            lblAnnee.Text = "Année";
            //
            // numAnnee
            //
            numAnnee.Location = new Point(140, 20);
            numAnnee.Maximum = new decimal(new int[] { 2100, 0, 0, 0 });
            numAnnee.Minimum = new decimal(new int[] { 2000, 0, 0, 0 });
            numAnnee.Name = "numAnnee";
            numAnnee.Size = new Size(100, 27);
            numAnnee.TabIndex = 1;
            numAnnee.Value = new decimal(new int[] { 2026, 0, 0, 0 });
            //
            // btnCharger
            //
            btnCharger.Location = new Point(140, 58);
            btnCharger.Name = "btnCharger";
            btnCharger.Size = new Size(110, 29);
            btnCharger.TabIndex = 2;
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
            grpJours.Location = new Point(20, 105);
            grpJours.Name = "grpJours";
            grpJours.Size = new Size(480, 70);
            grpJours.TabIndex = 3;
            grpJours.TabStop = false;
            grpJours.Text = "Jours fermés globalement";
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
            // btnEnregistrer
            //
            btnEnregistrer.Location = new Point(20, 195);
            btnEnregistrer.Name = "btnEnregistrer";
            btnEnregistrer.Size = new Size(140, 29);
            btnEnregistrer.TabIndex = 4;
            btnEnregistrer.Text = "Enregistrer";
            btnEnregistrer.UseVisualStyleBackColor = true;
            btnEnregistrer.Click += btnEnregistrer_Click;
            //
            // btnSupprimer
            //
            btnSupprimer.Location = new Point(180, 195);
            btnSupprimer.Name = "btnSupprimer";
            btnSupprimer.Size = new Size(140, 29);
            btnSupprimer.TabIndex = 5;
            btnSupprimer.Text = "Supprimer";
            btnSupprimer.UseVisualStyleBackColor = true;
            btnSupprimer.Click += btnSupprimer_Click;
            //
            // lblMessage
            //
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(20, 245);
            lblMessage.MaximumSize = new Size(480, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 20);
            lblMessage.TabIndex = 6;
            //
            // FermetureHebdoGlobaleForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(524, 300);
            Controls.Add(lblMessage);
            Controls.Add(btnSupprimer);
            Controls.Add(btnEnregistrer);
            Controls.Add(grpJours);
            Controls.Add(btnCharger);
            Controls.Add(numAnnee);
            Controls.Add(lblAnnee);
            Name = "FermetureHebdoGlobaleForm";
            Text = "Fermeture hebdomadaire globale";
            ((System.ComponentModel.ISupportInitialize)numAnnee).EndInit();
            grpJours.ResumeLayout(false);
            grpJours.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

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
        private Button btnEnregistrer;
        private Button btnSupprimer;
        private Label lblMessage;
    }
}
