namespace PadelManager.WinForms
{
    partial class MembreMenuForm
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
            btnCreerMatch = new Button();
            btnCreerMatchPublic = new Button();
            btnMatchsPublics = new Button();
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
            // btnCreerMatch
            //
            btnCreerMatch.Location = new Point(20, 70);
            btnCreerMatch.Name = "btnCreerMatch";
            btnCreerMatch.Size = new Size(280, 34);
            btnCreerMatch.TabIndex = 1;
            btnCreerMatch.Text = "Créer un match privé";
            btnCreerMatch.UseVisualStyleBackColor = true;
            btnCreerMatch.Click += btnCreerMatch_Click;
            //
            // btnCreerMatchPublic
            //
            btnCreerMatchPublic.Location = new Point(20, 115);
            btnCreerMatchPublic.Name = "btnCreerMatchPublic";
            btnCreerMatchPublic.Size = new Size(280, 34);
            btnCreerMatchPublic.TabIndex = 2;
            btnCreerMatchPublic.Text = "Créer un match public";
            btnCreerMatchPublic.UseVisualStyleBackColor = true;
            btnCreerMatchPublic.Click += btnCreerMatchPublic_Click;
            //
            // btnMatchsPublics
            //
            btnMatchsPublics.Location = new Point(20, 160);
            btnMatchsPublics.Name = "btnMatchsPublics";
            btnMatchsPublics.Size = new Size(280, 34);
            btnMatchsPublics.TabIndex = 3;
            btnMatchsPublics.Text = "Matchs publics";
            btnMatchsPublics.UseVisualStyleBackColor = true;
            btnMatchsPublics.Click += btnMatchsPublics_Click;
            //
            // MembreMenuForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(340, 225);
            Controls.Add(btnMatchsPublics);
            Controls.Add(btnCreerMatchPublic);
            Controls.Add(btnCreerMatch);
            Controls.Add(lblConnecte);
            Name = "MembreMenuForm";
            Text = "PadelManager — Membre";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblConnecte;
        private Button btnCreerMatch;
        private Button btnCreerMatchPublic;
        private Button btnMatchsPublics;
    }
}
