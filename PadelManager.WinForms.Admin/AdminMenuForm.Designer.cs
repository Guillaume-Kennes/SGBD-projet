namespace PadelManager.WinForms.Admin
{
    partial class AdminMenuForm
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
            lblConnecte = new Label();
            btnHoraires = new Button();
            btnFermeturesPonctuelles = new Button();
            btnFermetureHebdoGlobale = new Button();
            SuspendLayout();
            //
            // lblConnecte
            //
            lblConnecte.AutoSize = true;
            lblConnecte.Location = new Point(20, 20);
            lblConnecte.Name = "lblConnecte";
            lblConnecte.Size = new Size(100, 20);
            lblConnecte.TabIndex = 0;
            //
            // btnHoraires
            //
            btnHoraires.Location = new Point(20, 70);
            btnHoraires.Name = "btnHoraires";
            btnHoraires.Size = new Size(280, 34);
            btnHoraires.TabIndex = 1;
            btnHoraires.Text = "Horaires d'ouverture";
            btnHoraires.UseVisualStyleBackColor = true;
            btnHoraires.Click += btnHoraires_Click;
            //
            // btnFermeturesPonctuelles
            //
            btnFermeturesPonctuelles.Location = new Point(20, 115);
            btnFermeturesPonctuelles.Name = "btnFermeturesPonctuelles";
            btnFermeturesPonctuelles.Size = new Size(280, 34);
            btnFermeturesPonctuelles.TabIndex = 2;
            btnFermeturesPonctuelles.Text = "Fermetures ponctuelles";
            btnFermeturesPonctuelles.UseVisualStyleBackColor = true;
            btnFermeturesPonctuelles.Click += btnFermeturesPonctuelles_Click;
            //
            // btnFermetureHebdoGlobale
            //
            btnFermetureHebdoGlobale.Location = new Point(20, 160);
            btnFermetureHebdoGlobale.Name = "btnFermetureHebdoGlobale";
            btnFermetureHebdoGlobale.Size = new Size(280, 34);
            btnFermetureHebdoGlobale.TabIndex = 3;
            btnFermetureHebdoGlobale.Text = "Fermeture hebdomadaire globale";
            btnFermetureHebdoGlobale.UseVisualStyleBackColor = true;
            btnFermetureHebdoGlobale.Click += btnFermetureHebdoGlobale_Click;
            //
            // AdminMenuForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(340, 230);
            Controls.Add(btnFermetureHebdoGlobale);
            Controls.Add(btnFermeturesPonctuelles);
            Controls.Add(btnHoraires);
            Controls.Add(lblConnecte);
            Name = "AdminMenuForm";
            Text = "PadelManager — Administration";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblConnecte;
        private Button btnHoraires;
        private Button btnFermeturesPonctuelles;
        private Button btnFermetureHebdoGlobale;
    }
}
