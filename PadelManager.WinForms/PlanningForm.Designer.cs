namespace PadelManager.WinForms
{
    partial class PlanningForm
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
            lblDu = new Label();
            dtpDu = new DateTimePicker();
            lblAu = new Label();
            dtpAu = new DateTimePicker();
            btnRechercher = new Button();
            grdPlanning = new DataGridView();
            colDate = new DataGridViewTextBoxColumn();
            colHeureDebut = new DataGridViewTextBoxColumn();
            colHeureFin = new DataGridViewTextBoxColumn();
            lblMessage = new Label();
            ((System.ComponentModel.ISupportInitialize)grdPlanning).BeginInit();
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
            // lblDu
            //
            lblDu.AutoSize = true;
            lblDu.Location = new Point(20, 63);
            lblDu.Name = "lblDu";
            lblDu.Size = new Size(24, 20);
            lblDu.TabIndex = 2;
            lblDu.Text = "Du";
            //
            // dtpDu
            //
            dtpDu.Format = DateTimePickerFormat.Short;
            dtpDu.Location = new Point(140, 60);
            dtpDu.Name = "dtpDu";
            dtpDu.Size = new Size(150, 27);
            dtpDu.TabIndex = 3;
            //
            // lblAu
            //
            lblAu.AutoSize = true;
            lblAu.Location = new Point(310, 63);
            lblAu.Name = "lblAu";
            lblAu.Size = new Size(22, 20);
            lblAu.TabIndex = 4;
            lblAu.Text = "Au";
            //
            // dtpAu
            //
            dtpAu.Format = DateTimePickerFormat.Short;
            dtpAu.Location = new Point(430, 60);
            dtpAu.Name = "dtpAu";
            dtpAu.Size = new Size(150, 27);
            dtpAu.TabIndex = 5;
            //
            // btnRechercher
            //
            btnRechercher.Location = new Point(140, 98);
            btnRechercher.Name = "btnRechercher";
            btnRechercher.Size = new Size(130, 29);
            btnRechercher.TabIndex = 6;
            btnRechercher.Text = "Rechercher";
            btnRechercher.UseVisualStyleBackColor = true;
            btnRechercher.Click += btnRechercher_Click;
            //
            // grdPlanning
            //
            grdPlanning.AllowUserToAddRows = false;
            grdPlanning.AllowUserToDeleteRows = false;
            grdPlanning.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grdPlanning.AutoGenerateColumns = false;
            grdPlanning.Columns.AddRange(new DataGridViewColumn[] { colDate, colHeureDebut, colHeureFin });
            grdPlanning.Location = new Point(20, 145);
            grdPlanning.Name = "grdPlanning";
            grdPlanning.ReadOnly = true;
            grdPlanning.RowHeadersWidth = 25;
            grdPlanning.Size = new Size(660, 300);
            grdPlanning.TabIndex = 7;
            //
            // colDate
            //
            colDate.DataPropertyName = "Date";
            colDate.HeaderText = "Date";
            colDate.Name = "colDate";
            colDate.ReadOnly = true;
            //
            // colHeureDebut
            //
            colHeureDebut.DataPropertyName = "HeureDebut";
            colHeureDebut.HeaderText = "Heure début";
            colHeureDebut.Name = "colHeureDebut";
            colHeureDebut.ReadOnly = true;
            //
            // colHeureFin
            //
            colHeureFin.DataPropertyName = "HeureFin";
            colHeureFin.HeaderText = "Heure fin";
            colHeureFin.Name = "colHeureFin";
            colHeureFin.ReadOnly = true;
            //
            // lblMessage
            //
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(20, 458);
            lblMessage.MaximumSize = new Size(660, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 20);
            lblMessage.TabIndex = 8;
            //
            // PlanningForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 490);
            Controls.Add(lblMessage);
            Controls.Add(grdPlanning);
            Controls.Add(btnRechercher);
            Controls.Add(dtpAu);
            Controls.Add(lblAu);
            Controls.Add(dtpDu);
            Controls.Add(lblDu);
            Controls.Add(cboSite);
            Controls.Add(lblSite);
            Name = "PlanningForm";
            Text = "Consultation du planning";
            Load += PlanningForm_Load;
            ((System.ComponentModel.ISupportInitialize)grdPlanning).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblSite;
        private ComboBox cboSite;
        private Label lblDu;
        private DateTimePicker dtpDu;
        private Label lblAu;
        private DateTimePicker dtpAu;
        private Button btnRechercher;
        private DataGridView grdPlanning;
        private DataGridViewTextBoxColumn colDate;
        private DataGridViewTextBoxColumn colHeureDebut;
        private DataGridViewTextBoxColumn colHeureFin;
        private Label lblMessage;
    }
}
