namespace PadelManager.WinForms.Admin
{
    partial class AdminLoginForm
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
            lblMatricule = new Label();
            txtMatricule = new TextBox();
            btnConnexion = new Button();
            lblMessage = new Label();
            SuspendLayout();
            //
            // lblMatricule
            //
            lblMatricule.AutoSize = true;
            lblMatricule.Location = new Point(237, 206);
            lblMatricule.Name = "lblMatricule";
            lblMatricule.Size = new Size(71, 20);
            lblMatricule.TabIndex = 0;
            lblMatricule.Text = "Matricule";
            //
            // txtMatricule
            //
            txtMatricule.Location = new Point(326, 203);
            txtMatricule.Name = "txtMatricule";
            txtMatricule.Size = new Size(125, 27);
            txtMatricule.TabIndex = 1;
            //
            // btnConnexion
            //
            btnConnexion.Location = new Point(326, 255);
            btnConnexion.Name = "btnConnexion";
            btnConnexion.Size = new Size(110, 29);
            btnConnexion.TabIndex = 2;
            btnConnexion.Text = "Se connecter";
            btnConnexion.UseVisualStyleBackColor = true;
            btnConnexion.Click += btnConnexion_Click;
            //
            // lblMessage
            //
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(246, 87);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 20);
            lblMessage.TabIndex = 3;
            //
            // AdminLoginForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblMessage);
            Controls.Add(btnConnexion);
            Controls.Add(txtMatricule);
            Controls.Add(lblMatricule);
            Name = "AdminLoginForm";
            Text = "PadelManager — Administration";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblMatricule;
        private TextBox txtMatricule;
        private Button btnConnexion;
        private Label lblMessage;
    }
}
